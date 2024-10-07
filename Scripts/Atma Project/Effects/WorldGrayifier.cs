using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using NaughtyAttributes;

using Atma.Events;
using Helpers.Events.GameEventSystem;
using Timed;
// Original Authors - Wyatt Senalik

namespace Atma
{
    [DisallowMultipleComponent]
    public sealed class WorldGrayifier : MonoBehaviour
    {
        public const string GRAYSCALE_PERCENT_VAR_NAME = "_GrayscalePercent";

        [SerializeField, Required] private CloneDiedEventIdentifierSO m_cloneDiedEventIDSO = null;
        [SerializeField, Required] private IneptusDiedEventIdentifierSO m_ineptusDiedEventIDSO = null;

        [SerializeField, Required] private TimeDisplay m_timeDisplay = null;
        [SerializeField, Required] private RoomExplosion m_roomExplosion = null;
        [SerializeField, Required] private Material m_litGrayMaterial = null;
        [SerializeField, Required] private Material m_unlitGrayMaterial = null;
        [SerializeField, Min(0.0f)] private float m_grayFadeInTime = 0.5f;

        private PlayerHealth m_playerHealth = null;
        private TimeRewinder m_timeRewinder = null;
        private SubManager<ICloneDiedContext> m_cloneDiedSubMan = null;
        private SubManager<IIneptusDiedContext> m_ineptusDiedSubMan = null;

        private bool m_isCoroutActive = false;
        private readonly List<SpriteRenderer> m_excludedLitRenderers = new List<SpriteRenderer>();
        private readonly List<SpriteRenderer> m_excludedUnlitRenderers = new List<SpriteRenderer>();


        private void Awake()
        {
            #region Asserts
            //CustomDebug.AssertSerializeFieldIsNotNull(m_cloneDiedEventIDSO, nameof(m_cloneDiedEventIDSO), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_timeDisplay, nameof(m_timeDisplay), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_roomExplosion, nameof(m_roomExplosion), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_litGrayMaterial, nameof(m_litGrayMaterial), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_unlitGrayMaterial, nameof(m_unlitGrayMaterial), this);
            #endregion Asserts
        }
        private void Start()
        {
            PlayerSingleton t_playerSingleton = PlayerSingleton.instance;
            #region Asserts
            //CustomDebug.AssertSingletonIsNotNull(t_playerSingleton, this);
            #endregion Asserts
            m_playerHealth = t_playerSingleton.GetComponent<PlayerHealth>();
            m_timeRewinder = t_playerSingleton.GetComponent<TimeRewinder>();
            #region Asserts
            //CustomDebug.AssertComponentOnOtherIsNotNull(m_playerHealth, t_playerSingleton.gameObject, this);
            //CustomDebug.AssertComponentOnOtherIsNotNull(m_timeRewinder, t_playerSingleton.gameObject, this);
            #endregion Asserts

            SetGrayPercent(0.0f);

            m_cloneDiedSubMan = new SubManager<ICloneDiedContext>(m_cloneDiedEventIDSO, OnCloneDied);
            m_ineptusDiedSubMan = new SubManager<IIneptusDiedContext>(m_ineptusDiedEventIDSO, OnIneptusDied);
            ToggleSubscriptions(true);
        }
        private void OnDestroy()
        {
            SetGrayPercent(0.0f);
            ToggleSubscriptions(false);
        }


        public void SetGrayPercent(float percent)
        {
            m_litGrayMaterial.SetFloat(GRAYSCALE_PERCENT_VAR_NAME, percent);
            m_unlitGrayMaterial.SetFloat(GRAYSCALE_PERCENT_VAR_NAME, percent);
        }
        public void ExcludeLitGameObjectFromGrayification(GameObject obj)
        {
            SpriteRenderer[] t_renderers = obj.GetComponentsInChildren<SpriteRenderer>();
            foreach (SpriteRenderer t_singleRenderer in t_renderers)
            {
                Material t_ogMat = t_singleRenderer.material;
                Material t_copiedMat = new Material(t_ogMat);
                t_copiedMat.SetFloat(GRAYSCALE_PERCENT_VAR_NAME, 0.0f);
                t_singleRenderer.material = t_copiedMat;
            }
            m_excludedLitRenderers.AddRange(t_renderers);
        }
        public void UnExcludeAllLitGameObjectsFromGrayification()
        {
            foreach (SpriteRenderer t_singleRenderer in m_excludedLitRenderers)
            {
                // Can be null if excluded renderer was a bullet
                if (t_singleRenderer != null)
                {
                    t_singleRenderer.material = m_litGrayMaterial;
                }
            }
            m_excludedLitRenderers.Clear();
        }
        public void ExcludeUnlitSpriteRendererFromGrayification(SpriteRenderer rend)
        {
            Material t_ogMat = rend.material;
            Material t_copiedMat = new Material(t_ogMat);
            t_copiedMat.SetFloat(GRAYSCALE_PERCENT_VAR_NAME, 0.0f);
            rend.material = t_copiedMat;
            m_excludedUnlitRenderers.Add(rend);
        }
        public void UnExcludeAllUnlitGameObjectsFromGrayification()
        {
            foreach (SpriteRenderer t_singleRenderer in m_excludedUnlitRenderers)
            {
                // Can be null if excluded renderer was a bullet
                if (t_singleRenderer != null)
                {
                    t_singleRenderer.material = m_unlitGrayMaterial;
                }
            }
            m_excludedUnlitRenderers.Clear();
        }
        public void UnExcludeAllGameObjectsFromGrayification()
        {
            UnExcludeAllLitGameObjectsFromGrayification();
            UnExcludeAllUnlitGameObjectsFromGrayification();
        }


        private void OnCloneDied(ICloneDiedContext context)
        {
            ExcludeLitGameObjectFromGrayification(context.cloneObj);
            ExcludeLitGameObjectFromGrayification(context.killerObj);

            StartUpdateGrayDuringTimeRewindCoroutine();
        }
        private void OnIneptusDied(IIneptusDiedContext context)
        {
            ExcludeLitGameObjectFromGrayification(context.ineptusObj);
            ExcludeLitGameObjectFromGrayification(context.killerObj);

            StartUpdateGrayDuringTimeRewindCoroutine();
        }
        private void OnPlayerDeath(IDamageContext context)
        {
            ExcludeLitGameObjectFromGrayification(m_playerHealth.gameObject);
            ExcludeLitGameObjectFromGrayification(context.damageSourceObj);

            StartUpdateGrayDuringTimeRewindCoroutine();
        }
        private void OnTimeRanOut()
        {
            foreach (SpriteRenderer t_explosionRend in m_roomExplosion.explosionsSpriteRends)
            {
                if (t_explosionRend == null) { continue; }
                ExcludeUnlitSpriteRendererFromGrayification(t_explosionRend);
            }

            StartUpdateGrayDuringTimeRewindCoroutine();
        }
        private void ToggleSubscriptions(bool cond)
        {
            m_cloneDiedSubMan.ToggleSubscription(cond);
            m_ineptusDiedSubMan.ToggleSubscription(cond);
            m_playerHealth?.onDeathWContext.ToggleSubscription(OnPlayerDeath, cond);
            m_timeDisplay?.onReachedZero.ToggleSubscription(OnTimeRanOut, cond);
        }

        private void StartUpdateGrayDuringTimeRewindCoroutine()
        {
            if (m_isCoroutActive) { return; }
            StartCoroutine(UpdateGrayDuringTimeRewindCoroutine());
        }
        private IEnumerator UpdateGrayDuringTimeRewindCoroutine()
        {
            m_isCoroutActive = true;

            float t_elapsedTime = 0.0f;
            while (t_elapsedTime < m_grayFadeInTime)
            {
                SetGrayPercent(t_elapsedTime / m_grayFadeInTime);
                yield return null;
                t_elapsedTime += Time.deltaTime;
            }

            while (m_timeRewinder.hasStarted)
            {
                float t_timeFromEnd = m_timeRewinder.farthestTime - m_timeRewinder.curTime;
                if (t_timeFromEnd < BranchPlayerController.RESUME_BUFFER_TIME)
                {
                    // Get more gray the closer the end (death) time is.
                    SetGrayPercent(1.0f - t_timeFromEnd / BranchPlayerController.RESUME_BUFFER_TIME);
                }
                else
                {
                    SetGrayPercent(0.0f);
                }

                yield return null;
            }
            // Time rewind has ended
            SetGrayPercent(0.0f);
            UnExcludeAllGameObjectsFromGrayification();

            m_isCoroutActive = false;
        }
    }
}