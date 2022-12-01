using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace UIModule
{
    public abstract class AUIScreen : MonoBehaviour
    {
        // Serializable Fileds ---------------------------------------------
        [Header("UIScreenBase - GeneralConfig")]
        [SerializeField] private float m_fadeDelay = 1f;
        [SerializeField] private bool m_showOnAwake = false;

        [Header("UIScreenBase - LinkedReferences")]
        [SerializeField] private CanvasGroup m_uiScreenTargetCanvasGroup = null;
        
        // Unity Methods -----------------------------------------------------
        protected virtual void Awake()
        {
            // If no CanvasGroup, try to get it from this gameObject
            if(!m_uiScreenTargetCanvasGroup)
                m_uiScreenTargetCanvasGroup = GetComponent<CanvasGroup>();
            if(!m_uiScreenTargetCanvasGroup)
            {
                Debug.LogError("UIScreen does not have a CanvasGroup component!");
                return;
            }

            // If show on awake, first do a fade on canvas group with 0s delay (immediately)
            // After that, DOFade with fadeDelay
            if(m_showOnAwake)
            {
                m_uiScreenTargetCanvasGroup.DOFade(0,0f);
                ShowScreen();
            }
        }

        // Public Methods ------------------------------------------------------
        public virtual void ShowScreen()
        {
            if(!m_uiScreenTargetCanvasGroup) return;
            m_uiScreenTargetCanvasGroup.DOFade(1,m_fadeDelay);
        }

        public virtual void HideScreen()
        {
            if(!m_uiScreenTargetCanvasGroup) return;
            m_uiScreenTargetCanvasGroup.DOFade(0,m_fadeDelay);
        }
    }
}
