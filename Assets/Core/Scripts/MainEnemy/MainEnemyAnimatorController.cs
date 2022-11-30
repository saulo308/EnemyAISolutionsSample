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

        [SerializeField] private GameSharedEvent m_enemyHurtEvent;
        [SerializeField] private GameSharedEvent m_enemyEndLifeEvent;

        // Unity Methods -----------------------------------------
        protected override void Awake()
        {
            base.Awake();

            // Binding events on sharedEvents
            m_enemyVelocityDataEvent.AddListener(OnEnemyVelocityUpdate);
            m_enemyAttackDataEvent.AddListener(OnEnemyAttack);
            m_enemyHurtEvent.AddListener(OnEnemyHurt);
            m_enemyEndLifeEvent.AddListener(OnEnemyEndLife);
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

        void OnEnemyHurt()
        {
            SetAnimatorTrigger("Hurt");
        }

        void OnEnemyEndLife()
        {
            SetAnimatorTrigger("Dead");
        }
    }
}