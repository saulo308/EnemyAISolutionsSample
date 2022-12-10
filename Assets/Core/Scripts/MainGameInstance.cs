using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

namespace AIProject.GameModule
{
    public class MainGameInstance : MonoBehaviour
    {
        // Serializable Fields -------------------------------------------
        [Header("GameInstance - LinkedRefs - Characters")]
        [SerializeField] private MainPlayerCharacterController m_mainPlayerController = null;
        [SerializeField] private MainEnemyCharacterController m_mainEnemyPlayerController = null;

        // Non-Serializable Fields ----------------------------------------
        private static MainGameInstance m_mainGameInstance = null;

        // Properties ----------------------------------------------------
        public static MainGameInstance GameInstance => m_mainGameInstance;

        public MainPlayerCharacterController MainPlayerController => m_mainPlayerController;
        public MainEnemyCharacterController MainEnemyPlayerController => m_mainEnemyPlayerController;

        // Unity Methods --------------------------------------------------
        void Awake()
        {
            m_mainGameInstance = this;
        }

        void Update()
        {
            if(Input.GetKeyDown(KeyCode.R))
                SceneManager.LoadScene("GOAPTestScene");
        }
    }
}