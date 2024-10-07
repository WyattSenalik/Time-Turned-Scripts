using System.Collections.Generic;
using UnityEngine;

using NaughtyAttributes;

using Helpers.Events;
using Helpers.Extensions;
using Timed;
// Original Authors - Wyatt Senalik

namespace Atma
{
    [DisallowMultipleComponent]
    public sealed class KillDisplay : MonoBehaviour
    {
        [SerializeField, Required] private SpriteRenderer m_numberSprRend = null;
        [SerializeField, Required] private GameObject m_plusNumberObj = null;
        [SerializeField, Tag] private string m_exitDoorTag = "ExitDoor";

        [SerializeField] private Sprite m_num0Sprite = null;
        [SerializeField] private Sprite m_num1Sprite = null;
        [SerializeField] private Sprite m_num2Sprite = null;
        [SerializeField] private Sprite m_num3Sprite = null;
        [SerializeField] private Sprite m_num4Sprite = null;
        [SerializeField] private Sprite m_num5Sprite = null;
        [SerializeField] private Sprite m_num6Sprite = null;
        [SerializeField] private Sprite m_num7Sprite = null;
        [SerializeField] private Sprite m_num8Sprite = null;
        [SerializeField] private Sprite m_num9Sprite = null;

        private readonly List<IEventPrimer> m_onActivesSubbedTo = new List<IEventPrimer>();
        private readonly List<IEventPrimer> m_onDeactivesSubbedTo = new List<IEventPrimer>();

        private TimedInt m_amountEnemiesDead = null;
        private int m_amountEnemiesDeadInternal = 0;
        private int m_amountThatNeedToDie = -1;


        private void Start()
        {
            GameObject t_exitDoorObj = GameObject.FindWithTag(m_exitDoorTag);
            #region Asserts
            //CustomDebug.AssertGameObjectWithTagIsNotNull(t_exitDoorObj, m_exitDoorTag, this);
            #endregion Asserts
            Door t_exitDoor = t_exitDoorObj.GetComponentSafe<Door>();

            m_amountEnemiesDead = new TimedInt(0, eInterpolationOption.Step);

            m_amountThatNeedToDie = 0;
            foreach (GameObject t_linkedObj in t_exitDoor.linkedObjects)
            {
                if (t_linkedObj.TryGetComponent(out DeathActivatorImplementation t_deathActivator))
                {
                    m_onActivesSubbedTo.Add(t_deathActivator.onActivated);
                    m_onDeactivesSubbedTo.Add(t_deathActivator.onDeactivated);
                    ++m_amountThatNeedToDie;
                }
            }
            UpdateSprite();
            ToggleSubscriptions(true);
        }
        private void OnDestroy()
        {
            ToggleSubscriptions(false);
        }
        private void FixedUpdate()
        {
            if (!m_amountEnemiesDead.isRecording)
            {
                m_amountEnemiesDeadInternal = m_amountEnemiesDead.curData;
                UpdateSprite();
            }
            else
            {
                m_amountEnemiesDead.curData = m_amountEnemiesDeadInternal;
            }
        }


        private void ToggleSubscriptions(bool cond)
        {
            foreach (IEventPrimer t_eventPrimer in m_onActivesSubbedTo)
            {
                t_eventPrimer.ToggleSubscription(OnEnemyDied, cond);
            }
            foreach (IEventPrimer t_eventPrimer in m_onDeactivesSubbedTo)
            {
                t_eventPrimer.ToggleSubscription(OnUndoEnemyDeath, cond);
            }
        }
        private void OnEnemyDied()
        {
            if (m_amountEnemiesDead.isRecording)
            {
                ++m_amountEnemiesDeadInternal;
            }
            UpdateSprite();
        }
        private void OnUndoEnemyDeath()
        {
            if (m_amountEnemiesDead.isRecording)
            {
                --m_amountEnemiesDeadInternal;
            }
            UpdateSprite();
        }
        private void UpdateSprite()
        {
            int t_displayNum = m_amountThatNeedToDie - m_amountEnemiesDeadInternal;
            m_plusNumberObj.SetActive(t_displayNum > 9);
            m_numberSprRend.sprite = GetSpriteForNumber(Mathf.Clamp(t_displayNum, 0, 9));
        }
        private Sprite GetSpriteForNumber(int number)
        {
            switch (number)
            {
                case 0: return m_num0Sprite;
                case 1: return m_num1Sprite;
                case 2: return m_num2Sprite;
                case 3: return m_num3Sprite;
                case 4: return m_num4Sprite;
                case 5: return m_num5Sprite;
                case 6: return m_num6Sprite;
                case 7: return m_num7Sprite;
                case 8: return m_num8Sprite;
                case 9: return m_num9Sprite;
                default:
                {
                    CustomDebug.UnhandledEnum(number, nameof(KillDisplay));
                    return null;
                }
            }
        }
    }
}