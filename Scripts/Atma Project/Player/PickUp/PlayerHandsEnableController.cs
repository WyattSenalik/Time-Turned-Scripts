using NaughtyAttributes;
using UnityEngine;
// Original Authors - Wyatt Senalik

namespace Atma
{
    [RequireComponent(typeof(PickupController))]
    [RequireComponent(typeof(PlayerMovement))]
    [RequireComponent(typeof(BoxPushingDetector))]
    [RequireComponent(typeof(HoldHandPositionController))]
    [DisallowMultipleComponent]
    public sealed class PlayerHandsEnableController : HandsEnableController
    {
        public PickupController pickupCont { get; private set; }
        public PlayerMovement playerMovement { get; private set; }
        public BoxPushingDetector boxPushingDetector { get; private set; }
        public HoldHandPositionController holdHandPosCont { get; private set; }

        [SerializeField] private AppearFromFloorPortal m_appearFromFloorPortal = null;


        protected override void Awake()
        {
            base.Awake();

            pickupCont = GetComponent<PickupController>();
            playerMovement = GetComponent<PlayerMovement>();
            boxPushingDetector = GetComponent<BoxPushingDetector>();
            holdHandPosCont = GetComponent<HoldHandPositionController>();
            #region Asserts
            //CustomDebug.AssertComponentIsNotNull(pickupCont, this);
            //CustomDebug.AssertComponentIsNotNull(playerMovement, this);
            //CustomDebug.AssertComponentIsNotNull(boxPushingDetector, this);
            //CustomDebug.AssertComponentIsNotNull(holdHandPosCont, this);
            #endregion Asserts
        }

        protected override bool IsCarryingPickup()
        {
            if (pickupCont.isCarrying)
            {
                bool t_isHat = pickupCont.curCarriedObj.releaseType == IPickUpObject.eReleaseType.Drop;
                if (t_isHat)
                {
                    return false;
                }
                return true;
            }
            return false;
        }
        protected override bool IsPlayingWindupAnimation() => holdHandPosCont.isPlayingWindupAnim;
        protected override bool IsPlayerPushingBox() => boxPushingDetector.isABoxBeingPushed;
        protected override Vector2 GetMostRecentNonZeroMoveDirectionBeforeCurTime() => playerMovement.GetMostRecentNonZeroMoveDirectionBeforeCurTime();
        protected override IPickUpObject GetCurrentCarriedObject() => pickupCont.curCarriedObj;
        protected override IPickUpObject GetWindupObject() => holdHandPosCont.throwingObj;
        protected override bool IsLoadingIntoLevel()
        {
            if (m_appearFromFloorPortal == null)
            {
                return false;
            }
            return m_appearFromFloorPortal.isLoadingIntoLevel;
        }
    }
}