using System.Collections;
using System.Collections.Generic;
using CharacterModule;
using EnemyAIModule.GOAP;
using GameSharedEventModule;
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
        private bool m_canCast = false;

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
            return m_canCast;
        }

        // Private Methods ------------------------------------------------
        void OnPlayerShieldToggle(bool bIsShieldUp)
        {
            m_canCast = bIsShieldUp;
        }
    }
}
