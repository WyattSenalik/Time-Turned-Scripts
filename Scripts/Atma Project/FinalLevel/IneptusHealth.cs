using UnityEngine;

using NaughtyAttributes;

using Atma.Events;
using Helpers.Events.GameEventSystem;
using Timed;
// Original Authors - Wyatt Senalik

namespace Atma
{
    [DisallowMultipleComponent]
    public sealed class IneptusHealth : TimedRecorder, IHealth
    {
        public bool isDead => !float.IsNaN(m_deathTime);

        [SerializeField, Required, BoxGroup("Event IDs")]
        private IneptusDiedEventIdentifierSO m_ineptusDiedEventID = null;

        private IGameEvent<IIneptusDiedContext> m_ineptusDiedEvent = null;
        private float m_deathTime = float.NaN;


        protected override void Awake()
        {
            base.Awake();

            m_ineptusDiedEvent = m_ineptusDiedEventID.CreateEvent<IIneptusDiedContext>();
        }
        private void OnDestroy()
        {
            m_ineptusDiedEvent.DeleteEvent();
        }

        public override void OnRecordingResume(float time)
        {
            base.OnRecordingResume(time);

            if (curTime < m_deathTime)
            {
                m_deathTime = float.NaN;
            }
        }

        public bool TakeDamage(IDamageContext context)
        {
            m_deathTime = context.damageTime;
            m_ineptusDiedEvent.Invoke(new IneptusDiedContext(context, transform.position, gameObject));
            // Don't want the bullt to disappear because time is about to die.
            return false;
        }
    }
}