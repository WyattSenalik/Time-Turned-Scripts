using Helpers.Extensions;
using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;
// Original Authors - Wyatt Senalik

namespace Timed.TimedComponentImplementations
{
    /// <summary>
    /// Toggles enabled state of Animator based on when recording starts and stops.
    /// </summary>
    public sealed class TimedAnimator : SnapshotRecorder<AnimatorSnapshot, AnimatorSnapshotData>
    {
        [SerializeField] private bool m_isDebugging = false;
        [SerializeField] private bool m_useSerializedAnimator = false;
        [SerializeField, Required, ShowIf(nameof(m_useSerializedAnimator))] private Animator m_serializedAnimator = null;

        private Animator m_anim = null;

        private bool m_paramDataInitialized = false;
        private AnimatorCompactParamData[] m_constantParamData = null;


        protected override void Awake()
        {
            base.Awake();

            if (m_useSerializedAnimator)
            {
                m_anim = m_serializedAnimator;
            }
            else
            {
                m_anim = this.GetComponentSafe<Animator>(this);
            }
        }
        private void Start()
        {
            InitializeConstantParamData();
        }


        public override void OnRecordingStop(float time)
        {
            base.OnRecordingStop(time);

            m_anim.enabled = false;
        }
        public override void OnRecordingResume(float time)
        {
            base.OnRecordingResume(time);

            m_anim.enabled = true;
            ApplySnapshotData(timedVariable.curData);
        }

        protected override AnimatorSnapshotData GatherCurrentData()
        {
            if (!gameObject.activeInHierarchy)
            {
                return timedVariable.scrapbook.GetSnapshot(curTime).data;
            }
            bool t_wasAnimEnabled = m_anim.enabled;
            if (!t_wasAnimEnabled)
            {
                m_anim.enabled = true;
            }

            // Grab the animator state information.
            AnimatorStateInfo t_stateInfo = m_anim.GetCurrentAnimatorStateInfo(0);
            int t_stateHash = t_stateInfo.fullPathHash;
            float t_normTime = t_stateInfo.normalizedTime;
            if (t_stateInfo.loop)
            {
                t_normTime %= 1.0f;
            }
            else
            {
                t_normTime = Mathf.Clamp01(t_normTime);
            }

            // Try initialize if not already initialized.
            InitializeConstantParamData();
            // Grab all the param values.
            int t_paramCount = m_anim.parameterCount;
            float[] t_paramVals = new float[t_paramCount];
            for (int i = 0; i < t_paramCount; ++i)
            {
                t_paramVals[i] = ExtractParamValue(i);
            }

            if (!t_wasAnimEnabled)
            {
                m_anim.enabled = false;
            }

            return new AnimatorSnapshotData(t_stateHash, t_normTime, t_paramVals, m_isDebugging);
        }
        protected override void ApplySnapshotData(AnimatorSnapshotData snapData)
        {
            if (snapData == null) { return; }
            if (!gameObject.activeInHierarchy) { return; }
            bool t_wasAnimEnabled = m_anim.enabled;
            if (!t_wasAnimEnabled)
            {
                m_anim.enabled = true;
            }

            int t_paramCount = snapData.paramCount;
            if (t_paramCount != m_constantParamData.Length)
            {
                #region Logs
                //CustomDebug.LogWarningForComponent($"Parameter count for snap data ({snapData}). Does not match constant param data length of {m_constantParamData.Length}.", this);
                #endregion Logs
            }
            else
            {
                // Apply the parameter data.
                IReadOnlyList<float> t_paramVals = snapData.paramValues;
                for (int i = 0; i < t_paramCount; ++i)
                {
                    float t_curVal = t_paramVals[i];
                    AnimatorCompactParamData t_compactData = m_constantParamData[i];
                    switch (t_compactData.type)
                    {
                        case AnimatorControllerParameterType.Float:
                            m_anim.SetFloat(t_compactData.id, t_curVal);
                            break;
                        case AnimatorControllerParameterType.Int:
                            m_anim.SetInteger(t_compactData.id, Mathf.RoundToInt(t_curVal));
                            break;
                        case AnimatorControllerParameterType.Bool:
                            m_anim.SetBool(t_compactData.id, t_curVal >= 1);
                            break;
                        case AnimatorControllerParameterType.Trigger:
                            // Does this work? `\_(**)_/`
                            if (t_curVal >= 1)
                            {
                                m_anim.SetTrigger(t_compactData.id);
                            }
                            break;
                    }
                }
            }

            // Go to the state it was in before.
            m_anim.Play(snapData.stateHash, 0, snapData.normTime);

            if (!t_wasAnimEnabled)
            {
                m_anim.enabled = false;
            }
        }
        protected override AnimatorSnapshot CreateSnapshot(AnimatorSnapshotData data, float time)
        {
            return new AnimatorSnapshot(time, data, eInterpolationOption.Linear);
        }


        private void InitializeConstantParamData()
        {
            // Already initialized
            if (m_paramDataInitialized) { return; }
            // Can't initialize when gameobject is off
            if (!gameObject.activeInHierarchy) { return; }
            bool t_wasAnimEnabled = m_anim.enabled;
            if (!t_wasAnimEnabled)
            {
                m_anim.enabled = true;
            }

            int t_paramCount = m_anim.parameterCount;
            m_constantParamData = new AnimatorCompactParamData[m_anim.parameterCount];
            for (int i = 0; i < t_paramCount; ++i)
            {
                AnimatorControllerParameter t_rawParamData = m_anim.GetParameter(i);
                m_constantParamData[i] = new AnimatorCompactParamData(t_rawParamData.type, t_rawParamData.nameHash);
            }

            m_paramDataInitialized = true;

            if (!t_wasAnimEnabled)
            {
                m_anim.enabled = false;
            }
        }
        private float ExtractParamValue(int index)
        {
            #region Asserts
            //CustomDebug.AssertIndexIsInRange(index, m_constantParamData, this);
            #endregion Asserts
            AnimatorCompactParamData t_paramData = m_constantParamData[index];
            AnimatorControllerParameterType t_type = t_paramData.type;
            int t_paramID = t_paramData.id;
            switch (t_type)
            {
                case AnimatorControllerParameterType.Float:
                    return m_anim.GetFloat(t_paramID);
                case AnimatorControllerParameterType.Int:
                    return m_anim.GetInteger(t_paramID);
                case AnimatorControllerParameterType.Bool:
                    return m_anim.GetBool(t_paramID) ? 1 : 0;
                case AnimatorControllerParameterType.Trigger:
                    // Does this work? `\_(**)_/`
                    return m_anim.GetBool(t_paramID) ? 1 : 0;
                default:
                    CustomDebug.UnhandledEnum(t_type, this);
                    return float.NaN;
            }
        }
    }
}