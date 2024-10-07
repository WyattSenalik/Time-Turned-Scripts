using UnityEngine;

using static Dialogue.TypeTextVertexController;
// Original Authors - Wyatt Senalik

namespace Dialogue
{
    /// <summary>
    /// Adds a little bit of a drop to the last typed character to make it plop into place.
    /// </summary>
    [RequireComponent(typeof(TypeTextVertexController))]
    [DisallowMultipleComponent]
    public sealed class TypeCharDropAnimator : MonoBehaviour
    {
        private const bool IS_DEBUGGING = false;

        [SerializeField] private float m_startVertOffset = 15.0f;

        private TypeTextVertexController m_textVertexCont = null;
        private int m_prevVertexIndex = -1;
        private string m_prevLiteralText = "";


        private void Awake()
        {
            m_textVertexCont = GetComponent<TypeTextVertexController>();
            #region Asserts
            //CustomDebug.AssertComponentIsNotNull(m_textVertexCont, this);
            #endregion Asserts
        }
        private void Start()
        {
            m_textVertexCont.onTextMeshUpdated.ToggleSubscription(UpdateCurrentCharacterVerticalOffset, true);
        }
        private void OnDestroy()
        {
            m_textVertexCont?.onTextMeshUpdated.ToggleSubscription(UpdateCurrentCharacterVerticalOffset, false);
        }


        private void UpdateCurrentCharacterVerticalOffset(TextMeshUpdatedEventData eventData)
        {
            // No need to make a space drop. And actually, a space has no mesh, so it will actually mess up and occasionally move the 1st character (0th index) instead.
            if (eventData.curChar == ' ') { return; }
            int t_vertexIndex = m_textVertexCont.ConvertCharIndexToVertexIndex(eventData.curCharIndex);
            if (t_vertexIndex < m_prevVertexIndex)
            {
                if (m_prevLiteralText == eventData.newLiteralText)
                {
                    // No new text was started, so it was the result of some weird text mesh pro stuff.
                    return;
                }
            }
            Vector3 t_offset = new Vector3(0.0f, Mathf.Lerp(m_startVertOffset, 0.0f, eventData.percentWaitUntilTextChar), 0.0f);
            for (int i = 0; i < 4; ++i)
            {
                m_textVertexCont.AddOffsetToVertex(t_vertexIndex + i, t_offset);
            }

            m_prevVertexIndex = t_vertexIndex;
            m_prevLiteralText = eventData.newLiteralText;
        }
    }
}