using Helpers.Physics.Custom2DInt;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Helpers.Physics.Custom2D.Tests
{
    public sealed class CircleLineTest : MonoBehaviour
    {
        [SerializeField] private eTestChoice m_test = eTestChoice.CircleLine;
        [SerializeField] private float m_speed = 1.0f;

        [SerializeField, BoxGroup("Circle"), ShowIf(nameof(ShouldShowCircleInfo))] 
        private float m_circleRadius = 1.0f;
        [SerializeField, BoxGroup("Circle"), ShowIf(nameof(ShouldShowCircleInfo))] 
        private Vector2 m_circlePos = Vector2.zero;

        [SerializeField, BoxGroup("OtherCircle"), ShowIf(nameof(ShouldShowOtherCircleInfo))]
        private float m_otherCircleRadius = 2.0f;
        [SerializeField, BoxGroup("OtherCircle"), ShowIf(nameof(ShouldShowOtherCircleInfo))]
        private Vector2 m_otherCirclePos = new Vector2(1.5f, 1.0f);

        [SerializeField, BoxGroup("Capsule"), ShowIf(nameof(ShouldShowCapsuleInfo))]
        private float m_capsuleRadius = 1.0f;
        [SerializeField, BoxGroup("Capsule"), ShowIf(nameof(ShouldShowCapsuleInfo))]
        private Vector2 m_capsulePos1 = new Vector2(0.0f, 1.0f);
        [SerializeField, BoxGroup("Capsule"), ShowIf(nameof(ShouldShowCapsuleInfo))]
        private Vector2 m_capsulePos2 = new Vector2(0.0f, -1.0f);

        [SerializeField, BoxGroup("Line"), ShowIf(nameof(ShouldShowLineInfo))] 
        private Vector2 m_linePos1 = new Vector2(2.0f, 0.5f);
        [SerializeField, BoxGroup("Line"), ShowIf(nameof(ShouldShowLineInfo))]
        private Vector2 m_linePos2 = new Vector2(2.0f, -0.5f);

        [SerializeField, BoxGroup("OtherLine"), ShowIf(nameof(ShouldShowOtherLineInfo))]
        private Vector2 m_otherLinePos1 = new Vector2(-2.0f, 0.0f);
        [SerializeField, BoxGroup("OtherLine"), ShowIf(nameof(ShouldShowOtherLineInfo))]
        private Vector2 m_otherLinePos2 = new Vector2(-1.0f, 0.0f);

        [SerializeField, BoxGroup("Rectangle"), ShowIf(nameof(ShouldShowRectangleInfo))]
        private Vector2 m_rectangleCenter = new Vector2(0.0f, 0.0f);
        [SerializeField, BoxGroup("Rectangle"), ShowIf(nameof(ShouldShowRectangleInfo))]
        private Vector2 m_rectangleSize = new Vector2(1.0f, 0.5f);

        [SerializeField, BoxGroup("OtherRectangle"), ShowIf(nameof(ShouldShowOtherRectangleInfo))]
        private Vector2 m_otherRectangleCenter = new Vector2(1.0f, 0.0f);
        [SerializeField, BoxGroup("OtherRectangle"), ShowIf(nameof(ShouldShowOtherRectangleInfo))]
        private Vector2 m_otherRectangleSize = new Vector2(0.5f, 1.0f);

        [SerializeField, BoxGroup("Cast Info"), ShowIf(nameof(ShouldShowCastInfo))]
        private Vector2 m_castDir = Vector2.right;
        [SerializeField, BoxGroup("Cast Info"), ShowIf(nameof(ShouldShowCastInfo))]
        private float m_castDistance = 1.0f;

        [SerializeField, BoxGroup("CircleInt"), ShowIf(nameof(ShouldShowCircleIntInfo)), ReadOnly]
        private Vector2Int m_circleIntCenter = Vector2Int.zero;
        [SerializeField, BoxGroup("CircleInt"), ShowIf(nameof(ShouldShowCircleIntInfo))]
        private int m_circleIntRadius = 64;

        [SerializeField, BoxGroup("OtherCircleInt"), ShowIf(nameof(ShouldShowOtherCircleIntInfo))]
        private Vector2Int m_otherCircleIntCenter = new Vector2Int(128, 128);
        [SerializeField, BoxGroup("OtherCircleInt"), ShowIf(nameof(ShouldShowOtherCircleIntInfo))]
        private int m_otherCircleIntRadius = 32;

        [SerializeField, BoxGroup("LineInt"), ShowIf(nameof(ShouldShowLineIntInfo))]
        private Vector2Int m_lineIntPos1 = new Vector2Int(128, 32);
        [SerializeField, BoxGroup("LineInt"), ShowIf(nameof(ShouldShowLineIntInfo))]
        private Vector2Int m_lineIntPos2 = new Vector2Int(128, -32);

        [SerializeField, BoxGroup("OtherLineInt"), ShowIf(nameof(ShouldShowOtherLineIntInfo))]
        private Vector2Int m_otherLineIntPos1 = new Vector2Int(-128, 16);
        [SerializeField, BoxGroup("OtherLineInt"), ShowIf(nameof(ShouldShowOtherLineIntInfo))]
        private Vector2Int m_otherLineIntPos2 = new Vector2Int(-64, 48);

        [SerializeField, BoxGroup("PointInt"), ShowIf(nameof(ShouldShowPointIntInfo)), ReadOnly]
        private Vector2Int m_pointInt = new Vector2Int(0, 0);
        [SerializeField, BoxGroup("PointInt"), ShowIf(nameof(ShouldShowPointIntInfo))]
        private int m_pointIntVisualRadius = 8;

        [SerializeField, BoxGroup("RectangleInt"), ShowIf(nameof(ShouldShowRectangleIntInfo))]
        private Vector2Int m_rectangleIntSize = new Vector2Int(64, 32);

        [SerializeField, BoxGroup("OtherRectangleInt"), ShowIf(nameof(ShouldShowOtherRectangleIntInfo))]
        private Vector2Int m_otherRectangleIntBotLeftPos = new Vector2Int(-32, -16);
        [SerializeField, BoxGroup("OtherRectangleInt"), ShowIf(nameof(ShouldShowOtherRectangleIntInfo))]
        private Vector2Int m_otherRectangleIntSize = new Vector2Int(64, 32);

        private Vector2 m_internalIntTestPosAsFloat = Vector2.zero;


        // Update is called once per frame
        private void Update()
        {
            Vector2 t_moveDir = Vector2.zero;
            if (Input.GetKey(KeyCode.D))
            {
                t_moveDir += Vector2.right;
            }
            if (Input.GetKey(KeyCode.A))
            {
                t_moveDir += Vector2.left;
            }
            if (Input.GetKey(KeyCode.W))
            {
                t_moveDir += Vector2.up;
            }
            if (Input.GetKey(KeyCode.S))
            {
                t_moveDir += Vector2.down;
            }
            if (t_moveDir != Vector2.zero)
            {
                t_moveDir.Normalize();
            }

            Vector2 t_moveAm = t_moveDir * (m_speed * Time.deltaTime);

            switch (m_test)
            {
                case eTestChoice.CircleLine:
                {
                    m_circlePos += t_moveAm;

                    Circle t_circle = new Circle(m_circlePos, m_circleRadius);
                    Line t_line = new Line(m_linePos1, m_linePos2);
                    bool t_isOverlapping = CustomPhysics2D.CircleLineOverlap(t_circle, t_line, true);

                    t_circle.DrawDebug(t_isOverlapping ? Color.red : Color.green);
                    t_line.DrawDebug(Color.white);
                    break;
                }
                case eTestChoice.CapsuleLine:
                {
                    m_capsulePos1 += t_moveAm;
                    m_capsulePos2 += t_moveAm;

                    Capsule t_capsule = new Capsule(m_capsulePos1, m_capsulePos2, m_capsuleRadius);
                    Line t_line = new Line(m_linePos1, m_linePos2);
                    bool t_isOverlapping = CustomPhysics2D.CapsuleLineOverlap(t_capsule, t_line, true);

                    t_capsule.DrawDebug(t_isOverlapping ? Color.red : Color.green);
                    t_line.DrawDebug(Color.white);
                    break;
                }
                case eTestChoice.InfiniteLines:
                {
                    m_otherLinePos1 += t_moveAm;
                    m_otherLinePos2 += t_moveAm;

                    Line t_line0 = new Line(m_linePos1, m_linePos2);
                    Line t_line1 = new Line(m_otherLinePos1, m_otherLinePos2);
                    bool t_isOverlapping = CustomPhysics2D.InfiniteLinesIntersect(t_line0, t_line1, out Vector2 t_interPoint, true);

                    t_line1.DrawDebug(t_isOverlapping ? Color.red : Color.green);
                    t_line0.DrawDebug(Color.white);
                    break;
                }
                case eTestChoice.CircleCircle:
                {
                    m_circlePos += t_moveAm;

                    Circle t_circle0 = new Circle(m_circlePos, m_circleRadius);
                    Circle t_circle1 = new Circle(m_otherCirclePos, m_otherCircleRadius);
                    bool t_isOverlapping = CustomPhysics2D.CircleCircleOverlap(t_circle0, t_circle1);

                    t_circle0.DrawDebug(t_isOverlapping ? Color.red : Color.green);
                    t_circle1.DrawDebug(Color.white);
                    break;
                }
                case eTestChoice.RectangleRectangle:
                {
                    m_rectangleCenter += t_moveAm;

                    Rectangle t_rectangle0 = new Rectangle(m_rectangleCenter, m_rectangleSize);
                    Rectangle t_rectangle1 = new Rectangle(m_otherRectangleCenter, m_otherRectangleSize);
                    bool t_isOverlapping = CustomPhysics2D.RectangleRectangleOverlap(t_rectangle0, t_rectangle1);

                    t_rectangle0.DrawOutlineDebug(t_isOverlapping ? Color.red : Color.green);
                    t_rectangle1.DrawOutlineDebug(Color.white);
                    break;
                }
                case eTestChoice.CapsuleRectangle:
                {
                    m_capsulePos1 += t_moveAm;
                    m_capsulePos2 += t_moveAm;

                    Capsule t_capsule = new Capsule(m_capsulePos1, m_capsulePos2, m_capsuleRadius);
                    Rectangle t_rectangle = new Rectangle(m_otherRectangleCenter, m_otherRectangleSize);
                    bool t_isOverlapping = CustomPhysics2D.CapsuleRectangleOverlap(t_capsule, t_rectangle, out _, true);

                    t_capsule.DrawDebug(t_isOverlapping ? Color.red : Color.green);
                    t_rectangle.DrawOutlineDebug(Color.white);
                    break;
                }
                case eTestChoice.CircleRectangle:
                {
                    m_circlePos += t_moveAm;

                    Circle t_circle = new Circle(m_circlePos, m_circleRadius);
                    Rectangle t_rectangle = new Rectangle(m_otherRectangleCenter, m_otherRectangleSize);
                    bool t_isOverlapping = CustomPhysics2D.CircleRectangleOverlap(t_circle, t_rectangle, true);

                    t_circle.DrawDebug(t_isOverlapping ? Color.red : Color.green);
                    t_rectangle.DrawOutlineDebug(Color.white);
                    break;
                }
                case eTestChoice.CircleCastRectangle:
                {
                    m_circlePos += t_moveAm;

                    Circle t_circle = new Circle(m_circlePos, m_circleRadius);
                    Capsule t_castedCircle = new Capsule(t_circle.center, t_circle.center + m_castDir * m_castDistance, t_circle.radius);
                    Rectangle t_rectangle = new Rectangle(m_otherRectangleCenter, m_otherRectangleSize);
                    t_rectangle.DrawOutlineDebug(Color.white);
                    bool t_isOverlapping = CustomPhysics2D.CircleCastToRectangle(t_circle, m_castDir, m_castDistance, t_rectangle, out float t_hitDist, out Vector2 t_hitPointCircle, out Vector2 t_hitPointRectangle, true);

                    Color t_circleColor = t_isOverlapping ? Color.red : Color.green;
                    t_castedCircle.DrawDebug(0.5f * t_circleColor);
                    t_circle.DrawDebug(t_circleColor);

                    //CustomDebug.DrawCircle(t_hitPointCircle, 0.05f, 6, Color.cyan, true);
                    //CustomDebug.DrawCircle(t_hitPointRectangle, 0.05f, 4,  Color.yellow, true);
                    break;
                }
                case eTestChoice.CircleCastLine:
                {
                    m_circlePos += t_moveAm;

                    Circle t_circle = new Circle(m_circlePos, m_circleRadius);
                    Capsule t_castedCircle = new Capsule(t_circle.center, t_circle.center + m_castDir * m_castDistance, t_circle.radius);
                    Line t_line = new Line(m_otherLinePos1, m_otherLinePos2);
                    t_line.DrawDebug(Color.white);
                    bool t_isOverlapping = CustomPhysics2D.CircleCastToLine(t_circle, m_castDir, m_castDistance, t_line, out float t_hitDist, out Vector2 t_hitPointCircle, out Vector2 t_hitPointLine, true);

                    Color t_circleColor = t_isOverlapping ? Color.red : Color.green;
                    t_castedCircle.DrawDebug(0.5f * t_circleColor);
                    t_circle.DrawDebug(t_circleColor);

                    CustomDebug.DrawCircle(t_hitPointCircle, 0.05f, 6, Color.cyan, true);
                    CustomDebug.DrawCircle(t_hitPointLine, 0.05f, 4, Color.yellow, true);
                    break;
                }
                case eTestChoice.RectangleCastLine:
                {
                    m_rectangleCenter += t_moveAm;

                    Rectangle t_rectangle = new Rectangle(m_rectangleCenter, m_rectangleSize);
                    Line t_castLine = new Line(t_rectangle.center, t_rectangle.center + m_castDir * m_castDistance);
                    Rectangle t_endRectangle = new Rectangle(t_castLine.point2, m_rectangleSize);
                    Line t_line = new Line(m_otherLinePos1, m_otherLinePos2);
                    t_line.DrawDebug(Color.white);
                    bool t_isOverlapping = CustomPhysics2D.RectangleCastToLine(t_rectangle, m_castDir, m_castDistance, t_line, out float t_hitDist, out Vector2 t_hitPointCircle, out Vector2 t_hitPointLine, false);

                    Color t_rectangleColor = t_isOverlapping ? Color.red : Color.green;
                    t_castLine.DrawDebug(t_rectangleColor * 0.5f, true, false);
                    foreach (Vector2 t_edgePoint in t_rectangle.points)
                    {
                        Line t_castFromEdge = new Line(t_edgePoint, t_edgePoint + m_castDir * m_castDistance);
                        t_castFromEdge.DrawDebug(t_rectangleColor * 0.5f, true, false);
                    }
                    t_endRectangle.DrawOutlineDebug(t_rectangleColor * 0.5f, true);
                    t_rectangle.DrawOutlineDebug(t_rectangleColor);

                    CustomDebug.DrawCircle(t_hitPointCircle, 0.05f, 6, Color.cyan, true);
                    CustomDebug.DrawCircle(t_hitPointLine, 0.05f, 4, Color.yellow, true);
                    break;
                }
                case eTestChoice.RectangleCastRectangle:
                {
                    m_rectangleCenter += t_moveAm;

                    Rectangle t_rectangle = new Rectangle(m_rectangleCenter, m_rectangleSize);
                    Line t_castLine = new Line(t_rectangle.center, t_rectangle.center + m_castDir * m_castDistance);
                    Rectangle t_endRectangle = new Rectangle(t_castLine.point2, m_rectangleSize);
                    Rectangle t_otherRectangle = new Rectangle(m_otherRectangleCenter, m_otherRectangleSize);
                    t_otherRectangle.DrawOutlineDebug(Color.white);
                    bool t_isOverlapping = CustomPhysics2D.RectangleCastToRectangle(t_rectangle, m_castDir, m_castDistance, t_otherRectangle, out float t_hitDist, out Vector2 t_hitPointCircle, out Vector2 t_hitPointLine, true);

                    Color t_rectangleColor = t_isOverlapping ? Color.red : Color.green;
                    t_castLine.DrawDebug(t_rectangleColor * 0.5f, true, false);
                    foreach (Vector2 t_edgePoint in t_rectangle.points)
                    {
                        Line t_castFromEdge = new Line(t_edgePoint, t_edgePoint + m_castDir * m_castDistance);
                        t_castFromEdge.DrawDebug(t_rectangleColor * 0.5f, true, false);
                    }
                    t_endRectangle.DrawOutlineDebug(t_rectangleColor * 0.5f, true);
                    t_rectangle.DrawOutlineDebug(t_rectangleColor);

                    CustomDebug.DrawCircle(t_hitPointCircle, 0.05f, 6, Color.cyan, true);
                    CustomDebug.DrawCircle(t_hitPointLine, 0.05f, 4, Color.yellow, true);
                    break;
                }
                case eTestChoice.BoundingCircleCircleCircle:
                {
                    m_circlePos += t_moveAm;

                    Circle t_circle = new Circle(m_circlePos, m_circleRadius);
                    Circle t_otherCircle = new Circle(m_otherCirclePos, m_otherCircleRadius);
                    Circle t_boundingCircle = CustomPhysics2D.CreateMinimumBoundingCircle(t_circle, t_otherCircle);

                    t_circle.DrawDebug(Color.blue, true);
                    t_otherCircle.DrawDebug(Color.magenta, true);
                    t_boundingCircle.DrawDebug(Color.white, true);
                    break;
                }
                case eTestChoice.BoundingCircleCircleCast:
                {
                    m_circlePos += t_moveAm;

                    Circle t_circle = new Circle(m_circlePos, m_circleRadius);
                    Line t_ray = new Line(m_circlePos, m_circlePos + m_castDir * m_castDistance);
                    Circle t_endPosCircle = new Circle(t_ray.point2, m_circleRadius);
                    Circle t_boundingCircle = CustomPhysics2D.CreateMinimumBoundingCircleForCircleCast(t_circle, m_castDir, m_castDistance);

                    t_circle.DrawDebug(Color.green, true);
                    t_ray.DrawDebug(Color.green * 0.5f, true, false);
                    t_endPosCircle.DrawDebug(Color.green * 0.5f, true);
                    t_boundingCircle.DrawDebug(Color.white, true);
                    break;
                }
                case eTestChoice.CircleCompletelyInsideCircle:
                {
                    m_circlePos += t_moveAm;

                    Circle t_circle = new Circle(m_circlePos, m_circleRadius);
                    Circle t_otherCircle = new Circle(m_otherCirclePos, m_otherCircleRadius);

                    bool t_isCompletelyInside = CustomPhysics2D.IsEitherCircleCompletelyInsideOtherCircle(t_circle, t_otherCircle);

                    Color t_color = t_isCompletelyInside ? Color.red : Color.green;
                    t_circle.DrawDebug(t_color);
                    t_otherCircle.DrawDebug(t_color);

                    break;
                }
                case eTestChoice.CircleCircleInt:
                {
                    m_internalIntTestPosAsFloat += t_moveAm;

                    m_circleIntCenter = CustomPhysics2DInt.ConvertFloatPositionToIntPosition(m_internalIntTestPosAsFloat);
                    CircleInt t_circle = new CircleInt(m_circleIntCenter, m_circleIntRadius);
                    CircleInt t_otherCircle = new CircleInt(m_otherCircleIntCenter, m_otherCircleIntRadius);

                    bool t_isOverlapping = CustomPhysics2DInt.CircleCircleOverlap(t_circle, t_otherCircle);

                    t_circle.DrawDebug(t_isOverlapping ? Color.red : Color.green);
                    t_otherCircle.DrawDebug(Color.white);

                    break;
                }
                case eTestChoice.CircleLineInt:
                {
                    m_internalIntTestPosAsFloat += t_moveAm;

                    m_circleIntCenter = CustomPhysics2DInt.ConvertFloatPositionToIntPosition(m_internalIntTestPosAsFloat);
                    CircleInt t_circle = new CircleInt(m_circleIntCenter, m_circleIntRadius);
                    LineInt t_line = new LineInt(m_lineIntPos1, m_lineIntPos2);

                    int t_intersectionCount = CustomPhysics2DInt.CircleLineOverlap(t_circle, t_line, out List<Vector2> t_intersectPoints);

                    foreach (Vector2 t_intersectionPoint in t_intersectPoints)
                    {
                        CustomDebug.DrawCircle(t_intersectionPoint, 0.1f, 6, Color.cyan, true);
                    }

                    t_circle.DrawDebug(t_intersectionCount > 0 ? Color.red : Color.green);
                    t_line.DrawDebug(Color.white);
                    break;
                }
                case eTestChoice.LineLineInt:
                {
                    m_internalIntTestPosAsFloat += t_moveAm;

                    LineInt t_line = new LineInt(m_lineIntPos1, m_lineIntPos2);
                    Vector2Int t_lineOffset = CustomPhysics2DInt.ConvertFloatPositionToIntPosition(m_internalIntTestPosAsFloat) - t_line.center;
                    t_line = new LineInt(t_lineOffset + m_lineIntPos1, t_lineOffset + m_lineIntPos2);

                    LineInt t_otherLine = new LineInt(m_otherLineIntPos1, m_otherLineIntPos2);

                    bool t_doIntersect = CustomPhysics2DInt.LineLineIntersect(t_line, t_otherLine, out Vector2 t_intersectPoint);

                    t_line.DrawDebug(t_doIntersect ? Color.red : Color.green);
                    t_otherLine.DrawDebug(Color.white);
                    if (t_doIntersect)
                    {
                        CustomDebug.DrawCrossHair(t_intersectPoint, 0.1f, Color.blue, true);
                        CustomDebug.DrawCircle(t_intersectPoint, 0.1f, 6, Color.blue, true);
                    }
                    break;
                }
                case eTestChoice.PointOnLineInt:
                {
                    m_internalIntTestPosAsFloat += t_moveAm;

                    m_pointInt = CustomPhysics2DInt.ConvertFloatPositionToIntPosition(m_internalIntTestPosAsFloat);
                    CircleInt t_pointVisual = new CircleInt(m_pointInt, m_pointIntVisualRadius);
                    LineInt t_line = new LineInt(m_lineIntPos1, m_lineIntPos2);

                    bool t_isPointOnLine = CustomPhysics2DInt.IsPointOnLine(t_line, m_pointInt);

                    t_line.DrawDebug(Color.white);
                    Color t_pointColor = t_isPointOnLine ? Color.red : Color.green;
                    t_pointVisual.DrawDebug(t_pointColor);
                    CustomDebug.DrawCrossHair(CustomPhysics2DInt.ConvertIntPositionToFloatPosition(m_pointInt), (t_pointVisual.radius * 2) * CustomPhysics2DInt.INVERTED_INTS_PER_UNIT_AS_FLOAT, t_pointColor, true);

                    break;
                }
                case eTestChoice.CircleRectangleInt:
                {
                    m_internalIntTestPosAsFloat += t_moveAm;

                    Vector2Int t_rectangleBotLeft = CustomPhysics2DInt.ConvertFloatPositionToIntPosition(m_internalIntTestPosAsFloat);
                    RectangleInt t_rectangle = new RectangleInt(t_rectangleBotLeft, m_rectangleIntSize);
                    CircleInt t_circle = new CircleInt(m_otherCircleIntCenter, m_otherCircleIntRadius);

                    bool t_hasOverlap = CustomPhysics2DInt.CircleRectangleOverlap(t_circle, t_rectangle, out List<Vector2> t_intersectPoints) > 0;

                    Color t_color = t_hasOverlap ? Color.red : Color.green;
                    t_rectangle.DrawOutlineDebug(t_color);
                    t_circle.DrawDebug(Color.white);
                    
                    foreach (Vector2 t_point in t_intersectPoints)
                    {
                        CustomDebug.DrawCrossHair(t_point, 0.1f, Color.blue, true);
                        CustomDebug.DrawCircle(t_point, 0.1f, 6, Color.blue, true);
                    }

                    break;
                }
                case eTestChoice.LineRectangleInt:
                {
                    m_internalIntTestPosAsFloat += t_moveAm;

                    Vector2Int t_rectangleBotLeft = CustomPhysics2DInt.ConvertFloatPositionToIntPosition(m_internalIntTestPosAsFloat);
                    RectangleInt t_rectangle = new RectangleInt(t_rectangleBotLeft, m_rectangleIntSize);
                    LineInt t_line = new LineInt(m_lineIntPos1, m_lineIntPos2);

                    bool t_hasOverlap = CustomPhysics2DInt.LineRectangleOverlap(t_line, t_rectangle, out List<Vector2> t_intersectionPoints) > 0;
                    bool t_hasOverlapSanityCheck = CustomPhysics2DInt.LineRectangleOverlap(t_line, t_rectangle);

                    Color t_color = t_hasOverlap ? Color.red : Color.green;
                    t_rectangle.DrawOutlineDebug(t_color);
                    t_line.DrawDebug(Color.white);

                    foreach (Vector2 t_point in t_intersectionPoints)
                    {
                        CustomDebug.DrawCrossHair(t_point, 0.1f, Color.blue, true);
                        CustomDebug.DrawCircle(t_point, 0.1f, 6, Color.blue, true);
                    }

                    if (t_hasOverlapSanityCheck != t_hasOverlap)
                    {
                        //CustomDebug.LogError($"LINE RECTANGLE OVERLAP FUNCTIONS RETURNING DIFFERENT VALUES");
                    }

                    break;
                }
                case eTestChoice.PointRectangleInt:
                {
                    m_internalIntTestPosAsFloat += t_moveAm;

                    Vector2Int t_point = CustomPhysics2DInt.ConvertFloatPositionToIntPosition(m_internalIntTestPosAsFloat);
                    RectangleInt t_rectangle = new RectangleInt(m_otherRectangleIntBotLeftPos, m_otherRectangleIntSize);

                    bool t_hasOverlap = CustomPhysics2DInt.IsPointInRectangle(t_rectangle, t_point);

                    Color t_color = t_hasOverlap ? Color.red : Color.green;
                    t_rectangle.DrawOutlineDebug(t_color);

                    Vector2 t_debugPoint = CustomPhysics2DInt.ConvertIntPositionToFloatPosition(t_point);
                    CustomDebug.DrawCrossHair(t_debugPoint, 0.1f, Color.blue, true);
                    CustomDebug.DrawCircle(t_debugPoint, 0.1f, 6, Color.blue, true);

                    break;
                }
                case eTestChoice.RectangleRectangleInt:
                {
                    m_internalIntTestPosAsFloat += t_moveAm;

                    Vector2Int t_rectangleBotLeft = CustomPhysics2DInt.ConvertFloatPositionToIntPosition(m_internalIntTestPosAsFloat);
                    RectangleInt t_rectangle = new RectangleInt(t_rectangleBotLeft, m_rectangleIntSize);
                    RectangleInt t_otherRectangle = new RectangleInt(m_otherRectangleIntBotLeftPos, m_otherRectangleIntSize);

                    bool t_hasOverlap = CustomPhysics2DInt.RectangleRectangleOverlap(t_rectangle, t_otherRectangle, out List<Vector2> t_intersectionPoints) > 0;

                    Color t_color = t_hasOverlap ? Color.red : Color.green;
                    t_rectangle.DrawOutlineDebug(t_color);
                    t_otherRectangle.DrawOutlineDebug(Color.white);

                    foreach (Vector2 t_point in t_intersectionPoints)
                    {
                        CustomDebug.DrawCrossHair(t_point, 0.1f, Color.blue, true);
                        CustomDebug.DrawCircle(t_point, 0.1f, 6, Color.blue, true);
                    }

                    break;
                }
                case eTestChoice.RectangleCastRectangleInt:
                {
                    m_internalIntTestPosAsFloat += t_moveAm;

                    Vector2Int t_rectangleBotLeft = CustomPhysics2DInt.ConvertFloatPositionToIntPosition(m_internalIntTestPosAsFloat);
                    RectangleInt t_rectangle = new RectangleInt(t_rectangleBotLeft, m_rectangleIntSize);
                    RectangleInt t_otherRectangle = new RectangleInt(m_otherRectangleIntBotLeftPos, m_otherRectangleIntSize);

                    Vector2 t_rectBotLeftFloat = CustomPhysics2DInt.ConvertIntPositionToFloatPosition(t_rectangleBotLeft);
                    Vector2Int t_castEndRectBotLeftInt = CustomPhysics2DInt.ConvertFloatPositionToIntPosition(t_rectBotLeftFloat + m_castDir * m_castDistance);
                    RectangleInt t_castEndRect = new RectangleInt(t_castEndRectBotLeftInt, m_rectangleIntSize);

                    bool t_doesCastHit = CustomPhysics2DInt.RectangleCastToRectangle(t_rectangle, m_castDir, m_castDistance, t_otherRectangle, out float t_hitDist, out Vector2 t_hitPointOnRect, out Vector2 t_hitPointOnOther, out int t_hitEdgeIndex);

                    Color t_color = t_doesCastHit ? Color.red : Color.green;
                    t_rectangle.DrawOutlineDebug(t_color);
                    t_otherRectangle.DrawOutlineDebug(Color.white);

                    t_castEndRect.DrawOutlineDebug(t_color * 0.5f);
                    for (int i = 0; i < 4; ++i)
                    {
                        CustomDebug.DrawLine(CustomPhysics2DInt.ConvertIntPositionToFloatPosition(t_rectangle.points[i]), CustomPhysics2DInt.ConvertIntPositionToFloatPosition(t_castEndRect.points[i]), t_color * 0.5f, true);
                    }

                    if (t_doesCastHit)
                    {
                        CustomDebug.DrawCrossHair(t_hitPointOnRect, 0.1f, Color.blue, true);
                        CustomDebug.DrawCircle(t_hitPointOnRect, 0.1f, 6, Color.blue, true);
                        CustomDebug.DrawCrossHair(t_hitPointOnOther, 0.1f, Color.blue, true);
                        CustomDebug.DrawCircle(t_hitPointOnOther, 0.1f, 6, Color.blue, true);

                        t_otherRectangle.edges[t_hitEdgeIndex].DrawDebug(Color.magenta, true, false);
                    }
                    break;
                }
                case eTestChoice.BoundingCircleCircleCastInt:
                {
                    m_internalIntTestPosAsFloat += t_moveAm;

                    m_circleIntCenter = CustomPhysics2DInt.ConvertFloatPositionToIntPosition(m_internalIntTestPosAsFloat);
                    CircleInt t_circle = new CircleInt(m_circleIntCenter, m_circleIntRadius);

                    Vector2 t_circleCenterFloat = CustomPhysics2DInt.ConvertIntPositionToFloatPosition(m_circleIntCenter);
                    Vector2 t_castEndPosFloat = t_circleCenterFloat + m_castDir * m_castDistance;
                    Vector2Int t_castEndPos = CustomPhysics2DInt.ConvertFloatPositionToIntPosition(t_castEndPosFloat);

                    CircleInt t_endCastCircle = new CircleInt(t_castEndPos, m_circleIntRadius);

                    LineInt t_ray = new LineInt(m_circleIntCenter, t_castEndPos);
                   
                    CircleInt t_boundingCircle = CustomPhysics2DInt.CreateMinimumBoundingCircleForCircleCast(t_circle, m_castDir, m_castDistance);

                    t_circle.DrawDebug(Color.green, true);
                    t_ray.DrawDebug(Color.green * 0.5f, true, false);
                    t_endCastCircle.DrawDebug(Color.green * 0.5f, true);
                    t_boundingCircle.DrawDebug(Color.white, true);

                    break;
                }
                case eTestChoice.BoundingCircleRectangleCastInt:
                {
                    m_internalIntTestPosAsFloat += t_moveAm;

                    Vector2Int t_rectangleBotLeftPos = CustomPhysics2DInt.ConvertFloatPositionToIntPosition(m_internalIntTestPosAsFloat);
                    RectangleInt t_rectangle = new RectangleInt(t_rectangleBotLeftPos, m_rectangleIntSize);

                    Vector2 t_rectBotLeftFloat = t_rectangle.GetBotLeftPointAsFloatPosition();
                    Vector2 t_castEndBotLeftPosFloat = t_rectBotLeftFloat + m_castDir * m_castDistance;
                    Vector2Int t_castEndBotLeftPos = CustomPhysics2DInt.ConvertFloatPositionToIntPosition(t_castEndBotLeftPosFloat);

                    RectangleInt t_endCastRectangle = new RectangleInt(t_castEndBotLeftPos, m_rectangleIntSize);

                    LineInt t_ray = new LineInt(t_rectangle.center, t_endCastRectangle.center);

                    CircleInt t_boundingCircle = CustomPhysics2DInt.CreateMinimumBoundingCircleForRectangleCast(t_rectangle, m_castDir, m_castDistance);

                    t_rectangle.DrawOutlineDebug(Color.green, true);
                    t_ray.DrawDebug(Color.green * 0.5f, true, false);
                    t_endCastRectangle.DrawOutlineDebug(Color.green * 0.5f, true);
                    t_boundingCircle.DrawDebug(Color.white, true);

                    break;
                }
                case eTestChoice.RectangleCompletelyInsideRectangle:
                {
                    m_internalIntTestPosAsFloat += t_moveAm;

                    Vector2Int t_rectangleBotLeftPos = CustomPhysics2DInt.ConvertFloatPositionToIntPosition(m_internalIntTestPosAsFloat);
                    RectangleInt t_rectangle = new RectangleInt(t_rectangleBotLeftPos, m_rectangleIntSize);

                    RectangleInt t_otherRect = new RectangleInt(m_otherRectangleIntBotLeftPos, m_otherRectangleIntSize);

                    bool t_isInside = CustomPhysics2DInt.DoesRectangleEncapsulateRectangle(t_otherRect, t_rectangle);

                    t_rectangle.DrawOutlineDebug(t_isInside ? Color.red : Color.green, true);
                    t_otherRect.DrawOutlineDebug(Color.white, true);

                    break;
                }
            }
        }


        private bool ShouldShowCircleInfo()
        {
            switch (m_test)
            {
                case eTestChoice.CircleLine: return true;
                case eTestChoice.CircleCircle: return true;
                case eTestChoice.CircleRectangle: return true;
                case eTestChoice.CircleCastRectangle: return true;
                case eTestChoice.CircleCastLine: return true;
                case eTestChoice.BoundingCircleCircleCast: return true;
                case eTestChoice.BoundingCircleCircleCircle: return true;
                case eTestChoice.CircleCompletelyInsideCircle: return true;
                default: return false;
            }
        }
        private bool ShouldShowCapsuleInfo()
        {
            switch (m_test)
            {
                case eTestChoice.CapsuleLine: return true;
                case eTestChoice.CapsuleRectangle: return true;
                default: return false;
            }
        }
        private bool ShouldShowLineInfo()
        {
            switch (m_test)
            {
                case eTestChoice.CircleLine: return true;
                case eTestChoice.CapsuleLine: return true;
                case eTestChoice.InfiniteLines: return true;
                default: return false;
            }
        }
        private bool ShouldShowOtherLineInfo()
        {
            switch (m_test)
            {
                case eTestChoice.InfiniteLines: return true;
                case eTestChoice.CircleCastLine: return true;
                case eTestChoice.RectangleCastLine: return true;
                default: return false;
            }
        }
        private bool ShouldShowOtherCircleInfo()
        {
            switch (m_test)
            {
                case eTestChoice.CircleCircle: return true;
                case eTestChoice.BoundingCircleCircleCircle: return true;
                case eTestChoice.CircleCompletelyInsideCircle: return true;
                default: return false;
            }
        }
        private bool ShouldShowRectangleInfo()
        {
            switch (m_test)
            {
                case eTestChoice.RectangleRectangle: return true;
                case eTestChoice.RectangleCastLine: return true;
                case eTestChoice.RectangleCastRectangle: return true;
                default: return false;
            }
        }
        private bool ShouldShowOtherRectangleInfo()
        {
            switch (m_test)
            {
                case eTestChoice.RectangleRectangle: return true;
                case eTestChoice.CapsuleRectangle: return true;
                case eTestChoice.CircleRectangle: return true;
                case eTestChoice.CircleCastRectangle: return true;
                case eTestChoice.RectangleCastRectangle: return true;
                default: return false;
            }
        }
        private bool ShouldShowCastInfo()
        {
            switch (m_test)
            {
                case eTestChoice.CircleCastRectangle: return true;
                case eTestChoice.CircleCastLine: return true;
                case eTestChoice.RectangleCastLine: return true;
                case eTestChoice.RectangleCastRectangle: return true;
                case eTestChoice.BoundingCircleCircleCast: return true;
                case eTestChoice.RectangleCastRectangleInt: return true;
                case eTestChoice.BoundingCircleCircleCastInt: return true;
                case eTestChoice.BoundingCircleRectangleCastInt: return true;
                default: return false;
            }
        }
        private bool ShouldShowCircleIntInfo()
        {
            switch (m_test)
            {
                case eTestChoice.CircleCircleInt: return true;
                case eTestChoice.CircleLineInt: return true;
                case eTestChoice.BoundingCircleCircleCastInt: return true;
                default: return false;
            }
        }
        private bool ShouldShowOtherCircleIntInfo()
        {
            switch (m_test)
            {
                case eTestChoice.CircleCircleInt: return true;
                case eTestChoice.CircleRectangleInt: return true;
                default: return false;
            }
        }
        private bool ShouldShowLineIntInfo()
        {
            switch (m_test)
            {
                case eTestChoice.CircleLineInt: return true;
                case eTestChoice.LineLineInt: return true;
                case eTestChoice.PointOnLineInt: return true;
                case eTestChoice.LineRectangleInt: return true;
                default: return false;
            }
        }
        private bool ShouldShowOtherLineIntInfo()
        {
            switch (m_test)
            {
                case eTestChoice.LineLineInt: return true;
                default: return false;
            }
        }
        private bool ShouldShowPointIntInfo()
        {
            switch (m_test)
            {
                case eTestChoice.PointOnLineInt: return true;
                case eTestChoice.PointRectangleInt: return true;
                default: return false;
            }
        }
        private bool ShouldShowRectangleIntInfo()
        {
            switch (m_test)
            {
                case eTestChoice.CircleRectangleInt: return true;
                case eTestChoice.LineRectangleInt: return true;
                case eTestChoice.RectangleRectangleInt: return true;
                case eTestChoice.RectangleCastRectangleInt: return true;
                case eTestChoice.BoundingCircleRectangleCastInt: return true;
                case eTestChoice.RectangleCompletelyInsideRectangle: return true;
                default: return false;
            }
        }
        private bool ShouldShowOtherRectangleIntInfo()
        {
            switch (m_test)
            {
                case eTestChoice.PointRectangleInt: return true;
                case eTestChoice.RectangleRectangleInt: return true;
                case eTestChoice.RectangleCastRectangleInt: return true;
                case eTestChoice.RectangleCompletelyInsideRectangle: return true;
                default: return false;
            }
        }

        public enum eTestChoice { CircleLine, CapsuleLine, InfiniteLines, CircleCircle, RectangleRectangle, CapsuleRectangle, CircleRectangle, CircleCastRectangle, CircleCastLine, RectangleCastLine, RectangleCastRectangle, BoundingCircleCircleCircle, BoundingCircleCircleCast, CircleCompletelyInsideCircle, CircleCircleInt, CircleLineInt, LineLineInt, PointOnLineInt, CircleRectangleInt, LineRectangleInt, PointRectangleInt, RectangleRectangleInt, RectangleCastRectangleInt, BoundingCircleCircleCastInt, BoundingCircleRectangleCastInt, RectangleCompletelyInsideRectangle }
    }
}