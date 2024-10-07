using UnityEngine;

using Helpers.Math;
using System.Collections.Generic;
// Original Authors - Wyatt Senalik

namespace Helpers.Extensions
{
    public static class TransformExtensions
    {
        public static void ResetLocal(this Transform transform)
        {
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;
        }
        public static void ScaleAround(this Transform target, Vector3 pivot, Vector3 newScale)
        {
            // relative scale factor
            float t_relativeScaleFactor = newScale.x / target.localScale.x; 
            // calc final position post-scale
            Vector3 t_finalPosition = MathHelpers.ScalePointAboutPosition(target.localPosition, pivot, t_relativeScaleFactor);

            // finally, actually perform the scale/translation
            target.transform.localScale = newScale;
            target.transform.localPosition = t_finalPosition;
        }
        /// <summary>
        /// Like <see cref="Transform.Find(string)"/> but searches all descendants instead of just direct children. Returns null if no descendant with the given name is found.
        /// </summary>
        public static Transform FindDescendant(this Transform transform, string name)
        {
            Transform t_asChild = transform.Find(name);
            if (t_asChild != null)
            {
                return t_asChild;
            }
            Stack<Transform> t_parents = new Stack<Transform>(new Transform[]{ transform });
            while (t_parents.Count > 0)
            {
                Transform t_curParent = t_parents.Pop();
                foreach (Transform t_child in t_curParent)
                {
                    if (t_child.name.Equals(name))
                    {
                        return t_child;
                    }
                    t_parents.Push(t_child);
                }
            }
            return null;
        }
    }
}
