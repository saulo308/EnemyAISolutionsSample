using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
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
        [SerializeField] private float m_endActionDelay = 0f;

        [Header("GOAPActionBase - States")]
        [SerializeField] private List<GoapStateData> m_actionPreconditionsData = new List<GoapStateData>();
        [SerializeField] private List<GoapStateData> m_actionEffectsData = new List<GoapStateData>();

        // Non-Serializable Fields -------------------------------------------------
        private GoapStateDataDict m_actionPreconditions = new GoapStateDataDict();
        private GoapStateDataDict m_actionEffects = new GoapStateDataDict();

        protected bool m_isActionComplete = false;
        protected bool m_isActionPerfoming = false;

        // Properties -----------------------------------------------------
        public string ActionName => m_actionName;
        public float ActionCost => m_actionCost;
        public GameObject ActionTarget => m_actionTarget;
        public float EndActionDelay => m_endActionDelay;

        public GoapStateDataDict ActionEffects => m_actionEffects;

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

        public virtual void OnActionComplete()
        {
            m_isActionComplete = true;
            DOVirtual.DelayedCall(m_endActionDelay, () => m_isActionPerfoming = false);
        }

        public abstract bool IsActionUsable(AGoapAgent goapAgent);

        public abstract bool RequiresRangeToExecute();
        public virtual bool IsInRangeToExecute() => true;

	    public virtual bool Perform()
        {
            m_isActionPerfoming = true;
            return true;
        }

	    public virtual bool IsActionComplete() => m_isActionComplete;
        public virtual bool IsActionPerforming() => m_isActionPerfoming;

        public virtual void ResetAction()
        {
            m_isActionComplete = false;
        }

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
