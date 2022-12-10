using System.Collections;
using System.Collections.Generic;
using UIModule;
using GameSharedEventModule;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine;

namespace AIProject.GameModule
{
    public class UIScreenMainGameHUD : AUIScreen
    {
        // Serializable Fields ------------------------------------
        [Header("MainHUD - LinkedReferences")]
        [SerializeField] private Image m_playerHealthFillBar = null;
        [SerializeField] private Image m_mainEnemyHealthFillBar = null;

        [Header("MainHUD - SharedEvents")]
        [SerializeField] private GameSharedDataEventFloat m_playerHealthPercentageSharedEvent = null;
        [SerializeField] private GameSharedDataEventFloat m_mainEnemyHealthPercentageSharedEvent = null;

        // Unity Methods --------------------------------------------
        protected override void Awake()
        {
            base.Awake();

            // Bind SharedData events
            m_playerHealthPercentageSharedEvent.AddListener(OnPlayerHealthChange);
            m_mainEnemyHealthPercentageSharedEvent.AddListener(OnMainEnemyHealthChange);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            m_playerHealthPercentageSharedEvent.RemoveAllListeners();
            m_mainEnemyHealthPercentageSharedEvent.RemoveAllListeners();
        }

        // Private Methods ------------------------------------------------
        void OnPlayerHealthChange(float newPlayerHealthPercentage)
        {
            m_playerHealthFillBar.DOFillAmount(newPlayerHealthPercentage,0.5f);
        }

        void OnMainEnemyHealthChange(float newMainEnemyPlayerHealthPercentage)
        {
            m_mainEnemyHealthFillBar.DOFillAmount(newMainEnemyPlayerHealthPercentage,0.5f);
        }
    }
}
