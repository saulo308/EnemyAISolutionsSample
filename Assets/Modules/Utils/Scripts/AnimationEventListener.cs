using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UtilsModule
{
    public class AnimationEventListener : MonoBehaviour
    {
        public System.Action<AnimationEvent> OnAnimationEventTrigger = delegate { };

        public void OnAnimationTrigger(AnimationEvent animationEvent)
        {
            OnAnimationEventTrigger.Invoke(animationEvent);
        }
    }
}
