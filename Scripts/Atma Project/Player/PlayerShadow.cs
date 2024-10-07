using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using NaughtyAttributes;

using Helpers.Animation.BetterCurve;
using Helpers.Extensions;
using Timed;
// Original Authors - Wyatt Senalik

namespace Atma
{
    public sealed class PlayerShadow : MonoBehaviour
    {
        [SerializeField, Required] private Transform m_shadowTrans = null;
        [SerializeField, Required] private SpriteRenderer m_shadowSprRend = null;

        [SerializeField] private Vector2 m_shadowMaxOffset = new Vector2(0.0f, -0.25f);

        private PlayerSingleton m_playerSingleton = null;
        private PlayerStateManager m_playerStateMan = null;
        private SpriteRenderer m_playerSprRend = null;
        private Leaper m_playerLeaper = null;
        private PlayerPitfallHandler m_playerPitfallHandler = null;


        private void Start()
        {
            m_playerSingleton = PlayerSingleton.GetInstanceSafe();
            m_playerStateMan = m_playerSingleton.GetComponentSafe<PlayerStateManager>(this);
            m_playerSprRend = m_playerSingleton.GetComponentSafe<SpriteRenderer>(this);
            m_playerLeaper = m_playerSingleton.GetComponentSafe<Leaper>(this);
            m_playerPitfallHandler = m_playerSingleton.GetComponentSafe<PlayerPitfallHandler>(this);

            #region Asserts
            //CustomDebug.AssertSerializeFieldIsNotNull(m_shadowTrans, nameof(m_shadowTrans), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_shadowSprRend, nameof(m_shadowSprRend), this);
            #endregion Asserts
        }
        // Assumes this script's execution order is after everything that has to do with the player's position.
        private void LateUpdate()
        {
            if (!m_playerSprRend.enabled || !m_playerSingleton.gameObject.activeInHierarchy)
            {
                HideShadow();
                return;
            }

            switch (m_playerStateMan.timeAwareCurState)
            {
                case ePlayerState.Default:
                {
                    DefaultFollow();
                    break;
                }
                case ePlayerState.Leap:
                {
                    LeapFollow();
                    break;
                }
                case ePlayerState.Dead:
                {
                    Vector2 t_playerScale = m_playerSingleton.transform.localScale;
                    if (t_playerScale.x == 1.0f)
                    {
                        // Died by getting shot or something that wasn't falling into a pit.
                        DefaultFollow();
                    }
                    else
                    {
                        // Death caused by falling into a pit.
                        HideShadow();
                    }
                    break;
                }
                default:
                {
                    CustomDebug.UnhandledEnum(m_playerStateMan.curState, this);
                    break;
                }
            }
        }


        private void DefaultFollow()
        {
            if (m_playerPitfallHandler.isOverPit)
            {
                DirectFollow();
            }
            else
            {
                ShowShadow();
                m_shadowTrans.position = m_playerSingleton.transform.position;
                m_shadowTrans.localScale = Vector3.one;
            }
        }
        private void LeapFollow()
        {
            ShowShadow();
            float t_curTime = m_playerLeaper.curTime;
            if (m_playerLeaper.TryGetLeapTimeFrame(t_curTime, out TimeFrame t_leapTimeFrame))
            {
                float t_leapDuration = t_leapTimeFrame.length;
                float t_elapsedTime = t_curTime - t_leapTimeFrame.startTime;
                float t_percentDuration = t_elapsedTime / t_leapDuration;

                float t_playerScale = m_playerSingleton.transform.localScale.x;
                if (t_playerScale >= 1.0f)
                {
                    float t_invertedScale = 1.0f / t_playerScale;
                    m_shadowTrans.localScale = new Vector3(t_invertedScale, t_invertedScale, 1.0f);
                }
                else
                {
                    m_shadowTrans.localScale = new Vector3(t_playerScale, t_playerScale, 1.0f);
                }

                Vector2 t_playerPos2D = m_playerSingleton.transform.position;
                Vector2 t_downPos = t_playerPos2D + m_shadowMaxOffset;
                if (t_percentDuration < 0.5f)
                {
                    m_shadowTrans.position = Vector2.Lerp(t_playerPos2D, t_downPos, t_percentDuration * 2);
                }
                else
                {
                    m_shadowTrans.position = Vector2.Lerp(t_downPos, t_playerPos2D, (t_percentDuration - 0.5f) * 2);
                }
            }
            else
            {
                // Fallback is Default Follow
                #region Logs
                //CustomDebug.LogWarningForComponent($"Leap Follow was called but there is no leap occuring at this time ({t_curTime})", this);
                #endregion Logs
                DefaultFollow();
            }
        }
        private void DirectFollow()
        {
            ShowShadow();
            m_shadowTrans.position = m_playerSingleton.transform.position;
            m_shadowTrans.localScale = m_playerSingleton.transform.localScale;
        }
        private void ShowShadow()
        {
            m_shadowSprRend.enabled = true;
        }
        private void HideShadow()
        {
            m_shadowSprRend.enabled = false;
        }
    }
}