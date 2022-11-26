using System.Collections;
using System.Collections.Generic;
using CharacterModule.TopDown2D;
using GameSharedEventModule;
using UnityEngine;

namespace AIProject.GameModule
{
    public class MainPlayerCharacterController : APlayerControllerTopDown2DBase
    {
        // Serializable Fields ------------------------------------
        [Header("Input - ShieldUp")]
        [SerializeField] private GameSharedDataEvent<bool> m_playerShieldInputDataEvent;

        // Unity Methods ---------------------------------------------------
        protected override void Update()
        {
            base.Update();

            // Check for player "block"(shield up) animation (Right mouse button)
            if(Input.GetMouseButtonDown(1))
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
            m_playerShieldInputDataEvent.SharedDataValue = true;

            // Disable player movement (There's no animation of running and blocking, so we should be idle when shield up)
            m_characterMovement.DisabelMovement(true);
        }

        void OnShieldDown()
        {
            m_playerShieldInputDataEvent.SharedDataValue = false;

            // Re-enable player movement
            m_characterMovement.EnableMovement();
        }
    }
}
