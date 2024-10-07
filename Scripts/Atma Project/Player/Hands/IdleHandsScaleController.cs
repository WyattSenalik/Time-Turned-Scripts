using UnityEngine;

using NaughtyAttributes;
// Original Authors - Wyatt Senalik

namespace Atma
{
    /// <summary>
    /// Sets x local scale of the transform to negative when the specified sprite renderer's flipX is true.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class IdleHandsScaleController : MonoBehaviour
    {
        [SerializeField, Required] private SpriteRenderer m_sprRend = null;


        private void Awake()
        {
            #region Asserts
            //CustomDebug.AssertSerializeFieldIsNotNull(m_sprRend, nameof(m_sprRend), this);
            #endregion Asserts
        }
        private void Update()
        {
            Vector3 t_newScale = Vector3.one;
            if (m_sprRend.flipX)
            {
                t_newScale.x = -1.0f;
            }
            transform.localScale = t_newScale;
        }
    }
}