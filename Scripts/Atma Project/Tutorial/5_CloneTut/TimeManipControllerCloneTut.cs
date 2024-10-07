using System.Collections;
using UnityEngine;
using UnityEngine.UI;

using NaughtyAttributes;

using Atma.UI;
using Helpers.UI;
using Timed;
using TMPro.Extension;
using TMPro;
using Helpers.Events;
// Original Authors - Wyatt Senalik

namespace Atma.Tutorial
{
    /// <summary>
    /// <see cref="ITimeManipController"/> for the clone tutorial. Doesn't allow the player to do anything but create a time clone when they are prompted to do so.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(BranchPlayerController))]
    [RequireComponent(typeof(ITimeRewinder))]
    public sealed class TimeManipControllerCloneTut : MonoBehaviour, ITimeManipController
    {
        public IEventPrimer onSkipToBegin => m_onSkipToBegin;
        public ITimeRewinder rewinder => m_timeRewinder;
        public bool isFirstPause { private set; get; }

        [SerializeField, Required] private TimeSlider m_timeSlider = null;
        [SerializeField, Required] private GameObject m_actionRestrictedUIPopup = null;
        [SerializeField, Required] private InitialGainChargeAnimationController m_initGainChargeAnimCont = null;
        [SerializeField] private ImageWithMaxAlpha[] m_imgsToFade = new ImageWithMaxAlpha[0];
        [SerializeField] private TextWithMaxAlpha[] m_textsToFade = new TextWithMaxAlpha[0];
        [SerializeField] private float m_solidTime = 2.0f;
        [SerializeField, NaughtyAttributes.Tag] private string m_timeSliderTag = "Timeline";
        [SerializeField] private MixedEvent m_onSkipToBegin = new MixedEvent();

        private UISoundController m_soundCont = null;
        private BranchPlayerController m_playerCont = null;
        private ITimeRewinder m_timeRewinder = null;

        private IEventPrimer m_onSliderValChanged = null;

        private eCloneTutState m_curState = eCloneTutState.Begin;

        private bool m_isFadePopupCoroutActive = false;
        private Coroutine m_fadePopupCorout = null;


        private void Awake()
        {
            #region Asserts
            //CustomDebug.AssertSerializeFieldIsNotNull(m_timeSlider, nameof(m_timeSlider), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_actionRestrictedUIPopup, nameof(m_actionRestrictedUIPopup), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_initGainChargeAnimCont, nameof(m_initGainChargeAnimCont), this);
            #endregion Asserts
            m_playerCont = GetComponent<BranchPlayerController>();
            m_timeRewinder = GetComponent<ITimeRewinder>();
            #region Asserts
            //CustomDebug.AssertComponentIsNotNull(m_playerCont, this);
            //CustomDebug.AssertIComponentIsNotNull(m_timeRewinder, this);
            #endregion Asserts
        }
        private void Start()
        {
            m_soundCont = UISoundController.GetInstanceSafe();

            m_onSliderValChanged = m_timeSlider.onSliderValChanged;
            m_onSliderValChanged.ToggleSubscription(OnSliderValChanged, true);
        }
        private void OnDestroy()
        {
            m_onSliderValChanged.ToggleSubscription(OnSliderValChanged, false);
        }



        public void BeginTimeManipulation(bool isFirstPause = false)
        {
            // Not allowed to do pause time at begin (we don't check for this anywhere else, because if this doesn't happen, nothing else can).
            if (m_curState == eCloneTutState.Begin)
            {
                ShowActionRestrictedPopup();
                return;
            }

            #region Asserts
            //CustomDebug.AssertIsTrueForComponent(!m_playerCont.timeRewinder.hasStarted, $"time to be not currently be manipulated before calling {nameof(BeginTimeManipulation)}.", this);
            #endregion Asserts
            m_playerCont.ForceBeginTimeManipulation(null, isFirstPause);
        }
        public void CreateTimeClone()
        {
            // Only allowed Player to create a time clone after they have stepped on the pressure plate.
            if (m_curState == eCloneTutState.Begin)
            {
                ShowActionRestrictedPopup();
                return;
            }

            m_playerCont.CreateTimeCloneAtCurrentTime();
        }

        public void SkipToBeginning()
        {
            // Only allowed to do change the pause time after clone has been created.
            if (m_curState != eCloneTutState.AfterCloneCreated)
            {
                ShowActionRestrictedPopup();
                return;
            }

            // If player is using mouse to interact w/ time slider, do nothing when they try inputting or pressing buttons.
            if (m_timeSlider.interactState != ITimeSlider.eInteractState.None) { return; }
            // Already at earliest time
            if (m_timeRewinder.curTime == m_timeRewinder.earliestTime) { return; }

            m_timeRewinder.ForceSetTime(m_timeRewinder.earliestTime);
            m_onSkipToBegin.Invoke();
        }
        public void Rewind()
        {
            // Only allowed to do change the pause time after clone has been created.
            if (m_curState != eCloneTutState.AfterCloneCreated)
            {
                ShowActionRestrictedPopup();
                return;
            }

            // If player is using mouse to interact w/ time slider, do nothing when they try inputting or pressing buttons.
            if (m_timeSlider.interactState != ITimeSlider.eInteractState.None)
            { return; }

            m_playerCont.RewindNavVelocity();
        }
        public void Play()
        {
            // Allowed to play at anytime (won't occur during begin).

            // If player is using mouse to interact w/ time slider, do nothing when they try inputting or pressing buttons.
            if (m_timeSlider.interactState != ITimeSlider.eInteractState.None)
            { return; }

            // Need to wait for init anim to be done if its first one.
            if (!m_initGainChargeAnimCont.hasFinished) { return; }

            m_playerCont.TryEndTimeManipulation();
        }
        public void Pause()
        {
            // Allowed to pause at anytime (but won't occur until after clone created).

            // If player is using mouse to interact w/ time slider, do nothing when they try inputting or pressing buttons.
            if (m_timeSlider.interactState != ITimeSlider.eInteractState.None)
            { return; }

            m_playerCont.PauseNavVelocity();
        }
        public void FastForward()
        {
            // Only allowed to do change the pause time after clone has been created.
            if (m_curState != eCloneTutState.AfterCloneCreated)
            {
                ShowActionRestrictedPopup();
                return;
            }

            // If player is using mouse to interact w/ time slider, do nothing when they try inputting or pressing buttons.
            if (m_timeSlider.interactState != ITimeSlider.eInteractState.None)
            { return; }

            m_playerCont.FastForwardNavVelocity();
        }
        public void SkipToEnd()
        {
            // Only allowed to do change the pause time after clone has been created.
            if (m_curState != eCloneTutState.AfterCloneCreated)
            {
                ShowActionRestrictedPopup();
                return;
            }

            // If player is using mouse to interact w/ time slider, do nothing when they try inputting or pressing buttons.
            if (m_timeSlider.interactState != ITimeSlider.eInteractState.None)
            { return; }

            m_timeRewinder.ForceSetTime(m_timeRewinder.farthestTime);
        }

        public void SetCurState(eCloneTutState state)
        {
            m_curState = state;
        }


        private void ShowActionRestrictedPopup()
        {
            m_actionRestrictedUIPopup.SetActive(true);
            m_soundCont.PlayActionRestrictedSound();
            UpdateImgAndTextAlphaValues(0.0f);
            if (m_isFadePopupCoroutActive)
            {
                StopCoroutine(m_fadePopupCorout);
            }
            m_fadePopupCorout = StartCoroutine(FadeActionRestrictedPopupCorout());
        }
        private IEnumerator FadeActionRestrictedPopupCorout()
        {
            m_isFadePopupCoroutActive = true;

            yield return new WaitForSeconds(m_solidTime);
            float t = 0.0f;
            while (t < 1.0f)
            {
                UpdateImgAndTextAlphaValues(t);

                yield return null;
                t += Time.deltaTime;
            }
            UpdateImgAndTextAlphaValues(1.0f);
            m_actionRestrictedUIPopup.SetActive(false);

            m_isFadePopupCoroutActive = false;
        }
        private void UpdateImgAndTextAlphaValues(float t)
        {
            foreach (ImageWithMaxAlpha t_imgAndAlpha in m_imgsToFade)
            {
                Image t_img = t_imgAndAlpha.img;
                float t_maxA = t_imgAndAlpha.maxAlpha;

                Color t_col = t_img.color;
                t_col.a = Mathf.Lerp(t_maxA, 0.0f, t);
                t_img.color = t_col;
            }
            foreach (TextWithMaxAlpha t_txtAndAlpha in m_textsToFade)
            {
                TextMeshProUGUI t_txt = t_txtAndAlpha.text;
                float t_maxA = t_txtAndAlpha.maxAlpha;

                Color t_col = t_txt.color;
                t_col.a = Mathf.Lerp(t_maxA, 0.0f, t);
                t_txt.color = t_col;
            }
        }

        private void OnSliderValChanged()
        {
            // Is the player clicking on the slider?
            // If yes, is the time rewinder not currently rewinding?
            // That means the player is clicking when we are not trying to create a clone.
            // The player is only allowed to do that after they've created the clone.
            if (m_timeSlider.interactState != ITimeSlider.eInteractState.None &&
                m_timeRewinder.navigationDir == 0.0f &&
                m_curState != eCloneTutState.AfterCloneCreated)
            {
                m_timeSlider.slider.value = 1.0f;
                m_timeRewinder.ForceSetTime(m_timeRewinder.farthestTime);

                ShowActionRestrictedPopup();
            } 
        }


        public enum eCloneTutState { Begin, CloneCreating, AfterCloneCreated }
    }



}