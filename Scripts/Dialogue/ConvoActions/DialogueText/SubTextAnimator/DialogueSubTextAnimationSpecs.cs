using System;
using UnityEngine;

using NaughtyAttributes;

using Helpers.Animation.BetterCurve;
using System.Collections.Generic;
// Original Authors - Wyatt Senalik

namespace Dialogue.ConvoActions.Text
{
    /// <summary>
    /// Specifications for animating a dialogue's sub text.
    /// </summary>
    [Serializable]
    public sealed class DialogueSubTextAnimationSpecs
    {
        public bool loop => m_loop;
        public bool waitUntilAfterFullSubTyped => m_waitUntilAfterFullSubTyped;

        [SerializeField] private bool m_loop = true;
        [SerializeField] private bool m_waitUntilAfterFullSubTyped = false;

        #region Vertical Motion
        public bool hasVerticalAnimation => m_hasVerticalAnimation;
        public MotionAnimationSpecs verticalAnimSpecs => m_verticalAnimSpecs;

        [SerializeField]
        private bool m_hasVerticalAnimation = true;
        [SerializeField, ShowIf(nameof(m_hasVerticalAnimation)), AllowNesting]
        private MotionAnimationSpecs m_verticalAnimSpecs = new MotionAnimationSpecs();
        #endregion Vertical Motion


        #region Horizontal Motion
        public bool hasHorizontalAnimation => m_hasHorizontalAnimation;
        public MotionAnimationSpecs horizontalAnimSpecs => m_horizontalAnimSpecs;

        [SerializeField]
        private bool m_hasHorizontalAnimation = false;
        [SerializeField, ShowIf(nameof(m_hasHorizontalAnimation)), AllowNesting]
        private MotionAnimationSpecs m_horizontalAnimSpecs = new MotionAnimationSpecs();
        #endregion Horizontal Motion


        #region Rotational Motion
        public bool hasRotationalAnimation => m_hasRotationalAnimation;
        public MotionAnimationSpecs rotationAnimSpecs => m_rotationAnimSpecs;

        [SerializeField]
        private bool m_hasRotationalAnimation = false;
        [SerializeField, ShowIf(nameof(m_hasRotationalAnimation)), AllowNesting]
        private MotionAnimationSpecs m_rotationAnimSpecs = new MotionAnimationSpecs();
        #endregion Rotational Motion


        #region Color Change
        public bool hasColorAnimation => m_hasColorAnimation;
        public ColorAnimationSpecs colorAnimSpecs => m_colorAnimSpecs;

        [SerializeField]
        private bool m_hasColorAnimation = false;

        [SerializeField, ShowIf(nameof(m_hasColorAnimation)), AllowNesting]
        private ColorAnimationSpecs m_colorAnimSpecs = new ColorAnimationSpecs();

        #endregion Color Change


        public float GetLongestAnimationTime()
        {
            List<float> t_animLengths = new List<float>();
            if (hasVerticalAnimation)
            {
                t_animLengths.Add(verticalAnimSpecs.animCurve.GetEndTime());
            }
            if (hasHorizontalAnimation)
            {
                t_animLengths.Add(horizontalAnimSpecs.animCurve.GetEndTime());
            }
            if (hasRotationalAnimation)
            {
                t_animLengths.Add(rotationAnimSpecs.animCurve.GetEndTime());
            }
            if (hasColorAnimation)
            {
                t_animLengths.Add(colorAnimSpecs.GetLongestUsedAnimationEndTime());
            }
            return Mathf.Max(t_animLengths.ToArray());
        }
    }

    [Serializable]
    public sealed class MotionAnimationSpecs
    {
        public bool isPerChar => m_isPerChar;
        public float delayBetweenChars => m_delayBetweenChars;
        public BetterCurve animCurve => m_animCurve;

        [SerializeField] private bool m_isPerChar = false;
        [SerializeField, ShowIf(nameof(m_isPerChar)), AllowNesting]
        private float m_delayBetweenChars = 0.1f;
        [SerializeField, AllowNesting]
        private BetterCurve m_animCurve = new BetterCurve();
    }
    [Serializable]
    public sealed class ColorAnimationSpecs
    {
        public bool isPerChar => m_isPerChar;
        public float delayBetweenChars => m_delayBetweenChars;
        public ColorCurve animCurve => m_animCurve;
        public bool isPerVertex => m_isPerVertex;
        public ColorCurve curveBL => m_curveBL;
        public ColorCurve curveTL => m_curveTL;
        public ColorCurve curveTR => m_curveTR;
        public ColorCurve curveBR => m_curveBR;


        [SerializeField]
        private bool m_isPerChar = false;
        [SerializeField, ShowIf(nameof(m_isPerChar)), AllowNesting]
        private float m_delayBetweenChars = 0.1f;

        [SerializeField]
        private bool m_isPerVertex = false;
        [SerializeField, ShowIf(nameof(m_isPerVertex)), AllowNesting]
        private ColorCurve m_curveBL = new ColorCurve();
        [SerializeField, ShowIf(nameof(m_isPerVertex)), AllowNesting]
        private ColorCurve m_curveTL = new ColorCurve();
        [SerializeField, ShowIf(nameof(m_isPerVertex)), AllowNesting]
        private ColorCurve m_curveTR = new ColorCurve();
        [SerializeField, ShowIf(nameof(m_isPerVertex)), AllowNesting]
        private ColorCurve m_curveBR = new ColorCurve();

        [SerializeField, ShowIf(nameof(ShowColorAnimCurve)), AllowNesting]
        private ColorCurve m_animCurve = new ColorCurve();

        #region Editor
        private bool ShowColorAnimCurve() => !m_isPerVertex;
        #endregion Editor


        public float GetLongestUsedAnimationEndTime()
        {
            if (m_isPerVertex)
            {
                return Mathf.Max(curveBL.GetEndTime(), curveTL.GetEndTime(), curveTR.GetEndTime(), curveBR.GetEndTime());
            }
            else
            {
                return animCurve.GetEndTime();
            }
        }
    }
}