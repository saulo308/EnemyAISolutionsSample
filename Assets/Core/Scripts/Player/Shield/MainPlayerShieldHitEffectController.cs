using System.Collections;
using System.Collections.Generic;
using CharacterModule;
using GameSharedEventModule;
using DG.Tweening;
using UnityEngine;

namespace AIProject.GameModule
{
    public class MainPlayerShieldHitEffectController : MonoBehaviour
    {
        // Serializable Fields ---------------------------------------------------
        [Header("GeneralConfig")]
        [SerializeField] private SpriteRenderer m_spellSpriteRenderer = null;
        [SerializeField] private float m_destroyDelay = 0.1f;

        [Header("SharedEvents")]
        [SerializeField] private GameSharedDataEvent<AnimationEvent> m_shieldHitSharedEventAnimationEvent = null;

        // Unity Methods --------------------------------------------------------
        void Awake()
        {
            // Execute fade in
            m_spellSpriteRenderer.DOFade(0,0f);
            m_spellSpriteRenderer.DOFade(1,0.1f);

            // Bind animationEvent
            m_shieldHitSharedEventAnimationEvent.AddListener(OnAnimationEventTrigerred);
        }

        void OnDestroy()
        {
            // Unbind animationEvent
            m_shieldHitSharedEventAnimationEvent.RemoveListener(OnAnimationEventTrigerred);
        }

        // Event Handlers ---------------------------------------------------------------
        void OnAnimationEventTrigerred(AnimationEvent triggeredAnimationEvent)
        {
            if(triggeredAnimationEvent.stringParameter.Equals("OnAnimationFinished"))
            {
                // Execute fade out and destroy on completed
                _ = DOVirtual.DelayedCall(m_destroyDelay,()=>
                {
                    m_spellSpriteRenderer.DOFade(0,0.1f)
                        .OnComplete(() => Destroy(gameObject));
                });
            }
        }
    }
}

