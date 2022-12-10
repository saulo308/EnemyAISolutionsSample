using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

namespace GameSharedEventModule
{
    public class GameSharedEventListener : MonoBehaviour
    {
        // Serializable Fields -------------------------------
        [SerializeField] private GameSharedEvent m_targetSharedEvent = null;
        [SerializeField] private UnityEvent m_onSharedEventDispatch = new UnityEvent();

        // Unity Methods ----------------------------------------
        void Awake()
        {
            m_targetSharedEvent.AddListener(OnSharedEventDispatch);
        }

        void OnDestroy()
        {
            m_targetSharedEvent.RemoveAllListeners();
        }

        // Private Methods ----------------------------------------
        void OnSharedEventDispatch()
        {
            m_onSharedEventDispatch.Invoke();
        }
    }
}
