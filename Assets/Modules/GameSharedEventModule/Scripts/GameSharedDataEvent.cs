using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameSharedEventModule
{
    public abstract class GameSharedDataEvent<T> : ScriptableObject
    {
        // Serializable Fields ---------------------------------------------------
        [SerializeField] private T m_defaultValue = default(T);
        [SerializeField] private bool m_dispatchOnChanged = true;

        // Non-Serializable Fields ----------------------------------------------
        private T m_sharedDataValue = default (T);
        private HashSet<System.Action<T>> m_dataEventListeners = new HashSet<System.Action<T>>();

        // Properties ---------------------------------------------------------
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
            m_sharedDataValue = m_defaultValue;
        }

        // Public Methods ------------------------------------------------------
        public void DispatchDataEvent()
        {
            foreach(var listener in m_dataEventListeners)
            {
                if(listener != null) 
                    listener.Invoke(m_sharedDataValue);
            }
        }

        public void AddListener(System.Action<T> newListener)
        {
            m_dataEventListeners.Add(newListener);
        }

        public void RemoveListener(System.Action<T> listenerToRemove)
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

        // Private Methods ------------------------------------------------------------
        void OnDataChanged()
        {
            if(m_dispatchOnChanged)
                DispatchDataEvent();
        }
    }
}