using System.Collections.Generic;
using UnityEngine;

using NaughtyAttributes;

using Timed;
// Original Authors - Eslis Vang
// Tweaked by Wyatt Senalik

namespace Atma.UI
{
    /// <summary>
    /// Manages the charges the timeline has. Updates them to reflect the currently used charges.
    /// </summary>
	public sealed class ChargeManager : MonoBehaviour
	{
        [SerializeField, Required] private CloneManager m_cloneMan = null;

        [SerializeField] private Charge[] m_charges = new Charge[0];

        private GlobalTimeManager m_timeMan = null;
        private LevelOptions m_levelOptions = null;


        private void Awake()
        {
            #region Asserts
            //CustomDebug.AssertSerializeFieldIsNotNull(m_cloneMan, nameof(m_cloneMan), this);
            #endregion Asserts
        }
        private void Start()
        {
            m_timeMan = GlobalTimeManager.instance;
            m_levelOptions = LevelOptions.instance;
            #region Asserts
            //CustomDebug.AssertSingletonIsNotNull(m_timeMan, this);
            //CustomDebug.AssertSingletonIsNotNull(m_levelOptions, this);
            #endregion Asserts

            InitializeChargeAmount();
        }


        private void Update()
        {
            // Reset all and then we will use the ones that should be used.
            ResetAllCharges();
            // Figure out which charges are currently being used and use them.
            List<TimeClone> t_clones = m_cloneMan.GetClonesActiveAtOrBeforeTime(m_timeMan.curTime);
            foreach (TimeClone t_singleClone in t_clones)
            {
                UseCharge(t_singleClone.cloneData.occupyingCharge);
            }
        }

        /// <summary>
        /// Uses the charge with the given charge index.
        /// </summary>
        private void UseCharge(int index)
        {
            #region Asserts
            //CustomDebug.AssertIndexIsInRange(index, m_charges, this);
            #endregion Asserts
            m_charges[index].UseCharge();
        }
        /// <summary>
        /// Resets all the charges.
        /// </summary>
        private void ResetAllCharges()
        {
            foreach (Charge t_charge in m_charges)
            {
                t_charge.ResetCharge();
            }
        }

        /// <summary>
        /// Turns on or off the charge game objects to match the amount the player has for this level.
        /// </summary>
        private void InitializeChargeAmount()
        {
            if (m_levelOptions.maxCloneCharges > m_charges.Length)
            {
                #region Logs
                //CustomDebug.LogErrorForComponent($"Not enough charges for the clone manager. Has only ({m_charges.Length}). Needs ({m_levelOptions.maxCloneCharges}).", this);
                #endregion Logs
                return;
            }
            for (int i = 0; i < m_levelOptions.maxCloneCharges; ++i)
            {
                #region Asserts
                //CustomDebug.AssertIndexIsInRange(i, m_charges, this);
                #endregion Asserts
                m_charges[i].Show();
            }
            for (int i = m_levelOptions.maxCloneCharges; i < m_charges.Length; ++i)
            {
                #region Asserts
                //CustomDebug.AssertIndexIsInRange(i, m_charges, this);
                #endregion Asserts
                m_charges[i].Hide();
            }
        }
    }
}
