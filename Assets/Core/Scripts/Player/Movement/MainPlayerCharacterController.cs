using System.Collections;
using System.Collections.Generic;
using CharacterModule.TopDown2D;
using GameSharedEventModule;
using DG.Tweening;
using UnityEngine;

namespace AIProject.GameModule
{
    public class MainPlayerCharacterController : APlayerControllerTopDown2DBase
    {
        // Serializable Fields -------------------------------------------
        [Header("Input")]
        [SerializeField] private GameSharedDataEvent<bool> m_playerShieldInputDataEvent;
        [SerializeField] private GameSharedEvent m_playerRollInputEvent;

        // Non-Serializable Fields -----------------------------------------
        private MainPlayerCharacterMovement m_mainPlayerCharacterMovement;

        // Unity Methods ---------------------------------------------------
        protected override void Awake()
        {
            base.Awake();

            // Cast CharacterMovementBase to MainPlayerCharacter so we can use specific functions (such as roll)
            m_mainPlayerCharacterMovement = m_characterMovement as MainPlayerCharacterMovement;
        }

        protected override void Update()
        {
            base.Update();

            // Check for player "roll" input
            if(Input.GetKeyDown(KeyCode.Space))
            {
                OnRoll();
            }

            // Check for player "block"(shield up) animation (Right mouse button)
            if(Input.GetMouseButtonDown(1) && !m_mainPlayerCharacterMovement.IsPlayerRolling)
            {
                OnShieldUp();
            }

            // If player is not pressing right mouse button, shield down
            if(Input.GetMouseButtonUp(1))
            {
                OnShieldDown();
            }
        }

        // Private Methods -----------------------------------------------------
        void OnShieldUp()
        {
            // Dispatch sharedEvent so animator can listen
            m_playerShieldInputDataEvent.SharedDataValue = true;

            // Disable player movement (There's no animation of running and blocking, so we should be idle when shield up)
            m_characterMovement.DisableMovement(true);
        }

        void OnShieldDown()
        {
            // Dispatch sharedEvent so animator can listen
            m_playerShieldInputDataEvent.SharedDataValue = false;

            // Re-enable player movement
            m_characterMovement.EnableMovement();
        }

        void OnRoll()
        {   
            // Check if player can roll (considering things such as roll delay)
            if(!m_mainPlayerCharacterMovement.CanPlayerRoll) return;

            // Dispatch sharedEvent so animator can listen to it and play roll animation
            m_playerRollInputEvent.DispatchEvent();

            // Request player roll to movement
            m_mainPlayerCharacterMovement.ExecutePlayerRoll();
        }
    }
}
