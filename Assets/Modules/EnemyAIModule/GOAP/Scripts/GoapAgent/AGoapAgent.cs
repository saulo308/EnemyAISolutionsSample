using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace EnemyAIModule.GOAP
{
    /** Stores a single agent goal data */
    [System.Serializable]
    public class AgentGoal
    {
        public string AgentGoalName = "";
        public int GoalPriority = 1;
        public List<GoapStateData> GoalTargetStates = new List<GoapStateData>(); // States that this goal needs to meet to consider achieved

        public GoapStateDataDict GetGoalTargetStatesAsStateDataDict() =>
            GoapStateDataDict.StateDataListToGoapStateDataDict(GoalTargetStates);
    }

    /** Represent a GOAP agent that will have actions, goals and a plan to execute and reach given goal */
    public abstract class AGoapAgent : MonoBehaviour
    {
        // Serializable Fields -----------------------------------------------
        [SerializeField] private List<AGoapAction> m_agentActionList = new List<AGoapAction>();
        [SerializeField] protected List<AgentGoal> m_agentGoalList = new  List<AgentGoal>();
        [SerializeField] private GoapAIFSM m_goapAgentFSM = null;

        // Non-Serializable Fields --------------------------------------------- 
        private GoapPlanner m_goapPlanner = new GoapPlanner();
        private Queue<AGoapAction> m_currentActionQueue = new Queue<AGoapAction>();

        // Unity Methods -----------------------------------------------------------
        protected virtual void Awake()
        {
            // Alternatively, GoapAction could be on the same script as this GoapAgent (though I do not recommend)
            // Get all actions on this object on Awake
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
            // Get new agent goal (this will go through all agent's and get the highest priority goal)
            AgentGoal newAgentGoal = GetNewAgentGoal();
            if(newAgentGoal == null) return false;

            // Get target states that fulfills chosen agens goal
            GoapStateDataDict agentGoalTargetStates = newAgentGoal.GetGoalTargetStatesAsStateDataDict();

            // Request a new agent plan to goapPlanner
            m_currentActionQueue = m_goapPlanner.CreateNewAgentPlan(this, m_agentActionList, agentGoalTargetStates);
            
            // If no plan found, execute feedback and push idle state to FSM
            if(m_currentActionQueue == null)
            {
                // Execute feedback
                OnNoPlanFound();

                // Push Idle to stack so we can keep trying to calculate new plan (IdleState calls request plan func)
                m_goapAgentFSM.PopFSMStack();
                m_goapAgentFSM.PushFSMStack(m_goapAgentFSM.IdleState);
                return false;
            } 

            // Found new plan (m_currentActionQueue.Count > 0)
            // Start executing plan action by pushing ActionState to FSM
            m_goapAgentFSM.ClearFSMStack();
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

            // If action is already performing, return false and FSM will keep calling this into action is done
            if(actionToExecute.IsActionPerforming()) return false;

            // If action is marked as complete and it's not performing anymore, then we can remove it from queue
            if(actionToExecute.IsActionComplete() && !actionToExecute.IsActionPerforming()) 
            {
                // This will eventually lead to (m_currentActionQueue.count == 0). Meaning we finished this plan's actions
                m_currentActionQueue.Dequeue();
                return true;
            }

            // Check if action requeires range to execute. If not, then just set it to true
            // If id does require range, check if we are in range to execute
            bool bIsActionInRangeToExecute = actionToExecute.RequiresRangeToExecute() ? 
                actionToExecute.IsInRangeToExecute() : 
                true;

            // If not in range, move to it GoapAgent to range
            // This is done by pushing a 'MoveTo' state to FSM
            if(!bIsActionInRangeToExecute)
            {
				m_goapAgentFSM.PushFSMStack(m_goapAgentFSM.MoveToState);
                return false;
            }

            // If in action's range, try to perform it
            bool bDidActionPerformedSuccessfully = actionToExecute.Perform();

            // If action fails, abort plan (IdleState will not calculate a new plan)
			if (!bDidActionPerformedSuccessfully) 
            {
				m_goapAgentFSM.ClearFSMStack();
				m_goapAgentFSM.PushFSMStack(m_goapAgentFSM.IdleState);
                return false;
			} 

            // If action succeded, mark action as complete 
            // (altough it might be still performing, keeping it on stack)
            actionToExecute.OnActionComplete();
            return true;
        }

        public virtual bool MoveAgentToActionRange()
        {
            // Get action that is being executed (FSM peek)
            AGoapAction actionToExecute = m_currentActionQueue.Peek();

            // Move agent to action's range into it has not yet reached it
            bool bReachedActionRange = MoveAgentToExecuteAction(actionToExecute);

            // If we get into action's range, we can pop this 'MoveTo' state from FSM stack
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

        // Private Methods -------------------------------------------------------
        /**
        * Goes through "m_agentGoalList" and get next plan for this goapAgent.
        * First, sort agentList by priority. Then, get goals with highest priority.
        * Then chooses randomly among the same (highest) priority goals
        */
        AgentGoal GetNewAgentGoal()
        {
            // If no goals are available, return null
            if(m_agentGoalList.Count == 0)
            {
                Debug.LogError("No goals for GoapAgent!");
                return null;
            }

            // If only one goal, just return it
            if(m_agentGoalList.Count == 1)
                return m_agentGoalList[0];

            // Sort goals using predicate
            m_agentGoalList.Sort((p1,p2)=>p2.GoalPriority.CompareTo(p1.GoalPriority));

            // Get highest priority found (first on sorted list)
            int highestPriority = m_agentGoalList[0].GoalPriority;

            // Foreach goal, get those with highest priority
            List<GoapStateDataDict> goalsWithHighestPriority = new List<GoapStateDataDict>();
            for(int i = 0; i < m_agentGoalList.Count; i++)
            {
                if(m_agentGoalList[i].GoalPriority == highestPriority)
                    goalsWithHighestPriority.Add(m_agentGoalList[i].GetGoalTargetStatesAsStateDataDict());
                else
                    break;
            }

            // From those with same (highest) priority, get one randomly
            int randomIdx = Random.Range(0,goalsWithHighestPriority.Count);

            // Return random goal
            return m_agentGoalList[randomIdx];
        }
    }
}
