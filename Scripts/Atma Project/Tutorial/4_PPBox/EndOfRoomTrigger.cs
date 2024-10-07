using UnityEngine;

using NaughtyAttributes;

using Helpers.Events;
// Original Authors - Wyatt Senalik

namespace Atma.Tutorial.PPBox
{
    /// <summary>
    /// Invokes serialized event when the player reaches the collider and they have met the door open condition.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class EndOfRoomTrigger : MonoBehaviour
    {
        public IEventPrimer onEndOfRoomReached => m_onEndOfRoomReached;

        [SerializeField, Required] private Door m_endDoor = null;
        [SerializeField, Tag] private string m_playerTag = "Player";

        [SerializeField] private MixedEvent m_onEndOfRoomReached = new MixedEvent();

        private bool m_alreadyInvoked = false;


        private void Awake()
        {
            #region Asserts
            //CustomDebug.AssertSerializeFieldIsNotNull(m_endDoor, nameof(m_endDoor), this);
            #endregion Asserts
        }


        private void OnTriggerEnter2D(Collider2D other)
        {
            HandlePhysics2DContact(other);
        }
        private void OnTriggerStay2D(Collider2D other)
        {
            HandlePhysics2DContact(other);
        }


        private void HandlePhysics2DContact(Collider2D other)
        {
            // Already called
            if (m_alreadyInvoked) { return; }
            // Wrong tag.
            if (!other.CompareTag(m_playerTag)) { return; }
            // Room win condition not met.
            if (!m_endDoor.isOn) { return; }

            m_onEndOfRoomReached.Invoke();
            m_alreadyInvoked = true;
        }
    }
}