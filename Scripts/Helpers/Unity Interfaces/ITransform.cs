using UnityEngine;
// Original Authors - Wyatt Senalik

namespace Helpers.UnityInterfaces
{
    /// <summary>
    /// Collection of all <see cref="Transform"/> properties and functions.
    /// 
    /// There should only ever be a single <see cref="ITransform"/> attached to
    /// a <see cref="GameObject"/>.
    /// </summary>
    public interface ITransform : IComponent
    {
        /// <summary>
        /// The number of children the parent Transform has.
        /// </summary>
        int childCount { get; }
        /// <summary>
        /// The rotation as Euler angles in degrees.
        /// </summary>
        Vector3 eulerAngles { get; set; }
        /// <summary>
        /// Returns a normalized vector representing the blue axis
        /// of the transform in world space.
        /// </summary>
        Vector3 forward { get; set; }
        /// <summary>
        /// Has the transform changed since the last time
        /// the flag was set to 'false'?
        /// </summary>
        bool hasChanged { get; set; }
        /// <summary>
        /// The transform capacity of the transform's hierarchy data structure.
        /// </summary>
        int hierarchyCapacity { get; set; }
        /// <summary>
        /// The number of transforms in the transform's hierarchy data structure.
        /// </summary>
        int hierarchyCount { get; }
        /// <summary>
        /// The rotation as Euler angles in degrees relative
        /// to the parent transform's rotation.
        /// </summary>
        Vector3 localEulerAngles { get; set; }
        /// <summary>
        /// Position of the transform relative to the parent transform.
        /// </summary>
        Vector3 localPosition { get; set; }
        /// <summary>
        /// The rotation of the transform relative to the
        /// transform rotation of the parent.
        /// </summary>
        Quaternion localRotation { get; set; }
        /// <summary>
        /// The scale of the transform relative to the GameObjects parent.
        /// </summary>
        Vector3 localScale { get; set; }
        /// <summary>
        /// Matrix that transforms a point from local space
        /// into world space (Read Only).
        /// </summary>
        Matrix4x4 localToWorldMatrix { get; }
        /// <summary>
        /// The global scale of the object (Read Only).
        /// </summary>
        Vector3 lossyScale { get; }
        /// <summary>
        /// The parent of the transform.
        /// </summary>
        ITransform parent { get; set; }
        /// <summary>
        /// The world space position of the Transform.
        /// </summary>
        Vector3 position { get; set; }
        /// <summary>
        /// The red axis of the transform in world space.
        /// </summary>
        Vector3 right { get; set; }
        /// <summary>
        /// Returns the topmost transform in the hierarchy.
        /// </summary>
        ITransform root { get; }
        /// <summary>
        /// A Quaternion that stores the rotation of the Transform in world space.
        /// </summary>
        Quaternion rotation { get; set; }
        /// <summary>
        /// The green axis of the transform in world space.
        /// </summary>
        Vector3 up { get; set; }
        /// <summary>
        /// Matrix that transforms a point from world space
        /// into local space (Read Only).
        /// </summary>
        Matrix4x4 worldToLocalMatrix { get; }

        /// <summary>
        /// Unparents all children.
        /// 
        /// Useful if you want to destroy the root of a
        /// hierarchy without destroying the children.
        /// 
        /// See also <seealso cref="parent"/> to detach/change the
        /// parent of a single transform.
        /// </summary>
        void DetachChildren();
        /// <summary>
        /// Finds a child by name n and returns it.
        /// 
        /// If no child with name n can be found, null is returned.
        /// If n contains a '/' character it will access the Transform in the
        /// hierarchy like a path name.
        ///
        /// Note: Find does not perform a recursive
        /// descend down a Transform hierarchy.
        /// Note: Find can find transform on disabled GameObjects.
        /// </summary>
        /// <param name="n">Name of child to be found.</param>
        /// <returns>Transform The found child transform.
        /// Null if child with matching name isn't found.</returns>
        ITransform Find(string n);
        /// <summary>
        /// Returns a transform child by index.
        ///
        /// If the transform has no child, or the index argument has a value
        /// greater than the number of children then an error will be generated.
        /// In this case "Transform child out of bounds" error will be given.
        /// The number of children can be provided by <see cref="childCount"/>.
        /// </summary>
        /// <param name="index">Index of the child transform to return.
        /// Must be smaller than Transform.childCount.</param>
        /// <returns>Transform Transform child by index.</returns>
        ITransform GetChild(int index);
        /// <summary>
        /// Gets the sibling index.
        ///
        /// Use this to return the sibling index of the GameObject.
        /// If a GameObject shares a parent with other GameObjects and
        /// are on the same level (i.e. they share the same direct parent),
        /// these GameObjects are known as siblings. The sibling index shows
        /// where each GameObject sits in this sibling hierarchy.
        ///
        /// Use GetSiblingIndex to find out the GameObject’s place
        /// in this hierarchy. When the sibling index of a GameObject is changed,
        /// its order in the Hierarchy window will also change.
        /// This is useful if you are intentionally ordering the children of a
        /// GameObject such as when using Layout Group components.
        ///
        /// Layout Groups will also visually reorder the group by their index.
        /// To read more about Layout Groups see AutoLayout.
        /// To set the sibling index of a GameObject, see
        /// <see cref="SetSiblingIndex(int)"/>
        /// </summary>
        int GetSiblingIndex();
        /// <summary>
        /// Transforms a direction from world space to local space.
        /// The opposite of Transform.TransformDirection.
        ///
        /// This operation is unaffected by scale.
        ///
        /// You should use <see cref="InverseTransformPoint"/> if the vector
        /// represents a position in space rather than a direction.
        /// </summary>
        Vector3 InverseTransformDirection(Vector3 direction);
        /// <summary>
        /// Transforms the direction x, y, z from world space to local space.
        /// The opposite of Transform.TransformDirection.
        ///
        /// This operation is unaffected by scale.
        /// </summary>
        Vector3 InverseTransformDirection(float x, float y, float z);
        /// <summary>
        /// Transforms position from world space to local space.
        ///
        /// This function is essentially the opposite of
        /// <see cref="TransformPoint"/>, which is used to convert from
        /// local to world space.
        ///
        /// Note that the returned position is affected by scale. Use
        /// <see cref="InverseTransformDirection"/> if you are dealing with
        /// direction vectors rather than positions.
        /// </summary>
        Vector3 InverseTransformPoint(Vector3 position);
        /// <summary>
        /// Transforms the position x, y, z from world space to local space.
        /// The opposite of Transform.TransformPoint.
        ///
        /// Note that the returned position is affected by scale.
        /// Use <see cref="InverseTransformDirection"/> if you are
        /// dealing with directions.
        /// </summary>
        Vector3 InverseTransformPoint(float x, float y, float z);
        /// <summary>
        /// Transforms a vector from world space to local space.
        /// The opposite of Transform.TransformVector.
        ///
        /// This operation is affected by scale.
        /// </summary>
        Vector3 InverseTransformVector(Vector3 vector);
        /// <summary>
        /// Transforms the vector x, y, z from world space to local space.
        /// The opposite of Transform.TransformVector.
        ///
        /// This operation is affected by scale.
        /// </summary>
        Vector3 InverseTransformVector(float x, float y, float z);
        /// <summary>
        /// Is this transform a child of parent?
        /// 
        /// Returns a boolean value that indicates whether the transform is a
        /// child of a given transform. true if this transform is a child,
        /// deep child (child of a child) or identical to this transform,
        /// otherwise false.
        /// </summary>
        bool IsChildOf(Transform parent);
        /// <summary>
        /// Rotates the transform so the forward vector points at target's
        /// current position.
        ///
        /// Then it rotates the transform to point its up direction vector in the
        /// direction hinted at by the worldUp vector. If you leave out the worldUp
        /// parameter, the function will use the world y axis. The up vector of the
        /// rotation will only match the worldUp vector if the forward direction is
        /// perpendicular to worldUp.
        /// </summary>
        /// <param name="target">Object to point towards.</param>
        void LookAt(Transform target);
        /// <summary>
        /// Rotates the transform so the forward vector points at target's
        /// current position.
        ///
        /// Then it rotates the transform to point its up direction vector in the
        /// direction hinted at by the worldUp vector. If you leave out the worldUp
        /// parameter, the function will use the world y axis. The up vector of the
        /// rotation will only match the worldUp vector if the forward direction is
        /// perpendicular to worldUp.
        /// </summary>
        /// <param name="target">Object to point towards.</param>
        /// <param name="worldUp">Vector specifying the upward direction.</param>
        void LookAt(Transform target, Vector3 worldUp);
        /// <summary>
        /// Rotates the transform so the forward vector points at worldPosition.
        ///
        /// Then it rotates the transform to point its up direction vector in the
        /// direction hinted at by the worldUp vector. If you leave out the worldUp
        /// parameter, the function will use the world y axis. The up vector of the
        /// rotation will only match the worldUp vector if the forward direction is
        /// perpendicular to worldUp.
        /// </summary>
        /// <param name="worldPosition">Point to look at.</param>
        void LookAt(Vector3 worldPosition);
        /// <summary>
        /// Rotates the transform so the forward vector points at worldPosition.
        ///
        /// Then it rotates the transform to point its up direction vector in the
        /// direction hinted at by the worldUp vector. If you leave out the worldUp
        /// parameter, the function will use the world y axis. The up vector of the
        /// rotation will only match the worldUp vector if the forward direction is
        /// perpendicular to worldUp.
        /// </summary>
        /// <param name="worldPosition">Point to look at.</param>
        /// <param name="worldUp">Vector specifying the upward direction.</param>
        void LookAt(Vector3 worldPosition, Vector3 worldUp);
        /// <summary>
        /// Applies a rotation of eulerAngles.z degrees around the z-axis,
        /// eulerAngles.x degrees around the x-axis, and eulerAngles.y degrees
        /// around the y-axis (in that order).
        ///
        /// Rotate takes a Vector3 argument as an Euler angle. The second argument
        /// is the rotation axes, which can be set to local axis (Space.Self) or
        /// global axis (Space.World). The rotation is by the Euler amount.
        /// </summary>
        /// <param name="eulers">The rotation to apply in euler angles.</param>
        /// <param name="relativeTo">Determines whether to rotate the GameObject
        /// either locally to the GameObject or relative to the Scene in world
        /// space.</param>
        void Rotate(Vector3 eulers, Space relativeTo = Space.Self);
        /// <summary>
        /// The implementation of this method applies a rotation of zAngle degrees
        /// around the z axis, xAngle degrees around the x axis, and yAngle degrees
        /// around the y axis (in that order).
        ///
        /// Rotate can have the euler angle specified in 3 floats for x, y, and z.
        ///
        /// The example shows two cubes: one cube uses <see cref="Space.Self"/>
        /// (the local space and axes of the <see cref="GameObject"/>) and the other
        /// uses <see cref="Space.World"/> (the space and axes in relation to the
        /// Scene). They are both first rotated 90 in the X axis so they are not
        /// aligned with the world axis by default. Use the xAngle, yAngle and
        /// zAngle values exposed in the inspector to see how different rotation
        /// values apply to both cubes. You might notice the way the cubes visually
        /// rotate is dependant on the current orientation and Space option used.
        /// Play around with the values while selecting the cubes in the scene view
        /// to try to understand how the values interact.
        /// </summary>
        /// <param name="xAngle">Degrees to rotate the GameObject around the X axis.
        /// </param>
        /// <param name="yAngle">Degrees to rotate the GameObject around the Y axis.
        /// </param>
        /// <param name="zAngle">Degrees to rotate the GameObject around the Z axis.
        /// </param>
        /// <param name="relativeTo">Determines whether to rotate the GameObject
        /// either locally to the GameObject or relative to the Scene in world
        /// space.</param>
        void Rotate(float xAngle, float yAngle, float zAngle,
            Space relativeTo = Space.Self);
        /// <summary>
        /// Rotates the object around the given axis by the number of degrees
        /// defined by the given angle.
        ///
        /// Rotate has an axis, angle and the local or global parameters. The
        /// rotation axis can be in any direction.
        /// </summary>
        /// <param name="axis">The axis to apply rotation to.</param>
        /// <param name="angle">The degrees of rotation to apply.</param>
        /// <param name="relativeTo">Determines whether to rotate the GameObject
        /// either locally to the GameObject or relative to the Scene in world
        /// space.</param>
        void Rotate(Vector3 axis, float angle, Space relativeTo = Space.Self);
        /// <summary>
        /// Applies a rotation of eulerAngles.z degrees around the z-axis,
        /// eulerAngles.x degrees around the x-axis, and eulerAngles.y degrees
        /// around the y-axis (in that order).
        ///
        /// The rotation is relative to the GameObject's local space
        /// (<see cref="Space.Self"/>).
        /// </summary>
        /// <param name="eulers">The rotation to apply in euler angles.</param>
        void Rotate(Vector3 eulers);
        /// <summary>
        /// The implementation of this method applies a rotation of zAngle degrees
        /// around the z axis, xAngle degrees around the x axis, and yAngle degrees
        /// around the y axis (in that order).
        ///
        /// The rotation is relative to the GameObject's local space
        /// (<see cref="Space.Self"/>).
        /// </summary>
        /// <param name="axis">The axis to apply rotation to.</param>
        /// <param name="angle">The degrees of rotation to apply.</param>
        void Rotate(Vector3 axis, float angle);
        /// <summary>
        /// Rotates the transform about axis passing through point in world
        /// coordinates by angle degrees.
        ///
        /// This modifies both the position and the rotation of the transform.
        /// </summary>
        void RotateAround(Vector3 point, Vector3 axis, float angle);
        /// <summary>
        /// Move the transform to the start of the local transform list.
        /// </summary>
        void SetAsFirstSibling();
        /// <summary>
        /// Move the transform to the end of the local transform list.
        /// </summary>
        void SetAsLastSibling();
        /// <summary>
        /// Set the parent of the transform.
        ///
        /// This method is the same as the <see cref="parent"/> property except that
        /// it also lets the <see cref="Transform"/> keep its local orientation
        /// rather than its global orientation. This means for example, if the
        /// GameObject was previously next to its parent, setting worldPositionStays
        /// to false will move the GameObject to be positioned next to its new
        /// parent in the same way.
        ///
        /// The default value of worldPositionStays argument is true.
        /// </summary>
        /// <param name="parent">The parent Transform to use.</param>
        void SetParent(Transform parent, bool worldPositionStays = true);
        /// <summary>
        /// Sets the world space position and rotation of the Transform component.
        /// </summary>
        void SetPositionAndRotation(Vector3 position, Quaternion rotation);
        /// <summary>
        /// Sets the sibling index.
        ///
        /// Use this to change the sibling index of the GameObject. If a GameObject
        /// shares a parent with other GameObjects and are on the same level (i.e.
        /// they share the same direct parent), these GameObjects are known as
        /// siblings. The sibling index shows where each GameObject sits in this
        /// sibling hierarchy.
        ///
        /// Use SetSiblingIndex to change the GameObject’s place in this hierarchy.
        /// When the sibling index of a GameObject is changed, its order in the
        /// Hierarchy window will also change. This is useful if you are
        /// intentionally ordering the children of a GameObject such as when using
        /// Layout Group components.
        ///
        /// Layout Groups will also visually reorder the group by their index. To
        /// read more about Layout Groups see AutoLayout. To return the sibling
        /// index of a GameObject, see <see cref="GetSiblingIndex"/>.
        /// </summary>
        /// <param name="index">Index to set.</param>
        void SetSiblingIndex(int index);
        /// <summary>
        /// Transforms direction from local space to world space.
        ///
        /// This operation is not affected by scale or position of the transform.
        /// The returned vector has the same length as direction.
        ///
        /// You should use <see cref="TransformPoint"/> for the conversion if the
        /// vector represents a position rather than a direction.
        /// </summary>
        Vector3 TransformDirection(Vector3 direction);
        /// <summary>
        /// Transforms direction x, y, z from local space to world space.
        ///
        /// This operation is not affected by scale or position of the transform.
        /// The returned vector has the same length as direction.
        /// </summary>
        Vector3 TransformDirection(float x, float y, float z);
        /// <summary>
        /// Transforms position from local space to world space.
        ///
        /// Note that the returned position is affected by scale. Use
        /// <see cref="TransformDirection"/> if you are dealing with direction
        /// vectors.
        ///
        /// You can perform the opposite conversion, from world to local space using
        /// <see cref="InverseTransformPoint"/>.
        /// </summary>
        Vector3 TransformPoint(Vector3 position);
        /// <summary>
        /// Transforms the position x, y, z from local space to world space.
        ///
        /// Note that the returned position is affected by scale. Use
        /// <see cref="TransformDirection"/> if you are dealing with directions.
        /// </summary>
        Vector3 TransformPoint(float x, float y, float z);
        /// <summary>
        /// Transforms vector from local space to world space.
        ///
        /// This operation is not affected by position of the transform, but it is
        /// affected by scale. The returned vector may have a different length than
        /// vector.
        /// </summary>
        Vector3 TransformVector(Vector3 vector);
        /// <summary>
        /// Transforms vector x, y, z from local space to world space.
        ///
        /// This operation is not affected by position of the transform, but is is
        /// affected by scale. The returned vector may have a different length than
        /// vector.
        /// </summary>
        Vector3 TransformVector(float x, float y, float z);
        /// <summary>
        /// Moves the transform in the direction and distance of translation.
        ///
        /// If relativeTo is left out or set to <see cref="Space.Self"/> the
        /// movement is applied relative to the transform's local axes. (the x, y
        /// and z axes shown when selecting the object inside the Scene View.) If
        /// relativeTo is <see cref="Space.World"/> the movement is applied relative
        /// to the world coordinate system.
        /// </summary>
        void Translate(Vector3 translation, Space relativeTo = Space.Self);
        /// <summary>
        /// Moves the transform by x along the x axis, y along the y axis, and z
        /// along the z axis.
        ///
        /// If relativeTo is left out or set to <see cref="Space.Self"/> the
        /// movement is applied relative to the transform's local axes. (the x, y
        /// and z axes shown when selecting the object inside the Scene View.) If
        /// relativeTo is <see cref="Space.World"/> the movement is applied relative
        /// to the world coordinate system.
        /// </summary>
        void Translate(float x, float y, float z, Space relativeTo = Space.Self);
        /// <summary>
        /// Moves the transform in the direction and distance of translation.
        ///
        /// The movement is applied relative to relativeTo's local coordinate
        /// system. If relativeTo is null, the movement is applied relative to the
        /// world coordinate system.
        /// </summary>
        void Translate(Vector3 translation, Transform relativeTo);
        /// <summary>
        /// Moves the transform by x along the x axis, y along the y axis, and z
        /// along the z axis.
        ///
        /// The movement is applied relative to relativeTo's local coordinate
        /// system. If relativeTo is null, the movement is applied relative to the
        /// world coordinate system.
        /// </summary>
        void Translate(float x, float y, float z, Transform relativeTo);
    }
}
