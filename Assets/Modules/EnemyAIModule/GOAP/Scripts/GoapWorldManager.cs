using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EnemyAIModule.GOAP
{
    [System.Serializable]
    public class GoapStateData
    {
        public string Key;
        public int Value;
    }

    public class GoapStateDataDict
    {
        public Dictionary<string, int> StateDict = new Dictionary<string, int>();
    }

    public class GoapWorldManager
    {
        // Non-Serializable Fields ----------------------------------------
        private static readonly GoapWorldManager m_goapWorldInstance = new GoapWorldManager();
        private GoapStateDataDict m_worldStates = new GoapStateDataDict();

        // Properties ----------------------------------------------------
        public static GoapWorldManager GoapWorldInstance => m_goapWorldInstance;

        // Public Methods ---------------------------------------------------
        public GoapStateDataDict GetWorldStates()
        {
            return m_worldStates;
        }
    }
}
