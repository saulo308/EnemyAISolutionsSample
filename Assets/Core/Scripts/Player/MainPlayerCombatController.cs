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

        [Header("MainPlayer - GeneralConfig")]
        [SerializeField] private float m_playerDamage = 5f;
        [SerializeField] private float m_attackRaycastDistance = 2.0f;

        [SerializeField] private int m_maxNumberOfAttacksCombo = 3;
        [SerializeField] private float m_attackDelay = 0.4f;
        [SerializeField] private float m_resetComboDelay = 1f;

        // Non-Serializable Fields -----------------------------------------

        // Attack flags
        private int m_curAttackComboIndex = 1;
        private float m_timeSinceLastAttack = 0f;
        private Tween m_resetComboTween = null;

        private LayerMask m_playerLayer;
        private LayerMask m_mainEnemyLayer;

        // Unity Methods ----------------------------------------------------
        protected override void Awake()
        {
            base.Awake();

            // Add 'PlayerAlive' state on GOAP world state (pre-condition to most enemy actions)
            GoapWorldManager.GoapWorldInstance.GetWorldStates().AddUniquePair("PlayerAlive",0);
        }

        // Public Methods -----------------------------------------------------
        public void OnShieldUp()
        {
            // Dispatch sharedEvent so animator can listen
            m_playerShieldInputDataEvent.SharedDataValue = true;
        }

        public void OnShieldDown()
        {
            // Dispatch sharedEvent so animator can listen
            m_playerShieldInputDataEvent.SharedDataValue = false;
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

        // Protected Methods ---------------------------------------------------------------------
        protected override void OnCharacterHurt()
        {
            // If player is rolling, he can not be damaged (dodging attacks)
            if(m_mainPlayerCharacterMovement.IsPlayerRolling) return;

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
            Vector2 raycastOrigin = transform.position;
            Vector2 raycastDirection = transform.right;
            var outHit = Physics2D.Raycast(raycastOrigin, raycastDirection, m_attackRaycastDistance, m_mainEnemyLayer);
            // Debug.DrawRay(raycastOrigin, raycastDirection * m_attackRaycastDistance, Color.red, 0.1f);

            // Check hit. If nothing found, return
            if(!outHit) return;
            
            // Else, we hit enemy. Make it take damage
            var hitEnemy = outHit.collider.GetComponentInChildren<MainEnemyCombatController>();
            hitEnemy.TakeDamage(m_playerDamage);
        }

        // Event Handlers ---------------------------------------------------------------
        protected override void OnAnimationEventTrigerred(AnimationEvent triggeredAnimationEvent)
        {
            if(triggeredAnimationEvent.stringParameter.Equals("OnHurtEnd"))
            {
                // Re-enable player movement on hurt animation end
                m_mainPlayerCharacterMovement.EnableMovement();
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