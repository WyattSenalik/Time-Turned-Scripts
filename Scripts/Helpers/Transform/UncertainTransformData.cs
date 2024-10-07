using System;
using UnityEngine;
// Original Authors - Wyatt Senalik

namespace Helpers.Transforms
{
    /// <summary>
    /// Made stuff serializable using floats and bools instead of
    /// simply using Vector3? and Quaternion?
    /// </summary>
    [Serializable]
    public class UncertainTransformData
    {
        private float[] m_position = new float[3];
        private bool m_hasPosition = false;

        private float[] m_rotation = new float[4];
        private bool m_hasRotation = false;

        private float[] m_scale = new float[3];
        private bool m_hasScale = false;

        public Vector3? position => m_hasPosition ?
            new Vector3?(FloatArrToVector3(m_position)) : null;
        public Quaternion? rotation => m_hasRotation ?
            new Quaternion?(FloatArrToQuaternion(m_rotation)) : null;
        public Vector3? scale => m_hasScale ?
            new Vector3?(FloatArrToVector3(m_scale)) : null;


        public UncertainTransformData(Vector3? pos, Quaternion? rot, Vector3? size)
        {
            m_hasPosition = pos.HasValue;
            if (m_hasPosition)
            {
                m_position = new float[3];
                m_position[0] = pos.Value.x;
                m_position[1] = pos.Value.y;
                m_position[2] = pos.Value.z;
            }

            m_hasRotation = rot.HasValue;
            if (m_hasRotation)
            {
                m_rotation = new float[4];
                m_rotation[0] = rot.Value.x;
                m_rotation[1] = rot.Value.y;
                m_rotation[2] = rot.Value.z;
                m_rotation[3] = rot.Value.w;
            }

            m_hasScale = size.HasValue;
            if (m_hasScale)
            {
                m_scale = new float[3];
                m_scale[0] = size.Value.x;
                m_scale[1] = size.Value.y;
                m_scale[2] = size.Value.z;
            }
        }

        public override string ToString()
        {
            return $"(Position={position}; Rotation={rotation}; Scale={scale})";
        }

        private static Vector3 FloatArrToVector3(float[] floatArray)
        {
            //CustomDebug.AssertIndexIsInRange(2, floatArray, nameof(UncertainTransformData));

            return new Vector3(floatArray[0], floatArray[1], floatArray[2]);
        }
        private static Quaternion FloatArrToQuaternion(float[] floatArray)
        {
            //CustomDebug.AssertIndexIsInRange(3, floatArray, nameof(UncertainTransformData));

            return new Quaternion(floatArray[0], floatArray[1], floatArray[2],
                floatArray[3]);
        }
    }
}
