using System.Collections;
using System.Collections.Generic;
using GameSharedEventModule;
using CharacterModule;
using DG.Tweening;
using UnityEngine;

namespace AIProject.GameModule
{
    public class MainPlayerCombatController : ACharacterCombatControllerBase
    {
        // Serializable Fields ---------------------------------------------
        [Header("MainPlayer - LinkedReferences")]
        [SerializeField] private MainPlayerCharacterMovement m_mainPlayerCharacterMovement;

        [Header("MainPlayer - SharedEvents")]
        [SerializeField] private GameSharedDataEvent<string> m_playerAttackInputDataEvent;
        [SerializeField] private GameSharedDataEvent<bool> m_playerShieldInputDataEvent;
        [SerializeField] private GameSharedDataEvent<AnimationEvent> m_playerSharedEventAnimationEvent = null;
        [SerializeField] private GameSharedEvent m_playerHurtEvent;
        [SerializeField] private GameSharedEvent m_playerDeadEvent;

        [Header("MainPlayer - GeneralConfig")]
        [SerializeField] private int m_maxNumberOfAttacksCombo = 3;
        [SerializeField] private float m_attackDelay = 0.4f;
        [SerializeField] private float m_resetComboDelay = 1f;

        // Non-Serializable Fields -----------------------------------------

        // Attack flags
        private int m_curAttackComboIndex = 1;
        private float m_timeSinceLastAttack = 0f;
        private Tween m_resetComboTween = null;

        // Life flags
        private bool m_isHurt = false;
        private bool m_isDead = false;

        // Properties ---------------------------------------------------------
        public bool IsPlayerHurt => m_isHurt;
        public bool IsPlayerDead => m_isDead;

        // Unity Methods ---------------------------------------------------
        protected override void Awake()
        {
            // Bind event on animation event listener
            m_playerSharedEventAnimationEvent.AddListener(OnAnimationEventTrigerred);
        }

        void OnDestroy()
        {
            m_playerSharedEventAnimationEvent.RemoveListener(OnAnimationEventTrigerred);
        }

        // Public Methods -----------------------------------------------------
        public void OnShieldUp()
        {
            // Dispatch sharedEvent so animator can listen
            m_playerShieldInputDataEvent.SharedDataValue = true;
        }

        public void OnShieldDown()
        {
            // Dispatch sharedEvent so animator can listen
            m_playerShieldInputDataEvent.SharedDataValue = false;
        }

        public void OnAttackPressed()
        {
            // Get current time and check if it's smaller than attackDelay. If it's then it's too soon to attack
            float curTime = Time.time;
            if(curTime - m_timeSinceLastAttack < m_attackDelay) return;

            // Check if reset coroutine (delay) is active. If it's, then kill it (so we can refresh combo reset delay)
            if(m_resetComboTween.IsActive()) m_resetComboTween.Kill();

            // Create string "Attack[1,2,3,..]" to send to animator as trigger
            string attackComboAnimatorTriggerName = string.Format("Attack{0}",m_curAttackComboIndex);
            m_playerAttackInputDataEvent.SharedDataValue = attackComboAnimatorTriggerName;

            // Increase attack combo index.
            // If it's already max, then reset to 1
            if(m_curAttackComboIndex == m_maxNumberOfAttacksCombo) m_curAttackComboIndex = 1;
            else m_curAttackComboIndex++;

            // Increase time since last attack by time.deltaTime so we can check for attackDelay against future attack time
            m_timeSinceLastAttack = Time.time;

            // Also, start a delay to rest comboIndex if next combo does not come within treshold
            m_resetComboTween = DOVirtual.DelayedCall(m_resetComboDelay,() => m_curAttackComboIndex = 1);
        }

        // Protected Methods ---------------------------------------------------------------------
        protected override void OnCharacterHurt()
        {
            // TODO: Should be replaced by "IsInvicible" during roll
            if(m_mainPlayerCharacterMovement.IsPlayerRolling) return;

            // Disable player movement (When hurt, player should be 'stunned' during hurt animation)
            m_mainPlayerCharacterMovement.DisableMovement(true);

            // Dispatch hurt event to notify player animator
            m_playerHurtEvent.DispatchEvent();

            // Set flag to avoid other inputs
            m_isHurt = true;
        }

        protected override void OnCharacterDead()
        {
            base.OnCharacterDead();

            // TODO: Should be replaced by "IsInvicible" during roll
            if(m_mainPlayerCharacterMovement.IsPlayerRolling) return;

            // Disable player movement (When hurt, player should be 'stunned' during hurt animation)
            m_mainPlayerCharacterMovement.DisableMovement(true);

            // Dispatch hurt event to notify player animator
            m_playerDeadEvent.DispatchEvent();

            // Set flag to avoid other inputs
            m_isDead = true;
        }

        // Evnet Handlers ---------------------------------------------------------------
        void OnAnimationEventTrigerred(AnimationEvent triggeredAnimationEvent)
        {
            if(triggeredAnimationEvent.stringParameter.Equals("OnHurtEnd"))
            {
                // Re-enable player movement on hurt animation end
                m_mainPlayerCharacterMovement.EnableMovement();

                // Reset flag to re-enable inputs
                m_isHurt = false;
            }

            if(triggeredAnimationEvent.stringParameter.Equals("OnDeadEnd"))
            {
                Debug.Log("Player dead!");
            }
        }
    }
}