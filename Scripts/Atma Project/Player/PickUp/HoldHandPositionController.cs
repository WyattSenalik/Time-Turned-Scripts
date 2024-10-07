using UnityEngine;

using NaughtyAttributes;

using Timed;
// Original Authors - Wyatt Senalik

namespace Atma
{
    /// <summary>
    /// Controls the position of the hold hand when holding an item or throwing it.
    /// 
    /// Records windows of if the throw animation should be playing or not (and the object being thrown).
    /// </summary>
    [RequireComponent(typeof(PickupController))]
    [DisallowMultipleComponent]
    public sealed class HoldHandPositionController : WindowRecorder<ThrowWindowData>
    {
        public PickupController pickupCont { get; private set; }
        public bool isPlayingWindupAnim
        {
            get
            {
                WindowData<ThrowWindowData> t_recentWindow = timeAwareMostRecentWindow;
                if (t_recentWindow == null)
                {
                    return false;
                }
                ThrowWindowData t_windowData = t_recentWindow.data;
                if (t_windowData == null)
                {
                    return false;
                }
                return t_windowData.isThrowing;
            }
        }
        public IPickUpObject throwingObj => timeAwareMostRecentWindow.data.thrownObj;

        [SerializeField, Required] private Transform m_holdHandTrans = null;
        [SerializeField, Required] private Transform m_pickupHoldCenterPosTrans = null;


        protected override void Awake()
        {
            base.Awake();

            pickupCont = GetComponent<PickupController>();
            #region Asserts
            //CustomDebug.AssertComponentIsNotNull(pickupCont, this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_holdHandTrans, nameof(m_holdHandTrans), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_pickupHoldCenterPosTrans, nameof(m_pickupHoldCenterPosTrans), this);
            #endregion Asserts
        }
        private void Start()
        {
            ToggleSubscriptions(true);

            // Not throwing by default.
            StartNewWindow(new ThrowWindowData());
        }
        private void OnDestroy()
        {
            ToggleSubscriptions(false);
        }
        private void LateUpdate()
        {
            // Done in late update to override position on replay when holding an object
            UpdatePosition();
            UpdateScale();
        }


        private void ToggleSubscriptions(bool cond)
        {
            pickupCont?.onThrow.ToggleSubscription(OnPickUpThrown, cond);
        }
        private void OnPickUpThrown(IPickUpObject thrownObj)
        {
            EndCurrentWindow();
            StartNewWindow(new ThrowWindowData(thrownObj));
        }
        private void UpdatePosition()
        {
            // Hold Hand
            if (pickupCont.isCarrying)
            {
                IPickUpObject t_curCarriedObj = pickupCont.curCarriedObj;
                m_holdHandTrans.position = t_curCarriedObj.handPlacementPosition;
            }
            // Otherwise, we might be throwing
            else
            {
                #region Asserts
                //CustomDebug.AssertIsTrueForComponent(HasCurrentWindow(), $"a current window to exist at time {curTime}.", this);
                #endregion Asserts
                ThrowWindowData t_recentData = timeAwareMostRecentWindow.data;
                // If throwing
                if (t_recentData.isThrowing)
                {
                    float t_animStartTime = timeAwareMostRecentWindow.window.startTime;
                    float t_animThrowEndTime = t_animStartTime + t_recentData.windUpAnimLength;
                    if (curTime < t_animThrowEndTime)
                    {
                        // Should play more of the throw anim
                        IPickUpObject t_thrownObj = t_recentData.thrownObj;
                        m_holdHandTrans.position = t_thrownObj.handPlacementPosition;
                    }
                    else if (isRecording)
                    {
                        // Should end this window (only if recording)
                        EndCurrentWindow();
                        StartNewWindow(new ThrowWindowData());
                    }
                }
            }
        }
        private void UpdateScale()
        {
            m_holdHandTrans.localScale = transform.localScale;
        }
    }

    public sealed class ThrowWindowData
    {
        public bool isThrowing { get; private set; } = false;
        public IPickUpObject thrownObj { get; private set; } = null;

        public bool hasThrowBehav { get; private set; } = false;
        public ThrowBehavior throwBehav { get; private set; } = null;
        public float windUpAnimLength
        {
            get
            {
                if (!m_hasCheckedForThrownBehav)
                {
                    hasThrowBehav = thrownObj.TryGetComponent(out ThrowBehavior t_throwBehav);
                    throwBehav = t_throwBehav;
                    m_hasCheckedForThrownBehav = true;
                }
                return hasThrowBehav ? throwBehav.windupAnimLength : 0.0f;
            }
        }

        private bool m_hasCheckedForThrownBehav = false;


        public ThrowWindowData()
        {
            isThrowing = false;
            thrownObj = null;
        }
        public ThrowWindowData(IPickUpObject thrownObj)
        {
            isThrowing = true;
            this.thrownObj = thrownObj;
        }
    }
}