using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
// Original Authors - Wyatt Senalik and Aaron Duffey

namespace Helpers.UnityMessage
{
    public class InvokeUnityEventOnUnityMessage : MonoBehaviour
    {
        [SerializeField] private UnityEvent m_eventToInvoke = new UnityEvent();
        [SerializeField]
        private eUnityMessage m_messageToInvokeDuring = eUnityMessage.Start;

        [SerializeField] private bool m_limitCalls = true;
        [SerializeField, Min(0), ShowIf(nameof(m_limitCalls))]
        private int m_maxCalls = 1;
        [SerializeField, ShowIf(nameof(ShowUseTags))]
        private bool m_useTags = false;
        [SerializeField, Tag, ShowIf(nameof(ShowTags))]
        private string[] m_tags = new string[1];

        private int m_amountTimesInvoked = 0;


        private void Awake()
        {
            if (m_messageToInvokeDuring == eUnityMessage.Awake)
            {
                InvokeEvent();
            }
        }
        private void Start()
        {
            if (m_messageToInvokeDuring == eUnityMessage.Start)
            {
                InvokeEvent();
            }
        }
        private void OnEnable()
        {
            if (m_messageToInvokeDuring == eUnityMessage.OnEnable)
            {
                InvokeEvent();
            }
        }
        private void OnDisable()
        {
            if (m_messageToInvokeDuring == eUnityMessage.OnDisable)
            {
                InvokeEvent();
            }
        }
        private void OnDestroy()
        {
            if (m_messageToInvokeDuring == eUnityMessage.OnDestroy)
            {
                InvokeEvent();
            }
        }
        private void Reset()
        {
            if (m_messageToInvokeDuring == eUnityMessage.Reset)
            {
                InvokeEvent();
            }
        }
        private void Update()
        {
            if (m_messageToInvokeDuring == eUnityMessage.Update)
            {
                InvokeEvent();
            }
        }
        private void LateUpdate()
        {
            if (m_messageToInvokeDuring == eUnityMessage.LateUpdate)
            {
                InvokeEvent();
            }
        }
        private void FixedUpdate()
        {
            if (m_messageToInvokeDuring == eUnityMessage.FixedUpdate)
            {
                InvokeEvent();
            }
        }
        private void OnTriggerEnter(Collider other)
        {
            if (m_messageToInvokeDuring == eUnityMessage.OnTriggerEnter 
                && CheckTag(other.gameObject))
            {
                InvokeEvent();
            }
        }
        private void OnTriggerStay(Collider other)
        {
            if (m_messageToInvokeDuring == eUnityMessage.OnTriggerStay
                && CheckTag(other.gameObject))
            {
                InvokeEvent();
            }
        }
        private void OnTriggerExit(Collider other)
        {
            if (m_messageToInvokeDuring == eUnityMessage.OnTriggerExit
                && CheckTag(other.gameObject))
            {
                InvokeEvent();
            }
        }
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (m_messageToInvokeDuring == eUnityMessage.OnTriggerEnter2D
                && CheckTag(other.gameObject))
            {
                InvokeEvent();
            }
        }
        private void OnTriggerStay2D(Collider2D other)
        {
            if (m_messageToInvokeDuring == eUnityMessage.OnTriggerStay2D
                && CheckTag(other.gameObject))
            {
                InvokeEvent();
            }
        }
        private void OnTriggerExit2D(Collider2D other)
        {
            if (m_messageToInvokeDuring == eUnityMessage.OnTriggerExit2D
                && CheckTag(other.gameObject))
            {
                InvokeEvent();
            }
        }
        private void OnCollisionEnter(Collision other)
        {
            if (m_messageToInvokeDuring == eUnityMessage.OnCollisionEnter
                && CheckTag(other.gameObject))
            {
                InvokeEvent();
            }
        }
        private void OnCollisionStay(Collision other)
        {
            if (m_messageToInvokeDuring == eUnityMessage.OnCollisionStay
                && CheckTag(other.gameObject))
            {
                InvokeEvent();
            }
        }
        private void OnCollisionExit(Collision other)
        {
            if (m_messageToInvokeDuring == eUnityMessage.OnCollisionExit
                && CheckTag(other.gameObject))
            {
                InvokeEvent();
            }
        }
        private void OnCollisionEnter2D(Collision2D other)
        {
            if (m_messageToInvokeDuring == eUnityMessage.OnCollisionEnter2D
                && CheckTag(other.gameObject))
            {
                InvokeEvent();
            }
        }
        private void OnCollisionStay2D(Collision2D other)
        {
            if (m_messageToInvokeDuring == eUnityMessage.OnCollisionStay2D
                && CheckTag(other.gameObject))
            {
                InvokeEvent();
            }
        }
        private void OnCollisionExit2D(Collision2D other)
        {
            if (m_messageToInvokeDuring == eUnityMessage.OnCollisionExit2D
                && CheckTag(other.gameObject))
            {
                InvokeEvent();
            }
        }


        private void InvokeEvent()
        {
            if (m_limitCalls && m_amountTimesInvoked < m_maxCalls)
            {
                m_eventToInvoke.Invoke();
                ++m_amountTimesInvoked;
            }
        }
        private bool CheckTag(GameObject objectToCheck)
        {
            if (!m_useTags) { return false; }

            foreach (string t_tag in m_tags)
            {
                if (objectToCheck.CompareTag(t_tag))
                {
                    return true;
                }
            }

            return false;
        }

        #region Editor
        private bool ShowUseTags()
        {
            switch (m_messageToInvokeDuring)
            {
                case eUnityMessage.OnTriggerEnter: return true;
                case eUnityMessage.OnTriggerStay: return true;
                case eUnityMessage.OnTriggerExit: return true;
                case eUnityMessage.OnTriggerEnter2D: return true;
                case eUnityMessage.OnTriggerStay2D: return true;
                case eUnityMessage.OnTriggerExit2D: return true;
                case eUnityMessage.OnCollisionEnter: return true;
                case eUnityMessage.OnCollisionStay: return true;
                case eUnityMessage.OnCollisionExit: return true;
                case eUnityMessage.OnCollisionEnter2D: return true;
                case eUnityMessage.OnCollisionStay2D: return true;
                case eUnityMessage.OnCollisionExit2D: return true;
                default: return false;
            }
        }
        private bool ShowTags() => ShowUseTags() && m_useTags;
        #endregion Editor
    }
}
