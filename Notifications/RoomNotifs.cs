using HarmonyLib;
using Mono.Security.X509.Extensions;
using MysticClient.Menu;
using MysticClient.Utils;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace MysticClient.Notifications
{
    public class RoomNotifs : MonoBehaviour
    {
        [HarmonyPatch(typeof(MonoBehaviourPunCallbacks), "OnPlayerLeftRoom")]
        private class OnLeave : MonoBehaviour
        {
            private static void Prefix(Player otherPlayer)
            {
                if (otherPlayer != leftPlayer)
                {
                    NotifiLib.SendNotification(NotifUtils.Room() + "Player " + otherPlayer.NickName + " Left The Lobby");
                    leftPlayer = otherPlayer;
                }
            }
            private static Player leftPlayer;
        }
        [HarmonyPatch(typeof(MonoBehaviourPunCallbacks), "OnPlayerEnteredRoom")]
        private class OnJoin : MonoBehaviour
        {
            private static void Prefix(Player newPlayer)
            {
                if (newPlayer != joinedPlayer)
                {
                    NotifiLib.SendNotification(NotifUtils.Room() + "Player " + newPlayer.NickName + " Joined The Lobby");
                    joinedPlayer = newPlayer;
                }
            }
            private static Player joinedPlayer;
        }
        [HarmonyPatch(typeof(MonoBehaviourPunCallbacks), "OnMasterClientSwitched")]
        private class OnMasterSwitched : MonoBehaviour
        {
            private static void Prefix(Player newMasterClient)
            {
                if (newMasterClient != masterPlayer)
                {
                    NotifiLib.SendNotification(NotifUtils.Room() + "Player " + newMasterClient.NickName + " Is Now Master Client");
                    masterPlayer = newMasterClient;
                }
            }
            private static Player masterPlayer;
        }
    }
}