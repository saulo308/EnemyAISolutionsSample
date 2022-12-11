using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EnemyAIModule.GOAP
{
    /** 
    * Every state is stored with a given key and a value so it can be used in a dictionary
    * Note that in this sample, we are using only key, but value could also be used for states and actions
    * (e.g. we could have a state with key 'Player health' and value '50' to tell that player has 50 health)
    */
    [System.Serializable]
    public class GoapStateData
    {
        public string Key;
        public int Value;
    }

    // Stores state data as a dictionary, both for easier use and also for performance
    public class GoapStateDataDict
    {
        // Dictionary that hold state data
        public Dictionary<string, int> StateDict = new Dictionary<string, int>();

        // Helper functions -----------

        /** Merges a given GoapStateDataDict with this current StateDict */
        public void MergeWithGoapStateDict(GoapStateDataDict mergeTarget)
        {
            foreach(var state in mergeTarget.StateDict)
                AddUniquePair(state.Key,state.Value);
        }

        /** Adds a entry on this StateDict (if does not already exists) */
        public void AddUniquePair(string Key, int Value)
        {
            if(!StateDict.ContainsKey(Key))
                StateDict.Add(Key,Value);
        }

        /** Removes a entry on this state dict */
        public void Remove(string Key)
        {
            if(StateDict.ContainsKey(Key))
                StateDict.Remove(Key);
        }

        /** Transform a list of GoapStateData into a dictionary. Lists are used to serialize it on editor.
        * however in code, it's easier to use dictionaries.
        */
        public static GoapStateDataDict StateDataListToGoapStateDataDict(List<GoapStateData> statesTargetList)
        {
            // Aux
            GoapStateDataDict newGoapStateDataDict = new GoapStateDataDict();
            Dictionary<string, int> targetStatesDict = new Dictionary<string, int>();

            // Ads each state data on list into new dictionary
            foreach(var goalTargetState in statesTargetList)
                targetStatesDict.Add(goalTargetState.Key, goalTargetState.Value);
            newGoapStateDataDict.StateDict = targetStatesDict;
            
            return newGoapStateDataDict;
        }
    }

    /** Store any world state. E.g. Player being alive is used as a world state to trigger especific actions. */
    public class GoapWorldManager
    {
        // Non-Serializable Fields ----------------------------------------
        private static readonly GoapWorldManager m_goapWorldInstance = new GoapWorldManager();
        private GoapStateDataDict m_worldStates = new GoapStateDataDict(); // Uses dict to store states

        // Properties ----------------------------------------------------
        public static GoapWorldManager GoapWorldInstance => m_goapWorldInstance;

        // Public Methods ---------------------------------------------------
        public GoapStateDataDict GetWorldStates()
        {
            return m_worldStates;
        }
    }
}
