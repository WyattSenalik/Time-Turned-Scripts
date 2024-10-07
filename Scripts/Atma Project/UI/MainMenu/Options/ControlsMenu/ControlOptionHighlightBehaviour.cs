using UnityEngine;

using TMPro;
// Original Authors - Wyatt Senalik

namespace Atma.UI
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(NavigatableMenuOption))]
    [RequireComponent(typeof(TextMeshProUGUI))]
    public sealed class ControlOptionHighlightBehaviour : MonoBehaviour
    {
        private TextMeshProUGUI m_textMesh = null;

        [SerializeField] private Color m_highlightColor = new Color(0.8f, 0.6980392f, 0.2392157f);
        [SerializeField] private Color m_unhighlightColor = new Color(0.854902f, 0.8745098f, 0.9019608f);

        private NavigatableMenuOption m_navMenuOpt = null;
        private bool m_isSubbed = false;


        private void Awake()
        {
            m_textMesh = GetComponent<TextMeshProUGUI>();
            m_navMenuOpt = GetComponent<NavigatableMenuOption>();
            #region Asserts
            //CustomDebug.AssertComponentIsNotNull(m_textMesh, this);
            //CustomDebug.AssertComponentIsNotNull(m_navMenuOpt, this);
            #endregion Asserts
        }
        private void Start()
        {
            ToggleSubscriptions(true);
            if (m_navMenuOpt.isHighlighted)
            {
                Highlight();
            }
        }
        private void OnDestroy()
        {
            ToggleSubscriptions(false);
        }


        private void ToggleSubscriptions(bool cond)
        {
            if (m_isSubbed == cond) { return; }
            m_isSubbed = cond;

            m_navMenuOpt?.onHighlighted.ToggleSubscription(Highlight, cond);
            m_navMenuOpt?.onUnhighlighted.ToggleSubscription(Unhighlight, cond);
        }
        private void Highlight()
        {
            m_textMesh.color = m_highlightColor;
        }
        private void Unhighlight()
        {
            m_textMesh.color = m_unhighlightColor;
        }
    }
}