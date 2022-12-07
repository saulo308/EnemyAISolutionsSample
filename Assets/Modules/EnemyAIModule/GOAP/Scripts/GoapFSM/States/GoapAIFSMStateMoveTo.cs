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
            bool bReachedActionRange = goapAgent.MoveAgentToActionRange();
        }
    }
}

