using System.Collections;
using UnityEngine;

using NaughtyAttributes;

using Timed;
// Original Authors - Wyatt Senalik

namespace Atma.UI
{
    /// <summary>
    /// The background for the slider for the time that the the player is allowed to scrub to.
    /// There will be another background for the full length of the level, but we need some difference in coloration to make it more clear why the player can't rewind past that point.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(RectTransform))]
    public sealed class TimeSliderAchievedBackground : MonoBehaviour
    {
        private RectTransform m_rectTrans = null;

        private LevelOptions m_levelOpts = null;
        private ITimeRewinder m_timeRewinder = null;

        private bool m_areRefsInitialized = false;


        private void Awake()
        {
            m_rectTrans = GetComponent<RectTransform>();
            #region Asserts
            //CustomDebug.AssertComponentIsNotNull(m_rectTrans, this);
            #endregion Asserts
        }
        private void OnEnable()
        {
            // Must protect against race conditions with reference initialization because
            StartCoroutine(AdjustVisualCorot());
        }
        private void Start()
        {
            if (m_areRefsInitialized) { return; }
            // If this is called here, it should definitely be able to initialize.
            m_areRefsInitialized = TryIntializeReferences();
            #region Asserts
            //CustomDebug.AssertIsTrueForComponent(m_areRefsInitialized, $"all refs to be initialized.", this);
            #endregion Asserts
        }


        private void AdjustBackgroundVisual()
        {
            // Find out what percent of the total time has been achieved so far.
            float t_percentTimeAchieved;
            if (m_levelOpts.noTimeLimit)
            {
                t_percentTimeAchieved = 1.0f;
            }
            else
            {
                float t_achievedTime = m_timeRewinder.farthestTime;
                float t_levelTime = m_levelOpts.time;
                t_percentTimeAchieved = t_achievedTime / t_levelTime;
            }
            // Set the x max anchor to be that percent time (min is assumed to be 0).
            Vector2 t_anchorMax = m_rectTrans.anchorMax;
            t_anchorMax.x = t_percentTimeAchieved;
            m_rectTrans.anchorMax = t_anchorMax;
            // Set the offset from the anchors to be 0 for x.
            Vector2 t_anchorPos = m_rectTrans.anchoredPosition;
            t_anchorPos.x = 0.0f;
            m_rectTrans.anchoredPosition = t_anchorPos;
        }

        /// <summary>
        /// Coroutine to try initializing references again if by chance, the awake/enable of this script is called before the awake/enable of the singleton.
        /// </summary>
        private IEnumerator AdjustVisualCorot()
        {
            while (!m_areRefsInitialized)
            {
                m_areRefsInitialized = TryIntializeReferences();
                yield return null;
            }

            // Now that refs are initialized correctly
            AdjustBackgroundVisual();
        }
        private bool TryIntializeReferences()
        {
            // These two should be fine, because they do not need to wait on another script.
            PlayerSingleton t_player = PlayerSingleton.instance;
            #region Asserts
            //CustomDebug.AssertSingletonMonoBehaviourIsNotNull(t_player, this);
            #endregion Asserts
            m_timeRewinder = t_player.GetComponent<ITimeRewinder>();
            #region Asserts
            //CustomDebug.AssertIComponentOnOtherIsNotNull(m_timeRewinder, t_player.gameObject, this);
            #endregion Asserts

            // It is possible that LevelOptions will be null if it it's Awake/Enable is slated to be called after this object's Awake/Enable.
            m_levelOpts = LevelOptions.instance;
            if (m_levelOpts == null)
            {
                return false;
            }
            return true;
        }
    }
}
