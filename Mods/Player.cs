using GorillaNetworking;
using MysticClient.Menu;
using MysticClient.Utils;
using Photon.Pun;
using System.Collections;
using static MysticClient.Menu.Main;
using UnityEngine;
using GorillaExtensions;
using BepInEx;
using HarmonyLib;

namespace MysticClient.Mods
{
    public class Player : MonoBehaviour
    {
        public static void SetName(string PlayerName) // i did too much
        {
            PhotonNetwork.LocalPlayer.NickName = PlayerName;
            PhotonNetwork.NickName = PlayerName;
            PhotonNetwork.NetworkingClient.NickName = PlayerName;
            GorillaComputer.instance.currentName = PlayerName;
            GorillaComputer.instance.savedName = PlayerName;
            GorillaComputer.instance.SetLocalNameTagText(PlayerName);
            Traverse.Create(GorillaComputer.instance).Method("SetNameTagText", PlayerName);
            PhotonSystem.SetMyNickName(PlayerName);
            RigUtils.MyOfflineRig.playerNameVisible = PlayerName;
            CustomMapsTerminal.RequestDriverNickNameRefresh();
            NetworkSystemPUN.Instance.SetMyNickName(PlayerName);
            PlayerPrefs.SetString("playerNameBackup", PlayerName);
            PlayerPrefs.SetString("playerName", PlayerName);
            PlayerPrefs.Save();
            Traverse.Create(GorillaComputer.instance).Method("UpdateNametagSetting", true);
            GorillaComputer.instance.SetNameBySafety(true);
        }
    }
}