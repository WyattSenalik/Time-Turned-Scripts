using UnityEngine;

using NaughtyAttributes;

using Atma.Events;
using Helpers.Events.GameEventSystem;
using Timed;
// Original Authors - Wyatt Senalik

namespace Atma
{
    [RequireComponent(typeof(ModularTurretController))]
    public sealed class EnemyHealth : TimedRecorder, IHealth
    {
        private const bool IS_DEBUGGING = false;

        public bool isDead => !float.IsPositiveInfinity(m_deathTime);
        public float deathTime => m_deathTime;

        [SerializeField, Required]
        private PlayerTimeManipEndEventIdentifierSO m_playerTimeManipEndEventID = null;

        private GlobalTimeManager m_timeMan = null;
        private ModularTurretController m_turretCont = null;
        private IEnemyDeathHandler m_deathHandler = null;

        private float m_deathTime = float.PositiveInfinity;
        private int m_recordingStopRequestID = -1;


        protected override void Awake()
        {
            base.Awake();
            #region Asserts
            //CustomDebug.AssertSerializeFieldIsNotNull(m_playerTimeManipEndEventID, nameof(m_playerTimeManipEndEventID), this);
            #endregion Asserts

            m_turretCont = GetComponent<ModularTurretController>();
            #region Asserts
            //CustomDebug.AssertComponentIsNotNull(m_turretCont, this);
            #endregion Asserts
            // Its okay if it doesn't have one (for now, TODO: give every enemy a death handler).
            m_deathHandler = GetComponent<IEnemyDeathHandler>();
        }
        private void Start()
        {
            m_timeMan = GlobalTimeManager.instance;
            #region Asserts
            //CustomDebug.AssertSingletonIsNotNull(m_timeMan, this);
            #endregion Asserts

            m_playerTimeManipEndEventID.SubscribeToEvent(OnPlayerTimeManipEnd);
        }
        private void OnDestroy()
        {
            m_playerTimeManipEndEventID.UnsubscribeFromEvent(OnPlayerTimeManipEnd);
        }


        public bool TakeDamage(IDamageContext context)
        {
            m_turretCont.health = 0;
            m_deathTime = context.damageTime;
            if (m_deathHandler != null)
            {
                m_deathHandler.HandleDeath(context.damageTime);
            }
            else
            {
                m_recordingStopRequestID = timedObject.RequestSuspendRecording();
            }

            #region Logs
            //CustomDebug.LogForComponent($"took damage. (damageTime:{context.damageTime}) (curTime:{curTime}).", this, IS_DEBUGGING);
            #endregion Logs

            // Want the bullet to go away because the enemy has tanked this shot.
            return true;
        }


        // Using an event instead of TrimDataAfter because we ask the recording to be suspended when we kill this enemy.
        // TODO Change this later? Need to play death animation, so maybe use a state machine instead.
        private void OnPlayerTimeManipEnd(IPlayerTimeManipEndContext context)
        {
            // If died during playtime (death was not at positive inf) and the rewound to time is before death.
            if (isDead && context.timeManipEnded < m_deathTime)
            {
                #region Logs
                //CustomDebug.LogForComponent($"undead enemy. Death time was {m_deathTime}.", this, IS_DEBUGGING);
                #endregion Logs

                m_turretCont.health = 1;
                m_deathTime = float.PositiveInfinity;
                if (m_deathHandler != null)
                {
                    m_deathHandler.RevertDeath();
                }
                else
                {
                    timedObject.CancelSuspendRecordingRequest(m_recordingStopRequestID);
                }
                m_recordingStopRequestID = -1;
            }
            // Otherwise, it can stay dead.
        }
    }
}