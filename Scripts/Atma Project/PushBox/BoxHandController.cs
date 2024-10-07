using UnityEngine;

using NaughtyAttributes;
using UnityEngine.Serialization;
using Helpers.Extensions;
// Original Authors - Wyatt Senalik

namespace Atma
{
    [RequireComponent(typeof(PushBox))]
    [DisallowMultipleComponent]
    public sealed class BoxHandController : MonoBehaviour
    {
        public PickupController pickupCont { get; private set; }
        public PushBox pushBox { get; private set; }

        [SerializeField, Required, FormerlySerializedAs("m_topSingleHandObj")] private GameObject m_playerTopSingleHandObj = null;
        [SerializeField, Required, FormerlySerializedAs("m_topDoubleHandObj")] private GameObject m_playerTopDoubleHandObj = null;
        [SerializeField, Required, FormerlySerializedAs("m_botSingleHandObj")] private GameObject m_playerBotSingleHandObj = null;
        [SerializeField, Required, FormerlySerializedAs("m_botDoubleHandObj")] private GameObject m_playerBotDoubleHandObj = null;
        [SerializeField, Required, FormerlySerializedAs("m_leftSingleHandObj")] private GameObject m_playerLeftSingleHandObj = null;
        [SerializeField, Required, FormerlySerializedAs("m_leftDoubleHandObj")] private GameObject m_playerLeftDoubleHandObj = null;
        [SerializeField, Required, FormerlySerializedAs("m_rightSingleHandObj")] private GameObject m_playerRightSingleHandObj = null;
        [SerializeField, Required, FormerlySerializedAs("m_rightDoubleHandObj")] private GameObject m_playerRightDoubleHandObj = null;

        [SerializeField, Required] private GameObject m_cloneTopSingleHandObj = null;
        [SerializeField, Required] private GameObject m_cloneTopDoubleHandObj = null;
        [SerializeField, Required] private GameObject m_cloneBotSingleHandObj = null;
        [SerializeField, Required] private GameObject m_cloneBotDoubleHandObj = null;
        [SerializeField, Required] private GameObject m_cloneLeftSingleHandObj = null;
        [SerializeField, Required] private GameObject m_cloneLeftDoubleHandObj = null;
        [SerializeField, Required] private GameObject m_cloneRightSingleHandObj = null;
        [SerializeField, Required] private GameObject m_cloneRightDoubleHandObj = null;

        [SerializeField, Required] private GameObject m_chaserTopHandObj = null;
        [SerializeField, Required] private GameObject m_chaserBotHandObj = null;
        [SerializeField, Required] private GameObject m_chaserLeftHandObj = null;
        [SerializeField, Required] private GameObject m_chaserRightHandObj = null;


        private void Awake()
        {
            pushBox = GetComponent<PushBox>();
            #region Asserts
            //CustomDebug.AssertComponentIsNotNull(pushBox, this);
            // Player Hands
            //CustomDebug.AssertSerializeFieldIsNotNull(m_playerTopSingleHandObj, nameof(m_playerTopSingleHandObj), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_playerTopDoubleHandObj, nameof(m_playerTopDoubleHandObj), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_playerBotSingleHandObj, nameof(m_playerBotSingleHandObj), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_playerBotDoubleHandObj, nameof(m_playerBotDoubleHandObj), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_playerLeftSingleHandObj, nameof(m_playerLeftSingleHandObj), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_playerLeftDoubleHandObj, nameof(m_playerLeftDoubleHandObj), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_playerRightSingleHandObj, nameof(m_playerRightSingleHandObj), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_playerRightDoubleHandObj, nameof(m_playerRightDoubleHandObj), this);
            // Clone Hands
            //CustomDebug.AssertSerializeFieldIsNotNull(m_cloneTopSingleHandObj, nameof(m_cloneTopSingleHandObj), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_cloneTopDoubleHandObj, nameof(m_cloneTopDoubleHandObj), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_cloneBotSingleHandObj, nameof(m_cloneBotSingleHandObj), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_cloneBotDoubleHandObj, nameof(m_cloneBotDoubleHandObj), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_cloneLeftSingleHandObj, nameof(m_cloneLeftSingleHandObj), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_cloneLeftDoubleHandObj, nameof(m_cloneLeftDoubleHandObj), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_cloneRightSingleHandObj, nameof(m_cloneRightSingleHandObj), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_cloneRightDoubleHandObj, nameof(m_cloneRightDoubleHandObj), this);
            // Chaser Hands
            //CustomDebug.AssertSerializeFieldIsNotNull(m_chaserTopHandObj, nameof(m_chaserTopHandObj), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_chaserBotHandObj, nameof(m_chaserBotHandObj), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_chaserLeftHandObj, nameof(m_chaserLeftHandObj), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_chaserRightHandObj, nameof(m_chaserRightHandObj), this);
            #endregion Asserts
        }
        private void Start()
        {
            PlayerSingleton t_playerSingleton = PlayerSingleton.GetInstanceSafe();
            pickupCont = t_playerSingleton.GetComponentSafe<PickupController>();
        }
        private void FixedUpdate()
        {
            // Turn off all the hands
            TurnOffAllHands();
            // Re-enable the hands that should be on
            if (pushBox.isBeingPushed)
            {
                foreach (PushInfo t_pusherInfo in pushBox.curPusherInfos)
                {
                    GatherHandsForPusher(t_pusherInfo.boxPusher, out GameObject t_lSingle, out GameObject t_lDouble, out GameObject t_rSingle, out GameObject t_rDouble, out GameObject t_tSingle, out GameObject t_tDouble, out GameObject t_bSingle, out GameObject t_bDouble);
                    switch (t_pusherInfo.pushDir)
                    {
                        case ePushDirection.None: break;
                        case ePushDirection.Left:
                        {
                            TurnOnDirectionHands(t_pusherInfo, t_lSingle, t_lDouble);
                            break;
                        }
                        case ePushDirection.Right:
                        {
                            TurnOnDirectionHands(t_pusherInfo, t_rSingle, t_rDouble);
                            break;
                        }
                        case ePushDirection.Up:
                        {
                            TurnOnDirectionHands(t_pusherInfo, t_tSingle, t_tDouble);
                            break;
                        }
                        case ePushDirection.Down:
                        {
                            TurnOnDirectionHands(t_pusherInfo, t_bSingle, t_bDouble);
                            break;
                        }
                        default:
                        {
                            CustomDebug.UnhandledEnum(t_pusherInfo.pushDir, this);
                            break;
                        }
                    }
                }
            }
        }


        private void GatherHandsForPusher(eBoxPusher boxPusher, out GameObject lSingle, out GameObject lDouble, out GameObject rSingle, out GameObject rDouble, out GameObject tSingle, out GameObject tDouble, out GameObject bSingle, out GameObject bDouble)
        {
            switch (boxPusher)
            {
                case eBoxPusher.Player:
                {
                    lSingle = m_playerLeftSingleHandObj;
                    lDouble = m_playerLeftDoubleHandObj;
                    rSingle = m_playerRightSingleHandObj;
                    rDouble = m_playerRightDoubleHandObj;
                    tSingle = m_playerTopSingleHandObj;
                    tDouble = m_playerTopDoubleHandObj;
                    bSingle = m_playerBotSingleHandObj;
                    bDouble = m_playerBotDoubleHandObj;
                    break;
                }
                case eBoxPusher.Clone:
                {
                    lSingle = m_cloneLeftSingleHandObj;
                    lDouble = m_cloneLeftDoubleHandObj;
                    rSingle = m_cloneRightSingleHandObj;
                    rDouble = m_cloneRightDoubleHandObj;
                    tSingle = m_cloneTopSingleHandObj;
                    tDouble = m_cloneTopDoubleHandObj;
                    bSingle = m_cloneBotSingleHandObj;
                    bDouble = m_cloneBotDoubleHandObj;
                    break;
                }
                case eBoxPusher.Chaser:
                {
                    lSingle = m_chaserLeftHandObj;
                    lDouble = m_chaserLeftHandObj;
                    rSingle = m_chaserRightHandObj;
                    rDouble = m_chaserRightHandObj;
                    tSingle = m_chaserTopHandObj;
                    tDouble = m_chaserTopHandObj;
                    bSingle = m_chaserBotHandObj;
                    bDouble = m_chaserBotHandObj;
                    break;
                }
                case eBoxPusher.OtherBox:
                {
                    lSingle = null;
                    lDouble = null;
                    rSingle = null;
                    rDouble = null;
                    tSingle = null;
                    tDouble = null;
                    bSingle = null;
                    bDouble = null;
                    break;
                }
                case eBoxPusher.Door:
                {
                    lSingle = null;
                    lDouble = null;
                    rSingle = null;
                    rDouble = null;
                    tSingle = null;
                    tDouble = null;
                    bSingle = null;
                    bDouble = null;
                    break;
                }
                case eBoxPusher.None:
                {
                    //CustomDebug.LogErrorForComponent($"No hands for a None pusher.", this);
                    lSingle = null;
                    lDouble = null;
                    rSingle = null;
                    rDouble = null;
                    tSingle = null;
                    tDouble = null;
                    bSingle = null;
                    bDouble = null;
                    break;
                }
                default:
                {
                    CustomDebug.UnhandledEnum(boxPusher, this);
                    lSingle = null;
                    lDouble = null;
                    rSingle = null;
                    rDouble = null;
                    tSingle = null;
                    tDouble = null;
                    bSingle = null;
                    bDouble = null;
                    break;
                }
            }
        }
        private void TurnOnDirectionHands(PushInfo pushInfo, GameObject singleHandsObj, GameObject doubleHandsObj)
        {
            switch (pushInfo.handsBeingUsed)
            {
                case 0: break;
                case 1:
                {
                    singleHandsObj.SetActive(true);
                    break;
                }
                case 2:
                {
                    doubleHandsObj.SetActive(true);
                    break;
                }
                default:
                {
                    CustomDebug.UnhandledValue(pushInfo.handsBeingUsed, this);
                    break;
                }
            }
        }
        private void TurnOffAllHands()
        {
            m_playerTopSingleHandObj.SetActive(false);
            m_playerTopDoubleHandObj.SetActive(false);
            m_playerBotSingleHandObj.SetActive(false);
            m_playerBotDoubleHandObj.SetActive(false);
            m_playerLeftSingleHandObj.SetActive(false);
            m_playerLeftDoubleHandObj.SetActive(false);
            m_playerRightSingleHandObj.SetActive(false);
            m_playerRightDoubleHandObj.SetActive(false);

            m_cloneTopSingleHandObj.SetActive(false);
            m_cloneTopDoubleHandObj.SetActive(false);
            m_cloneBotSingleHandObj.SetActive(false);
            m_cloneBotDoubleHandObj.SetActive(false);
            m_cloneLeftSingleHandObj.SetActive(false);
            m_cloneLeftDoubleHandObj.SetActive(false);
            m_cloneRightSingleHandObj.SetActive(false);
            m_cloneRightDoubleHandObj.SetActive(false);

            m_chaserTopHandObj.SetActive(false);
            m_chaserBotHandObj.SetActive(false);
            m_chaserLeftHandObj.SetActive(false);
            m_chaserRightHandObj.SetActive(false);
        }
    }
}