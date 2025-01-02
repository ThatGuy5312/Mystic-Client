using BepInEx;
using MysticClient.Classes;
using MysticClient.Menu;
using MysticClient.Mods;
using MysticClient.Notifications;
using MysticClient.Patches;
using MysticClient.Utils;
using System;
using System.Collections;
using System.IO;
using System.Linq;
using UnityEngine;

namespace MysticClient
{
    [BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]
    public class Plugin : BaseUnityPlugin
    {
        private IEnumerator afterload;
        private void Start()
        {
            Console.Title = "Mystic Client || Version [" + PluginInfo.Version + "]";
            Patches.Menu.ApplyHarmonyPatches();
            var load = new GameObject("Menu Loader");
            load.AddComponent<Main>();
            load.AddComponent<Settings>();
            load.AddComponent<MUtils>();
            load.AddComponent<UI>();
            DontDestroyOnLoad(load);
            afterload = AfterLoad();
            Load();
        }

        private void Load()
        {
            Debug.Log($"There Are {Settings.systemColorNames.Length} System.Drawing.Color Names Inside The Menu");
            Debug.Log($"There Are {Settings.scolor.Length} System.Drawing.Color Colors Inside The Menu");
            if (Settings.systemColorNames.Length != Settings.scolor.Length)
                Debug.LogError($"The System.Drawing.Color Length [{Settings.scolor.Length}] And The System.Drawing.Color Name Length [{Settings.systemColorNames.Length}] Are Not Matching Up Please Contect A Mystic Client Developer To Get This Fixed");
            Debug.Log($"Current Menu Mod Count {MenuButtonCount}");
            StartCoroutine(afterload);
        }

        private IEnumerator AfterLoad()
        {
            yield return new WaitForSeconds(3f);
            if (File.Exists("MysticClient\\Buttons\\Save.txt"))
            {
                var lines = File.ReadAllLines("MysticClient\\Buttons\\Save.txt");
                foreach (var line in lines)
                    if (line.Contains("Auto Load Save"))
                        Saving.Load();
                foreach (var buttons in Main.GetKeyboardButtons())
                {
                    var colorSetting = buttons.ButtonColorSettings;
                    colorSetting.UnpressedColor = Color.cyan;
                    colorSetting.PressedColor = Color.magenta;
                    buttons.PressButtonColourUpdate();
                }
                StopCoroutine(afterload);
            }
        }

        private int MenuButtonCount => Buttons.buttons.Sum((ButtonInfo[][] i) => i.Length);
    }
}