using UnityEngine;
using UnityEngine.InputSystem;

using NaughtyAttributes;

using Helpers;
// Original Authors - Wyatt Senalik

namespace Atma
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(PlayerStateManager))]
    [RequireComponent(typeof(ILeaper))]
    public sealed class PlayerLeap : MonoBehaviour
    {
        [SerializeField, Required] private LeapObjCollider m_leapObjCol = null;
        [SerializeField] private ePlayerState[] m_noLeapStates = { ePlayerState.Dead };

        private PlayerStateManager m_playerStateMan = null;
        private ILeaper m_leaper = null;

        private ILeapObject m_curLeapingObj = null;


        private void Awake()
        {
            m_playerStateMan = GetComponent<PlayerStateManager>();
            m_leaper = GetComponent<ILeaper>();
            #region Asserts
            //CustomDebug.AssertSerializeFieldIsNotNull(m_leapObjCol, nameof(m_leapObjCol), this);
            //CustomDebug.AssertComponentIsNotNull(m_playerStateMan, this);
            //CustomDebug.AssertIComponentIsNotNull(m_leaper, this);
            #endregion Asserts
        }
        private void Start()
        {
            ToggleSubscriptions(true);
        }
        private void OnDestroy()
        {
            ToggleSubscriptions(false);
        }


        private void ToggleSubscriptions(bool cond)
        {
            m_playerStateMan?.onStateChange.ToggleSubscription(OnPlayerStateChange, cond);
            m_leaper?.onLeapEnd.ToggleSubscription(OnLeapEnd, cond);
        }

        private void OnPlayerStateChange(ePlayerState prevState, ePlayerState newState)
        {
            if (prevState == ePlayerState.Leap && newState != ePlayerState.Leap)
            {
                // Reset the cur leaping object when state changes from leap to account for rewinding/resuming without needing to get a buttload of info from everywhere.
                m_curLeapingObj = null;
                m_leapObjCol.curLeapingObj = null;
            } 
        }
        private void OnLeapEnd()
        {
            m_curLeapingObj = null;
            m_leapObjCol.curLeapingObj = null;
            m_playerStateMan.TrySetState(ePlayerState.Default, eAllowListType.Whitelist, ePlayerState.Leap);

            SteamAchievementManager.instance.GrantAchievement( AchievementId.FIRST_LEAP );
        }

        #region InputMessages
        private void OnLeap(InputValue value)
        {
            if (value.isPressed)
            {
                ILeapObject t_targetedLeapObj = m_leapObjCol.targetedLeapObj;
                // Can't leap if there is nothing to leap over.
                if (t_targetedLeapObj == null) { return; }
                // Can't leap if not available
                if (!t_targetedLeapObj.availableToUse) { return; }
                // Can't leap if new leap object is the current leap object (if still leaping)
                if (t_targetedLeapObj == m_curLeapingObj) { return; }
                // Try to change the player to the leap state.
                if (m_playerStateMan.TrySetState(ePlayerState.Leap, eAllowListType.Blacklist, m_noLeapStates))
                {
                    // If succeeded, also start leaping.
                    m_leaper.Leap(m_leapObjCol.highlightDir, t_targetedLeapObj);

                    m_curLeapingObj = t_targetedLeapObj;
                    // Set the cur leap object for the collider too.
                    m_leapObjCol.curLeapingObj = m_curLeapingObj;

                    m_curLeapingObj.OnLeptFrom();
                }
            }
        }
        #endregion InputMessages
    }
}