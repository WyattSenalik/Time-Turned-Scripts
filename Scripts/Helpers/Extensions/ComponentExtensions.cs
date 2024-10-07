using UnityEngine;

using Helpers.UnityInterfaces;
// Original Authors - Wyatt Senalik

namespace Helpers.Extensions
{
    public static class ComponentExtensions
    {
        /// <summary>
        /// <see cref="Component.GetComponents{T}()"/> but throws an assertion exception if it doesn't exist.
        /// </summary>
        /// <typeparam name="T">Type of component to get.</typeparam>
        public static T GetComponentSafe<T>(this Component component, Component queryComp = null) where T : Component
        {
            T t_comp = component.GetComponent<T>();
            #region Asserts
            string t_queryCompName = queryComp == null ? nameof(GetComponentSafe) : CustomDebug.GetComponentDebugName(queryComp);
            if (t_comp == null)
            {
                //CustomDebug.LogError($"{t_queryCompName} expected component {typeof(T)} to be attached.");
            }
            #endregion Asserts
            return t_comp;
        }

        /// <summary>
        /// <see cref="Component.GetComponents{T}()"/> but throws an assertion exception if it doesn't exist.
        /// </summary>
        /// <typeparam name="T">Type of component to get.</typeparam>
        public static T GetIComponentSafe<T>(this Component component, Component queryComp = null) where T : IComponent
        {
            T t_comp = component.GetComponent<T>();
            #region Asserts
            string t_queryCompName = queryComp == null ? nameof(GetComponentSafe) : CustomDebug.GetComponentDebugName(queryComp);
            //CustomDebug.AssertIComponentOnOtherIsNotNull(t_comp, component.gameObject, t_queryCompName);
            #endregion Asserts
            return t_comp;
        }
    }
}
