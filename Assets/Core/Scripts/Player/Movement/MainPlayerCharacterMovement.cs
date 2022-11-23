using System.Collections;
using System.Collections.Generic;
using CharacterModule.TopDown2D;
using GameSharedEventModule;
using UnityEngine;

namespace AIProject.GameModule
{
    public class MainPlayerCharacterMovement : ACharacterMovementTopDown2DBase
    {
        // Serializable Fields ---------------------------------------------
        [Header("MainPlayer - SharedDataEvents")]
        [SerializeField] private GameSharedDataEvent<float> m_playerVelocityMagnitude = null;

        // Public Methods -----------------------------------------------------
        public override void ExecuteMovement(float horizontalMovementValue, float verticalMovementValue)
        {
            base.ExecuteMovement(horizontalMovementValue, verticalMovementValue);

            // Update SharedData for player velocity magnitude (used to update player animator)
            m_playerVelocityMagnitude.SharedDataValue = m_characterRigidBody.velocity.magnitude;
        }
    }
}
