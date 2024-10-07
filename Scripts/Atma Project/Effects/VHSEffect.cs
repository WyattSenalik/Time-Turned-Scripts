using UnityEngine;
using UnityEngine.UI;

using Helpers.Animation;
using Helpers.Extensions;
using Timed;
// Original Authors - Wyatt Senalik

namespace Atma
{
    public sealed class VHSEffect : MonoBehaviour
    {
        public bool isEffectActive { get; private set; } = false;

        [SerializeField] private ManualSpriteAnimation m_anim = null;
        [SerializeField] private Image m_img = null;
        [SerializeField, Range(0.0f, 1.0f)] private float m_baseAlpha = 0.5f;

        private GlobalTimeManager m_timeMan = null;
        private BranchPlayerController m_playerCont = null;


        private void Start ()
        {
            m_timeMan = GlobalTimeManager.instance;

            PlayerSingleton t_playerSingleton = PlayerSingleton.GetInstanceSafe();
            m_playerCont = t_playerSingleton.GetComponentSafe<BranchPlayerController>();

            ToggleSubscriptions(true);
        }
        private void OnDestroy()
        {
            ToggleSubscriptions(false);
        }
        private void FixedUpdate()
        {
            if (!isEffectActive) { return; }

            ManualSpriteAnimation.Frame t_frame = m_anim.GetFrameForTime(0.0f, m_timeMan.curTime);
            m_img.sprite = t_frame.sprite;

            Color t_col = m_img.color;
            if (Mathf.Abs(m_timeMan.deltaTime) > 0.0f)
            {
                t_col.a = m_baseAlpha;
            }
            else
            {
                t_col.a *= 0.5f;
                if (t_col.a < 0.01f)
                {
                    t_col.a = 0.0f;
                }
            }
            m_img.color = t_col;
        }


        private void ToggleSubscriptions(bool cond)
        {
            if (m_playerCont != null)
            {
                m_playerCont.onBeginTimeManip.ToggleSubscription(OnBeginTimeManip, cond);
                m_playerCont.onEndTimeManip.ToggleSubscription(OnEndTimeManip, cond);
            }
        }
        private void OnBeginTimeManip()
        {
            m_img.enabled = true;
            isEffectActive = true;
        }
        private void OnEndTimeManip()
        {
            m_img.enabled = false;
            isEffectActive = false;
        }
    }
}