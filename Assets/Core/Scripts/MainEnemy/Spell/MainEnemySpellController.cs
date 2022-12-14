using System.Collections;
using System.Collections.Generic;
using CharacterModule;
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
        [SerializeField] private float m_destroyDelay = 0.2f;

        [Header("SharedEvents")]
        [SerializeField] private GameSharedDataEvent<AnimationEvent> m_spellSharedEventAnimationEvent = null;

        // Non-Serializable Fields -------------------------------------------------
        private BoxCollider2D[] m_spellColliderList;

        private float m_spellDamage = 0f;

        private bool m_hasHitGround = false;
        private bool m_hasHitEnemy = false;

        // Unity Methods --------------------------------------------------------
        void Awake()
        {
            // Execute fade in
            m_spellSpriteRenderer.DOFade(0,0f);
            m_spellSpriteRenderer.DOFade(1,0.5f);

            // Bind animationEvent
            m_spellSharedEventAnimationEvent.AddListener(OnAnimationEventTrigerred);

            // Deactivate colliders
            m_spellColliderList = GetComponents<BoxCollider2D>();
            foreach(var boxCollider in m_spellColliderList)
                boxCollider.enabled = false; 
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

        // Event Handlers ---------------------------------------------------------------
        void OnAnimationEventTrigerred(AnimationEvent triggeredAnimationEvent)
        {
            if(triggeredAnimationEvent.stringParameter.Equals("OnSpellHitGround") && !m_hasHitGround)
            {
                // Set flag to avoid multiple calls
                m_hasHitGround = true;

                // When spell hits ground, activate colliders
                foreach(var boxCollider in m_spellColliderList)
                    boxCollider.enabled = true; 

                // Execute fade out and destroy on completed
                _ = DOVirtual.DelayedCall(m_destroyDelay,()=>
                {
                    m_spellSpriteRenderer.DOFade(0,0.2f)
                        .OnComplete(() => Destroy(gameObject));
                });
            }

            if(triggeredAnimationEvent.stringParameter.Equals("OnSpellFinished"))
            {
                // When spell finishes, deactivate colliders
                foreach(var boxCollider in m_spellColliderList)
                    boxCollider.enabled = false; 
            }
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            // Check if we collided with main player
            var hitTarget = other.GetComponentInChildren<MainPlayerCombatController>();
            if(!hitTarget) return;

            // If already hit player, return
            if(m_hasHitEnemy) return;

            // Call TakeDamage() on player
            m_hasHitEnemy = true;
            hitTarget.TakeDamage(m_spellDamage,EEnemyAttackType.Cast);
        }
    }
}
