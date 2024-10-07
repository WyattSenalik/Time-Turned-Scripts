using Helpers.Physics.Custom2D;
using Helpers.Singletons;
using System.Collections.Generic;
using UnityEngine;

namespace Atma
{
    [DisallowMultipleComponent]
    public sealed class BulletPhysicsManager : DynamicSingletonMonoBehaviour<BulletPhysicsManager>
    {
        private const bool IS_DEBUGGING = true;

        private readonly List<BulletCollider> m_mobileColliders = new List<BulletCollider>();
        private readonly List<BulletCollider> m_staticColliders = new List<BulletCollider>();


        public void RegisterCollider(BulletCollider colliderToAdd)
        {
            if (colliderToAdd.isStatic)
            {
                m_staticColliders.Add(colliderToAdd);
            }
            else
            {
                m_mobileColliders.Add(colliderToAdd);
            }
        }
        public void UnregisterCollider(BulletCollider colliderToRemove)
        {
            if (colliderToRemove.isStatic)
            {
                m_staticColliders.Remove(colliderToRemove);
            }
            else
            {
                m_mobileColliders.Remove(colliderToRemove);
            }
        }

        public int CircleCastNonAlloc(Vector2 origin, float radius, Vector2 direction, Hit[] preallocatedHits, float distance, List<BulletCollider> staticCollidersOverride = null)
        {
            Circle t_originCircle = new Circle(origin, radius);
            Circle t_castBoundingCircle = CustomPhysics2D.CreateMinimumBoundingCircleForCircleCast(t_originCircle, direction, distance);

            bool t_checkAll = false;
            byte t_horiCheck = 0;
            byte t_vertCheck = 0;
            // If bullet is diagonal, we want to check all the edges no matter which dir they are supposed to be.
            if (direction.x != 0.0f && direction.y != 0.0f)
            {
                t_checkAll = true;
            }
            else
            {
                if (direction.x > 0.0f)
                {
                    // Bullet is going right, so check the left edge
                    t_horiCheck = 1;
                }
                else if (direction.x < 0.0f)
                {
                    // Bullet is going left, so check the right edge
                    t_horiCheck = 2;
                }

                if (direction.y > 0.0f)
                {
                    // Bullet is going up, so check the bottom edge
                    t_vertCheck = 1;
                }
                else if (direction.y < 0.0f)
                {
                    // Bullet is going down, so check the top edge
                    t_vertCheck = 2;
                }
            }

            int t_curIndex = 0;
            // Check the moving colliders
            if (CheckColliderList(m_mobileColliders, preallocatedHits, direction, distance, t_originCircle, t_castBoundingCircle, t_checkAll, t_horiCheck, t_vertCheck, ref t_curIndex))
            {
                return t_curIndex;
            }

            // Now check the static colliders
            if (staticCollidersOverride != null)
            {
                // Use the override static colliders
                if (CheckColliderList(staticCollidersOverride, preallocatedHits, direction, distance, t_originCircle, t_castBoundingCircle, t_checkAll, t_horiCheck, t_vertCheck, ref t_curIndex))
                {
                    return t_curIndex;
                }
            }
            else
            {
                // Just use the normal static colliders.
                if (CheckColliderList(m_staticColliders, preallocatedHits, direction, distance, t_originCircle, t_castBoundingCircle, t_checkAll, t_horiCheck, t_vertCheck, ref t_curIndex))
                {
                    return t_curIndex;
                }
            }

            return t_curIndex;
        }

        private static bool CheckColliderList(List<BulletCollider> colliderList, Hit[] preallocatedHits, Vector2 direction, float distance, Circle originCircle, Circle castBoundingCircle, bool checkAll, byte horiCheck, byte vertCheck, ref int curIndex)
        {
            foreach (BulletCollider t_col in colliderList)
            {
                if (!t_col.enabled) {  continue; }
                if (CircleCastSingleCollider(t_col, preallocatedHits, direction, distance, originCircle, castBoundingCircle, checkAll, horiCheck, vertCheck, ref curIndex))
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// Returns if exceeded the amount of hits we can find.
        /// </summary>
        private static bool CircleCastSingleCollider(BulletCollider collider, Hit[] preallocatedHits, Vector2 direction, float distance, Circle originCircle, Circle castBoundingCircle, bool checkAll, byte horiCheck, byte vertCheck, ref int curIndex)
        {
            if (!CustomPhysics2D.CircleCircleOverlap(collider.boundingCircle, castBoundingCircle))
            {
                // They aren't even close enough to potentially collide
                return false;
            }

            if (collider.checkContained)
            {
                // Check if there is any overlap with the contained rectangle.
                Rectangle t_containedRectangle = collider.GetContainingRectangle();
                if (CustomPhysics2D.CircleCastToRectangle(originCircle, direction, distance, t_containedRectangle, out float t_dist, IS_DEBUGGING))
                {
                    preallocatedHits[curIndex] = new Hit(t_dist, collider);
                    if (++curIndex >= preallocatedHits.Length)
                    {
                        return true;
                    }
                    return false;
                }
            }
            if (checkAll)
            {
                // Check all edges
                foreach (Line t_edge in collider.edges)
                {
                    if (CustomPhysics2D.CircleCastToLine(originCircle, direction, distance, t_edge, out float t_dist, IS_DEBUGGING))
                    {
                        preallocatedHits[curIndex] = new Hit(t_dist, collider);
                        if (++curIndex >= preallocatedHits.Length)
                        {
                            return true;
                        }
                    }
                }
            }
            else
            {
                // Check the edges
                if (horiCheck > 0)
                {
                    Line t_horiLine = horiCheck == 1 ? collider.leftEdgeLine : collider.rightEdgeLine;
                    if (CustomPhysics2D.CircleCastToLine(originCircle, direction, distance, t_horiLine, out float t_dist, IS_DEBUGGING))
                    {
                        preallocatedHits[curIndex] = new Hit(t_dist, collider);
                        if (++curIndex >= preallocatedHits.Length)
                        {
                            return true;
                        }
                    }
                }
                if (vertCheck > 0)
                {
                    Line t_vertLine = vertCheck == 1 ? collider.downEdgeLine : collider.upEdgeLine;
                    if (CustomPhysics2D.CircleCastToLine(originCircle, direction, distance, t_vertLine, out float t_dist, IS_DEBUGGING))
                    {
                        preallocatedHits[curIndex] = new Hit(t_dist, collider);
                        if (++curIndex >= preallocatedHits.Length)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public struct Hit
        {
            /// <summary>Distance from the cast that the collision occurred</summary>
            public float distance { get; private set; }
            public BulletCollider collider { get; private set; }

            public Hit(float distance, BulletCollider collider)
            {
                this.distance = distance;
                this.collider = collider;
            }
        }
    }
}