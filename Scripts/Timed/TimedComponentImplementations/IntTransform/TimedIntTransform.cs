using UnityEngine;

using Helpers.Extensions;
using Helpers.Physics.Custom2DInt;

namespace Timed.TimedComponentImplementations
{
    [RequireComponent(typeof(Int2DTransform))]
    public sealed class TimedIntTransform : MonoBehaviour
    {
        public bool isRecording => m_timedPosX.isRecording;
        public Int2DTransform intTransform => m_intTransform;
        public TimedInt timedPosX => m_timedPosX;
        public TimedInt timedPosY => m_timedPosY;
        public TimedInt timedAngle => m_timedAngle;
        public TimedInt timedSizeX => m_timedSizeX;
        public TimedInt timedSizeY => m_timedSizeY;
        public bool shouldRecordRotation => m_recordRotation;
        public bool shouldRecordSize => m_recordSize;

        [SerializeField] private bool m_recordEveryNewSnap = false;
        [SerializeField] private bool m_recordRotation = true;
        [SerializeField] private bool m_recordSize = true;

        private Int2DTransform m_intTransform = null;

        private TimedInt m_timedPosX = null;
        private TimedInt m_timedPosY = null;

        private TimedInt m_timedAngle = null;

        private TimedInt m_timedSizeX = null;
        private TimedInt m_timedSizeY = null;

        private bool m_isFixedUpdateDirty = false;


        private void Awake()
        {
            m_intTransform = this.GetComponentSafe<Int2DTransform>();
        }
        private void Start()
        {
            m_timedPosX = new TimedInt(m_intTransform.position.x, eInterpolationOption.Linear, shouldRecordEverySnap: m_recordEveryNewSnap);
            m_timedPosY = new TimedInt(m_intTransform.position.y, eInterpolationOption.Linear, shouldRecordEverySnap: m_recordEveryNewSnap);

            if (m_recordRotation)
            {
                m_timedAngle = new TimedInt(m_intTransform.localAngle, eInterpolationOption.Linear, shouldRecordEverySnap: m_recordEveryNewSnap);
            }

            if (m_recordSize)
            {
                m_timedSizeX = new TimedInt(m_intTransform.localSize.x, eInterpolationOption.Linear, shouldRecordEverySnap: m_recordEveryNewSnap);
                m_timedSizeY = new TimedInt(m_intTransform.localSize.y, eInterpolationOption.Linear, shouldRecordEverySnap: m_recordEveryNewSnap);
            }
        }
        private void Update()
        {
            if (isRecording)
            {
                UpdateSingleTimedVariable(m_timedPosX, m_intTransform.localPosition.x);
                UpdateSingleTimedVariable(m_timedPosY, m_intTransform.localPosition.y);
                if (m_recordRotation)
                {
                    UpdateSingleTimedVariable(m_timedAngle, m_intTransform.localAngle);
                }
                if (m_recordSize)
                {
                    UpdateSingleTimedVariable(m_timedSizeX, m_intTransform.localSize.x);
                    UpdateSingleTimedVariable(m_timedSizeY, m_intTransform.localSize.y);
                }
            }
            else
            {
                m_intTransform.localPosition = new Vector2Int(m_timedPosX.curData, m_timedPosY.curData);
                if (m_recordRotation)
                {
                    m_intTransform.localAngle = m_timedAngle.curData;
                }
                if (m_recordSize)
                {
                    m_intTransform.localSize = new Vector2Int(m_timedSizeX.curData, m_timedSizeY.curData);
                }
            }

            m_isFixedUpdateDirty = false;
        }
        private void FixedUpdate()
        {
            m_isFixedUpdateDirty = true;
        }


        private void UpdateSingleTimedVariable(TimedInt timedVar, int newValue)
        {
            // In some cases, we don't want to set curData every frame.
            // For example, if we are storing transform data and moving via physics we only want to set curData every FixedUpdate.
            // BUT if the data is being changed BOTH via physics AND via update calls, then we want to set curData every FixedUpdate OR if the data has been changed.
            bool t_shouldSet;
            if (newValue == timedVar.curData)
            {
                // Data is equal to last one, so only want to set if fixed upate is dirty.
                t_shouldSet = m_isFixedUpdateDirty;
            }
            else
            {
                // Data is not equal, so we want to set.
                t_shouldSet = true;
            }

            if (t_shouldSet)
            {
                timedVar.curData = newValue;
            }

            if (!timedVar.isRecording)
            {
                //CustomDebug.LogErrorForComponent($"Tried to set a timed variable when not recording", this);
            }
        }

        public void ClearAllRecordedData()
        {
            ClearSingleTimedVariableRecordedData(timedPosX, m_intTransform.localPosition.x);
            ClearSingleTimedVariableRecordedData(timedPosY, m_intTransform.localPosition.y);
            if (m_recordRotation)
            {
                ClearSingleTimedVariableRecordedData(timedAngle, m_intTransform.angle);
            }
            if (m_recordSize)
            {
                ClearSingleTimedVariableRecordedData(timedSizeX, m_intTransform.localSize.x);
                ClearSingleTimedVariableRecordedData(timedSizeY, m_intTransform.localSize.y);
            }
        }
        private void ClearSingleTimedVariableRecordedData(TimedInt timedVar, int newVal)
        {
            timedVar.ClearAllRecordedData();
            timedVar.curData = newVal;
        }
    }
}