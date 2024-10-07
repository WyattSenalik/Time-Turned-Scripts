using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
// Original Authors - Eslis Vang


namespace Atma.Editors
{
    /// <summary>
    /// Modifies the default inspector for the <see cref="InteractionSingleton"/> script.
    /// </summary>
    [CustomEditor(typeof(InteractionSingleton))]
    public class InteractionSingletonInspector : Editor
    {
        private InteractionSingleton m_interactSingleton;
        private bool m_showDebug = false;
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            m_interactSingleton = (InteractionSingleton)target;
            if (GUILayout.Button("Scrape interactables in scene"))
            {
                m_interactSingleton.ClearDictionaries();
                GameObject[] t_goList = FindObjectsOfType<GameObject>();
                GetInteractables(
                    ExtractComponentFromGameObjectArrayToList<IToggleable>(t_goList),
                    ExtractComponentFromGameObjectArrayToList<IActivator>(t_goList)
                    );
                if (!m_showDebug) { return; }
                m_interactSingleton.PrintDictionaries();
            }
            string t_debugPrompt = m_showDebug ? "Hide" : "Show";
            if (GUILayout.Button($"{t_debugPrompt} Debug Logs"))
            {
                m_showDebug = !m_showDebug;
            }
        }

        /// <summary>
        /// Extracts a component from a GameObject array.
        /// </summary>
        /// <returns>A list of the corresponding component in the GameObject array</returns>
        private List<T> ExtractComponentFromGameObjectArrayToList<T>(GameObject[] t_objList)
        {
            List<T> t_list = new List<T>();
            foreach (GameObject t_obj in t_objList)
            {
                if (t_obj.TryGetComponent(out T t_component))
                {
                    t_list.Add(t_component);
                }
            }
            return t_list;
        }

        private void GetInteractables(List<IToggleable> t_toggleables, List<IActivator> t_activators)
        {
            FillDict<IToggleable, IActivator>(t_toggleables);
            FillDict<IActivator, IToggleable>(t_activators);
        }

        private void FillDict<T1, T2>(List<T1> t_list)
        {
            foreach (T1 t_o in t_list)
            {
                // Caches the property "linkedObjects" if t_o has it.
                PropertyInfo t_prop = t_o.GetType().GetProperty("linkedObjects");
                IReadOnlyList<GameObject> t_goList;
                List<T2> t_dictList = new List<T2>();
                if (t_prop != null) // If linkedObjects property was found.
                {
                    // Cache the value assigned to linkedObjects property.
                    object t_linkedObjects = t_o.GetType().GetProperty("linkedObjects").GetValue(t_o);
                    t_goList = t_linkedObjects as IReadOnlyList<GameObject>; // Cast object to IReadOnlyList.
                }
                else
                {
                    // Make function call to GetLinkedObjects and cache it.
                    object t_linkedObjects = t_o.GetType().GetMethod("GetLinkedObjects").Invoke(t_o, null);
                    t_goList = t_linkedObjects as IReadOnlyList<GameObject>; // Cast object to IReadOnlyList.
                }
                // Iterate IReadOnlyList to build the value of the current key.
                foreach (GameObject t_go in t_goList)
                {
                    if (t_go == null) { continue; }
                    if (t_go.TryGetComponent(out T2 t_component))
                    {
                        t_dictList.Add(t_component);
                    }
                }
                // Add key and value to the correct dictionary based on the type of T1.
                if (typeof(T1) == typeof(IToggleable))
                {
                    m_interactSingleton.AddToggleable((IToggleable)t_o, t_dictList as List<IActivator>);
                }
                else if (typeof(T1) == typeof(IActivator))
                {
                    m_interactSingleton.AddActivator((IActivator)t_o, t_dictList as List<IToggleable>);
                }
            }
        }

    }
}