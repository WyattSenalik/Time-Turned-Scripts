using UnityEngine;
// Original Authors - Wyatt Senalik

namespace Helpers.UnityInterfaces
{
    /// <summary>
    /// Wrapper that gives a <see cref="Transform"/> the ability to utilize
    /// <see cref="ITransform"/> by newing this class up.
    /// </summary>
    /// </summary>
    public class FakeTransform : FakeComponent, ITransform
    {
        public int childCount => transform.childCount;
        public Vector3 eulerAngles
        {
            get => transform.eulerAngles;
            set => transform.eulerAngles = value;
        }
        public Vector3 forward
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
        public int hierarchyCount => m_transform.hierarchyCount;
        public Vector3 localEulerAngles
        {
            get => transform.localEulerAngles;
            set => transform.localEulerAngles = value;
        }
        public Vector3 localPosition
        {
            get => transform.localPosition;
            set => transform.localPosition = value;
        }
        public Quaternion localRotation
        {
            get => transform.localRotation;
            set => transform.localRotation = value;
        }
        public Vector3 localScale
        {
            get => transform.localScale;
            set => transform.localScale = value;
        }
        public Matrix4x4 localToWorldMatrix => transform.localToWorldMatrix;
        public Vector3 lossyScale => transform.lossyScale;
        public ITransform parent
        {
            get => transform.parent.ToITransform();
            set => transform.parent = value != null ? value.transform : null;
        }
        public Vector3 position
        {
            get => transform.position;
            set => transform.position = value;
        }
        public Vector3 right
        {
            get => transform.right;
            set => transform.right = value;
        }
        public ITransform root => transform.root.ToITransform();
        public Quaternion rotation
        {
            get => transform.rotation;
            set => transform.rotation = value;
        }
        public Vector3 up
        {
            get => transform.up;
            set => transform.up = up;
        }
        public Matrix4x4 worldToLocalMatrix => transform.worldToLocalMatrix;


        // Reference to the actual Transform
        private Transform m_transform = null;


        public FakeTransform(Transform trans) : base(trans)
        {
            m_transform = trans;
        }


        public void DetachChildren() => transform.DetachChildren();
        public ITransform Find(string n) => transform.Find(n).ToITransform();
        public ITransform GetChild(int index)
            => transform.GetChild(index).ToITransform();
        public int GetSiblingIndex() => transform.GetSiblingIndex();
        public Vector3 InverseTransformDirection(Vector3 direction)
            => transform.InverseTransformDirection(direction);
        public Vector3 InverseTransformDirection(float x, float y, float z)
            => transform.InverseTransformDirection(x, y, z);
        public Vector3 InverseTransformPoint(Vector3 position)
            => transform.InverseTransformPoint(position);
        public Vector3 InverseTransformPoint(float x, float y, float z)
            => transform.InverseTransformPoint(x, y, z);
        public Vector3 InverseTransformVector(Vector3 vector)
            => transform.InverseTransformVector(vector);
        public Vector3 InverseTransformVector(float x, float y, float z)
            => transform.InverseTransformVector(x, y, z);
        public bool IsChildOf(Transform parent) => transform.IsChildOf(parent);
        public void LookAt(Transform target) => transform.LookAt(target);
        public void LookAt(Transform target, Vector3 worldUp)
            => transform.LookAt(target, worldUp);
        public void LookAt(Vector3 worldPosition)
            => transform.LookAt(worldPosition);
        public void LookAt(Vector3 worldPosition, Vector3 worldUp)
            => transform.LookAt(worldPosition, worldUp);
        public void Rotate(Vector3 eulers, Space relativeTo = Space.Self)
            => transform.Rotate(eulers, relativeTo);
        public void Rotate(float xAngle, float yAngle, float zAngle,
            Space relativeTo = Space.Self)
            => transform.Rotate(xAngle, yAngle, zAngle, relativeTo);
        public void Rotate(Vector3 axis, float angle, Space relativeTo = Space.Self)
            => transform.Rotate(axis, angle, relativeTo);
        public void Rotate(Vector3 eulers) => transform.Rotate(eulers);
        public void Rotate(Vector3 axis, float angle)
            => transform.Rotate(axis, angle);
        public void RotateAround(Vector3 point, Vector3 axis, float angle)
            => transform.RotateAround(point, axis, angle);
        public void SetAsFirstSibling() => transform.SetAsFirstSibling();
        public void SetAsLastSibling() => transform.SetAsLastSibling();
        public void SetParent(Transform parent, bool worldPositionStays = true)
            => transform.SetParent(parent, worldPositionStays);
        public void SetPositionAndRotation(Vector3 position, Quaternion rotation)
            => transform.SetPositionAndRotation(position, rotation);
        public void SetSiblingIndex(int index) => transform.SetSiblingIndex(index);
        public Vector3 TransformDirection(Vector3 direction)
            => transform.TransformDirection(direction);
        public Vector3 TransformDirection(float x, float y, float z)
            => transform.TransformDirection(x, y, z);
        public Vector3 TransformPoint(Vector3 position)
            => transform.TransformPoint(position);
        public Vector3 TransformPoint(float x, float y, float z)
            => transform.TransformPoint(x, y, z);
        public Vector3 TransformVector(Vector3 vector)
            => transform.TransformVector(vector);
        public Vector3 TransformVector(float x, float y, float z)
            => transform.TransformVector(x, y, z);
        public void Translate(Vector3 translation, Space relativeTo = Space.Self)
            => transform.Translate(translation, relativeTo);
        public void Translate(float x, float y, float z,
            Space relativeTo = Space.Self)
            => transform.Translate(x, y, z, relativeTo);
        public void Translate(Vector3 translation, Transform relativeTo)
             => transform.Translate(translation, relativeTo);
        public void Translate(float x, float y, float z, Transform relativeTo)
             => transform.Translate(x, y, z, relativeTo);
    }

    public static class TransformExtensionsForFakeTransform
    {
        /// <summary>
        /// Finds the <see cref="ITransform"/> driving this <see cref="Transform"/>
        /// and returns it. If no <see cref="ITransform"/> is driving this
        /// <see cref="Transform"/>, then news up a <see cref="FakeTransform"/> and
        /// returns that.
        /// </summary>
        public static ITransform ToITransform(this Transform transform)
        {
            if (transform == null) { return null; }

            // If an ITransform exists on the transform, return that.
            // (There should only ever be 1 ITransform on an object).
            if (transform.TryGetComponent(out ITransform trans))
            {
                return trans;
            }
            // Otherwise, just return a fake version of it.
            return new FakeTransform(transform);
        }
    }
}
