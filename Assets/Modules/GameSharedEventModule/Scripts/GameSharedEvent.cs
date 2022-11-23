using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace GameSharedEventModules
{
    [Serializable, CreateAssetMenu(menuName = "GameSharedEventModule/GameSharedEvent", fileName = "NewGameSharedEvent")]
    public class GameSharedEvent : ScriptableObject
    {
        // Non-Serializable Fields ----------------------------------------------
        HashSet<System.Action> m_dataEventListeners = new HashSet<System.Action>();

        // Public Methods ------------------------------------------------------
        public void DispatchDataEvent()
        {
            foreach(var listener in m_dataEventListeners)
            {
                if(listener != null) 
                    listener.Invoke();
            }
        }

        public void AddListener(System.Action newListener)
        {
            m_dataEventListeners.Add(newListener);
        }

        public void RemoveListener(System.Action listenerToRemove)
        {
            m_dataEventListeners.Remove(listenerToRemove);
        }

        public void RemoveAllListeners()
        {
            m_dataEventListeners.Clear();
        }

        public void PurgeListeners()
        {
            foreach(var listener in m_dataEventListeners)
            {
                if(listener == null) m_dataEventListeners.Remove(listener);
            }
        }
    }
}
