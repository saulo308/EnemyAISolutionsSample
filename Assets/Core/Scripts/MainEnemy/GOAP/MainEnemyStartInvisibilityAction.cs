using System.Collections;
using System.Collections.Generic;
using CharacterModule;
using DG.Tweening;
using EnemyAIModule.GOAP;
using UnityEngine;

namespace AIProject.GameModule
{
    public class MainEnemyStartInvisibilityAction : AGoapAction
    {
        // Serializable Fields ---------------------------------
        [SerializeField] private MainEnemyCharacterController m_mainEnemyController = null;
        [SerializeField] private float m_invisibilityAbilityCooldown = 15.0f;

        // Non-Serializable Fields -----------------------------------
        private Tween m_invisibilityAbilityCooldownTween = null;
        private bool m_isInvisibilityAbilityOnCooldown = false;

        // Public Methods -----------------------------------------
        public override bool RequiresRangeToExecute()
        {
            return false;
        }

        public override bool Perform()
        {
            base.Perform();

            m_mainEnemyController.RequestStartInvisibilityAbility();

            // Start cooldown
            m_isInvisibilityAbilityOnCooldown = true;
            m_invisibilityAbilityCooldownTween = DOVirtual.DelayedCall(m_invisibilityAbilityCooldown, () => 
            {
                m_isInvisibilityAbilityOnCooldown = false;
                m_invisibilityAbilityCooldownTween = null;
            });

            return true;
        }
        
        public override bool IsInRangeToExecute()
        {
            return true;
        }

        public override bool IsActionUsable(AGoapAgent goapAgent)
        {
            // If invisibility ability is on cooldown, then action is not usable
            return !m_isInvisibilityAbilityOnCooldown;
        }
    }
}
