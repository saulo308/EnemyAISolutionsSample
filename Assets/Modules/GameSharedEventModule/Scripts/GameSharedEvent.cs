using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace GameSharedEventModule
{
    [Serializable, CreateAssetMenu(menuName = "GameSharedEventModule/GameSharedEvent", fileName = "NewGameSharedEvent")]
    public class GameSharedEvent : ScriptableObject
    {
        // Non-Serializable Fields ----------------------------------------------
        HashSet<System.Action> m_sharedEventListeners = new HashSet<System.Action>();

        // Public Methods ------------------------------------------------------
        public void DispatchEvent()
        {
            foreach(var listener in m_sharedEventListeners)
            {
                if(listener != null) 
                    listener.Invoke();
            }
        }

        public void AddListener(System.Action newListener)
        {
            m_sharedEventListeners.Add(newListener);
        }

        public void RemoveListener(System.Action listenerToRemove)
        {
            m_sharedEventListeners.Remove(listenerToRemove);
        }

        public void RemoveAllListeners()
        {
            m_sharedEventListeners.Clear();
        }

        public void PurgeListeners()
        {
            foreach(var listener in m_sharedEventListeners)
            {
                if(listener == null) m_sharedEventListeners.Remove(listener);
            }
        }
    }
}
