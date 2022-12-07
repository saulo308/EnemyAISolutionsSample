using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CharacterModule.TopDown2D
{
    public abstract class AAIControllerTopDown2DBase : MonoBehaviour
    {
        // Serializable Fields ---------------------------------------------------------------------
        [Header("LinkedReferences")]
        [SerializeField] protected GameObject m_targetReference = null;
        [SerializeField] protected float m_targetDistanceLimit = 50f;
        [SerializeField] protected ACharacterMovementTopDown2DBase m_characterMovement = null;

        // Unity Methods -----------------------------------------------------------------
        protected virtual void Awake()
        {
            // If component is null, try and get the component on this gameObject
            if(m_characterMovement == null)
                m_characterMovement = GetComponent<ACharacterMovementTopDown2DBase>();
        }
        
        protected virtual void FixedUpdate()
        {
            // Move enemy to target position (is this sample, target = player)
            // NOTE: This is really simple, movement logic v.0.1
            // TODO: Redo enemy movement
            FollowTarget();
        }

        // Public Methods ---------------------------------------------------------
        public void SetNewTarget(GameObject newTarget) => m_targetReference = newTarget;
        public void SetNewTargetDistanceLimit(float newTargetDistance) => m_targetDistanceLimit = newTargetDistance;
        public bool IsDistanceToTargetLessThanLimit() => (GetDistanceToTarget() < m_targetDistanceLimit);

        // Private Methods -------------------------------------------------------------
        void FollowTarget()
        {
            if(!m_characterMovement)
            {
                Debug.LogError("No CharacterMovement found for PlayerController!");
                return;
            }

            // While did not reach target limit, keep going on it's direction
            if(GetDistanceToTarget() > m_targetDistanceLimit)
            {
                // Get direction to move (move towards player)
                Vector2 moveDirection = GetDirectionToFollowPlayer();

                // Move AI towards target direction
                m_characterMovement.MoveToDirection(moveDirection);
            }
            else
            {
                // If on range, stop movement
                m_characterMovement.StopCurrentMovement();
            }
        }

        Vector2 GetDirectionToFollowPlayer()
        {
            if(!m_targetReference) return Vector2.zero;

            Vector2 directionToFollowTarget = Vector2.zero;

            // Get target position
            var targetPos = m_targetReference.transform.position;

            // Get AI pos
            var currentPos = transform.position;

            // Calculate direction to target
            directionToFollowTarget = targetPos - currentPos;

            return directionToFollowTarget;
        }

        float GetDistanceToTarget()
        {
            if(!m_targetReference) return 0;

            // Get target position
            var targetPos = m_targetReference.transform.position;

            // Get AI pos
            var currentPos = transform.position;

            return Vector2.Distance(currentPos,targetPos);
        }
    }
}
