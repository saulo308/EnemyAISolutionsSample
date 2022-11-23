using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CharacterModule
{
    public abstract class ACharacterAnimatorControllerBase : MonoBehaviour
    {
        // Serializable Fields --------------------------
        [SerializeField] private Animator m_characterAnimator = null;

        // Unity Methods ------------------------------------
        protected virtual void Awake()
        {
            if(m_characterAnimator == null)
                m_characterAnimator = GetComponent<Animator>();
        }

        // Public Methods -----------------------------------
        protected void SetAnimatorInteger(string name, int value)
        {
            m_characterAnimator.SetInteger(name,value);
        }

        protected void SetAnimatorFloat(string name, float value)
        {
            m_characterAnimator.SetFloat(name,value);
        }

        protected void SetAnimatorBool(string name, bool value)
        {
            m_characterAnimator.SetBool(name,value);
        }

        protected void SetAnimatorTrigger(string name)
        {
            m_characterAnimator.SetTrigger(name);
        }
    }
}
