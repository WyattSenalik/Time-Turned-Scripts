using UnityEngine;
using UnityEditor;

using NaughtyAttributes;

using Helpers.Extensions;
using Helpers.Physics.Custom2DInt;
using Helpers.Physics.Custom2DInt.NavAI;
// Original Authors - Wyatt Senalik

namespace Atma
{
    [CustomEditor(typeof(NavGraphInt2D))]
    public sealed class NavGraphAtmaEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();
            if (GUILayout.Button("Update Mesh"))
            {
                UpdateNavMesh();
                serializedObject.ApplyModifiedProperties();
                SceneView.RepaintAll();
            }
        }


        private void UpdateNavMesh()
        {
            NavGraphInt2D t_navGraph = target as NavGraphInt2D;
            SerializedProperty t_gridPassableTable = serializedObject.FindProperty("m_gridPassableTable");

            // Set all tiles to be passable.
            for (int i = 0; i < t_navGraph.tileCount; ++i)
            {
                t_gridPassableTable.GetArrayElementAtIndex(i).boolValue = true;
            }

            // Mark tiles as impassable if there is a collider there.
            PushableBoxCollider[] t_colliders = FindObjectsOfType<PushableBoxCollider>();
            foreach (PushableBoxCollider t_col in t_colliders)
            {
                if (!t_col.isImmobile) { continue; }
                RectangleIntCollider t_rectCol = t_col.GetComponentSafe<RectangleIntCollider>();
                RectangleInt t_rectangle = t_rectCol.rectangle;
                BlockTilesRectangleEnters(t_navGraph, t_gridPassableTable, t_rectangle);
            }
            
        }

        private static void BlockTilesRectangleEnters(NavGraphInt2D navGraph, SerializedProperty gridPassableTable, RectangleInt rectangle)
        {
            // Going to do this the dumb way because it's easy and this will only run in the editor.
            RectangleInt t_graphBounds = navGraph.GetBoundsOfGraph();
            Vector2Int t_tileSize = navGraph.tileSize;
            for (int x = t_graphBounds.min.x + (t_tileSize.x / 2); x < t_graphBounds.max.x; x += t_tileSize.x)
            {
                for (int y = t_graphBounds.min.y + (t_tileSize.y / 2); y < t_graphBounds.max.y; y += t_tileSize.y)
                {
                    Vector2Int t_checkTileCenterPos = new Vector2Int(x, y);
                    if (CustomPhysics2DInt.IsPointInRectangle(rectangle, t_checkTileCenterPos))
                    {
                        int t_tileIndex = navGraph.GetIndexOfTileThatContainsIntPoint(t_checkTileCenterPos);
                        gridPassableTable.GetArrayElementAtIndex(t_tileIndex).boolValue = false;
                    }
                }
            }
        }
    }
}