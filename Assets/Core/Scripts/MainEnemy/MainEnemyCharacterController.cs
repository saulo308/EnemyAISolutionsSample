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
            if(m_mainEnemyCombatController.IsExecutingAbility) return;
            m_mainEnemyCombatController.RequestAttack(attackType);
        }

        public void RequestHealAbility()
        {
            if(m_mainEnemyCombatController.IsExecutingAbility) return;
            m_mainEnemyCombatController.RequestHeal();
        }

        public void RequestStartInvisibilityAbility()
        {
            if(m_mainEnemyCombatController.IsExecutingAbility) return;
            m_mainEnemyCombatController.RequestStartInvisibility();
        } 
        
        public void RequestEndInvisibilityAbility()
        {
            if(m_mainEnemyCombatController.IsExecutingAbility) return;
            m_mainEnemyCombatController.RequestEndInvisibility();
        }
    }
}
