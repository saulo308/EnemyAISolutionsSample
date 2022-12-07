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

        public void MergeWithGoapStateDict(GoapStateDataDict mergeTarget)
        {
            foreach(var state in mergeTarget.StateDict)
                AddUniquePair(state.Key,state.Value);
        }

        public void AddUniquePair(string Key, int Value)
        {
            if(!StateDict.ContainsKey(Key))
                StateDict.Add(Key,Value);
        }

        public static GoapStateDataDict StateDataListToGoapStateDataDict(List<GoapStateData> statesTargetList)
        {
            GoapStateDataDict newGoapStateDataDict = new GoapStateDataDict();
            Dictionary<string, int> targetStatesDict = new Dictionary<string, int>();

            foreach(var goalTargetState in statesTargetList)
                targetStatesDict.Add(goalTargetState.Key, goalTargetState.Value);
            newGoapStateDataDict.StateDict = targetStatesDict;
            
            return newGoapStateDataDict;
        }
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
