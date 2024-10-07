using UnityEditor;
using UnityEngine;
// Original Authors - Eslis Vang


namespace Atma.Editors
{
    /// <summary>
    /// Modifies the default inspector for the <see cref="SwitchPressurePlateHandler"/> script.
    /// </summary>
    [CustomEditor(typeof(SwitchPressurePlateHandler))]
	public class PressurePlateInspector : Editor
    {
		private const string GIZMO_TOGGLED_KEY = "gizmosToggled";


		private bool m_gizmosToggled = true;

        private void OnEnable()
        {
			LinkedObjectEditor.UpdateGizmos.AddListener(ToggleGizmos);
        }

        public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

            m_gizmosToggled = EditorPrefs.GetBool(GIZMO_TOGGLED_KEY, true);

            if (GUILayout.Button("Open Editor GUI"))
			{
				LinkedObjectEditor.OpenEditor();
			}
			if (GUILayout.Button("Toggle Gizmos"))
			{
                ToggleGizmos();
            }
		}

		public void OnSceneGUI()
		{
			LinkedObjectGizmoDrawer.OnSceneGUI(m_gizmosToggled, target);
		}

		private void ToggleGizmos()
		{
            m_gizmosToggled = !m_gizmosToggled;
            EditorPrefs.SetBool(GIZMO_TOGGLED_KEY, m_gizmosToggled);
            SceneView.RepaintAll();
        }
	}
}