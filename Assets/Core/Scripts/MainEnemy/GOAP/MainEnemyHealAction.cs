using System.Collections;
using System.Collections.Generic;
using CharacterModule;
using EnemyAIModule.GOAP;
using DG.Tweening;
using UnityEngine;

namespace AIProject.GameModule
{
    public class MainEnemyHealAction : AGoapAction
    {
        // Serializable Fields ---------------------------------
        [SerializeField] private MainEnemyCharacterController m_mainEnemyController = null;
        [SerializeField] private float m_healAbilityCooldown = 15.0f;

        // Non-Serializable Fields -----------------------------------
        private Tween m_healAbilityCooldownTween = null;
        private bool m_isHealAbilityOnCooldown = false;

        // Public Methods -----------------------------------------

        // If heal ability is on cooldown, then action is not usable
        public override bool IsActionUsable(AGoapAgent goapAgent) => !m_isHealAbilityOnCooldown;

        public override bool RequiresRangeToExecute() => false;
        public override bool IsInRangeToExecute() => true;

        public override bool Perform()
        {
            base.Perform();

            // Request main enemy heal
            m_mainEnemyController.RequestHealAbility();

            // Start cooldown
            m_isHealAbilityOnCooldown = true;
            m_healAbilityCooldownTween = DOVirtual.DelayedCall(m_healAbilityCooldown, () => 
            {
                m_isHealAbilityOnCooldown = false;
                m_healAbilityCooldownTween = null;
            });

            return true;
        }
    }
}
