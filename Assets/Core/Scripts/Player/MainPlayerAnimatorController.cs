using System.Collections;
using System.Collections.Generic;
using CharacterModule;
using GameSharedEventModule;
using DG.Tweening;
using UnityEngine;

namespace AIProject.GameModule
{
    public class MainPlayerAnimatorController : ACharacterAnimatorControllerBase
    {
        // Serializable Fields ------------------------------------
        [SerializeField] private GameSharedDataEvent<float> m_playerVelocityDataEvent;
        [SerializeField] private GameSharedDataEvent<bool> m_playerShieldInputDataEvent;

        // Unity Methods -----------------------------------------
        protected override void Awake()
        {
            base.Awake();

            // Binding events on sharedEvents
            m_playerVelocityDataEvent.AddListener(OnPlayerVelocityUpdate);
            m_playerShieldInputDataEvent.AddListener(OnPlayerShieldInputPressed);
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
    }
}
