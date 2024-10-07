using UnityEngine;
using UnityEngine.InputSystem;

using Timed;
using Helpers;
// Original Authors - Wyatt Senalik

namespace Atma.UI
{
    /// <summary>
    /// Reads input for manipulating time. Expects to be attached to the player.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(ITimeManipController))]
    [RequireComponent(typeof(ITimeRewinder))]
    public sealed class TimeManipInputControls : MonoBehaviour
    {
        private ITimeManipController m_timeManipCont = null;
        private ITimeRewinder m_timeRewinder = null;

        private float m_prevNavDir = 0;


        private void Awake()
        {
            m_timeManipCont = GetComponent<ITimeManipController>();
            m_timeRewinder = GetComponent<ITimeRewinder>();
            #region Asserts
            //CustomDebug.AssertIComponentIsNotNull(m_timeManipCont, this);
            //CustomDebug.AssertIComponentIsNotNull(m_timeRewinder, this);
            #endregion Asserts
        }


        #region Input Messages
        private void OnTimeRewindBegin(InputValue value)
        {
            // Don't allow this if the game is paused (like not GlobalTimeManager paused, like really paused), cause it can break the game.
            if (GamePauseController.instance.isPaused){ return; }
            // Button wasn't pressed
            if (!value.isPressed) { return; }

            m_timeManipCont.BeginTimeManipulation();
        }
        private void OnTimeRewindEnd(InputValue value)
        {
            // Button wasn't pressed
            if (!value.isPressed) { return; }

            // Play = false, pause = true
            bool t_playOrPause = false;

            // If the player is current rewinding or fast forwarding, pause instead to match UI buttons.
            float t_navDir = m_timeRewinder.navigationDir;
            if (t_navDir > 0.1f || t_navDir < -0.1f)
            {
                // Must check if current time is 0 (beginning) or farthest time (end). Because in those scenarios, we play instead of pause.
                float t_curTime = m_timeRewinder.curTime;
                if (t_curTime != 0.0f && t_curTime != m_timeRewinder.farthestTime)
                {
                    // We want to pause since we rewinding or fast forwarding.
                    t_playOrPause = true;
                }
            }

            if (t_playOrPause)
            {
                // True -> pause
                m_timeManipCont.Pause();
            }
            else
            {
                // False -> play
                m_timeManipCont.Play();
            }
        }
        private void OnNavigate(InputValue value)
        {
            #region Asserts
            //CustomDebug.AssertIsTrueForComponent(m_timeRewinder.hasStarted, $"time to be being manipulated before {nameof(OnNavigate)}.", this);
            #endregion Asserts

            float t_navDir = value.Get<float>();
            if (t_navDir <= -0.1f)
            {
                if (m_prevNavDir <= -0.1f) { return; }
                m_prevNavDir = -1;
                m_timeManipCont.Rewind();
            }
            else if (t_navDir >= 0.1f)
            {
                if (m_prevNavDir >= 0.1f) { return; }
                m_prevNavDir = 1;
                m_timeManipCont.FastForward();
            }
            else
            {
                // When released, stop rewinding or fast forwarding (now a hold).
                m_prevNavDir = 0;
                m_timeManipCont.Pause();
            }
        }
        private void OnCreateTimeClone(InputValue value)
        {
            // Only create a time clone when pressed.
            if (!value.isPressed) { return; }

            m_timeManipCont.CreateTimeClone();
        }
        private void OnSkipToBegin(InputValue value)
        {
            //  Not pressed, so do nothing.
            if (!value.isPressed) { return; }

            m_timeManipCont.SkipToBeginning();
        }
        private void OnSkipToEnd(InputValue value)
        {
            //  Not pressed, so do nothing.
            if (!value.isPressed) { return; }

            m_timeManipCont.SkipToEnd();
        }
        #endregion Input Messages
    }
}