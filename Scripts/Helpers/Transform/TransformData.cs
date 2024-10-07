using System;
using UnityEngine;

using Helpers.UnityEnums;
using UnityEngine.UIElements;
using Helpers.Math;
// Original Authors - Wyatt Senalik

namespace Helpers.Transforms
{
    /// <summary>
    /// Data held in a transform (Position, Rotation, and Scale).
    /// </summary>
    [Serializable]
    public class TransformData : IEquatable<TransformData>
    {
        private readonly float[] m_position;
        private readonly float[] m_rotation;
        private readonly float[] m_scale;
        private readonly float m_angle;

        public Vector3 position => new Vector3(m_position[0], m_position[1],
            0.0f);
        public float angle => m_angle;
        public Vector3 scale => new Vector3(m_scale[0], m_scale[1], 1.0f);
        public Vector2 position2D => new Vector2(m_position[0], m_position[1]);
        public Vector2 scale2D => new Vector2(m_scale[0], m_scale[1]);


        public TransformData()
        {
            m_position = new float[2] { 0.0f, 0.0f };
            m_angle = 0.0f;
            m_scale = new float[2] { 1.0f, 1.0f };
        }
        public TransformData(Vector3 pos, Quaternion rot, Vector3 size) : this(pos, rot.eulerAngles.z, size) { }
        public TransformData(Vector2 pos, float angleDegrees, Vector2 size)
        {
            m_position = new float[2] { pos.x, pos.y };
            m_angle = AngleHelpers.RestrictAngle(angleDegrees);
            m_scale = new float[2] { size.x, size.y };
        }
        public TransformData(Transform transform, bool isWorldSpace = true)
        {
            Vector2 t_pos = isWorldSpace ? transform.position :
                transform.localPosition;
            Quaternion t_rot = isWorldSpace ? transform.rotation :
                transform.localRotation;
            Vector2 t_size = transform.localScale;

            m_position = new float[2] { t_pos.x, t_pos.y };
            m_angle = AngleHelpers.RestrictAngle(t_rot.eulerAngles.z);
            m_scale = new float[2] { t_size.x, t_size.y };
        }


        /// <summary>
        /// Creates <see cref="TransformData"/> holding position, rotation,
        /// and localScale.
        /// </summary>
        /// <param name="transform">Transform to create the data from.</param>
        public static TransformData CreateGlobalTransformData(Transform transform)
        {
            return new TransformData(transform.position, transform.rotation,
                transform.localScale);
        }
        /// <summary>
        /// Creates <see cref="TransformData"/> holding localPosition,
        /// localRotation, and localScale.
        /// </summary>
        /// <param name="transform">Transform to create the data from.</param>
        public static TransformData CreateLocalTransformData(Transform transform)
        {
            return new TransformData(transform.localPosition,
                transform.localRotation, transform.localScale);
        }


        /// <summary>
        /// Applies the <see cref="TransformData"/>'s position, rotation,
        /// and scale to the given <see cref="Transform"/>'s position, rotation,
        /// and localScale respectively.
        /// </summary>
        /// <param name="transform">Transform to apply
        /// the <see cref="TransformData"/> to.</param>
        public void ApplyGlobal(Transform transform)
        {
            transform.position = position;
            transform.rotation = Quaternion.Euler(0.0f, 0.0f, angle);
            transform.localScale = scale;
        }
        /// <summary>
        /// Applies the <see cref="TransformData"/>'s position, rotation,
        /// and scale to the given <see cref="Transform"/>'s localPosition,
        /// localRotation, and localScale respectively.
        /// </summary>
        /// <param name="transform">Transform to apply
        /// the <see cref="TransformData"/> to.</param>
        public void ApplyLocal(Transform transform)
        {
            transform.localPosition = position;
            transform.localRotation = Quaternion.Euler(0.0f, 0.0f, angle);
            transform.localScale = scale;
        }
        /// <summary>
        /// Applies <see cref="TransformData"/> either locally or globally.
        /// </summary>
        public void Apply(Transform transform, eRelativeSpace relativeSpace)
        {
            switch (relativeSpace)
            {
                case eRelativeSpace.Local:
                    ApplyLocal(transform);
                    break;
                case eRelativeSpace.World:
                    ApplyGlobal(transform);
                    break;
                default:
                    CustomDebug.UnhandledEnum(relativeSpace, GetType().Name);
                    break;
            }
        }
        public void CalculateDistances(TransformData other, out float xPosDiff, out float yPosDiff, out float angleDiff, out float xScaleDiff, out float yScaleDiff)
        {
            xPosDiff = Mathf.Abs(position2D.x - other.position2D.x);
            yPosDiff = Mathf.Abs(position2D.y - other.position2D.y);

            angleDiff = Mathf.Abs(angle - other.angle);
            while (angleDiff > 180.0f)
            {
                angleDiff -= 360.0f;
            }
            angleDiff = Mathf.Abs(angleDiff);

            xScaleDiff = Mathf.Abs(scale2D.x - other.scale2D.x);
            yScaleDiff = Mathf.Abs(scale2D.y - other.scale2D.y);
        }
        public float CalculateMaxDistance(TransformData other)
        {
            CalculateDistances(other, out float t_xPosDiff, out float t_yPosDiff, out float t_angleDiff, out float t_xScaleDiff, out float t_yScaleDiff);
            return Mathf.Max(t_xPosDiff, t_yPosDiff, t_angleDiff, t_xScaleDiff, t_yScaleDiff);
        }
        /// <summary>
        /// Compares the given <see cref="TransformData"/> to this
        /// <see cref="TransformData"/> and returns false if any changes
        /// are outside the specified tolerance.
        /// If all changes fall within the tolerance, returns true.
        /// </summary>
        /// <param name="data">Other <see cref="TransformData"/>
        /// to compare this against.</param>
        /// <param name="positionTolerance">Amount of changes to allow without
        /// returning true. Checks this against x, y, and z separately.</param>
        /// <param name="rotationTolerance">Amount of changes to allow without
        /// returning true. Checks this against x, y, and z separately.</param>
        /// <param name="scaleTolerance">Amount of changes to allow without
        /// returning true. Checks this against x, y, and z separately.</param>
        /// <returns>If the <see cref="TransformData"/>s are close enough
        /// to one another to be considered the same.</returns>
        public bool Compare(TransformData data, float positionTolerance = 0.05f,
            float rotationTolerance = 0.05f, float scaleTolerance = 0.05f)
        {
            // Position
            float t_xPosDiff = Mathf.Abs(position2D.x - data.position2D.x);
            if (t_xPosDiff > positionTolerance) { return false; }
            float t_yPosDiff = Mathf.Abs(position2D.y - data.position2D.y);
            if (t_yPosDiff > positionTolerance) { return false; }
            // Rotation
            float t_angleDiff = Mathf.Abs(angle - data.angle);
            while (t_angleDiff > 180.0f)
            {
                t_angleDiff -= 360.0f;
            }
            t_angleDiff = Mathf.Abs(t_angleDiff);
            if (t_angleDiff > rotationTolerance) { return false; }
            // Scale
            float t_xScaleDiff = Mathf.Abs(scale2D.x - data.scale2D.x);
            if (t_xScaleDiff > scaleTolerance) { return false; }
            float t_yScaleDiff = Mathf.Abs(scale2D.y - data.scale2D.y);
            if (t_yScaleDiff > scaleTolerance) { return false; }
            // Everything is within the tolerance
            return true;
        }

        public override string ToString()
        {
            return $"{GetType().Name} ({nameof(position2D)}=({position2D.x}, {position2D.y})) ({nameof(angle)}={angle}) ({nameof(scale2D)}=({scale2D.x}, {scale2D.y}))";
        }

        /// <summary>
        /// Linearly interpolates between the transform data.
        /// t must be within the range [0, 1].
        /// </summary>
        public static TransformData Lerp(TransformData a, TransformData b, float t)
        {
            Vector2 t_pos = Vector2.Lerp(a.position2D, b.position2D, t);
            float t_angle = AngleHelpers.RestrictAngle(Mathf.LerpAngle(a.angle, b.angle, t));
            Vector2 t_size = Vector2.Lerp(a.scale2D, b.scale2D, t);

            return new TransformData(t_pos, t_angle, t_size);
        }
        /// <summary>
        /// Linearly interpolates between the transform data. 
        /// </summary>
        public static TransformData LerpUnclamped(TransformData a, TransformData b, float t)
        {
            Vector2 t_pos = Vector2.LerpUnclamped(a.position2D, b.position2D, t);
            float t_angle = MathHelpers.LerpAngleUnclamped(a.angle, b.angle, t);
            Vector2 t_size = Vector2.LerpUnclamped(a.scale2D, b.scale2D, t);

            return new TransformData(t_pos, t_angle, t_size);
        }

        public bool Equals(TransformData other) => Compare(other, 0.001f, 0.01f, 0.001f);
    }
}
