using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif
// Original Authors - Wyatt Senalik

namespace Helpers.Animation.BetterCurve
{
    public class TransparentMeshBlinkBCA : BetterCurveAnimation
    {
        // SerializeField to save the processing of GetComponentInChildren
        [SerializeField] private MeshRenderer[] m_meshesToBlink = new MeshRenderer[1];


        // Foreign Initialization
        protected override void Start()
        {
            base.Start();

            // Have all of the materials be clones so we don't edit
            // the actual assets
            foreach (MeshRenderer temp_mr in m_meshesToBlink)
            {
                for (int i = 0; i < temp_mr.materials.Length; ++i)
                {
                    Material temp_mat = temp_mr.materials[i];
                    temp_mr.materials[i] = new Material(temp_mat);
                }
            }
        }


        public void SetMeshRenderers(MeshRenderer[] meshRenderers)
        {
            m_meshesToBlink = meshRenderers;
        }

        protected override void TakeCurveAction(float curveValue)
        {
            // Curve value is alpha value.
            ApplyTransparencyToMeshes(curveValue);
        }

        private void ApplyTransparencyToMeshes(float alpha)
        {
            foreach (MeshRenderer temp_mr in m_meshesToBlink)
            {
                if (!temp_mr.enabled) { return; }
                foreach (Material temp_mat in temp_mr.materials)
                {
                    Color temp_matColor = temp_mat.color;
                    temp_matColor.a = alpha;
                    temp_mat.color = temp_matColor;
                }
            }
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(TransparentMeshBlinkBCA))]
    public class TransparentMeshBlinkEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            TransparentMeshBlinkBCA transparentMeshBlink = (TransparentMeshBlinkBCA)target;

            // When pressed, recalculate all the non-runtime properties
            if (GUILayout.Button("Find Renderers In Children"))
            {
                MeshRenderer[] foundRenderers =
                    transparentMeshBlink.GetComponentsInChildren<MeshRenderer>(true);
                transparentMeshBlink.SetMeshRenderers(foundRenderers);
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
#endif
}
