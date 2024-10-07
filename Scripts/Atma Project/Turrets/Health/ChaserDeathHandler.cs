using UnityEngine;

using NaughtyAttributes;
using Helpers.Animation;
using Timed.Animation;
using Timed;
using Helpers.Physics.Custom2DInt.NavAI;
using Helpers.Physics.Custom2DInt;
// Original Authors - Wyatt Senalik

namespace Atma
{
    [DisallowMultipleComponent]
    public sealed class ChaserDeathHandler : MonoBehaviour, IEnemyDeathHandler
    {
        public bool isDead => !float.IsPositiveInfinity(deathTime);
        public float deathTime { get; private set; } = float.PositiveInfinity;

        [SerializeField] private eAnimatorType m_animType = eAnimatorType.UnityAnimator;
        [SerializeField, Required, ShowIf(nameof(IsUnityAnimator))] private Animator m_animator = null;
        [SerializeField, AnimatorParam(nameof(m_animator)), ShowIf(nameof(IsUnityAnimator))] private string m_deathBoolParamName = "isDead";

        [SerializeField, Required, ShowIf(nameof(IsManualAnimator))] private TimedManualAnimation m_manualAnimator = null;

        [SerializeField] private Behaviour[] m_behavioursToToggle = new Behaviour[0];
        [SerializeField] private GameObject[] m_objectsToToggle = new GameObject[0];

        [SerializeField] private bool m_updateNavGraphOnDeath = false;

        private NavGraphInt2D m_navGraph = null;

        private TimedBool[] m_objectActiveStates = null;

        private DeathActivatorImplementation m_deathActivator = null;

        private bool[] m_priorBehavEnabledStates = null;
        private bool[] m_priorObjActiveStates = null;


        private void Awake()
        {
            #region Asserts
            if (IsUnityAnimator())
            {
                //CustomDebug.AssertSerializeFieldIsNotNull(m_animator, nameof(m_animator), this);
            }
            if (IsManualAnimator())
            {
                //CustomDebug.AssertSerializeFieldIsNotNull(m_manualAnimator, nameof(m_manualAnimator), this);
            }
            #endregion Asserts

            m_deathActivator = GetComponent<DeathActivatorImplementation>();

            m_priorBehavEnabledStates = new bool[m_behavioursToToggle.Length];
            m_priorObjActiveStates = new bool[m_objectsToToggle.Length];
        }
        private void Start()
        {
            if (m_updateNavGraphOnDeath)
            {
                m_navGraph = NavGraphInt2D.instance;
            }

            m_objectActiveStates = new TimedBool[m_objectsToToggle.Length];
            for (int i = 0; i < m_objectActiveStates.Length; ++i)
            {
                m_objectActiveStates[i] = new TimedBool(m_objectsToToggle[i].activeSelf);
            }
        }
        private void FixedUpdate()
        {
            for (int i = 0; i < m_objectActiveStates.Length; ++i)
            {
                TimedBool t_curObjActiveState = m_objectActiveStates[i];
                if (t_curObjActiveState.isRecording)
                {
                    t_curObjActiveState.curData = m_objectsToToggle[i].activeSelf;
                }
                else
                {
                    m_objectsToToggle[i].SetActive(t_curObjActiveState.curData);
                }
            }
        }


        public void HandleDeath(float deathTime)
        {
            this.deathTime = deathTime;
            ToggleBehavsAndObjsOff();

            if (m_deathActivator != null)
            {
                m_deathActivator.Activate();
            }

            if (IsUnityAnimator())
            {
                m_animator.SetBool(m_deathBoolParamName, true);
            }
            else if (IsManualAnimator())
            {
                m_manualAnimator.Play();
            }

            UpdateNavGraph(true);
        }
        public void RevertDeath()
        {
            this.deathTime = float.PositiveInfinity;
            ToggleBehavsAndObsToPriorState();

            if (m_deathActivator != null)
            {
                m_deathActivator.Deactivate();
            }

            if (IsUnityAnimator())
            {
                m_animator.SetBool(m_deathBoolParamName, false);
            }
            else if (IsManualAnimator())
            {
                m_manualAnimator.ClearAllPlayTimes();
            }

            UpdateNavGraph(false);
        }

        private void ToggleBehavsAndObjsOff()
        {
            for (int i = 0; i < m_behavioursToToggle.Length; ++i)
            {
                m_priorBehavEnabledStates[i] = m_behavioursToToggle[i].enabled;
                m_behavioursToToggle[i].enabled = false;
            }
            for (int i = 0; i < m_objectsToToggle.Length; ++i)
            {
                m_priorObjActiveStates[i] = m_objectsToToggle[i].activeSelf;
                m_objectsToToggle[i].SetActive(false);
            }
        }
        private void ToggleBehavsAndObsToPriorState()
        {
            for (int i = 0; i < m_behavioursToToggle.Length; ++i)
            {
                m_behavioursToToggle[i].enabled = m_priorBehavEnabledStates[i];
            }
            for (int i = 0; i < m_objectsToToggle.Length; ++i)
            {
                m_objectsToToggle[i].SetActive(m_priorObjActiveStates[i]);
            }
        }
        private void UpdateNavGraph(bool setValue)
        {
            if (m_updateNavGraphOnDeath && m_navGraph != null)
            {
                Vector2Int t_intPos = CustomPhysics2DInt.ConvertFloatPositionToIntPosition(transform.position);
                int t_tileIndex = m_navGraph.GetIndexOfTileThatContainsIntPoint(t_intPos);
                m_navGraph.SetStateOfTileAtIndex(t_tileIndex, setValue);
            }
        }


        public bool IsUnityAnimator() => m_animType == eAnimatorType.UnityAnimator;
        public bool IsManualAnimator() => m_animType == eAnimatorType.ManualAnimator;

        public enum eAnimatorType { UnityAnimator, ManualAnimator }
    }
}