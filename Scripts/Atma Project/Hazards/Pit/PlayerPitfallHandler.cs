using UnityEngine;

using NaughtyAttributes;

using Helpers;
using Timed;
using Timed.Animation.BetterCurve;
// Original Authors - Wyatt Senalik

namespace Atma
{
    /// <summary>
    /// Tries to change the player state to Dead when the player enters a pit.
    /// Doesn't kill them if in any of the survive states.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(PlayerStateManager))]
    [RequireComponent(typeof(PlayerHealth))]
    [RequireComponent(typeof(TimedObject))]
    [RequireComponent(typeof(IMovementSuspender))]
    public sealed class PlayerPitfallHandler : MonoBehaviour, IPitfallHandler
    {
        public bool isOverPit { get; private set; } = false;

        //[SerializeField, Required] private EnableBehaviourManager m_pitColliderEnableManager = null;
        [SerializeField, Required] private ScaleTimedBCA m_pitfallScaleBCA = null;

        private PlayerStateManager m_stateMan = null;
        private PlayerHealth m_playerHealth = null;
        private TimedObject m_playerTimedObj = null;
        private IMovementSuspender m_moveSus = null;

        private int m_disabledPitColliderID = -1;

        private GameObject m_recentPitVisualsParent = null;


        private void Awake()
        {
            #region Asserts
            ////CustomDebug.AssertSerializeFieldIsNotNull(m_pitColliderEnableManager, nameof(m_pitColliderEnableManager), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_pitfallScaleBCA, nameof(m_pitfallScaleBCA), this);
            #endregion Asserts
            m_stateMan = GetComponent<PlayerStateManager>();
            m_playerHealth = GetComponent<PlayerHealth>();
            m_playerTimedObj = GetComponent<TimedObject>();
            m_moveSus = GetComponent<IMovementSuspender>();
            #region Asserts
            //CustomDebug.AssertComponentIsNotNull(m_stateMan, this);
            //CustomDebug.AssertComponentIsNotNull(m_playerHealth, this);
            //CustomDebug.AssertComponentIsNotNull(m_playerTimedObj, this);
            //CustomDebug.AssertIComponentIsNotNull(m_moveSus, this);
            #endregion Asserts
        }
        private void Start()
        {
            m_pitfallScaleBCA.onEnd += KillAfterFall;
        }
        private void OnDestroy()
        {
            if (m_pitfallScaleBCA != null)
            {
                m_pitfallScaleBCA.onEnd -= KillAfterFall;
            }
        }

        public void OnEnclosedInPitStart()
        {
            isOverPit = true;
            //m_disabledPitColliderID = m_pitColliderEnableManager.RequestDisableBehaviour();
        }
        public void OnEnclosedInPitEnd()
        {
            isOverPit = false;
            //m_pitColliderEnableManager.CancelDisableRequest(m_disabledPitColliderID);
        }
        public void Fall(GameObject pitVisualsParentObj)
        {
            isOverPit = true;
            m_recentPitVisualsParent = pitVisualsParentObj;
            PlayFallAnim();
        }
        public void FallStay(GameObject pitVisualsParentObj)
        {
            isOverPit = true;
            m_recentPitVisualsParent = pitVisualsParentObj;
            PlayFallAnim();
        }


        private void PlayFallAnim()
        {
            // Don't fall into pit if leaping
            if (m_stateMan.curState == ePlayerState.Leap) { return; }
            if (!m_pitfallScaleBCA.isPlaying)
            {
                // Doesn't interrupt if already playing.
                m_pitfallScaleBCA.Play();
                // Don't let player move while falling.
                m_moveSus.SuspendForTime(m_pitfallScaleBCA.duration);
            }
        }
        private void KillAfterFall()
        {
            m_playerHealth.TakeDamage(new DamageContext(m_playerTimedObj.curTime, m_recentPitVisualsParent));
        }
    }
}