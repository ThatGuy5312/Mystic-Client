using BepInEx;
using MysticClient.Menu;
using MysticClient.Notifications;
using MysticClient.Patches;
using System;
using UnityEngine;

namespace MysticClient
{
    [BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]
    public class Plugin : BaseUnityPlugin
    {
        private void Start()
        {
            Console.Title = "Mystic Client || Version [" + PluginInfo.Version + "]";
            Patches.Menu.ApplyHarmonyPatches();
            GameObject load = new GameObject();
            load.AddComponent<Main>();
            //load.AddComponent<NotifiLib>();
            //load.AddComponent<PhotonRegister>();
            DontDestroyOnLoad(load);
        }
    }
}