using GorillaLocomotion.Gameplay;
using MysticClient.Menu;
using MysticClient.Utils;
using Photon.Pun;
using Photon.Realtime;
using static MysticClient.Menu.Main;
using UnityEngine;

namespace MysticClient.Classes
{
    public class RPCManager : MonoBehaviour
    {
        public static void DropBlock(BuilderPiece piece, Vector3 pos, Quaternion rot)
        {
            object[] parms = { piece.pieceType, VRRig.PackWorldPosForNetwork(pos), VRRig.PackQuaternionForNetwork(rot), piece.materialType };
            new PhotonView().RPC("RequestCreatePieceRPC", RpcTarget.MasterClient, parms);
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
        public static void WaterEvent(RpcTarget target, object[] args)
        {
            new PhotonView().RPC("RPC_PlaySplashEffect", target, new object[]
            {
             (Vector3)args[0],
             (Quaternion)args[1],
             4f,
             100f,
             true,
             false
            });
        }
        public static void WaterEvent(Player target, object[] args)
        {
            new PhotonView().RPC("RPC_PlaySplashEffect", target, new object[]
            {
             (Vector3)args[0],
             (Quaternion)args[1],
             4f,
             100f,
             true,
             false
            });
        }
        public static void TagSoundEvent(NetEventOptions.RecieverTarget target, object[] args)
        {
            if (!PhotonSystem.InRoom)
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
        public static void SoundEvent(RpcTarget target, object[] args)
        {
            new PhotonView().RPC("RPC_PlayHandTap", target, args);
        }
        public static void SoundEvent(Player target, object[] args)
        {
            new PhotonView().RPC("RPC_PlayHandTap", target, args);
        }
        /*public static void ProjectileEvent(RpcTarget target, object[] args)
        {
            new PhotonView().RPC("LaunchSlingshotProjectile", target, args);
        }
        public static void ProjectileEvent(Player target, object[] args)
        {
            new PhotonView().RPC("LaunchSlingshotProjectile", target, args);
        }*/
        public static void RopeEvent(Vector3 force, RpcTarget target)
        {
            foreach (var rope in GetRopes())
            {
                RopeSwingManager.instance.photonView.RPC("SetVelocity", target, new object[]
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
            RopeSwingManager.instance.photonView.RPC("SetVelocity", target, new object[]
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