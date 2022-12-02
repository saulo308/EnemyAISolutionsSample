using System.Collections;
using System.Collections.Generic;
using GameSharedEventModule;
using DG.Tweening;
using UnityEngine;

namespace AIProject.GameModule
{
    public class MainEnemySpellController : MonoBehaviour
    {
        // Serializable Fields ---------------------------------------------------
        [Header("GeneralConfig")]
        [SerializeField] private SpriteRenderer m_spellSpriteRenderer = null;
        [SerializeField] private BoxCollider2D m_spellCollider = null;
        [SerializeField] private float m_destroyDelay = 0.2f;

        [Header("SharedEvents")]
        [SerializeField] private GameSharedDataEvent<AnimationEvent> m_spellSharedEventAnimationEvent = null;

        // Non-Serializable Fields -------------------------------------------------
        private float m_spellDamage = 0f;
        private bool m_hasHitGround = false;

        // Unity Methods --------------------------------------------------------
        void Awake()
        {
            // Execute fade in
            m_spellSpriteRenderer.DOFade(0,0f);
            m_spellSpriteRenderer.DOFade(1,0.5f);

            // Bind animationEvent
            m_spellSharedEventAnimationEvent.AddListener(OnAnimationEventTrigerred);

            // Deactivate collider
            m_spellCollider.enabled = false; 
        }

        void OnDestroy()
        {
            // Unbind animationEvent
            m_spellSharedEventAnimationEvent.RemoveListener(OnAnimationEventTrigerred);
        }

        // Public Methods ----------------------------------------------------------
        public void SetupSpellData(float spellDamage)
        {
            m_spellDamage = spellDamage;
        }

        // Private Methods ----------------------------------------------------------

        // Event Handlers ---------------------------------------------------------------
        void OnAnimationEventTrigerred(AnimationEvent triggeredAnimationEvent)
        {
            if(m_hasHitGround) return;

            if(triggeredAnimationEvent.stringParameter.Equals("OnSpellHitGround"))
            {
                // Set flag to avoid multiple calls
                m_hasHitGround = true;

                // When spell hits ground, activate collider
                m_spellCollider.enabled = true;

                // Execute fade out and destroy on completed
                _ = DOVirtual.DelayedCall(m_destroyDelay,()=>
                {
                    m_spellSpriteRenderer.DOFade(0,0.2f)
                        .OnComplete(() => Destroy(gameObject));
                });
            }
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            var hitTarget = other.GetComponentInChildren<MainPlayerCombatController>();
            if(!hitTarget) return;

            hitTarget.TakeDamage(m_spellDamage);
        }
    }
}
