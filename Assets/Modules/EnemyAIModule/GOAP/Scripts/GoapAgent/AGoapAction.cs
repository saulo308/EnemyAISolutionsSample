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
        [SerializeField] private float m_endActionDelay = 0f; // Delay to execute after action has been executed

        [Header("GOAPActionBase - States")]
        [SerializeField] private List<GoapStateData> m_actionPreconditionsData = new List<GoapStateData>();
        [SerializeField] private List<GoapStateData> m_actionEffectsData = new List<GoapStateData>();

        // Non-Serializable Fields -------------------------------------------------
        private GoapStateDataDict m_actionPreconditions = new GoapStateDataDict();
        private GoapStateDataDict m_actionEffects = new GoapStateDataDict();

        protected bool m_isActionComplete = false;
        protected bool m_isActionPerfoming = false;

        private Tween m_endDelayTween = null;

        // Properties -----------------------------------------------------
        public string ActionName => m_actionName;
        public float ActionCost => m_actionCost;
        public GameObject ActionTarget => m_actionTarget;
        public float EndActionDelay => m_endActionDelay;

        public GoapStateDataDict ActionEffects => m_actionEffects;

        // Unity Methods -------------------------------------------------------------
        protected virtual void Awake()
        {
            // Note, for serialization, we use list to store data. But for implementation, it's easier (and better performance)
            // To use dictionaries instead.
            // Construct dictionaries from given list data
            m_actionPreconditions = ConstructStateDictFromStateList(m_actionPreconditionsData);
            m_actionEffects = ConstructStateDictFromStateList(m_actionEffectsData);
        }

        // Public Methods ----------------------------------------------------------------
        public bool DoesGivenStateFulfillPreconditions(GoapStateDataDict givenStates)
        {
            // Checks if 'givenStates' has all conditions needed for this actions execution (preconditions)
            foreach(var preconditionStateData in m_actionPreconditions.StateDict)
            {
                // If 'givenStates' does not have this precondition, then it does not fulfill all preconditions
                if(!givenStates.StateDict.ContainsKey(preconditionStateData.Key))
                    return false;
            }
            return true;
        }

        public virtual void OnActionComplete()
        {
            // set flag
            m_isActionComplete = true;

            // Start a delay tween with 'm_endActionDelay'. This will keep 'm_isActionPerfoming' to true
            // until delay is done
            // The effect of this is that the GoapAgent will keep this action on 'Action's queue' into it is still
            // performing and will not start performing the next action on it's queue
            if((m_endDelayTween != null) && m_endDelayTween.IsActive()) m_endDelayTween.Kill();
            m_endDelayTween = DOVirtual.DelayedCall(m_endActionDelay, () => m_isActionPerfoming = false);
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
