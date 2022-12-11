using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EnemyAIModule.GOAP
{
    public class GoapAIFSMStateMoveTo : AGoapAIFSMStateBase
    {
        // Public Methods --------------------------------------------
        public override void UpdateState(AGoapAgent goapAgent)
        {
            // On 'MoveTo' state, moves GoapAgent to action's range so it can be peformed
            bool bReachedActionRange = goapAgent.MoveAgentToActionRange();
        }
    }
}

