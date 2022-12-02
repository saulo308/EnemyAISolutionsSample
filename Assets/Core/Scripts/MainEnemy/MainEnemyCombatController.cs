using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using GameSharedEventModule;
using CharacterModule;
using UnityEngine;

namespace AIProject.GameModule
{
    // Enums ----------------------------------------------------------
    [System.Serializable]
    public enum EEnemyAttackType
    {
        Melee,
        Cast
    }

    public class MainEnemyCombatController : ACharacterCombatControllerBase
    {
        // Serializable Fields -------------------------------------------
        [Header("MainEnemy - LinkedReferences")]
        [SerializeField] private MainEnemyCharacterMovement m_mainEnemyCharacterMovement;

        [Header("MainEnemy - SharedEvents")]
        [SerializeField] private GameSharedDataEvent<string> m_enemyAttackDataEvent;

        [Header("MainEnemy - GeneralConfig")]
        [SerializeField] private float m_enemyDamage = 5f;
        [SerializeField] private float m_attackRaycastDistance = 2.0f;
        [SerializeField] private float m_attackRecoverTime = 0.5f;
        [SerializeField] private LayerMask m_playerLayerMask;

        // Non-Serializable Fields -------------------------------------------
        private bool m_isAttacking = false;
        private Tween m_attackRecoverTimeTween = null;

        // Properties ---------------------------------------------------------
        public bool IsAttacking => m_isAttacking;

        // Public Methods --------------------------------------------
        public void RequestAttack(EEnemyAttackType attackType)
        {
            // If enemy still in recover time, ignore attack (an attack has been requested before recover time)
            if(m_attackRecoverTimeTween.IsActive()) return;

            m_isAttacking = true;

            // Disable character movement
            m_mainEnemyCharacterMovement.DisableMovement(true);

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

        // Protected Methods ---------------------------------------------------------------------
        protected override void OnCharacterHurt()
        {
            // Disable enemy movement (When hurt, enemy should be 'stunned' during hurt animation)
            m_mainEnemyCharacterMovement.DisableMovement(true);

            // Call base
            base.OnCharacterHurt();
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
            Debug.DrawRay(raycastOrigin, raycastDirection * m_attackRaycastDistance, Color.red, 0.1f);

            // Re-enable character movement on recover time
            m_attackRecoverTimeTween = DOVirtual.DelayedCall(m_attackRecoverTime, () => 
            {
                m_mainEnemyCharacterMovement.EnableMovement();
                m_isAttacking = false;
            });

            // Check hit. If nothing found, return
            if(!outHit) return;
            
            // Else, we hit player. Make it take damage
            var hitEnemy = outHit.collider.GetComponentInChildren<MainPlayerCombatController>();
            hitEnemy.TakeDamage(m_enemyDamage);
        }

        void OnCastAttackAnimationCastTiming()
        {

        }

        // Event Handlers ---------------------------------------------------------------
        protected override void OnAnimationEventTrigerred(AnimationEvent triggeredAnimationEvent)
        {
            if(triggeredAnimationEvent.stringParameter.Equals("OnHurtEnd"))
            {
                // Re-enable player movement on hurt animation end
                m_mainEnemyCharacterMovement.EnableMovement();
            }
            
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

            // Call base
            base.OnAnimationEventTrigerred(triggeredAnimationEvent);
        }
    }
}
