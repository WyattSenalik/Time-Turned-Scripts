using UnityEngine;
// Original Authors - Wyatt Senalik

namespace Atma
{
    /// <summary>
    /// <see cref="ThrowBehavior"/> for items that are meant to be dropped not thrown.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class DropBehaviour : PickupBehavior
    {
        [SerializeField] private Vector2 m_dropOffset = Vector3.zero;

        public override void ThrowMe()
        {
            base.ThrowMe();

            transform.parent = null;
            transform.localScale = Vector3.one;
            Vector2 t_pos2D = transform.localPosition;
            transform.localPosition = t_pos2D + m_dropOffset;
        }
    }
}