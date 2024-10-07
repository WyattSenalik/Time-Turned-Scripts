using UnityEngine;
using UnityEditor;

using Helpers.UnityInterfaces;
// Original Authors - Wyatt Senalik

namespace Helpers.Editor
{
    /// <summary>
    /// Common functions to use for selecting game objects in the editor.
    /// </summary>
    public static class EditorSelection
    {
        /// <summary>
        /// Tries the get the given Component off the currently selected GameObject.
        /// </summary>
        /// <typeparam name="TComponent">Desired type of Component to pull off the GameObject.</typeparam>
        /// <param name="retrievedComponent">Component instance that was pulled of the GameObject.</param>
        /// <param name="errorMsg">Empty if returns true. If returns false, gives the reason why its false.</param>
        /// <returns>False if there is no selected GameObject or the specified Component is not attached
        /// to the given GameObject.</returns>
        public static bool TryGetEditorSelectedComponent<TComponent>(out TComponent retrievedComponent, out string errorMsg) where TComponent : Component
        {
            GameObject t_curSel = Selection.activeGameObject;
            // No selection object.
            if (t_curSel == null)
            {
                retrievedComponent = null;
                errorMsg = "Selection is null";
                return false;
            }
            // Selection object does not have a timed transform.
            if (!t_curSel.TryGetComponent(out retrievedComponent))
            {
                errorMsg = $"Object ({t_curSel}) has no {typeof(TComponent).Name}";
                return false;
            }

            errorMsg = "";
            return true;
        }
        public static bool TryGetEditorSelectedComponent<TComponent>(out TComponent retrievedComponent) where TComponent : Component => TryGetEditorSelectedComponent(out retrievedComponent, out _);
        /// <summary>
        /// Tries the get the given IComponent off the currently selected GameObject.
        /// </summary>
        /// <typeparam name="TComponent">Desired type of IComponent to pull off the GameObject.</typeparam>
        /// <param name="retrievedComponent">IComponent instance that was pulled of the GameObject.</param>
        /// <param name="errorMsg">Empty if returns true. If returns false, gives the reason why its false.</param>
        /// <returns>False if there is no selected GameObject or the specified IComponent is not attached
        /// to the given GameObject.</returns>
        public static bool TryGetEditorSelectedIComponent<TComponent>(out TComponent retrievedComponent, out string errorMsg) where TComponent : IComponent
        {
            GameObject t_curSel = Selection.activeGameObject;
            // No selection object.
            if (t_curSel == null)
            {
                retrievedComponent = default;
                errorMsg = "Selection is null";
                return false;
            }
            // Selection object does not have a timed transform.
            if (!t_curSel.TryGetComponent(out retrievedComponent))
            {
                errorMsg = $"Object ({t_curSel}) has no {typeof(TComponent).Name}";
                return false;
            }

            errorMsg = "";
            return true;
        }
        public static bool TryGetEditorSelectedIComponent<TComponent>(out TComponent retrievedComponent) where TComponent : IComponent => TryGetEditorSelectedIComponent(out retrievedComponent, out _);
    }
}