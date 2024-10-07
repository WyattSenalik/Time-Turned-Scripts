using UnityEngine;

//Original Authors - Bryce Cernohous-Schrader

namespace Atma
{
    public abstract class PickupBehavior : MonoBehaviour, IPickUpBehavior
    {
        /// <summary>Reference to the PickupObject this is attached to.</summary>
        protected IPickUpObject pickUpObject { get; private set; }
        /// <summary>Set to true OnPickUp and false OnRelease.</summary>
        public bool isHeld => pickUpObject.isHeld;

        protected virtual void Awake()
        {
            pickUpObject = GetComponentInParent<IPickUpObject>();
            #region ASSERTS
            //CustomDebug.AssertIComponentInParentIsNotNull(pickUpObject, this);
            #endregion ASSERTS
        }

        public virtual void TakeAction(bool onPressedOrReleased) { }

        public virtual void ThrowMe() { }
        public virtual void OnPickUp() { }
        public virtual void OnReleased() { }

    }
}