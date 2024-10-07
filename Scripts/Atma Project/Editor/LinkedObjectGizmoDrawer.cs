using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using System.Linq;

namespace Atma.Editors
{
    public static class LinkedObjectGizmoDrawer
    {

        private const float TILE_SIZE = 80f;
        private const float DOTTED_LINE_SIZE = 5f;
        private static List<GameObject> m_selectedObjsList = new List<GameObject>();

        public static void OnSceneGUI(bool m_gizmosToggled, Object target)
        {
            if (!m_gizmosToggled) { return; }
            IActivator t_target = target as IActivator;
            Transform t_tarTrans = (t_target as MonoBehaviour).transform;
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
                    new Vector2(t_targetPos.x - TILE_SIZE / (4 * HandleUtility.
                GetHandleSize(t_targetPos)), t_targetPos.y - TILE_SIZE / 4 / HandleUtility.
                GetHandleSize(t_targetPos)),
                    t_drawCubeSize / 2
                );

            // Begin Drawing SceneGUI elements.
            #region DrawGUI
            Handles.BeginGUI();
            Handles.DrawSolidRectangleWithOutline(t_targetRect, Color.green, Color.blue);
            //Handles.color = Color.blue;
            //Handles.DrawWireCube(t_targetPos, t_drawCubeSize);
            object t_lOObject = target.GetType().GetProperty("linkedObjects").GetValue(target);
            List<GameObject> t_lOList = (t_lOObject as IReadOnlyList<GameObject>).ToList();
            foreach (GameObject obj in t_lOList)
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
            if (m_selectedObjsList == null) { return; }

            Handles.color = Color.cyan;
            foreach (GameObject t_go in m_selectedObjsList)
            {
                Vector2 t_goPosition = HandleUtility.WorldToGUIPoint(t_go.transform.position);

                Handles.DrawLine(t_targetPos, t_goPosition);
                Handles.DrawWireCube(t_goPosition, t_drawCubeSize);
            }

            Handles.EndGUI();
            SceneView.RepaintAll();
            #endregion
        }

        public static void DrawLineToGameObjects(List<GameObject> objList)
        {
            if (objList == null)
            {
                m_selectedObjsList = null;
            }
            else
            {
                m_selectedObjsList = objList;
            }
        }
    }

}