using System.Collections;
using System.Collections.Generic;
using CharacterModule.TopDown2D;
using UnityEngine;

namespace AIProject.GameModule
{
    public class MainEnemyCharacterController : AAIControllerTopDown2DBase
    {
        // Serializable Fields -------------------------------------------
        [Header("MainEnemy - LinkedReferences")]
        [SerializeField] private MainEnemyCombatController m_mainEnemyCombatController = null;

        // Public Methods --------------------------------------------
        public void AttackTarget(EEnemyAttackType attackType)
        {
            m_mainEnemyCombatController.RequestAttack(attackType);
        }

        public void AttackTarget_Melee()
        {
            m_mainEnemyCombatController.RequestAttack(EEnemyAttackType.Melee);
        }

        public void AttackTarget_Cast()
        {
            m_mainEnemyCombatController.RequestAttack(EEnemyAttackType.Cast);
        }
    }
}
