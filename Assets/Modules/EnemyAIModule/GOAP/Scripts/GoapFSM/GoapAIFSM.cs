using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EnemyAIModule.GOAP
{
    public class GoapAIFSM : MonoBehaviour
    {
        // Non-Serializalbe Fields -----------------------------------
        private bool m_boolCanRun = false;
        private AGoapAgent m_targetGoapAgent = null;

        private Stack<AGoapAIFSMStateBase> m_fsmStack = new Stack<AGoapAIFSMStateBase>();

        private GoapAIFSMStateIdle m_idleState = new GoapAIFSMStateIdle();
        private GoapAIFSMStatePerformAction m_performActionState = new GoapAIFSMStatePerformAction();
        private GoapAIFSMStateMoveTo m_moveToState = new GoapAIFSMStateMoveTo();

        // Properties ------------------------------------------------------------
        public GoapAIFSMStateIdle IdleState => m_idleState;
        public GoapAIFSMStatePerformAction PerformActionState => m_performActionState;
        public GoapAIFSMStateMoveTo MoveToState => m_moveToState;

        // Unity Methods -----------------------------------------------
        void Update()
        {
            // Every tick, update FSM current stack (FSM.peek())
            UpdateCurrentState();
        }

        // Public Methods -----------------------------------------------
        public void InitializeFSM(AGoapAgent targetGoapAgent)
        {
            // Set goapAgent that owns this FSM
            m_targetGoapAgent = targetGoapAgent;

            // Push first idle state and set flag to true so we can start updating FSM
            m_fsmStack.Push(m_idleState);
            m_boolCanRun = true;
        }

        // Helping functions to update FSM stack
        public AGoapAIFSMStateBase PeekFSMStack() => m_fsmStack.Peek();
        public void ClearFSMStack() =>  m_fsmStack.Clear();

        public void PushFSMStack(AGoapAIFSMStateBase stateToPush) =>  m_fsmStack.Push(stateToPush);
        public AGoapAIFSMStateBase PopFSMStack() =>  m_fsmStack.Pop();

        // Private Methods -------------------------------------------------
        void UpdateCurrentState()
        {
            // Test if FSM can run
            if(!m_boolCanRun) return;

            // Get state that is on first position (peek()) and update it's state with 'UpdateState()' call()
            AGoapAIFSMStateBase currentState = m_fsmStack.Peek();
		    if (currentState != null)
			    currentState.UpdateState(m_targetGoapAgent);
        }
    }
}
