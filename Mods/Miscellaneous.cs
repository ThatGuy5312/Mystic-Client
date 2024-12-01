using static MysticClient.Menu.Main;
using UnityEngine;
using MysticClient.Utils;
using System.IO;
using Photon.Pun;
using MysticClient.Notifications;

namespace MysticClient.Mods
{
    public class Miscellaneous : GunLib
    {
        public static void SaveAllIDs()
        {
            if (PhotonSystem.InRoom)
            {
                string text = "";
                foreach (NetPlayer player in PhotonSystem.AllNetPlayers)
                {
                    text = string.Concat(new string[]
                    {
                     text,
                     "Player Name: ",
                     player.NickName,
                     " Player ID: ",
                     player.UserId,
                     "\n"
                    });
                }
                NotifiLib.SendNotification("Saved txt File To MysticClient\\Miscellaneous");
                File.WriteAllText($"MysticClient\\Miscellaneous\\{PhotonNetwork.CurrentRoom.Name}_PlayerIDs.txt", text);
            }
        }
    }
}