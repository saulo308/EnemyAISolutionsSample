using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CharacterModule
{
    public abstract class ACharacterCombatControllerBase : MonoBehaviour
    {
        // Serializable Fields --------------------------------------------
        [Header("CombatBase - GeneralConfig")]
        [SerializeField] private float m_characterMaxHealth = 100;

        // Non-Serializable Fields -----------------------------------------
        private float m_characterCurHealth;

        // Properties ----------------------------------------------------
        public float CharacterCurrentHealth => m_characterCurHealth;

        // Unity Methods -------------------------------------------------
        protected virtual void Awake()
        {
            m_characterCurHealth = m_characterMaxHealth;
        }

        // Public Methods -------------------------------------------------------
        public virtual void TakeDamage(float damageAmount)
        {
            // Remove character health
            RemoveLife(damageAmount);

            // Call hurt feedback
            OnCharacterHurt();
        }

        public virtual void KillCharacter()
        {
            // Take damage equal to character max health to kill him
            TakeDamage(m_characterMaxHealth);
        }

        // Protected Methods ----------------------------------------------------
        protected virtual void OnCharacterHurt()
        { 
        }

        protected virtual void OnCharacterDead()
        {
        }

        // Private Methods ---------------------------------------------------
        void RemoveLife(float lifeAmountToTake)
        {
            m_characterCurHealth -= lifeAmountToTake;

            // Check if character is dead
            if(m_characterCurHealth < 0) 
            {
                m_characterCurHealth = 0;
                OnCharacterDead();
            }
        }
    }
}
