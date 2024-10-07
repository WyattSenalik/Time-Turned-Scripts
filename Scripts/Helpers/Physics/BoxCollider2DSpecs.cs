using System;
using UnityEngine;
// Original Authors - Wyatt Senalik

namespace Helpers.Physics
{
    /// <summary>
    /// Specificiations for a BoxCollider2D that can be defined in the inspector without actually using a BoxCollider2D.
    /// </summary>
    [Serializable]
    public sealed class BoxCollider2DSpecs
    {
        public Vector2 offset { get => m_offset; set => m_offset = value; }
        public Vector2 size { get => m_size; set => m_size = value; }
        public float edgeRadius { get => m_edgeRadius; set => m_edgeRadius = value; }

        [SerializeField] private Vector2 m_offset = Vector2.zero;
        [SerializeField] private Vector2 m_size = Vector2.one;
        [SerializeField] private float m_edgeRadius = 0.0f;


        public BoxCollider2DSpecs()        {
            m_offset = Vector2.zero;
            m_size = Vector2.one;
            m_edgeRadius = 0.0f;
        }
        public BoxCollider2DSpecs(Vector2 offset, Vector2 size, float edgeRadius)        {
            m_offset = offset;
            m_size = size;
            m_edgeRadius = edgeRadius;
        }


        public void ApplyToCollider(BoxCollider2D collider)
        {
            collider.offset = offset;
            collider.size = size;
            collider.edgeRadius = edgeRadius;
        }
    }
}