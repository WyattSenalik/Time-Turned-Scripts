using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

using NaughtyAttributes;

using Helpers.Events;
using Timed;

using static Atma.UI.ITimeSlider;
// Original Authors - Wyatt Senalik

namespace Atma.UI
{
    /// <summary>
    /// Controls the time slider for the video controls.
    /// Allows player to click and drag the slider which will update the time.
    /// As well as updating the time slider if using keyboard controls.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class TimeSlider : MonoBehaviour, ITimeSlider
    {
        private const bool IS_DEBUGGING = false;

        public PlayerInput playerInp => m_playerInp;
        public string timeManipInputMapName => m_timeManipInputMapName;
        public Slider slider => m_timelineSlider;
        public IEventPrimer onSliderValChanged => m_onSliderValChanged;
        public GlobalTimeManager timeMan => m_timeMan;
        public LevelOptions levelOpts => m_levelOpts;
        public BranchPlayerController playerCont => m_playerCont;
        public ITimeRewinder timeRewinder => m_timeRewinder;
        public eInteractState interactState => m_interactState;

        [SerializeField, Required] private PlayerInput m_playerInp = null;
        [SerializeField] private string m_timeManipInputMapName = "TimeManipulation";
        [SerializeField, Required] private Slider m_timelineSlider = null;
        [SerializeField] private MixedEvent m_onSliderValChanged = new MixedEvent();

        private GlobalTimeManager m_timeMan = null;
        private LevelOptions m_levelOpts = null;

        private BranchPlayerController m_playerCont = null;
        private ITimeRewinder m_timeRewinder = null;

        private eInteractState m_interactState = eInteractState.None;
        private int m_prevSliderSetFrame = int.MinValue;


        private void Awake()
        {
            #region Asserts
            //CustomDebug.AssertSerializeFieldIsNotNull(m_playerInp, nameof(m_playerInp), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_timelineSlider, nameof(m_timelineSlider), this);
            #endregion Asserts
        }
        private void Start()
        {
            m_timeMan = GlobalTimeManager.instance;
            m_levelOpts = LevelOptions.instance;
            #region Asserts
            //CustomDebug.AssertSingletonIsNotNull(m_timeMan, this);
            //CustomDebug.AssertSingletonIsNotNull(m_levelOpts, this);
            #endregion Asserts
            // Get time rewinder from player.
            PlayerSingleton t_player = PlayerSingleton.instance;
            #region Asserts
            //CustomDebug.AssertSingletonMonoBehaviourIsNotNull(t_player, this);
            #endregion Asserts
            m_playerCont = t_player.GetComponent<BranchPlayerController>();
            m_timeRewinder = t_player.GetComponent<ITimeRewinder>();
            #region Asserts
            //CustomDebug.AssertComponentOnOtherIsNotNull(m_playerCont, t_player.gameObject, this);
            //CustomDebug.AssertIComponentOnOtherIsNotNull(m_timeRewinder, t_player.gameObject, this);
            #endregion Asserts

            SetSliderToTime(m_timeMan.curTime);

            m_timelineSlider.onValueChanged.AddListener(OnSliderValueChanged);
        }
        private void OnDestroy()
        {
            m_timelineSlider.onValueChanged.RemoveListener(OnSliderValueChanged);
        }
        private void Update()
        {
            bool t_canUseSlider = CanUseTimeSlider();
            m_timelineSlider.interactable = t_canUseSlider;

            // Adjust the slider automatically if not interacting with it.
            switch (m_interactState)
            {
                case eInteractState.None:
                {
                    SetSliderToTime(m_timeMan.curTime);
                    break;
                }
                case eInteractState.EndHandle:
                {
                    // We DO NOT want to allow player to pause time if they aren't even allowed to use the time slider.
                    if (!t_canUseSlider) { return; }
                    // If the player is currently dragging the time slider, we don't want any nav velocity to interfere with the player's bar manipulation.
                    m_playerCont.PauseNavVelocity();
                    break;
                }
                default:
                {
                    CustomDebug.UnhandledEnum(m_interactState, this);
                    break;
                }
            }
        }

        public float ConvertFromTimeToSliderValue(float time)
        {
            float t_earliestTime = m_timeRewinder.earliestTime;
            float t_upperTime = m_levelOpts.noTimeLimit ? m_timeRewinder.farthestTime : m_levelOpts.time;

            // v = (t - e) / (u - e)
            float t_denominator = t_upperTime - t_earliestTime;
            if (t_denominator == 0.0f)
            {
                return 1.0f;
            }
            else
            {
                return (time - t_earliestTime) / t_denominator;
            }
        }
        public float ConvertFromSliderValueToTime(float value)
        {
            float t_earliestTime = m_timeRewinder.earliestTime;
            float t_upperTime = m_levelOpts.noTimeLimit ? m_timeRewinder.farthestTime : m_levelOpts.time;

            // t = v * (u - e) + e
            return value * (t_upperTime - t_earliestTime) + t_earliestTime;
        }


        private void OnSliderValueChanged(float value)
        {
            if (m_prevSliderSetFrame == Time.frameCount) { return; }
            m_prevSliderSetFrame = Time.frameCount;

            // Restrict the value to stay below the current furthest time.
            float t_maxVal = ConvertFromTimeToSliderValue(m_timeRewinder.farthestTime);
            value = Mathf.Clamp(value, 0.0f, t_maxVal);
            #region Logs
            //CustomDebug.LogForComponent($"Restricted value of {m_timelineSlider.value} to {value} which is between 0 and {m_timeRewinder.farthestTime}.", this, IS_DEBUGGING);
            #endregion Logs
            m_timelineSlider.value = value;

            if (m_interactState != eInteractState.None)
            {
                // Set the time of the level to be at the time scrubbed to
                m_timeRewinder.ForceSetTime(ConvertFromSliderValueToTime(value));
            }

            m_onSliderValChanged.Invoke();
        }

        private void SetSliderToTime(float time)
        {
            m_timelineSlider.value = ConvertFromTimeToSliderValue(time);
        }

        private bool CanUseTimeSlider()
        {
            return m_playerInp.currentActionMap.name == m_timeManipInputMapName;
        }


        // Expected to be serialized as events in inspector
        #region UI Event Driven (Explicit)
        public void OnEndHandlePress()
        {
            m_interactState = eInteractState.EndHandle;
        }
        public void OnEndHandleRelease()
        {
            m_interactState = eInteractState.None;
        }
        #endregion UI Event Driven (Explicit)
    }
}
