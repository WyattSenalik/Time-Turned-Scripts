using UnityEngine;

using NaughtyAttributes;

using Helpers.Events;
using Helpers.UnityEnums;
// Original Authors - Wyatt Senalik

namespace Atma
{
    /// <summary>
    /// Shows an arrow facing the leap direction, pointing away from the leap object.
    /// </summary>
    public sealed class ArrowPreviewLeapObject : MonoBehaviour, ILeapObject
    {
        public bool availableToUse
        {
            get => m_availToUse;
            set => m_availToUse = value;
        }
        public IEventPrimer onLeptFrom => m_onLeptFrom;
        public Vector2 leapObjectPos => new Vector2(transform.position.x, transform.position.y) + m_centerPosOffset;

        [SerializeField, Required] private GameObject m_dirArrowActiveObj = null;
        [SerializeField, Required] private Transform m_promptTrans = null;
        [SerializeField, Required] private Transform m_dirArrowTrans = null;
        [SerializeField, Min(0.0f)] private float m_radius = 1.0f;
        [SerializeField] private bool m_availToUse = true;
        [SerializeField, Min(0.0f)] private float m_promptDistFromArrow = 0.5f;
        [SerializeField] private Vector2 m_centerPosOffset = Vector2.zero;
        [SerializeField] private MixedEvent m_onLeptFrom = new MixedEvent();
        [SerializeField] private bool m_showDebugLeapPositions = false;
        [SerializeField, Required] private SoundRecorder m_springSoundRecorder = null;


        private void Awake()
        {
            #region Asserts
            //CustomDebug.AssertSerializeFieldIsNotNull(m_dirArrowActiveObj, nameof(m_dirArrowActiveObj), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_promptTrans, nameof(m_promptTrans), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_dirArrowTrans, nameof(m_dirArrowTrans), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_springSoundRecorder, nameof(m_springSoundRecorder), this);
            #endregion Asserts
        }


        public void OnLeapHighlight(eEightDirection desiredLeapDir)
        {
            m_dirArrowActiveObj.SetActive(true);

            Vector2 t_pos2D = transform.position;
            Vector2 t_leapDir = desiredLeapDir.ToOffset();
            m_dirArrowTrans.position = t_pos2D + (t_leapDir * m_radius);
            float t_angle = Mathf.Atan2(t_leapDir.y, t_leapDir.x);
            m_dirArrowTrans.rotation = Quaternion.Euler(0.0f, 0.0f, t_angle * Mathf.Rad2Deg);

            PositionPrompt(desiredLeapDir);
        }
        public void OnLeapHighlightEnd()
        {
            m_dirArrowActiveObj.SetActive(false);
        }
        public void OnLeptFrom()
        {
            m_springSoundRecorder.Play();
            m_onLeptFrom.Invoke();
        }


        private void PositionPrompt(eEightDirection desiredLeapDir)
        {
            Vector2 t_pos2D = transform.position;
            Vector2 t_arrowPos = m_dirArrowTrans.position;

            Vector2 t_promptFromArrowDir;
            switch (desiredLeapDir)
            {
                case eEightDirection.Left:
                case eEightDirection.Right:
                    t_promptFromArrowDir = Vector2.up;
                    break;
                case eEightDirection.Up:
                case eEightDirection.Down:
                    t_promptFromArrowDir = Vector2.left;
                    break;
                case eEightDirection.LeftUp:
                case eEightDirection.RightDown:
                    t_promptFromArrowDir = new Vector2(1, 1).normalized;
                    break;
                case eEightDirection.RightUp:
                case eEightDirection.LeftDown:
                    t_promptFromArrowDir = new Vector2(-1, 1).normalized;
                    break;
                default:
                    t_promptFromArrowDir = Vector2.zero;
                    CustomDebug.UnhandledEnum(desiredLeapDir, this);
                    break;
            }
            m_promptTrans.position = t_arrowPos + t_promptFromArrowDir * m_promptDistFromArrow;
        }



        private Leaper m_playerLeaper = null;
        private void OnDrawGizmos()
        {
            if (!m_showDebugLeapPositions) { return; }
            if (m_playerLeaper == null)
            {
                m_playerLeaper = FindObjectOfType<Leaper>();
                if (m_playerLeaper == null) { return; }
            }
            Gizmos.color = Color.yellow;
            for (int i = 1; i <= 4; ++i)
            {
                Vector2 t_leapEndPos = Leaper.GetLeapEndPosition((eEightDirection)i, this, m_playerLeaper.leapDistance);
                Gizmos.DrawWireSphere(t_leapEndPos, 0.1f);

                t_leapEndPos = Leaper.GetLeapEndPosition((eEightDirection)(-i), this, m_playerLeaper.leapDistance);
                Gizmos.DrawWireSphere(t_leapEndPos, 0.1f);
            }
        }
    }
}
