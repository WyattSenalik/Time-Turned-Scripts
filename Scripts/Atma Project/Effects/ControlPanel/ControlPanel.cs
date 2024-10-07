using System.Collections.Generic;
using UnityEngine;

using NaughtyAttributes;

using Timed;

using static Atma.ControlPanelElement;

namespace Atma
{
    [DisallowMultipleComponent]
    public sealed class ControlPanel : MonoBehaviour
    {
        [SerializeField, MinMaxSlider(0.0f, 20.0f)] private Vector2 m_buttonsChangeDelay = Vector2.zero;
        [SerializeField, MinMaxSlider(0.0f, 20.0f)] private Vector2 m_bigButtonsChangeDelay = Vector2.zero;
        [SerializeField, MinMaxSlider(0.0f, 20.0f)] private Vector2 m_leversChangeDelay = Vector2.zero;
        [SerializeField, MinMaxSlider(0.0f, 20.0f)] private Vector2 m_valvesChangeDelay = Vector2.zero;
        [SerializeField, MinMaxSlider(0.0f, 20.0f)] private Vector2 m_lightsChangeDelay = Vector2.zero;

        private TimedBool m_isActivated = null;

        private readonly List<ControlPanelElement> m_buttons = new List<ControlPanelElement>();
        private readonly List<ControlPanelElement> m_bigButtons = new List<ControlPanelElement>();
        private readonly List<ControlPanelElement> m_levers = new List<ControlPanelElement>();
        private readonly List<ControlPanelElement> m_valves = new List<ControlPanelElement>();
        private readonly List<ControlPanelElement> m_lights = new List<ControlPanelElement>();

        private TimedFloat m_buttonChangedTimes = null;
        private TimedFloat m_bigButtonChangedTimes = null;
        private TimedFloat m_leverChangedTimes = null;
        private TimedFloat m_valveChangedTimes = null;
        private TimedFloat m_lightChangedTimes = null;


        private void Awake()
        {
            ControlPanelElement[] t_allElements = GetComponentsInChildren<ControlPanelElement>();
            foreach (ControlPanelElement t_singleEle in t_allElements)
            {
                switch (t_singleEle.type)
                {
                    case eElementType.Button:
                        m_buttons.Add(t_singleEle);
                        break;
                    case eElementType.BigButton:
                        m_bigButtons.Add(t_singleEle);
                        break;
                    case eElementType.Lever:
                        m_levers.Add(t_singleEle);
                        break;
                    case eElementType.Valve:
                        m_valves.Add(t_singleEle);
                        break;
                    case eElementType.Light:
                        m_lights.Add(t_singleEle);
                        break;
                    default:
                        CustomDebug.UnhandledEnum(t_singleEle.type, this);
                        break;
                }
            }
        }
        private void Start()
        {
            m_isActivated = new TimedBool(true);

            m_buttonChangedTimes = new TimedFloat(GetRandomNextTime(0.0f, m_buttonsChangeDelay), eInterpolationOption.Step);
            m_bigButtonChangedTimes = new TimedFloat(GetRandomNextTime(0.0f, m_bigButtonsChangeDelay), eInterpolationOption.Step);
            m_leverChangedTimes = new TimedFloat(GetRandomNextTime(0.0f, m_leversChangeDelay), eInterpolationOption.Step);
            m_valveChangedTimes = new TimedFloat(GetRandomNextTime(0.0f, m_valvesChangeDelay), eInterpolationOption.Step);
            m_lightChangedTimes = new TimedFloat(GetRandomNextTime(0.0f, m_lightsChangeDelay), eInterpolationOption.Step);
        }
        private void Update()
        {
            if (m_isActivated.curData)
            {
                ChangeElementIfNeeded(m_buttonChangedTimes, m_buttons, m_buttonsChangeDelay);
                ChangeElementIfNeeded(m_bigButtonChangedTimes, m_bigButtons, m_bigButtonsChangeDelay);
                ChangeElementIfNeeded(m_leverChangedTimes, m_levers, m_leversChangeDelay);
                ChangeElementIfNeeded(m_valveChangedTimes, m_valves, m_valvesChangeDelay);
                ChangeElementIfNeeded(m_lightChangedTimes, m_lights, m_lightsChangeDelay);
            }
        }


        public void SetActivated(bool cond)
        {
            m_isActivated.curData = cond;
        }
        public void Activate()
        {
            SetActivated(true);
        }
        public void Deactivate()
        {
            SetActivated(false);
        }


        private void ChangeElementIfNeeded(TimedFloat elementsChangedTimes, List<ControlPanelElement> elementsList, Vector2 elementChangeDelay)
        {
            float t_curTime = elementsChangedTimes.curTime;
            if (t_curTime >= elementsChangedTimes.curData)
            {
                int t_eleToChangeIndex = Random.Range(0, elementsList.Count);
                elementsList[t_eleToChangeIndex].Change();
                elementsChangedTimes.curData = GetRandomNextTime(t_curTime, elementChangeDelay);
            }
        }
        private float GetRandomNextTime(float curTime, Vector2 minMaxRange)
        {
            return curTime + Random.Range(minMaxRange.x, minMaxRange.y);
        }
    }
}