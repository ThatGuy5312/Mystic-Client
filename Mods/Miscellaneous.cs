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
        public static async void GetCreationDateGun(string btntext)
        {
            if (PhotonSystem.InRoom)
            {
                if (CreateGun(out VRRig rig))
                    NotifiLib.SendNotification($"{rig.Creator.NickName}: {await GetInfoFromPlayer(rig.Creator)}", 1);
            }
            else
            {
                NotifiLib.SendNotification(NotifUtils.Error() + "YOU ARE NOT IN A ROOM");
                GetIndex(btntext).enabled = false;
            }
        }
        public static void SaveIDGun(string btntext)
        {
            if (PhotonSystem.InRoom)
            {
                if (CreateGun(out VRRig rig))
                {
                    var text = $"Name: {rig.Creator.NickName} | UserID: {rig.Creator.UserId}";
                    NotifiLib.SendNotification(text, 1);
                    Directory.CreateDirectory("MysticClient\\Miscellaneous");
                    File.WriteAllText($"MysticClient\\Miscellaneous\\{PhotonNetwork.CurrentRoom.Name}_{rig.Creator.NickName}.txt", text);
                }
            } 
            else
            {
                NotifiLib.SendNotification(NotifUtils.Error() + "YOU ARE NOT IN A ROOM");
                GetIndex(btntext).enabled = false;
            }
        }
        public static void SaveAllIDs(string btntext)
        {
            if (PhotonSystem.InRoom)
            {
                var text = "";
                foreach (var player in PhotonSystem.AllNetPlayers)
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
                NotifiLib.SendNotification(text, 1);
                Directory.CreateDirectory("MysticClient\\Miscellaneous");
                File.WriteAllText($"MysticClient\\Miscellaneous\\{PhotonNetwork.CurrentRoom.Name}_PlayerIDs.txt", text);
            }
            else
            {
                NotifiLib.SendNotification(NotifUtils.Error() + "YOU ARE NOT IN A ROOM");
                GetIndex(btntext).enabled = false;
            }
        }
    }
}