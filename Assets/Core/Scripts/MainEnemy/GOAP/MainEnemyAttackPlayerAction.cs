using System.Collections;
using System.Collections.Generic;
using EnemyAIModule.GOAP;
using UnityEngine;

namespace AIProject.GameModule
{
    public class MainEnemyAttackPlayerAction : AGoapAction
    {
        // Serializable Fields ---------------------------------
        [SerializeField] private MainEnemyCharacterController m_mainEnemyController = null;

        // Public Methods -----------------------------------------
        public override bool RequiresRangeToExecute()
        {
            return true;
        }

        public override bool Perform()
        {
            m_mainEnemyController.AttackTarget_Melee();
            return true;
        }
        
        public override bool IsInRangeToExecute()
        {
            return m_mainEnemyController.IsDistanceToTargetLessThanLimit();
        }
    }
}
