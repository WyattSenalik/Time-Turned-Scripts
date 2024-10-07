using UnityEngine;
using UnityEngine.UIElements;

using NaughtyAttributes;

using Helpers.Math;

namespace Helpers.Physics.Custom2DInt
{
    [DisallowMultipleComponent]
    public sealed class Int2DTransform : MonoBehaviour
    {
        public int childCount => transform.childCount;
        public Vector2 forward
        {
            get => transform.forward;
            set => transform.forward = value;
        }
        public bool hasChanged
        {
            get => transform.hasChanged;
            set => transform.hasChanged = value;
        }
        public int hierarchyCapacity
        {
            get => transform.hierarchyCapacity;
            set => transform.hierarchyCapacity = value;
        }
        public int hierarchyCount => transform.hierarchyCount;
        public Matrix4x4 localToWorldMatrix => transform.localToWorldMatrix;
        public Int2DTransform parent
        {
            get
            {
                if (transform.parent == null)
                {
                    return null;
                }
                return transform.parent.GetComponent<Int2DTransform>();
            }
        }
        public Vector2 right => transform.right;
        public Int2DTransform root
        {
            get
            {
                if (transform.root == null)
                {
                    return null;
                }
                return transform.root.GetComponent<Int2DTransform>();
            }
        }
        public Vector2 up => transform.up;
        public Matrix4x4 worldToLocalMatrix => transform.worldToLocalMatrix;



        public Vector2Int localPosition
        {
            get => m_localPosition;
            set
            {
                m_localPosition = value;
                transform.position = CustomPhysics2DInt.ConvertIntPositionToFloatPosition(m_localPosition);
            }
        }
        [ShowNativeProperty]
        public Vector2 localPositionFloat
        {
            get => CustomPhysics2DInt.ConvertIntPositionToFloatPosition(localPosition);
            set => localPosition = CustomPhysics2DInt.ConvertFloatPositionToIntPosition(value);
        }
        [ShowNativeProperty]
        public Vector2Int position
        {
            get => CustomPhysics2DInt.ConvertFloatPositionToIntPosition(transform.position - transform.localPosition) + localPosition;
            set => localPosition = value - CustomPhysics2DInt.ConvertFloatPositionToIntPosition(transform.position - transform.localPosition);
        }
        [ShowNativeProperty]
        public Vector2 positionFloat
        {
            get => CustomPhysics2DInt.ConvertIntPositionToFloatPosition(position);
            set => position = CustomPhysics2DInt.ConvertFloatPositionToIntPosition(value);
        }

        /// <summary>Local rotation in degrees.</summary>
        public int localAngle
        {
            get => m_localAngle;
            set
            {
                m_localAngle = AngleHelpers.RestrictAngle(value);
                transform.localRotation = Quaternion.Euler(0.0f, 0.0f, m_localAngle);
            }
        }
        [ShowNativeProperty]
        /// <summary>World rotation in degrees.</summary>
        public int angle
        {
            get => Mathf.FloorToInt(transform.eulerAngles.z);
            set
            {
                int t_restrictedAngle = AngleHelpers.RestrictAngle(value);
                transform.rotation = Quaternion.Euler(0.0f, 0.0f, t_restrictedAngle);
                m_localAngle = Mathf.FloorToInt(transform.localEulerAngles.z);
            }
        }

        public Vector2Int localSize
        {
            get => m_localSize;
            set
            {
                m_localSize = value;
                transform.localScale = CustomPhysics2DInt.ConvertIntPositionToFloatPosition(m_localSize);
            }
        }
        [ShowNativeProperty]
        public Vector2 localSizeFloat
        {
            get => CustomPhysics2DInt.ConvertIntPositionToFloatPosition(localSize);
            set => localSize = CustomPhysics2DInt.ConvertFloatPositionToIntPosition(value);
        }
        [ShowNativeProperty]
        public Vector2Int lossySize
        {
            get
            {
                Vector2 t_nonLocalScale = Vector2.zero;
                if (transform.localScale.x != 0.0f)
                {
                    t_nonLocalScale.x = transform.lossyScale.x / transform.localScale.x;
                }
                if (transform.localScale.y != 0.0f)
                {
                    t_nonLocalScale.y = transform.lossyScale.y / transform.localScale.y;
                }
                Vector2 t_lossySizeFloat = t_nonLocalScale * localSize;
                return new Vector2Int(Mathf.CeilToInt(t_lossySizeFloat.x), Mathf.CeilToInt(t_lossySizeFloat.y));
            }
        }
        [ShowNativeProperty]
        public Vector2 lossySizeFloat
        {
            get => CustomPhysics2DInt.ConvertIntPositionToFloatPosition(lossySize);
        }


        [SerializeField] private Vector2Int m_localPosition = Vector2Int.zero;
        [SerializeField, Range(-179, 180)] private int m_localAngle = 0;
        [SerializeField] private Vector2Int m_localSize = Vector2Int.zero;


        private void Awake()
        {
            // Forcefully set the transform values if they are not already set.
            LoadValuesFromTransform();
        }
        private void Reset()
        {
            LoadValuesFromTransform();
        }

        [Button]
        private void ApplyIntTransformValues()
        {
            transform.localPosition = CustomPhysics2DInt.ConvertIntPositionToFloatPosition(m_localPosition);
            transform.localScale = CustomPhysics2DInt.ConvertIntPositionToFloatPosition(m_localSize);
            int t_restrictedAngle = AngleHelpers.RestrictAngle(m_localAngle);
            m_localAngle = t_restrictedAngle;
            transform.localRotation = Quaternion.Euler(0.0f, 0.0f, m_localAngle);
        }
        [Button]
        private void LoadValuesFromTransform()
        {
            m_localPosition = CustomPhysics2DInt.ConvertFloatPositionToIntPosition(transform.localPosition);
            m_localSize = CustomPhysics2DInt.ConvertFloatPositionToIntPosition(transform.localScale);
            m_localAngle = AngleHelpers.RestrictAngle(Mathf.FloorToInt(transform.localEulerAngles.z));

            ApplyIntTransformValues();
        }

        private void OnDrawGizmosSelected()
        {
            if (Application.isPlaying)
            {
                ApplyIntTransformValues();
            } 
        }
    }
}