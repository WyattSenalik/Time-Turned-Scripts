using UnityEngine;

using Helpers.Extensions;
// Original Authors - Wyatt Senalik

namespace Timed.TimedComponentImplementations
{
    /// <summary>
    /// Records rigidbody data (currently only velocity) and replays it.
    /// Not meant to accurately recreate movement, meant to simulate collision interactions in the same way. Like allowing the player to push boxes in the same way.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class TimedRigidbody2D : SnapshotRecorder<Rigidbody2DDataSnapshot, Rigidbody2DData>
    {
        public Rigidbody2D rb
        {
            get
            {
                if (m_rb == null)
                {
                    m_rb = this.GetComponentSafe<Rigidbody2D>();
                }
                return m_rb;
            }
        }

        private Rigidbody2D m_rb = null;


        public override void OnRecordingStop(float time)
        {
            base.OnRecordingStop(time);

            rb.velocity = Vector2.zero;
            rb.isKinematic = true;
            rb.simulated = false;
            rb.Sleep();
        }
        public override void OnRecordingResume(float time)
        {
            base.OnRecordingResume(time);

            rb.isKinematic = false;
            rb.simulated = true;
            rb.WakeUp();
            Rigidbody2DData t_snapData = scrapbook.GetSnapshot(time).data;
            rb.velocity = t_snapData.velocity;
        }


        protected override void ApplySnapshotData(Rigidbody2DData snapData)
        {
            // Special case. The only time data should be applied for a  matters for a rigidbody is when we resume play. Since there is no merit to applying rigidbody data when time is being rewound.
        }
        protected override Rigidbody2DData GatherCurrentData()
        {
            return new Rigidbody2DData(rb);
        }
        protected override Rigidbody2DDataSnapshot CreateSnapshot(Rigidbody2DData data, float time)
        {
            return new Rigidbody2DDataSnapshot(time, data, eInterpolationOption.Linear);
        }
    }
}