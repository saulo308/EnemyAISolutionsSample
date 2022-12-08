using System.Collections;
using System.Collections.Generic;
using EnemyAIModule.GOAP;
using UnityEngine;

namespace AIProject.GameModule
{
    public class MainEnemyCastAttackPlayerAction : AGoapAction
    {
        // Serializable Fields ---------------------------------
        [SerializeField] private MainEnemyCharacterController m_mainEnemyController = null;

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
            return true;
        }
    }
}
