using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using CharacterModule.TopDown2D;
using GameSharedEventModule;
using UnityEngine.UI;
using UnityEngine;

namespace AIProject.GameModule
{
    public class MainPlayerCharacterMovement : ACharacterMovementTopDown2DBase
    {
        // Serializable Fields ---------------------------------------------
        [Header("MainPlayer - GeneralConfig")]
        [SerializeField] private float m_rollPlayerSpeedMultipler = 1.8f;
        [SerializeField] private float m_playerRollDelay = 1f;
        [SerializeField] private Image m_dodgeFillBar = null;

        [Header("MainPlayer - SharedDataEvents")]
        [SerializeField] private GameSharedDataEvent<float> m_playerVelocityMagnitude = null;
        [SerializeField] private GameSharedEvent m_playerRollInputEvent;
        [SerializeField] private GameSharedDataEvent<AnimationEvent> m_playerSharedEventAnimationEvent = null;

        // Non-Serializable Fields ----------------------------------------------
        private Vector2 m_rollDirection = Vector2.zero;
        private bool m_isRolling = false;
        private bool m_canPlayerRoll = true;

        private LayerMask m_playerLayer;
        private LayerMask m_mainEnemyLayer;

        private Tween m_dodgeCooldownTween = null;

        // Properties ---------------------------------------------------------
        public bool CanPlayerRoll => m_canPlayerRoll;
        public bool IsPlayerRolling => m_isRolling;

        // Unity Methods -----------------------------------------------------------
        protected override void Awake()
        {
            base.Awake();

            // Bind event on animation event listener
            m_playerSharedEventAnimationEvent.AddListener(OnAnimationEventTrigerred);
        }

        protected virtual void Update()
        {
            // Update dodge bar
            if(m_dodgeCooldownTween != null && 
                m_dodgeCooldownTween.IsActive()) m_dodgeFillBar.fillAmount = (1 - m_dodgeCooldownTween.ElapsedPercentage());
            else 
                m_dodgeFillBar.fillAmount = 0;
        }

        protected void OnDestroy()
        {
            m_playerSharedEventAnimationEvent.RemoveListener(OnAnimationEventTrigerred);
            if(m_dodgeCooldownTween != null) m_dodgeCooldownTween.Kill();
        }

        // Public Methods -----------------------------------------------------
        public override void ExecuteMovement(float horizontalMovementValue, float verticalMovementValue)
        {
            // Can not move while player is rolling
            if(m_isRolling) return;

            // Call base
            base.ExecuteMovement(horizontalMovementValue, verticalMovementValue);

            // Update SharedData for player velocity magnitude (used to update player animator)
            m_playerVelocityMagnitude.SharedDataValue = m_characterRigidBody.velocity.magnitude;
        }

        public void ExecutePlayerRoll()
        {
            if(!m_canPlayerRoll) return;

            // Dispatch sharedEvent so animator can listen to it and play roll animation
            m_playerRollInputEvent.DispatchEvent();

            // Set flags, "m_canPlayerRoll" to await rollDelay and "m_isRolling" to avoid baseMovement
            m_canPlayerRoll = false;
            m_isRolling = true;

            // Get roll direction. Use currentMovementDirection. If null, then use facing direction
            m_rollDirection = (m_currentMovementDirection.Equals(Vector2.zero)) ?
                m_curFacingDirection :
                m_currentMovementDirection.normalized;

            // Execute roll. First stop baseMovement and then update velocity
            m_characterRigidBody.velocity = (m_characterSpeed * m_rollPlayerSpeedMultipler) * m_rollDirection * Time.fixedDeltaTime;

            // Start roll delay using DelayedCall
            m_dodgeCooldownTween = DOVirtual.DelayedCall(m_playerRollDelay, () => m_canPlayerRoll = true);

            // Update collider to IGNORE mainEnemey layer (player can dodge through enemy and enemy's attack)
            Physics2D.IgnoreLayerCollision(GetLayerIdByLayerMask(m_playerLayer),GetLayerIdByLayerMask(m_mainEnemyLayer),true);
        }

        // Function called by "Anim_Player_Roll" animation event to reset player roll flag
        public void OnPlayerFinishedRoll()
        {
            m_isRolling = false;

            // Trigger check for cached inputs
            PlayerInputCacheController.Instance.CheckForCachedInputs();
        }

        public void SetupData(LayerMask playerLayerMask, LayerMask enemyLayerMask)
        {
            m_playerLayer = playerLayerMask;
            m_mainEnemyLayer = enemyLayerMask;
        }

        // Private Methods -------------------------------------------------------------
        void OnAnimationEventTrigerred(AnimationEvent triggeredAnimationEvent)
        {
            if(triggeredAnimationEvent.stringParameter.Equals("OnRollEnd"))
            {
                // Update collider to consider mainEnemey layer
                // (player can no longer dodge trough enemy and enemy's attack)
                Physics2D.IgnoreLayerCollision(GetLayerIdByLayerMask(m_playerLayer),GetLayerIdByLayerMask(m_mainEnemyLayer),false);

                // Call finishedRoll
                OnPlayerFinishedRoll();
            }
        }

        int GetLayerIdByLayerMask(LayerMask target)
        {
            return Mathf.RoundToInt(Mathf.Log(target.value,2));
        }
    }
}
