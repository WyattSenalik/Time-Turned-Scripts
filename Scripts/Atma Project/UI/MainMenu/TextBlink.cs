// Original Authors - Wyatt Senalik
using UnityEngine;

using TMPro;

using Helpers.Animation.BetterCurve;

namespace Atma
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(TextMeshProUGUI))]
    public sealed class TextBlink : MonoBehaviour
    {
        [SerializeField] private ColorCurve m_blinkCurve = new ColorCurve();

        private TextMeshProUGUI m_textMesh = null;
        private float m_timeElapsed = 0.0f;


        private void Awake()
        {
            m_textMesh = GetComponent<TextMeshProUGUI>();
            #region Asserts
            //CustomDebug.AssertComponentIsNotNull(m_textMesh, this);
            #endregion Asserts
        }
        private void Update()
        {
            Color t_curColor = m_blinkCurve.Evaluate(m_timeElapsed % m_blinkCurve.GetEndTime());
            m_textMesh.color = t_curColor;
            m_timeElapsed += Time.deltaTime;
        }
    }
}