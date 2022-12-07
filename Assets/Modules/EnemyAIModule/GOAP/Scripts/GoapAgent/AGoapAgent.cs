using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EnemyAIModule.GOAP
{
    [System.Serializable]
    public class AgentGoal
    {
        public string AgentGoalName = "";
        public int GoalPriority = 1;
        public bool IsPersistent = true;
        public List<GoapStateData> GoalTargetStates = new List<GoapStateData>();

        public GoapStateDataDict GetGoalTargetStatesAsStateDataDict()
        {
            return GoapStateDataDict.StateDataListToGoapStateDataDict(GoalTargetStates);
        }
    }

    public abstract class AGoapAgent : MonoBehaviour
    {
        // Serializable Fields -----------------------------------------------
        [SerializeField] private List<AGoapAction> m_agentActionList = new List<AGoapAction>();
        [SerializeField] private List<AgentGoal> m_agentGoalList = new  List<AgentGoal>();
        [SerializeField] private GoapAIFSM m_goapAgentFSM = null;

        // Non-Serializable Fields --------------------------------------------- 
        private GoapPlanner m_goapPlanner = new GoapPlanner();

        private AGoapAction m_currentAgentAction = null;
        private AgentGoal m_currentAgentGoal = null;

        private Queue<AGoapAction> m_currentActionQueue = new Queue<AGoapAction>();

        // Unity Methods -----------------------------------------------------------
        protected virtual void Awake()
        {
            if(m_agentActionList.Count == 0)
            {
                var agentActions = GetComponents<AGoapAction>();
                foreach(var agentAction in agentActions)
                    m_agentActionList.Add(agentAction);
            }

            // Init agent FSM
            m_goapAgentFSM.InitializeFSM(this);
        }

        // Public Methods ------------------------------------------------------------
        public virtual bool RequestNewAgentPlan()
        {
            // Request a new agent plan 
            m_currentActionQueue = m_goapPlanner.CreateNewAgentPlan(m_agentActionList,m_agentGoalList[0].GetGoalTargetStatesAsStateDataDict());
            
            // If no plan found, execute feedback and push idle state to FSM
            if(m_currentActionQueue == null)
            {
                // Execute feedback
                OnNoPlanFound();

                // Push Idle to stack so we can keep trying to calculate new plan
                m_goapAgentFSM.PopFSMStack();
                m_goapAgentFSM.PushFSMStack(m_goapAgentFSM.IdleState);
                return false;
            } 

            // Found new plan (m_currentActionQueue.Count > 0)
            // Start executing plan action by pushing ActionState to FSM
            m_goapAgentFSM.PopFSMStack();
            m_goapAgentFSM.PushFSMStack(m_goapAgentFSM.PerformActionState);
            return true;
        }

        public virtual bool PerformNextActionOnQueue()
        {
            // If no actions to execute anymore, return false and FSM will get a new plan
            if (m_currentActionQueue.Count == 0) 
            {
				m_goapAgentFSM.PopFSMStack();
				m_goapAgentFSM.PushFSMStack(m_goapAgentFSM.IdleState);
				return false;
			}

            // Get next action to execute
            AGoapAction actionToExecute = m_currentActionQueue.Peek();

            // Check if action requeires range
            bool bIsActionInRangeToExecute = actionToExecute.RequiresRangeToExecute() ? 
                actionToExecute.IsInRangeToExecute() : 
                true;

            // If not in range, move to it....
            if(!bIsActionInRangeToExecute)
            {
				m_goapAgentFSM.PushFSMStack(m_goapAgentFSM.MoveToState);
                return false;
            }

            // In range, try execute
            bool bDidActionPerformedSuccessfully = actionToExecute.Perform();

            // If action fails, abort plan
			if (!bDidActionPerformedSuccessfully) 
            {
				m_goapAgentFSM.ClearFSMStack();
				m_goapAgentFSM.PushFSMStack(m_goapAgentFSM.IdleState);
                return false;
			} 

            // Else, complete action
            actionToExecute.OnActionComplete();
			m_currentActionQueue.Dequeue();

            return true;
        }

        public virtual bool MoveAgentToActionRange()
        {
            // Get next action to execute
            AGoapAction actionToExecute = m_currentActionQueue.Peek();

            bool bReachedActionRange = MoveAgentToExecuteAction(actionToExecute);
            if(bReachedActionRange)
            {
                m_goapAgentFSM.PopFSMStack();
                return true;
            }

            return false;
        }

        // Protected Methods ---------------------------------------------------
        protected abstract bool MoveAgentToExecuteAction(AGoapAction actionToExecute);
        protected abstract void OnNoPlanFound();
    }
}
