using System.Collections;
using System.Collections.Generic;
using CharacterModule;
using GameSharedEventModule;
using UnityEngine;

namespace AIProject.GameModule
{
    public class MainPlayerAnimatorController : ACharacterAnimatorControllerBase
    {
        // Serializable Fields ------------------------------------
        [Header("SharedEvents")]
        [SerializeField] private GameSharedDataEvent<float> m_playerVelocityDataEvent;
        [SerializeField] private GameSharedDataEvent<bool> m_playerShieldInputDataEvent;
        [SerializeField] private GameSharedDataEvent<string> m_playerAttackInputDataEvent;

        [SerializeField] private GameSharedEvent m_playerRollInputEvent;
        [SerializeField] private GameSharedEvent m_playerHurtEvent;
        [SerializeField] private GameSharedEvent m_playerEndLifeEvent;

        // Unity Methods -----------------------------------------
        protected override void Awake()
        {
            base.Awake();

            // Binding events on sharedEvents
            m_playerVelocityDataEvent.AddListener(OnPlayerVelocityUpdate);
            m_playerShieldInputDataEvent.AddListener(OnPlayerShieldInputPressed);
            m_playerRollInputEvent.AddListener(OnPlayerRollInputPressed);
            m_playerAttackInputDataEvent.AddListener(OnPlayerAttackInputPressed);
            m_playerHurtEvent.AddListener(OnPlayerHurt);
            m_playerEndLifeEvent.AddListener(OnPlayerEndLife);
        }

        void OnDestroy()
        {
            m_playerVelocityDataEvent.RemoveAllListeners();
            m_playerShieldInputDataEvent.RemoveAllListeners();
            m_playerRollInputEvent.RemoveAllListeners();
            m_playerAttackInputDataEvent.RemoveAllListeners();
            m_playerHurtEvent.RemoveAllListeners();
            m_playerEndLifeEvent.RemoveAllListeners();
        }

        // Private Methods ----------------------------------------------
        void OnPlayerVelocityUpdate(float playerVelocityMagnitude)
        {
            // If player velocity magnitude is greater than 0, player is moving. 
            bool bIsRunning = playerVelocityMagnitude > 0;
            SetAnimatorBool("IsRunning",bIsRunning);
        }

        void OnPlayerShieldInputPressed(bool bShieldUp)
        {
            // If param is true, then player is initiating a block. Set trigger to play BlockIdle animation
            if(bShieldUp) SetAnimatorTrigger("Block");

            // Also, set boolean to inform Animator if we are still blocking or not (Used to keep playing BlockIdle animation)
            SetAnimatorBool("IsShieldUp",bShieldUp);
        }

        // Set triggers on animator
        void OnPlayerRollInputPressed() =>  SetAnimatorTrigger("Roll");
        void OnPlayerAttackInputPressed(string attackTriggerName) => SetAnimatorTrigger(attackTriggerName);
        void OnPlayerHurt() => SetAnimatorTrigger("Hurt");
        void OnPlayerEndLife() => SetAnimatorTrigger("Dead");
    }
}
