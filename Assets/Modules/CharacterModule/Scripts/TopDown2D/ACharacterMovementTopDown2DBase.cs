using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CharacterModule.TopDown2D
{
    public abstract class ACharacterMovementTopDown2DBase : MonoBehaviour
    {
        // Serializable Fields ---------------------------------------
        [Header("Character - GeneralConfigs")]
        [SerializeField] protected float m_characterSpeed = 500f;

        [Header("Character - LinkedReferences")]
        [SerializeField] protected Rigidbody2D m_characterRigidBody = null;
        [SerializeField] private Transform m_characterContainer = null;

        // Non-serializable Fields -------------------------------------
        protected Vector2 m_currentMovementDirection = Vector2.zero;
        protected Vector2 m_curFacingDirection = Vector2.right;

        private bool m_canMove = true;

        // Unity Methods ---------------------------------------------------
        protected virtual void Awake()
        {
            // If component is null, try and get the component on this gameObject
            if(m_characterRigidBody == null)
                m_characterRigidBody = GetComponent<Rigidbody2D>();
        }

        // Public Methods ---------------------------------------------------
        public virtual void ExecuteMovement(float horizontalMovementValue, float verticalMovementValue)
        {
            // Create a vector2 from movement values as a direction to move
            // Horizontal: 1 = Right, -1 = Left
            // Vertical: 1 = top, -1 = bottom
            m_currentMovementDirection = new Vector2(horizontalMovementValue,verticalMovementValue);

            if(!m_characterRigidBody)
            {
                Debug.LogError("No rigidbody found for CharacterMovement!");
                return;
            }
            if(!m_canMove) return;

            // Use direction to drive rigidBody velocity
            m_characterRigidBody.velocity = m_characterSpeed * m_currentMovementDirection * Time.fixedDeltaTime;

            // Check if player is looking right or left and flip player accordingly
            CheckFlipMovement(horizontalMovementValue);
        }

        public virtual void EnableMovement()
        {
            m_canMove = true;
        }

        public virtual void DisableMovement(bool bStopCurrentMovement)
        {
            m_canMove = false;

            // If we should stop current movement, set velocity to zero
            if(bStopCurrentMovement) m_characterRigidBody.velocity = Vector3.zero;
        }

        // Private Methods -------------------------------------------------------
        void CheckFlipMovement(float horizontalMovementValue)
        {
            if(!m_characterContainer)
            {
                Debug.LogError("Character container is not set!");
                return;
            }

            // If no horizontalMovement is being send (player is still), do not check flip movement
            if(horizontalMovementValue == 0)
                return;

            // Check if player is looking right
            if(horizontalMovementValue > 0)
            {
                // If looking right, rotate player to zero (0,0,0)
                // @note: A rotation in Unity is always "Quaternion", a Vector4. 
                // However, we can use "Quaternion.Euler", which converts a given rotation in Vector3 (x,y,z) to a Quaternion
                m_characterContainer.rotation = Quaternion.Euler(Vector3.zero);
                m_curFacingDirection = Vector2.right;
                return;
            }

            // Player is looking left, rotate player 180° on y-axis (0,180,0)
            // @note: A rotation in Unity is always "Quaternion", a Vector4. 
            // However, we can use "Quaternion.Euler", which converts a given rotation in Vector3 (x,y,z) to a Quaternion
            m_characterContainer.rotation = Quaternion.Euler(new Vector3(0,180,0));
            m_curFacingDirection = Vector2.left;
        }
    }
}
