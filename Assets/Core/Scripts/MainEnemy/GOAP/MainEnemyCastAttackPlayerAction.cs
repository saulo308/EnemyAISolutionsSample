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

        void OnDestroy()
        {
            m_playerShieldUpSharedEvent.RemoveAllListeners();
        }

        // Public Methods -----------------------------------------
        public override bool IsActionUsable(AGoapAgent goapAgent)
        {
            // Gives a random chance to cast spell
            //      If player shield is up, then chance to cast action to be usable is 95% (randomChance > 0.05f)
            //      If player shield is not up, then chance to cast to be usable is 80% (randomChance > 0.2f)
            float randomChance = Random.Range(0f,1f);

            if(m_isPlayerShieldUp)
            {
                if(randomChance > 0.05f) return true;
                return false;
            }

            if(randomChance > 0.2f) return m_isPlayerShieldUp;
            return true;
        }

        public override bool RequiresRangeToExecute() => false;

        public override bool Perform()
        {
            base.Perform();

            m_mainEnemyController.AttackTarget(EEnemyAttackType.Cast);
            return true;
        }

        // Private Methods ------------------------------------------------
        void OnPlayerShieldToggle(bool bIsShieldUp)
        {
            m_isPlayerShieldUp = bIsShieldUp;
        }
    }
}
