using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CharacterModule.TopDown2D
{
    public abstract class APlayerControllerTopDown2DBase : MonoBehaviour
    {
        // Serializable Fields -----------------------------------------------
        [Header("LinkedReferences")]
        [SerializeField] protected ACharacterMovementTopDown2DBase m_characterMovement = null;

        // Non-Serializable Fields -------------------------------------------
        private float m_horizontalAxisValue = 0f;
        private float m_verticalAxisValue = 0f;

        // Unity Methods -----------------------------------------------------
        protected virtual void Awake()
        {
            // If component is null, try and get the component on this gameObject
            if(m_characterMovement == null)
                m_characterMovement = GetComponent<ACharacterMovementTopDown2DBase>();
        }

        protected virtual void Update()
        {
            // Get player input values (Horizontal and Vertical axis and store them)
            UpdatePlayerInputValues();
        }

        protected virtual void FixedUpdate()
        {
            // Move player according to input values
            RequestPlayerMovement();
        }

        // Private Methods ------------------------------------------------------
        void UpdatePlayerInputValues()
        {
            // Get horizontal axis input
            m_horizontalAxisValue = Input.GetAxis("Horizontal");

            // Get vertical axis input
            m_verticalAxisValue = Input.GetAxis("Vertical");
        }

        void RequestPlayerMovement()
        {
            if(!m_characterMovement)
            {
                Debug.LogError("No CharacterMovement found for PlayerController!");
                return;
            }
            m_characterMovement.ExecuteMovement(m_horizontalAxisValue,m_verticalAxisValue);
        }
    }
}
