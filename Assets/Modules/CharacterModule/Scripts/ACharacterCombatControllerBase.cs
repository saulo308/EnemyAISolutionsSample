using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using AIProject.GameModule;
using GameSharedEventModule;
using UnityEngine;

namespace CharacterModule
{
    // Enums ----------------------------------------------------------
    [System.Serializable]
    public enum EEnemyAttackType
    {
        Melee,
        Cast
    }
    
    public abstract class ACharacterCombatControllerBase : MonoBehaviour
    {
        // Serializable Fields --------------------------------------------
        [Header("CombatBase - GeneralConfig")]
        [SerializeField] private float m_characterMaxHealth = 100;
        [SerializeField] private bool m_destroyOnDead = true;

        [Header("CombatBase - LinkedRefs")]
        [SerializeField] protected GameObject m_characterContainer;
        
        [Header("CombatBase - SharedEvents")]
        [SerializeField] private GameSharedDataEvent<AnimationEvent> m_characterSharedEventAnimationEvent = null;
        [SerializeField] private GameSharedEvent m_characterHurtEvent;
        [SerializeField] private GameSharedEvent m_characterEndLifeEvent;
        [SerializeField] private GameSharedEvent m_characterDeadEvent;
        [SerializeField] private GameSharedDataEvent<float> m_characterHealthPercentageEvent;

        // Non-Serializable Fields -----------------------------------------
        [SerializeField] private float m_characterCurHealth;

        // Life flags
        protected bool m_isCharacterHurt = false;
        protected bool m_isCharacterDead = false;

        // Properties ----------------------------------------------------
        public float CharacterCurrentHealth => m_characterCurHealth;
        public bool IsCharacterHurt => m_isCharacterHurt;
        public bool IsCharacterDead => m_isCharacterDead;

        // Unity Methods -------------------------------------------------
        protected virtual void Awake()
        {
            // Set curHealth and Update health percentage
            m_characterCurHealth = m_characterMaxHealth;
            m_characterHealthPercentageEvent.SharedDataValue = m_characterCurHealth / m_characterMaxHealth;

            // Bind event on animation event listener
            m_characterSharedEventAnimationEvent.AddListener(OnAnimationEventTrigerred);
        }

        void OnDestroy()
        {
            m_characterSharedEventAnimationEvent.RemoveListener(OnAnimationEventTrigerred);
        }

        // Public Methods -------------------------------------------------------
        public virtual void TakeDamage(float damageAmount)
        {
            if(m_isCharacterDead) return;

            // Remove character health
            RemoveLife(damageAmount);

            // Update health percentage
            m_characterHealthPercentageEvent.SharedDataValue = m_characterCurHealth / m_characterMaxHealth;

            // Call hurt feedback
            OnCharacterHurt();
        }

        public virtual void TakeDamage(float damageAmount, EEnemyAttackType attackType)
        {
            TakeDamage(damageAmount);
        }

        public virtual void KillCharacter()
        {
            // Take damage equal to character max health to kill him
            TakeDamage(m_characterMaxHealth);
        }
        
        // Protected Methods ---------------------------------------------------------------------
        protected virtual void OnCharacterHurt()
        {
            // Dispatch hurt event to notify player animator
            m_characterHurtEvent.DispatchEvent();

            // Set flag to avoid other inputs
            m_isCharacterHurt = true;
        }

        protected virtual void OnCharacterDead()
        {
            // Dispatch hurt event to notify player animator
            m_characterEndLifeEvent.DispatchEvent();

            // Set flag to avoid other inputs
            m_isCharacterDead = true;
        }

        // Private Methods ---------------------------------------------------
        void RemoveLife(float lifeAmountToTake)
        {
            m_characterCurHealth -= lifeAmountToTake;

            // Check if character is dead
            if(m_characterCurHealth <= 0) 
            {
                m_characterCurHealth = 0;
                OnCharacterDead();
            }
        }

        // Event Handlers ---------------------------------------------------------------
        protected virtual void OnAnimationEventTrigerred(AnimationEvent triggeredAnimationEvent)
        {
            if(triggeredAnimationEvent.stringParameter.Equals("OnHurtEnd"))
            {
                // Reset flag to re-enable inputs
                m_isCharacterHurt = false;
            }

            if(triggeredAnimationEvent.stringParameter.Equals("OnDeadEnd"))
            {
                // Dispatch onDead event
                m_characterDeadEvent.DispatchEvent();

                // Destroy gameObj
                if(m_destroyOnDead) Destroy(m_characterContainer);
            }
        }
    }
}
