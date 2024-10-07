using UnityEngine;

using NaughtyAttributes;

using Timed;
// Original Authors - Wyatt Senalik

namespace Atma
{
    /// <summary>
    /// Records when a ILeapObject is available to leap.
    /// </summary>
    [RequireComponent(typeof(SpringHatBehaviour))]
    public class SpringHatMomentRecorder : MomentRecorder<LeapAvailableMoment>
    {
        private const bool IS_DEBUGGING = false;

        [SerializeField, ValidateInput(nameof(GameObjectHasILeapObjectComponent), "Object must have an implementation of ILeapObject attached.")]
        private GameObject m_leapGameObj = null;
        private ILeapObject m_leapObj = null;
        private SpringHatBehaviour m_springHatBehav = null;

        private bool m_isLeapAvailLastFrame = false;


        protected override void Awake()
        {
            base.Awake();

            #region Asserts
            //CustomDebug.AssertSerializeFieldIsNotNull(m_leapGameObj, nameof(m_leapGameObj), this);
            #endregion Asserts
            m_leapObj = m_leapGameObj.GetComponent<ILeapObject>();
            m_springHatBehav = GetComponent<SpringHatBehaviour>();
            #region Asserts
            //CustomDebug.AssertIComponentOnOtherIsNotNull(m_leapObj, m_leapGameObj, this);
            //CustomDebug.AssertComponentIsNotNull(m_springHatBehav, this);
            #endregion Asserts
        }
        private void Start()
        {
            // Add a moment right away so that there is some default state.
            // Otherwise if rewound past the least recent moment, 
            AddMoment(new LeapAvailableMoment(curTime, m_leapObj.availableToUse, m_leapObj.availableToUse, m_leapObj));
            m_isLeapAvailLastFrame = m_leapObj.availableToUse;
        }

        public override void SetToTime(float time)
        {
            base.SetToTime(time);

            // Only add moments if recording.
            if (!isRecording) { return; }

            // Only create a new moment when the available to use changes.
            AddMomentIfAvailabilityHasChanged(time);
        }
        public override void OnRecordingResume(float time)
        {
            base.OnRecordingResume(time);

            // If spring hat behaviour is not currently being held (was held by
            // a time clone who disappeared). Then add a moment where it is not
            // available to use.
            if (!m_springHatBehav.isHeld)
            {
                AddMoment(new LeapAvailableMoment(time, false, m_isLeapAvailLastFrame, m_leapObj));
                m_isLeapAvailLastFrame = m_leapObj.availableToUse;
            }
            else
            {
                // Only create a new moment when the available to use changes.
                AddMomentIfAvailabilityHasChanged(time);
            }
            //CustomDebug.LogForComponent(nameof(OnRecordingResume), this, IS_DEBUGGING);
        }


        private void AddMomentIfAvailabilityHasChanged(float time)
        {
            // Only create a new moment when the available to use changes.
            if (m_isLeapAvailLastFrame != m_leapObj.availableToUse)
            {
                AddMoment(new LeapAvailableMoment(time, m_leapObj.availableToUse, m_isLeapAvailLastFrame, m_leapObj));
                m_isLeapAvailLastFrame = m_leapObj.availableToUse;
            }
        }

        #region Editor
        private bool GameObjectHasILeapObjectComponent(GameObject go)
        {
            if (go == null)
            {
                return false;
            }
            return go.GetComponent<ILeapObject>() != null;
        }
        #endregion Editor
    }
}