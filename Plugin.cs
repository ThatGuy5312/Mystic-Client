using BepInEx;
using MysticClient.Classes;
using MysticClient.Menu;
using MysticClient.Mods;
using MysticClient.Notifications;
using MysticClient.Utils;
using System;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using BepInEx.Configuration;
using Debug = UnityEngine.Debug;
using static MysticClient.Menu.Buttons;
using System.Diagnostics;
using MysticClient.UISetting;

namespace MysticClient
{
    [BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]
    public class Plugin : BaseUnityPlugin
    {
        private IEnumerator afterload;
        private IEnumerator afterafterload;
        private IEnumerator afterafterafterload;

        private string version;

        public static bool updated;
        private static bool hasContinued = false;
        private bool runOnce = true;
        private bool hasChecked = false;

        public static ButtonInfo[][][] buttonBackup;
        private ButtonInfo[][][] continued
        {
            get
            {
                return new ButtonInfo[][][]
                {
                    new ButtonInfo[][]
                    {
                        new ButtonInfo[]
                        {
                            AddButton("Continue", ()=> hasContinued = true, true, false, "Continued Un-updated Mods"),
                            AddButton("Join Discord", ()=> Process.Start("https://discord.gg/78d7rUVV9J"), false, false, "Joined Discord"),
                            AddButton("Open Repository", ()=> Process.Start("https://github.com/ThatGuy5312/Mystic-Client/releases"), false, false, "Opened GitHub Repository"),
                        }
                    },

                    buttonBackup[1]
                };
            }
        }

        void Awake() => FixPlugins();

        private void Start()
        {
            Console.Title = "Mystic Client || Version [" + PluginInfo.Version + "]";
            Patches.Menu.ApplyHarmonyPatches();
            var load = new GameObject("Mystic Menu Loader");
            load.AddComponent<Main>(); // its not that many i swear
            load.AddComponent<Settings>();
            load.AddComponent<MUtils>();
            load.AddComponent<UI>();
            load.AddComponent<DevUI>();
            load.AddComponent<ScreenNotifs>();
            load.AddComponent<BackupInputs>();
            load.AddComponent<UIInputs>();
            load.AddComponent<UIMenu>();
            load.AddComponent<MysticModManager>();
            DontDestroyOnLoad(load);
            afterload = AfterLoad();
            afterafterload = AfterAfterLoad();
            afterafterafterload = AfterAfterAfterLoad();
            Load();
            foreach (var file in Directory.GetFiles("BepInEx/Plugins", "*.dll")) // to fix any issus with menu plugins
            {
                var fileName = Path.GetFileName(file);
                if (fileName.Contains("MysticClient") && fileName != "MysticClient.dll")
                {
                    MUtils.RenameFile($"BepInEx/Plugins/{file}", Path.Combine("BepInEx/Plugins", "MysticClient.dll"));
                    File.Delete(Path.Combine("BepInEx/Plugins", fileName));
                }
            }
        }

        private void Load()
        {
            Debug.Log($"There Are {Settings.systemColorNames.Length} System.Drawing.Color Names Inside The Menu");
            Debug.Log($"There Are {Settings.scolor.Length} System.Drawing.Color Colors Inside The Menu");
            if (Settings.systemColorNames.Length != Settings.scolor.Length)
                Debug.LogError($"you dirty skid what did you do");
            Debug.Log($"Current Menu Mod Count {MenuButtonCount}");

            version = Loaders.GetTextFromURL("https://pastebin.com/raw/4NZJvjn7");
            buttonBackup = buttons;

            var file = new ConfigFile(Path.Combine(Paths.ConfigPath, "MysticClient.cfg"), true);
            Main.fastLoad = file.Bind("Configuration", "Fast Load", false, "Makes the menu load faster but makes some mods mods not work").Value;

            StartCoroutine(afterload);
            StartCoroutine(afterafterload);
            StartCoroutine(afterafterafterload);
            //StartCoroutine(LoadAllMenuSounds());
        }

        private IEnumerator AfterLoad()
        {
            yield return new WaitForSeconds(3f);
            Loaders.MCObject.volume = .3f;
            Loaders.Object.volume = .3f;
            if (Main.fastLoad)
                StartCoroutine(LoadAllMenuSounds());
            StopCoroutine(afterload);
        }

        private IEnumerator AfterAfterLoad()
        {
            yield return new WaitForSeconds(5f);
            if (version == PluginInfo.Version) { updated = true; buttons = buttonBackup; } else
            {
                updated = false;
                buttons = continued;
            }
            hasChecked = true;
            StopCoroutine(afterafterload);
        }

        private IEnumerator AfterAfterAfterLoad()
        {
            yield return new WaitForSeconds(7f);
            if (File.Exists("MysticClient\\Buttons\\Save.txt"))
            {
                var lines = File.ReadAllLines("MysticClient\\Buttons\\Save.txt");
                foreach (var line in lines)
                    if (line.Contains("Auto Load Save"))
                        Saving.Load();
            }
            StopCoroutine(afterafterafterload);
        }

        private void Update()
        {
            if (hasChecked)
                if (!updated)
                    if (hasContinued)
                    {
                        if (!runOnce)
                        {
                            Debug.Log("Restoring original buttons and reloading save.");
                            buttons = buttonBackup;
                            MenuSettings.menuTitle = $"<color=magenta>Mystic</color> <color=blue>Client</color> <color=cyan>V{PluginInfo.Version}</color>";
                            Saving.Load();
                            runOnce = true;
                        }
                    }
                    else
                    {
                        if (runOnce)
                        {
                            Debug.Log("Version is outdated, changing menu look");
                            MenuSettings.NormalColor = Color.red;
                            MenuSettings.ButtonColorEnabled = Color.red;
                            MenuSettings.ButtonColorDisable = Color.red;
                            MenuSettings.menuTitle = "<color=black>OUTDATED VERSION PLEASE UPDATE</color>";
                            runOnce = false;
                        }
                    }
        }

        public static int MenuButtonCount 
        { 
            get 
            {
                int count = 0;
                foreach (var btnss in buttons)
                    foreach (var btns in btnss)
                        count += btns.Length;
                return count;
            } 
        }

        public static IEnumerator LoadAllMenuSounds()
        {
            int index = 0;
            for (int i = 0; i < Main.Stringys[0].Length; i++)
            {
                if (Main.AudioClips[index] == null)
                {
                    yield return MUtils._RunCoroutine(Loaders.GetAudioFromURL(Main.Stringys[0][i], clip =>
                    {
                        Main.AudioClips[index] = clip;
                        if (clip != null) Debug.Log($"Loaded menu clip for index {index}"); 
                        else Debug.Log($"Failed to load menu clip for index {index}");
                    }));
                }
                index++;
            }
            Main.fastLoad = false;
        }

        public static bool CheckForAudio()
        {
            foreach (var a in Main.AudioClips)
            {
                if (a != null)
                    return true;
            }
            return false;
        }


        private void FixPlugins()
        {
            var path = Paths.ConfigPath + "/BepInEx.cfg";
            File.WriteAllText(path, Regex.Replace(File.ReadAllText(path), "HideManagerGameObject = .+", "HideManagerGameObject = true"));
        }
    }
}