using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CharacterModule.TopDown2D
{
    public class CharacterMovementTopDown2D : MonoBehaviour
    {
        // Serializable Fields ------------------------------
        [Header("Character - GeneralConfigs")]
        [SerializeField] private float m_characterSpeed = 500f;

        [Header("Character - References")]
        [SerializeField] private Rigidbody2D m_characterRigidBody = null;
        [SerializeField] private Transform m_characterContainer = null;

        // Unity Methods ---------------------------------------------------
        void Awake()
        {
            // If component is null, try and get the component on this gameObject
            if(m_characterRigidBody == null)
                m_characterRigidBody = GetComponent<Rigidbody2D>();
        }

        void Update()
        {
            
        }

        // Public Methods ---------------------------------------------------
        public void ExecuteMovement(float horizontalMovementValue, float verticalMovementValue)
        {
            if(!m_characterRigidBody)
            {
                Debug.LogError("No rigidbody found for CharacterMovement!");
                return;
            }

            // Create a vector2 from movement values as a direction to move
            // Horizontal: 1 = Right, -1 = Left
            // Vertical: 1 = top, -1 = bottom
            Vector2 movementDirection = new Vector2(horizontalMovementValue,verticalMovementValue);

            // Use direction to drive rigidBody velocity
            m_characterRigidBody.velocity = m_characterSpeed * movementDirection * Time.fixedDeltaTime;

            // Check if player is looking right or left and flip player accordingly
            CheckFlipMovement(horizontalMovementValue);
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
                return;
            }

            // Player is looking left, rotate player 180° on y-axis (0,180,0)
            // @note: A rotation in Unity is always "Quaternion", a Vector4. 
            // However, we can use "Quaternion.Euler", which converts a given rotation in Vector3 (x,y,z) to a Quaternion
            m_characterContainer.rotation = Quaternion.Euler(new Vector3(0,180,0));
        }
    }
}
