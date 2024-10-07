using System.Collections.Generic;
using UnityEngine;

namespace Helpers.Physics.Custom2D
{
    public static class CustomPhysics2D
    {
        public static bool CircleCircleOverlap(Circle a, Circle b)
        {
            float t_sqDist = (a.center - b.center).sqrMagnitude;
            float t_sum = a.radius + b.radius;
            return t_sqDist <= t_sum * t_sum;

            //float t_distance = Vector2.Distance(a.center, b.center);
            //return t_distance <= a.radius + b.radius;
        }
        
        public static bool RectangleRectangleOverlap(Rectangle a, Rectangle b)
        {
            return a.min.x <= b.max.x && a.max.x >= b.min.x && a.min.y <= b.max.y && a.max.y >= b.min.y;
        }
        public static bool RectangleRectangleOverlapAllowEdgeTouch(Rectangle a, Rectangle b)
        {
            return a.min.x < b.max.x && a.max.x > b.min.x && a.min.y < b.max.y && a.max.y > b.min.y;
        }

        public static bool CircleLineOverlap(Circle circle, Line line, bool isDebugging = false) => CircleLineOverlap(circle, line, out _, out _, isDebugging);
        public static bool CircleLineOverlap(Circle circle, Line line, out Vector2? intersectionPoint1, out Vector2? intersectionPoint2, bool isDebugging = false)
        {
            // (We make an assumption that the circle is at the origin to simplify the math, because of this, we just need to adjust the line's points to counterbalance that).
            Vector2 t_adjP1 = line.point1 - circle.center;
            Vector2 t_adjP2 = line.point2 - circle.center;
            float r = circle.radius;
            float t_sqrtInside;

            if (line.point1.x == line.point2.x)
            {
                // The line is completely vertical, we'll need to do a special check and not the one below.
                // Since the line is completly vertical, solve the circle equation: r^2 = x^2 + y^2 for y (since we know r and x)
                // We get: y = +/- sqrt(r^2 - x^2)
                // Check if the inside of the sqrt is positive or negative first.
                float x = t_adjP1.x;
                t_sqrtInside = r * r - x * x;
                if (t_sqrtInside >= 0.0f)
                {
                    // Okay, so if the line was infinite, we know for sure these overlap, but the line is not infinite, so we need to now find the 2 intersection points and figure out if they are a part of the line segment.
                    float t_sqrt = Mathf.Sqrt(t_sqrtInside);
                    Vector2 t_intersectOffset1 = new Vector2(x, t_sqrt);
                    Vector2 t_intersectOffset2 = new Vector2(x, -t_sqrt);
                    intersectionPoint1 = circle.center + t_intersectOffset1;
                    intersectionPoint2 = circle.center + t_intersectOffset2;
                    //DrawIntersectPoint(intersectionPoint1.Value, isDebugging);
                    //DrawIntersectPoint(intersectionPoint2.Value, isDebugging);
                    // If a dot is negative it means that point is on the line.
                    Line t_adjLine = new Line(t_adjP1, t_adjP2);
                    if (!IsInfiniteLinePointOnLineSegment(t_adjLine, t_intersectOffset1))
                    {
                        intersectionPoint1 = null;
                    }
                    if (!IsInfiniteLinePointOnLineSegment(t_adjLine, t_intersectOffset2))
                    {
                        intersectionPoint2 = null;
                    }
                    return intersectionPoint1.HasValue || intersectionPoint2.HasValue;
                }
                else
                {
                    // Negative inside of square root, that means imaginary value, so no overlap.
                    intersectionPoint1 = null;
                    intersectionPoint2 = null;
                    return false;
                }
            }

            // To figure out if the circle and line have intersecting points, we will use the quadratic equation.
            // Circle equation is r^2 = y^2 + x^2
            // Line equation is y = m * x + b
            // Setting these equal to each other simplifies to 0 = (m^2 + 1) * x^2 + (2 * m * b) * x + (b^2 - r^2)
            // Which can be solved using the quadratic equation:
            // x = (-2b +/- sqrt(b^2 - 4 * a * c)) / (2 * a) where 0 = a * x^2 + b * x + c
            // Plugging our values in we get:
            // x = -(2 * m * b +/- sqrt(4 * m^2 * b^2 - 4 * (m^2 + 1) * (b^2 - r^2))) / (2 * (m^2 + 1))
            // And to tell if they intersect or not, we can check if the inside of the square root is positive or negative. If negative, they don't overlap. If positive they do.
            float t_run = t_adjP1.x - t_adjP2.x;
            float t_rise = t_adjP1.y - t_adjP2.y;
            float m = t_rise / t_run;
            float b = -m * t_adjP1.x + t_adjP1.y;
            float q = m * m + 1;
            t_sqrtInside = (q * r * r) - (b * b);
            if (t_sqrtInside >= 0.0f)
            {
                // Okay, so if the line was infinite, we know for sure these overlap, but the line is not infinite, so we need to now find the 2 intersection points and figure out if they are a part of the line segment.
                float t_2b = -m * b;
                float t_invertedQ = 1.0f / q;
                float t_sqrt = Mathf.Sqrt(t_sqrtInside);
                float t_intersectX1 = (t_2b + t_sqrt) * t_invertedQ;
                float t_intersectX2 = (t_2b - t_sqrt) * t_invertedQ;
                Vector2 t_intersectOffset1 = new Vector2(t_intersectX1, m * t_intersectX1 + b);
                Vector2 t_intersectOffset2 = new Vector2(t_intersectX2, m * t_intersectX2 + b);
                intersectionPoint1 = circle.center + t_intersectOffset1;
                intersectionPoint2 = circle.center + t_intersectOffset2;
                // If a dot is negative it means that point is on the line.
                Line t_adjLine = new Line(t_adjP1, t_adjP2);
                if (!IsInfiniteLinePointOnLineSegment(t_adjLine, t_intersectOffset1))
                {
                    intersectionPoint1 = null;
                }
                if (!IsInfiniteLinePointOnLineSegment(t_adjLine, t_intersectOffset2))
                {
                    intersectionPoint2 = null;
                }

                //if (intersectionPoint1.HasValue)
                //{
                //    DrawIntersectPoint(intersectionPoint1.Value, isDebugging);
                //}
                //if (intersectionPoint2.HasValue)
                //{
                //    DrawIntersectPoint(intersectionPoint2.Value, isDebugging);
                //}

                return intersectionPoint1.HasValue || intersectionPoint2.HasValue;
            }
            else
            {
                // Negative inside of square root, that means imaginary value, so no overlap.
                intersectionPoint1 = null;
                intersectionPoint2 = null;
                return false;
            }
        }
        public static bool DoesLineStartOnCircleEdgeAndNotPassInsideCircle(Circle circle, Line line, bool isDebugging = false)
        {
            Vector2 t_nearPoint = NearestPointOnLine(line, circle.center);
            if (t_nearPoint != line.point1 && t_nearPoint != line.point2)
            {
                // Line does not start on circle edge.
                return false;
            }

            if (CircleLineOverlap(circle, line, out Vector2? t_ray0Intersect1, out Vector2? t_ray0Intersect2, isDebugging))
            {
                // Only try to find line intersection if the intersection points are the same, or there is only 1 intersection.
                bool t_only1Intersetion = t_ray0Intersect1.HasValue != t_ray0Intersect2.HasValue;
                bool t_bothPointsAreSame = t_ray0Intersect1.HasValue && t_ray0Intersect2.HasValue && t_ray0Intersect1.Value == t_ray0Intersect2.Value;
                if (t_only1Intersetion || t_bothPointsAreSame)
                {
                    // Only 1 intersection point
                    return true;
                }
                else
                {
                    // More than 1 intersection point
                    return false;
                }
            }
            else
            {
                // No intersections
                return true;
            }
        }

        public static bool CircleBoundaryOverlap(Circle circle, Line boundaryLine)
        {
            Vector2 t_diff = boundaryLine.center - circle.center;
            float t_dot = Vector2.Dot(t_diff, boundaryLine.normal);
            // If dot is zero or negative, then its not overlapping
            if (t_dot <= 0)
            {
                return false;
            }
            // Boundary is an line of infinite lengthand also extends infinitely backwards. So its definitely beyound the boundary
            return true;
        }

        public static bool CapsuleLineOverlap(Capsule capsule, Line line, bool isDebugging = false) => CapsuleLineOverlap(capsule, line, out _, out _, isDebugging);
        public static bool CapsuleLineOverlap(Capsule capsule, Line line, out float distance, bool isDebugging = false) => CapsuleLineOverlap(capsule, line, out distance, out _, isDebugging);
        public static bool CapsuleLineOverlap(Capsule capsule, Line line, out float distance, out Vector2 intersectionPoint, bool isDebugging = false)
        {
            // Find the shortest distance between the lines and if that's within the capsule radius, we're good.
            distance = FindMinimumDistanceBetweenLines(line, capsule.innerLine, out intersectionPoint, out _);

            if (distance <= capsule.radius)
            {
                //DrawIntersectPoint(intersectionPoint, isDebugging);
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool CapsuleRectangleOverlap(Capsule capsule, Rectangle rectangle, bool isDebugging = false)
        {
            // There are 4 cases:
            // 1: The capsule is fully contained by the rectangle (no edges touching)
            // 2: The rectangle is fully contained by the capsule (no edges touching)
            // 3: The capsule and rectangle are touching edges (edges are intersecting)
            // 4: Neither are overlapping at all.

            // 1. Checking if the capsule is fully contained by the rectangle is very easy, just check if the capsule's center is in the rectangle.
            if (IsPointInRectangle(rectangle, capsule.center))
            {
                return true;
            }

            // 2. To check if the rectangle is fully contained by the capsule, we'll check how far the rectangle's center is away from the capsule's line.
            float t_rectDistFromCap = DistanceFromLine(capsule.innerLine, rectangle.center);
            if (t_rectDistFromCap <= capsule.radius)
            {
                return true;
            }

            // 3. To check if the capsule and rectangle are checking edges, we'll check each edge of the rectangle against the capsule.
            foreach (Line t_edge in rectangle.edges)
            {
                if (CapsuleLineOverlap(capsule, t_edge, out float t_curDist, isDebugging))
                {
                    return true;
                }
            }

            // 4. No overlap
            return false;
        }
        public static bool CapsuleRectangleOverlap(Capsule capsule, Rectangle rectangle, out float dist, bool isDebugging = false)
        {
            // There are 4 cases:
            // 1: The capsule is fully contained by the rectangle (no edges touching)
            // 2: The rectangle is fully contained by the capsule (no edges touching)
            // 3: The capsule and rectangle are touching edges (edges are intersecting)
            // 4: Neither are overlapping at all.

            // 1. Checking if the capsule is fully contained by the rectangle is very easy, just check if the capsule's center is in the rectangle.
            if (IsPointInRectangle(rectangle, capsule.center))
            {
                // TODO Calculate Dist?
                dist = float.NaN;
                return true;
            }

            // 2. To check if the rectangle is fully contained by the capsule, we'll check how far the rectangle's center is away from the capsule's line.
            float t_rectDistFromCap = DistanceFromLine(capsule.innerLine, rectangle.center);
            if (t_rectDistFromCap <= capsule.radius)
            {
                // TODO Calculate Dist?
                dist = float.NaN;
                return true;
            }

            // 3. To check if the capsule and rectangle are checking edges, we'll check each edge of the rectangle against the capsule.
            float t_minDist = float.PositiveInfinity;
            Vector2 t_nearestIntersectionPoint = new Vector2(float.NaN, float.NaN);
            foreach (Line t_edge in rectangle.edges)
            {
                if (CapsuleLineOverlap(capsule, t_edge, out float t_curDist, out Vector2 t_intersectionPoint, isDebugging))
                {
                    if (t_curDist < t_minDist)
                    {
                        t_minDist = t_curDist;
                        t_nearestIntersectionPoint = t_intersectionPoint;
                    }
                }
            }
            if (t_minDist != float.PositiveInfinity)
            {
                dist = t_minDist;
                //DrawIntersectPoint(t_nearestIntersectionPoint, Color.cyan, isDebugging);
                return true;
            }

            // 4. No overlap
            dist = float.NaN;
            return false;
        }

        public static bool CircleRectangleOverlap(Circle circle, Rectangle rectangle, bool isDebugging = false)
        {
            // 4 Cases:
            // 1. Rectangle is inside circle
            // 2. Circle is inside rectangle
            // 3. Circle and rectangle have their outlines intersecting.
            // 4. Not overlapping at all.

            // 1. Is the rectangle in the circle?
            if (IsPointInCircle(circle, rectangle.center))
            {
                return true;
            }
            
            // 2. Is the circle in the rectangle?
            if (IsPointInRectangle(rectangle, circle.center))
            {
                return true;
            }

            // 3. Are the outlines intersecting?
            foreach (Line t_edge in rectangle.edges)
            {
                if (CircleLineOverlap(circle, t_edge, isDebugging))
                {
                    return true;
                }
            }

            // 4. Not overlapping.
            return false;
        }

        public static bool RectangleLineOverlap(Rectangle rectangle, Line line, bool isDebugging = false)
        {
            // Case 1: Line completely is inside rectangle.
            if (IsPointInRectangle(rectangle, line.point1))
            {
                return true;
            }
            // Case 2: There is an intersection between rectangle edges and line.
            foreach (Line t_edge in rectangle.edges)
            {
                if (FindLineIntersection(t_edge, line, out Vector2 t_intersectPoint, isDebugging))
                {
                    //DrawIntersectPoint(t_intersectPoint, Color.magenta, isDebugging);
                    return true;
                }
            }
            // Case 3: There is no overlap.
            return false;
        }
        public static bool RectangleLineClosestIntersect(Rectangle rectangle, Line line, out Vector2 intersectPoint, bool isDebugging = false)
        {
            intersectPoint = new Vector2(float.NaN, float.NaN);
            if (RectangleLineIntersect(rectangle, line, out List<Vector2> t_intersectPoints, isDebugging))
            {
                float t_closestSqDist = float.PositiveInfinity;
                foreach (Vector2 t_point in t_intersectPoints)
                {
                    float t_curSqDist = (rectangle.center - t_point).sqrMagnitude;
                    if (t_curSqDist < t_closestSqDist)
                    {
                        t_closestSqDist = t_curSqDist;
                        intersectPoint = t_point;
                    }
                }
                if (!float.IsPositiveInfinity(t_closestSqDist))
                {
                    return true;
                }
            }
            return false;
        }
        public static bool RectangleLineIntersect(Rectangle rectangle, Line line, out List<Vector2> intersectPoints, bool isDebugging = false)
        {
            intersectPoints = new List<Vector2>();
            // Case 1: Line completely is inside rectangle.
            if (IsPointInRectangle(rectangle, line.point1) && IsPointInRectangle(rectangle, line.point2))
            {
                return true;
            }
            // Case 2: There is an intersection between rectangle edges and line.
            foreach (Line t_edge in rectangle.edges)
            {
                if (FindLineIntersection(t_edge, line, out Vector2 t_intersectPoint, isDebugging))
                {
                    //DrawIntersectPoint(t_intersectPoint, Color.magenta, isDebugging);
                    intersectPoints.Add(t_intersectPoint);
                }
            }
            if (intersectPoints.Count > 0)
            {
                return true;
            }
            // Case 3: There is no overlap.
            return false;
        }

        // TODO: THERE IS A BUG WITH WHEN CircleCastToRectangle and CircleCastToLine HIT A CORNER. IT RETURNS INCORRECT HIT POSITIONS AND BY EXTENSION INCORRECT HIT DISTANCES.
        /// <summary>Casts a circle in the direction given for the max distance. If it intersects the rectangle at all, returns true.</summary>
        /// <param name="hitDistance">Amount moved in direction before hit occurred.</param>
        public static bool CircleCastToRectangle(Circle circle, Vector2 dir, float maxDistance, Rectangle rectangle, out float hitDistance, bool isDebugging = false) => CircleCastToRectangle(circle, dir, maxDistance, rectangle, out hitDistance, out _, out _, isDebugging);
        /// <summary>Casts a circle in the direction given for the max distance. If it intersects the rectangle at all, returns true.</summary>
        /// <param name="hitDistance">Amount moved in direction before hit occurred.</param>
        public static bool CircleCastToRectangle(Circle circle, Vector2 dir, float maxDistance, Rectangle rectangle, out float hitDistance, out Vector2 hitPointOnCircle, out Vector2 hitPointOnRectangle, bool isDebugging = false)
        {
            // Cases:
            // 1. Rectangle starts inside the circle
            // 2. Circle starts inside rectangle
            // 3. Circle and rectangle have their outlines intersecting (overlap).
            // 4. Circle after it moves (capsule) intersects the rectangle.
            // 5. Not overlapping at all.

            // 1, 2, and 3 are all handled by this.
            // If they are already overlapping, don't need to cast.
            if (CircleRectangleOverlap(circle, rectangle))
            {
                hitDistance = 0.0f;
                hitPointOnCircle = circle.center;
                hitPointOnRectangle = rectangle.center;
                return true;
            }
            // 4. Does casting the circle cause it to intersect the rectangle?
            Capsule t_circleCastedAsCapsule = new Capsule(circle.center, circle.center + dir * maxDistance, circle.radius);
            List<Line> t_edgesToCheck = new List<Line>();
            List<int> t_cornerIndicesToCheck = new List<int>();
            for (int i = 0; i < 4; ++i)
            {
                float t_dot = Vector2.Dot(rectangle.edges[i].normal, dir);
                if (t_dot < 0)
                {
                    t_edgesToCheck.Add(rectangle.edges[i]);
                    // Add corner indices.
                    if (!t_cornerIndicesToCheck.Contains(i))
                    {
                        t_cornerIndicesToCheck.Add(i);
                    }
                    int t_otherCornerToEdgeIndex = i + 1 < 4 ? i + 1 : 0;
                    if (!t_cornerIndicesToCheck.Contains(t_otherCornerToEdgeIndex))
                    {
                        t_cornerIndicesToCheck.Add(t_otherCornerToEdgeIndex);
                    }
                }
            }
            if (t_edgesToCheck.Count <= 0)
            {
                hitDistance = 0.0f;
                hitPointOnCircle = new Vector2(float.NaN, float.NaN);
                hitPointOnRectangle = new Vector2(float.NaN, float.NaN);
                return false;
            }

            bool t_didAnyIntersect = false;
            bool[] t_didIntersect = new bool[t_edgesToCheck.Count];
            Vector2[] t_circlePoints = new Vector2[t_edgesToCheck.Count];
            Vector2[] t_nearPoints = new Vector2[t_edgesToCheck.Count];
            for (int i = 0; i < t_edgesToCheck.Count; ++i)
            {
                /// 4b. Get the normal for the edge to check.
                Line t_edge = t_edgesToCheck[i];
                //t_edge.DrawDebug(Color.magenta, isDebugging);
                Vector2 t_edgeN = t_edge.normal;
                /// 4c. Using those normals find the point on the circle that will hit that wall first.
                t_circlePoints[i] = circle.center + -t_edgeN * circle.radius;
                /// 4d. Create a line using thsese points on the circle to do raycasts.
                Line t_ray = new Line(t_circlePoints[i], t_circlePoints[i] + dir * maxDistance);
                /// 4e. Find the intersection between the ray and the corresponding edge of the rectangle.
                t_didIntersect[i] = FindLineIntersection(t_edge, t_ray, out t_nearPoints[i]);
                if (t_didIntersect[i])
                {
                    t_didAnyIntersect = true;
                }
            }
            /// 4f. If none intersected, we MIGHT be done with no overlap.
            if (!t_didAnyIntersect)
            {
                // Its possible that the circle will hit the closest corner.
                /// 4f0. Get the closest corner.
                Vector2 t_closestCorner = new Vector2(float.NaN, float.NaN);
                float t_closestCornerDist = float.PositiveInfinity;
                foreach (int t_cornerIndex in t_cornerIndicesToCheck)
                {
                    Vector2 t_curCorner = rectangle.points[t_cornerIndex];
                    if (!IsPointInCapsule(t_circleCastedAsCapsule, t_curCorner))
                    {
                        // Corner not in the capsule.
                        continue;
                    }

                    float t_curDist = (t_curCorner - circle.center).sqrMagnitude;
                    if (t_curDist < t_closestCornerDist)
                    {
                        t_closestCorner = t_curCorner;
                        t_closestCornerDist = t_curDist;
                    }
                }
                if (float.IsPositiveInfinity(t_closestCornerDist))
                {
                    // No hit.
                    hitDistance = 0.0f;
                    hitPointOnCircle = new Vector2(float.NaN, float.NaN);
                    hitPointOnRectangle = new Vector2(float.NaN, float.NaN);
                    return false;
                }

                /// 4f1. Create a ray from the corner towards the circle in the opposite direction of the circle cast.
                Line t_fromCornerRay = new Line(t_closestCorner, t_closestCorner + -dir * maxDistance);
                /// 4f2. Find the point of intersection of this corner with the circle
                if (CircleLineOverlap(circle, t_fromCornerRay, out Vector2? t_intersect1, out Vector2? t_intersect2))
                {
                    /// 4f3. Which of the intersection points is closer (or if 1 is null, which one isn't null)
                    Vector2 t_closerIntersect;
                    if (t_intersect1.HasValue && t_intersect2.HasValue)
                    {
                        Vector2 t_diff1 = t_intersect1.Value - t_closestCorner;
                        Vector2 t_diff2 = t_intersect2.Value - t_closestCorner;
                        t_closerIntersect = t_diff1.sqrMagnitude <= t_diff2.sqrMagnitude ? t_intersect1.Value : t_intersect2.Value;
                    }
                    else if (t_intersect1.HasValue)
                    {
                        t_closerIntersect = t_intersect1.Value;
                    }
                    else // if (t_intersect2.HasValue)
                    {
                        t_closerIntersect = t_intersect2.Value;
                    }
                    /// 4f4. We now have the point hit on the circle.
                    hitPointOnCircle = t_closerIntersect;
                    hitPointOnRectangle = t_closestCorner;
                    hitDistance = Vector2.Distance(hitPointOnCircle, hitPointOnRectangle);
                    //CustomDebug.DrawCircle(hitPointOnCircle, 0.05f, 4, Color.magenta, isDebugging);
                    //CustomDebug.DrawCircle(hitPointOnRectangle, 0.05f, 4, Color.yellow, isDebugging);
                    return true;
                }
                else
                {
                    // Line ray did not hit the circle, so there really is no overlap.
                    hitDistance = 0.0f;
                    hitPointOnCircle = new Vector2(float.NaN, float.NaN);
                    hitPointOnRectangle = new Vector2(float.NaN, float.NaN);
                    return false;
                }
            }
            /// 4g. Something intersected, if multiple did, find the closer. If only 1 did, use that one.
            else
            {
                if (t_edgesToCheck.Count == 1)
                {
                    hitPointOnCircle = t_circlePoints[0];
                    hitPointOnRectangle = t_nearPoints[0];
                }
                else
                {
                    hitPointOnCircle = new Vector2(float.NaN, float.NaN);
                    hitPointOnRectangle = new Vector2(float.NaN, float.NaN);

                    float t_nearestSqDist = float.PositiveInfinity;
                    for (int i = 0; i < t_edgesToCheck.Count; ++i)
                    {
                        Vector2 t_nearToCircleDiff = t_nearPoints[i] - t_circlePoints[i];
                        float t_sqDist = t_nearToCircleDiff.sqrMagnitude;
                        if (t_sqDist < t_nearestSqDist)
                        {
                            hitPointOnCircle = t_circlePoints[i];
                            hitPointOnRectangle = t_nearPoints[i];
                            t_nearestSqDist = t_sqDist;
                        }
                    }
                }
                /// 4h. Find the distance between the closest point found in 4 and the point on the circle found in 3.
                hitDistance = Vector2.Distance(hitPointOnCircle, hitPointOnRectangle);
                //CustomDebug.DrawCircle(hitPointOnCircle, 0.05f, 4, Color.magenta, isDebugging);
                //CustomDebug.DrawCircle(hitPointOnRectangle, 0.05f, 4, Color.yellow, isDebugging);
                return true;
            }
        }

        /// <summary>Casts a circle in the direction given for the max distance. If it intersects the line at all, returns true.</summary>
        /// <param name="hitDistance">Amount moved in direction before hit occurred.</param>
        public static bool CircleCastToLine(Circle circle, Vector2 dir, float maxDistance, Line line, out float hitDistance, bool isDebugging = false) => CircleCastToLine(circle, dir, maxDistance, line, out hitDistance, out _, out _, isDebugging);
        /// <summary>Casts a circle in the direction given for the max distance. If it intersects the line at all, returns true.</summary>
        /// <param name="hitDistance">Amount moved in direction before hit occurred.</param>
        public static bool CircleCastToLine(Circle circle, Vector2 dir, float maxDistance, Line line, out float hitDistance, out Vector2 hitPointOnCircle, out Vector2 hitPointOnLine, bool isDebugging = false)
        {
            // Case 1: The line is inside the circle
            if (IsPointInCircle(circle, line.center))
            {
                hitDistance = 0.0f;
                hitPointOnCircle = circle.center;
                hitPointOnLine = line.center;
                return true;
            }
            // Case 2: The line already intersects the circle
            else if (CircleLineOverlap(circle, line, isDebugging))
            {
                hitDistance = 0.0f;
                hitPointOnCircle = circle.center;
                hitPointOnLine = line.center;
                return true;
            }
            // Case 3: The line and the circle are not overlapping, but the casted circle and the line do.
            Capsule t_castedCircleAsCapsule = new Capsule(circle.center, circle.center + dir * maxDistance, circle.radius);

            Vector2 t_lineN = line.normal;
            /// 3a. Using those normals find the points on the circle that will hit the line first.
            Vector2 t_circlePoint0 = circle.center + -t_lineN * circle.radius;
            Vector2 t_circlePoint1 = circle.center + t_lineN * circle.radius;
            /// 3b. Create a line using thsese points on the circle to do raycasts.
            Line t_ray0 = new Line(t_circlePoint0, t_circlePoint0 + dir * maxDistance);
            Line t_ray1 = new Line(t_circlePoint1, t_circlePoint1 + dir * maxDistance);
            //line.DrawDebug(Color.magenta, isDebugging, false);
            Color t_debugCol0 = Color.gray;
            Color t_debugCol1 = Color.gray;
            /// 3c. Find the intersection between the ray and the corresponding edge of the rectangle.
            /// If the ray would intersect the circle twice, ignore that ray.
            bool t_didIntersect0 = false;
            Vector2 t_nearPoint0 = new Vector2(float.NaN, float.NaN);
            if (Vector2.Dot(-t_lineN, dir) >= 0.0f)
            {
                // Only try to find line intersection if the line does not cross through the circle.
                t_didIntersect0 = FindLineIntersection(line, t_ray0, out t_nearPoint0);
                t_debugCol0 = Color.cyan;
            }
            bool t_didIntersect1 = false;
            Vector2 t_nearPoint1 = new Vector2(float.NaN, float.NaN);
            if (Vector2.Dot(t_lineN, dir) >= 0.0f)
            {
                // Only try to find line intersection if the line does not cross through the circle.
                t_didIntersect1 = FindLineIntersection(line, t_ray1, out t_nearPoint1);
                t_debugCol1 = Color.yellow;
            }

            //t_ray0.DrawDebug(t_debugCol0, isDebugging, false);
            //t_ray1.DrawDebug(t_debugCol1, isDebugging, false);

            /// 3e. If none intersected, we MIGHT be done with no overlap.
            if (!t_didIntersect0 && !t_didIntersect1)
            {
                // Its possible that the circle will hit the closest point of the line.
                /// 3e0. Get the closest point of the line
                Vector2 t_closestCorner = new Vector2(float.NaN, float.NaN);
                float t_closestCornerDist = float.PositiveInfinity;
                foreach (Vector2 t_corner in line.points)
                {
                    if (!IsPointInCapsule(t_castedCircleAsCapsule, t_corner))
                    {
                        // Corner not in the capsule.
                        continue;
                    }

                    float t_curDist = (t_corner - circle.center).sqrMagnitude;
                    if (t_curDist < t_closestCornerDist)
                    {
                        t_closestCorner = t_corner;
                        t_closestCornerDist = t_curDist;
                    }
                }
                if (float.IsPositiveInfinity(t_closestCornerDist))
                {
                    // No corners on the line are inside the casted circle, but the end of the casted circle might intersect with the line.
                    Vector2 t_nearestPoint = NearestPointOnLine(line, t_castedCircleAsCapsule.point2);
                    Vector2 t_diffToNearPoint = t_nearestPoint - t_castedCircleAsCapsule.point2;
                    if (t_diffToNearPoint.sqrMagnitude <= circle.radius * circle.radius)
                    {
                        hitPointOnCircle = circle.center + t_diffToNearPoint.normalized * circle.radius;
                        hitPointOnLine = t_nearestPoint;
                        hitDistance = Vector2.Distance(hitPointOnCircle, hitPointOnLine);
                        return true;
                    }

                    hitDistance = 0.0f;
                    hitPointOnCircle = new Vector2(float.NaN, float.NaN);
                    hitPointOnLine = new Vector2(float.NaN, float.NaN);
                    return false;
                }

                /// 3e1. Create a ray from the corner towards the circle in the opposite direction of the circle cast.
                Line t_fromCornerRay = new Line(t_closestCorner, t_closestCorner + -dir * maxDistance);
                /// 3e2. Find the point of intersection of this corner with the circle
                if (CircleLineOverlap(circle, t_fromCornerRay, out Vector2? t_intersect1, out Vector2? t_intersect2))
                {
                    /// 3e3. Which of the intersection points is closer (or if 1 is null, which one isn't null)
                    Vector2 t_closerIntersect;
                    if (t_intersect1.HasValue && t_intersect2.HasValue)
                    {
                        Vector2 t_diff1 = t_intersect1.Value - t_closestCorner;
                        Vector2 t_diff2 = t_intersect2.Value - t_closestCorner;
                        t_closerIntersect = t_diff1.sqrMagnitude <= t_diff2.sqrMagnitude ? t_intersect1.Value : t_intersect2.Value;
                    }
                    else if (t_intersect1.HasValue)
                    {
                        t_closerIntersect = t_intersect1.Value;
                    }
                    else // if (t_intersect2.HasValue)
                    {
                        t_closerIntersect = t_intersect2.Value;
                    }
                    /// 3e4. We now have the point hit on the circle.
                    hitPointOnCircle = t_closerIntersect;
                    hitPointOnLine = t_closestCorner;
                    hitDistance = Vector2.Distance(hitPointOnCircle, hitPointOnLine);
                    //CustomDebug.DrawCircle(hitPointOnCircle, 0.05f, 4, Color.magenta, isDebugging);
                    //CustomDebug.DrawCircle(hitPointOnLine, 0.05f, 4, Color.yellow, isDebugging);
                    return true;
                }
                else
                {
                    // Line ray did not hit the circle, so there really is no overlap.
                    hitDistance = 0.0f;
                    hitPointOnCircle = new Vector2(float.NaN, float.NaN);
                    hitPointOnLine = new Vector2(float.NaN, float.NaN);
                    return false;
                }
            }
            /// 3f. Something intersected, if multiple did, find the closer. If only 1 did, use that one.
            else
            {
                if (t_didIntersect0 && t_didIntersect1)
                {
                    // Both intersected
                    float t_nearestSqDist = float.PositiveInfinity;
                    hitPointOnCircle = Vector2.zero;
                    hitPointOnLine = Vector2.zero;
                    Vector2[] t_nearPoints = new Vector2[2] { t_nearPoint0, t_nearPoint1 };
                    Vector2[] t_circlePoints = new Vector2[2] { t_circlePoint0, t_circlePoint1 };
                    for (int i = 0; i < 2; ++i)
                    {
                        Vector2 t_nearToCircleDiff = t_nearPoints[i] - t_circlePoints[i];
                        float t_sqDist = t_nearToCircleDiff.sqrMagnitude;
                        if (t_sqDist < t_nearestSqDist)
                        {
                            hitPointOnCircle = t_circlePoints[i];
                            hitPointOnLine = t_nearPoints[i];
                            t_nearestSqDist = t_sqDist;
                        }
                    }
                }
                else if (t_didIntersect0)
                {
                    // Only 0 intersected.
                    hitPointOnCircle = t_circlePoint0;
                    hitPointOnLine = t_nearPoint0;
                    
                }
                else //if (t_didIntersect1)
                {
                    hitPointOnCircle = t_circlePoint1;
                    hitPointOnLine = t_nearPoint1;
                }

                /// 3g. Find the distance between the closest point found in 4 and the point on the circle found in 3.
                hitDistance = Vector2.Distance(hitPointOnCircle, hitPointOnLine);
                //CustomDebug.DrawCircle(hitPointOnCircle, 0.05f, 4, Color.magenta, isDebugging);
                //CustomDebug.DrawCircle(hitPointOnLine, 0.05f, 4, Color.yellow, isDebugging);
                return true;
            }
        }


        /// <summary>Casts a recetangle in the direction given for the max distance. If it intersects the line at all, returns true.</summary>
        /// <param name="hitDistance">Amount moved in direction before hit occurred.</param>
        public static bool RectangleCastToLine(Rectangle rectangle, Vector2 dir, float maxDistance, Line line, out float hitDistance, bool isDebugging = false) => RectangleCastToLine(rectangle, dir, maxDistance, line, out hitDistance, out _, out _, isDebugging);
        /// <summary>Casts a recetangle in the direction given for the max distance. If it intersects the line at all, returns true.</summary>
        /// <param name="hitDistance">Amount moved in direction before hit occurred.</param>
        public static bool RectangleCastToLine(Rectangle rectangle, Vector2 dir, float maxDistance, Line line, out float hitDistance, out Vector2 hitPointOnRectangle, out Vector2 hitPointOnLine, bool isDebugging = false)
        {
            // Check if the rectangle and the line are already overlapping
            if (RectangleLineOverlap(rectangle, line, isDebugging))
            {
                hitDistance = 0.0f;
                hitPointOnRectangle = rectangle.center;
                hitPointOnLine = line.center;
                return true;
            }

            // 1. Gather points on the rectangle we will be raycasting from.
            List<Vector2> t_rectPointsToRaycastFrom = GetPointsOnRectangleToRaycastFrom(rectangle, dir);
            // 2. Raycast from each of these points and see if the ray intersects the line.
            List<(Vector2 rectPoint, Vector2 linePoint)> t_hits = new List<(Vector2 rectPoint, Vector2 linePoint)>();
            foreach (Vector2 t_startPoint in t_rectPointsToRaycastFrom)
            {
                Vector2 t_endPoint = t_startPoint + dir * maxDistance;
                Line t_ray = new Line(t_startPoint, t_endPoint);
                //t_ray.DrawDebug(Color.blue, isDebugging, false);

                if (FindLineIntersection(t_ray, line, out Vector2 t_intersectionPoint))
                {
                    t_hits.Add((t_startPoint, t_intersectionPoint));
                    //t_ray.DrawDebug(Color.magenta, isDebugging, false);
                    //CustomDebug.DrawCircle(t_intersectionPoint, 0.05f, 3, Color.magenta, isDebugging);
                }
            }
            // 3. Grab the lines on the rectangle that could be the ones getting hit (this is now done in the RaycastMultipleToRectangle).
            // 4. Raycast from each of the line points in reverse to see if those rays interesect the rectangle.
            Vector2 t_reverseDir = -dir;
            if (RaycastMultipleToRectangle(line.points, t_reverseDir, maxDistance, rectangle, out Vector2 t_startPosThatHit, out Vector2 t_hitPointOnRect, out float t_hitDist, isDebugging))
            {
                t_hits.Add((t_hitPointOnRect, t_startPosThatHit));
            }
            // 5. If there was no hits
            if (t_hits.Count == 0)
            {
                hitDistance = 0.0f;
                hitPointOnRectangle = new Vector2(float.NaN, float.NaN);
                hitPointOnLine = new Vector2(float.NaN, float.NaN);
                return false;
            }
            // 6. Find the hit that travelled the least distance (the one that hit first).
            float t_closestHitSqDist = float.PositiveInfinity;
            (Vector2 rectPoint, Vector2 linePoint) t_closestHit = (Vector2.zero, Vector2.zero);
            foreach ((Vector2 t_rectPoint, Vector2 t_linePoint) in t_hits)
            {
                Vector2 t_diff = t_rectPoint - t_linePoint;
                float t_sqMag = t_diff.sqrMagnitude;
                if (t_sqMag < t_closestHitSqDist)
                {
                    t_closestHitSqDist = t_sqMag;
                    t_closestHit = (t_rectPoint, t_linePoint);
                }
            }

            hitPointOnRectangle = t_closestHit.rectPoint;
            hitPointOnLine = t_closestHit.linePoint;
            hitDistance = Mathf.Sqrt(t_closestHitSqDist);
            return true;
        }
        /// <summary>Casts a rectangle in the direction given for the max distance. If it intersects the other rectangle at all, returns true.</summary>
        /// <param name="hitDistance">Amount moved in direction before hit occurred.</param>
        public static bool RectangleCastToRectangle(Rectangle rectangle, Vector2 dir, float maxDistance, Rectangle otherRectangle, out float hitDistance, bool isDebugging = false) => RectangleCastToRectangle(rectangle, dir, maxDistance, otherRectangle, out hitDistance, out _, out _, isDebugging);
        /// <summary>Casts a rectangle in the direction given for the max distance. If it intersects the other rectangle at all, returns true.</summary>
        /// <param name="hitDistance">Amount moved in direction before hit occurred.</param>
        public static bool RectangleCastToRectangle(Rectangle rectangle, Vector2 dir, float maxDistance, Rectangle otherRectangle, out float hitDistance, out Vector2 hitPointOnRectangle, out Vector2 hitPointOnOtherRectangle, bool isDebugging = false)
        {
            // Check if they are already overlapping
            if (RectangleRectangleOverlapAllowEdgeTouch(rectangle, otherRectangle))
            {
                //// Find the intersection between the move line and the rectangle (thats the hit point).
                //float t_checkLineLength = rectangle.width * rectangle.width + rectangle.height * rectangle.height;
                //if (t_checkLineLength < 1.0f)
                //{
                //    t_checkLineLength = 1.0f;
                //}
                //Line t_moveAlongLine = new Line(rectangle.center, rectangle.center + dir * t_checkLineLength);
                //if (RectangleLineIntersect(rectangle, t_moveAlongLine, out List<Vector2> t_intersectPoints, isDebugging))
                //{
                //    // The line we made should ONLY allow for 1 intersection.
                //    if (t_intersectPoints.Count == 0 || t_intersectPoints.Count > 1)
                //    {
                //        // This shouldn't happen, we crafted the line to specifically go through this rectangle.
                //        //CustomDebug.LogError($"Expected to find ONLY 1 intersect between rectangle {rectangle} and line {t_moveAlongLine} but instead found {t_intersectPoints.Count}");
                //        hitDistance = 0.0f;
                //        hitPointOnRectangle = rectangle.center;
                //        hitPointOnOtherRectangle = otherRectangle.center;
                //        return true;
                //    }
                //    else
                //    {
                //        hitPointOnRectangle = t_intersectPoints[0];
                //        // Now find the hit point on the other rectangle.
                //    }
                //}
                

                hitDistance = 0.0f;
                hitPointOnRectangle = rectangle.center;
                hitPointOnOtherRectangle = otherRectangle.center;
                return true;
            }

            // 1. Gather points on the rectangle we will be raycasting from.
            List<Vector2> t_rectPointsToRaycastFrom = GetPointsOnRectangleToRaycastFrom(rectangle, dir);
            // 2. Grab the lines on the other rectangle that could be the ones getting hit (now done inside RaycastMultipleToRectangle).
            // 3. Raycast from each of these points and see if the ray intersects any of those edges.
            List<(Vector2 rectPoint, Vector2 otherRectPoint)> t_hits = new List<(Vector2 rectPoint, Vector2 otherRectPoint)>();
            if (RaycastMultipleToRectangle(t_rectPointsToRaycastFrom, dir, maxDistance, otherRectangle, out Vector2 t_startPosThatHit, out Vector2 t_hitPointOnRectangle, out float t_hitDistance, isDebugging))
            {
                t_hits.Add((t_startPosThatHit, t_hitPointOnRectangle));
            }

            // 4. Do the same 3 steps as above, but for the other rectangle.
            Vector2 t_reverseDir = -dir;
            List<Vector2> t_otherRectPointsToRaycastFrom = GetPointsOnRectangleToRaycastFrom(otherRectangle, t_reverseDir);
            if (RaycastMultipleToRectangle(t_otherRectPointsToRaycastFrom, t_reverseDir, maxDistance, rectangle, out t_startPosThatHit, out t_hitPointOnRectangle, out t_hitDistance, isDebugging))
            {
                t_hits.Add((t_hitPointOnRectangle, t_startPosThatHit));
            }

            // 5. If there was no hits
            if (t_hits.Count == 0)
            {
                hitDistance = 0.0f;
                hitPointOnRectangle = new Vector2(float.NaN, float.NaN);
                hitPointOnOtherRectangle = new Vector2(float.NaN, float.NaN);
                return false;
            }
            // 6. Find the hit that travelled the least distance (the one that hit first).
            float t_closestHitSqDist = float.PositiveInfinity;
            (Vector2 rectPoint, Vector2 otherRectPoint) t_closestHit = (Vector2.zero, Vector2.zero);
            foreach ((Vector2 t_rectPoint, Vector2 t_otherRectPoint) in t_hits)
            {
                Vector2 t_diff = t_rectPoint - t_otherRectPoint;
                float t_sqMag = t_diff.sqrMagnitude;
                if (t_sqMag < t_closestHitSqDist)
                {
                    t_closestHitSqDist = t_sqMag;
                    t_closestHit = (t_rectPoint, t_otherRectPoint);
                }
            }

            hitPointOnRectangle = t_closestHit.rectPoint;
            hitPointOnOtherRectangle = t_closestHit.otherRectPoint;
            hitDistance = Mathf.Sqrt(t_closestHitSqDist);
            return true;
        }
        private static List<Vector2> GetPointsOnRectangleToRaycastFrom(Rectangle rectangle, Vector2 dir)
        {
            List<Vector2> t_pointsToRaycastFrom = new List<Vector2>(3);
            bool t_isLeft = dir.x < 0.0f;
            bool t_isRight = dir.x > 0.0f;
            bool t_isDown = dir.y < 0.0f;
            bool t_isUp = dir.y > 0.0f;
            if (t_isLeft || t_isDown)
            {
                t_pointsToRaycastFrom.Add(rectangle.botLeftPoint);
            }
            if (t_isLeft || t_isUp)
            {
                t_pointsToRaycastFrom.Add(rectangle.topLeftPoint);
            }
            if (t_isRight || t_isDown)
            {
                t_pointsToRaycastFrom.Add(rectangle.botRightPoint);
            }
            if (t_isRight || t_isUp)
            {
                t_pointsToRaycastFrom.Add(rectangle.topRightPoint);
            }
            return t_pointsToRaycastFrom;
        }
        private static List<Line> GetEdgesOfRectangleThatRaycastCouldHit(Rectangle rectangle, Vector2 dir)
        {
            List<Line> t_edgesThatCouldBeHit = new List<Line>(2);
            bool t_isUp = dir.y > 0.0f;
            if (dir.x < 0.0f)
            {
                t_edgesThatCouldBeHit.Add(rectangle.rightEdge);
            }
            else if (dir.x > 0.0f)
            {
                t_edgesThatCouldBeHit.Add(rectangle.leftEdge);
            }

            if (dir.y < 0.0f)
            {
                t_edgesThatCouldBeHit.Add(rectangle.topEdge);
            }
            if (dir.y > 0.0f)
            {
                t_edgesThatCouldBeHit.Add(rectangle.botEdge);
            }

            return t_edgesThatCouldBeHit;
        }

        /// <summary>
        /// Raycasts the given point to see if it hits the specified rectangle.
        /// </summary>
        /// <param name="startPositions">Positions for rays to begins.</param>
        /// <param name="dir">Direction the ray is fired in.</param>
        /// <param name="maxDistance">How far the ray is fired.</param>
        /// <param name="rectangle">The rectangle the ray is trying to hit.</param>
        /// <param name="startPosThatHit">Start position of the ray that hit the rectangle.</param>
        /// <param name="hitPointOnRectangle">Point on the rectangle the ray struck.</param>
        /// <param name="hitDistance">Distance the hitPoint is from the startPos.</param>
        /// <returns>True if raycast hit. False otherwise.</returns>
        public static bool RaycastMultipleToRectangle(ICollection<Vector2> startPositions, Vector2 dir, float maxDistance, Rectangle rectangle, out Vector2 startPosThatHit, out Vector2 hitPointOnRectangle, out float hitDistance, bool isDebugging = false)
        {
            List<Line> t_rectLinesThatCouldBeHit = GetEdgesOfRectangleThatRaycastCouldHit(rectangle, dir);
            if (isDebugging)
            {
                foreach (Line t_line in t_rectLinesThatCouldBeHit)
                {
                    //t_line.DrawDebug(Color.cyan, true, false);
                }
            }

            List<(Vector2 startPoint, Vector2 hitPoint)> t_hits = new List<(Vector2, Vector2)>(startPositions.Count * 3);
            foreach (Vector2 t_startPos in startPositions)
            {
                Vector2 t_endPoint = t_startPos + dir * maxDistance;
                Line t_ray = new Line(t_startPos, t_endPoint);
                //t_ray.DrawDebug(Color.yellow, isDebugging, false);

                foreach (Line t_rectEdge in t_rectLinesThatCouldBeHit)
                {
                    if (FindLineIntersection(t_ray, t_rectEdge, out Vector2 t_intersectionPoint, isDebugging))
                    {
                        t_hits.Add((t_startPos, t_intersectionPoint));
                        //t_ray.DrawDebug(Color.magenta, isDebugging, false);
                        //CustomDebug.DrawCircle(t_intersectionPoint, 0.05f, 3, Color.magenta, isDebugging);
                    }
                }
            }

            if (t_hits.Count == 0)
            {
                CustomDebug.RunDebugFunction(() =>
                {
                    bool t_isFirst = true;
                    string t_startPointsStr = "[";
                    foreach (Vector2 t_startPos in startPositions)
                    {
                        if (t_isFirst)
                        {
                            t_startPointsStr += $"{t_startPos}";
                            t_isFirst = false;
                        }
                        else
                        {
                            t_startPointsStr += $", {t_startPos}";
                        }
                    }
                    t_startPointsStr += "]";
                    //CustomDebug.Log($"Zero hits between rectangle {rectangle} and start points: {t_startPointsStr}", true);
                }, isDebugging);
                
                hitDistance = 0.0f;
                startPosThatHit = new Vector2(float.NaN, float.NaN);
                hitPointOnRectangle = new Vector2(float.NaN, float.NaN);
                return false;
            }

            // Find the closest hit point
            hitDistance = float.PositiveInfinity;
            (Vector2 startPoint, Vector2 hitPoint) t_hit = (new Vector2(float.NaN, float.NaN), new Vector2(float.NaN, float.NaN));
            foreach ((Vector2 t_startPoint, Vector2 t_hitPoint) in t_hits)
            {
                Vector2 t_diff = t_hitPoint - t_startPoint;
                float t_sqDist = t_diff.sqrMagnitude;
                if (t_sqDist < hitDistance)
                {
                    hitDistance = t_sqDist;
                    t_hit = (t_startPoint, t_hitPoint);
                }
            }

            startPosThatHit = t_hit.startPoint;
            hitPointOnRectangle = t_hit.hitPoint;
            return true;
        }
        /// <summary>
        /// Raycasts the given point to see if it hits the specified rectangle.
        /// </summary>
        /// <param name="startPos">Position the ray begins.</param>
        /// <param name="dir">Direction the ray is fired in.</param>
        /// <param name="maxDistance">How far the ray is fired.</param>
        /// <param name="rectangle">The rectangle the ray is trying to hit.</param>
        /// <param name="hitPoint">Point on the rectangle the ray struck.</param>
        /// <param name="hitDistance">Distance the hitPoint is from the startPos.</param>
        /// <returns>True if raycast hit. False otherwise.</returns>
        public static bool RaycastToRectangle(Vector2 startPos, Vector2 dir, float maxDistance, Rectangle rectangle, out Vector2 hitPoint, out float hitDistance, bool isDebugging = false)
        {
            List<Line> t_rectLinesThatCouldBeHit = GetEdgesOfRectangleThatRaycastCouldHit(rectangle, dir);

            Vector2 t_endPoint = startPos + dir * maxDistance;
            Line t_ray = new Line(startPos, t_endPoint);
            //t_ray.DrawDebug(Color.blue, isDebugging, false);

            List<Vector2> t_hitPoints = new List<Vector2>(3);
            foreach (Line t_rectEdge in t_rectLinesThatCouldBeHit)
            {
                if (FindLineIntersection(t_ray, t_rectEdge, out Vector2 t_intersectionPoint))
                {
                    t_hitPoints.Add(t_intersectionPoint);
                    //t_ray.DrawDebug(Color.magenta, isDebugging, false);
                    //CustomDebug.DrawCircle(t_intersectionPoint, 0.05f, 3, Color.magenta, isDebugging);
                }
            }

            if (t_hitPoints.Count == 0)
            {
                hitDistance = 0.0f;
                hitPoint = new Vector2(float.NaN, float.NaN);
                return false;
            }

            // Find the closest hit point
            hitDistance = float.PositiveInfinity;
            hitPoint = new Vector2(float.NaN, float.NaN);
            foreach (Vector2 t_hitPointCandidate in t_hitPoints)
            {
                Vector2 t_diff = t_hitPointCandidate - startPos;
                float t_sqDist = t_diff.sqrMagnitude;
                if (t_sqDist < hitDistance)
                {
                    hitDistance = t_sqDist;
                    hitPoint = t_hitPointCandidate;
                }
            }

            return true;
        }


        /// <summary>
        /// Finds the point where the 2 lines intersect. If the 2 lines are parallel, returns false. Otherwise returns true.
        /// </summary>
        public static bool InfiniteLinesIntersect(Line line1, Line line2, out Vector2 intersectionPoint, bool isDebugging = false)
        {
            // Check if lines are vertical
            if (line1.point1.x == line1.point2.x)
            {
                // Line1 is vertical.
                if (line2.point1.x == line2.point2.x)
                {
                    // Both are vertical (so they never intersect).
                    intersectionPoint = new Vector2(float.NaN, float.NaN);
                    return false;
                }
                else
                {
                    // Only line1 is vertical, which means not parallel, so they have an intersection.
                    return InfiniteLinesIntersectSingleVertical(line1, line2, out intersectionPoint, isDebugging);
                }
            }
            else if (line2.point1.x == line2.point2.x)
            {
                // Only line2 is vertical, which means not parallel, so they have an intersection.
                return InfiniteLinesIntersectSingleVertical(line2, line1, out intersectionPoint, isDebugging);
            }
            else
            {
                // Neither line is vertical, but they could still be parallel.
                Vector2 t_line1Diff = line1.point1 - line1.point2;
                Vector2 t_line2Diff = line2.point1 - line2.point2;
                float t_m1 = t_line1Diff.y / t_line1Diff.x;
                float t_m2 = t_line2Diff.y / t_line2Diff.x;
                if (t_m1 == t_m2)
                {
                    // Parallel
                    intersectionPoint = new Vector2(float.NaN, float.NaN);
                    return false;
                }
                else
                {
                    // Not parallel, they have an intersection.
                    float t_intersectX = (t_m1 * line1.point1.x - t_m2 * line2.point1.x - line1.point1.y + line2.point1.y) / (t_m1 - t_m2);
                    intersectionPoint = new Vector2(t_intersectX, t_m1 * (t_intersectX - line1.point1.x) + line1.point1.y);
                    //DrawIntersectPoint(intersectionPoint, isDebugging);
                    return true;
                }
            }
        }
        private static bool InfiniteLinesIntersectSingleVertical(Line verticalLine, Line nonVertLine, out Vector2 intersectionPoint, bool isDebugging)
        {
            // They must intersect at this x, since the vertical line only exists at this x.
            float x = verticalLine.point1.x;
            // Figure out the y of the non vertical line at that x.
            Vector2 t_diff = nonVertLine.point1 - nonVertLine.point2;
            float m = t_diff.y / t_diff.x;
            intersectionPoint = new Vector2(x, m * (x - nonVertLine.point1.x) + nonVertLine.point1.y);
            //DrawIntersectPoint(intersectionPoint, isDebugging);
            return true;
        }

        /// <summary>
        /// If the given point falls on the line segment between the end points of the given line.
        /// </summary>
        public static bool IsInfiniteLinePointOnLineSegment(Line line, Vector2 point, bool isDebugging = false)
        {
            float t_minX = line.point1.x;
            float t_maxX = line.point2.x;
            if (line.point2.x <= line.point1.x)
            {
                t_minX = line.point2.x;
                t_maxX = line.point1.x;
            }

            float t_minY = line.point1.y;
            float t_maxY = line.point2.y;
            if (line.point2.y <= line.point1.y)
            {
                t_minY = line.point2.y;
                t_maxY = line.point1.y;
            }

            if (t_minY == t_maxY)
            {
                t_minY -= 0.0001f;
                t_maxY += 0.0001f;
            }
            if (t_minX == t_maxX)
            {
                t_minX -= 0.0001f;
                t_maxX += 0.0001f;
            }

            Rectangle t_bounds = new Rectangle(t_minX, t_maxX, t_minY, t_maxY);
            bool t_isPointInRectangle = IsPointInRectangle(t_bounds, point);
            Color t_debugColor = (t_isPointInRectangle ? Color.red : Color.green) * 0.5f;
            //t_bounds.DrawOutlineDebug(t_debugColor, isDebugging);
            //CustomDebug.Log($"Bounding Rectangle: ({t_bounds}) for checking if point ({point}) is on line ({line})", isDebugging);
            return t_isPointInRectangle;

            //float t_dot1 = Vector2.Dot((line.point1 - point).normalized, (line.point2 - point).normalized);
            //return Mathf.Abs(t_dot1) <= float.Epsilon;
        }

        /// <summary>
        /// If the given point is contained within the given circle.
        /// </summary>
        public static bool IsPointInCircle(Circle circle, Vector2 point)
        {
            Vector2 t_diff = circle.center - point;
            return t_diff.sqrMagnitude <= circle.radius * circle.radius;
        }
        /// <summary>
        /// If the given point is contained within the given circle.
        /// </summary>
        /// <param name="closestCircleEdge">The closest point on the circle's edge to the point.</param>
        public static bool IsPointInCircle(Circle circle, Vector2 point, out Vector2 closestCircleEdge)
        {
            Vector2 t_diff = circle.center - point;
            float t_dist = t_diff.magnitude;
            closestCircleEdge = circle.center - t_diff / t_dist * circle.radius;
            return t_dist <= circle.radius;
        }

        /// <summary>
        /// If the given point is contained within the given rectangle.
        /// </summary>
        public static bool IsPointInRectangle(Rectangle rectangle, Vector2 point)
        {
            return point.x <= rectangle.max.x && point.x >= rectangle.min.x && point.y <= rectangle.max.y && point.y >= rectangle.min.y;
        }

        /// <summary>
        /// If the given point is contained within the given capsule.
        /// </summary>
        public static bool IsPointInCapsule(Capsule capsule, Vector2 point)
        {
            float t_dist = DistanceFromLine(capsule.innerLine, point);
            return t_dist <= capsule.radius;
        }

        /// <summary>
        /// Finds the point on the given infinite line that is closest to the given point.
        /// </summary>
        /// <param name="point">Point that may or may not be on the line.</param>
        public static Vector2 NearestPointOnInfiniteLine(Line infiniteLine, Vector2 point)
        {
            Vector2 t_lineDir = (infiniteLine.point1 - infiniteLine.point2).normalized;
            Vector2 t_diffToPoint = point - infiniteLine.point1;
            float t_dot = Vector2.Dot(t_diffToPoint, t_lineDir);
            return infiniteLine.point1 + t_lineDir * t_dot;
        }
        /// <summary>
        /// Finds the point on the given line that is closest to the given point.
        /// </summary>
        /// <param name="point">Point that may or may not be on the line.</param>
        public static Vector2 NearestPointOnLine(Line line, Vector2 point)
        {
            Vector2 t_infinteLinePoint = NearestPointOnInfiniteLine(line, point);

            if (!IsInfiniteLinePointOnLineSegment(line, t_infinteLinePoint))
            {
                // Not on the line, choose the closer end point to the nearest point
                Vector2 t_diff1ToNearPoint = line.point1 - t_infinteLinePoint;
                Vector2 t_diff2ToNearPoint = line.point2 - t_infinteLinePoint;
                if (t_diff1ToNearPoint.sqrMagnitude < t_diff2ToNearPoint.sqrMagnitude)
                {
                    return line.point1;
                }
                else
                {
                    return line.point2;
                }
            }
            else
            {
                // Point is already on the line, return it as it is.
                return t_infinteLinePoint;
            }
        }
        /// <summary>
        /// Gets the distance from the line the given point is.
        /// </summary>
        public static float DistanceFromLine(Line line, Vector2 point)
        {
            Vector2 t_nearestPointOnInfLine = NearestPointOnInfiniteLine(line, point);
            if (IsInfiniteLinePointOnLineSegment(line, t_nearestPointOnInfLine))
            {
                // If the nearest point is already on the line, great, just return the distance.
                return Vector2.Distance(t_nearestPointOnInfLine, point);
            }
            else
            {
                // Nearest point is not already on the line.
                // This means one of the endpoints of the line is closest to the point. Find both and then return the closer one.
                Vector2 t_diff1 = line.point1 - point;
                Vector2 t_diff2 = line.point2 - point;
                if (t_diff1.sqrMagnitude < t_diff2.sqrMagnitude)
                {
                    return t_diff1.magnitude;
                }
                else
                {
                    return t_diff2.magnitude;
                }
            }
        }
        public static float FindMinimumDistanceBetweenLines(Line line1, Line line2, out Vector2 nearestPosOnLine1, out Vector2 nearestPosOnLine2)
        {
            if (InfiniteLinesIntersect(line1, line2, out Vector2 t_intersectionPoint))
            {
                bool t_isOnLine1 = IsInfiniteLinePointOnLineSegment(line1, t_intersectionPoint);
                bool t_isOnLine2 = IsInfiniteLinePointOnLineSegment(line2, t_intersectionPoint);
                if (t_isOnLine1 && t_isOnLine2)
                {
                    nearestPosOnLine1 = t_intersectionPoint;
                    nearestPosOnLine2 = t_intersectionPoint;
                    return 0.0f;
                }
            }

            Vector2 t_nearPoint1 = NearestPointOnLine(line1, line2.point1);
            Vector2 t_nearPoint2 = NearestPointOnLine(line1, line2.point2);
            Vector2 t_nearPoint3 = NearestPointOnLine(line2, line1.point1);
            Vector2 t_nearPoint4 = NearestPointOnLine(line2, line1.point2);

            float t_dist1 = DistanceFromLine(line2, t_nearPoint1);
            float t_dist2 = DistanceFromLine(line2, t_nearPoint2);
            float t_dist3 = DistanceFromLine(line1, t_nearPoint3);
            float t_dist4 = DistanceFromLine(line1, t_nearPoint4);

            nearestPosOnLine1 = t_dist1 <= t_dist2 ? t_nearPoint1 : t_nearPoint2;
            nearestPosOnLine2 = t_dist3 <= t_dist4 ? t_nearPoint3 : t_nearPoint4;
            return Mathf.Min(t_dist1, t_dist2, t_dist3, t_dist4);
        }
        public static bool FindLineIntersection(Line line1, Line line2, out Vector2 intersectionPoint, bool isDebugging = false)
        {
            if (InfiniteLinesIntersect(line1, line2, out intersectionPoint, isDebugging))
            {
                bool t_isOnLine1 = IsInfiniteLinePointOnLineSegment(line1, intersectionPoint);
                bool t_isOnLine2 = IsInfiniteLinePointOnLineSegment(line2, intersectionPoint);
                if (t_isOnLine1)
                {
                    //CustomDebug.DrawCircle(intersectionPoint, 0.075f, 3, Color.red, isDebugging);
                }
                if (t_isOnLine2)
                {
                    //CustomDebug.DrawCircle(intersectionPoint, 0.0375f, 4, Color.red, isDebugging);
                }
                if (t_isOnLine1 && t_isOnLine2)
                {
                    //DrawIntersectPoint(intersectionPoint, Color.magenta, isDebugging);
                    return true;
                }
            }

            intersectionPoint = new Vector2(float.NaN, float.NaN);
            return false;
        }

        public static bool IsEitherCircleCompletelyInsideOtherCircle(Circle a, Circle b)
        {
            float t_sqDist = (a.center - b.center).sqrMagnitude;
            float t_sum = Mathf.Abs(a.radius - b.radius);
            return t_sqDist <= t_sum * t_sum;
        }


        public static Circle CreateMinimumBoundingCircle(Circle circleToBound1, Circle circleToBound2)
        {
            if (IsEitherCircleCompletelyInsideOtherCircle(circleToBound1, circleToBound2))
            {
                return circleToBound1.radius > circleToBound2.radius ? circleToBound1 : circleToBound2;
            }

            Vector2 t_centersDiff = circleToBound1.center - circleToBound2.center;
            float t_centerDist = t_centersDiff.magnitude;
            Vector2 t_dir = t_centersDiff / t_centerDist;

            Vector2 t_farPos1 = circleToBound1.center + t_dir * circleToBound1.radius;
            Vector2 t_farPos2 = circleToBound2.center - t_dir * circleToBound2.radius;

            //CustomDebug.DrawCrossHair(t_farPos1, 0.1f, Color.blue, true);
            //CustomDebug.DrawCrossHair(t_farPos2, 0.1f, Color.magenta, true);

            float t_newCircleRadius = 0.5f * (t_centerDist + circleToBound1.radius + circleToBound2.radius);
            Vector2 t_newCircleCenter = (t_farPos1 + t_farPos2) * 0.5f;
            return new Circle(t_newCircleCenter, t_newCircleRadius);
        }
        public static Circle CreateMinimumBoundingCircleForCircleCast(Circle circleBeingCast, Vector2 dir, float distance)
        {
            float t_halfDist = distance * 0.5f;
            return new Circle(circleBeingCast.center + (dir * t_halfDist), t_halfDist + circleBeingCast.radius);
        }

        public static Vector2 PerpendicularClockwise(this Vector2 v) => new Vector2(v.y, -v.x);
        public static Vector2 PerpendicularCounterClockwise(this Vector2 v) => new Vector2(-v.y, v.x);


        private static void DrawIntersectPoint(Vector2 point, bool isDebugging) => DrawIntersectPoint(point, Color.blue, isDebugging);
        private static void DrawIntersectPoint(Vector2 point, Color color, bool isDebugging)
        {
            //CustomDebug.DrawCircle(point, 0.1f, 8, color, isDebugging);
        }
    }
}