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
        [SerializeField] private GameSharedDataEvent<float> m_playerVelocityDataEvent;

        // Unity Methods -----------------------------------------
        void Awake()
        {
            m_playerVelocityDataEvent.AddListener(OnPlayerVelocityUpdate);
        }

        // Private Methods ----------------------------------------------
        void OnPlayerVelocityUpdate(float playerVelocity)
        {
            Debug.Log(playerVelocity);
        }
    }
}
