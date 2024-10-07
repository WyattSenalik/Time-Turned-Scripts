using UnityEngine;
// Original Authors - Wyatt Senalik

namespace Helpers.UnityInterfaces
{
    /// <summary>
    /// Wrapper that gives a <see cref="Object"/> the ability to utilize
    /// <see cref="IObject"/> by newing this class up.
    /// </summary>
    public class FakeObject : IObject
    {
        public HideFlags hideFlags
        {
            get => m_object.hideFlags;
            set => m_object.hideFlags = value;
        }
        public string name { get => m_object.name; set => m_object.name = value; }


        // Reference to the actual Object
        private Object m_object = null;


        public FakeObject(Object obj)
        {
            m_object = obj;
        }


        public int GetInstanceID() => m_object.GetInstanceID();
    }

    public static class ObjectExtensionsForFakeObject
    {
        /// <summary>
        /// Finds the <see cref="IObject"/> driving this <see cref="Object"/>
        /// and returns it. If no <see cref="IObject"/> is driving this
        /// <see cref="Object"/>, then news up a <see cref="FakeObject"/> and
        /// returns that.
        /// </summary>
        public static IObject ToIObject(this Object obj)
        {
            // If the object is a component, check the try to return an IComponent
            // of this.
            if (obj is Component)
            {
                return (obj as Component).ToIComponent();
            }
            // TODO (below)
            //else if (obj is GameObject)
            //{
            //    return (object as GameObject).ToIGameObject();
            //}

            // Otherwise, just return a fake version of it.
            return new FakeObject(obj);
        }
    }
}
