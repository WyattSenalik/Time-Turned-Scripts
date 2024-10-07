using UnityEngine;
using UnityEngine.Rendering;

namespace Atma
{
    /// <summary>
    /// Has the attached SortingGroup ALWAYS have the same values as the attached SpriteRenderer.
    /// Created because you can't set a SortingGroup's variables from an animation.
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(SortingGroup))]
    public sealed class SortingGroupSyncer : MonoBehaviour
    {
        private SpriteRenderer m_sprRend = null;
        private SortingGroup m_sortGrp = null;

        private void Awake()
        {
            m_sprRend = GetComponent<SpriteRenderer>();
            m_sortGrp = GetComponent<SortingGroup>();
            #region Asserts
            //CustomDebug.AssertComponentIsNotNull(m_sprRend, this);
            //CustomDebug.AssertComponentIsNotNull(m_sortGrp, this);
            #endregion Asserts
        }
        private void Update()
        {
            m_sortGrp.sortingLayerID = m_sprRend.sortingLayerID;
            m_sortGrp.sortingOrder = m_sprRend.sortingOrder;
        }
    }
}