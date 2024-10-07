using UnityEngine;

using NaughtyAttributes;
// Original Authors - Wyatt Senalik

namespace Atma.UI
{
    [DisallowMultipleComponent]
    public sealed class NavigatableMenu : MonoBehaviour, INavigatableMenu
    {
        [SerializeField, Required] private NavigatableMenuOption m_beginNavOpt = null;
        [SerializeField, Min(0.0f)] private float m_autoMoveDelay = 0.15f;
        [SerializeField, Min(0.0f)] private float m_delayBeforeAutoMove = 0.5f;
        private NavigatableMenuOption m_curSelNavOpt = null;

        private Vector2 m_curNavDir = Vector2.zero;
        private float m_prevNavTime = 0.0f;
        private bool m_isAutoNav = false;


        private void OnDisable()
        {
            ZeroOutNavigation();
        }
        private void Start()
        {
            m_curSelNavOpt = m_beginNavOpt;
            m_curSelNavOpt.OnHighlighted();
        }
        private void Update()
        {
            if (m_curNavDir != Vector2.zero)
            {
                if (m_isAutoNav)
                {
                    float t_nextNavTime = m_prevNavTime + m_autoMoveDelay;
                    if (Time.unscaledTime >= t_nextNavTime)
                    {
                        if (TryNavigateToNextOption())
                        {
                            m_prevNavTime = Time.unscaledTime;
                        }
                        else
                        {
                            ZeroOutNavigation();
                        }
                    }
                }
                else
                {
                    float t_nextNavTime = m_prevNavTime + m_delayBeforeAutoMove;
                    if (Time.unscaledTime >= t_nextNavTime)
                    {
                        if (TryNavigateToNextOption())
                        {
                            m_prevNavTime = Time.unscaledTime;
                            m_isAutoNav = true;
                        }
                        else
                        {
                            ZeroOutNavigation();
                        }
                    }
                }
            }
        }


        public void Navigate(Vector2 navDir)
        {
            float t_newAbsX = Mathf.Abs(navDir.x);
            float t_newAbsY = Mathf.Abs(navDir.y);
            if (t_newAbsX < 0.1f && t_newAbsY < 0.1f)
            {
                ZeroOutNavigation();
            }
            else
            {
                int t_curXDir = 0;
                if (m_curNavDir.x > 0.1f) { t_curXDir = 1; }
                else if (m_curNavDir.x < -0.1f) { t_curXDir = -1; }

                int t_curYDir = 0;
                if (m_curNavDir.y > 0.1f) { t_curYDir = 1; }
                else if (m_curNavDir.y < -0.1f) { t_curYDir = -1; }

                int t_newXDir = 0;
                if (navDir.x > 0.1f) { t_newXDir = 1; }
                else if (navDir.x < -0.1f) { t_newXDir = -1; }

                int t_newYDir = 0;
                if (navDir.y > 0.1f) { t_newYDir = 1; }
                else if (navDir.y < -0.1f) { t_newYDir = -1; }

                int t_signSumX = Mathf.Abs(t_curXDir + t_newXDir);
                int t_signSumY = Mathf.Abs(t_curYDir + t_newYDir);
                // If direction changed or went from nothing to something.
                // Sign sum of 2 means its going in the same direction.
                // Sign sum of 1 means it was stopped and is now going or vice versa.
                // Sign sum of 0 means its going in opposite directions or is doing nothing.
                if (t_signSumX == 1 || t_signSumY == 1 || (t_signSumX == 0 && t_signSumY == 0))
                {
                    m_curNavDir = navDir;
                    if (TryNavigateToNextOption())
                    {
                        m_prevNavTime = Time.unscaledTime;
                    }
                    else
                    {
                        ZeroOutNavigation();
                    }
                }
                else
                {
                    m_curNavDir = navDir;
                }
            }
        }
        public void Submit()
        {
            ZeroOutNavigation();
            m_curSelNavOpt.OnChosen();
        }
        public void SetBeginOptionAsCurOption()
        {
            ZeroOutNavigation();
            if (m_curSelNavOpt != null)
            {
                m_curSelNavOpt.OnUnhighlighted();
            }
            m_curSelNavOpt = m_beginNavOpt;
            m_curSelNavOpt.OnHighlighted();
        }

        public void OnPointerClickOption(NavigatableMenuOption option)
        {
            ZeroOutNavigation();
            if (m_curSelNavOpt != null)
            {
                m_curSelNavOpt.OnUnhighlighted();
            }
            m_curSelNavOpt = option;
            m_curSelNavOpt.OnChosen();
        }
        public void OnPointerEnterOption(NavigatableMenuOption option)
        {
            ZeroOutNavigation();
            if (m_curSelNavOpt != null)
            {
                m_curSelNavOpt.OnUnhighlighted();
            }
            m_curSelNavOpt = option;
            m_curSelNavOpt.OnHighlighted();
        }
        public void OnPointerExitOption(NavigatableMenuOption option)
        {

        }


        private bool TryNavigateToNextOption()
        {
            NavigatableMenuOption t_newOpt = m_curSelNavOpt.GetNavigatedToOption(m_curNavDir);

            if (t_newOpt != null && t_newOpt != m_curSelNavOpt)
            {
                m_curSelNavOpt.OnUnhighlighted();
                m_curSelNavOpt = t_newOpt;
                m_curSelNavOpt.OnHighlighted();

                return true;
            }
            return false;
        }
        private void ZeroOutNavigation()
        {
            m_curNavDir = Vector2.zero;
            m_isAutoNav = false;
            m_prevNavTime = float.NegativeInfinity;
        }
    }
}