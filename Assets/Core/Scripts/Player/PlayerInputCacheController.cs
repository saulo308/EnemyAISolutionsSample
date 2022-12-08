using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AIProject.GameModule
{
    public class PlayerInputCacheController : MonoBehaviour
    {
        // Serializable Fields ------------------------------------------
        [SerializeField] private MainPlayerCharacterController m_mainPlayerCharacterController = null;

        // Non-Serializable Fields ----------------------------------------
        private static PlayerInputCacheController m_instance = null;

        // Properties ----------------------------------------------------
        public static PlayerInputCacheController Instance => m_instance;

        // Unity Methods --------------------------------------------------
        void Awake()
        {
            m_instance = this;
        }

        // Public Methods ---------------------------------------------
        public void CheckForCachedInputs()
        {
            m_mainPlayerCharacterController.CheckCachedInputs();
        }
    }
}
