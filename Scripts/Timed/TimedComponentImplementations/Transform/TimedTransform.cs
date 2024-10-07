using UnityEngine;

using NaughtyAttributes;

using Helpers.Transforms;
using Helpers.UnityEnums;
// Original Authors - Wyatt Senalik

namespace Timed.TimedComponentImplementations
{
    /// <summary>
    /// Controls the transform this is attached to in order to allow for rewinding
    /// and going back in time.
    /// </summary>
    public sealed class TimedTransform : SnapshotRecorder<TransformDataSnapshot, TransformData>
    {
        [SerializeField] private bool m_isDebugging = false;
        [SerializeField] private eRelativeSpace m_syncSpace = eRelativeSpace.Local;
        [SerializeField] private bool m_useSerializedTransform = false;
        [SerializeField, Required, ShowIf(nameof(m_useSerializedTransform))] private Transform m_serializedTransform = null;

        private Transform m_transform = null;


        protected override void Awake()
        {
            base.Awake();
            if (m_useSerializedTransform)
            {
                m_transform = m_serializedTransform;
            }
            else
            {
                m_transform = transform;
            }
        }


        protected override TransformData GatherCurrentData()
        {
            return new TransformData(m_transform, m_syncSpace == eRelativeSpace.World);
        }
        protected override void ApplySnapshotData(TransformData snapData)
        {
            #region Logs
            //CustomDebug.LogForComponent($"Applying transform data ({snapData}) at time {curTime}.", this, m_isDebugging);
            #endregion Logs
            snapData.Apply(m_transform, m_syncSpace);
        }
        protected override TransformDataSnapshot CreateSnapshot(TransformData data, float time)
        {
            return new TransformDataSnapshot(time, data, eInterpolationOption.Linear);
        }

        protected override bool shouldObserveFixedUpdate => true;
    }
}