using UnityEngine;
// Original Authors - Wyatt Senalik

namespace Timed.TimedComponentImplementations
{
    /// <summary>
    /// Moment for changing the sprite for a SpriteRenderer.
    /// </summary>
    public sealed class SpriteRendererMoment : IMoment<SpriteRendererMoment>
    {
        public float time => m_time;
        public SpriteRenderer renderer => m_renderer;
        public SpriteRendererMomentDoData doData => m_doData;
        public SpriteRendererMomentDoData undoData => m_undoData;

        private readonly float m_time = -1.0f;
        private readonly SpriteRenderer m_renderer = null;
        private readonly SpriteRendererMomentDoData m_doData = null;
        private readonly SpriteRendererMomentDoData m_undoData = null;


        public SpriteRendererMoment(float occurTime, SpriteRenderer sprRend, SpriteRendererMomentDoData newData, SpriteRendererMomentDoData oldData)
        {
            m_time = occurTime;
            m_renderer = sprRend;
            m_doData = newData;
            m_undoData = oldData;
            #region Asserts
            //CustomDebug.AssertIsTrue(sprRend != null, $"a non-null sprite renderer was given in the constructor.", nameof(SpriteRendererMoment));
            #endregion Asserts
        }

        public void Do()
        {
            m_renderer.enabled = doData.enabled;
            m_renderer.sprite = doData.sprite;
            m_renderer.flipX = doData.flipX;
            m_renderer.sortingOrder = doData.sortingOrder;
        }
        public void Undo()
        {
            m_renderer.enabled = undoData.enabled;
            m_renderer.sprite = undoData.sprite;
            m_renderer.flipX = undoData.flipX;
            m_renderer.sortingOrder = undoData.sortingOrder;
        }
        public SpriteRendererMoment Clone() => new SpriteRendererMoment(time, renderer, doData, undoData);
        public void Destroy(float destroyTime) { /* No cleanup needed? */ }
    }
}