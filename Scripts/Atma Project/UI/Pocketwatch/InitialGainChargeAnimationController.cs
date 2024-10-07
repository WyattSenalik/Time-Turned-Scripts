using UnityEngine;

using NaughtyAttributes;

using Helpers.Extensions;
using Timed;
// Original Authors - Wyatt Senalik

namespace Atma
{
    public sealed class InitialGainChargeAnimationController : MonoBehaviour
    {
        public bool hasFinished { get; private set; } = false;

        [SerializeField] private Animator m_initialChargeAnimator1 = null;
        [SerializeField] private Animator m_initialChargeAnimator2 = null;
        [SerializeField] private Animator m_initialChargeAnimator3 = null;
        [SerializeField, AnimatorParam(nameof(m_initialChargeAnimator1))] private string m_playTriggerAnimParamName = "Start";
        [SerializeField, Min(0.0f)] private float m_animLength = 0.41666666666f;

        private LevelOptions m_levelOpt = null;
        private UISoundController m_soundCont = null;

        private BranchPlayerController m_playerCont = null;
        private TimeRewinder m_timeRewinder = null;

        private float m_startTime = 0.0f;
        private int m_amountCharges = 0;


        private void Start()
        {
            m_levelOpt = LevelOptions.GetInstanceSafe();
            m_soundCont = UISoundController.GetInstanceSafe();

            PlayerSingleton t_playerSingleton = PlayerSingleton.GetInstanceSafe();
            m_playerCont = t_playerSingleton.GetComponentSafe<BranchPlayerController>();
            m_timeRewinder = t_playerSingleton.GetComponentSafe<TimeRewinder>();

            ToggleSubscriptions(true);
        }
        private void OnDestroy()
        {
            ToggleSubscriptions(false);
        }


        public int GetAmountChargeAnimationsFinished()
        {
            float t_anim1EndTime = m_startTime + m_animLength;
            float t_anim2EndTime = t_anim1EndTime + m_animLength;
            float t_anim3EndTime = t_anim2EndTime + m_animLength;

            int t_amChargeAnimsFin = 0;
            if (Time.time >= t_anim1EndTime)
            {
                ++t_amChargeAnimsFin;
                if (Time.time >= t_anim2EndTime)
                {
                    ++t_amChargeAnimsFin;
                    if (Time.time >= t_anim3EndTime)
                    {
                        ++t_amChargeAnimsFin;
                    }
                }
            }
            return Mathf.Min(t_amChargeAnimsFin, m_amountCharges);
        }


        private void ToggleSubscriptions(bool cond)
        {
            if (m_playerCont != null)
            {
                m_playerCont.onBeginTimeManip.ToggleSubscription(OnBeginTimeManip, cond);
            }
        }
        private void OnBeginTimeManip()
        {
            if (m_playerCont.isFirstPause)
            {
                m_startTime = Time.time;
                float t_extraTimeToWait = 0.0f;

                m_amountCharges = m_levelOpt.maxCloneCharges;
                if (m_amountCharges >= 1)
                {
                    m_initialChargeAnimator1.SetTrigger(m_playTriggerAnimParamName);
                    m_soundCont.PlayShowChargeSound();
                    if (m_amountCharges >= 2)
                    {
                        Invoke(nameof(StartAnim2), m_animLength);
                        if (m_amountCharges >= 3)
                        {
                            Invoke(nameof(StartAnim3), m_animLength * 2);
                        }
                    }
                }
                t_extraTimeToWait = m_amountCharges * m_animLength;

                if (t_extraTimeToWait > 0.0f)
                {
                    Invoke(nameof(SetFinished), t_extraTimeToWait);
                }
                else
                {
                    SetFinished();
                }
            }
            else
            {
                // Non first, so set finished right away.
                SetFinished();
            }
        }

        private void StartAnim2()
        {
            m_initialChargeAnimator2.SetTrigger(m_playTriggerAnimParamName);
            m_soundCont.PlayShowChargeSound();
        }
        private void StartAnim3()
        {
            m_initialChargeAnimator3.SetTrigger(m_playTriggerAnimParamName);
            m_soundCont.PlayShowChargeSound();
        }
        private void SetFinished()
        {
            hasFinished = true;
        }
    }
}