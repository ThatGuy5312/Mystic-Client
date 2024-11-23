using GorillaLocomotion.Gameplay;
using HarmonyLib;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static MysticClient.Menu.Main;

namespace MysticClient.Utils
{
    public class RigUtils : MonoBehaviour
    {

        public static NetPlayer GetPlayerFromNick(string nick)
        {
            NetPlayer foundplayer = null;
            List<NetPlayer> list = NetworkSystem.Instance.AllNetPlayers.ToList();
            for (int l = 0; l < list.Count; l++)
            {
                if (list[l].NickName == nick)
                {
                    foundplayer = list[l];
                    break;
                }
            }
            return foundplayer;
        }
        public static VRRig[] VRRigs { get {  return GorillaParent.instance.vrrigs.ToArray(); } }
        public static NetworkView MyNetworkView { get { return GorillaTagger.Instance.myVRRig; } }
        public static VRRig MyOfflineRig { get { return GorillaTagger.Instance.offlineVRRig; } }
        public static NetPlayer MyNetPlayer { get { return PhotonSystem.LocalPlayer; } }
        public static Player MyRealtimePlayer { get { return MyNetPlayer.GetPlayerRef(); } }
        public static GorillaLocomotion.Player MyPlayer { get { return GorillaLocomotion.Player.Instance; } }
        public static GorillaTagger MyOnlineRig { get { return GorillaTagger.Instance; } }
        public static PhotonView MyPhotonView { get { return GorillaTagger.Instance.myVRRig.GetView; } }
        public static VRRig GetClosestVRRig(float threshold)
        {
            VRRig rig = null;
            foreach (VRRig vrrig in VRRigs)
            {
                if (Vector3.Distance(MyOnlineRig.bodyCollider.transform.position, vrrig.transform.position) < threshold && vrrig != MyOfflineRig)
                {
                    threshold = Vector3.Distance(MyOnlineRig.bodyCollider.transform.position, vrrig.transform.position);
                    rig = vrrig;
                }
            }
            return rig;
        }
        public static Player GetPlayerFromNet(NetPlayer net)
        {
            return net.GetPlayerRef();
        }
        public static NetPlayer GetNetFromRig(VRRig vrrig)
        {
            return vrrig.Creator;
        }
        public static NetPlayer GetNetFromPlayer(Player player)
        {
            return GetRigFromPlayer(player).Creator;
        }
        public static VRRig GetRigFromPlayer(NetPlayer p)
        {
            return GorillaGameManager.instance.FindPlayerVRRig(p);
        }
        public static PhotonView GetViewFromPlayer(NetPlayer p)
        {
            return GetViewFromRig(GetRigFromPlayer(p));
        }
        public static PhotonView GetViewFromRig(VRRig rig)
        {
            return (PhotonView)Traverse.Create(rig).Field("photonView").GetValue();
        }
        public static NetPlayer GetPlayerFromRig(VRRig rig)
        {
            return GetViewFromRig(rig).Owner;
        }
        public static GorillaRopeSwing[] GetPlayersRopes(VRRig rig)
        {
            return (GorillaRopeSwing[])Traverse.Create(rig).Field("currentRopeSwing").GetValue();
        }
        public static bool battleIsOnCooldown(VRRig rig)
        {
            return rig.mainSkin.material.name.Contains("hit");
        }
        public static VRRig GetRandomVRRig(bool includeSelf)
        {
            VRRig random = GorillaParent.instance.vrrigs[Random.Range(0, GorillaParent.instance.vrrigs.Count - 1)];
            if (includeSelf)
            {
                return random;
            }
            else
            {
                if (random != GorillaTagger.Instance.offlineVRRig)
                {
                    return random;
                }
                else
                {
                    return GetRandomVRRig(includeSelf);
                }
            }
        }
        public static NetPlayer GetRandomPlayer(bool includeSelf)
        {
            if (includeSelf)
            {
                NetPlayer p = NetworkSystem.Instance.AllNetPlayers[Random.Range(0, 11)];
                if (p != null)
                {
                    return p;
                }
                return GetRandomPlayer(includeSelf);
            }
            NetPlayer p2 = PhotonNetwork.PlayerListOthers[Random.Range(0, 10)];
            if (p2 != null)
            {
                return p2;
            }
            return GetRandomPlayer(includeSelf);
        }
    }
}