using UnityEngine;

using NaughtyAttributes;

using Timed;
// Original Authors - Wyatt Senalik

namespace Atma
{
    [DisallowMultipleComponent]
    public sealed class DealDamageOnContact : MonoBehaviour
    {
        private const bool IS_DEBUGGING = false;

        [SerializeField, Tag] private string m_playerTag = "Player";
        [SerializeField, Tag] private string m_cloneTag = "Clone";

        [SerializeField, Required] private GameObject m_rootDamagingObject = null;
        [SerializeField, Required] private TimedObject m_timedObj = null;
        [SerializeField] private bool m_dealDmgOnlyWhenRecording = true;


        private void Awake()
        {
            #region Asserts
            //CustomDebug.AssertSerializeFieldIsNotNull(m_rootDamagingObject, nameof(m_rootDamagingObject), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_timedObj, nameof(m_timedObj), this);
            #endregion Asserts
        }


        private void OnTriggerEnter2D(Collider2D collision)
        {
            DealDamageToCollision(collision);
        }
        private void OnTriggerStay2D(Collider2D collision)
        {
            DealDamageToCollision(collision);
        }


        private void DealDamageToCollision(Collider2D collision)
        {
            if (m_dealDmgOnlyWhenRecording)
            {
                // Do nothing if not recording.
                if (!m_timedObj.isRecording)
                { return; }
            }

            GameObject t_collisionObjRoot = collision.gameObject;
            #region Logs
            //CustomDebug.LogForComponent($"Collided with {t_collisionObjRoot.name}", this, IS_DEBUGGING);
            #endregion Logs
            if (collision.attachedRigidbody != null)
            {
                t_collisionObjRoot = collision.attachedRigidbody.gameObject;
            }
            if (!t_collisionObjRoot.CompareTag(m_playerTag) &&
                !t_collisionObjRoot.CompareTag(m_cloneTag))
            {
                // Is neither tag we want.
                return;
            }

            IHealth t_playerHealth = t_collisionObjRoot.GetComponent<IHealth>();
            if (!t_playerHealth.isDead)
            {
                #region Asserts
                //CustomDebug.AssertIComponentOnOtherIsNotNull(t_playerHealth, t_collisionObjRoot, this);
                #endregion Asserts
                t_playerHealth.TakeDamage(new DamageContext(m_timedObj.curTime, m_rootDamagingObject));
                #region Logs
                //CustomDebug.LogForComponent($"Killed {t_playerHealth.name}", this, IS_DEBUGGING);
                #endregion Logs
            }
            else
            {
                #region Logs
                //CustomDebug.LogForComponent($"{t_playerHealth.name} is already dead.", this, IS_DEBUGGING);
                #endregion Logs
            }
        }
    }
}