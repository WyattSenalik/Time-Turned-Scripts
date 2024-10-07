using System.Collections.Generic;
using UnityEngine;

using Helpers.Extensions;

namespace Helpers.Physics.Custom2DInt
{
    public sealed class CustomPhysics2DInt : MonoBehaviour
    {
        public const int INTS_PER_UNIT = 64;
        public const float INVERTED_INTS_PER_UNIT_AS_FLOAT = 1.0f / INTS_PER_UNIT;

        public static Vector2 ConvertIntPositionToFloatPosition(Vector2Int position)
        {
            float t_roundedX = position.x * INVERTED_INTS_PER_UNIT_AS_FLOAT;
            float t_roundedY = position.y * INVERTED_INTS_PER_UNIT_AS_FLOAT;
            return new Vector2(t_roundedX, t_roundedY);
        }
        public static Vector2Int ConvertFloatPositionToIntPosition(Vector2 position)
        {
            int t_xPos = Mathf.RoundToInt(position.x * INTS_PER_UNIT);
            int t_yPos = Mathf.RoundToInt(position.y * INTS_PER_UNIT);
            return new Vector2Int(t_xPos, t_yPos);
        }

        public static bool CircleCircleOverlap(CircleInt a, CircleInt b)
        {
            Vector2Int t_aCenter = a.center;
            Vector2Int t_bCenter = b.center;
            int t_xDiff = t_aCenter.x - t_bCenter.x;
            int t_yDiff = t_aCenter.y - t_bCenter.y;
            int t_sum = a.radius + b.radius;
            return t_xDiff * t_xDiff + t_yDiff * t_yDiff <= t_sum * t_sum;
        }

        public static bool CircleLineOverlap(CircleInt circle, LineInt line)
        {
            CircleInt t_lineBoundingCircle = line.boundingCircle;
            if (!CircleCircleOverlap(circle, t_lineBoundingCircle))
            {
                return false;
            }

            if (IsPointInCircle(circle, line.point1) || IsPointInCircle(circle, line.point2))
            {
                return true;
            } 
            else if (CircleLineIntersect(circle, line))
            {
                return true;
            }

            return false;
        }
        public static int CircleLineOverlap(CircleInt circle, LineInt line, out List<Vector2> intersectPoints)
        {
            CircleInt t_lineBoundingCircle = line.boundingCircle;
            if (!CircleCircleOverlap(circle, t_lineBoundingCircle))
            {
                intersectPoints = new List<Vector2>();
                return 0;
            }

            bool t_foundAnIntersect = CircleLineIntersect(circle, line, out intersectPoints) > 0;
            if (IsPointInCircle(circle, line.point1))
            {
                intersectPoints.Add(ConvertIntPositionToFloatPosition(line.point1));
                if (t_foundAnIntersect)
                {
                    return 2;
                }
            }
            if (IsPointInCircle(circle, line.point2))
            {
                intersectPoints.Add(ConvertIntPositionToFloatPosition(line.point2));
                if (t_foundAnIntersect)
                {
                    return 2;
                }
            }
            
            return intersectPoints.Count;
        }

        public static int CircleLineIntersect(CircleInt circle, LineInt line, out List<Vector2> intersectPoints)
        {
            // See https://math.stackexchange.com/questions/275529/check-if-line-intersects-with-circles-perimeter
            intersectPoints = new List<Vector2>();
            // Reframe the line's points as if the circle is the origin.
            Vector2Int t_point1 = line.point1 - circle.center;
            Vector2Int t_point2 = line.point2 - circle.center;

            int t_discriminant = CalculateDiscriminantForCircleLineIntersect(t_point1, t_point2, circle.radius, out int t_a, out int t_b);
            if (t_discriminant <= 0)
            {
                return 0;
            }
            CalculateTValuesForCircleLineIntersect(t_discriminant, t_a, t_b, out float t_t1, out float t_t2);

            Vector2 t_point1AsFloatPos = line.GetPoint1AsFloatPosition();
            if (0 <= t_t1 && t_t1 <= 1)
            {
                Vector2 t_intersectPoint = t_point1AsFloatPos + t_t1 * ConvertIntPositionToFloatPosition(line.point2 - line.point1);
                intersectPoints.Add(t_intersectPoint);
                
            }
            if (0 <= t_t2 && t_t2 <= 1)
            {
                Vector2 t_intersectPoint = t_point1AsFloatPos + t_t2 * ConvertIntPositionToFloatPosition(line.point2 - line.point1);
                intersectPoints.Add(t_intersectPoint);
            }
            return intersectPoints.Count;
        }
        public static bool CircleLineIntersect(CircleInt circle, LineInt line)
        {
            // See https://math.stackexchange.com/questions/275529/check-if-line-intersects-with-circles-perimeter
            // Reframe the line's points as if the circle is the origin.
            Vector2Int t_point1 = line.point1 - circle.center;
            Vector2Int t_point2 = line.point2 - circle.center;

            int t_discriminant = CalculateDiscriminantForCircleLineIntersect(t_point1, t_point2, circle.radius, out int t_a, out int t_b);
            if (t_discriminant <= 0)
            {
                return false;
            }
            CalculateTValuesForCircleLineIntersect(t_discriminant, t_a, t_b, out float t_t1, out float t_t2);
            return (0 <= t_t1 && t_t1 <= 1) || (0 <= t_t2 && t_t2 <= 1);
        }
        private static int CalculateDiscriminantForCircleLineIntersect(Vector2Int point1AboutCircle, Vector2Int point2AboutCircle, int radius, out int a, out int b)
        {
            int t_xDiff = point2AboutCircle.x - point1AboutCircle.x;
            int t_yDiff = point2AboutCircle.y - point1AboutCircle.y;
            a = t_xDiff * t_xDiff + t_yDiff * t_yDiff;
            b = 2 * (point1AboutCircle.x * t_xDiff + point1AboutCircle.y * t_yDiff);
            int t_c = point1AboutCircle.x * point1AboutCircle.x + point1AboutCircle.y * point1AboutCircle.y - radius * radius;
            return b * b - 4 * a * t_c;
        }
        private static void CalculateTValuesForCircleLineIntersect(int discriminant, int a, int b, out float t1, out float t2)
        {
            float t_sqrtDiscriminant = Mathf.Sqrt(discriminant);
            float t_inverted2A = 1.0f / (2 * a);
            float t_negB = -b;
            t1 = (t_negB + t_sqrtDiscriminant) * t_inverted2A;
            t2 = (t_negB - t_sqrtDiscriminant) * t_inverted2A;
        }

        /// <summary>
        /// Determines if the two lines intersect and returns where that intersection is.
        /// </summary>
        public static bool LineLineIntersect(LineInt a, LineInt b, out Vector2 intersectionPoint)
        {
            // Find the four orientations needed for general and special cases 
            eTriplePointOrientation t_orientation1 = Orientation(a.point1, a.point2, b.point1);
            eTriplePointOrientation t_orientation2 = Orientation(a.point1, a.point2, b.point2);
            eTriplePointOrientation t_orientation3 = Orientation(b.point1, b.point2, a.point1);
            eTriplePointOrientation t_orientation4 = Orientation(b.point1, b.point2, a.point2);

            // General case 
            if (t_orientation1 != t_orientation2 && t_orientation3 != t_orientation4)
            {
                if (!InfiniteLinesIntersect(a, b, out intersectionPoint))
                {
                    //CustomDebug.LogError($"Infinite lines didn't intersect but finite ones did???");
                }
                return true;
            }
            // Special Cases 
            // a.point1, a.point2, and b.point1 are collinear and b.point1 lies on segment A
            if (t_orientation1 == eTriplePointOrientation.Linear && IsPointWithinLineEncompassingRectangle(a, b.point1))
            {
                intersectionPoint = ConvertIntPositionToFloatPosition(b.point1);
                return true;
            }
            // a.point1, a.point2, and b.point2 are collinear and b.point1 lies on segment B
            if (t_orientation2 == eTriplePointOrientation.Linear && IsPointWithinLineEncompassingRectangle(a, b.point2))
            {
                intersectionPoint = ConvertIntPositionToFloatPosition(b.point2);
                return true;
            }
            // b.point1, b.point2, and a.point1 are collinear and a.point1 lies on segment B
            if (t_orientation3 == eTriplePointOrientation.Linear && IsPointWithinLineEncompassingRectangle(b, a.point1))
            {
                intersectionPoint = ConvertIntPositionToFloatPosition(a.point1);
                return true;
            }
            // b.point1, b.point2, and a.point2 are collinear and a.point2 lies on segment B
            if (t_orientation4 == eTriplePointOrientation.Linear && IsPointWithinLineEncompassingRectangle(b, a.point2))
            {
                intersectionPoint = ConvertIntPositionToFloatPosition(a.point2);
                return true;
            }

            // Doesn't fall in any of the above cases 
            intersectionPoint = new Vector2(float.NaN, float.NaN);
            return false;
        }
        /// <summary>
        /// Determines if the two lines intersect.
        /// </summary>
        public static bool LineLineIntersect(LineInt a, LineInt b)
        {
            // https://www.geeksforgeeks.org/check-if-two-given-line-segments-intersect/

            // Find the four orientations needed for general and special cases 
            eTriplePointOrientation t_orientation1 = Orientation(a.point1, a.point2, b.point1);
            eTriplePointOrientation t_orientation2 = Orientation(a.point1, a.point2, b.point2);
            eTriplePointOrientation t_orientation3 = Orientation(b.point1, b.point2, a.point1);
            eTriplePointOrientation t_orientation4 = Orientation(b.point1, b.point2, a.point2);

            // General case 
            if (t_orientation1 != t_orientation2 && t_orientation3 != t_orientation4)
            {
                return true;
            }
            // Special Cases 
            // a.point1, a.point2, and b.point1 are collinear and b.point1 lies on segment A
            if (t_orientation1 == eTriplePointOrientation.Linear && IsPointWithinLineEncompassingRectangle(a, b.point1))
            {
                return true;
            }
            // a.point1, a.point2, and b.point2 are collinear and b.point1 lies on segment B
            if (t_orientation2 == eTriplePointOrientation.Linear && IsPointWithinLineEncompassingRectangle(a, b.point2))
            {
                return true;
            }
            // b.point1, b.point2, and a.point1 are collinear and a.point1 lies on segment B
            if (t_orientation3 == eTriplePointOrientation.Linear && IsPointWithinLineEncompassingRectangle(b, a.point1))
            {
                return true;
            }
            // b.point1, b.point2, and a.point2 are collinear and a.point2 lies on segment B
            if (t_orientation4 == eTriplePointOrientation.Linear && IsPointWithinLineEncompassingRectangle(b, a.point2))
            {
                return true;
            }
            // Doesn't fall in any of the above cases 
            return false; 
        }
        /// <summary>
        /// Determines if the 3 points are collinear (all can lie on the can line), or clockwise, or counterclockwise.
        /// See https://www.geeksforgeeks.org/orientation-3-ordered-points/ for what constitutes each.
        /// </summary>
        private static eTriplePointOrientation Orientation(Vector2Int p, Vector2Int q, Vector2Int r)
        {
            int t_val = (q.y - p.y) * (r.x - q.x) - (q.x - p.x) * (r.y - q.y);

            // Collinear
            if (t_val == 0) { return eTriplePointOrientation.Linear; }

            // clock or counterclock wise 
            return (t_val > 0) ? eTriplePointOrientation.Clockwise : eTriplePointOrientation.CounterClockwise; 
        }
        private enum eTriplePointOrientation { Linear, Clockwise, CounterClockwise}

        /// <summary>
        /// Finds the point where the 2 lines intersect. If the 2 lines are parallel, returns false. Otherwise returns true.
        /// </summary>
        public static bool InfiniteLinesIntersect(LineInt line1, LineInt line2, out Vector2 intersectionPoint)
        {
            // Check if lines are vertical
            if (line1.point1.x == line1.point2.x)
            {
                // Line1 is vertical.
                if (line2.point1.x == line2.point2.x)
                {
                    // Both are vertical (but they could be the same line).
                    if (line1.point1.x == line2.point1.x)
                    {
                        // They are the same line.
                        intersectionPoint = line1.GetPoint1AsFloatPosition();
                        return true;
                    }
                    else
                    {
                        // Both vertical and not same line, so no intersection
                        intersectionPoint = new Vector2(float.NaN, float.NaN);
                        return false;
                    }
                }
                else
                {
                    // Only line1 is vertical, which means not parallel, so they have an intersection.
                    return InfiniteLinesIntersectSingleVertical(line1, line2, out intersectionPoint);
                }
            }
            else if (line2.point1.x == line2.point2.x)
            {
                // Only line2 is vertical, which means not parallel, so they have an intersection.
                return InfiniteLinesIntersectSingleVertical(line2, line1, out intersectionPoint);
            }
            else
            {
                // Neither line is vertical, but they could still be parallel.
                Vector2Int t_line1Diff = line1.point1 - line1.point2;
                Vector2Int t_line2Diff = line2.point1 - line2.point2;
                float t_m1 = ((float)t_line1Diff.y) / t_line1Diff.x;
                float t_m2 = ((float)t_line2Diff.y) / t_line2Diff.x;
                if (t_m1 == t_m2)
                {
                    // Parallel
                    // Could be same line
                    float t_b1 = -t_m1 * line1.point1.x + line1.point1.y;
                    float t_b2 = -t_m1 * line2.point1.x + line2.point1.y;
                    if (t_b1 == t_b2)
                    {
                        // Same line
                        intersectionPoint = line1.GetPoint1AsFloatPosition();
                        return true;
                    }
                    else
                    {
                        // Not same line
                        intersectionPoint = new Vector2(float.NaN, float.NaN);
                        return false;
                    }
                }
                else
                {
                    // Not parallel, they have an intersection.
                    float t_intersectX = (t_m1 * line1.point1.x - t_m2 * line2.point1.x - (line1.point1.y - line2.point1.y)) / (t_m1 - t_m2);
                    float t_intersectY = t_m1 * (t_intersectX - line1.point1.x) + line1.point1.y;
                    intersectionPoint = new Vector2(t_intersectX * INVERTED_INTS_PER_UNIT_AS_FLOAT, t_intersectY * INVERTED_INTS_PER_UNIT_AS_FLOAT);
                    return true;
                }
            }
        }
        private static bool InfiniteLinesIntersectSingleVertical(LineInt verticalLine, LineInt nonVertLine, out Vector2 intersectionPoint)
        {
            // They must intersect at this x, since the vertical line only exists at this x.
            int t_x = verticalLine.point1.x;
            // Figure out the y of the non vertical line at that x.
            Vector2Int t_diff = nonVertLine.point1 - nonVertLine.point2;
            float t_m = ((float)t_diff.y) / t_diff.x;
            float t_y = t_m * (t_x - nonVertLine.point1.x) + nonVertLine.point1.y;
            intersectionPoint = new Vector2(t_x * INVERTED_INTS_PER_UNIT_AS_FLOAT, t_y * INVERTED_INTS_PER_UNIT_AS_FLOAT);
            return true;
        }


        public static bool IsPointInCircle(CircleInt circle, Vector2Int point)
        {
            Vector2Int t_differenceFromCircle = point - circle.center;
            int t_sqRadius = circle.radius * circle.radius;
            return t_differenceFromCircle.sqrMagnitude <= t_sqRadius;
        }
        public static bool IsPointWithinLineEncompassingRectangle(LineInt line, Vector2Int point)
        {
            Vector2Int t_q = line.point1;
            Vector2Int t_p = line.point2;

            return point.x <= Mathf.Max(t_q.x, t_p.x) && point.x >= Mathf.Min(t_q.x, t_p.x) && point.y <= Mathf.Max(t_q.y, t_p.y) && point.y >= Mathf.Min(t_q.y, t_p.y);
        }
        public static bool IsPointOnLine(LineInt line, Vector2Int point)
        {
            // Figure out if they are collinear
            eTriplePointOrientation t_orientation = Orientation(line.point1, line.point2, point);

            switch (t_orientation)
            {
                case eTriplePointOrientation.Linear:
                {
                    // We now know the point lies on the infinite line, just check if its within the encompassing rectangle.
                    return IsPointWithinLineEncompassingRectangle(line, point);
                }
                default: return false;
            }
        }

        public static bool CircleRectangleOverlap(CircleInt circle, RectangleInt rectangle)
        {
            CircleInt t_rectContainingCircle = rectangle.boundingCircle;
            if (!CircleCircleOverlap(circle, t_rectContainingCircle))
            {
                // They are too far from each other to even check.
                return false;
            }

            // Circle is in rectangle
            if (IsPointInRectangle(rectangle, circle.center))
            {
                return true;
            }

            foreach (LineInt t_edge in rectangle.edges)
            {
                if (CircleLineOverlap(circle, t_edge))
                {
                    return true;
                }
            }

            return false;
        }
        public static int CircleRectangleOverlap(CircleInt circle, RectangleInt rectangle, out List<Vector2> intersectPoints)
        {
            CircleInt t_rectContainingCircle = rectangle.boundingCircle;
            if (!CircleCircleOverlap(circle, t_rectContainingCircle))
            {
                intersectPoints = new List<Vector2>();
                // They are too far from each other to even check.
                return 0;
            }

            intersectPoints = new List<Vector2>();
            List<Vector2> t_circLineIntersects;
            foreach (LineInt t_edge in rectangle.edges)
            {
                if (CircleLineOverlap(circle, t_edge, out t_circLineIntersects) > 0)
                {
                    intersectPoints.AddRange(t_circLineIntersects);
                }
            }

            // Circle is in rectangle
            if (intersectPoints.Count == 0 && IsPointInRectangle(rectangle, circle.center))
            {
                intersectPoints.Add(circle.center);
                return 1;
            }

            return intersectPoints.Count;
        }

        public static bool LineRectangleOverlap(LineInt line, RectangleInt rectangle)
        {
            CircleInt t_lineBoundingCircle = line.boundingCircle;
            CircleInt t_rectBoundingCircle = rectangle.boundingCircle;

            if (!CircleCircleOverlap(t_lineBoundingCircle, t_rectBoundingCircle))
            {
                return false;
            }

            if (IsPointInRectangle(rectangle, line.point1) || IsPointInRectangle(rectangle, line.point2))
            {
                return true;
            }

            foreach (LineInt t_edge in rectangle.edges)
            {
                if (LineLineIntersect(line, t_edge))
                {
                    return true;
                }
            }

            return false;
        }
        public static int LineRectangleOverlap(LineInt line, RectangleInt rectangle, out List<Vector2> intersectionPoints)
        {
            CircleInt t_lineBoundingCircle = line.boundingCircle;
            CircleInt t_rectBoundingCircle = rectangle.boundingCircle;

            if (!CircleCircleOverlap(t_lineBoundingCircle, t_rectBoundingCircle))
            {
                intersectionPoints = new List<Vector2>();
                return 0;
            }

            intersectionPoints = new List<Vector2>();
            foreach (LineInt t_edge in rectangle.edges)
            {
                if (LineLineIntersect(line, t_edge, out Vector2 t_interPointOnEdge))
                {
                    intersectionPoints.Add(t_interPointOnEdge);
                }
            }

            if (intersectionPoints.Count == 0)
            {
                if (IsPointInRectangle(rectangle, line.point1))
                {
                    intersectionPoints.Add(ConvertIntPositionToFloatPosition(line.point1));
                }
                if (IsPointInRectangle(rectangle, line.point2))
                {
                    intersectionPoints.Add(ConvertIntPositionToFloatPosition(line.point2));
                }
            }

            return intersectionPoints.Count;
        }
        public static int LineRectangleIntersect(LineInt line, RectangleInt rectangle, out List<Vector2> intersectionPoints)
        {
            CircleInt t_lineBoundingCircle = line.boundingCircle;
            CircleInt t_rectBoundingCircle = rectangle.boundingCircle;

            if (!CircleCircleOverlap(t_lineBoundingCircle, t_rectBoundingCircle))
            {
                intersectionPoints = new List<Vector2>();
                return 0;
            }

            intersectionPoints = new List<Vector2>();
            foreach (LineInt t_edge in rectangle.edges)
            {
                if (LineLineIntersect(line, t_edge, out Vector2 t_interPointOnEdge))
                {
                    intersectionPoints.Add(t_interPointOnEdge);
                }
            }

            return intersectionPoints.Count;
        }

        public static bool RectangleRectangleOverlap(RectangleInt a, RectangleInt b)
        {
            Vector2Int t_aMin = a.min;
            Vector2Int t_aMax = a.max;

            Vector2Int t_bMin = b.min;
            Vector2Int t_bMax = b.max;

            return t_aMin.x <= t_bMax.x && t_aMax.x >= t_bMin.x && t_aMin.y <= t_bMax.y && t_aMax.y >= t_bMin.y;
        }
        public static int RectangleRectangleOverlap(RectangleInt a, RectangleInt b, out List<Vector2> intersectionPoints)
        {
            if (!RectangleRectangleOverlap(a, b))
            {
                intersectionPoints = new List<Vector2>();
                return 0;
            }
            else
            {
                intersectionPoints = new List<Vector2>();
                foreach (LineInt t_line in b.edges)
                {
                    if (LineRectangleIntersect(t_line, a, out List<Vector2> t_singleLineIntersectPoints) > 0)
                    {
                        intersectionPoints.AddRange(t_singleLineIntersectPoints);
                    }
                }

                if (intersectionPoints.Count == 0)
                {
                    if (a.width < b.width)
                    {
                        foreach (Vector2Int t_cornerPoint in a.points)
                        {
                            intersectionPoints.Add(ConvertIntPositionToFloatPosition(t_cornerPoint));
                        }
                    }
                    else
                    {
                        foreach (Vector2Int t_cornerPoint in b.points)
                        {
                            intersectionPoints.Add(ConvertIntPositionToFloatPosition(t_cornerPoint));
                        }
                    }
                }

                return intersectionPoints.Count;
            }
        }
        public static bool RectangleRectangleOverlapIgnoreEdgeTouch(RectangleInt a, RectangleInt b)
        {
            Vector2Int t_aMin = a.min;
            Vector2Int t_aMax = a.max;

            Vector2Int t_bMin = b.min;
            Vector2Int t_bMax = b.max;

            return t_aMin.x < t_bMax.x && t_aMax.x > t_bMin.x && t_aMin.y < t_bMax.y && t_aMax.y > t_bMin.y;
        }
        public static bool DoesRectangleEncapsulateRectangle(RectangleInt a, RectangleInt b)
        {
            Vector2Int t_aMin = a.min;
            Vector2Int t_aMax = a.max;

            Vector2Int t_bMin = b.min;
            Vector2Int t_bMax = b.max;

            return t_aMin.x <= t_bMin.x && t_aMax.x >= t_bMax.x && t_aMin.y <= t_bMin.y && t_aMax.y >= t_bMax.y;
        }

        public static bool RectangleCastToRectangle(RectangleInt rectangle, Vector2 dir, float maxDistance, RectangleInt otherRectangle, out float hitDistance, out int hitEdgeIndex) => RectangleCastToRectangle(rectangle, dir, maxDistance, otherRectangle, out hitDistance, out _, out _, out hitEdgeIndex);
        public static bool RectangleCastToRectangle(RectangleInt rectangle, Vector2 dir, float maxDistance, RectangleInt otherRectangle, out float hitDistance, out Vector2 hitPointOnRectangle, out Vector2 hitPointOnOtherRectangle, out int hitEdgeIndex)
        {
            // Check if they are already overlapping
            if (RectangleRectangleOverlapIgnoreEdgeTouch(rectangle, otherRectangle))
            {
                // So, we do want to know where the hit WOULD be if they weren't overlapping (rewind time a bit kind of).
                // We will do the same calculations after moving the rectangle backwards along its direction.
                // The only question is how far do we move it? We just need to move it FAR ENOUGH, and far enough should be the difference between the rectangles.
                Vector2Int t_rectDiff = otherRectangle.botLeftPoint - rectangle.botLeftPoint;
                float t_distFloat = t_rectDiff.magnitude * INVERTED_INTS_PER_UNIT_AS_FLOAT;
                if (t_distFloat == 0.0f)
                {
                    t_distFloat = 1.0f;
                }
                Vector2 t_rectBotLeftFloat = ConvertIntPositionToFloatPosition(rectangle.botLeftPoint);
                Vector2 t_newCheckRectBotLeftFloat = t_rectBotLeftFloat - dir * t_distFloat;
                Vector2Int t_newCheckRectBotLeft = ConvertFloatPositionToIntPosition(t_newCheckRectBotLeftFloat);
                rectangle = new RectangleInt(t_newCheckRectBotLeft, rectangle.size);
                maxDistance += t_distFloat;
                if (!RectangleCastToRectangle(rectangle, dir, maxDistance, otherRectangle, out float t_newHitDist, out hitPointOnRectangle, out hitPointOnOtherRectangle, out hitEdgeIndex))
                {
                    //CustomDebug.LogError($"The RectangleCastToRectangle showed overlap but failed the backup cast.");
                    hitDistance = 0.0f;
                    hitPointOnRectangle = ConvertIntPositionToFloatPosition(rectangle.center);
                    hitPointOnOtherRectangle = ConvertIntPositionToFloatPosition(otherRectangle.center);
                    return true;
                }
                else
                {
                    hitDistance = t_newHitDist - maxDistance;
                    // We are all good, it did as expected.
                    return true;
                }
            }
            else if (RectangleRectangleOverlap(rectangle, otherRectangle))
            {
                // Edges are 100% touching
                // Edges can be handled correctly normally except if opposite corners of the rectangles are touching.
                for (int i = 0; i < 4; ++i)
                {
                    int t_oppCornerIndex = (i + 2) % 4;
                    if (rectangle.points[i].Equals(otherRectangle.points[t_oppCornerIndex]))
                    {
                        bool t_goingCorrectDir;
                        switch (i)
                        {
                            case 0: t_goingCorrectDir = dir.x < 0.0f && dir.y < 0.0f; break;
                            case 1: t_goingCorrectDir = dir.x < 0.0f && dir.y > 0.0f; break;
                            case 2: t_goingCorrectDir = dir.x > 0.0f && dir.y > 0.0f; break;
                            case 3: t_goingCorrectDir = dir.x > 0.0f && dir.y < 0.0f; break;
                            default: CustomDebug.UnhandledEnum(i, nameof(CustomPhysics2DInt)); t_goingCorrectDir = false; break;
                        }
                        if (t_goingCorrectDir)
                        {
                            hitEdgeIndex = t_oppCornerIndex;
                            hitDistance = 0.0f;
                            hitPointOnRectangle = ConvertIntPositionToFloatPosition(rectangle.points[i]);
                            hitPointOnOtherRectangle = hitPointOnRectangle;
                            return true;
                        }
                        else
                        {
                            hitEdgeIndex = -1;
                            hitDistance = float.NaN;
                            hitPointOnRectangle = new Vector2(float.NaN, float.NaN);
                            hitPointOnOtherRectangle = new Vector2(float.NaN, float.NaN);
                            return false;
                        }
                    }
                }



                //// Edge case: Edges are touching
                //List<(LineInt, int)> t_edgesToCheck = GetEdgesOfRectangleThatRaycastCouldHit(rectangle, dir);
                //List<(LineInt, int)> t_otherEdgesToCheck = new List<(LineInt, int)>(4 - t_edgesToCheck.Count);
                //for (int i = 0; i < 4; ++i)
                //{
                //    bool t_doesEdgesAlreadyContain = false;
                //    foreach ((LineInt t_rectEdge, int t_rectEdgeIndex) in t_edgesToCheck)
                //    {
                //        if (i == t_rectEdgeIndex)
                //        {
                //            t_doesEdgesAlreadyContain = true;
                //            break;
                //        }
                //    }
                //    if (!t_doesEdgesAlreadyContain)
                //    {
                //        t_otherEdgesToCheck.Add((otherRectangle.edges[i], i));
                //    }
                //}
                
                //foreach ((LineInt t_otherEdge, int t_otherEdgeIndex) in t_otherEdgesToCheck)
                //{
                //    foreach ((LineInt t_rectEdge, int t_recEdgeIndex) in t_edgesToCheck)
                //    {
                //        if (LineLineIntersect(t_otherEdge, t_rectEdge, out Vector2 t_intersectPoint))
                //        {
                //            hitEdgeIndex = t_otherEdgeIndex;
                //            hitDistance = 0.0f;
                //            hitPointOnRectangle = t_intersectPoint;
                //            hitPointOnOtherRectangle = t_intersectPoint;
                //            return false;
                //        }
                //    }
                //}
            }

            // 1. Gather points on the rectangle we will be raycasting from.
            List<Vector2Int> t_rectPointsToRaycastFrom = GetPointsOnRectangleToRaycastFrom(rectangle, dir);
            // 2. Grab the lines on the other rectangle that could be the ones getting hit (now done inside RaycastMultipleToRectangle).
            // 3. Raycast from each of these points and see if the ray intersects any of those edges.
            List<(Vector2 rectPoint, Vector2 otherRectPoint, int hitEdgeOfOtherRect)> t_hits = new List<(Vector2, Vector2, int)>();
            if (RaycastMultipleToRectangle(t_rectPointsToRaycastFrom, dir, maxDistance, otherRectangle, out Vector2 t_startPosThatHit, out Vector2 t_hitPointOnRectangle, out _, out int t_edgeIndex))
            {
                t_hits.Add((t_startPosThatHit, t_hitPointOnRectangle, t_edgeIndex));
            }

            // 4. Do the same 3 steps as above, but for the other rectangle.
            Vector2 t_reverseDir = -dir;
            List<Vector2Int> t_otherRectPointsToRaycastFrom = GetPointsOnRectangleToRaycastFrom(otherRectangle, t_reverseDir);
            if (RaycastMultipleToRectangle(t_otherRectPointsToRaycastFrom, t_reverseDir, maxDistance, rectangle, out t_startPosThatHit, out t_hitPointOnRectangle, out _, out t_edgeIndex))
            {
                t_hits.Add((t_hitPointOnRectangle, t_startPosThatHit, (t_edgeIndex + 2) % 4));
            }

            // 5. If there was no hits
            if (t_hits.Count == 0)
            {
                hitDistance = 0.0f;
                hitPointOnRectangle = new Vector2(float.NaN, float.NaN);
                hitPointOnOtherRectangle = new Vector2(float.NaN, float.NaN);
                hitEdgeIndex = -1;
                return false;
            }
            // 6. Find the hit that travelled the least distance (the one that hit first).
            float t_closestHitSqDist = float.PositiveInfinity;
            (Vector2 rectPoint, Vector2 otherRectPoint, int hitEdgedIndex) t_closestHit = (Vector2.zero, Vector2.zero, -1);
            foreach ((Vector2 t_rectPoint, Vector2 t_otherRectPoint, int t_hitEdgeIndex) in t_hits)
            {
                Vector2 t_diff = t_rectPoint - t_otherRectPoint;
                float t_sqMag = t_diff.sqrMagnitude;
                if (t_sqMag < t_closestHitSqDist)
                {
                    t_closestHitSqDist = t_sqMag;
                    t_closestHit = (t_rectPoint, t_otherRectPoint, t_hitEdgeIndex);
                }
            }

            hitPointOnRectangle = t_closestHit.rectPoint;
            hitPointOnOtherRectangle = t_closestHit.otherRectPoint;
            hitDistance = Mathf.Sqrt(t_closestHitSqDist);
            hitEdgeIndex = t_closestHit.hitEdgedIndex;
            return true;
        }
        private static List<Vector2Int> GetPointsOnRectangleToRaycastFrom(RectangleInt rectangle, Vector2 dir)
        {
            List<Vector2Int> t_pointsToRaycastFrom = new List<Vector2Int>(3);
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
        /// <param name="edgeIndex">Index of the edge on the rectangle that was hit.</param>
        /// <returns>True if raycast hit. False otherwise.</returns>
        public static bool RaycastMultipleToRectangle(ICollection<Vector2Int> startPositions, Vector2 dir, float maxDistance, RectangleInt rectangle, out Vector2 startPosThatHit, out Vector2 hitPointOnRectangle, out float hitDistance, out int edgeIndex)
        {
            List<(LineInt edge, int edgeIndex)> t_rectLinesThatCouldBeHit = GetEdgesOfRectangleThatRaycastCouldHit(rectangle, dir);

            List<(Vector2 startPoint, Vector2 hitPoint, int edgeIndex)> t_hits = new List<(Vector2, Vector2, int)>(startPositions.Count * 3);
            foreach (Vector2Int t_startPos in startPositions)
            {
                Vector2 t_startPosFloat = ConvertIntPositionToFloatPosition(t_startPos);
                Vector2 t_endPoint = t_startPosFloat + dir * maxDistance;
                Vector2Int t_endPointInt = ConvertFloatPositionToIntPosition(t_endPoint);
                LineInt t_ray = new LineInt(t_startPos, t_endPointInt);

                foreach ((LineInt t_edge, int t_edgeIndex) in t_rectLinesThatCouldBeHit)
                {
                    if (LineLineIntersect(t_ray, t_edge, out Vector2 t_intersectionPoint))
                    {
                        t_hits.Add((t_startPosFloat, t_intersectionPoint, t_edgeIndex));
                    }
                }
            }

            if (t_hits.Count == 0)
            {
                hitDistance = 0.0f;
                startPosThatHit = new Vector2(float.NaN, float.NaN);
                hitPointOnRectangle = new Vector2(float.NaN, float.NaN);
                edgeIndex = -1;
                return false;
            }

            // Find the closest hit point
            hitDistance = float.PositiveInfinity;
            (Vector2 startPoint, Vector2 hitPoint, int edgeIndex) t_hit = (new Vector2(float.NaN, float.NaN), new Vector2(float.NaN, float.NaN), -1);
            foreach ((Vector2 t_startPoint, Vector2 t_hitPoint, int t_edgeIndex) in t_hits)
            {
                Vector2 t_diff = t_hitPoint - t_startPoint;
                float t_sqDist = t_diff.sqrMagnitude;
                if (t_sqDist < hitDistance)
                {
                    hitDistance = t_sqDist;
                    t_hit = (t_startPoint, t_hitPoint, t_edgeIndex);
                }
            }

            hitDistance = Mathf.Sqrt(hitDistance);
            startPosThatHit = t_hit.startPoint;
            hitPointOnRectangle = t_hit.hitPoint;
            edgeIndex = t_hit.edgeIndex;
            return true;
        }
        private static List<(LineInt, int)> GetEdgesOfRectangleThatRaycastCouldHit(RectangleInt rectangle, Vector2 dir)
        {
            List<(LineInt, int)> t_edgesThatCouldBeHit = new List<(LineInt, int)>(2);
            if (dir.x < 0.0f)
            {
                t_edgesThatCouldBeHit.Add((rectangle.rightEdge, 2));
            }
            else if (dir.x > 0.0f)
            {
                t_edgesThatCouldBeHit.Add((rectangle.leftEdge, 0));
            }

            if (dir.y < 0.0f)
            {
                t_edgesThatCouldBeHit.Add((rectangle.topEdge, 1));
            }
            if (dir.y > 0.0f)
            {
                t_edgesThatCouldBeHit.Add((rectangle.botEdge, 3));
            }

            return t_edgesThatCouldBeHit;
        }

        /// <param name="dir">Assumed to be normalized.</param>
        /// <param name="distance">Assumed to be positive.</param>
        public static CircleInt CreateMinimumBoundingCircleForCircleCast(CircleInt circleBeingCast, Vector2 dir, float distance)
        {
            float t_halfDistFloat = distance * 0.5f;
            Vector2 t_circleBeingCastCenterFloat = ConvertIntPositionToFloatPosition(circleBeingCast.center);
            Vector2 t_boundCircleCenterFloat = t_circleBeingCastCenterFloat + (dir * t_halfDistFloat);
            Vector2Int t_boundCircleCenter = ConvertFloatPositionToIntPosition(t_boundCircleCenterFloat);
            int t_halfDist = Mathf.CeilToInt(INTS_PER_UNIT * t_halfDistFloat);
            return new CircleInt(t_boundCircleCenter, t_halfDist + circleBeingCast.radius);
        }
        /// <param name="dir">Assumed to be normalized.</param>
        /// <param name="distance">Assumed to be positive.</param>
        public static CircleInt CreateMinimumBoundingCircleForRectangleCast(RectangleInt rectangleBeingCast, Vector2 dir, float distance)
        {
            CircleInt t_rectCircleBounds = rectangleBeingCast.boundingCircle;

            Vector2 t_rectBotLeftPosFloat = rectangleBeingCast.GetBotLeftPointAsFloatPosition();
            Vector2 t_desiredEndBotLeftPosFloat = t_rectBotLeftPosFloat + dir * distance;
            Vector2 t_boundingCircleBotLeftPosFloat = (t_rectBotLeftPosFloat + t_desiredEndBotLeftPosFloat) * 0.5f;
            Vector2 t_rectSizeFloat = rectangleBeingCast.GetSizeAsFloatSize();
            Vector2 t_boundingCircleCenterPosFloat = t_boundingCircleBotLeftPosFloat + t_rectSizeFloat * 0.5f;
            Vector2Int t_boundingCircleCenterPos = ConvertFloatPositionToIntPosition(t_boundingCircleCenterPosFloat);

            float t_rectCircleBoundsRadiusFloat = INVERTED_INTS_PER_UNIT_AS_FLOAT * t_rectCircleBounds.radius;
            float t_boundingCircleRadiusFloat = distance * 0.5f + t_rectCircleBoundsRadiusFloat;
            int t_boundingCircleRadius = Mathf.CeilToInt(INTS_PER_UNIT * t_boundingCircleRadiusFloat);

            return new CircleInt(t_boundingCircleCenterPos, t_boundingCircleRadius);
        }

        public static bool IsPointInRectangle(RectangleInt rectangle, Vector2Int point)
        {
            Vector2Int t_q = rectangle.min;
            Vector2Int t_p = rectangle.max;

            return point.x <= Mathf.Max(t_q.x, t_p.x) && point.x >= Mathf.Min(t_q.x, t_p.x) && point.y <= Mathf.Max(t_q.y, t_p.y) && point.y >= Mathf.Min(t_q.y, t_p.y);
        }
        public static bool IsPointInRectangleIgnoreEdgeTouch(RectangleInt rectangle, Vector2Int point)
        {
            Vector2Int t_q = rectangle.min;
            Vector2Int t_p = rectangle.max;

            return point.x < Mathf.Max(t_q.x, t_p.x) && point.x > Mathf.Min(t_q.x, t_p.x) && point.y < Mathf.Max(t_q.y, t_p.y) && point.y > Mathf.Min(t_q.y, t_p.y);
        }
        public static float DistanceFromLine(LineInt line, Vector2Int point)
        {
            Vector2Int t_nearestPointOnInfLine = NearestPointOnInfiniteLine(line, point);
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
        /// <summary>
        /// Finds the point on the given infinite line that is closest to the given point.
        /// </summary>
        /// <param name="point">Point that may or may not be on the line.</param>
        public static Vector2Int NearestPointOnInfiniteLine(LineInt infiniteLine, Vector2Int point)
        {
            Vector2 t_lineDir = (infiniteLine.point1.ToVector2() - infiniteLine.point2.ToVector2()).normalized;
            Vector2 t_diffToPoint = point - infiniteLine.point1;
            float t_dot = Vector2.Dot(t_diffToPoint, t_lineDir);
            return (infiniteLine.point1 + t_lineDir * t_dot).RoundToVector2Int();
        }
        /// <summary>
        /// If the given point falls on the line segment between the end points of the given line.
        /// </summary>
        private static bool IsInfiniteLinePointOnLineSegment(LineInt line, Vector2Int point)
        {
            int t_minX = line.point1.x;
            int t_maxX = line.point2.x;
            if (line.point2.x <= line.point1.x)
            {
                t_minX = line.point2.x;
                t_maxX = line.point1.x;
            }

            int t_minY = line.point1.y;
            int t_maxY = line.point2.y;
            if (line.point2.y <= line.point1.y)
            {
                t_minY = line.point2.y;
                t_maxY = line.point1.y;
            }

            Vector2Int t_botLeft = new Vector2Int(t_minX, t_minY);
            Vector2Int t_size = new Vector2Int(t_maxX - t_minX, t_maxY - t_minY);
            RectangleInt t_bounds = new RectangleInt(t_botLeft, t_size);
            return IsPointInRectangle(t_bounds, point);
        }
    }
}