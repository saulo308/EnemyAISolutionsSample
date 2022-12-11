using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EnemyAIModule.GOAP
{   
    // FSM state base that every state must inherit
    public abstract class AGoapAIFSMStateBase
    {
        public abstract void UpdateState(AGoapAgent goapAgent);
    }
}
