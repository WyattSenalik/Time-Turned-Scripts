using System;
using System.Collections.Generic;
// Original Authors - Wyatt Senalik

namespace Helpers.StateManager
{
    /// <summary>
    /// Handles determining if the state changed to is an active state or not.
    /// 
    /// IMPORTANT: Make sure to call ToggleActive(false) when the controller for this
    /// object is destroyed.
    /// </summary>
    /// <typeparam name="TEnum"></typeparam>
    public sealed class StateChangeHandler<TEnum> : IStateChangeHandler
        where TEnum : Enum
    {
        private const bool IS_DEBUGGING = false;

        private readonly IStateManager<TEnum> m_stateMan = null;
        private readonly List<TEnum> m_activateStates = null;
        private readonly Action m_activateAction = null;
        private readonly Action m_deactivateAction = null;

        private bool m_isActive = false;


        /// <summary>
        /// Constructs a <see cref="StateChangeHandler{TEnum}"/>.
        /// </summary>
        /// <param name="stateMan">The state manager to respond to the state
        /// changing.</param>
        /// <param name="activateAction">The action to invoke when an activate state is
        /// changed to from an inactivate state.</param>
        /// <param name="deactivateAction">The action to invoke when an inactivate state
        /// is change to from an activate state.</param>
        /// <param name="activateStates">States that are marked as the activate ones.
        /// AKA the states that you want to be active for.</param>
        public StateChangeHandler(IStateManager<TEnum> stateMan,
            Action activateAction = null, Action deactivateAction = null,
            params TEnum[] activateStates)
        {
            m_stateMan = stateMan;
            m_activateStates = new List<TEnum>(activateStates);
            m_activateAction = activateAction;
            m_deactivateAction = deactivateAction;

            ToggleActive(true);
        }


        /// <summary>
        /// Activate the <see cref="StateChangeHandler{TEnum}"/> to
        /// subscribe to the <see cref="BaseStateManager{TEnum, TSelf}"/>
        /// state changes.
        ///
        /// Starts enabled by default.
        /// </summary>
        /// <param name="cond">Enable (true) or disable (false).</param>
        public void ToggleActive(bool cond)
        {
            if (m_isActive == cond) { return; }

            // If this is called on destroy, its possible that the state
            // manager is null, in which case its fine.
            if (m_stateMan == null) { return; }

            m_stateMan.onInitialStateSet.ToggleSubscription(HandleOnInitialStateSet,
                cond);
            m_stateMan.onStateChange.ToggleSubscription(HandleOnStateChange, cond);

            m_isActive = cond;
        }


        /// <summary>
        /// Calls the activate action if the initial state is one of the activate
        /// states.
        /// </summary>
        private void HandleOnInitialStateSet(TEnum initState)
        {

            // If the initial state is one of the activate states,
            // invoke the activate action
            if (m_activateStates.Contains(initState))
            {
                #region Logs
                //CustomDebug.Log($"{nameof(HandleOnInitialStateSet)} with param {initState} is <color=green>an active</color> state.", IS_DEBUGGING);
                #endregion Logs

                m_activateAction?.Invoke();
            }
            else
            {
                #region Logs
                //CustomDebug.Log($"{nameof(HandleOnInitialStateSet)} with param {initState} is <color=red>not an active</color> state", IS_DEBUGGING);
                #endregion Logs
            }
        }
        /// <summary>
        /// Handles moving from an inactive state to an active one
        /// or moving from an activate state to an inactive one.
        /// </summary>
        private void HandleOnStateChange(TEnum prevState, TEnum newState)
        {
            // Old state was an activate one, so if the new state is not also
            // an activate state, then we need to invoke the deactivate action
            if (m_activateStates.Contains(prevState))
            {
                // New state is not an activate state, so deactivate
                if (!m_activateStates.Contains(newState))
                {
                    #region Logs
                    //CustomDebug.Log($"{nameof(HandleOnStateChange)} with params {nameof(prevState)}={prevState} and {nameof(newState)}={newState} caused a <color=red>deactivation</color>.", IS_DEBUGGING);
                    #endregion Logs

                    m_deactivateAction?.Invoke();
                }
                else
                {
                    #region Logs
                    //CustomDebug.Log($"{nameof(HandleOnStateChange)} with params {nameof(prevState)}={prevState} and {nameof(newState)}={newState} caused <color=yellow>nothing</color>. They are both <color>=green>active</color> states.", IS_DEBUGGING);
                    #endregion Logs
                }
            }
            // Old state was not an activate one, so if the new state is, then
            // we need to invoke the activate action
            else
            {
                // New state is an activate state, so activate
                if (m_activateStates.Contains(newState))
                {
                    #region Logs
                    //CustomDebug.Log($"{nameof(HandleOnStateChange)} with params {nameof(prevState)}={prevState} and {nameof(newState)}={newState} caused a <color=green>activation</color>.", IS_DEBUGGING);
                    #endregion Logs

                    m_activateAction?.Invoke();
                }
                else
                {
                    #region Logs
                    //CustomDebug.Log($"{nameof(HandleOnStateChange)} with params {nameof(prevState)}={prevState} and {nameof(newState)}={newState} caused <color=yellow>nothing</color>. They are both <color=red>inactive</color> states.",  IS_DEBUGGING);
                    #endregion Logs
                }
            }
        }
    }
}
