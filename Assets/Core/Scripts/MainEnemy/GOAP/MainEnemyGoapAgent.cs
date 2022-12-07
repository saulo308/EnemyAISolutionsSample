using System.Collections;
using System.Collections.Generic;
using EnemyAIModule.GOAP;
using UnityEngine;

namespace AIProject.GameModule
{
    public class MainEnemyGoapAgent : AGoapAgent
    {
        // Serializable Fields ---------------------------------------------------------
        [SerializeField] private MainEnemyCharacterController m_mainEnemyCharacterController = null;

        protected override bool MoveAgentToExecuteAction(AGoapAction actionToExecute)
        {
            m_mainEnemyCharacterController.SetNewTarget(actionToExecute.ActionTarget);
            return m_mainEnemyCharacterController.IsDistanceToTargetLessThanLimit();
        }

        protected override void OnNoPlanFound()
        {
            m_mainEnemyCharacterController.SetNewTarget(null);
        }
    }
}
