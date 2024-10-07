using UnityEditor;
using UnityEngine;

using Helpers.Editor;
// Original Authors - Wyatt Senalik

namespace Timed.TimedComponentImplementations.Editor
{
    
    public sealed class TimedTimlineEditorWindow : EditorWindow
    {
        private const bool IS_DEBUGGING = false;

        private const float TWO_PI = 2 * Mathf.PI;
        private const int POINT_LOD = 12;
        private const float POINT_INC = TWO_PI / POINT_LOD;

        private static readonly Color BG_COLOR = new Color(70 / 256.0f, 70 / 256.0f, 70 / 256.0f);//new Color(56 / 256.0f, 56 / 256.0f, 56 / 256.0f);
        private static readonly Color THIN_LINE_COLOR = new Color(45 / 256.0f, 45 / 256.0f, 45 / 256.0f);
        private static readonly Color THICK_LINE_COLOR = new Color(70 / 256.0f, 70 / 256.0f, 70 / 256.0f);

        private Material mat
        {
            get
            {
                if (m_mat == null)
                {
                    Shader t_shader = Shader.Find("Hidden/Internal-Colored");
                    m_mat = new Material(t_shader);
                }
                return m_mat;
            }
        }
        private Material m_mat = null;
        private float m_pointSize = 2.0f;
        private float m_curPointSize = 5.0f;
        private eTimedRecorder m_recorderType = eTimedRecorder.Transform;
        private bool m_hasValidSelection = false;
        private string m_errorMsg = "";

        private void OnEnable()
        {
            Shader t_shader = Shader.Find("Hidden/Internal-Colored");
            m_mat = new Material(t_shader);
        }
        private void OnDisable()
        {
            DestroyImmediate(mat);
        }
        private void OnDestroy()
        {
            DestroyImmediate(mat);
        }

        [MenuItem("Tools/TimedTimeline")]
        public static void ShowWindow()
        {
            GetWindow<TimedTimlineEditorWindow>("Timed Timeline");
        }

        private void Update()
        {
            if (mouseOverWindow)
            {
                Repaint();
            }
        }

        public void OnGUI()
        {
            // Area to draw in.
            Rect t_drawRect = GUILayoutUtility.GetRect(10, 1000, 10, 1000);

            GUILayout.BeginVertical();
            if (!m_hasValidSelection)
            {
                GUIStyle t_style = new GUIStyle();
                t_style.normal.textColor = Color.red;
                GUILayout.Label(m_errorMsg, t_style);
            }
            // Type of recorder
            m_recorderType = (eTimedRecorder)EditorGUILayout.EnumPopup("Recorder Component", m_recorderType);
            // Data Point Size
            GUILayout.BeginHorizontal();
            GUILayout.Label("Data Point Size: ");
            m_pointSize = EditorGUILayout.Slider(m_pointSize, 0, 25);
            GUILayout.Space(10);
            // Current Time Point size
            GUILayout.Label("Cur Time Size: ");
            m_curPointSize = EditorGUILayout.Slider(m_curPointSize, 0, 25);
            GUILayout.EndHorizontal();
            //
            // Show the spawn time and furthest time of the currently selected object as well as
            // the current time.
            ShowCurrentObjectTimes();
            //
            // Data for the current time
            ITimedRecorderEditorGraphs t_graphs = m_recorderType.GetCorrespondingGraphs();
            if (Application.isPlaying)
            {
                t_graphs.DisplayCurrentData();
            }
            //
            GUILayout.EndVertical();

            if (Event.current.type == EventType.Repaint)
            {

                GUI.BeginClip(t_drawRect);
                GL.PushMatrix();

                GL.Clear(true, false, Color.black);
                mat.SetPass(0);

                // background
                GL.Begin(GL.QUADS);
                GL.Color(BG_COLOR);
                GL.Vertex3(0, 0, 0);
                GL.Vertex3(t_drawRect.width, 0, 0);
                GL.Vertex3(t_drawRect.width, t_drawRect.height, 0);
                GL.Vertex3(0, t_drawRect.height, 0);
                GL.End();

                // Draw scrapbook data.

                if (Application.isPlaying)
                {
                    m_hasValidSelection = t_graphs.DrawAllDataAsCurve(t_drawRect, m_pointSize, m_curPointSize, out m_errorMsg);
                }

                GL.PopMatrix();
                GUI.EndClip();
            }
        }

        private void ShowCurrentObjectTimes()
        {
            // Try to get the currently selected timedobject. If there is none, do nothing.
            if (!EditorSelection.TryGetEditorSelectedIComponent(out ITimedObject t_timedObj)) { return; }

            GUILayout.BeginHorizontal();
            // Spawn Time
            GUILayout.Label($"Spawn Time: {t_timedObj.spawnTime}");
            GUILayout.Space(10);
            // Current Time
            GUILayout.Label($"Current Time: {t_timedObj.curTime}");
            GUILayout.Space(10);
            // Furthest time
            GUILayout.Label($"Farthest Time: {t_timedObj.farthestTime}");
            GUILayout.EndHorizontal();
            // Slider for clarity.
            EditorGUILayout.Slider(t_timedObj.curTime, t_timedObj.spawnTime, t_timedObj.farthestTime);
        }
    }
}