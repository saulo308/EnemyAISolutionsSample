using System.Collections;
using System.Collections.Generic;
using CharacterModule.TopDown2D;
using GameSharedEventModule;
using UnityEngine;

namespace AIProject.GameModule
{
    public class MainEnemyCharacterMovement : ACharacterMovementTopDown2DBase
    {
        [Header("MainEnemy - SharedDataEvents")]
        [SerializeField] private GameSharedDataEvent<float> m_enemyVelocityMagnitude = null;

        // Public Methods -----------------------------------------------------
        public override void MoveToDirection(Vector2 movementDirection)
        {
            // Call base
            base.MoveToDirection(movementDirection);

            // Update SharedData for player velocity magnitude (used to update player animator)
            m_enemyVelocityMagnitude.SharedDataValue = m_characterRigidBody.velocity.magnitude;
        }
    }
}
