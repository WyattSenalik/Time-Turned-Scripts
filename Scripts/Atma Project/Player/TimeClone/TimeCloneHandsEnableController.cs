using UnityEngine;

using NaughtyAttributes;
using Timed;
using Helpers.Extensions;
// Original Authors - Wyatt Senalik

namespace Atma
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(TimeClone))]
    [RequireComponent(typeof(TimeClonePickup))]
    [RequireComponent(typeof(TimeCloneTransform))]
    [RequireComponent(typeof(TimeCloneMoveDirections))]
    public sealed class TimeCloneHandsEnableController : HandsEnableController
    {
        public BoxPushingDetector boxPushingDetector => m_boxPushingDetector;
        public float curTime => timeMan.curTime;
        public bool wasRecording { get; private set; } = false;
        public WindowData<ThrowWindowData> timeAwareMostRecentWindow
        {
            get
            {
                if (timeMan.shouldRecord)
                {
                    return m_windupWindowCollection.mostRecentWindow;
                }
                m_windupWindowCollection.TryGetWindow(curTime, out WindowData<ThrowWindowData> t_window);
                return t_window;
            }
        }

        public GlobalTimeManager timeMan { get; private set; }
        public TimeClone clone { get; private set; }
        public TimeClonePickup pickupCont { get; private set; }
        public TimeCloneMoveDirections cloneMoveDirs { get; private set; }

        [SerializeField, Required] private BoxPushingDetector m_boxPushingDetector = null;

        /// <summary>Collection of windows for when the windup throw animation is being played.</summary>
        private readonly WindowCollection<ThrowWindowData> m_windupWindowCollection = new WindowCollection<ThrowWindowData>();

        [SerializeField, BoxGroup("Debugging"), ReadOnly] private bool m_hasEarlyDisappearance = false;
        [SerializeField, BoxGroup("Debugging"), ReadOnly] private bool m_isCarryingPickup = false;
        [SerializeField, BoxGroup("Debugging"), ReadOnly] private bool m_isPlayingWindupAnimation = false;
        [SerializeField, BoxGroup("Debugging"), ReadOnly] private bool m_isPlayerPushingBox = false;
        [SerializeField, BoxGroup("Debugging"), ReadOnly] private Vector2 m_mostRecentNonZeroMoveDirectionBeforeCurTime = Vector2.zero;
        [SerializeField, BoxGroup("Debugging"), ReadOnly] private PickUpObject m_currentCarriedObject = null;
        [SerializeField, BoxGroup("Debugging"), ReadOnly] private PickUpObject m_windupObject = null;


        protected override void Awake()
        {
            base.Awake();
            #region Asserts
            //CustomDebug.AssertSerializeFieldIsNotNull(m_boxPushingDetector, nameof(m_boxPushingDetector), this);
            #endregion Asserts

            clone = this.GetComponentSafe<TimeClone>();
            pickupCont = this.GetComponentSafe<TimeClonePickup>();
            cloneMoveDirs = this.GetComponentSafe<TimeCloneMoveDirections>();
        }
        private void Start()
        {
            timeMan = GlobalTimeManager.instance;
            ToggleSubscriptions(true);
            // Start the first window
            m_windupWindowCollection.Add(new WindowData<ThrowWindowData>(curTime, new ThrowWindowData()));
        }
        private void OnDestroy()
        {
            ToggleSubscriptions(false);
        }
        protected override void Update()
        {
            // Don't update the hands enabled state if the clone disappears before the cur time.
            if (!clone.HasEarlyDisappearanceBeforeOrAtTime(curTime))
            {
                base.Update();
            }

            UpdateWindupWindow();
            wasRecording = timeMan.shouldRecord;

            // Debugging
            m_hasEarlyDisappearance = clone.HasEarlyDisappearanceBeforeOrAtTime(curTime);
            m_isCarryingPickup = IsCarryingPickup();
            m_isPlayingWindupAnimation = IsPlayingWindupAnimation();
            m_isPlayerPushingBox = IsPlayerPushingBox();
            m_mostRecentNonZeroMoveDirectionBeforeCurTime = GetMostRecentNonZeroMoveDirectionBeforeCurTime();
            m_currentCarriedObject = GetCurrentCarriedObject() as PickUpObject;
            m_windupObject = GetWindupObject() as PickUpObject;
        }

        protected override bool IsCarryingPickup()
        {
            if (pickupCont.isCarrying)
            {
                bool t_isHat = pickupCont.curCarriedObj.releaseType == IPickUpObject.eReleaseType.Drop;
                if (t_isHat)
                {
                    return false;
                }
                return true;
            }
            return false;
        }
        protected override bool IsPlayingWindupAnimation()
        {
            WindowData<ThrowWindowData> t_window = timeAwareMostRecentWindow;
            if (t_window == null)
            {
                return false;
            }
            else if (t_window.data == null)
            {
                return false;
            }
            else
            {
                return t_window.data.isThrowing;
            }
        }
        protected override bool IsPlayerPushingBox() => boxPushingDetector.isABoxBeingPushed;
        protected override Vector2 GetMostRecentNonZeroMoveDirectionBeforeCurTime() => cloneMoveDirs.GetMostRecentNonZeroMoveDirectionBeforeTime(curTime);
        protected override IPickUpObject GetCurrentCarriedObject() => pickupCont.curCarriedObj;
        protected override IPickUpObject GetWindupObject()
        {
            if (timeAwareMostRecentWindow == null)
            {
                return null;
            }
            else if (timeAwareMostRecentWindow.data == null)
            {
                return null;
            }
            else
            {
                return timeAwareMostRecentWindow.data.thrownObj;
            }
        }
        protected override bool IsLoadingIntoLevel() => false;


        private void UpdateWindupWindow()
        {
            if (timeMan.shouldRecord)
            {
                // FakeOnResume
                if (!wasRecording)
                {
                    m_windupWindowCollection.RemoveAfterTime(curTime);
                }
                // If we are throwing
                WindowData<ThrowWindowData> t_recentWindow = m_windupWindowCollection.mostRecentWindow;
                if (m_windupWindowCollection.mostRecentWindow == null)
                {
                    return;
                }
                ThrowWindowData t_recentData = t_recentWindow.data;
                if (t_recentData.isThrowing)
                {
                    float t_windupAnimFinTime = t_recentWindow.window.startTime + t_recentData.windUpAnimLength;
                    if (curTime >= t_windupAnimFinTime)
                    {
                        // Finish the throw window
                        m_windupWindowCollection.mostRecentWindow.SetWindowEndTime(curTime);
                        m_windupWindowCollection.Add(new WindowData<ThrowWindowData>(curTime, new ThrowWindowData()));
                    }
                }
            }
        }
        private void ToggleSubscriptions(bool cond)
        {
            pickupCont?.onThrown.ToggleSubscription(OnCloneThrow, cond);
        }
        private void OnCloneThrow(IPickUpObject thrownObj)
        {
            #region Asserts
            //CustomDebug.AssertIsTrueForComponent(timeMan.shouldRecord, $"the clone to throw only while recording.", this);
            #endregion Asserts
            // End last window and start a new one.
            m_windupWindowCollection.mostRecentWindow.SetWindowEndTime(curTime);
            m_windupWindowCollection.Add(new WindowData<ThrowWindowData>(curTime, new ThrowWindowData(thrownObj)));
        }
    }
}