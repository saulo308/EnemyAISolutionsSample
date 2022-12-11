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
        // Store all the listeners to this shared event
        HashSet<System.Action> m_sharedEventListeners = new HashSet<System.Action>();

        // Public Methods ------------------------------------------------------
        public void DispatchEvent()
        {
            // When we dispatch, we call Invoke() to all registered listeners
            foreach(var listener in m_sharedEventListeners)
            {
                if(listener != null) 
                    listener.Invoke();
            }
        }

        // Helper functions to manipulate HashSet
        public void AddListener(System.Action newListener) => m_sharedEventListeners.Add(newListener);
        public void RemoveListener(System.Action listenerToRemove) => m_sharedEventListeners.Remove(listenerToRemove);
        public void RemoveAllListeners() => m_sharedEventListeners.Clear();
    }
}
