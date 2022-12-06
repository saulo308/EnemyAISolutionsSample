using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EnemyAIModule.GOAP
{
    public abstract class AGoapAction : MonoBehaviour
    {
        // Serializable Fields ------------------------------------------------
        [Header("GOAPActionBase - GeneralConfig")]
        [SerializeField] private string m_actionName = "";
        [SerializeField] private float m_actionCost = 0f;
        [SerializeField] private GameObject m_actionTarget = null;

        [Header("GOAPActionBase - States")]
        [SerializeField] private List<GoapStateData> m_actionPreconditionsData = new List<GoapStateData>();
        [SerializeField] private List<GoapStateData> m_actionEffectsData = new List<GoapStateData>();

        // Non-Serializable Fields -------------------------------------------------
        private GoapStateDataDict m_actionPreconditions = new GoapStateDataDict();
        private GoapStateDataDict m_actionEffects = new GoapStateDataDict();

        // Unity Methods -------------------------------------------------------------
        protected virtual void Awake()
        {
            m_actionPreconditions = ConstructStateDictFromStateList(m_actionPreconditionsData);
            m_actionEffects = ConstructStateDictFromStateList(m_actionEffectsData);
        }

        // Public Methods ----------------------------------------------------------------
        public bool DoesGivenStateFulfillPreconditions(GoapStateDataDict givenStates)
        {
            foreach(var preconditionStateData in m_actionPreconditions.StateDict)
            {
                if(!givenStates.StateDict.ContainsKey(preconditionStateData.Key))
                    return false;
            }
            return true;
        }

        public abstract bool PrePerform();
        public abstract bool Perform();
        public abstract bool PostPerform();

        // Private Methods ---------------------------------------------------------------
        GoapStateDataDict ConstructStateDictFromStateList(List<GoapStateData> goapStateList)
        {
            GoapStateDataDict newStateDict = new GoapStateDataDict();

            foreach(var state in goapStateList)
                newStateDict.StateDict.Add(state.Key,state.Value);

            return newStateDict;
        }
    }
}
