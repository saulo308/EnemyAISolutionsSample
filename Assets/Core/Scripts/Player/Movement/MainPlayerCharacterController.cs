using System.Collections;
using System.Collections.Generic;
using CharacterModule.TopDown2D;
using GameSharedEventModule;
using DG.Tweening;
using UnityEngine;

namespace AIProject.GameModule
{
    // TODO: Shield and attack logic should be in another component, just like movement
    public class MainPlayerCharacterController : APlayerControllerTopDown2DBase
    {
        // Serializable Fields -------------------------------------------
        [Header("Input")]
        [SerializeField] private GameSharedDataEvent<bool> m_playerShieldInputDataEvent;
        [SerializeField] private GameSharedDataEvent<string> m_playerAttackInputDataEvent;
        [SerializeField] private GameSharedEvent m_playerRollInputEvent;

        [Header("GeneralConfig")]
        [SerializeField] private int m_maxNumberOfAttacksCombo = 3;
        [SerializeField] private float m_attackDelay = 0.4f;
        [SerializeField] private float m_resetComboDelay = 1f;

        // Non-Serializable Fields -----------------------------------------
        private MainPlayerCharacterMovement m_mainPlayerCharacterMovement;

        
        private bool m_isShieldUp = false;

        private int m_curAttackComboIndex = 1;
        private float m_timeSinceLastAttack = 0f;
        private Tween m_resetComboTween = null;




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

        // Private Methods -----------------------------------------------------
        void OnShieldUp()
        {
            // Dispatch sharedEvent so animator can listen
            m_playerShieldInputDataEvent.SharedDataValue = true;

            // Disable player movement (There's no animation of running and blocking, so we should be idle when shield up)
            m_characterMovement.DisableMovement(true);

            m_isShieldUp = true;
        }

        void OnShieldDown()
        {
            // Dispatch sharedEvent so animator can listen
            m_playerShieldInputDataEvent.SharedDataValue = false;

            // Re-enable player movement
            m_characterMovement.EnableMovement();

            m_isShieldUp = false;
        }

        void OnRoll()
        {   
            // Check if player can roll (considering things such as roll delay)
            if(!m_mainPlayerCharacterMovement.CanPlayerRoll) return;

            // Check if shield is up, if it's, put shield down and roll (roll has priority)
            // TODO: If player is still holding right-mouse button, shield should be up once again
            OnShieldDown();

            // Dispatch sharedEvent so animator can listen to it and play roll animation
            m_playerRollInputEvent.DispatchEvent();

            // Request player roll to movement
            m_mainPlayerCharacterMovement.ExecutePlayerRoll();
        }

        void OnAttackPressed()
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
    }
}
