using System;
using UnityEngine;

using Helpers.UnityInterfaces;
// Original Authors - Wyatt Senalik

namespace Helpers.Transforms
{
    /// <summary>
    /// Takes control of a transform and invokes events when the
    /// transform is changed. Transform must be updated through this
    /// class for changes to be seen.
    /// </summary>
    [DisallowMultipleComponent]
    public class DirtyTransform : MonoBehaviour, ITransform
    {
        #region ITransform Props
        public Vector3 position
        {
            get => transform.position;
            set
            {
                Vector3 worldPos = value;
                Vector3 localPos = worldToLocalMatrix * worldPos;

                localPosition = localPos;
            }
        }
        public Quaternion rotation
        {
            get => transform.rotation;
            set
            {
                Quaternion oldRot = transform.rotation;
                Quaternion worldRot = value;
                transform.rotation = worldRot;
                Quaternion localRot = transform.localRotation;
                transform.rotation = oldRot;

                localRotation = localRot;
            }
        }
        public Vector3 eulerAngles
        {
            get => transform.eulerAngles;
            set => rotation = Quaternion.Euler(value);
        }

        public Vector3 localPosition
        {
            get => transform.localPosition;
            set => ChangeLocalPosition(value);
        }
        public Quaternion localRotation
        {
            get => transform.localRotation;
            set => ChangeLocalRotation(value);
        }
        public Vector3 localEulerAngles
        {
            get => transform.localEulerAngles;
            set => localRotation = Quaternion.Euler(value);
        }
        public Vector3 localScale
        {
            get => transform.localScale;
            set => ChangeLocalScale(value);
        }

        public Vector3 forward
        {
            get => transform.forward;
            set
            {
                Vector3 oldForward = transform.forward;
                transform.forward = value;
                Quaternion worldRot = transform.rotation;
                transform.forward = oldForward;

                rotation = worldRot;
            }
        }
        public Vector3 right
        {
            get => transform.right;
            set
            {
                Vector3 oldRight = transform.right;
                transform.right = value;
                Quaternion worldRot = transform.rotation;
                transform.right = oldRight;

                rotation = worldRot;
            }
        }
        public Vector3 up
        {
            get => transform.up;
            set
            {
                Vector3 oldUp = transform.up;
                transform.up = value;
                Quaternion worldRot = transform.rotation;
                transform.up = oldUp;

                rotation = worldRot;
            }
        }

        public ITransform parent
        {
            get
            {
                // Internal value exists.
                if (m_parent != null) { return m_parent; }
                // Internal value is null and the actual parent is also null.
                if (transform.parent == null) { return null; }

                // Internal value is null but the actual parent is not.
                m_parent = transform.parent.ToITransform();
                return m_parent;
            }
            set => ChangeParent(value);
        }

        public ITransform root => transform.root.ToITransform();

        #region Unchanged (ITransform Props)
        public int childCount => transform.childCount;
        [Obsolete]
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
        public Vector3 lossyScale => transform.lossyScale;
        public Matrix4x4 worldToLocalMatrix => transform.worldToLocalMatrix;
        #endregion Unchanged (ITransform Props)
        #endregion ITransform Props

        // Parameter events
        public event Action<Vector3, Vector3> onLocalPositionChanged;
        public event Action<Quaternion, Quaternion> onLocalRotationChanged;
        public event Action<Vector3, Vector3> onLocalScaleChanged;
        public event Action<ITransform, ITransform> onParentChanged;
        public event Action<int, int> onSiblingIndexChanged;

        public event Action onLocalPositionChangedNoParam;
        public event Action onLocalRotationChangedNoParam;
        public event Action onLocalScaleChangedNoParam;
        public event Action onParentChangedNoParam;
        public event Action onSiblingIndexChangedNoParam;


        private ITransform m_parent = null;


        #region ITransform Funcs
        public void SetSiblingIndex(int index) => ChangeSiblingIndex(index);

        public void DetachChildren()
        {
            while (childCount > 0)
            {
                ITransform child = GetChild(0);
                child.parent = null;
            }
        }
        public ITransform Find(string n) => transform.Find(n).ToITransform();
        public ITransform GetChild(int index)
            => transform.GetChild(index).ToITransform();
        public void LookAt(Transform target) => LookAt(target, Vector3.up);
        public void LookAt(Transform target, Vector3 worldUp)
            => LookAt(target.position, worldUp);
        public void LookAt(Vector3 worldPosition)
            => LookAt(worldPosition, Vector3.up);
        public void LookAt(Vector3 worldPosition, Vector3 worldUp)
        {
            CauseGenericChange(() =>
            {
                transform.LookAt(worldPosition, worldUp);
            });
        }
        public void Rotate(Vector3 eulers, Space relativeTo = Space.Self)
        {
            CauseGenericChange(() =>
            {
                transform.Rotate(eulers, relativeTo);
            });
        }
        public void Rotate(float xAngle, float yAngle, float zAngle,
            Space relativeTo = Space.Self)
            => Rotate(new Vector3(xAngle, yAngle, zAngle), relativeTo);
        public void Rotate(Vector3 axis, float angle, Space relativeTo = Space.Self)
        {
            CauseGenericChange(() =>
            {
                transform.Rotate(axis, angle, relativeTo);
            });
        }
        public void Rotate(Vector3 eulers) => Rotate(eulers, Space.Self);
        public void Rotate(Vector3 axis, float angle)
            => Rotate(axis, angle, Space.Self);
        public void RotateAround(Vector3 point, Vector3 axis, float angle)
        {
            CauseGenericChange(() =>
            {
                transform.RotateAround(point, axis, angle);
            });
        }
        public void SetAsFirstSibling() => SetSiblingIndex(0);
        public void SetAsLastSibling()
        {
            if (parent == null) { SetSiblingIndex(0); return; }
            SetSiblingIndex(parent.childCount - 1);
        }
        public void SetParent(Transform parent, bool worldPositionStays = true)
        {
            CauseGenericChange(() =>
            {
                transform.SetParent(parent, worldPositionStays);
            });
        }
        public void SetPositionAndRotation(Vector3 position, Quaternion rotation)
        {
            this.position = position;
            this.rotation = rotation;
        }
        public void Translate(Vector3 translation, Space relativeTo = Space.Self)
        {
            CauseGenericChange(() =>
            {
                transform.Translate(translation, relativeTo);
            });
        }
        public void Translate(float x, float y, float z, Space relativeTo = Space.Self)
            => Translate(new Vector3(x, y, z), relativeTo);
        public void Translate(Vector3 translation, Transform relativeTo)
        {
            CauseGenericChange(() =>
            {
                transform.Translate(translation, relativeTo);
            });
        }
        public void Translate(float x, float y, float z, Transform relativeTo)
            => Translate(new Vector3(x, y, z), relativeTo);

        #region Unchanged (ITransform Funcs)
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
        #endregion Unchanged (ITransform Funcs)
        #endregion ITransform Funcs


        private void ChangeLocalPosition(Vector3 newPos)
        {
            Vector3 oldPos = transform.localPosition;
            if (oldPos == newPos) { return; }

            transform.localPosition = newPos;

            onLocalPositionChanged?.Invoke(oldPos, newPos);
            onLocalPositionChangedNoParam?.Invoke();
        }
        private void ChangeLocalRotation(Quaternion newRot)
        {
            Quaternion oldRot = transform.localRotation;
            if (oldRot == newRot) { return; }

            transform.localRotation = newRot;

            onLocalRotationChanged?.Invoke(oldRot, newRot);
            onLocalRotationChangedNoParam?.Invoke();
        }
        private void ChangeLocalScale(Vector3 newScale)
        {
            Vector3 oldScale = transform.localScale;
            if (oldScale == newScale) { return; }

            transform.localScale = newScale;

            onLocalScaleChanged?.Invoke(oldScale, newScale);
            onLocalScaleChangedNoParam?.Invoke();
        }
        private void ChangeParent(ITransform newParent)
        {
            ITransform oldParent = parent;
            if (oldParent == newParent) { return; }

            m_parent = newParent;
            transform.parent = parent?.transform;

            onParentChanged?.Invoke(oldParent, newParent);
            onParentChangedNoParam?.Invoke();
        }
        private void ChangeSiblingIndex(int newIndex)
        {
            int oldIndex = GetSiblingIndex();
            if (newIndex == oldIndex) { return; }

            transform.SetSiblingIndex(newIndex);

            onSiblingIndexChanged?.Invoke(oldIndex, newIndex);
            onSiblingIndexChangedNoParam?.Invoke();
        }

        private void CauseGenericChange(Action changeAction)
        {
            Vector3 oldPos = localPosition;
            Quaternion oldRot = localRotation;
            Vector3 oldScale = localScale;
            ITransform oldParent = parent;
            int oldSiblingIndex = GetSiblingIndex();

            changeAction.Invoke();

            Vector3 newPos = localPosition;
            Quaternion newRot = localRotation;
            Vector3 newScale = localScale;
            ITransform newParent = parent;
            int newSiblingIndex = GetSiblingIndex();

            // Act directly on transform to restore un-monitored changes
            transform.localPosition = oldPos;
            transform.localRotation = oldRot;
            transform.localScale = oldScale;
            transform.parent = oldParent?.transform;
            transform.SetSiblingIndex(oldSiblingIndex);

            // Apply changes to DirtyTransform
            localPosition = newPos;
            localRotation = newRot;
            localScale = newScale;
            parent = newParent;
            SetSiblingIndex(newSiblingIndex);
        }
    }
}
