using GorillaLocomotion.Gameplay;
using MysticClient.Utils;
using Photon.Pun;
using Photon.Realtime;
using static MysticClient.Menu.Main;
using UnityEngine;
using GorillaTagScripts;
using Viveport;

namespace MysticClient.Classes
{
    public class RPCManager : MonoBehaviour
    {
        public static void LagEvent(Player target)
        {
            SendRPC(FriendshipGroupDetection.Instance.photonView, "RPC_NotifyNoPartyToMerge", target, new object[1]);
        }
        public static void LagEvent(RpcTarget target)
        {
            SendRPC(FriendshipGroupDetection.Instance.photonView, "RPC_NotifyNoPartyToMerge", target, new object[1]);
        }
        private static float dropDelay = 0f;
        public static void PieceEvent(int piece, Vector3 pos, Quaternion rot)
        {
            if (Time.time > dropDelay)
            {
                BuilderTableNetworking.instance.RequestCreatePiece(piece, pos, rot, 1);
                dropDelay = Time.time + .2f;
            }
        }
        public static void VibrateEvent(NetEventOptions.RecieverTarget target)
        {
            var status = new object[1]; status[0] = 1;
            object[] content = { (byte)2, status };
            SendEvent(3, content, new NetEventOptions
            {
                Reciever = target
            }, false);
        }
        public static void VibrateEvent(Player target)
        {
            var status = new object[1]; status[0] = 1;
            object[] content = { (byte)2, status };
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
            var status = new object[1]; status[0] = 0;
            object[] content = { (byte)2, status };
            SendEvent(3, content, new NetEventOptions
            {
                Reciever = target
            }, false);
        }
        public static void SlowEvent(Player target)
        {
            var status = new object[1]; status[0] = 0;
            object[] content = { (byte)2, status };
            SendEvent(3, content, new NetEventOptions
            {
                TargetActors = new int[1]
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
        private static float tagSoundDelay = 0f;
        public static void TagSoundEvent(NetEventOptions.RecieverTarget target, object[] args)
        {
            if (!PhotonSystem.InRoom || !PhotonSystem.IsMasterClient)
            {
                RigUtils.MyOfflineRig.PlayTagSoundLocal((int)args[0], (float)args[1], false);
                return;
            }
            if (Time.time > tagSoundDelay)
            {
                SendEvent(3, args, new NetEventOptions
                {
                    Reciever = target
                }, false);
                tagSoundDelay = Time.time + .2f;
            }
        }
        public static void TagSoundEvent(Player target, object[] args)
        {
            if (!PhotonSystem.InRoom)
            {
                RigUtils.MyOfflineRig.PlayTagSoundLocal((int)args[0], (float)args[1], false);
                return;
            }
            if (Time.time > tagSoundDelay)
            {
                SendEvent(3, args, new NetEventOptions
                {
                    TargetActors = new int[]
                    {
                        RigUtils.GetNetFromPlayer(target).ActorNumber
                    }
                }, false);
                tagSoundDelay = Time.time + .2f;
            }
        }
        public static void SoundEvent(RpcTarget target, int index, bool hand, float volume)
        {
            if (!PhotonSystem.InRoom)
            {
                RigUtils.MyOfflineRig.PlayHandTapLocal(index, GetEnabled("Right Hand Menu"), volume);
                return;
            }
            SendRPC(RigUtils.MyPhotonView, "RPC_PlayHandTap", target, new object[] { index, hand, volume });
        }
        public static void SoundEvent(Player target, int index, bool hand, float volume)
        {
            if (!PhotonSystem.InRoom)
            {
                RigUtils.MyOfflineRig.PlayHandTapLocal(index, GetEnabled("Right Hand Menu"), volume);
                return;
            }
            SendRPC(RigUtils.MyPhotonView, "RPC_PlayHandTap", target, new object[] { index, hand, volume });
        }
        private static float ropeDelay = 0f;
        public static void RopeEvent(Vector3 force, RpcTarget target)
        {
            if (Time.time > ropeDelay)
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
                    ropeDelay = Time.time + .25f;
                }
        }
        public static void RopeEvent(Vector3 force, GorillaRopeSwing rope, RpcTarget target)
        {
            if (Time.time > ropeDelay)
            {
                SendRPC(RopeSwingManager.instance.photonView, "SetVelocity", target, new object[]
                {
                    rope.ropeId,
                    1,
                    force,
                    true,
                    null
                });
                ropeDelay = Time.time + .25f;
            }
        }
    }
}