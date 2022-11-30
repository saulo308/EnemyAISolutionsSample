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

        [SerializeField] private GameSharedEvent m_enemyHurtEvent;
        [SerializeField] private GameSharedEvent m_enemyDeadEvent;

        // Unity Methods -----------------------------------------
        protected override void Awake()
        {
            base.Awake();

            // Binding events on sharedEvents
            m_enemyVelocityDataEvent.AddListener(OnEnemyVelocityUpdate);
            m_enemyHurtEvent.AddListener(OnEnemyHurt);
            m_enemyDeadEvent.AddListener(OnEnemyDead);
        }

        // Private Methods ----------------------------------------------
        void OnEnemyVelocityUpdate(float enemyVelocityMagnitude)
        {
            // If enemy velocity magnitude is greater than 0, player is moving. 
            bool bIsRunning = enemyVelocityMagnitude > 0;
            SetAnimatorBool("IsRunning",bIsRunning);
        }

        void OnEnemyHurt()
        {
            SetAnimatorTrigger("Hurt");
        }

        void OnEnemyDead()
        {
            SetAnimatorTrigger("Dead");
        }
    }
}