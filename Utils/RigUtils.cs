using Fusion;
using GorillaLocomotion.Gameplay;
using HarmonyLib;
using Photon.Pun;
using Photon.Realtime;
using Photon.Voice.Unity;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Viveport;
using static MysticClient.Menu.Main;

namespace MysticClient.Utils
{
    public class RigUtils : MonoBehaviour
    {
        // cleaned up by ChatGPT 3/21/25

        public static Player[] OtherRealtimePlayers => PhotonNetwork.PlayerListOthers;
        public static Player[] AllRealtimePlayers => PhotonNetwork.PlayerList;
        public static NetPlayer[] NetPlayerOther => PhotonSystem.PlayerListOthers;
        public static NetPlayer[] NetPlayers => PhotonSystem.AllNetPlayers;
        public static VRRig[] VRRigs => GorillaParent.instance.vrrigs.ToArray();
        public static VRRig[] OtherVRRigs => VRRigs.Where(rig => rig != MyOfflineRig).ToArray();
        public static NetworkView MyNetworkView => GorillaTagger.Instance.myVRRig;
        public static VRRig MyOfflineRig => GorillaTagger.Instance.offlineVRRig;
        public static NetPlayer MyNetPlayer => PhotonSystem.LocalPlayer;
        public static Player MyRealtimePlayer => MyNetPlayer.GetPlayerRef();
        public static GorillaLocomotion.GTPlayer MyPlayer => GorillaLocomotion.GTPlayer.Instance;
        public static GorillaTagger MyOnlineRig => GorillaTagger.Instance;
        public static PhotonView MyPhotonView => GorillaTagger.Instance.myVRRig.GetView;
        public static NetworkRunner MyRunner => GorillaTagger.Instance.myVRRig.Runner;
        public static PlayerRef MyPlayerRef => GorillaTagger.Instance.myVRRig.Runner.LocalPlayer;
        public static Recorder MyRecorder => GorillaTagger.Instance.myRecorder;

        public static Vector3 MyVelocity
        {
            get => MyPlayer.bodyCollider.attachedRigidbody.velocity;
            set => MyPlayer.bodyCollider.attachedRigidbody.velocity = value;
        }

        public static VRRig GetClosestVRRig(float threshold) =>
            VRRigs.FirstOrDefault(rig =>
                Vector3.Distance(MyOnlineRig.bodyCollider.transform.position, rig.transform.position) < threshold &&
                rig != MyOfflineRig);

        public static Player GetPlayerFromNet(NetPlayer net) => net.GetPlayerRef();

        public static Player GetPlayerFromRig(VRRig rig) => rig.Creator.GetPlayerRef();

        public static NetPlayer GetNetFromPlayer(Player player) => GetRigFromPlayer(player).Creator;

        public static VRRig GetRigFromPlayer(NetPlayer p) => GorillaGameManager.StaticFindRigForPlayer(p);

        public static PhotonView GetViewFromPlayer(NetPlayer p) => GetViewFromRig(GetRigFromPlayer(p));

        public static PhotonView GetViewFromRig(VRRig rig) =>
            (PhotonView)Traverse.Create(rig).Field("photonView").GetValue();

        public static NetworkView GetNetViewFromRig(VRRig rig) =>
            (NetworkView)Traverse.Create(rig).Field("netView").GetValue();

        public static NetworkRunner GetRunnerFromRig(VRRig rig) => GetNetViewFromRig(rig).Runner;

        public static PlayerRef GetPlayerRefFromRig(VRRig rig) => GetRunnerFromRig(rig).LocalPlayer;

        public static NetPlayer GetNetFromRig(VRRig rig) => rig.Creator;

        public static GorillaRopeSwing[] GetPlayersRopes(VRRig rig) =>
            (GorillaRopeSwing[])Traverse.Create(rig).Field("currentRopeSwing").GetValue();

        public static bool BattleCooldown(VRRig rig) => rig.mainSkin.material.name.Contains("hit");

        public static VRRig GetRandomVRRig(bool includeSelf) =>
            VRRigs.Where(rig => includeSelf || rig != MyOfflineRig)
                .OrderBy(_ => Random.value)
                .FirstOrDefault();

        public static NetPlayer GetRandomPlayer(bool includeSelf) =>
            NetPlayers.Where(player => includeSelf || player != MyNetPlayer)
                .OrderBy(_ => Random.value)
                .FirstOrDefault();

        public static NetPlayer GetPlayerFromNick(string nick) =>
            NetPlayers.FirstOrDefault(player => player.NickName == nick);
    }
}