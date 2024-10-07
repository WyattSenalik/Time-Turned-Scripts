using Helpers.UnityInterfaces;
using System.Collections.Generic;
using UnityEngine;
// Original Authors - Wyatt Senalik.

namespace Helpers.Extensions
{
    /// <summary>
    /// Extensions for the GameObject class.
    /// </summary>
    public static class GameObjectExtensions
    {
        /// <summary>
        /// Finds all children of this GameObject that have the specified tag.
        /// Will include this GameObject in the return list if it also has the tag.
        /// </summary>
        /// <param name="gameObject">Current GameObject to check the children of.</param>
        /// <param name="tag">Tag to compare the children against.</param>
        public static GameObject[] FindChildrenWithTag(this GameObject gameObject,
            string tag)
        {
            // List to be returned. Holds all found children that have been tagged.
            List<GameObject> temp_foundChildrenWithTag = new List<GameObject>();

            // Special case: check if the root has the tag.
            if (gameObject.CompareTag(tag))
            {
                temp_foundChildrenWithTag.Add(gameObject);
            }

            // The current transforms whose children will be checked for having the tag.
            List<Transform> temp_curGen = new List<Transform>() { gameObject.transform };
            // The generation formed when the current generation is iterated over.
            // Will replace the current generation after all children of the current
            // generation have been iterated over and added to this list.
            List<Transform> temp_nextGen = new List<Transform>();
            // Continue until we exhaust all descendents
            while (temp_curGen.Count > 0)
            {
                // Each parent transform in our current generation list.
                foreach (Transform temp_curParent in temp_curGen)
                {
                    // Each child of the current parent transform.
                    foreach (Transform temp_curChild in temp_curParent)
                    {
                        // If the tag is what we are looking for,
                        // add it to the return list.
                        if (temp_curChild.CompareTag(tag))
                        {
                            temp_foundChildrenWithTag.Add(temp_curChild.gameObject);
                        }
                        // No matter what, we need to check the children of this child
                        // in the next iteration, so add it to the next generation.
                        temp_nextGen.Add(temp_curChild);
                    }
                }
                // The current generations' direct children have all been searched,
                // so replace the current generation with the next generation.
                temp_curGen.Clear();
                temp_curGen.AddRange(temp_nextGen);
                // Free up the next generation to hold the
                // new current generation's children next iteration.
                temp_nextGen.Clear();
            }

            return temp_foundChildrenWithTag.ToArray();
        }
        /// <summary>
        /// Sets the layer of this gameobject and all its descendents to
        /// the given layer.
        /// </summary>
        /// <param name="layer">Layer to set the gameobject and all its descendents
        /// to.</param>
        public static void SetLayerRecursively(this GameObject gameObject, int layer)
        {
            // Change the current gameobject's layer
            gameObject.layer = layer;
            // Call this function on each of the gameobject's children
            foreach (Transform child in gameObject.transform)
            {
                SetLayerRecursively(child.gameObject, layer);
            }
        }
        public static string GetFullName(this GameObject gameObject)
        {
            List<string> t_ancestorNames = new List<string>();
            Transform t_curAncestor = gameObject.transform;
            while (t_curAncestor != null)
            {
                t_ancestorNames.Add(t_curAncestor.name);
                t_curAncestor = t_curAncestor.parent;
            }
            if (t_ancestorNames.Count == 1)
            {
                return t_ancestorNames[0];
            }
            else
            {
                string t_rtnStr = $"{t_ancestorNames[^1]}";
                for (int i = t_ancestorNames.Count - 2; i >= 0; --i)
                {
                    t_rtnStr += $"/{t_ancestorNames[i]}";
                }
                return t_rtnStr;
            }
        }
        /// <summary>
        /// <see cref="GameObject.GetComponents{T}()"/> but throws an assertion exception if it doesn't exist.
        /// </summary>
        /// <typeparam name="T">Type of component to get.</typeparam>
        public static T GetComponentSafe<T>(this GameObject gameObject, Component queryComp = null) where T : Component
        {
            T t_comp = gameObject.GetComponent<T>();
            #region Asserts
            string t_queryCompName = queryComp == null ? nameof(GetComponentSafe) : CustomDebug.GetComponentDebugName(queryComp);
            //CustomDebug.AssertComponentOnOtherIsNotNull(t_comp, gameObject, t_queryCompName);
            #endregion Asserts
            return t_comp;
        }
        /// <summary>
        /// <see cref="GameObject.GetComponents{T}()"/> but throws an assertion exception if it doesn't exist.
        /// </summary>
        /// <typeparam name="T">Type of component to get.</typeparam>
        public static T GetIComponentSafe<T>(this GameObject gameObject, IComponent queryComp = null) where T : IComponent
        {
            T t_comp = gameObject.GetComponent<T>();
            #region Asserts
            string t_queryCompName = queryComp == null ? nameof(GetComponentSafe) : CustomDebug.GetComponentDebugName(queryComp);
            //CustomDebug.AssertIComponentOnOtherIsNotNull(t_comp, gameObject, t_queryCompName);
            #endregion Asserts
            return t_comp;
        }
    }
}
