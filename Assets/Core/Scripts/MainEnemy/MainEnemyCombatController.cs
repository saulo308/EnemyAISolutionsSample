using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using GameSharedEventModule;
using CharacterModule;
using EnemyAIModule.GOAP;
using UnityEngine;

namespace AIProject.GameModule
{
    public class MainEnemyCombatController : ACharacterCombatControllerBase
    {
        // Serializable Fields -------------------------------------------
        [Header("MainEnemy - LinkedReferences")]
        [SerializeField] private MainEnemyCharacterMovement m_mainEnemyCharacterMovement;
        [SerializeField] private SpriteRenderer m_mainEnemySpriteRenderer;

        [Header("MainEnemy - SharedEvents")]
        [SerializeField] private GameSharedDataEvent<string> m_enemyAttackDataEvent;
        [SerializeField] private GameSharedEvent m_enemyHealEvent;
        [SerializeField] private GameSharedEvent m_invisibilityStartAbilityEvent;
        [SerializeField] private GameSharedEvent m_invisibilityEndAbilityEvent;
        
        [Header("MainEnemy - GeneralConfig")]
        [SerializeField] private float m_abilityRecoverTime = 0.5f;

        [Header("MainEnemy - GeneralConfig - Melee")]
        [SerializeField] private float m_enemyDamage = 5f;
        [SerializeField] private float m_attackRaycastDistance = 2.0f;
        [SerializeField] private LayerMask m_playerLayerMask;

        [Header("MainEnemy - GeneralConfig - Spell")]
        [SerializeField] private MainEnemySpellController m_spellTemplate = null;
        [SerializeField] private float m_spellYSpawnOffset = 0.82f;
        [SerializeField] private float m_spellDamageAmount = 10f;

        [Header("MainEnemy - GeneralConfig - Heal")]
        [SerializeField] private float m_healAmount = 50f;
        [SerializeField] private ParticleSystem m_healParticleSystem = null;

        [Header("MainEnemy - GeneralConfig - Invisibility")]
        [SerializeField] private SpriteRenderer m_enemySpriteRenderer = null;
        [SerializeField] private BoxCollider2D m_enemyBoxCollider = null;

        // Non-Serializable Fields -------------------------------------------
        private GameObject m_curTargetRef = null;

        private bool m_isExecutingAbility = false;
        private Tween m_abilityRecoverTimeTween = null;

        // Properties ---------------------------------------------------------
        public bool IsExecutingAbility => m_isExecutingAbility;

        // Public Methods --------------------------------------------
        public override void TakeDamage(float damageAmount)
        {
            base.TakeDamage(damageAmount);

            // If main enemy reaches half health, add state to GOAP world state.
            // If not, remove it (if it exists)
            bool bIsMainEnemyHalfHealth = m_characterCurHealth <= (m_characterMaxHealth / 2f);
            if(bIsMainEnemyHalfHealth) GoapWorldManager.GoapWorldInstance.GetWorldStates().AddUniquePair("MainEnemyIsHalfHealth",0);
            else GoapWorldManager.GoapWorldInstance.GetWorldStates().Remove("MainEnemyIsHalfHealth");
        }

        public void RequestAttack(EEnemyAttackType attackType)
        {
            // If enemy still in recover time, ignore attack (an attack has been requested before recover time)
            if(m_abilityRecoverTimeTween.IsActive()) return;
            if(m_isCharacterDead) return;

            m_isExecutingAbility = true;

            // Disable character movement
            m_mainEnemyCharacterMovement.DisableMovement(true);

            // Rotate towards player ---
            // Get player and calculate direction from enemy to player pos
            var mainPlayer = MainGameInstance.GameInstance.MainPlayerController;
            Vector2 enemyToPlayerDirection = (transform.position - mainPlayer.transform.position).normalized;

            // If x-axis is positive, player is on enemy's right. Else, is on enemy's left
            if(enemyToPlayerDirection.x > 0) m_mainEnemyCharacterMovement.FlipCharacter(Vector2.right);
            else m_mainEnemyCharacterMovement.FlipCharacter(Vector2.left);
            // ----

            // Create animator trigger string by attack type
            string attackAnimatorTriggerStr = attackType switch
            {
                EEnemyAttackType.Melee => "AttackMelee",
                EEnemyAttackType.Cast => "AttackCast",
                _ => "None"
            };

            // Set shared data event that will trigger animator
            m_enemyAttackDataEvent.SharedDataValue = attackAnimatorTriggerStr;
        }

        public void RequestHeal()
        {
            // If enemy still in recover time, ignore ability use
            if(m_abilityRecoverTimeTween.IsActive()) return;
            if(m_isCharacterDead) return;

            m_isExecutingAbility = true;

            // Disable character movement
            m_mainEnemyCharacterMovement.DisableMovement(true);

            // Rotate towards player ---
            // Get player and calculate direction from enemy to player pos
            var mainPlayer = MainGameInstance.GameInstance.MainPlayerController;
            Vector2 enemyToPlayerDirection = (transform.position - mainPlayer.transform.position).normalized;

            // If x-axis is positive, player is on enemy's right. Else, is on enemy's left
            if(enemyToPlayerDirection.x > 0) m_mainEnemyCharacterMovement.FlipCharacter(Vector2.right);
            else m_mainEnemyCharacterMovement.FlipCharacter(Vector2.left);
            // ----

            // Dispatch event that will trigger animator
            m_enemyHealEvent.DispatchEvent();
        }

        public void RequestStartInvisibility()
        {
            // If enemy still in recover time, ignore ability use
            if(m_abilityRecoverTimeTween.IsActive()) return;
            if(m_isCharacterDead) return;

            m_isExecutingAbility = true;

            // Disable character movement
            m_mainEnemyCharacterMovement.DisableMovement(true);

            // Dispatch event that will trigger animator
            m_invisibilityStartAbilityEvent.DispatchEvent();
        }

        public void RequestEndInvisibility()
        {
            // If enemy still in recover time, ignore ability use
            if(m_abilityRecoverTimeTween.IsActive()) return;
            if(m_isCharacterDead) return;

            m_isExecutingAbility = true;

            // Disable character movement
            m_mainEnemyCharacterMovement.DisableMovement(true);

            // Enable sprite renderer
            m_enemySpriteRenderer.enabled = true;

            // Dispatch event that will trigger animator
            m_invisibilityEndAbilityEvent.DispatchEvent();
        }

        public void SetupData(GameObject targetRef)
        {
            m_curTargetRef = targetRef;
        }

        // Protected Methods ---------------------------------------------------------------------
        protected override void OnCharacterHurt()
        {
            // Blink character on hurt
            Sequence blinkSequence = DOTween.Sequence();
            blinkSequence.Append(m_mainEnemySpriteRenderer.DOColor(Color.red,0.05f));
            blinkSequence.Append(m_mainEnemySpriteRenderer.DOColor(Color.white,0.05f));
            blinkSequence.Play().SetLoops(2);
        }

        protected override void OnCharacterDead()
        {
            // Disable enemy movement (When hurt, enemy should be 'stunned' during hurt animation)
            m_mainEnemyCharacterMovement.DisableMovement(true);

            // Call base
            base.OnCharacterDead();
        }

        // Private Methods --------------------------------------------------------------------------
        void OnMeleeAttackAnimationConnected()
        {
            // On attackMelee animation play, it will dispatch a 'animationEvent' so we can execute raycast and damage enemy
            // respecting attack animation frame timing
            //(e.g. "attack" takes 5 frames to actually appear to hit something, frames 0 to 4 are just anticipating attack)

            //Execute a raycast from enemy to enemy left direction (facing direction) to check if we hit something
            Vector2 raycastOrigin = transform.position;
            Vector2 raycastDirection = -transform.right; // == transform.left
            var outHit = Physics2D.Raycast(raycastOrigin, raycastDirection, m_attackRaycastDistance, m_playerLayerMask);
            // Debug.DrawRay(raycastOrigin, raycastDirection * m_attackRaycastDistance, Color.red, 0.1f);

            // Re-enable character movement on recover time
            m_abilityRecoverTimeTween = DOVirtual.DelayedCall(m_abilityRecoverTime, () => 
            {
                m_mainEnemyCharacterMovement.EnableMovement();
                m_isExecutingAbility = false;
            });

            // Check hit. If nothing found, return
            if(!outHit) return;
            
            // Else, we hit player. Make it take damage
            var hitEnemy = outHit.collider.GetComponentInChildren<MainPlayerCombatController>();
            hitEnemy.TakeDamage(m_enemyDamage,EEnemyAttackType.Melee);
        }

        void OnCastAttackAnimationCastTiming()
        {
            if(!m_spellTemplate)
            {
                Debug.LogError("No spell template!");
                m_mainEnemyCharacterMovement.EnableMovement();
                return;
            }

            // On attackCast animation play, it will dispatch a 'animationEvent' so we can execute spawn cast attack on player
            // However, it must spawn only when enemy has actually "casted" spell
            //(e.g. "attackCast" takes 6 frames to actually appear to hit something, frames 0 to 5 are just anticipating attack)

            // Get target position
            Vector3 targetPos = m_curTargetRef.transform.position;

            // Calculate spell spawn pos (targetPos + yOffset)
            Vector3 spellSpawnPos = targetPos + new Vector3(0,m_spellYSpawnOffset,0);

            // Spawn spell on position and parent the same parent as this
            var spawnedSpell = Instantiate(m_spellTemplate,spellSpawnPos,Quaternion.identity,m_characterContainer.transform.parent);
            spawnedSpell.SetupSpellData(m_spellDamageAmount);

            // Re-enable character movement on (recover time / 2f)
            m_abilityRecoverTimeTween = DOVirtual.DelayedCall(m_abilityRecoverTime / 2f, () => 
            {
                m_mainEnemyCharacterMovement.EnableMovement();
                m_isExecutingAbility = false;
            });
        }

        void OnHealAnimationExecuted()
        {
            // Spawn particles 
            if(m_healParticleSystem) m_healParticleSystem.Play();

            // Request heal
            HealCharacter(m_healAmount);

            // Check if life is greater than half health. If it's, remove world state
            bool bIsMainEnemyHalfHealth = m_characterCurHealth <= (m_characterMaxHealth / 2f);
            if(!bIsMainEnemyHalfHealth)
                GoapWorldManager.GoapWorldInstance.GetWorldStates().Remove("MainEnemyIsHalfHealth");
            
            // Re-enable character movement on recover time
            m_abilityRecoverTimeTween = DOVirtual.DelayedCall(m_abilityRecoverTime, () => 
            {
                m_mainEnemyCharacterMovement.EnableMovement();
                m_isExecutingAbility = false;
            });
        }

        void OnStartInvisibility()
        {
            // Deactivate spriteRenderer and collider
            m_enemySpriteRenderer.enabled = false;
            m_enemyBoxCollider.enabled = false;

            // Re-enable character movement
            m_mainEnemyCharacterMovement.EnableMovement();
            m_isExecutingAbility = false;
        }

        void OnEndInvisibility()
        {
            // Activate collider
            m_enemyBoxCollider.enabled = true;

            m_mainEnemyCharacterMovement.EnableMovement();
            m_isExecutingAbility = false;
        }

        // Event Handlers ---------------------------------------------------------------
        protected override void OnAnimationEventTrigerred(AnimationEvent triggeredAnimationEvent)
        {
            // On attackMelee animation play, it will dispatch a 'animationEvent' so we can execute raycast and damage enemy
            // respecting attack animation frame timing
            //(e.g. "attack" takes 5 frames to actually appear to hit something, frames 0 to 4 are just anticipating attack)
            if(triggeredAnimationEvent.stringParameter.Equals("OnAttackMeleeConnected"))
            {
                OnMeleeAttackAnimationConnected();
            }

            // On attackCast animation play, it will dispatch a 'animationEvent' so we can execute spawn cast attack on player
            // However, it must spawn only when enemy has actually "casted" spell
            //(e.g. "attackCast" takes 6 frames to actually appear to hit something, frames 0 to 5 are just anticipating attack)
            if(triggeredAnimationEvent.stringParameter.Equals("OnAttackCastTiming"))
            {
                OnCastAttackAnimationCastTiming();
            }

            // On heal animation correct frame time, spawn heal particles and heal enemy
            if(triggeredAnimationEvent.stringParameter.Equals("OnHeal"))
            {
                OnHealAnimationExecuted();
            }

            // On invisible animation end, make enemy 'inivisible' (disable image and colliders)
            if(triggeredAnimationEvent.stringParameter.Equals("OnInvisibleStart"))
            {
                OnStartInvisibility();
            }
            
            // On invisible animation end, make enemy shown (disable image and colliders)
            if(triggeredAnimationEvent.stringParameter.Equals("OnInvisibleReverse"))
            {
                OnEndInvisibility();
            }

            // Call base
            base.OnAnimationEventTrigerred(triggeredAnimationEvent);
        }
    }
}
