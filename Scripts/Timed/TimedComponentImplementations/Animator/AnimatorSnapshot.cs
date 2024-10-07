using System.Collections.Generic;
// Original Authors - Wyatt Senalik

namespace Timed.TimedComponentImplementations
{
    public sealed class AnimatorSnapshot : ISnapshot<AnimatorSnapshotData, AnimatorSnapshot>
    {
        public float time => m_time;
        public AnimatorSnapshotData data => m_data;
        public eInterpolationOption interpolationOpt { get; set; } = eInterpolationOption.Linear;

        private readonly float m_time = -1.0f;
        private readonly AnimatorSnapshotData m_data = null;
        private readonly bool m_isDebugging = false;


        public AnimatorSnapshot(float occurTime, AnimatorSnapshotData snapData, eInterpolationOption interpolationOpt, bool isDebugging = false)
        {
            m_time = occurTime;
            m_data = snapData;
            this.interpolationOpt = interpolationOpt;

            m_isDebugging = isDebugging;
        }


        public AnimatorSnapshot Interpolate(AnimatorSnapshot other, float targetTime)
        {
            #region Asserts
            //CustomDebug.AssertIsTrueForObj(data.paramCount == other.data.paramCount, $"both snapshots to have the same paramCount when interpolating. This ({data.paramCount}). Other ({other.data.paramCount}).", this);
            #endregion Asserts
            SnapshotHelpers.GatherInterpolationInfo<AnimatorSnapshotData, AnimatorSnapshot>(this, other, targetTime, out AnimatorSnapshot t_earlierSnap, out AnimatorSnapshot t_laterSnap, out eInterpolationOption t_intOpt);
            AnimatorSnapshotData t_earlierData = t_earlierSnap.data;
            AnimatorSnapshotData t_laterData = t_laterSnap.data;
            float t_earlierTime = t_earlierSnap.time;
            float t_laterTime = t_laterSnap.time;

            // StateHash is always step.
            int t_stateHash;
            if (targetTime < t_laterSnap.time)
            {
                t_stateHash = t_earlierData.stateHash;
            }
            else
            {
                t_stateHash = t_laterData.stateHash;
            }

            // Interpolate Norm Time
            // Unsure the below worked
            //// Same state
            //if (t_earlierData.stateHash == t_laterData.stateHash)
            //{
            //    t_laterNormTime = t_laterData.normTime;
            //}
            //// Different state
            //else
            //{
            //    // Add 1 to other because it is the next state.
            //    t_laterNormTime = t_laterData.normTime + 1.0f;
            //}
            // Utilize FloatSnapshots for easy interpolation
            float t_normTime = FloatSnapshot.Interpolate(t_earlierData.normTime, t_earlierTime, t_laterData.normTime, t_laterTime, t_intOpt, targetTime);

            // Lerp parameters
            IReadOnlyList<float> t_earlierValues = t_earlierData.paramValues;
            IReadOnlyList<float> t_laterValues = t_laterData.paramValues;
            int t_paramCount = t_earlierValues.Count;
            float[] t_interpParamValues = new float[t_paramCount];
            for (int i = 0; i < t_paramCount; ++i)
            {
                // Utilize FloatSnapshots for easy interpolation
                t_interpParamValues[i] = FloatSnapshot.Interpolate(t_earlierValues[i], t_earlierTime, t_laterValues[i], t_laterTime, t_intOpt, targetTime);
            }

            AnimatorSnapshotData t_intData = new AnimatorSnapshotData(t_stateHash, t_normTime, t_interpParamValues, m_isDebugging);
            return new AnimatorSnapshot(targetTime, t_intData, t_intOpt, m_isDebugging);
        }
        public AnimatorSnapshot Clone() => Clone(time);
        public AnimatorSnapshot Clone(float timeForClone) => new AnimatorSnapshot(timeForClone, data, interpolationOpt, m_isDebugging);


        public override string ToString()
        {
            return $"time:{time}; data:{data}; interpolationOpt:{interpolationOpt}";
        }
    }
}