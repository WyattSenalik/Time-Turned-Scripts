using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using NaughtyAttributes;
// Original Authors - Eslis Vang

namespace Atma.Editors
{
    /// <summary>
    /// Modifies the default inspector for the <see cref="Door"/> scripts
    /// </summary>
    [CustomEditor(typeof(Door)), CanEditMultipleObjects]
    public class DoorInspector : Editor
    {
        private const string GIZMO_TOGGLED_KEY = "gizmosToggled";
        private const float TILE_SIZE = 80f;
        private const float DOTTED_LINE_SIZE = 5f;

        private bool m_gizmosToggled = true;

        private void OnEnable()
        {
            LinkedObjectEditor.UpdateGizmos.AddListener(ToggleGizmos);
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            m_gizmosToggled = EditorPrefs.GetBool(GIZMO_TOGGLED_KEY, true);

            if (GUILayout.Button("Toggle Gizmos"))
            {
                ToggleGizmos();
            }
        }

        public void OnSceneGUI()
        {
            if (!m_gizmosToggled) { return; }
            Door t_target = target as Door;
            Transform t_tarTrans = t_target.transform;
            // Actively selected object's world position to GUI screen position.
            Vector2 t_targetPos = HandleUtility.WorldToGUIPoint(t_tarTrans.position);
            // Size of GUI cube scaled with zoom.
            Vector2 t_drawCubeSize = t_tarTrans.localScale * TILE_SIZE / HandleUtility.
                GetHandleSize(t_targetPos);
            // Size of GUI dotted line scaled with zoom.
            float t_dottedLineSize = DOTTED_LINE_SIZE / HandleUtility.
                GetHandleSize(t_targetPos);

            Rect t_targetRect = new Rect
                (
                    new Vector2(t_targetPos.x - TILE_SIZE / 4 / HandleUtility.
                GetHandleSize(t_targetPos), t_targetPos.y - TILE_SIZE / 4 / HandleUtility.
                GetHandleSize(t_targetPos)),
                    t_drawCubeSize / 2
                );

            // Begin Drawing SceneGUI elements.
            #region DrawGUI
            Handles.BeginGUI();
            Handles.DrawSolidRectangleWithOutline(t_targetRect, Color.green, Color.blue);
            //Handles.color = Color.blue;
            //         Handles.DrawWireCube(t_targetPos, t_drawCubeSize);
            foreach (GameObject obj in t_target.linkedObjects)
            {
                if (obj == null) { continue; }

                // The linked object's world position to GUI screen position.
                Vector2 t_objPos = HandleUtility.WorldToGUIPoint(obj.transform.position);

                Rect t_objRect = new Rect
                    (
                        new Vector2(t_objPos.x - TILE_SIZE / 4 / HandleUtility.
                    GetHandleSize(t_objPos), t_objPos.y - TILE_SIZE / 4 / HandleUtility.
                    GetHandleSize(t_objPos)),
                        t_drawCubeSize / 2
                    );

                Handles.DrawSolidRectangleWithOutline(t_objRect, Color.red, Color.black);

                Handles.color = Color.green;
                Handles.DrawDottedLine(t_targetPos, t_objPos, t_dottedLineSize);
                Handles.color = Color.red;
                Handles.DrawWireCube(t_objPos, t_drawCubeSize);
            }
            Handles.EndGUI();
            #endregion
        }

        private void ToggleGizmos()
        {
            m_gizmosToggled = !m_gizmosToggled;
            EditorPrefs.SetBool(GIZMO_TOGGLED_KEY, m_gizmosToggled);
            SceneView.RepaintAll();
        }
    }
}