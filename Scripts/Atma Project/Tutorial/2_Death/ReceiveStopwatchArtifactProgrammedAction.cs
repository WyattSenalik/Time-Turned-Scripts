using System;
using UnityEngine;

using NaughtyAttributes;

using Dialogue.ConvoActions.Programmed;
using Dialogue;
using Timed;
using Helpers.Animation;
using Atma.Dialogue;
// Original Authors - Wyatt Senalik

namespace Atma.Tutorial.Death
{
    /// <summary>
    /// Scripted event for the Rewind/Death Tutorial (Scene 3 as of writing this) for when the stopwatch artifact is given to the player.
    /// </summary>
    public sealed class ReceiveStopwatchArtifactProgrammedAction : MonoBehavEndpointProgrammedConvoAction
    {
        private const bool IS_DEBUGGING = false;

        [SerializeField, Required] private TimeRewinder m_timeRewinder = null;
        [SerializeField, Required] private ScriptedAnimationMonoBehaviour m_showStopwatchPopupScriptedAnimation = null;
        [SerializeField, Required] private HideStopwatchPopupScriptedAnimation m_hideStopwatchPopupScriptedAnimation = null;

        [SerializeField, Min(0)] private int m_intensityLevelOnReceiveStopwatch = 1;

        private ConversationSkipper m_convoSkipper = null;
        private UISoundController m_soundCont = null;
        private MusicIntensitySetter m_intensitySetter = null;

        private Action m_onFinished = null;
        private int m_curSequence = 0;
        private bool m_isCurSequenceDone = false;


        protected override void Awake()
        {
            base.Awake();
            #region Asserts
            //CustomDebug.AssertSerializeFieldIsNotNull(m_timeRewinder, nameof(m_timeRewinder), this);
            #endregion Asserts
        }
        private void Start()
        {
            m_convoSkipper = ConversationSkipper.GetInstanceSafe(this);
            m_soundCont = UISoundController.GetInstanceSafe(this);
            m_intensitySetter = MusicIntensitySetter.GetInstanceSafe(this);
        }


        public override void Begin(ConvoData convoData, Action onFinished = null)
        {
            m_intensitySetter.SetIntensity(m_intensityLevelOnReceiveStopwatch);

            if (m_convoSkipper.ShouldSkipDialogue())
            {
                m_hideStopwatchPopupScriptedAnimation.stopwatchTimeManipUIObj.SetActive(true);
                onFinished?.Invoke();
                return;
            }

            m_soundCont.PlayObtainStopwatchSound();

            m_onFinished = onFinished;
            PartOneOfSequence();
        }
        public override bool Advance(ConvoData convoData)
        {
            if (m_convoSkipper.ShouldSkipDialogue())
            {
                return true;
            }

            if (m_isCurSequenceDone)
            {
                #region Logs
                //CustomDebug.LogForComponent($"Advance {m_curSequence}", this, IS_DEBUGGING);
                #endregion Logs
                switch (m_curSequence)
                {
                    case 0:
                    {
                        ++m_curSequence;
                        PartTwoOfSequence();
                        return false;
                    }
                    default:
                    {
                        return true;
                    }
                }
            }
            #region Logs
            //CustomDebug.LogForComponent($"Advance: Nope!", this, IS_DEBUGGING);
            #endregion Logs
            return false;
        }


        private void PartOneOfSequence()
        {
            #region Logs
            //CustomDebug.LogForComponent(nameof(PartOneOfSequence), this, IS_DEBUGGING);
            #endregion Logs
            m_isCurSequenceDone = false;

            // 1 - freeze time
            m_timeRewinder.StartRewind(0);
            // 2 - show the popup.
            m_showStopwatchPopupScriptedAnimation.onEnd += OnShowAnimEnd;
            m_showStopwatchPopupScriptedAnimation.Play();
            // 3 - wait until advance to begin next part.
        }
        private void PartTwoOfSequence()
        {
            #region Logs
            //CustomDebug.LogForComponent(nameof(PartTwoOfSequence), this, IS_DEBUGGING);
            #endregion Logs
            m_isCurSequenceDone = false;

            // 1 - hide the popup and show the stopwatch
            m_hideStopwatchPopupScriptedAnimation.onEnd += OnHideAnimEnd;
            m_hideStopwatchPopupScriptedAnimation.Play();
            // 2 - wait until anim finished to proceed.
            
            SteamAchievementManager.instance.GrantAchievement( AchievementId.GET_STOPWATCH );
        }

        private void OnShowAnimEnd()
        {
            m_showStopwatchPopupScriptedAnimation.onEnd -= OnShowAnimEnd;

            m_isCurSequenceDone = true;
        }
        private void OnHideAnimEnd()
        {
            m_hideStopwatchPopupScriptedAnimation.onEnd -= OnHideAnimEnd;

            // Unfreeze time
            m_timeRewinder.CancelRewind();

            m_isCurSequenceDone = true;
            m_onFinished?.Invoke();
        }
    }
}