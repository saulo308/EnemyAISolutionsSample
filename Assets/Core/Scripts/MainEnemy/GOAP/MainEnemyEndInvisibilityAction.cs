using System.Collections;
using System.Collections.Generic;
using CharacterModule;
using EnemyAIModule.GOAP;
using UnityEngine;

namespace AIProject.GameModule
{
    public class MainEnemyEndInvisibilityAction : AGoapAction
    {
        // Serializable Fields ---------------------------------
        [SerializeField] private MainEnemyCharacterController m_mainEnemyController = null;

        // Public Methods -----------------------------------------
        public override bool IsActionUsable(AGoapAgent goapAgent) => true;

        public override bool RequiresRangeToExecute() => true;
        public override bool IsInRangeToExecute() => m_mainEnemyController.IsDistanceToTargetLessThanLimit();

        public override bool Perform()
        {
            base.Perform();
            m_mainEnemyController.RequestEndInvisibilityAbility();
            return true;
        }
    }
}
