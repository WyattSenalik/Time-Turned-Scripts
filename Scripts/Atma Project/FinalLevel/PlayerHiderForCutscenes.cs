using UnityEngine;

using Timed.TimedComponentImplementations;
using NaughtyAttributes;
// Original Authors - Wyatt Senalik

namespace Atma
{
    /// <summary>
    /// Used during final level cutscenes to hide the player. Unlike <see cref="FakePlayerVisuals"/>, it has nothing to do with reaching new quadrants or the associated animation.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class PlayerHiderForCutscenes : MonoBehaviour
    {
        [SerializeField] private TimedSpriteRenderer[] m_timedSprRendsToToggle = new TimedSpriteRenderer[0];
        [SerializeField] private GameObject[] m_realPlayerGOsToToggle = new GameObject[0];
        [SerializeField] private SpriteRenderer[] m_fakePlayerSprRendsToToggle = new SpriteRenderer[0];
        [SerializeField] private GameObject[] m_fakePlayerGOsToToggle = new GameObject[0];
        [SerializeField, Required] private AppearFromFloorPortal m_appearFromFloorPortal = null;
        [SerializeField] private Transform m_newQuadFakePlayerTrans = null;


        private void Start()
        {
            HideFakePlayer();
            ToggleSubscriptions(true);
        }
        private void OnDestroy()
        {
            ToggleSubscriptions(false);
        }


        public void HidePlayer()
        {
            TogglePlayerShown(false);
        }
        public void ShowPlayer()
        {
            TogglePlayerShown(true);
        }
        public void TogglePlayerShown(bool toggle)
        {
            foreach (TimedSpriteRenderer t_timedRenderer in m_timedSprRendsToToggle)
            {
                t_timedRenderer.enabled = toggle;
                t_timedRenderer.renderer.enabled = toggle;
            }
            foreach (GameObject t_realPlayerGO in m_realPlayerGOsToToggle)
            {
                t_realPlayerGO.SetActive(toggle);
            }
        }
        public void TeleportThisToNewQuadrantFakePlayer()
        {
            transform.position = m_newQuadFakePlayerTrans.position;
        }
        public void ShowFakePlayer() => ToggleFakePlayer(true);
        public void HideFakePlayer() => ToggleFakePlayer(false);


        private void ToggleSubscriptions(bool toggle)
        {
            if (m_appearFromFloorPortal != null)
            {
                m_appearFromFloorPortal.onSkipToEnd.ToggleSubscription(ShowFakePlayer, toggle);
                m_appearFromFloorPortal.onSpawnPlayer.ToggleSubscription(ShowFakePlayer, toggle);
            }
        }
        private void ToggleFakePlayer(bool toggle)
        {
            foreach (SpriteRenderer t_fakePlayerSprRend in m_fakePlayerSprRendsToToggle)
            {
                t_fakePlayerSprRend.enabled = toggle;
            }
            foreach (GameObject t_fakePlayerGO in m_fakePlayerGOsToToggle)
            {
                t_fakePlayerGO.SetActive(toggle);
            }
        }
    }
}