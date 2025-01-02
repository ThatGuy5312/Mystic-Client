using Fusion;
using GorillaLocomotion.Gameplay;
using HarmonyLib;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using static MysticClient.Menu.Main;

namespace MysticClient.Utils
{
    public class RigUtils : MonoBehaviour
    {
        public static Player[] OtherRealtimePlayers { get { return PhotonNetwork.PlayerListOthers; } }
        public static Player[] AllRealtimePlayers { get { return PhotonNetwork.PlayerList; } }
        public static NetPlayer[] NetPlayerOther { get { return PhotonSystem.PlayerListOthers; } }
        public static NetPlayer[] NetPlayers { get { return PhotonSystem.AllNetPlayers; } }
        public static VRRig[] VRRigs { get { return GorillaParent.instance.vrrigs.ToArray(); } }
        public static NetworkView MyNetworkView { get { return GorillaTagger.Instance.myVRRig; } }
        public static VRRig MyOfflineRig { get { return GorillaTagger.Instance.offlineVRRig; } }
        public static NetPlayer MyNetPlayer { get { return PhotonSystem.LocalPlayer; } }
        public static Player MyRealtimePlayer { get { return MyNetPlayer.GetPlayerRef(); } }
        public static GorillaLocomotion.Player MyPlayer { get { return GorillaLocomotion.Player.Instance; } }
        public static GorillaTagger MyOnlineRig { get { return GorillaTagger.Instance; } }
        public static PhotonView MyPhotonView { get { return GorillaTagger.Instance.myVRRig.GetView; } }

        //threshold = Vector3.Distance(MyOnlineRig.bodyCollider.transform.position, vrrig.transform.position);
        public static VRRig GetClosestVRRig(float threshold)
        {
            foreach (var rigs in VRRigs)
                if (Vector3.Distance(MyOnlineRig.bodyCollider.transform.position, rigs.transform.position) < threshold && rigs != MyOfflineRig)
                    return rigs;
            return null;
        }
        public static Player GetPlayerFromNet(NetPlayer net) => net.GetPlayerRef();
        public static NetPlayer GetNetFromRig(VRRig vrrig) => vrrig.Creator;
        public static NetPlayer GetNetFromPlayer(Player player) => GetRigFromPlayer(player).Creator;
        public static VRRig GetRigFromPlayer(NetPlayer p) => GorillaGameManager.StaticFindRigForPlayer(p);
        public static PhotonView GetViewFromPlayer(NetPlayer p) => GetViewFromRig(GetRigFromPlayer(p));
        public static PhotonView GetViewFromRig(VRRig rig) => (PhotonView)Traverse.Create(rig).Field("photonView").GetValue();
        public static NetworkView GetNetViewFromRig(VRRig rig) => (NetworkView)Traverse.Create(rig).Field("netView").GetValue();
        public static NetworkRunner GetRunnerFromRig(VRRig rig) => ((NetworkView)Traverse.Create(rig).Field("netView").GetValue()).Runner;
        public static NetPlayer GetPlayerFromRig(VRRig rig) => GetViewFromRig(rig).Owner;
        public static GorillaRopeSwing[] GetPlayersRopes(VRRig rig) => (GorillaRopeSwing[])Traverse.Create(rig).Field("currentRopeSwing").GetValue();
        public static bool BattleCooldown(VRRig rig) => rig.mainSkin.material.name.Contains("hit");
        public static VRRig GetRandomVRRig(bool includeSelf)
        {
            var rig = VRRigs[Random.Range(0, VRRigs.Length)];
            if (rig != MyOfflineRig && !includeSelf)
                return rig; else return rig;
        }
        public static NetPlayer GetRandomPlayer(bool includeSelf)
        {
            var player = NetPlayers[Random.Range(0, NetPlayers.Length)];
            if (player != MyNetPlayer && !includeSelf)
                return player; else return player;
        }
        public static NetPlayer GetPlayerFromNick(string nick)
        {
            foreach (var players in NetPlayers)
                if (players.NickName == nick)
                    return players;
            return null;
        }
    }
}