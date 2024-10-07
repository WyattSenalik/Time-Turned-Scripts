using UnityEngine;
// Original Authors - Wyatt Senalik

namespace Helpers.UnityInterfaces
{
    /// <summary>
    /// Interface for Unity.<see cref="Object"/> if a new Unity.Object script is created
    /// with its own interface and you want to be able to do Unity.Object things from
    /// that interface.
    /// </summary>
    public interface IObject
    {
        /// <summary>
        /// Should the object be hidden, saved with the Scene or modifiable by the user?
        ///
        /// See Also: <seealso cref="HideFlags"/>.
        /// </summary>
        public HideFlags hideFlags { get; set; }
        /// <summary>
        /// The name of the object.
        ///
        /// Components share the same name with the game object and all attached
        /// components.If a class derives from MonoBehaviour it inherits the "name"
        /// field from MonoBehaviour.If this class is also attached to GameObject, then
        /// "name" field is set to the name of that GameObject.
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// Returns the instance id of the object.
        ///
        /// The instance id of an object is always guaranteed to be unique.
        /// </summary>
        public int GetInstanceID();
        /// <summary>
        /// Returns the name of the object.
        /// </summary>
        /// <returns>The name returned by ToString.</returns>
        public string ToString();
    }
}
