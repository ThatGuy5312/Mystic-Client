using GorillaNetworking;
using Photon.Pun;
using System.Collections;
using UnityEngine;

namespace MysticClient.Mods
{
    public class Player : MonoBehaviour
    {
        public static void SetName(string PlayerName)
        {
            PhotonNetwork.LocalPlayer.NickName = PlayerName;
            PhotonNetwork.NickName = PlayerName;
            PhotonNetwork.NetworkingClient.NickName = PlayerName;
            GorillaComputer.instance.currentName = PlayerName;
            GorillaComputer.instance.savedName = PlayerName;
            GorillaComputer.instance.offlineVRRigNametagText.text = PlayerName;
            NetworkSystem.Instance.SetMyNickName(PlayerName);
            ModIOMapsTerminal.RequestDriverNickNameRefresh();
            PlayerPrefs.SetString("playerName", PlayerName);
            PlayerPrefs.Save();
        }
    }
}