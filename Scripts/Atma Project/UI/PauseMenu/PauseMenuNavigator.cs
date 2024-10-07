using System;
using UnityEngine;
using UnityEngine.UI;

using TMPro;
using NaughtyAttributes;
using System.Collections.Generic;
using Helpers.Events;
using Helpers;
// Original Authors - Wyatt Senalik

namespace Atma.UI
{
    [DisallowMultipleComponent]
    public sealed class PauseMenuNavigator : MonoBehaviour
    {
        public SelectOption curSelectionOpt
        {
            get
            {
                #region Asserts
                //CustomDebug.AssertIndexIsInRange(m_curSelIndex, m_selectOptions, this);
                #endregion Asserts
                return m_selectOptions[m_curSelIndex];
            }
        }
        public int curSelIndex => m_curSelIndex;
        public int prevSelIndex => m_prevSelIndex;
        public IReadOnlyList<SelectOption> selectOptions => m_selectOptions;
        public Color defaultTextColor => m_defaultTextColor;
        public Color selectedTextColor => m_selectedTextColor;
        public IEventPrimer onSelectionUpdated => m_onSelectionUpdated;

        [SerializeField, Required] private Image m_topBar = null;
        [SerializeField, Required] private Animator m_selectIconAnimator = null;
        [SerializeField, Required] private RectTransform m_selectIconTrans = null;
        [SerializeField] private SelectOption[] m_selectOptions = new SelectOption[6];

        [SerializeField, AnimatorParam(nameof(m_selectIconAnimator))]
        private string m_restartTriggerParamName = "Restart"; 
        [SerializeField, Required] private Sprite m_defaultBarSpr = null;
        [SerializeField, Required] private Sprite m_selectedBarSpr = null;
        [SerializeField] private Vector2 m_defaultBarSize = new Vector2(512.0f, 62.0f);
        [SerializeField] private Vector2 m_selectedBarSize = new Vector2(572.0f, 62.0f);
        [SerializeField] private Color m_defaultTextColor = Color.white;
        [SerializeField] private Color m_selectedTextColor = Color.yellow;

        [SerializeField, Min(0.0f)] private float m_autoMoveDelay = 0.15f;
        [SerializeField, Min(0.0f)] private float m_delayBeforeAutoMove = 0.5f;

        [SerializeField] private MixedEvent m_onSelectionUpdated = new MixedEvent();

        private UISoundController m_soundMan = null;

        private int m_curSelIndex = 0;
        private int m_prevSelIndex = 0;

        private float m_curNavValue = 0.0f;
        private float m_prevNavTime = 0.0f;
        private bool m_isAutoNav = false;


        private void Awake()
        {
            #region Asserts
            //CustomDebug.AssertSerializeFieldIsNotNull(m_topBar, nameof(m_topBar), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_selectIconAnimator, nameof(m_selectIconAnimator), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_selectIconTrans, nameof(m_selectIconTrans), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_defaultBarSpr, nameof(m_defaultBarSpr), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_selectedBarSpr, nameof(m_selectedBarSpr), this);
            foreach (SelectOption t_selOpt in m_selectOptions)
            {
                t_selOpt.AssertFieldsAreNotNull();
            }
            #endregion Asserts
        }
        private void Start()
        {
            m_soundMan = UISoundController.GetInstanceSafe();
        }
        private void OnDisable()
        {
            ZeroOutNavigation();
        }
        private void Update()
        {
            // NOTE: CHANGE THIS TO USE MenuMover (new close in Helpers).
            if (m_curNavValue != 0.0f)
            {
                if (m_isAutoNav)
                {
                    float t_nextNavTime = m_prevNavTime + m_autoMoveDelay;
                    if (Time.unscaledTime >= t_nextNavTime)
                    {
                        m_prevNavTime = Time.unscaledTime;
                        MoveVerticalToNext();
                    }
                }
                else
                {
                    float t_nextNavTime = m_prevNavTime + m_delayBeforeAutoMove;
                    if (Time.unscaledTime >= t_nextNavTime)
                    {
                        m_prevNavTime = Time.unscaledTime;
                        m_isAutoNav = true;
                        MoveVerticalToNext();
                    }
                }
            }
        }


        public void NavigateVertical(float axisValue)
        {
            if (Mathf.Abs(axisValue) < 0.1f)
            {
                ZeroOutNavigation();
            }
            else
            {
                int t_curXDir = 0;
                if (m_curNavValue > 0.1f) { t_curXDir = 1; }
                else if (m_curNavValue < -0.1f) { t_curXDir = -1; }

                int t_newXDir = 0;
                if (axisValue > 0.1f) { t_newXDir = 1; }
                else if (axisValue < -0.1f) { t_newXDir = -1; }

                int t_xSum = Mathf.Abs(t_curXDir + t_newXDir);
                // If direction changed or went from nothing to something.
                // Sign sum of 2 means its going in the same direction.
                // Sign sum of 1 means it was stopped and is now going or vice versa.
                // Sign sum of 0 means its going in opposite directions.
                if (t_xSum < 2)
                {
                    m_curNavValue = axisValue;
                    m_prevNavTime = Time.unscaledTime;
                    MoveVerticalToNext();
                }
                else
                {
                    m_curNavValue = axisValue;
                }
            }
        }
        public void SetSelected(int selIndex)
        {
            ZeroOutNavigation();
            m_prevSelIndex = m_curSelIndex;
            m_curSelIndex = selIndex;
            UpdateToCurrentSelection();
        }
        public void ChooseCurSelection()
        {
            ZeroOutNavigation();
            SelectOption t_curOption = curSelectionOpt;
            t_curOption.menuOption.OnChosen();
        }
        public void HorizontalInputForCurSelection(float inputValue)
        {
            ZeroOutNavigation();
            SelectOption t_curOption = curSelectionOpt;
            t_curOption.menuOption.OnHorizontalInput(inputValue);
        }
        public void ZeroHorizontalInputForCurSelection()
        {
            curSelectionOpt.menuOption.OnHorizontalInputZeroed();
        }
        public void Vector2InputForMovingAndHorizontalInput(Vector2 inputVector)
        {
            NavigateVertical(inputVector.y);

            float t_absX = Mathf.Abs(inputVector.x);
            float t_absY = Mathf.Abs(inputVector.y);
            if (t_absX > 0.05f && t_absY < 0.05f)
            {
                HorizontalInputForCurSelection(inputVector.x);
            }
            else //if (t_absX <= 0.05f)
            {
                ZeroHorizontalInputForCurSelection();
            }
        }


        private void MoveDown()
        {
            m_prevSelIndex = m_curSelIndex;
            m_curSelIndex = (m_curSelIndex + 1) % m_selectOptions.Length;
            UpdateToCurrentSelection();
        }
        private void MoveUp()
        {
            m_prevSelIndex = m_curSelIndex;
            if (--m_curSelIndex < 0)
            {
                m_curSelIndex = m_selectOptions.Length - 1;
            }
            UpdateToCurrentSelection();
        }
        private void UpdateToCurrentSelection()
        {
            // Reset all options and then we will just turn on the current option.
            ResetAllOptions();
            SelectOption t_curOption = curSelectionOpt;
            t_curOption.textMesh.color = m_selectedTextColor;
            t_curOption.botBar.rectTransform.sizeDelta = m_selectedBarSize;
            t_curOption.botBar.sprite = m_selectedBarSpr;
            // Move the select icon and restart the animation for it.
            m_selectIconTrans.anchoredPosition = t_curOption.position;
            m_selectIconAnimator.SetTrigger(m_restartTriggerParamName);

            // Now need to also turn on the bar above this selection
            if (m_curSelIndex == 0)
            {
                // First in list, use the top bar.
                m_topBar.rectTransform.sizeDelta = m_selectedBarSize;
                m_topBar.sprite = m_selectedBarSpr;
            }
            else
            {
                // Use the option above this one
                int t_aboveIndex = m_curSelIndex - 1;
                #region Asserts
                //CustomDebug.AssertIndexIsInRange(t_aboveIndex, m_selectOptions, this);
                #endregion Asserts
                SelectOption t_aboveOption = m_selectOptions[t_aboveIndex];
                t_aboveOption.botBar.rectTransform.sizeDelta = m_selectedBarSize;
                t_aboveOption.botBar.sprite = m_selectedBarSpr;
            }

            t_curOption.menuOption.OnHighlighted();
            m_onSelectionUpdated.Invoke();
        }
        private void ResetAllOptions()
        {
            m_topBar.rectTransform.sizeDelta = m_defaultBarSize;
            m_topBar.sprite = m_defaultBarSpr;
            foreach (SelectOption t_opt in m_selectOptions)
            {
                t_opt.textMesh.color = m_defaultTextColor;
                t_opt.botBar.rectTransform.sizeDelta = m_defaultBarSize;
                t_opt.botBar.sprite = m_defaultBarSpr;

                t_opt.menuOption.OnUnhighlighted();
            }
        }

        private void MoveVerticalToNext()
        {
            if (m_curNavValue > 0.1f)
            {
                MoveUp();
            }
            else if (m_curNavValue < -0.1f)
            {
                MoveDown();
            }
        }
        private void ZeroOutNavigation()
        {
            m_curNavValue = 0.0f;
            m_isAutoNav = false;
            m_prevNavTime = float.NegativeInfinity;
        }



        [Serializable]
        public sealed class SelectOption
        {
            public TextMeshProUGUI textMesh => m_textMesh;
            public Image botBar => m_botBar;
            public PauseMenuOption menuOption => m_menuOption;
            public Vector2 position => m_position;

            [SerializeField, Required] private TextMeshProUGUI m_textMesh = null;
            [SerializeField, Required] private Image m_botBar = null;
            [SerializeField, Required] private PauseMenuOption m_menuOption = null;
            [SerializeField] private Vector2 m_position = Vector2.zero;


            public void AssertFieldsAreNotNull()
            {
                #region Asserts
                //CustomDebug.AssertSerializeFieldIsNotNull(m_textMesh, nameof(m_textMesh), this);
                //CustomDebug.AssertSerializeFieldIsNotNull(m_botBar, nameof(m_botBar), this);
                //CustomDebug.AssertSerializeFieldIsNotNull(m_menuOption, nameof(m_menuOption), this);
                #endregion Asserts
            }
        }
    }
}