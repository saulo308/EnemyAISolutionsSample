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
        void Awake()
        {
            if(m_characterAnimator == null)
                m_characterAnimator = GetComponent<Animator>();
        }

        // Public Methods -----------------------------------
        public void SetAnimatorInteger(string name, int value)
        {
            m_characterAnimator.SetInteger(name,value);
        }

        public void SetAnimatorFloat(string name, float value)
        {
            m_characterAnimator.SetFloat(name,value);
        }

        public void SetAnimatorBool(string name, bool value)
        {
            m_characterAnimator.SetBool(name,value);
        }

        public void SetAnimatorTrigger(string name)
        {
            m_characterAnimator.SetTrigger(name);
        }
    }
}
