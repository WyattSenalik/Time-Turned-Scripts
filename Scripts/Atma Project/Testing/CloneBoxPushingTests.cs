using Atma.UI;
using Helpers.Extensions;
using Helpers.Physics.Custom2DInt;
using Helpers.UnityEnums;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atma.Testing
{
    public sealed class CloneBoxPushingTests : MonoBehaviour
    {
        [SerializeField] private MoveSpec[] m_moveSpecs = new MoveSpec[4];


        private IEnumerator Start()
        {
            PlayerSingleton t_player = PlayerSingleton.instance;
            Int2DTransform t_intTransform = t_player.GetComponentSafe<Int2DTransform>();
            TimeManipController t_timeManipCont = t_player.GetComponentSafe<TimeManipController>();
            PlayerMovement t_playerMove = t_player.GetComponentSafe<PlayerMovement>();

            // Wait for game to start up and time to pause.
            yield return new WaitForSeconds(1.5f);

            t_timeManipCont.Play();

            // Wait for timeline animation to end
            yield return new WaitForSeconds(1.5f);

            foreach (MoveSpec t_moveSpec in m_moveSpecs)
            {
                if (t_moveSpec.isTeleport)
                {
                    t_intTransform.positionFloat = t_moveSpec.teleportPos;
                }
                else
                {
                    t_playerMove.FakeMoveInput(t_moveSpec.dir);
                }
                yield return new WaitForSeconds(t_moveSpec.time);
            }

            t_timeManipCont.BeginTimeManipulation();
            // Wait for timeline animation to end
            yield return new WaitForSeconds(1.5f);

            t_timeManipCont.CreateTimeClone();
        }


        [Serializable]
        public sealed class MoveSpec
        {
            public bool isTeleport => m_isTeleport;
            public Vector2 teleportPos => m_teleportPos;
            public float time => m_time;

            [SerializeField] private bool m_isTeleport = false;
            [SerializeField] private Vector2 m_teleportPos = Vector2.zero;
            [SerializeField] private eDir m_dir = eDir.None;
            [SerializeField] private float m_time = 0.0f;


            public Vector2 dir
            {
                get
                {
                    switch (m_dir)
                    {
                        case eDir.None: return Vector2.zero;
                        case eDir.Left: return Vector2.left;
                        case eDir.Right: return Vector2.right;
                        case eDir.Up: return Vector2.up;
                        case eDir.Down: return Vector2.down;
                        default: return Vector2.zero;
                    }
                }
            }
        }
        public enum eDir { None, Left, Right, Up, Down }
    }
}