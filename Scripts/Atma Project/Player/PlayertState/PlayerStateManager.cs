using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using NaughtyAttributes;

using Helpers;
using Helpers.Events;
using Helpers.Events.CatchupEvents;
using Helpers.StateManager;
using Timed;
// Original Authors - Wyatt Senalik

namespace Atma
{
    /// <summary>
    /// Holds the current state of the player.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class PlayerStateManager : WindowRecorder<ePlayerState>, IStateManager<ePlayerState>
    {
        private const bool IS_DEBUGGING = false;

        public ePlayerState curState { get; private set; }
        public ePlayerState timeAwareCurState => timeAwareMostRecentWindow == null ? curState : timeAwareMostRecentWindow.data;
        public IEventPrimer<ePlayerState> onInitialStateSet => m_onInitialStateSet;
        /// <summary>
        /// Param0 is previous state.
        /// Param1 is new current state.
        /// </summary>
        public IEventPrimer<ePlayerState, ePlayerState> onStateChange => m_onStateChange;

        [SerializeField] private ePlayerState m_startingState = ePlayerState.Default;

        private readonly CatchupEvent<ePlayerState> m_onInitialStateSet = new CatchupEvent<ePlayerState>();
        private readonly CatchupEvent<ePlayerState, ePlayerState> m_onStateChange = new CatchupEvent<ePlayerState, ePlayerState>();

        [SerializeField, ReadOnly, ShowIf(nameof(IS_DEBUGGING))]
        private DebugWindowState[] m_debugWindows = null;


        private void Start()
        {
            curState = m_startingState;

            StartNewWindow(m_startingState);
            m_onInitialStateSet.Invoke(curState);
        }
        private void Update()
        {
            CustomDebug.RunDebugFunction(() =>
            {
                m_debugWindows = GetDebugStates();
            }, IS_DEBUGGING);
        }

        protected override void OnRecordingResumeDuringWindow(WindowData<ePlayerState> windowResumedDuring)
        {
            base.OnRecordingResumeDuringWindow(windowResumedDuring);

            ePlayerState newState = windowResumedDuring.data;
            // Don't set the state if it is the current state
            if (newState == curState) { return; }

            // Update state
            ePlayerState t_prevState = curState;
            curState = newState;

            m_onStateChange.Invoke(t_prevState, curState);
        }


        /// <summary>
        /// Sets the current state to the given state.
        /// </summary>
        public void SetState(ePlayerState newState)
        {
            // Don't set state if not recording.
            if (!isRecording) { return; }
            // Don't set the state if it is the current state
            if (newState == curState) { return; }

            // Update state
            ePlayerState t_prevState = curState;
            curState = newState;
            // Record the change for rewinding
            // 1. End previous window
            #region Asserts
            //CustomDebug.AssertIsTrueForComponent(HasCurrentWindow(), $"a current window to already exist", this);
            #endregion Asserts
            EndCurrentWindow();
            // 2. Start new window
            StartNewWindow(newState);

            m_onStateChange.Invoke(t_prevState, curState);
        }
        /// <summary>
        /// Tries to set the state to the given state, but will only change the state
        /// if the current state is one of the prevStates (Whitelist) or if the current
        /// state is not one of the prevStates (Blacklist).
        /// </summary>
        /// <returns>If the state was changed.</returns>
        public bool TrySetState(ePlayerState newState, eAllowListType listType,
            params ePlayerState[] prevStates)
        {
            // Don't set state if not recording.
            if (!isRecording) { return false; }

            switch (listType)
            {
                case eAllowListType.Whitelist:
                    // If is contained.
                    if (prevStates.Contains(curState))
                    {
                        SetState(newState);
                        return true;
                    }
                    break;
                case eAllowListType.Blacklist:
                    // If is not contained.
                    if (!prevStates.Contains(curState))
                    {
                        SetState(newState);
                        return true;
                    }
                    break;
                default:
                    CustomDebug.UnhandledEnum(listType, this);
                    break;
            }
            return false;
        }


        private DebugWindowState[] GetDebugStates()
        {
            List<DebugWindowState> t_debugWindows = new List<DebugWindowState>();
            foreach (WindowData<ePlayerState> t_window in windowCollection.GetAllWindows())
            {
                t_debugWindows.Add(new DebugWindowState(t_window.data, t_window.window));
            }
            return t_debugWindows.ToArray();
        }
        [Serializable]
        struct DebugWindowState
        {
            [SerializeField] private ePlayerState m_state;
            [SerializeField] private TimeFrame m_frame;


            public DebugWindowState(ePlayerState state, TimeFrame frame)
            {
                m_state = state;
                m_frame = frame;
            }
        }
    }
}