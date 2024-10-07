using UnityEngine;

using TMPro;
// Original Authors - Wyatt Senalik

namespace Atma.UI
{
    /// <summary>
    /// Displays the max time of the room.
    /// </summary>
    [RequireComponent(typeof(TextMeshProUGUI))]
    public sealed class EndTimeDisplay : MonoBehaviour
    {
        [SerializeField, Min(0)] private int m_decimalPlaces = 0;

        private LevelOptions m_levelOptions = null;

        private TextMeshProUGUI m_textMesh = null;


        // Domestic Initialization
        private void Awake()
        {
            m_textMesh = GetComponent<TextMeshProUGUI>();
            #region Asserts
            //CustomDebug.AssertComponentIsNotNull(m_textMesh, this);
            #endregion Asserts
        }
        // Foreign Initialization
        private void Start()
        {
            m_levelOptions = LevelOptions.instance;
            #region Asserts
            //CustomDebug.AssertSingletonIsNotNull(m_levelOptions, this);
            #endregion Asserts
        }
        private void Update()
        {
            if (m_levelOptions.noTimeLimit)
            {
                m_textMesh.text = "";
                return;
            }
            m_textMesh.text = GetTextContent();
        }


        private string GetTextContent()
        {
            return m_levelOptions.time.ToString($"n{m_decimalPlaces}");
        }
    }
}