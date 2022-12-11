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
        [SerializeField] private GameSharedEvent m_enemyStartInvisibilityEvent;
        [SerializeField] private GameSharedEvent m_enemyEndInvisibilityEvent;

        // Unity Methods -----------------------------------------
        protected override void Awake()
        {
            base.Awake();

            // Binding events on sharedEvents
            m_enemyVelocityDataEvent.AddListener(OnEnemyVelocityUpdate);
            m_enemyAttackDataEvent.AddListener(OnEnemyAttack);
            m_enemyEndLifeEvent.AddListener(OnEnemyEndLife);
            m_enemyHealEvent.AddListener(OnEnemyHeal);
            m_enemyStartInvisibilityEvent.AddListener(OnEnemyStartInvisibility);
            m_enemyEndInvisibilityEvent.AddListener(OnEnemyEndInvisibility);
        }

        void OnDestroy()
        {
            m_enemyVelocityDataEvent.RemoveAllListeners();
            m_enemyAttackDataEvent.RemoveAllListeners();
            m_enemyEndLifeEvent.RemoveAllListeners();
            m_enemyHealEvent.RemoveAllListeners();
            m_enemyStartInvisibilityEvent.RemoveAllListeners();
            m_enemyEndInvisibilityEvent.RemoveAllListeners();
        }

        // Private Methods ----------------------------------------------
        void OnEnemyVelocityUpdate(float enemyVelocityMagnitude)
        {
            // If enemy velocity magnitude is greater than 0, player is moving. 
            bool bIsRunning = enemyVelocityMagnitude > 0;
            SetAnimatorBool("IsRunning",bIsRunning);
        }

        // Set triggers on animator
        void OnEnemyAttack(string attackTriggerName) => SetAnimatorTrigger(attackTriggerName);
        void OnEnemyEndLife() => SetAnimatorTrigger("Dead");
        void OnEnemyHeal() => SetAnimatorTrigger("Heal");
        void OnEnemyStartInvisibility() => SetAnimatorTrigger("InvisibleStart");
        void OnEnemyEndInvisibility() => SetAnimatorTrigger("InvisibleEnd");
    }
}