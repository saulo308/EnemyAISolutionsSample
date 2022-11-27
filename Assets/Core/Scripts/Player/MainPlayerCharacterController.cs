using System.Collections;
using System.Collections.Generic;
using CharacterModule.TopDown2D;
using GameSharedEventModule;
using DG.Tweening;
using UtilsModule;
using UnityEngine;

namespace AIProject.GameModule
{
    // TODO: Shield and attack logic should be in another component, just like movement
    public class MainPlayerCharacterController : APlayerControllerTopDown2DBase
    {
        // Serializable Fields -------------------------------------------
        [Header("SharedEvents")]
        [SerializeField] private GameSharedEvent m_playerRollInputEvent;

        [Header("MainPlayer - LinkedReferences")]
        [SerializeField] private MainPlayerCombatController m_mainPlayerCombatController = null;

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

            if(!m_mainPlayerCombatController.IsPlayerHurt || !m_mainPlayerCombatController.IsPlayerDead) 
            {
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

                if(Input.GetMouseButtonDown(0) && !m_mainPlayerCharacterMovement.IsPlayerRolling)
                {
                    OnAttackPressed();
                }
            }
        }

        // Private Methods -----------------------------------------------------
        void OnRoll()
        {   
            // Check if player can roll (considering things such as roll delay)
            if(!m_mainPlayerCharacterMovement.CanPlayerRoll) return;

            // Check if shield is up, if it's, put shield down and roll (roll has priority)
            // TODO: If player is still holding right-mouse button, shield should be up once again
            OnShieldDown();

            // Request player roll to movement
            m_mainPlayerCharacterMovement.ExecutePlayerRoll();
        }

        void OnShieldUp()
        {
            // Disable player movement (There's no animation of running and blocking, so we should be idle when shield up)
            m_characterMovement.DisableMovement(true);

            // Call shield up on combatController
            m_mainPlayerCombatController.OnShieldUp();
        }

        void OnShieldDown()
        {
            // Re-enable player movement
            m_characterMovement.EnableMovement();

            // Call shield down on combatController
            m_mainPlayerCombatController.OnShieldDown();
        }

        void OnAttackPressed()
        {
            m_mainPlayerCombatController.OnAttackPressed();
        }
    }
}
