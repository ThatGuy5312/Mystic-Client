using GorillaLocomotion.Gameplay;
using MysticClient.Utils;
using Photon.Pun;
using Photon.Realtime;
using static MysticClient.Menu.Main;
using UnityEngine;
using GorillaTagScripts;

namespace MysticClient.Classes
{
    public class RPCManager : MonoBehaviour
    {
        private static float dropDelay = 0f;
        public static void DropPiece(int piece, Vector3 pos, Quaternion rot)
        {
            if (Time.time > dropDelay)
            {
                BuilderTableNetworking.instance.RequestCreatePiece(piece, pos, rot, 1);
                dropDelay = Time.time + .2f;
            }
        }
        public static void VibrateEvent(NetEventOptions.RecieverTarget target)
        {
            object[] content = { 2, 1 };
            SendEvent(3, content, new NetEventOptions
            {
                Reciever = target
            }, false);
        }
        public static void VibrateEvent(Player target)
        {
            object[] content = { 2, 1 };
            SendEvent(3, content, new NetEventOptions
            {
                TargetActors = new int[]
                {
                    RigUtils.GetNetFromPlayer(target).ActorNumber
                }
            }, false);
        }
        public static void SlowEvent(NetEventOptions.RecieverTarget target)
        {
            object[] content = { 2, 0 };
            SendEvent(3, content, new NetEventOptions
            {
                Reciever = target
            }, false);
        }
        public static void SlowEvent(Player target)
        {
            object[] content = { 2, 0 };
            SendEvent(3, content, new NetEventOptions
            {
                TargetActors = new int[]
                {
                    RigUtils.GetNetFromPlayer(target).ActorNumber
                }
            }, false);
        }
        public static void WaterEvent(RpcTarget target, Vector3 pos, Quaternion rot)
        {
            SendRPC(RigUtils.MyPhotonView, "RPC_PlaySplashEffect", target, new object[]
            {
             pos,
             rot,
             4f,
             100f,
             true,
             false
            });
        }
        public static void WaterEvent(Player target, Vector3 pos, Quaternion rot)
        {
            SendRPC(RigUtils.MyPhotonView, "RPC_PlaySplashEffect", target, new object[]
            {
             pos,
             rot,
             4f,
             100f,
             true,
             false
            });
        }
        public static void TagSoundEvent(NetEventOptions.RecieverTarget target, object[] args)
        {
            if (!PhotonSystem.InRoom || !PhotonSystem.IsMasterClient)
            {
                RigUtils.MyOfflineRig.PlayTagSoundLocal((int)args[0], (float)args[1]);
                return;
            }
            SendEvent(3, args, new NetEventOptions
            {
                Reciever = target
            }, false);
        }
        public static void TagSoundEvent(Player target, object[] args)
        {
            if (!PhotonSystem.InRoom)
            {
                RigUtils.MyOfflineRig.PlayHandTapLocal((int)args[0], GetEnabled("Right Hand Menu"), (float)args[1]);
                return;
            }
            SendEvent(3, args, new NetEventOptions
            {
                TargetActors = new int[]
                {
                    RigUtils.GetNetFromPlayer(target).ActorNumber
                }
            }, false);
        }
        public static void SoundEvent(RpcTarget target, int index, bool hand, float volume)
        {
            SendRPC(RigUtils.MyPhotonView, "RPC_PlayHandTap", target, new object[] { index, hand, volume });
        }
        public static void SoundEvent(Player target, int index, bool hand, float volume)
        {
            SendRPC(RigUtils.MyPhotonView, "RPC_PlayHandTap", target, new object[] { index, hand, volume });
        }
        public static void RopeEvent(Vector3 force, RpcTarget target)
        {
            foreach (var rope in GetRopes())
            {
                SendRPC(RopeSwingManager.instance.photonView, "SetVelocity", target, new object[]
                {
                    rope.ropeId,
                    1,
                    force,
                    true,
                    null
                });
            }
        }
        public static void RopeEvent(Vector3 force, GorillaRopeSwing rope, RpcTarget target)
        {
            SendRPC(RopeSwingManager.instance.photonView, "SetVelocity", target, new object[]
            {
                rope.ropeId,
                1,
                force,
                true,
                null
            });
        }
    }
}