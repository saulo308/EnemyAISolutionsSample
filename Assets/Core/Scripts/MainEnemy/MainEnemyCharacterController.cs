using System.Collections;
using System.Collections.Generic;
using CharacterModule;
using CharacterModule.TopDown2D;
using UnityEngine;

namespace AIProject.GameModule
{
    public class MainEnemyCharacterController : AAIControllerTopDown2DBase
    {
        // Serializable Fields -------------------------------------------
        [Header("MainEnemy - LinkedReferences")]
        [SerializeField] private MainEnemyCombatController m_mainEnemyCombatController = null;

        // Unity Methods ----------------------------------------------
        protected override void Awake()
        {
            base.Awake();
            m_mainEnemyCombatController.SetupData(m_targetReference);
        }

        // Public Methods --------------------------------------------
        public void AttackTarget(EEnemyAttackType attackType)
        {
            if(m_mainEnemyCombatController.IsAttacking) return;
            m_mainEnemyCombatController.RequestAttack(attackType);
        }

        public void AttackTarget_Melee()
        {
            if(m_mainEnemyCombatController.IsAttacking) return;
            m_mainEnemyCombatController.RequestAttack(EEnemyAttackType.Melee);
        }

        public void AttackTarget_Cast()
        {
            if(m_mainEnemyCombatController.IsAttacking) return;
            m_mainEnemyCombatController.RequestAttack(EEnemyAttackType.Cast);
        }
    }
}
