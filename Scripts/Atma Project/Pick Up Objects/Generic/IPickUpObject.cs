using UnityEngine;

using Helpers.Events;
using Helpers.UnityInterfaces;
// Original Authors - Bryce Cernohous-Schrader
// Tweaked by Wyatt Senalik

namespace Atma
{
    public interface IPickUpObject : IMonoBehaviour
    {
        bool shouldChangeCursorIcon { get; }

        IEventPrimer onPickup { get; }
        IEventPrimer onReleased { get; }

        /// <summary>Normalized direction that the pick up object is being held in.</summary>
        Vector2 direction { get; set; }

        /// <summary>Normalized direction the item should be aimed.</summary>
        Vector2 aimDirection { get; }

        Rigidbody2D rb2D { get; }
        float holdingRange { get; }
        bool isHeld { get;  }
        /// <summary>
        /// If the pickup object is available to be picked up. Is not related to isHeld. If an object isHeld, this value may still be true.
        /// </summary>
        bool canBePickedUp { get; }
        /// <summary>
        /// If the pickup object has an action associated with it.
        /// </summary>
        bool hasAction { get; }
        /// <summary>
        /// If the pickup object is one that can be thrown or if it is simply dropped.
        /// </summary>
        eReleaseType releaseType { get; }
        /// <summary>
        /// What is currently holding the pickup (not recorded).
        /// </summary>
        IPickUpHolder holder { get; }
        /// <summary>
        /// Where the hand of the holder should be placed.
        /// </summary>
        Vector2 handPlacementPosition { get; }

        void ThrowMe();

        void TakeAction(bool onPressedOrReleased);
        void OnPickUp(IPickUpHolder holder);
        void OnReleased();

        int RequestDisallowPickup();
        void CancelDisallowPickupRequest(int requestID);

        /// <summary>
        /// Updates the position of the pickup object based on its holding range and current direction.
        /// </summary>
        void UpdateCarryObjPosition(Vector2 parentPos, float parentSize);


        public enum eReleaseType { Throw, Drop }
    }
}