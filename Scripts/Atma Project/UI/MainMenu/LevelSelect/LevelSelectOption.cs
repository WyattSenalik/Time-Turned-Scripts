using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

using NaughtyAttributes;
using TMPro;

using Atma.Translation;
// Original Authors - Wyatt Senalik

namespace Atma.UI
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(RectTransform))]
    public sealed class LevelSelectOption : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
    {
        public TextMeshProUGUI textMesh => m_textMesh;
        public IReadOnlyList<Image> sideWallImages => m_sideWallImages;
        public IReadOnlyList<Image> cornerWallImages => m_cornerWallImages;
        public IReadOnlyList<Image> additionalGrayableImages => m_additionalGrayableImages;
        public IEnumerable<Image> allGrayableImages
        {
            get
            {
                return m_sideWallImages.Concat(m_cornerWallImages).Concat(m_additionalGrayableImages);
            }
        }
        public Material grayableMat => m_grayableMat;
        public Sprite sideWallDefault => m_sideWallDefault;
        public Sprite cornerWallDefault => m_cornerWallDefault;
        public Sprite sideWallHighlight => m_sideWallHighlight;
        public Sprite cornerWallHighlight => m_cornerWallHighlight;
        public Color textDefaultColor => m_textDefaultColor;
        public Color textHighlightColor => m_textHighlightColor;
        public Material runtimeMat => m_runtimeMat;
        public LevelSelectOptionsMenu levelSelOptMenu { get; set; } = null;
        public RectTransform rectTrans { get; private set; } = null;
        public bool isSelectable { get; private set; } = true;
        public bool isHighlighted { get; private set; } = false;

        [SerializeField, Required] private TextMeshProUGUI m_textMesh = null;
        [SerializeField] private Image[] m_sideWallImages = new Image[0];
        [SerializeField] private Image[] m_cornerWallImages = new Image[0];
        [SerializeField] private Image[] m_additionalGrayableImages = new Image[0];
        [SerializeField, Required] private Image m_fadeBG = null;

        [SerializeField, Required] private Material m_grayableMat = null;
        [SerializeField, Required] private Sprite m_sideWallDefault = null;
        [SerializeField, Required] private Sprite m_cornerWallDefault = null;
        [SerializeField, Required] private Sprite m_sideWallHighlight = null;
        [SerializeField, Required] private Sprite m_cornerWallHighlight = null;
        [SerializeField] private Color m_textDefaultColor = Color.white;
        [SerializeField] private Color m_textHighlightColor = Color.yellow;
        [SerializeField] private Color m_bgDefaultColor = Color.blue;
        [SerializeField] private Color m_bgGrayColor = Color.gray;

        private Material m_runtimeMat = null;
        private eLevelCompleteState m_levelState = eLevelCompleteState.Cleared;


        private void Awake()
        {
            rectTrans = GetComponent<RectTransform>();
            #region Asserts
            //CustomDebug.AssertComponentIsNotNull(rectTrans, this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_textMesh, nameof(m_textMesh), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_grayableMat, nameof(m_grayableMat), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_sideWallDefault, nameof(m_sideWallDefault), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_cornerWallDefault, nameof(m_cornerWallDefault), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_sideWallHighlight, nameof(m_sideWallHighlight), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_cornerWallHighlight, nameof(m_cornerWallHighlight), this);
            #endregion Asserts
            m_runtimeMat = new Material(m_grayableMat);
            foreach (Image t_img in allGrayableImages)
            {
                t_img.material = m_runtimeMat;
            }
        }


        public void SetLevelName(string flavorName)
        {
            m_textMesh.text = flavorName;
            m_textMesh.ApplyFontToTextMeshForCurLanguage();
        }
        public void UpdateColor()
        {
            m_textMesh.color = isHighlighted ? m_textHighlightColor : m_textDefaultColor;

            switch (m_levelState)
            {
                case eLevelCompleteState.Unreached:
                {
                    m_runtimeMat.SetFloat(WorldGrayifier.GRAYSCALE_PERCENT_VAR_NAME, 1.0f);
                    m_fadeBG.color = m_bgGrayColor;
                    break;
                }
                case eLevelCompleteState.Reached:
                {
                    float t_grayValue = isHighlighted ? 0.0f : 1.0f;
                    m_runtimeMat.SetFloat(WorldGrayifier.GRAYSCALE_PERCENT_VAR_NAME, t_grayValue);
                    m_fadeBG.color = m_bgGrayColor;
                    break;
                }
                case eLevelCompleteState.Cleared:
                {
                    m_runtimeMat.SetFloat(WorldGrayifier.GRAYSCALE_PERCENT_VAR_NAME, 0.0f);
                    m_fadeBG.color = m_bgDefaultColor;
                    break;
                }
                default:
                {
                    CustomDebug.UnhandledEnum(m_levelState, this);
                    break;
                }
            }
        }

        public void SetReachedButNotCleared(string flavorName)
        {
            m_levelState = eLevelCompleteState.Reached;
            m_textMesh.enabled = true;
            isSelectable = true;
            SetLevelName(flavorName);
            UpdateColor();
        }
        public void SetUnreached()
        {
            m_levelState = eLevelCompleteState.Unreached;
            m_textMesh.enabled = false;
            isSelectable = false;
            UpdateColor();
        }
        public void SetCleared(string flavorName)
        {
            m_levelState = eLevelCompleteState.Cleared;
            m_textMesh.enabled = true;
            isSelectable = true;
            SetLevelName(flavorName);
            UpdateColor();
        }

        public void Highlight()
        {
            #region Asserts
            //CustomDebug.AssertIsTrueForComponent(isSelectable, $"only selectable options to be highlighted.", this);
            #endregion Asserts
            if (isHighlighted) { return; }
            isHighlighted = true;

            UpdateColor();
            UpdateWallImgs(m_sideWallHighlight, m_cornerWallHighlight);
        }
        public void Unhighlight()
        {
            if (!isHighlighted) { return; }
            isHighlighted = false;

            UpdateColor();
            UpdateWallImgs(m_sideWallDefault, m_cornerWallDefault);
        }


        public void OnPointerEnter(PointerEventData eventData)
        {
            if (SceneLoader.instance.isLoading) { return; }
            if (levelSelOptMenu.isParentMoving) { return; }
            levelSelOptMenu.SetSelected(this);
        }
        public void OnPointerClick(PointerEventData eventData)
        {
            if (SceneLoader.instance.isLoading) { return; }
            if (levelSelOptMenu.isParentMoving) { return; }
            levelSelOptMenu.SetSelected(this);
            levelSelOptMenu.Submit();
        }


        private void UpdateWallImgs(Sprite sideWallSprite, Sprite cornerWallSprite)
        {
            UpdateSideWallImgs(sideWallSprite);
            UpdateCornerWallImgs(cornerWallSprite);
        }
        private void UpdateSideWallImgs(Sprite sprite)
        {
            foreach (Image t_sideWallImg in m_sideWallImages)
            {
                t_sideWallImg.sprite = sprite;
            }
        }
        private void UpdateCornerWallImgs(Sprite sprite)
        {
            foreach (Image t_cornerWallImg in m_cornerWallImages)
            {
                t_cornerWallImg.sprite = sprite;
            }
        }
    }

    public enum eLevelCompleteState { Unreached, Reached, Cleared }
}