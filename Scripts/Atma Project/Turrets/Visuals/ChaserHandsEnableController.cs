using UnityEngine;

using Helpers.Extensions;
using Timed;
// Original Authors - Wyatt Senalik

namespace Atma
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(BoxPushingDetector))]
    [RequireComponent(typeof(DirectFollowModule))]
    [RequireComponent(typeof(CharacterMover))]
    public sealed class ChaserHandsEnableController : HandsEnableController
    {
        public BoxPushingDetector boxPushingDetector => m_boxPushingDetector;
        public DirectFollowModule followModule => m_followModule;

        private BoxPushingDetector m_boxPushingDetector = null;
        private DirectFollowModule m_followModule = null;
        private CharacterMover m_charMover = null;


        protected override void Awake()
        {
            base.Awake();

            m_boxPushingDetector = this.GetComponentSafe<BoxPushingDetector>();
            m_followModule = this.GetComponentSafe<DirectFollowModule>();
            m_charMover = this.GetComponentSafe<CharacterMover>();
        }

        protected override bool IsCarryingPickup() => false;
        protected override bool IsPlayingWindupAnimation() => false;
        protected override bool IsPlayerPushingBox() => boxPushingDetector.isABoxBeingPushed;
        protected override Vector2 GetMostRecentNonZeroMoveDirectionBeforeCurTime()
        {
            IReadOnlySnapshotScrapbook<Vector2Snapshot, Vector2> t_internalVel = m_charMover.internalVelocity.scrapbook;
            for (int i = t_internalVel.Count - 1; i >= 0; --i)
            {
                Vector2Snapshot t_curSnap = t_internalVel.GetSnapshotAtIndex(i);
                if (t_curSnap.data != Vector2.zero)
                {
                    return t_curSnap.data;
                }
            }
            return Vector2.down;
        }
        protected override IPickUpObject GetCurrentCarriedObject() => null;
        protected override IPickUpObject GetWindupObject() => null;
        protected override bool IsLoadingIntoLevel() => false;
    }
}