using System;
using UnityEngine;
// Original Authors - Wyatt Senalik

namespace Helpers.UI
{
    /// <summary>
    /// Specifications for calling the <see cref="Cursor.SetCursor(Texture2D, Vector2, CursorMode)"/> function.
    /// </summary>
    [Serializable]
    public sealed class CursorSpecs
    {
        public Texture2D texture => m_texture;
        public Vector2 hotspot => m_hotspot;
        public CursorMode cursorMode => m_cursorMode;

        [SerializeField] private Texture2D m_texture = null;
        [SerializeField] private Vector2 m_hotspot = Vector2.zero;
        [SerializeField] private CursorMode m_cursorMode = CursorMode.Auto;


        public CursorSpecs()
        {
            m_texture = null;
            m_hotspot = Vector2.zero;
            m_cursorMode = CursorMode.Auto;
        }
        public CursorSpecs(Texture2D texture, Vector2 hotspot, CursorMode cursorMode)
        {
            m_texture = texture;
            m_hotspot = hotspot;
            m_cursorMode = cursorMode;
        }


        public override bool Equals(object other)
        {
            if (other == null) { return false; }
            if (other is CursorSpecs t_otherSpecs)
            {
                return texture == t_otherSpecs.texture && hotspot == t_otherSpecs.hotspot && cursorMode == t_otherSpecs.cursorMode;
            }
            else
            {
                return false;
            }
        }
        public override string ToString()
        {
            return $"(Texture = {texture}; Hotspot = {hotspot}; CursorMode = {cursorMode})";
        }
    }
}