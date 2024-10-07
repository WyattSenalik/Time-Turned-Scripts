using System.Collections.Generic;
using UnityEngine;

using NaughtyAttributes;

using Helpers.Singletons;
using Helpers.Physics.Custom2D;
using Helpers.Physics.Custom2DInt;
using Helpers.Extensions;
using Timed;

namespace Atma
{
    [DisallowMultipleComponent]
    public sealed class BoxPhysicsManager : DynamicSingletonMonoBehaviour<BoxPhysicsManager>
    {
        private const bool IS_DEBUGGING = false;
        private const bool IS_DEBUGGING_DRAWING = false;
        private const bool IS_DEBUGGING_LEAPING = false;


        [SerializeField, Tag] private string m_cloneTag = "Clone";        
        [SerializeField, Tag] private string m_wallTag = "Wall";
        [SerializeField, Tag] private string m_pitTag = "Pit";

        private readonly List<PushableBoxCollider> m_boxColliders = new List<PushableBoxCollider>();
        private readonly List<PushableBoxCollider> m_pusherColliders = new List<PushableBoxCollider>();
        private readonly List<PushableBoxCollider> m_wallColliders = new List<PushableBoxCollider>();


        public void Register(PushableBoxCollider collider)
        {
            if (collider.isImmobile)
            {
                m_wallColliders.Add(collider);
            }
            else if (collider.isPusherOnly)
            {
                m_pusherColliders.Add(collider);
            }
            else
            {
                m_boxColliders.Add(collider);
            }
        }
        public void Unregister(PushableBoxCollider collider)
        {
            if (collider.isImmobile)
            {
                m_wallColliders.Remove(collider);
            }
            else if (collider.isPusherOnly)
            {
                m_pusherColliders.Remove(collider);
            }
            else
            {
                m_boxColliders.Remove(collider);
            }
        }

        public bool DoesRectangleOverlapWalls(RectangleInt rectangle) => DoesRectangleOverlapList(rectangle, m_wallColliders);
        public bool DoesRectangleOverlapBoxes(RectangleInt rectangle) => DoesRectangleOverlapList(rectangle, m_boxColliders);
        private bool DoesRectangleOverlapList(RectangleInt rectangle, List<PushableBoxCollider> colliderList)
        {
            CircleInt t_boundingCirc = rectangle.boundingCircle;
            foreach (PushableBoxCollider t_singleCol in colliderList)
            {
                RectangleInt t_curColRect = t_singleCol.rectangleCollider.rectangle;
                CircleInt t_curColBoundingCirc = t_curColRect.boundingCircle;
                if (CustomPhysics2DInt.CircleCircleOverlap(t_curColBoundingCirc, t_boundingCirc))
                {
                    if (CustomPhysics2DInt.RectangleRectangleOverlap(t_curColRect, rectangle))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        private bool DoesRectangleOverlapListWithTag(RectangleInt rectangle, List<PushableBoxCollider> colliderList, string tag)
        {
            CircleInt t_boundingCirc = rectangle.boundingCircle;
            foreach (PushableBoxCollider t_singleCol in colliderList)
            {
                RectangleInt t_curColRect = t_singleCol.rectangleCollider.rectangle;
                CircleInt t_curColBoundingCirc = t_curColRect.boundingCircle;
                if (CustomPhysics2DInt.CircleCircleOverlap(t_curColBoundingCirc, t_boundingCirc))
                {
                    if (CustomPhysics2DInt.RectangleRectangleOverlapIgnoreEdgeTouch(t_curColRect, rectangle))
                    {
                        if (t_singleCol.gameObject.CompareTag(tag))
                        {
                            //t_curColBoundingCirc.DrawDebug(Color.yellow, IS_DEBUGGING_LEAPING);
                            //t_curColRect.DrawOutlineDebug(Color.yellow, IS_DEBUGGING_LEAPING);
                            //t_boundingCirc.DrawDebug(Color.cyan, IS_DEBUGGING_LEAPING);
                            //rectangle.DrawOutlineDebug(Color.cyan, IS_DEBUGGING_LEAPING);
                            //CustomDebug.Log($"Hit {t_singleCol.gameObject.GetFullName()}", IS_DEBUGGING_LEAPING);
                            //CustomDebug.Break(IS_DEBUGGING);
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        private bool DoesRectangleOverlapListWithoutTag(RectangleInt rectangle, List<PushableBoxCollider> colliderList, string ignoreTag)
        {
            CircleInt t_boundingCirc = rectangle.boundingCircle;
            foreach (PushableBoxCollider t_singleCol in colliderList)
            {
                // Ignore colliders with the tag
                if (t_singleCol.gameObject.CompareTag(ignoreTag)) { continue; }

                RectangleInt t_curColRect = t_singleCol.rectangleCollider.rectangle;
                CircleInt t_curColBoundingCirc = t_curColRect.boundingCircle;
                if (CustomPhysics2DInt.CircleCircleOverlap(t_curColBoundingCirc, t_boundingCirc))
                {
                    if (CustomPhysics2DInt.RectangleRectangleOverlap(t_curColRect, rectangle))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public List<PushableBoxCollider> GetBoxesRectangleOverlapsIgnoreEdgeTouch(RectangleInt rectangle)
        {
            List<PushableBoxCollider> t_hitBoxes = new List<PushableBoxCollider>();
            CircleInt t_boundingCirc = rectangle.boundingCircle;
            foreach (PushableBoxCollider t_boxCol in m_boxColliders)
            {
                RectangleInt t_boxColRect = t_boxCol.rectangleCollider.rectangle;
                CircleInt t_boxColBoundingCirc = t_boxColRect.boundingCircle;
                if (CustomPhysics2DInt.CircleCircleOverlap(t_boxColBoundingCirc, t_boundingCirc))
                {
                    if (CustomPhysics2DInt.RectangleRectangleOverlapIgnoreEdgeTouch(t_boxColRect, rectangle))
                    {
                        t_hitBoxes.Add(t_boxCol);
                    }
                }
            }
            return t_hitBoxes;
        }
        public List<PushableBoxCollider> GetBoxesRectangleOverlapsIgnoreEdgeTouch(RectangleInt rectangle, List<PushableBoxCollider> ignoreColliders)
        {
            List<PushableBoxCollider> t_hitBoxes = new List<PushableBoxCollider>();
            CircleInt t_boundingCirc = rectangle.boundingCircle;
            foreach (PushableBoxCollider t_boxCol in m_boxColliders)
            {
                RectangleInt t_boxColRect = t_boxCol.rectangleCollider.rectangle;
                CircleInt t_boxColBoundingCirc = t_boxColRect.boundingCircle;
                if (CustomPhysics2DInt.CircleCircleOverlap(t_boxColBoundingCirc, t_boundingCirc))
                {
                    if (!ignoreColliders.Contains(t_boxCol))
                    {
                        if (CustomPhysics2DInt.RectangleRectangleOverlapIgnoreEdgeTouch(t_boxCol.rectangleCollider.rectangle, rectangle))
                        {
                            t_hitBoxes.Add(t_boxCol);
                        }
                    }
                }
            }
            return t_hitBoxes;
        }
        public bool DoesCheckBoxRectangleOverlapPushersAndHandleHitBlinkingClones(PushableBoxCollider boxCol, RectangleInt boxCheckRect, PushableBoxCollider pusherColToIgnore)
        {
            CircleInt t_boundingCirc = boxCheckRect.boundingCircle;
            for (int i = 0; i < m_pusherColliders.Count; ++i)
            {
                PushableBoxCollider t_singleCol = m_pusherColliders[i];
                if (t_singleCol != pusherColToIgnore)
                {
                    RectangleInt t_curColRect = t_singleCol.rectangleCollider.rectangle;
                    CircleInt t_curColBoundingCirc = t_curColRect.boundingCircle;
                    if (CustomPhysics2DInt.CircleCircleOverlap(t_curColBoundingCirc, t_boundingCirc))
                    {
                        if (CustomPhysics2DInt.RectangleRectangleOverlapIgnoreEdgeTouch(t_curColRect, boxCheckRect))
                        {
                            if (HandleBoxPotentiallyHittingABlinkingClone(t_singleCol, boxCol))
                            {
                                // Did hit a blinking clone, so just continue.
                            }
                            else
                            {
                                // Did not hit a blinking clone, so stop.
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }
        public bool DoesCheckBoxRectangleOverlapPushers(PushableBoxCollider boxCol, RectangleInt boxCheckRect, PushableBoxCollider pusherColToIgnore)
        {
            CircleInt t_boundingCirc = boxCheckRect.boundingCircle;
            for (int i = 0; i < m_pusherColliders.Count; ++i)
            {
                PushableBoxCollider t_singleCol = m_pusherColliders[i];
                if (t_singleCol != pusherColToIgnore)
                {
                    RectangleInt t_curColRect = t_singleCol.rectangleCollider.rectangle;
                    CircleInt t_curColBoundingCirc = t_curColRect.boundingCircle;
                    if (CustomPhysics2DInt.CircleCircleOverlap(t_curColBoundingCirc, t_boundingCirc))
                    {
                        if (CustomPhysics2DInt.RectangleRectangleOverlap(t_curColRect, boxCheckRect))
                        {
                            // Did not hit a blinking clone, so stop.
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        public bool IsRectangleInValidStandingOrFallingPosition(RectangleInt rectangle)
        {
            CircleInt t_boundingCirc = rectangle.boundingCircle;
            bool t_isPartiallyOverlappingPit = false;
            foreach (PushableBoxCollider t_singleCol in m_wallColliders)
            {
                RectangleInt t_curColRect = t_singleCol.rectangleCollider.rectangle;
                CircleInt t_curColBoundingCirc = t_curColRect.boundingCircle;

                if (CustomPhysics2DInt.CircleCircleOverlap(t_curColBoundingCirc, t_boundingCirc))
                {
                    // Pit wall check. Its only invalid if rectangle is partially in the wall, its okay if its either not overlapping the other collider at all, or if the other collider completely contains the rectangle collider.
                    if (t_singleCol.gameObject.CompareTag(m_pitTag))
                    {
                        if (CustomPhysics2DInt.DoesRectangleEncapsulateRectangle(t_curColRect, rectangle))
                        {
                            // We know this is a valid place, because the player can fall here.
                            return true;
                        }
                        else
                        {
                            t_isPartiallyOverlappingPit = true;
                        }
                    }
                    // Normal wall check
                    else
                    {

                        if (CustomPhysics2DInt.RectangleRectangleOverlap(t_curColRect, rectangle))
                        {
                            return false;
                        }
                    }
                }
            }
            return !t_isPartiallyOverlappingPit;
        }


        /// <summary>
        /// Returns true when a blinking clone was hit.
        /// </summary>
        private bool HandleBoxPotentiallyHittingABlinkingClone(PushableBoxCollider hitCollider, PushableBoxCollider boxCollider)
        {
            if (!hitCollider.isPusherOnly) { return false; }

            // Check if its structure is that of a clone.
            Transform t_hitColParent = hitCollider.transform.parent;
            if (t_hitColParent == null) { return false; }
            Transform t_hitColGrandparent = t_hitColParent.parent;
            if (t_hitColGrandparent == null) { return false; }
            // Check tag
            if (!t_hitColGrandparent.gameObject.CompareTag(m_cloneTag)) { return false; }

            // Yes its a clone. Is it blinking?
            TimeCloneBlink t_cloneBlink = t_hitColGrandparent.GetComponentSafe<TimeCloneBlink>();
            if (!t_cloneBlink.isBlinking) { return false; }
            // It IS blinking
            TimeCloneHealth t_cloneHealth = t_hitColGrandparent.GetComponentSafe<TimeCloneHealth>();
            t_cloneHealth.TakeDamage(new DamageContext(GlobalTimeManager.instance.curTime, boxCollider.rootTransform.gameObject));
            return true;
        }


        /// <summary>
        /// Door collider is assumed to be in the wall's list.
        /// </summary>
        public bool IsTherePusherOrBoxOnDoor(PushableBoxCollider doorCollider)
        {
            RectangleInt t_doorRectangle = doorCollider.rectangleCollider.rectangle;
            foreach (PushableBoxCollider t_pusherCol in m_pusherColliders)
            {
                RectangleInt t_pusherRectangle = t_pusherCol.rectangleCollider.rectangle;
                if (CustomPhysics2DInt.RectangleRectangleOverlapIgnoreEdgeTouch(t_doorRectangle, t_pusherRectangle))
                {
                    return true;
                }
            }
            foreach (PushableBoxCollider t_boxCol in m_boxColliders)
            {
                RectangleInt t_boxRectangle = t_boxCol.rectangleCollider.rectangle;
                if (CustomPhysics2DInt.RectangleRectangleOverlapIgnoreEdgeTouch(t_doorRectangle, t_boxRectangle))
                {
                    return true;
                }
            }

            return false;
        }
        public bool RaycastToHitWalls(Vector2Int origin, Vector2 direction, int distance)
        {
            Vector2 t_originFloat = CustomPhysics2DInt.ConvertIntPositionToFloatPosition(origin);
            float t_distFloat = CustomPhysics2DInt.INVERTED_INTS_PER_UNIT_AS_FLOAT * distance;
            Vector2Int t_endPos = CustomPhysics2DInt.ConvertFloatPositionToIntPosition(t_originFloat + direction * t_distFloat);
            LineInt t_ray = new LineInt(origin, t_endPos);

            foreach (PushableBoxCollider t_wallCol in m_wallColliders)
            {
                if (CustomPhysics2DInt.LineRectangleOverlap(t_ray, t_wallCol.rectangleCollider.rectangle))
                {
                    //t_wallCol.rectangleCollider.rectangle.DrawOutlineDebug(Color.magenta, true);
                    //t_ray.DrawDebug(Color.red, true, false);
                    //CustomDebug.DrawCrossHair(t_ray.GetPoint2AsFloatPosition(), 0.1f, Color.red, IS_DEBUGGING_DRAWING);
                    return true;
                }
            }
            //t_ray.DrawDebug(Color.cyan, true, false);
            //CustomDebug.DrawCircle(t_ray.point1, 1.0f, 16, Color.yellow, IS_DEBUGGING_DRAWING);
            return false;
        }

        public bool DoesRectangleOverlapWallWithWallTag(RectangleInt rectangle) => DoesRectangleOverlapListWithTag(rectangle, m_wallColliders, m_wallTag);
        public bool DoesRectangleOverlapWallWithoutPitTag(RectangleInt rectangle) => DoesRectangleOverlapListWithoutTag(rectangle, m_wallColliders, m_pitTag);



        public sealed class Hit
        {
            /// <summary>Collider hit.</summary>
            public PushableBoxCollider collider { get; private set; }
            /// <summary>Distance moved before hit.</summary>
            public float distance { get; private set; }
            /// <summary>World point where the hit occurred.</summary>
            public Vector2 hitPointOnMover { get; private set; }
            /// <summary>World point where the hit occurred.</summary>
            public Vector2 hitPointOnHitCollider { get; private set; }
            /// <summary>Index of the edge on the hit collider that was hit.</summary>
            public int hitEdgeIndex { get; private set; }


            public Hit(PushableBoxCollider collider, float distance, Vector2 hitPointOnMover, Vector2 hitPointOnHitCollider, int hitEdgeIndex)
            {
                this.collider = collider;
                this.distance = distance;
                this.hitPointOnMover = hitPointOnMover;
                this.hitPointOnHitCollider = hitPointOnHitCollider;
                this.hitEdgeIndex = hitEdgeIndex;
            }
        }
    }
}