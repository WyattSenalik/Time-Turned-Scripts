using System;
using UnityEngine;

using Helpers.Events;

namespace Helpers.UI
{
    [Serializable]
    public sealed class MenuMover
    {
        public Vector2 curNavDir { get; set; }
        public IEventPrimer<Vector2> onMove => m_onMove;

        [SerializeField, Min(0.0f)] private float m_autoMoveDelay = 0.15f;
        [SerializeField, Min(0.0f)] private float m_delayBeforeAutoMove = 0.5f;
        [SerializeField] private MixedEvent<Vector2> m_onMove = new MixedEvent<Vector2>();

        private float m_prevNavTime = 0.0f;
        private bool m_wasZero = false;
        private bool m_isAutoNav = false;


        public MenuMover() { }
        public MenuMover(float autoMoveDelay, float delayBeforeAutoMove)
        {
            m_autoMoveDelay = autoMoveDelay;
            m_delayBeforeAutoMove = delayBeforeAutoMove;
        }

        public void Update()
        {
            if (curNavDir.x != 0.0f || curNavDir.y != 0.0f)
            {
                if (m_isAutoNav)
                {
                    float t_nextNavTime = m_prevNavTime + m_autoMoveDelay;
                    if (Time.unscaledTime >= t_nextNavTime)
                    {
                        m_prevNavTime = Time.unscaledTime;
                        m_onMove.Invoke(curNavDir);
                    }
                }
                else
                {
                    if (m_wasZero)
                    {
                        m_prevNavTime = Time.unscaledTime;
                        m_onMove.Invoke(curNavDir);
                    }
                    else
                    {
                        float t_nextNavTime = m_prevNavTime + m_delayBeforeAutoMove;
                        if (Time.unscaledTime >= t_nextNavTime)
                        {
                            m_prevNavTime = Time.unscaledTime;
                            m_isAutoNav = true;
                            m_onMove.Invoke(curNavDir);
                        }
                    }
                }
                m_wasZero = false;
            }
            else
            {
                m_wasZero = true;
                m_isAutoNav = false;
            }
        }
    }
}