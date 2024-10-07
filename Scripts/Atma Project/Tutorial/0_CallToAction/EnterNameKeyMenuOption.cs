using System.Collections;
using UnityEngine;

using TMPro;

using Atma.UI;
using Helpers.Extensions;
using Helpers.Animation.BetterCurve;
// Original Authors - Bryce and Wyatt

namespace Atma.Tutorial
{
    /// <summary>
    /// When the navigatable option is selected, this will tell the input field to add the corresponding character
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(NavigatableMenuOption))]
    public sealed class EnterNameKeyMenuOption : MonoBehaviour
    {
        public UISoundController soundCont
        {
            get
            {
                if (m_soundCont == null)
                {
                    m_soundCont = UISoundController.GetInstanceSafe(this);
                }
                return m_soundCont;
            }
        }
        public ButtonsAddText buttonsAddText
        {
            get
            {
                if (m_buttonsAddText == null)
                {
                    m_buttonsAddText = ButtonsAddText.GetInstanceSafe(this);
                }
                return m_buttonsAddText;
            }
        }
        public ConfirmNameInput confirmNameInput
        {
            get
            {
                if (m_confirmNameInput == null)
                {
                    m_confirmNameInput = PlayerSingleton.GetInstanceSafe(this).GetComponentSafe<ConfirmNameInput>(this);
                }
                return m_confirmNameInput;
            }
        }

        [SerializeField] private eEnterNameKeyType m_keyType = eEnterNameKeyType.Char;
        [SerializeField] private ColorCurveSO m_highlightColorCurve = null;

        private UISoundController m_soundCont = null;
        private ButtonsAddText m_buttonsAddText = null;
        private ConfirmNameInput m_confirmNameInput = null;

        private NavigatableMenuOption m_option = null;
        private TextMeshProUGUI m_displayCharTextMeshPro = null;

        private bool m_isColorCoroutActive = false;
        private Coroutine m_colorCorout = null;

        private void Awake()
        {
            m_option = this.GetComponentSafe<NavigatableMenuOption>(this);
            m_displayCharTextMeshPro = GetComponentInChildren<TextMeshProUGUI>(true);
        }
        private void OnEnable()
        {
            ToggleSubscriptions(true);
        }
        private void OnDisable()
        {
            if (m_isColorCoroutActive)
            {
                m_isColorCoroutActive = false;
                StopCoroutine(m_colorCorout);

                Color t_curColor = m_highlightColorCurve.Evaluate(0.0f);
                m_displayCharTextMeshPro.color = t_curColor;
            }

            ToggleSubscriptions(false);
        }


        private void ToggleSubscriptions(bool toggle)
        {
            if (m_option != null)
            {
                m_option.onChosen.ToggleSubscription(OnChosen, toggle);
                m_option.onHighlighted.ToggleSubscription(OnHighlighted, toggle);
                m_option.onUnhighlighted.ToggleSubscription(OnUnhighlighted, toggle);
            }
        }
        private void OnChosen()
        {
            switch (m_keyType)
            {
                case eEnterNameKeyType.Char: 
                    buttonsAddText.AddCharToText(m_displayCharTextMeshPro.text[0].ToString());
                    break;

                case eEnterNameKeyType.BackSpace: 
                    buttonsAddText.RemoveCharFromText();
                    break;

                case eEnterNameKeyType.Enter: 
                    confirmNameInput.SubmitName(); 
                    break;

                default: 
                    CustomDebug.UnhandledEnum(m_keyType, this);
                    break;
            }
        }
        private void OnHighlighted()
        {
            StartColorTransitionCorout(false);
            soundCont.PlayMenuHighlightSound();
        }
        private void OnUnhighlighted()
        {
            StartColorTransitionCorout(true);
        }

        private void StartColorTransitionCorout(bool isReversed)
        {
            if (m_isColorCoroutActive)
            {
                StopCoroutine(m_colorCorout);
            }

            m_colorCorout = StartCoroutine(ColorTransitionCorout(isReversed));
        }
        private IEnumerator ColorTransitionCorout(bool isReversed)
        {
            m_isColorCoroutActive = true;
            
            float t_elapsedTime = 0.0f;
            float t_endTime = m_highlightColorCurve.GetEndTime();
            Color t_curColor;
            while (t_elapsedTime < t_endTime)
            {
                float t_correctedTime = isReversed ? t_endTime - t_elapsedTime : t_elapsedTime;
                t_curColor = m_highlightColorCurve.Evaluate(t_correctedTime);
                m_displayCharTextMeshPro.color = t_curColor;
                yield return null;
                t_elapsedTime += Time.deltaTime;
            }
            t_curColor = m_highlightColorCurve.Evaluate(isReversed ? 0.0f : t_endTime);
            m_displayCharTextMeshPro.color = t_curColor;

            m_isColorCoroutActive = false;
        }
    }
    public enum eEnterNameKeyType { Char, Enter, BackSpace}
}
