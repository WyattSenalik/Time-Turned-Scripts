using Atma.Dialogue;
using Atma.Tutorial;
using Helpers.Animation.BetterCurve;
using Helpers.Physics.Custom2DInt;
using System;
using UnityEngine;

namespace Atma
{
    [DisallowMultipleComponent]
    public sealed class ShrinkAsEnterCave : MonoBehaviour
    {
        [SerializeField] private AdvanceToNextLevelTrigger m_nextLevelTrigger = null;

        [SerializeField] private Transform m_agentDirigoTrans = null;
        [SerializeField] private SpriteRenderer[] m_agentDirigoSprRends = null;
        [SerializeField] private Int2DTransform m_playerTrans = null;
        [SerializeField] private SpriteRenderer[] m_playerSprRends = null;

        [SerializeField] private float m_startShrinkYValue = 4.5f;
        [SerializeField] private float m_endShrinkYValue = 5.0f;

        [SerializeField] private Vector2 m_endSize = Vector2.zero;
        [SerializeField] private Color m_fadeToColor = Color.black;

        [SerializeField] private BetterCurve m_progressionCurve = new BetterCurve();

        private ConversationSkipper m_convoSkipper = null;


        private void Start()
        {
            m_convoSkipper = ConversationSkipper.GetInstanceSafe(this);
        }
        private void FixedUpdate()
        {
            if (!m_convoSkipper.ShouldSkipDialogue())
            {
                UpdateSingleCharacter(m_agentDirigoTrans.position.y, (Vector2 size) => { m_agentDirigoTrans.localScale = size; }, m_agentDirigoSprRends, false);
            }
            else
            {
                m_agentDirigoTrans.gameObject.SetActive(false);
            }
            UpdateSingleCharacter(m_playerTrans.positionFloat.y, (Vector2 size) => { m_playerTrans.localSizeFloat = size; }, m_playerSprRends, true);
        }
        private void UpdateSingleCharacter(float charYPos, Action<Vector2> setScaleAction, SpriteRenderer[] charSprRends, bool isPlayer)
        {
            if (charYPos <= m_startShrinkYValue)
            {
                //setScaleAction.Invoke(Vector2.one);
                foreach (SpriteRenderer t_rend in charSprRends)
                {
                    t_rend.color = Color.white;
                } 
            }
            else if (charYPos >= m_endShrinkYValue)
            {
                //setScaleAction.Invoke(m_endSize);
                foreach (SpriteRenderer t_rend in charSprRends)
                {
                    t_rend.color = m_fadeToColor;
                }

                if (isPlayer)
                {
                    m_nextLevelTrigger.StartAdvance();
                }
            }
            else
            {
                float t_evalTime = (charYPos - m_startShrinkYValue) / (m_endShrinkYValue - m_startShrinkYValue);
                float t = m_progressionCurve.Evaluate(t_evalTime);
                //setScaleAction.Invoke(Vector2.Lerp(Vector2.one, m_endSize, t));
                Color t_lerpedCol = Color.Lerp(Color.white, m_fadeToColor, t);
                foreach (SpriteRenderer t_rend in charSprRends)
                {
                    t_rend.color = t_lerpedCol;
                }
            }
        }
    }
}