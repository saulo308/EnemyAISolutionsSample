using System.Collections;
using System.Collections.Generic;
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
        [Header("MainEnemy - LinkedReferences")]
        [SerializeField] private MainEnemyCharacterMovement m_mainEnemyCharacterMovement;

        [Header("MainEnemy - SharedEvents")]
        [SerializeField] private GameSharedDataEvent<string> m_enemyAttackDataEvent;

        // Public Methods --------------------------------------------
        public void RequestAttack(EEnemyAttackType attackType)
        {
            // Create animator trigger string by attack type
            string attackAnimatorTriggerStr = attackType switch
            {
                EEnemyAttackType.Melee => "AttackMelee",
                EEnemyAttackType.Cast => "AttackCast",
                _ => "None"
            };

            // Set shared data event
            m_enemyAttackDataEvent.SharedDataValue = attackAnimatorTriggerStr;

            // TODO: Execute attack

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

        // Evnet Handlers ---------------------------------------------------------------
        protected override void OnAnimationEventTrigerred(AnimationEvent triggeredAnimationEvent)
        {
            if(triggeredAnimationEvent.stringParameter.Equals("OnHurtEnd"))
            {
                // Re-enable player movement on hurt animation end
                m_mainEnemyCharacterMovement.EnableMovement();
            }

            // Call base
            base.OnAnimationEventTrigerred(triggeredAnimationEvent);
        }
    }
}
