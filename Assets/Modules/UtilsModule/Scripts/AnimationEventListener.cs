using System.Collections;
using System.Collections.Generic;
using GameSharedEventModule;
using UnityEngine;

namespace UtilsModule
{
    public class AnimationEventListener : MonoBehaviour
    {
        // Serializable Fields -----------------------------------------------------------------
        [SerializeField] private GameSharedDataEventAnimationEvent OnAnimationEventTrigger;

        // Public Methods -------------------------------------------------------------------
        public void OnAnimationTrigger(AnimationEvent animationEvent)
        {
            OnAnimationEventTrigger.SharedDataValue = animationEvent;
        }
    }
}
