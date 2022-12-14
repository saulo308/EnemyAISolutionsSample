using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameSharedEventModule
{
    /** This shared event works like 'GameSharedEvent' but send a value [T] instead */
    public abstract class GameSharedDataEvent<T> : ScriptableObject
    {
        // Serializable Fields ---------------------------------------------------
        [SerializeField] private T m_defaultValue = default(T);
        [SerializeField] private bool m_dispatchOnChanged = true;

        // Non-Serializable Fields ----------------------------------------------
        private T m_sharedDataValue = default (T);
        private HashSet<System.Action<T>> m_dataEventListeners = new HashSet<System.Action<T>>();

        // Properties ---------------------------------------------------------
        /** When stored data is changed, we call OnDataChanged() to dispatch new data to all listeners (if flag is true) */
        public T SharedDataValue 
        {
            get { return m_sharedDataValue; }
            set 
            { 
                m_sharedDataValue = value;
                OnDataChanged();
            }
        }

        // Unity Methods ------------------------------------------------------
        void OnEnable()
        {
            // Set data as default on enabled
            m_sharedDataValue = m_defaultValue;
        }

        // Public Methods ------------------------------------------------------
        public void DispatchDataEvent()
        {
            // When we dispatch, we call Invoke() to all registered listeners
            foreach(var listener in m_dataEventListeners)
            {
                if(listener != null) 
                    listener.Invoke(m_sharedDataValue);
            }
        }

        // Helper functions to manipulate HashSet
        public void AddListener(System.Action<T> newListener) => m_dataEventListeners.Add(newListener);
        public void RemoveListener(System.Action<T> listenerToRemove) => m_dataEventListeners.Remove(listenerToRemove);
        public void RemoveAllListeners() => m_dataEventListeners.Clear();

        // Private Methods ------------------------------------------------------------
        void OnDataChanged()
        {
            // Data has changed, if flag is true, dispatch to all listeners
            if(m_dispatchOnChanged)
                DispatchDataEvent();
        }
    }
}