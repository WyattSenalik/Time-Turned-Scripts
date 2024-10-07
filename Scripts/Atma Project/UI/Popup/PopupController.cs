using System;
using System.Collections.Generic;
using UnityEngine;

using NaughtyAttributes;

using Helpers.Singletons;
using System.Collections;
// Original Authors - Jack Dekko, Wyatt Senalik

namespace Atma.UI
{
    /// <summary>
    /// Controls popup behavior that supports all popups
    /// </summary>
    public sealed class PopupController : SingletonMonoBehaviour<PopupController>
    {
        [SerializeField, Required] private RectTransform m_popupParent = null;

        private readonly Dictionary<string, PopupInstance> m_activeInstances= new Dictionary<string, PopupInstance>();
        private readonly Dictionary<string, PopupInstance> m_inactiveInstances= new Dictionary<string, PopupInstance>();
        
        private readonly Dictionary<string, Coroutine> m_showCorouts = new Dictionary<string, Coroutine>();

        [ShowNativeProperty] private int showCoroutsCount => m_showCorouts.Count;


        protected override void Awake()
        {
            base.Awake();
            #region Asserts
            //CustomDebug.AssertSerializeFieldIsNotNull(m_popupParent, nameof(m_popupParent), this);
            #endregion Asserts
        }


        public string ShowPopup(GameObject popupPrefab)
        {
            // TODO: Validate if popupPrefab is a prefab

            // Get unique id
            PopupDetails t_popupPrefabDetails = popupPrefab.GetComponent<PopupDetails>();
            #region Asserts
            //CustomDebug.AssertComponentOnOtherIsNotNull(t_popupPrefabDetails, popupPrefab, this);
            #endregion Asserts
            string t_popupUniqueID = t_popupPrefabDetails.uniqueID;

            // If already showing, do nothing
            if (m_activeInstances.ContainsKey(t_popupUniqueID))
            {
                return t_popupUniqueID;
            }

            // Check if there is already an existing instance.
            PopupInstance t_popupInstance;
            if (m_inactiveInstances.TryGetValue(t_popupUniqueID, out PopupInstance t_existingInstance))
            {
                t_popupInstance = t_existingInstance;
                // Remove from inactive. (will put into active later).
                m_inactiveInstances.Remove(t_popupUniqueID);
            }
            else
            {
                // Spawn popup since there isn't an existing one already.
                GameObject t_popupInstanceObj = Instantiate(popupPrefab, m_popupParent);
                PopupDetails t_popupInstanceDetails = t_popupInstanceObj.GetComponent<PopupDetails>();
                #region Asserts
                //CustomDebug.AssertComponentOnOtherIsNotNull(t_popupInstanceDetails, t_popupInstanceObj, this);
                #endregion Asserts
                t_popupInstance = new PopupInstance(t_popupInstanceObj, t_popupInstanceDetails);
            }
            PopupDetails t_popupDetails = t_popupInstance.details;

            Type t_newPopupType = t_popupDetails.GetType();
            int t_popupCount = 0;
            int t_popupIndex = 0;
            foreach (PopupInstance t_currentInstance in m_activeInstances.Values)
            {
                PopupDetails t_currentDetails = t_currentInstance.details;
                Type t_curPopupType = t_currentDetails.GetType();
                bool t_isPopupSeen = t_currentDetails.shownState == PopupDetails.eShownState.shown || t_currentDetails.shownState == PopupDetails.eShownState.showing;
                if (t_newPopupType == t_curPopupType && t_isPopupSeen)
                {
                    ++t_popupCount;
                }
            }
            foreach (PopupInstance t_currentInstance in m_activeInstances.Values)
            {
                PopupDetails t_currentDetails = t_currentInstance.details;
                Type t_curPopupType = t_currentDetails.GetType();
                bool t_isPopupSeen = t_currentDetails.shownState == PopupDetails.eShownState.shown || t_currentDetails.shownState == PopupDetails.eShownState.showing;
                if (t_newPopupType == t_curPopupType && t_isPopupSeen)
                {
                    // Count backwards because we want to have the 1st added be the furthest away from the origin.
                    int t_backwardsIndex = t_popupCount - 1 - t_popupIndex;
                    t_currentDetails.OnDuplicateAdded(t_popupCount, t_backwardsIndex);
                    ++t_popupIndex;
                }
            }

            
            string t_key = t_popupDetails.uniqueID;
            // Call show once no other popups are hiding (if any).
            ShowAfterNoPopupsHiding(t_popupDetails);
            m_activeInstances.Add(t_key, t_popupInstance);
            return t_key;
        }
        public void ShowPopupNoReturn(GameObject popupPrefab) => ShowPopup(popupPrefab);
        public void HidePopup(string popupID) => HidePopup(popupID, false);
        public void HidePopup(GameObject popupPrefab) => HidePopup(popupPrefab, false);
        public void HidePopup(GameObject popupPrefab, bool expectedToNotExistSometimes)
        {
            // Get popupDetails component
            PopupDetails t_popupDetails = popupPrefab.GetComponent<PopupDetails>();
            #region Asserts
            //CustomDebug.AssertComponentOnOtherIsNotNull(t_popupDetails, popupPrefab, this);
            #endregion Asserts
            HidePopup(t_popupDetails.uniqueID, expectedToNotExistSometimes);
        }
        public void HidePopup(string popupID, bool expectedToNotExistSometimes)
        {
            if (!m_activeInstances.TryGetValue(popupID, out PopupInstance t_popupInstance))
            {
                #region Logs
                if (!expectedToNotExistSometimes)
                {
                    //CustomDebug.LogWarningForComponent($"Popup with id {popupID} is not being shown, yet it being asked to hide.", this);
                }
                #endregion Logs
                return;
            }
            t_popupInstance.details.Hide();
            // Remove from active and put in inactive.
            m_activeInstances.Remove(popupID);
            m_inactiveInstances.Add(popupID, t_popupInstance);
            // Also remove any show calls that may exist for this popup (since its now hiding)
            if (m_showCorouts.TryGetValue(popupID, out Coroutine t_existingCorout))
            {
                StopCoroutine(t_existingCorout);
                m_showCorouts.Remove(popupID);
            }
        }


        private void ShowAfterNoPopupsHiding(PopupDetails popupDetails)
        {
            if (m_showCorouts.ContainsKey(popupDetails.uniqueID))
            {
                return;
            }
            Coroutine t_corout = StartCoroutine(ShowAfterNoPopupsHidingCoroutine(popupDetails));
            m_showCorouts.Add(popupDetails.uniqueID, t_corout);
        }
        private IEnumerator ShowAfterNoPopupsHidingCoroutine(PopupDetails popupDetails)
        {
            yield return new WaitUntil(() =>
            {
                foreach (PopupInstance t_currentInstance in m_inactiveInstances.Values)
                {
                    PopupDetails t_currentDetails = t_currentInstance.details;
                    if (t_currentDetails.shownState == PopupDetails.eShownState.hiding)
                    {
                        return false;
                    }
                }
                foreach (PopupInstance t_currentInstance in m_activeInstances.Values)
                {
                    PopupDetails t_currentDetails = t_currentInstance.details;
                    if (t_currentDetails.shownState == PopupDetails.eShownState.hiding)
                    {
                        return false;
                    }
                }
                return true;
            });

            m_showCorouts.Remove(popupDetails.uniqueID);
            popupDetails.Show();
        }
    }
}
