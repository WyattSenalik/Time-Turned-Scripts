using System;
using UnityEngine;

using Helpers.UnityInterfaces;
// Original Authors - Wyatt Senalik

namespace Helpers.Transforms
{
    /// <summary>
    /// To know where Batman is in the world.
    /// </summary>
    public class OrphanTransform : MonoBehaviour, ITransform
    {
        public int childCount => transform.childCount;
        public Vector3 eulerAngles { get => transform.eulerAngles; set => transform.eulerAngles = value; }
        public Vector3 forward { get => transform.forward; set => transform.forward = value; }
        public bool hasChanged { get => transform.hasChanged; set => transform.hasChanged = value; }
        public int hierarchyCapacity { get => transform.hierarchyCapacity; set => transform.hierarchyCapacity = value; }
        public int hierarchyCount => transform.hierarchyCount;

        public Vector3 localEulerAngles
        {
            get => localRotation.eulerAngles;
            set => localRotation = Quaternion.Euler(value);
        }
        public Vector3 localPosition
        {
            get => parent.worldToLocalMatrix * position;
            set => position = parent.localToWorldMatrix * value;
        }
        public Quaternion localRotation
        {
            get
            {
                transform.parent = parent.transform;
                Quaternion t_localRot = transform.localRotation;
                transform.parent = null;
                return t_localRot;
            }
            set
            {
                transform.parent = parent.transform;
                transform.localRotation = value;
                transform.parent = null;
            }
        }
        public Vector3 localScale
        {
            get
            {
                transform.parent = parent.transform;
                Vector3 t_localScale = transform.localScale;
                transform.parent = null;
                return t_localScale;
            }
            set
            {
                transform.parent = parent.transform;
                transform.localScale = value;
                transform.parent = null;
            }
        }

        public Matrix4x4 localToWorldMatrix
        {
            get
            {
                transform.parent = parent.transform;
                Matrix4x4 t_mat = transform.localToWorldMatrix;
                transform.parent = null;
                return t_mat;
            }
        }
        public ITransform parent { get; set; }
        public Matrix4x4 worldToLocalMatrix
        {
            get
            {
                transform.parent = parent.transform;
                Matrix4x4 t_mat = transform.worldToLocalMatrix;
                transform.parent = null;
                return t_mat;
            }
        }

        public Vector3 lossyScale => transform.lossyScale;
        public Vector3 position { get => transform.position; set => transform.position = value; }
        public Vector3 right { get => transform.right; set => transform.right = value; }
        public ITransform root => transform.root.ToITransform();
        public Quaternion rotation { get => transform.rotation; set => transform.rotation = value; }
        public Vector3 up { get => transform.up; set => transform.up = value; }



        public void DetachChildren() => transform.DetachChildren();
        public ITransform Find(string n) => transform.Find(n).ToITransform();
        public ITransform GetChild(int index) => transform.GetChild(index).ToITransform();
        public int GetSiblingIndex() => transform.GetSiblingIndex();

        public Vector3 InverseTransformDirection(Vector3 direction)
        {
            transform.parent = parent.transform;
            Vector3 t_rtnVect = transform.InverseTransformDirection(direction);
            transform.parent = null;
            return t_rtnVect;
        }
        public Vector3 InverseTransformDirection(float x, float y, float z)
        {
            transform.parent = parent.transform;
            Vector3 t_rtnVect = transform.InverseTransformDirection(x, y, z);
            transform.parent = null;
            return t_rtnVect;
        }
        public Vector3 InverseTransformPoint(Vector3 position)
        {
            transform.parent = parent.transform;
            Vector3 t_rtnVect = transform.InverseTransformPoint(position);
            transform.parent = null;
            return t_rtnVect;
        }
        public Vector3 InverseTransformPoint(float x, float y, float z)
        {
            transform.parent = parent.transform;
            Vector3 t_rtnVect = transform.InverseTransformPoint(x, y, z);
            transform.parent = null;
            return t_rtnVect;
        }
        public Vector3 InverseTransformVector(Vector3 vector)
        {
            transform.parent = parent.transform;
            Vector3 t_rtnVect = transform.InverseTransformVector(vector);
            transform.parent = null;
            return t_rtnVect;
        }
        public Vector3 InverseTransformVector(float x, float y, float z)
        {
            transform.parent = parent.transform;
            Vector3 t_rtnVect = transform.InverseTransformVector(x, y, z);
            transform.parent = null;
            return t_rtnVect;
        }

        public bool IsChildOf(Transform parent) => false;
        public void LookAt(Transform target) => transform.LookAt(target);
        public void LookAt(Transform target, Vector3 worldUp) => transform.LookAt(transform, worldUp);
        public void LookAt(Vector3 worldPosition) => transform.LookAt(worldPosition);
        public void LookAt(Vector3 worldPosition, Vector3 worldUp) => transform.LookAt(worldPosition, worldUp);

        public void Rotate(Vector3 eulers, Space relativeTo = Space.Self)
        {
            transform.parent = parent.transform;
            transform.Rotate(eulers, relativeTo);
            transform.parent = null;
        }
        public void Rotate(float xAngle, float yAngle, float zAngle, Space relativeTo = Space.Self)
        {
            transform.parent = parent.transform;
            transform.Rotate(xAngle, yAngle, zAngle, relativeTo);
            transform.parent = null;
        }
        public void Rotate(Vector3 axis, float angle, Space relativeTo = Space.Self)
        {
            transform.parent = parent.transform;
            transform.Rotate(axis, angle, relativeTo);
            transform.parent = null;
        }
        public void Rotate(Vector3 eulers)
        {
            transform.parent = parent.transform;
            transform.Rotate(eulers);
            transform.parent = null;
        }
        public void Rotate(Vector3 axis, float angle)
        {
            transform.parent = parent.transform;
            transform.Rotate(axis, angle);
            transform.parent = null;
        }
        public void RotateAround(Vector3 point, Vector3 axis, float angle)
        {
            transform.parent = parent.transform;
            transform.RotateAround(point, axis, angle);
            transform.parent = null;
        }

        public void SetAsFirstSibling() => transform.SetAsFirstSibling();
        public void SetAsLastSibling() => transform.SetAsLastSibling();
        public void SetParent(Transform parent, bool worldPositionStays = true)
        {
            Vector3 t_prevLocalPos = localPosition;
            this.parent = parent.ToITransform();
            localPosition = t_prevLocalPos;
        }
        public void SetPositionAndRotation(Vector3 position, Quaternion rotation)
            => transform.SetPositionAndRotation(position, rotation);
        public void SetSiblingIndex(int index) => transform.SetSiblingIndex(index);

        public Vector3 TransformDirection(Vector3 direction)
        {
            transform.parent = parent.transform;
            Vector3 t_rtnVec = transform.TransformDirection(direction);
            transform.parent = null;
            return t_rtnVec;
        }
        public Vector3 TransformDirection(float x, float y, float z)
        {
            transform.parent = parent.transform;
            Vector3 t_rtnVec = transform.TransformDirection(x, y, z);
            transform.parent = null;
            return t_rtnVec;
        }
        public Vector3 TransformPoint(Vector3 position)
        {
            transform.parent = parent.transform;
            Vector3 t_rtnVec = transform.TransformPoint(position);
            transform.parent = null;
            return t_rtnVec;
        }
        public Vector3 TransformPoint(float x, float y, float z)
        {
            transform.parent = parent.transform;
            Vector3 t_rtnVec = transform.TransformPoint(x, y, z);
            transform.parent = null;
            return t_rtnVec;
        }
        public Vector3 TransformVector(Vector3 vector)
        {
            transform.parent = parent.transform;
            Vector3 t_rtnVec = transform.TransformVector(vector);
            transform.parent = null;
            return t_rtnVec;
        }
        public Vector3 TransformVector(float x, float y, float z)
        {
            transform.parent = parent.transform;
            Vector3 t_rtnVec = transform.TransformVector(x, y, z);
            transform.parent = null;
            return t_rtnVec;
        }

        public void Translate(Vector3 translation, Space relativeTo = Space.Self)
        {
            transform.parent = parent.transform;
            transform.Translate(translation, relativeTo);
            transform.parent = null;
        }
        public void Translate(float x, float y, float z, Space relativeTo = Space.Self)
        {
            transform.parent = parent.transform;
            transform.Translate(x, y, z, relativeTo);
            transform.parent = null;
        }
        public void Translate(Vector3 translation, Transform relativeTo)
        {
            transform.parent = parent.transform;
            transform.Translate(translation, relativeTo);
            transform.parent = null;
        }
        public void Translate(float x, float y, float z, Transform relativeTo)
        {
            transform.parent = parent.transform;
            transform.Translate(x, y, z, relativeTo);
            transform.parent = null;
        }
    }
}