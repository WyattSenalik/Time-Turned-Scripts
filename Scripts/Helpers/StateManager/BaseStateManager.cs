using System;
using UnityEngine;

using Helpers.Events;
using Helpers.Events.CatchupEvents;
// Original Authors - Wyatt Senalik

namespace Helpers.StateManager
{
    /// <summary>
    /// Base class for a state manager that is also a singleton mono behaviour.
    /// </summary>
    /// <typeparam name="TEnum">Enum of the states. MUST have default int conversion
    /// values.</typeparam>
    public abstract class BaseStateManager<TEnum> : MonoBehaviour
        where TEnum : Enum
    {
        private const bool IS_DEBUGGING = false;

        private TEnum m_curState = default;

        /// <summary>
        /// Parameter: Initial State.
        /// </summary>
        private readonly CatchupEvent<TEnum> m_onInitialStateSet = new CatchupEvent<TEnum>();
        /// <summary>
        /// Parameters: Previous State and New State.
        /// </summary>
        private readonly CatchupEvent<TEnum, TEnum> m_onStateChange = new CatchupEvent<TEnum, TEnum>();

        protected ICatchupEventReset resetOnInitialStateSet => m_onInitialStateSet;
        protected ICatchupEventReset resetOnStateChange => m_onStateChange;

        public TEnum curState => m_curState;
        public IEventPrimer<TEnum> onInitialStateSet => m_onInitialStateSet;
        public IEventPrimer<TEnum, TEnum> onStateChange => m_onStateChange;


        // Domestic Initialization
        protected virtual void Awake()
        {
            m_curState = default;
            m_onInitialStateSet.Invoke(m_curState);
            #region Logs
            //CustomDebug.LogForComponent($"Invoking {nameof(m_onInitialStateSet)} for state <color=green>{m_curState}</color>", this, IS_DEBUGGING);
            #endregion Logs
        }


        /// <summary>
        /// Advances to the next state.
        /// See <see cref="TEnum"/> for the order.
        /// </summary>
        public void AdvanceState()
        {
            TEnum temp_newState = default;
            // This is kindof gross, but yea
            try
            {
                int temp_incrIntState = (int)(object)m_curState + 1;
                temp_newState = (TEnum)(object)temp_incrIntState;
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to cast between enum and int.\n" +
                    $"Exception Message: {e.Message}.\n" +
                    $"StackTrace: {e.StackTrace}.");
            }
            SetState(temp_newState);
        }
        /// <summary>
        /// Sets the currently active state to the one specified.
        /// </summary>
        /// <param name="newState">State to set.</param>
        public void SetState(TEnum newState)
        {
            TEnum temp_prevState = m_curState;
            m_curState = newState;
            m_onStateChange.Invoke(temp_prevState, m_curState);
            #region Logs
            //CustomDebug.LogForComponent($"Invoking {nameof(m_onStateChange)} for state <color=green>{m_curState}</color>. Prev state was <color=red>{temp_prevState}</color>.", this, IS_DEBUGGING);
            #endregion Logs
        }
    }
}
