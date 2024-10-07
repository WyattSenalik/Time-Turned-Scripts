using System;
using UnityEngine;

using NaughtyAttributes;

using Dialogue;
using Dialogue.ConvoActions.Programmed;
using Helpers.SceneLoading;
// Original Authors - Wyatt Senalik

namespace Atma.Tutorial
{
    /// <summary>
    /// Hides dialouge box and starts loading scene to new level.
    /// </summary>
    public sealed class LeaveCallToActionScene : MonoBehavEndpointProgrammedConvoAction
    {
        [SerializeField, Scene] private string m_sceneToLoad = "";
        [SerializeField] private bool m_isRoomToRoom = false;
        private SceneLoader m_sceneLoader = null;


        private void Start()
        {
            m_sceneLoader = SceneLoader.instance;
            #region Asserts
            //CustomDebug.AssertSingletonIsNotNull(m_sceneLoader, this);
            #endregion Asserts
        }


        public override void Begin(ConvoData convoData, Action onFinished = null)
        {
            convoData.dialougeBoxDisplay.Hide(() =>
            {
                // When done hiding the dialogue, start changing the scene.
                m_sceneLoader.LoadScene(m_sceneToLoad, m_isRoomToRoom);
            });
        }
        public override bool Advance(ConvoData convoData)
        {
            // Will never happen, so its fine.
            return false;
        }
    }
}