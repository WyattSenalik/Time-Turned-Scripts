using System;
using UnityEngine;
using UnityEngine.UI;

using NaughtyAttributes;
// Original Authors - Eslis Vang
// Tweaked by Wyatt Senalik

namespace Atma.UI
{
    /// <summary>
    /// Charge for the stopwatch. Helps to swap its color.
    /// </summary>
    [Serializable]
    public sealed class Charge
    {
        public Image chargeImg => m_charge;
        public Sprite baseSpr => m_baseSpr;
        public Color baseColor => m_baseColor;
        public Sprite usedSpr => m_usedSpr;
        public Color usedColor => m_usedColor;

        [SerializeField, Required] private Image m_charge = null;
        [SerializeField, Required] private Sprite m_baseSpr = null;
        [SerializeField] private Color m_baseColor = Color.magenta;
        [SerializeField, Required] private Sprite m_usedSpr = null;
        [SerializeField] private Color m_usedColor = Color.gray;


        public Charge()
        {
            m_charge = null;
            m_baseSpr = null;
            m_baseColor = Color.magenta;
            m_usedSpr = null;
            m_usedColor = Color.gray;
        }


        public void UseCharge()
        {
            #region Asserts
            //CustomDebug.AssertSerializeFieldIsNotNull(m_charge, nameof(m_charge), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_usedSpr, nameof(m_usedSpr), this);
            #endregion Asserts
            m_charge.sprite = m_usedSpr;
            m_charge.color = m_usedColor;
        }
        public void ResetCharge()
        {
            #region Asserts
            //CustomDebug.AssertSerializeFieldIsNotNull(m_charge, nameof(m_charge), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_baseSpr, nameof(m_baseSpr), this);
            #endregion Asserts
            m_charge.sprite = m_baseSpr;
            m_charge.color = m_baseColor;
        }
        public void Hide()
        {
            m_charge.gameObject.SetActive(false);
        }
        public void Show()
        {
            m_charge.gameObject.SetActive(true);
        }
    } 
}