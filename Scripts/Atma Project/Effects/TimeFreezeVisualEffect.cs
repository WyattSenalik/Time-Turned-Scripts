using System.Collections.Generic;
using UnityEngine;

using Helpers.Animation.BetterCurve;
using NaughtyAttributes;

namespace Atma
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(MeshFilter))]
    public sealed class TimeFreezeVisualEffect : MonoBehaviour
    {
        private const float TWO_PI = 2 * Mathf.PI;

        [SerializeField, Required] private MeshFilter m_innerMeshFilter = null;
        [SerializeField] private BetterCurve m_growthCurve = new BetterCurve();
        [SerializeField] private int m_polygonSides = 32;
        [SerializeField] private float m_thickness = 0.1f;
        [SerializeField] private float m_noiseWeight = 1.0f;
        [SerializeField] private float m_noiseSpeed = 1.0f;

        private MeshFilter m_meshFilter = null;
        private Mesh m_mesh = null;
        private Mesh m_innerMesh = null;
        private List<Vector3> m_outerPoints = new List<Vector3>();
        private List<Vector3> m_innerPoints = new List<Vector3>();
        private readonly List<Vector3> m_polygonPoints = new List<Vector3>();

        private float m_noiseTime = 0.0f;
        private float m_growthTime = 0.0f;


        private void Awake()
        {
            m_meshFilter = GetComponent<MeshFilter>();
            #region Asserts
            //CustomDebug.AssertComponentIsNotNull(m_meshFilter, this);

            //CustomDebug.AssertSerializeFieldIsNotNull(m_innerMeshFilter, nameof(m_innerMeshFilter), this);
            #endregion Asserts
            m_mesh = new Mesh();
            m_meshFilter.mesh = m_mesh;

            m_innerMesh = new Mesh();
            m_innerMeshFilter.mesh = m_innerMesh;
        }
        private void OnEnable()
        {
            m_noiseTime = 0.0f;
            m_growthTime = 0.0f;
        }


        private void Update()
        {
            if (m_growthTime <= 0.1f)
            {
                // For hiding effect when time is paused at t=0.
                m_mesh.Clear();
                m_innerMesh.Clear();
                m_noiseTime += Time.deltaTime * m_noiseSpeed;
                m_growthTime += Time.deltaTime;
                return;
            }
            if (m_growthTime >= m_growthCurve.GetEndTime())
            {
                gameObject.SetActive(false);
                return;
            }
            float t_radius = m_growthCurve.Evaluate(m_growthTime);
            DrawHollow(m_polygonSides, t_radius, t_radius - m_thickness);

            m_noiseTime += Time.deltaTime * m_noiseSpeed;
            m_growthTime += Time.deltaTime;
        }


        private void DrawHollow(int sides, float outerRadius, float innerRadius)
        {
            GetCircumferencePoints(sides, outerRadius, ref m_outerPoints);      
            GetCircumferencePoints(sides, innerRadius, ref m_innerPoints);
            for (int i = 0; i < sides; ++i)
            {
                Vector3 t_circleDir = m_outerPoints[i].normalized;
                float t_noise = Mathf.PerlinNoise(m_noiseTime + t_circleDir.x, m_noiseTime + t_circleDir.y);
                float t_randomness = t_noise * t_noise * m_noiseWeight * outerRadius;
                m_outerPoints[i] += t_circleDir * t_randomness;
                m_innerPoints[i] += t_circleDir * t_randomness;
            }
            m_polygonPoints.Clear();
            m_polygonPoints.AddRange(m_outerPoints);
            m_polygonPoints.AddRange(m_innerPoints);

            m_mesh.Clear();
            m_mesh.vertices = m_polygonPoints.ToArray();
            m_mesh.triangles = DrawHollowTriangles(m_polygonPoints);

            m_innerMesh.Clear();
            m_innerMesh.vertices = m_innerPoints.ToArray();
            m_innerMesh.triangles = DrawFilledTriangles(m_innerPoints);
        }
        private void GetCircumferencePoints(int sides, float radius, ref List<Vector3> points)
        {
            points.Clear();
            float t_circumferenceProgressPerStep = (float)1 / sides;
            float t_radianProgressPerStep = t_circumferenceProgressPerStep * TWO_PI;

            for (int i = 0; i < sides; i++)
            {
                float t_currentRadian = t_radianProgressPerStep * i;
                points.Add(new Vector3(Mathf.Cos(t_currentRadian) * radius, Mathf.Sin(t_currentRadian) * radius, 0));
            }
        }
        private int[] DrawHollowTriangles(List<Vector3> points)
        {
            int t_sides = points.Count / 2;

            List<int> t_newTriangles = new List<int>();
            for (int i = 0; i < t_sides; i++)
            {
                int t_outerIndex = i;
                int t_innerIndex = i + t_sides;

                //first triangle starting at outer edge i
                t_newTriangles.Add(t_outerIndex);
                t_newTriangles.Add(t_innerIndex);
                t_newTriangles.Add((i + 1) % t_sides);

                //second triangle starting at outer edge i
                t_newTriangles.Add(t_outerIndex);
                t_newTriangles.Add(t_sides + ((t_sides + i - 1) % t_sides));
                t_newTriangles.Add(t_outerIndex + t_sides);
            }
            return t_newTriangles.ToArray();
        }
        private int[] DrawFilledTriangles(List<Vector3> points)
        {
            int t_triangleAmount = points.Count - 2;
            List<int> t_newTriangles = new List<int>();
            for (int i = 0; i < t_triangleAmount; i++)
            {
                t_newTriangles.Add(0);
                t_newTriangles.Add(i + 2);
                t_newTriangles.Add(i + 1);
            }
            return t_newTriangles.ToArray();
        }
    }
}