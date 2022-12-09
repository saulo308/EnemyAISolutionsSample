using System.Collections;
using System.Collections.Generic;
using CharacterModule;
using GameSharedEventModule;
using UnityEngine;

namespace AIProject.GameModule
{
    public class MainEnemyAnimatorController : ACharacterAnimatorControllerBase
    {
        // Serializable Fields ------------------------------------
        [Header("SharedEvents")]
        [SerializeField] private GameSharedDataEvent<float> m_enemyVelocityDataEvent;
        [SerializeField] private GameSharedDataEvent<string> m_enemyAttackDataEvent;

        [SerializeField] private GameSharedEvent m_enemyEndLifeEvent;
        [SerializeField] private GameSharedEvent m_enemyHealEvent;

        // Unity Methods -----------------------------------------
        protected override void Awake()
        {
            base.Awake();

            // Binding events on sharedEvents
            m_enemyVelocityDataEvent.AddListener(OnEnemyVelocityUpdate);
            m_enemyAttackDataEvent.AddListener(OnEnemyAttack);
            m_enemyEndLifeEvent.AddListener(OnEnemyEndLife);
            m_enemyHealEvent.AddListener(OnEnemyHeal);
        }

        // Private Methods ----------------------------------------------
        void OnEnemyVelocityUpdate(float enemyVelocityMagnitude)
        {
            // If enemy velocity magnitude is greater than 0, player is moving. 
            bool bIsRunning = enemyVelocityMagnitude > 0;
            SetAnimatorBool("IsRunning",bIsRunning);
        }

        void OnEnemyAttack(string attackTriggerName)
        {
            // Set trigger on animator
            SetAnimatorTrigger(attackTriggerName);
        }

        void OnEnemyEndLife()
        {
            SetAnimatorTrigger("Dead");
        }
        
        void OnEnemyHeal()
        {
            SetAnimatorTrigger("Heal");
        }
    }
}