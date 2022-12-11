using System.Collections;
using System.Collections.Generic;
using GameSharedEventModule;
using CharacterModule;
using DG.Tweening;
using EnemyAIModule.GOAP;
using UnityEngine;

namespace AIProject.GameModule
{
    public class MainPlayerCombatController : ACharacterCombatControllerBase
    {
        // Serializable Fields ---------------------------------------------
        [Header("MainPlayer - LinkedReferences")]
        [SerializeField] private MainPlayerCharacterMovement m_mainPlayerCharacterMovement;

        [Header("MainPlayer - SharedEvents")]
        [SerializeField] private GameSharedDataEvent<string> m_playerAttackInputDataEvent;
        [SerializeField] private GameSharedDataEvent<bool> m_playerShieldInputDataEvent;

        [Header("MainPlayer - GeneralConfig - Attack")]
        [SerializeField] private float m_playerBaseDamage = 5f;
        [SerializeField] private float m_attackRaycastDistance = 2.0f;

        [SerializeField] private int m_maxNumberOfAttacksCombo = 3;
        [SerializeField] private float m_attackDelay = 0.4f;
        [SerializeField] private float m_resetComboDelay = 1f;

        [Header("MainPlayer - GeneralConfig - Shield")]
        [SerializeField] private Transform m_shieldFxSpawnTransform = null;
        [SerializeField] private MainPlayerShieldHitEffectController m_shieldEffectTemplate = null;

        // Non-Serializable Fields -----------------------------------------

        // Attack flags
        private int m_curAttackComboIndex = 1;
        private float m_timeSinceLastAttack = 0f;
        private Tween m_resetComboTween = null;
        private float m_playerCurDamage = 0f;

        private bool m_isShieldUp = false;

        private LayerMask m_playerLayer;
        private LayerMask m_mainEnemyLayer;

        // Properties --------------------------------------------------------

        // Unity Methods ----------------------------------------------------
        protected override void Awake()
        {
            base.Awake();

            // Setup cur damage
            m_playerCurDamage = m_playerBaseDamage;

            // Add 'PlayerAlive' state on GOAP world state (pre-condition to most enemy actions)
            GoapWorldManager.GoapWorldInstance.GetWorldStates().AddUniquePair("PlayerAlive",0);
        }

        // Public Methods -----------------------------------------------------
        public void OnShieldUp()
        {
            // Dispatch sharedEvent so animator can listen
            m_playerShieldInputDataEvent.SharedDataValue = true;
            m_isShieldUp = true;
        }

        public void OnShieldDown()
        {
            // Dispatch sharedEvent so animator can listen
            m_playerShieldInputDataEvent.SharedDataValue = false;
            m_isShieldUp = false;
        }

        public void OnAttackPressed()
        {
            // Get current time and check if it's smaller than attackDelay. If it's then it's too soon to attack
            if(!CanPlayerAttack()) return;

            // Play attack animation
            PlayAttackAnimation();

            // Get time as timeSinceLastAttack so we can check for attackDelay against future attack time
            m_timeSinceLastAttack = Time.time;
        }
        
        public void SetupData(LayerMask playerLayerMask, LayerMask enemyLayerMask)
        {
            m_playerLayer = playerLayerMask;
            m_mainEnemyLayer = enemyLayerMask;
        }

        public override void TakeDamage(float damageAmount, EEnemyAttackType attackType)
        {
            // If player is rolling, he can not be damaged (dodging attacks)
            if(m_mainPlayerCharacterMovement.IsPlayerRolling) return;

            // If character hit by a melee attack and shield is up, neglect damage and spawn particle effect
            if(attackType == EEnemyAttackType.Melee && m_isShieldUp) 
            {
                var shieldHitEffect = Instantiate(m_shieldEffectTemplate, m_shieldFxSpawnTransform.parent);
                shieldHitEffect.transform.localPosition = Vector3.zero;
                return;
            }

            base.TakeDamage(damageAmount);
        }

        // Protected Methods ---------------------------------------------------------------------
        protected override void OnCharacterHurt()
        {
            // Disable player movement (When hurt, player should be 'stunned' during hurt animation)
            m_mainPlayerCharacterMovement.DisableMovement(true);

            // Call base
            base.OnCharacterHurt();
        }

        protected override void OnCharacterDead()
        {
            // Disable player movement (When hurt, player should be 'stunned' during hurt animation)
            m_mainPlayerCharacterMovement.DisableMovement(true);

            // Remove 'PlayerAlive' state from GOAP world state
            GoapWorldManager.GoapWorldInstance.GetWorldStates().Remove("PlayerAlive");

            // Call base
            base.OnCharacterDead();
        }

        // Private Methods ---------------------------------------------------------------
        bool CanPlayerAttack()
        {
            // Get current time and check if it's smaller than attackDelay. If it's then it's too soon to attack
            float curTime = Time.time;
            if(curTime - m_timeSinceLastAttack < m_attackDelay) return false;
            return true;
        }

        void PlayAttackAnimation()
        {
            // Check if reset coroutine (delay) is active. If it's, then kill it (so we can refresh combo reset delay)
            if(m_resetComboTween.IsActive()) m_resetComboTween.Kill();

            // Create string "Attack[1,2,3,..]" to send to animator as trigger
            string attackComboAnimatorTriggerName = string.Format("Attack{0}",m_curAttackComboIndex);
            m_playerAttackInputDataEvent.SharedDataValue = attackComboAnimatorTriggerName;
            
            // If started combo, calculate new damage by multiplying it by a randomMultiplier (30% to 50% more damage)
            // Else, reset curDamage
            if(m_curAttackComboIndex > 1)
            {
                float damageMultiplier = Random.Range(1.3f,1.5f);
                m_playerCurDamage *= damageMultiplier;
            }
            else
            {
                m_playerCurDamage = m_playerBaseDamage;
            }

            // Increase attack combo index.
            // If it's already max, then reset to 1
            if(m_curAttackComboIndex == m_maxNumberOfAttacksCombo) m_curAttackComboIndex = 1;
            else m_curAttackComboIndex++;

            // Also, start a delay to rest comboIndex if next combo does not come within treshold
            m_resetComboTween = DOVirtual.DelayedCall(m_resetComboDelay,() => m_curAttackComboIndex = 1);
        }

        void OnAttackAnimationConnected()
        {
            // On attack animation play, it will dispatch a 'animationEvent' so we can execute raycast and damage enemy
            // respecting attack animation frame time 
            //(e.g. "attack01" takes 2 frames to actually hit something, frames 0 and 1 are just anticipating attack)

            //Execute a raycast from player to player right direction (facing direction) to check if we hit something

            // @note Little 'gambiarra' here. The game is top down, and player may not be in the same 'y' as enemy, making
            // a single raycast not hit anyone. 
            // Workarround: Execute 3 raycasts with different 'y' offset (one o player head, one on player chest and 
            // another one on player's feet)
            // To see this, enable 'Debug' line
            Vector2 raycastOrigin = transform.position + new Vector3(0,2,0);
            for(int i = 0;i < 3; i ++)
            {
                // For each raycast, subtract given offset
                raycastOrigin -= new Vector2(0,1);
                Vector2 raycastDirection = transform.right; 
                var outHit = Physics2D.Raycast(raycastOrigin, raycastDirection, m_attackRaycastDistance, m_mainEnemyLayer);
                
                // Enable debug to see how we use 3 raycasts here
                // Debug.DrawRay(raycastOrigin, raycastDirection * m_attackRaycastDistance, Color.red, 0.1f);

                // Check hit. If nothing found, return
                if(!outHit) continue;
                
                // Else, we hit enemy. Make it take damage
                var hitEnemy = outHit.collider.GetComponentInChildren<MainEnemyCombatController>();
                hitEnemy.TakeDamage(m_playerCurDamage);
                break;
            }
        }

        // Event Handlers ---------------------------------------------------------------
        protected override void OnAnimationEventTrigerred(AnimationEvent triggeredAnimationEvent)
        {
            if(triggeredAnimationEvent.stringParameter.Equals("OnHurtEnd"))
            {
                // Re-enable player movement on hurt animation end
                m_mainPlayerCharacterMovement.EnableMovement();
                
                // Trigger check for cached inputs
                PlayerInputCacheController.Instance.CheckForCachedInputs();
            }

            // On attack animation play, it will dispatch a 'animationEvent' so we can execute raycast and damage enemy
            // respecting attack animation frame timing
            //(e.g. "attack01" takes 2 frames to actually appear to hit something, frames 0 and 1 are just anticipating attack)
            if(triggeredAnimationEvent.stringParameter.Equals("OnAttackConnected"))
            {
                OnAttackAnimationConnected();
            }

            // Call base
            base.OnAnimationEventTrigerred(triggeredAnimationEvent);
        }
    }
}