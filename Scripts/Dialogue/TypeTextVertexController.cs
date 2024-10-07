using UnityEngine;

using TMPro;

using Helpers.Events;
using static Dialogue.TypeWriter;
// Original Authors - Wyatt Senalik

namespace Dialogue
{
    /// <summary>
    /// Centralized controller for changing the TextMesh's vertices of the TextMesh that is being typed to.
    /// 
    /// This fixes a problem where if multiple things are trying to change the text mesh, it was only doing the one that ran 2nd in the execution order.
    /// </summary>
    [RequireComponent(typeof(TypeWriter))]
    [DisallowMultipleComponent]
    public sealed class TypeTextVertexController : MonoBehaviour
    {
        private const bool IS_DEBUGGING = false;

        public IEventPrimer<TextMeshUpdatedEventData> onTextMeshUpdated => m_onTextMeshUpdated;
        public IEventPrimer<VertexControllerUpdateEventData> onVertexContUpdate => m_onVertexContUpdate;
        public TextMeshProUGUI textMesh => m_textMesh;

        [SerializeField] private MixedEvent<TextMeshUpdatedEventData> m_onTextMeshUpdated = new MixedEvent<TextMeshUpdatedEventData>();
        [SerializeField] private MixedEvent<VertexControllerUpdateEventData> m_onVertexContUpdate = new MixedEvent<VertexControllerUpdateEventData>();

        private TypeWriter m_typeWriter = null;
        private TextMeshProUGUI m_textMesh = null;

        private Vector3[] m_vertices = null;
        private bool m_isPositionDirty = false;
        private Color32[] m_colors = null;
        private bool m_isColorDirty = false;
        private int m_mostRecentCharIndex = 0;
        private bool m_wasTextUpdateCalled = false;


        private void Awake()
        {
            m_typeWriter = GetComponent<TypeWriter>();
            #region Asserts
            //CustomDebug.AssertComponentIsNotNull(m_typeWriter, this);
            #endregion Asserts
        }
        private void Start()
        {
            m_textMesh = m_typeWriter.typeText;
            #region Asserts
            //CustomDebug.AssertIsTrueForComponent(m_textMesh != null, $"TypeWriter ({m_typeWriter}) to have its {nameof(m_typeWriter.typeText)} specified.", this);
            #endregion Asserts

            m_typeWriter.onTextChanged.ToggleSubscription(OnTypeWriterTextUpdate, true);
        }
        private void OnDestroy()
        {
            m_typeWriter.onTextChanged.ToggleSubscription(OnTypeWriterTextUpdate, false);
        }
        // Called onTextChanged (every frame after the text has finished being changed for the type writer).
        private void OnTypeWriterTextUpdate(TextChangedEventData eventData)
        {
            m_wasTextUpdateCalled = true;
            m_mostRecentCharIndex = eventData.curCharIndex;

            ForceUpdateMeshAndResetOffsets(eventData.curCharIndex);

            m_onTextMeshUpdated.Invoke(new TextMeshUpdatedEventData(eventData));
        }
        // OnTypeWriterTextUpdate will only happen every frame when the TypeWriter is typing while LateUpdate will simply happen every frame.
        private void LateUpdate()
        {
            TMP_TextInfo t_textInfo;
            if (!m_wasTextUpdateCalled)
            {
                ForceUpdateMeshAndResetOffsets(m_mostRecentCharIndex);
            }
            // Reset if it was called for next frame.
            m_wasTextUpdateCalled = false;

            m_onVertexContUpdate.Invoke(new VertexControllerUpdateEventData(m_mostRecentCharIndex));

            // Gotta set color before position. Found out from testing. Unsure why.
            if (m_isColorDirty)
            {
                m_textMesh.mesh.colors32 = m_colors;
                m_isColorDirty = false;
            }
            if (m_isPositionDirty)
            {
                t_textInfo = m_textMesh.textInfo;
                for (int i = 0; i < t_textInfo.meshInfo.Length; ++i)
                {
                    TMP_MeshInfo t_meshInfo = t_textInfo.meshInfo[i];
                    // Need to check for null mesh because the generated mesh gets deleted by the typewriter because otherwise sprites will popup as this updates the mesh even though they aren't in the text any longer.
                    if (t_meshInfo.mesh != null)
                    {
                        t_meshInfo.mesh.vertices = t_meshInfo.vertices;
                        m_textMesh.UpdateGeometry(t_meshInfo.mesh, i);
                    }
                }
                m_isPositionDirty = false;
            }
        }


        public void AddOffsetToVertex(int index, Vector3 vertexOffset)
        {
            if (index >= m_vertices.Length)
            {
                // Used to be an assert, but after we started deleting the submesh, this now just happens sometimes.
                return;
            }
            m_vertices[index] += vertexOffset;

            m_isPositionDirty = true;
        }
        public TMP_CharacterInfo GetCharacterInfo(int index)
        {
            TMP_TextInfo t_textInfo = m_textMesh.textInfo;
            return t_textInfo.characterInfo[index];
        }
        public Vector3 GetVertex(int index)
        {
            #region Asserts
            //CustomDebug.AssertIndexIsInRange(index, m_vertices, this);
            #endregion Asserts
            return m_vertices[index];
        }
        public int ConvertCharIndexToVertexIndex(int charIndex)
        {
            TMP_TextInfo t_textInfo = m_textMesh.textInfo;
            TMP_CharacterInfo t_charInfo = t_textInfo.characterInfo[charIndex];
            return t_charInfo.vertexIndex;
        }
        public void SetVertexColor(int index, Color color)
        {
            #region Asserts
            //CustomDebug.AssertIndexIsInRange(index, m_colors, this);
            #endregion Asserts
            m_colors[index] = color;

            m_isColorDirty = true;
        }
        public void SetCharacterColor(int charIndex, Color color)
        {
            TMP_TextInfo t_textInfo = m_textMesh.textInfo;
            TMP_CharacterInfo t_charInfo = t_textInfo.characterInfo[charIndex];

            for (int i = 0; i < 4; ++i)
            {
                SetVertexColor(t_charInfo.vertexIndex + i, color);
            }
        } 


        private void ForceUpdateMeshAndResetOffsets(int charIndex)
        {
            m_textMesh.ForceMeshUpdate();
            TMP_TextInfo t_textInfo = m_textMesh.textInfo;
            TMP_CharacterInfo t_charInfo = t_textInfo.characterInfo[charIndex];
            if (!t_charInfo.isVisible) { return; }
            // Create a new list that is same size as the vertices array
            m_vertices = t_textInfo.meshInfo[t_charInfo.materialReferenceIndex].vertices;
            m_colors = t_textInfo.meshInfo[t_charInfo.materialReferenceIndex].colors32;
        }


        public sealed class TextMeshUpdatedEventData
        {
            public TextChangedEventData textChangedEventData { get; private set; }
            public string newLiteralText => textChangedEventData.newLiteralText;
            public string newText => textChangedEventData.newText;
            public int curCharIndex => textChangedEventData.curCharIndex;
            public char curChar => textChangedEventData.curChar;
            public float curCharDelay => textChangedEventData.curCharDelay;
            public float percentWaitUntilTextChar => textChangedEventData.percentWaitUntilTextChar;

            public TextMeshUpdatedEventData(TextChangedEventData textChangedEventData)
            {
                this.textChangedEventData = textChangedEventData;
            }
        }
        public sealed class VertexControllerUpdateEventData
        { 
            public int mostRecentlyTypedCharIndex { get; private set; }

            public VertexControllerUpdateEventData(int mostRecentlyTypedCharIndex)
            {
                this.mostRecentlyTypedCharIndex = mostRecentlyTypedCharIndex;
            }
        }
    }
}