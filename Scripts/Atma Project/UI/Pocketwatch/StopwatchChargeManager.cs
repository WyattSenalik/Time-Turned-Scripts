using System.Collections.Generic;
using UnityEngine;

using NaughtyAttributes;
using Timed;
using Helpers;
// Original Authors - Wyatt Senalik

namespace Atma.UI
{
    /// <summary>
    /// Manages the charges the stopwatch has. Updates them to reflect the currently used charges.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class StopwatchChargeManager : MonoBehaviour
    {
        [SerializeField, Required] private CloneManager m_cloneMan = null;
        [SerializeField, Required] private InitialGainChargeAnimationController m_initChargeAnimCont = null;

        [SerializeField, Required] private GameObject m_onCharge1 = null;
        [SerializeField, Required] private GameObject m_offCharge1 = null;
        [SerializeField, Required] private GameObject m_onCharge2 = null;
        [SerializeField, Required] private GameObject m_offCharge2 = null;
        [SerializeField, Required] private GameObject m_onCharge3 = null;
        [SerializeField, Required] private GameObject m_offCharge3 = null;

        private GlobalTimeManager m_timeMan = null;
        private LevelOptions m_levelOptions = null;

        private readonly IDLibrary m_chargesBeHiddenLibrary = new IDLibrary();


        private void Awake()
        {
            #region Asserts
            //CustomDebug.AssertSerializeFieldIsNotNull(m_cloneMan, nameof(m_cloneMan), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_initChargeAnimCont, nameof(m_initChargeAnimCont), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_onCharge1, nameof(m_onCharge1), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_offCharge1, nameof(m_offCharge1), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_onCharge2, nameof(m_onCharge2), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_offCharge2, nameof(m_offCharge2), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_onCharge3, nameof(m_onCharge3), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_offCharge3, nameof(m_offCharge3), this);
            #endregion Asserts
        }
        private void Start()
        {
            m_timeMan = GlobalTimeManager.instance;
            m_levelOptions = LevelOptions.GetInstanceSafe();

            InitializeChargeAmount();
        }


        private void Update()
        {
            if (!m_chargesBeHiddenLibrary.AreAllIDsReturned())
            {
                TurnOffEverything();
                m_offCharge1.SetActive(true);
                m_offCharge2.SetActive(true);
                m_offCharge3.SetActive(true);
                return;
            }

            if (!m_initChargeAnimCont.hasFinished)
            {
                int t_amountChargeAnimsFin = m_initChargeAnimCont.GetAmountChargeAnimationsFinished();
                TurnOffEverything();
                if (t_amountChargeAnimsFin >= 1)
                {
                    m_onCharge1.SetActive(true);
                    if (t_amountChargeAnimsFin >= 2)
                    {
                        m_onCharge2.SetActive(true);
                        if (t_amountChargeAnimsFin >= 3)
                        {
                            m_onCharge3.SetActive(true);
                        }
                    }
                }
                return;
            }

            // Reset all and then we will use the ones that should be used.
            ResetAllCharges();
            // Figure out which charges are currently being used and use them.
            List<TimeClone> t_clones = m_cloneMan.GetClonesActiveAtOrBeforeTime(m_timeMan.curTime);
            foreach (TimeClone t_singleClone in t_clones)
            {
                UseCharge(t_singleClone.cloneData.occupyingCharge);
            }
        }

        
        public int RequestActiveChargesBeHidden()
        {
            return m_chargesBeHiddenLibrary.CheckoutID();
        }
        public void CancelRequestForActiveChargesToBeHidden(int requestID)
        {
            m_chargesBeHiddenLibrary.ReturnID(requestID);
        }


        /// <summary>
        /// Uses the charge with the given charge index.
        /// </summary>
        private void UseCharge(int index)
        {
            switch (index)
            {
                case 0:
                {
                    m_onCharge1.SetActive(false);
                    m_offCharge1.SetActive(true);
                    break;
                }
                case 1:
                {
                    m_onCharge2.SetActive(false);
                    m_offCharge2.SetActive(true);
                    break;
                }
                case 2:
                {
                    m_onCharge3.SetActive(false);
                    m_offCharge3.SetActive(true);
                    break;
                }
                default:
                {
                    CustomDebug.UnhandledEnum(index, GetType().Name);
                    break;
                }
            }
        }
        /// <summary>
        /// Resets all the charges.
        /// </summary>
        private void ResetAllCharges()
        {
            if (m_levelOptions.maxCloneCharges >= 1)
            {
                m_onCharge1.SetActive(true);
                m_offCharge1.SetActive(false);
            }
            if (m_levelOptions.maxCloneCharges >= 2)
            {
                m_onCharge2.SetActive(true);
                m_offCharge2.SetActive(false);
            }
            if (m_levelOptions.maxCloneCharges >= 3)
            {
                m_onCharge3.SetActive(true);
                m_offCharge3.SetActive(false);
            }
        }

        /// <summary>
        /// Turns on or off the charge game objects to match the amount the player has for this level.
        /// </summary>
        private void InitializeChargeAmount()
        {
            if (m_levelOptions.maxCloneCharges > 3)
            {
                #region Logs
                //CustomDebug.LogErrorForComponent($"Not enough charges for the clone manager. Has only (3). Needs ({m_levelOptions.maxCloneCharges}).", this);
                #endregion Logs
                return;
            }
            m_onCharge1.SetActive(false);
            m_offCharge1.SetActive(false);
            m_onCharge2.SetActive(false);
            m_offCharge2.SetActive(false);
            m_onCharge3.SetActive(false);
            m_offCharge3.SetActive(false);
            if (m_levelOptions.maxCloneCharges >= 1)
            {
                m_onCharge1.SetActive(true);
            }
            if (m_levelOptions.maxCloneCharges >= 2)
            {
                m_onCharge2.SetActive(true);
            }
            if (m_levelOptions.maxCloneCharges >= 3)
            {
                m_onCharge3.SetActive(true);
            }
        }

        private void TurnOffEverything()
        {
            m_onCharge1.SetActive(false);
            m_offCharge1.SetActive(false);
            m_onCharge2.SetActive(false);
            m_offCharge2.SetActive(false);
            m_onCharge3.SetActive(false);
            m_offCharge3.SetActive(false);
        }
    }
}