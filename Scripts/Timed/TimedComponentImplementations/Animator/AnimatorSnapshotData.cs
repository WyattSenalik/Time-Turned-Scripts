using System;
using System.Collections.Generic;
using UnityEngine;
// Original Authors - Wyatt Senalik

namespace Timed.TimedComponentImplementations
{
    public sealed class AnimatorSnapshotData : IEquatable<AnimatorSnapshotData>
    {
        public int stateHash => m_stateHash;
        public float normTime => m_normTime;
        public IReadOnlyList<float> paramValues => m_paramValues;
        public int paramCount => m_paramValues.Length;

        private readonly int m_stateHash = int.MinValue;
        private readonly float m_normTime = float.NaN;
        private readonly float[] m_paramValues = null;

        private bool m_isDebugging = false;


        public AnimatorSnapshotData(int curStateHash, float normalizedTime, float[] animParamValues)
        {
            m_stateHash = curStateHash;
            m_normTime = normalizedTime;
            m_paramValues = animParamValues;
        }
        public AnimatorSnapshotData(int curStateHash, float normalizedTime, float[] animParamValues, bool isDebugging) : this(curStateHash, normalizedTime, animParamValues)
        {
            m_isDebugging = isDebugging;
        }


        public bool Equals(AnimatorSnapshotData other)
        {
            if (stateHash != other.stateHash)
            {
                //CustomDebug.Log($"AnimatorData ({this}) NOT equal other ({other}). States are different.", m_isDebugging);
                return false;
            }
            float t_normTimeDiff = Mathf.Abs(normTime - other.normTime);
            if (t_normTimeDiff > 0.01f)
            {
                //CustomDebug.Log($"AnimatorData ({this}) NOT equal other ({other}). NormTimes are different.", m_isDebugging);
                return false;
            }
            if (m_paramValues.Length != other.m_paramValues.Length)
            {
                //CustomDebug.Log($"AnimatorData ({this}) NOT equal other ({other}). ParamLengths are different.", m_isDebugging);
                return false;
            }
            for (int i = 0; i < m_paramValues.Length; ++i)
            {
                float t_paramValDiff = Mathf.Abs(m_paramValues[i] - other.m_paramValues[i]);
                if (t_paramValDiff > 0.01f)
                {
                    //CustomDebug.Log($"AnimatorData ({this}) NOT equal other ({other}). Param {i}s are different.", m_isDebugging);
                    return false;
                }
            }

            //CustomDebug.Log($"AnimatorData ({this}) IS equal other ({other}).", m_isDebugging);
            return true;
        }

        public override string ToString()
        {
            string t_rtnStr = $"({nameof(stateHash)}:{stateHash}; {nameof(normTime)}:{normTime}; {nameof(paramValues)}:[";
            if (paramCount > 0)
            {
                t_rtnStr += $"{m_paramValues[0]}";
            }
            for (int i = 1; i < paramCount; ++i)
            {
                t_rtnStr += $", {m_paramValues[i]}";
            }
            t_rtnStr += "])";
            return t_rtnStr ;
        }
    }
}