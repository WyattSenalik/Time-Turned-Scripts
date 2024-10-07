using UnityEngine;

using NaughtyAttributes;

using Helpers;
using Timed;
// Original Authors - Bryce Cernohous-Schrader 
// Tweaked by Wyatt Senalik
//
// Jan. 10, 2023 - Used to have physics checks on here. I created a new script called PickupCollider to do them instead
// so that we could place it on the player and do the check there instead of in here. Benefit of that is that the clones
// will no longer make the pickup indicator appear.

namespace Atma
{
    [RequireComponent(typeof(IPickUpObject))]
    public class PickupIndicatorBehavior : TimedRecorder
    {
        [SerializeField, Required] private GameObject m_indicatorTextObj = null;

        private IPickUpObject m_pickUpObj = null;
        private readonly IDLibrary m_idLibrary = new IDLibrary();


        protected override void Awake()
        {
            base.Awake();

            m_pickUpObj = GetComponent<IPickUpObject>();
            #region Asserts
            //CustomDebug.AssertSerializeFieldIsNotNull(m_pickUpObj, nameof(m_pickUpObj), this);
            //CustomDebug.AssertIComponentIsNotNull(m_pickUpObj, this);
            #endregion Asserts
        }
        private void Start()
        {
            // Need to hide the indicator when the object is picked up.
            m_pickUpObj.onPickup.ToggleSubscription(UpdateIndicatorToggleState, true);
            m_pickUpObj.onReleased.ToggleSubscription(UpdateIndicatorToggleState, true);
        }
        private void OnDestroy()
        {
            m_pickUpObj.onPickup.ToggleSubscription(UpdateIndicatorToggleState, false);
            m_pickUpObj.onReleased.ToggleSubscription(UpdateIndicatorToggleState, false);
        }


        public override void SetToTime(float time)
        {
            base.SetToTime(time);
            if (!isRecording) { return; }

            // Now needs to be called at all times since you can't pick up when its being thrown so you can walk alongside it.
            UpdateIndicatorToggleState();
        }
        public override void OnRecordingStop(float time)
        {
            base.OnRecordingStop(time);

            UpdateIndicatorToggleState();
        }
        public override void OnRecordingResume(float time)
        {
            base.OnRecordingResume(time);

            UpdateIndicatorToggleState();
        }


        public int RequestEnableIndicator()
        {
            int t_id = m_idLibrary.CheckoutID();
            UpdateIndicatorToggleState();
            return t_id;
        }
        public void CancelEnableIndicatorRequest(int requestID)
        {
            m_idLibrary.ReturnID(requestID);
            UpdateIndicatorToggleState();
            // If all ids are not returned, turn off the indicator.
            m_indicatorTextObj.SetActive(!m_idLibrary.AreAllIDsReturned());
        }


        private void UpdateIndicatorToggleState()
        {
            // Active only when pickup is not held and not all IDs are returned (aka someone has requested this be on) and we are recording.
            m_indicatorTextObj.SetActive(!m_pickUpObj.isHeld && m_pickUpObj.canBePickedUp && !m_idLibrary.AreAllIDsReturned() && isRecording);
        }
    }
}
