using UnityEngine;

using NaughtyAttributes;

using Helpers.Transforms;
using Helpers.UnityEnums;
// Original Authors - Wyatt Senalik

namespace Timed.TimedComponentImplementations
{
    public sealed class TimedOtherTransform : SnapshotRecorder<TransformDataSnapshot, TransformData>
    {
        [SerializeField, Required] private Transform m_transToTime = null;
        [SerializeField] private eRelativeSpace m_syncSpace = eRelativeSpace.Local;


        protected override void Awake()
        {
            base.Awake();

            #region Asserts
            //CustomDebug.AssertSerializeFieldIsNotNull(m_transToTime, nameof(m_transToTime), this);
            #endregion Asserts
        }

        protected override void ApplySnapshotData(TransformData snapData)
        {
            snapData.Apply(m_transToTime, m_syncSpace);
        }
        protected override TransformDataSnapshot CreateSnapshot(TransformData data, float time)
        {
            return new TransformDataSnapshot(time, data, eInterpolationOption.Linear);
        }
        protected override TransformData GatherCurrentData()
        {
            return new TransformData(m_transToTime, m_syncSpace == eRelativeSpace.World);
        }


        protected override bool shouldObserveFixedUpdate => true;
    }
}