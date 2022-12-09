using System.Collections;
using System.Collections.Generic;
using CharacterModule;
using EnemyAIModule.GOAP;
using GameSharedEventModule;
using DG.Tweening;
using UnityEngine;

namespace AIProject.GameModule
{
    public class MainEnemyCastAttackPlayerAction : AGoapAction
    {
        // Serializable Fields ---------------------------------
        [Header("CastAction - LinkedReferences")]
        [SerializeField] private MainEnemyCharacterController m_mainEnemyController = null;

        [Header("CastAction - SharedReferences")]
        [SerializeField] private GameSharedDataEvent<bool> m_playerShieldUpSharedEvent = null;

        // Non-Serializable Fields ----------------------------------
        private bool m_isPlayerShieldUp = false;

        // Unity Methods -------------------------------------------
        protected override void Awake()
        {
            base.Awake();
            m_playerShieldUpSharedEvent.AddListener(OnPlayerShieldToggle);
        }

        // Public Methods -----------------------------------------
        public override bool RequiresRangeToExecute()
        {
            return false;
        }

        public override bool Perform()
        {
            m_mainEnemyController.AttackTarget(EEnemyAttackType.Cast);
            return true;
        }

        public override bool IsActionUsable(AGoapAgent goapAgent)
        {
            // Gives a random chance to: 
            // Either choose that action is usable immediately or only if player shield is up
            // 50% (0.5f) will only mark this action as usable if player shield is up and the other 50%
            // will mark this action as usable regardless
            float randomChance = Random.Range(0f,1f);

            if(randomChance > 0.5f) return m_isPlayerShieldUp;
            return true;
        }

        // Private Methods ------------------------------------------------
        void OnPlayerShieldToggle(bool bIsShieldUp)
        {
            m_isPlayerShieldUp = bIsShieldUp;
        }
    }
}
