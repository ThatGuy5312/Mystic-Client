using MysticClient.Classes;
using MysticClient.Menu;
using System.Collections.Generic;
using static MysticClient.Menu.Main;
using static MysticClient.Menu.MenuSettings;
using System.IO;
using UnityEngine;
using UnityEngine.XR;
using static MysticClient.Notifications.NotifUtils;
using static MysticClient.Notifications.NotifiLib;
using static MysticClient.Mods.Projectiles;
using MysticClient.Utils;
using System;
using MysticClient.Notifications;
using System.Collections;
using System.Linq;
using UnityEngine.Windows.Speech;
using static OVRPlugin;
using static MysticClient.Mods.Visuals;
using UnityEngine.UI;
using SColor = System.Drawing.Color;
using Valve.VR.InteractionSystem;
using static UnityEngine.UIElements.UxmlAttributeDescription;
using Viveport;
using TMPro;
using Unity.XR.CoreUtils;
using GorillaNetworking;
using System.Xml;
using Steamworks;
using GorillaExtensions;

namespace MysticClient.Mods
{
    public class Settings : MonoBehaviour
    {
        public static int[] Mode = new int[9999];
        private static PrimitiveType[] shapes = { PrimitiveType.Cube, PrimitiveType.Sphere, PrimitiveType.Capsule, PrimitiveType.Cylinder, PrimitiveType.Plane, PrimitiveType.Quad }; // iidk told me to do (PrimitiveType)Mode[ii] but it breaks so im not
        public static Color[] colors = new Color[] { Color.black, new Color(.541f, .027f, .761f, 1), Color.blue, Color.yellow, new Color(1, .51f, 0, 1), Color.white, Color.cyan, Color.green, Color.red, new Color32(135, 0, 0, 1), Color.magenta, Color.gray };
        public static Color[] unityc = { Color.black, Color.blue, Color.yellow, Color.white, Color.cyan, Color.green, Color.red, Color.magenta, Color.gray };
        private static int[] flySpeeds = { 15, 10, 25 };
        private static int[] speedBoostSpeeds = { 12, 9, 16, 25, int.MaxValue };
        private static int[] times = { 3, 6, 0, 1 };
        private static int[] destroyDelays = { 0, 2, 5, 10, int.MaxValue };
        private static int[] buttonSounds = { 67, 66, 176, 8, 18, 244, 221, 84 };
        private static Vector3[] pointerPoses = { new Vector3(0, -.1f, 0), new Vector3(0, .1f, 0), new Vector3(0, .0666f, .1f) };
        private static int[] mcsoundid = { 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 26 };
        private static int[] pickaxeStrikes = { 4, 3, 2, 1 };
        public static string[][] Names =
        {
            new string[] { "Black", "Purple", "Blue", "Yellow", "Orange", "White", "Cyan", "Green", "Red", "Blood", "Magenta", "Gray" }, // 0
            new string[] { "None", "Trowable", "Throwable And Collidable" }, // 1
            new string[] { "None", "Hands", "Body" }, // 2
            new string[] { "Normal", "Slow", "Fast" }, // 3
            new string[] { "Normal", "Small", "Big", "Huge", "???" }, // 4
            new string[] { "Day", "Dawn", "Night", "Sun Rise", "Untouched" }, // 5
            new string[] { "Bottom", "Sides", "Top" }, // 6
            new string[] { "Normal", "Under", "Over" }, // 7
            new string[] { "Mystic", "ThatGuy", "AZ", "KFJ" }, // 8 (not used)
            new string[] { "Black", "Blue", "Yellow", "White", "Cyan", "Green", "Red", "Magenta", "Gray" }, // 9
            new string[] { "Instant", "2s", "5s", "10s", "Never" }, // 10
            new string[] { "Click", "Key", "Cloud", "Wood", "Metal", "Ding", "Glass", "Bubble" }, // 11
            new string[] { "Sharp", "Boring", "Bendy", "Spiral" }, // 12
            new string[] { "Grass", "Dirt", "Wood", "Leaf", "Oak Plank", "Stone", "Cobblestone", "Hay Bale", "Glass", "Obsidian", "Water", "Trap Door" }, // 13
            new string[] { "Living Mice", "Clark", "Danny", "Oxygene", "Key", "Droopy Likes your Face", "Moog City", "Moog City 2", "Subwoofer Lullaby", "Dog", "Cat", "Aria Math", "Haggstorm", "Pigstep", "Pigstep (Alan Becker)", "Moon 2" }, // 14
            new string[] { "Arial", "Consolas", "Constantia", "Corbel", "Cascadia Mono", "Courier New", "Segoe Print", "Segoe Script", "Gabriola", "Ink Free", "Lucida Console", "Comic Sans MS", "Palatino Linotype", "Impact", "Candara", "Verdana" }, // 15
            new string[] { "4", "3", "2", "1" }, // 16
        };

        public static void ChangeMinecraftPickaxeMaxStrike(string tooltip)
        {
            Mode[30]++;
            if (Mode[30] >= Names[16].Length) { Mode[30] = 0; }
            Fun.pickaxeMaxHit = pickaxeStrikes[Mode[30]];
            string[] text = GetToolTip(tooltip).buttonText.Split(':');
            GetToolTip(tooltip).buttonText = text[0] + ": " + Names[16][Mode[30]];
            GetToolTip(tooltip).enabled = false;
        }
        public static void ChangeMenuTrailColor(string tooltip)
        {
            Mode[29]++;
            var len = GetEnabled("Use System Colors") ? scolor.Length : colors.Length;
            if (Mode[29] >= len) { Mode[25] = 0; }
            var name = GetEnabled("Use System Colors") ? systemColorNames[Mode[29]] : Names[0][Mode[29]];
            menuTrailColor = GetEnabled("Use System Colors") ? SCToUC(scolor[Mode[29]]) : colors[Mode[29]];
            string[] text = GetToolTip(tooltip).buttonText.Split(':');
            GetToolTip(tooltip).buttonText = text[0] + ": " + name;
            GetToolTip(tooltip).enabled = false;
        }
        public static void ChangeFont(string tooltip)
        {
            Mode[28]++;
            if (Mode[28] >= Names[15].Length) { Mode[28] = 0; }
            currentFont = Font.CreateDynamicFontFromOSFont(Names[15][Mode[28]], 1);
            string[] text = GetToolTip(tooltip).buttonText.Split(':');
            GetToolTip(tooltip).buttonText = text[0] + ": " + Names[15][Mode[28]];
            GetToolTip(tooltip).enabled = false;
        }
        public static void ChangeMCSong(string tooltip)
        {
            Mode[27]++;
            if (Mode[27] >= Names[14].Length) { Mode[27] = 0; }
            Fun.mcSongClip = AudioClips[mcsoundid[Mode[27]]];
            string[] text = GetToolTip(tooltip).buttonText.Split(':');
            GetToolTip(tooltip).buttonText = text[0] + ": " + Names[14][Mode[27]];
            GetToolTip(tooltip).enabled = false;
        }
        public static void ChangeMCTexture(string tooltip)
        {
            Mode[26]++;
            if (Mode[26] >= Names[13].Length) { Mode[26] = 0; }
            Fun.mcBlockTexture = MCTextures[Mode[26]];
            string[] text = GetToolTip(tooltip).buttonText.Split(':');
            GetToolTip(tooltip).buttonText = text[0] + ": " + Names[13][Mode[26]];
            GetToolTip(tooltip).enabled = false;
        }
        public static void ChangeHandTrailColor(string tooltip)
        {
            Mode[25]++;
            var len = GetEnabled("Use System Colors") ? scolor.Length : colors.Length;
            if (Mode[25] >= len) { Mode[25] = 0; }
            var name = GetEnabled("Use System Colors") ? systemColorNames[Mode[25]] : Names[0][Mode[25]];
            handTrailColor = GetEnabled("Use System Colors") ? SCToUC(scolor[Mode[25]]) : colors[Mode[25]];
            string[] text = GetToolTip(tooltip).buttonText.Split(':');
            GetToolTip(tooltip).buttonText = text[0] + ": " + name;
            GetToolTip(tooltip).enabled = false;
        }
        public static void ChangeMenuOutlineColor(string tooltip)
        {
            Mode[24]++;
            var len = GetEnabled("Use System Colors") ? scolor.Length : colors.Length;
            if (Mode[24] >= len) { Mode[24] = 0; }
            var name = GetEnabled("Use System Colors") ? systemColorNames[Mode[24]] : Names[0][Mode[24]];
            outlineColor = GetEnabled("Use System Colors") ? SCToUC(scolor[Mode[24]]) : colors[Mode[24]];
            string[] text = GetToolTip(tooltip).buttonText.Split(':');
            GetToolTip(tooltip).buttonText = text[0] + ": " + name;
            GetToolTip(tooltip).enabled = false;
        }
        // Mode[23] used near bottom
        public static void ChangeGunType(string tooltip)
        {
            Mode[22]++;
            if (Mode[22] >= Names[12].Length) { Mode[22] = 0; }
            GunLib.gunType = Mode[22];
            string[] text = GetToolTip(tooltip).buttonText.Split(':');
            GetToolTip(tooltip).buttonText = text[0] + ": " + Names[12][Mode[22]];
            GetToolTip(tooltip).enabled = false;
        }

        public static void ChangeButtonSound(string tooltip)
        {
            Mode[21]++;
            if (Mode[21] >= buttonSounds.Length) { Mode[21] = 0; }
            buttonSound = buttonSounds[Mode[21]];
            string[] text = GetToolTip(tooltip).buttonText.Split(':');
            GetToolTip(tooltip).buttonText = text[0] + ": " + Names[11][Mode[21]];
            GetToolTip(tooltip).enabled = false;
        }
        public static void ChangePlatformSecondColor(string tooltip)
        {
            Mode[20]++;
            var len = GetEnabled("Use System Colors") ? scolor.Length : colors.Length;
            if (Mode[20] >= len) { Mode[20] = 0; }
            var name = GetEnabled("Use System Colors") ? systemColorNames[Mode[20]] : Names[0][Mode[20]];
            Movement.PlatSecondColor = GetEnabled("Use System Colors") ? SCToUC(scolor[Mode[20]]) : colors[Mode[20]];
            string[] text = GetToolTip(tooltip).buttonText.Split(':');
            GetToolTip(tooltip).buttonText = text[0] + ": " + name;
            GetToolTip(tooltip).enabled = false;
        }
        public static void ChangePlatformFirstColor(string tooltip)
        {
            Mode[19]++;
            var len = GetEnabled("Use System Colors") ? scolor.Length : colors.Length;
            if (Mode[19] >= len) { Mode[19] = 0; }
            var name = GetEnabled("Use System Colors") ? systemColorNames[Mode[19]] : Names[0][Mode[19]];
            Movement.PlatFirstColor = GetEnabled("Use System Colors") ? SCToUC(scolor[Mode[19]]) : colors[Mode[19]];
            string[] text = GetToolTip(tooltip).buttonText.Split(':');
            GetToolTip(tooltip).buttonText = text[0] + ": " + name;
            GetToolTip(tooltip).enabled = false;
        }
        public static void ChangePlatformColor(string tooltip)
        {
            Mode[18]++;
            var len = GetEnabled("Use System Colors") ? scolor.Length : colors.Length;
            if (Mode[18] >= len) { Mode[18] = 0; }
            var name = GetEnabled("Use System Colors") ? systemColorNames[Mode[18]] : Names[0][Mode[18]];
            Movement.PlatColor = GetEnabled("Use System Colors") ? SCToUC(scolor[Mode[18]]) : colors[Mode[18]];
            string[] text = GetToolTip(tooltip).buttonText.Split(':');
            GetToolTip(tooltip).buttonText = text[0] + ": " + name;
            GetToolTip(tooltip).enabled = false;
        }
        public static void ChangeMenuDestroyTime(string tooltip)
        {
            Mode[17]++;
            if (Mode[17] >= destroyDelays.Length) { Mode[17] = 0; }
            destroyDelay = destroyDelays[Mode[17]];
            string[] text = GetToolTip(tooltip).buttonText.Split(':');
            GetToolTip(tooltip).buttonText = text[0] + ": " + Names[10][Mode[17]];
            GetToolTip(tooltip).enabled = false;
        }
        public static void ChangeButtonTextColor(string tooltip)
        {
            Mode[16]++;
            if (Mode[16] >= unityc.Length) { Mode[16] = 0; }
            buttonTextColor = unityc[Mode[16]];
            string[] text = GetToolTip(tooltip).buttonText.Split(':');
            GetToolTip(tooltip).buttonText = text[0] + ": " + Names[9][Mode[16]];
            GetToolTip(tooltip).enabled = false;
        }
        // 15 was used for change menu type
        // 14 was used for change page type
        public static void ChangePointerPosition(string tooltip)
        {
            Mode[13]++;
            if (Mode[13] >= pointerPoses.Length) { Mode[13] = 0; }
            pointerPosition = pointerPoses[Mode[13]];
            string[] text = GetToolTip(tooltip).buttonText.Split(':');
            GetToolTip(tooltip).buttonText = text[0] + ": " + Names[7][Mode[13]];
            GetToolTip(tooltip).enabled = false;
        }
        public static void ChangeTimeOfDay(string tooltip)
        {
            Mode[12]++;
            if (Mode[12] >= times.Length) { Mode[12] = 0; }
            BetterDayNightManager.instance.SetTimeOfDay(times[Mode[12]]);
            string[] text = GetToolTip(tooltip).buttonText.Split(':');
            GetToolTip(tooltip).buttonText = text[0] + ": " + Names[5][Mode[12]];
            GetToolTip(tooltip).enabled = false;
        }
        public static void ChangeSpeedBoostSpeed(string tooltip)
        {
            Mode[11]++;
            if (Mode[11] >= speedBoostSpeeds.Length) { Mode[11] = 0; }
            Movement.speedBoostSpeed = speedBoostSpeeds[Mode[11]];
            string[] text = GetToolTip(tooltip).buttonText.Split(':');
            GetToolTip(tooltip).buttonText = text[0] + ": " + Names[4][Mode[11]];
            GetToolTip(tooltip).enabled = false;
        }
        public static void ChangeFlySpeed(string tooltip)
        {
            Mode[10]++;
            if (Mode[10] >= flySpeeds.Length) { Mode[10] = 0; }
            Movement.flySpeed = flySpeeds[Mode[10]];
            string[] text = GetToolTip(tooltip).buttonText.Split(':');
            GetToolTip(tooltip).buttonText = text[0] + ": " + Names[3][Mode[10]];
            GetToolTip(tooltip).enabled = false;
        }
        public static void ChangeGunDisabled(string tooltip)
        {
            Mode[9]++;
            if (Mode[9] >= unityc.Length) { Mode[9] = 0; }
            GunLib.disabledColor = unityc[Mode[9]];
            string[] text = GetToolTip(tooltip).buttonText.Split(':');
            GetToolTip(tooltip).buttonText = text[0] + ": " + Names[9][Mode[9]];
            GetToolTip(tooltip).enabled = false;
        }
        public static void ChangeGunEnabled(string tooltip)
        {
            Mode[8]++;
            if (Mode[8] >= unityc.Length) { Mode[8] = 0; }
            GunLib.enabledColor = unityc[Mode[8]];
            string[] text = GetToolTip(tooltip).buttonText.Split(':');
            GetToolTip(tooltip).buttonText = text[0] + ": " + Names[9][Mode[8]];
            GetToolTip(tooltip).enabled = false;
        }
        public static void ChangeGunShape(string tooltip)
        {
            Mode[7]++;
            if (Mode[7] >= shapes.Length) { Mode[7] = 0; }
            GunLib.gunShape = shapes[Mode[7]];
            string[] text = GetToolTip(tooltip).buttonText.Split(':');
            GetToolTip(tooltip).buttonText = text[0] + ": " + shapes[Mode[7]].ToString();         
            GetToolTip(tooltip).enabled = false;
            GunLib.pointer = null;
        }
        public static void ChangeGhostType(string tooltip)
        {
            Mode[6]++;
            if (Mode[6] >= Names[2].Length) { Mode[6] = 0; }
            Rig.GhostType = Mode[6];
            string[] text = GetToolTip(tooltip).buttonText.Split(':');
            GetToolTip(tooltip).buttonText = text[0] + ": " + Names[2][Mode[6]];
            GetToolTip(tooltip).enabled = false;
        }
        public static void ChangeMenuPhysics(string tooltip)
        {
            Mode[5]++;
            if (Mode[5] >= Names[1].Length) { Mode[5] = 0; }
            physicSetting = Mode[5];
            string[] text = GetToolTip(tooltip).buttonText.Split(':');
            GetToolTip(tooltip).buttonText = text[0] + ": " + Names[1][Mode[5]];
            GetToolTip(tooltip).enabled = false;
        }
        public static void ChangeMenuButtonOffColor(string tooltip)
        {
            Mode[4]++;
            var len = GetEnabled("Use System Colors") ? scolor.Length : colors.Length;
            if (Mode[4] >= len) { Mode[4] = 0; }
            var name = GetEnabled("Use System Colors") ? systemColorNames[Mode[4]] : Names[0][Mode[4]];
            ButtonColorDisable = GetEnabled("Use System Colors") ? SCToUC(scolor[Mode[4]]) : colors[Mode[4]];
            string[] text = GetToolTip(tooltip).buttonText.Split(':');
            GetToolTip(tooltip).buttonText = text[0] + ": " + name;
            GetToolTip(tooltip).enabled = false;
        }
        public static void ChangeMenuButtonOnColor(string tooltip)
        {
            Mode[3]++;
            var len = GetEnabled("Use System Colors") ? scolor.Length : colors.Length;
            if (Mode[3] >= len) { Mode[3] = 0; }
            var name = GetEnabled("Use System Colors") ? systemColorNames[Mode[3]] : Names[0][Mode[3]];
            ButtonColorEnabled = GetEnabled("Use System Colors") ? SCToUC(scolor[Mode[3]]) : colors[Mode[3]];
            string[] text = GetToolTip(tooltip).buttonText.Split(':');
            GetToolTip(tooltip).buttonText = text[0] + ": " + name;
            GetToolTip(tooltip).enabled = false;
        }
        public static void ChangeMenuSecondColor(string tooltip)
        {
            Mode[2]++;
            if (Mode[2] >= colors.Length) { Mode[2] = 0; }
            SecondColor = colors[Mode[2]];
            string[] text = GetToolTip(tooltip).buttonText.Split(':');
            GetToolTip(tooltip).buttonText = text[0] + ": " + Names[0][Mode[2]];
            GetToolTip(tooltip).enabled = false;
        }
        public static void ChangeMenuFirstColor(string tooltip)
        {
            Mode[1]++;
            if (Mode[1] >= colors.Length) { Mode[1] = 0; }
            FirstColor = colors[Mode[1]];
            string[] text = GetToolTip(tooltip).buttonText.Split(':');
            GetToolTip(tooltip).buttonText = text[0] + ": " + Names[0][Mode[1]];
            GetToolTip(tooltip).enabled = false;
        }
        public static void ChangeMenuColor(string tooltip)
        {
            Mode[0]++;
            var len = GetEnabled("Use System Colors") ? scolor.Length : colors.Length;
            if (Mode[0] >= len) { Mode[0] = 0; }
            var name = GetEnabled("Use System Colors") ? systemColorNames[Mode[0]] : Names[0][Mode[0]];
            NormalColor = GetEnabled("Use System Colors") ? SCToUC(scolor[Mode[0]]) : colors[Mode[0]];
            string[] text = GetToolTip(tooltip).buttonText.Split(':');
            GetToolTip(tooltip).buttonText = text[0] + ": " + name;
            GetToolTip(tooltip).enabled = false;
        }
        public static void RefreshSettings()
        {
            //NormalColor = colors[Mode[0]];
            NormalColor = GetEnabled("Use System Colors") ? SCToUC(scolor[Mode[0]]) : colors[Mode[0]];
            //FirstColor = colors[Mode[1]]; // not used
            //SecondColor = colors[Mode[2]]; // not used
            ButtonColorEnabled = GetEnabled("Use System Colors") ? SCToUC(scolor[Mode[3]]) : colors[Mode[3]];
            ButtonColorDisable = GetEnabled("Use System Colors") ? SCToUC(scolor[Mode[4]]) : colors[Mode[4]];
            physicSetting = Mode[5];
            Rig.GhostType = Mode[6];
            GunLib.gunShape = shapes[Mode[7]]; GunLib.pointer = null;
            GunLib.enabledColor = unityc[Mode[8]];
            GunLib.disabledColor = unityc[Mode[9]];
            Movement.flySpeed = flySpeeds[Mode[10]];
            Movement.speedBoostSpeed = speedBoostSpeeds[Mode[11]];
            BetterDayNightManager.instance.SetTimeOfDay(times[Mode[12]]);
            pointerPosition = pointerPoses[Mode[13]];
            buttonTextColor = unityc[Mode[16]];
            destroyDelay = destroyDelays[Mode[17]];
            Movement.PlatColor = GetEnabled("Use System Colors") ? SCToUC(scolor[Mode[18]]) : colors[Mode[18]];
            Movement.PlatFirstColor = GetEnabled("Use System Colors") ? SCToUC(scolor[Mode[19]]) : colors[Mode[19]];
            Movement.PlatSecondColor = GetEnabled("Use System Colors") ? SCToUC(scolor[Mode[20]]) : colors[Mode[20]];
            buttonSound = buttonSounds[Mode[21]];
            GunLib.gunType = Mode[22];
            RefreshTheme(); // 23
            outlineColor = GetEnabled("Use System Colors") ? SCToUC(scolor[Mode[24]]) : colors[Mode[24]];
            handTrailColor = GetEnabled("Use System Colors") ? SCToUC(scolor[Mode[25]]) : colors[Mode[25]];
            Fun.mcBlockTexture = MCTextures[Mode[26]];
            if (!fastLoad)
                Fun.mcSongClip = AudioClips[mcsoundid[Mode[27]]];
            currentFont = Font.CreateDynamicFontFromOSFont(Names[15][Mode[28]], 1);
            menuTrailColor = GetEnabled("Use System Colors") ? SCToUC(scolor[Mode[29]]) : colors[Mode[29]];
            Fun.pickaxeMaxHit = pickaxeStrikes[Mode[30]];
        }

        public static void SetWind(bool enabled)
        {
            foreach (var volumes in GetForceVolumes())
                volumes.enabled = enabled;
        }
        private static ForceVolume[] forceVolumes = null;
        public static ForceVolume[] GetForceVolumes()
        {
            forceVolumes ??= Resources.FindObjectsOfTypeAll<ForceVolume>();
            return forceVolumes;
        }
        private static GameObject FPCamera;
        public static void FPC()
        {
            if (XRSettings.isDeviceActive)
            {
                if (GameObject.Find("Third Person Camera") != null)
                {
                    FPCamera = GameObject.Find("Third Person Camera");
                    FPCamera.SetActive(false);
                }
                if (GameObject.Find("CameraTablet(Clone)") != null)
                {
                    FPCamera = GameObject.Find("CameraTablet(Clone)");
                    FPCamera.SetActive(false);
                }
            } else { SendNotification(Error() + "Only Use While Using Headset"); GetIndex("First Person Camera").enabled = false; }
        }

        public static void TPC()
        {
            if (FPCamera != null)
            {
                FPCamera.SetActive(true);
                FPCamera = null;
            }
        }

        public static void EnterPage(int page, int easyPage = 0, int pageNumber = 0)
        {
            buttonsType = page;
            Main.pageNumber = pageNumber;
            Main.easyPage = easyPage;
        }

        // originaly made by kingofnetflix https://github.com/kingofnetflix/BAnANA/blob/master/BAnANA/BAnANA/Main/VoiceManager.cs
        private static KeywordRecognizer enablePhrase;
        private static KeywordRecognizer modPhrase;

        private static Dictionary<string, Action> commands;

        private static string[] phrase = { "mystic", "client", "jarvis", "google", "siri", "alaxa", "computer", "console", "bitch", "azmyth", "thatguy", "that guy", "chat GPT", "goober", "rat", "skid", "skidder" };
        private static string[] discardPhrase = { "cancel", "stop", "nevermind", "never mind", "shut the fuck up" };

        private static bool listening = false;

        void Start()
        {
            commands = new Dictionary<string, Action>();
            foreach (var btnss in Buttons.buttons)
                foreach (var btns in btnss)
                    foreach (var btn in btns)
                        if (btn.buttonText.Contains("["))
                            commands[btn.buttonText.Split('[')[0].ToLower()] = () => Toggle(btn.buttonText);
                        else if (btn.buttonText.Contains(":"))
                            commands["change " + btn.buttonText.Split(':')[0].ToLower()] = () => Toggle(btn.buttonText);
                        else commands[btn.buttonText.ToLower()] = () => Toggle(btn.buttonText);
            foreach (var discard in discardPhrase)
                commands.Add(discard, () => CancelVoiceCommand());
        }

        public static void EnableVoiceCommands()
        {
            if (enablePhrase == null || !enablePhrase.IsRunning)
            {
                enablePhrase = new KeywordRecognizer(phrase);
                enablePhrase.OnPhraseRecognized += Recognition;
                enablePhrase.Start();
            }
        }

        private static void Recognition(PhraseRecognizedEventArgs args)
        {
            if (Array.Exists(phrase, element => element == args.text))
            {
                listening = true;
                if (GetEnabled("Dynamic Sounds"))
                    Loaders.PlayAudio(AudioClips[3]);
                StartCommandRecognition();
                listeningCoroutine = MUtils._RunCoroutine(Timeout());
                SendNotification(Voice() + "listening..", MessageInfo.None, false);
            }
        }
        private static void StartCommandRecognition()
        {
            modPhrase = new KeywordRecognizer(commands.Keys.ToArray());
            modPhrase.OnPhraseRecognized += CommandRecognition;
            modPhrase.Start();
        }

        private static void CommandRecognition(PhraseRecognizedEventArgs args)
        {
            if (listening && commands.ContainsKey(args.text))
            {
                commands[args.text]?.Invoke();
                if (listeningCoroutine != null)
                    MUtils._EndCoroutine(listeningCoroutine);
                SendNotification(Voice() + $"Toggled {args.text}", MessageInfo.None, false);
                Loaders.PlayAudio(AudioClips[4]);
                listening = false;
            }
        }

        public static void StopVoiceCommands()
        {
            listening = false;
            enablePhrase?.Stop();
            modPhrase?.Stop();
            enablePhrase = null;
            modPhrase = null;
        }
        private static Coroutine listeningCoroutine;
        public static IEnumerator Timeout()
        {
            yield return new WaitForSeconds(6);
            if (listening)
            {
                listening = false;
                if (enablePhrase != null && modPhrase != null)
                    CancelVoiceCommand();
                if (GetEnabled("Dynamic Sounds"))
                    Loaders.PlayAudio(AudioClips[4]);
                SendNotification(Voice() + "No input stopped listening");
            }
        }

        private static void CancelVoiceCommand()
        {
            SendNotification(Voice() + "Canceling..");
            listening = false;
            modPhrase.Stop();
            modPhrase.Dispose();
        }

        public static void NoCamMod(bool active)
        {
            foreach (var objs in GetGameObjects())
                if (objs.name.Contains("LCK"))
                    objs.SetActive(active);
        }
        private static string[] themeNames =
        {
            "Mystic",
            "Mystic Sides",
            "ThatGuy",
            "ThatGuy Sides",
            "AZ",
            "II Template",
            "KMan",
            "Mango",
            "Sial Template?",
            //"PayPal",
            "Skelo Template (not really)",
            "NXO Template",
            "Soup",
            "Wide / Small",
            "Wide",
            "Really Wide",
            "Long"
        };
        // pagePoss[0] = previous
        // pagePoss[1] = next

        // size is in depth, width, hight
        public static void ChangeMenuTheme(string tooltip)
        {
            Mode[23]++;
            if (Mode[23] >= themeNames.Length) { Mode[23] = 0; }
            string[] text = GetToolTip(tooltip).buttonText.Split(':');
            GetToolTip(tooltip).buttonText = text[0] + ": " + themeNames[Mode[23]];
            GetToolTip(tooltip).enabled = false;
            RefreshTheme();
        }

        private static void RefreshTheme()
        {
            if (Mode[23] == 0) // mystic
            {
                parentSize = new Vector3(.1f, .3f, .4f);
                pageButtonSize = new Vector3(0.045f, 0.25f, 0.064295f);
                pagePoss[0] = new Vector3(0.56f, 0.37f, 0.55f);
                pagePoss[1] = new Vector3(0.56f, -0.37f, 0.55f);
                menuSize = new Vector3(.1f, 1, 1.2f);
                menuPos = new Vector3(0.05f, 0f, -0.05f);
                buttonSize = new Vector3(.09f, .8f, .08f);
                buttonPos = new Vector3(.56f, 0, .29f);
                buttonOffset = 1f;
                internalButtonOffset = .13f;
                settingButtonOffset = .35f;
                search = new Vector2(-.45f, -.77f);
                //buttonTextPos = new Vector3(.064f, 0f, .111f);
                //buttonTextOffset = 2.6f; // 3.05
                buttonFontStyle = FontStyle.Normal;
                titleFontStyle = FontStyle.Normal;
                pageSize = 8;
                toolTipZ = -.27f;
                menuTitleZ = .173f;
                nextPageText = "";
                lastPageText = "";
                disconnectSize = new Vector3(0.045f, 0.66f, 0.17f);
                disconnectPos = new Vector3(0.50f, -1.122f, .1f);
            }
            else if (Mode[23] == 1) // mystic sides
            {
                pageButtonSize = new Vector3(0.045f, 0.25f, 1f);
                pagePoss[0] = new Vector3(0.56f, 0.65f, -.1f); // -.571f
                pagePoss[1] = new Vector3(0.56f, -0.65f, -.1f);
                menuSize = new Vector3(.1f, 1, 1.2f);
                buttonSize = new Vector3(.09f, .8f, .08f);
                buttonPos = new Vector3(.56f, 0, .29f);
                buttonOffset = 1f;
                //buttonTextPos = new Vector3(.064f, 0f, .111f);
                //buttonTextOffset = 2.6f; // 3.05
                buttonFontStyle = FontStyle.Normal;
                titleFontStyle = FontStyle.Normal;
                pageSize = 8;
                toolTipZ = -.27f;
                menuTitleZ = .173f;
            }
            else if (Mode[23] == 2) // thatguy
            {
                pageButtonSize = new Vector3(0.045f, 0.25f, 0.064295f);
                pagePoss[0] = new Vector3(0.56f, 0.37f, 0.45f);
                pagePoss[1] = new Vector3(0.56f, -0.37f, 0.45f);
                menuSize = new Vector3(.1f, 1, 1);
                buttonSize = new Vector3(.09f, .8f, .08f);
                buttonPos = new Vector3(.56f, 0, .19f);
                search = new Vector2(-.45f, -.675f);
                buttonOffset = 1;
                //buttonTextPos = new Vector3(.064f, 0, .076f);
                //buttonTextOffset = 2.55f;
                buttonFontStyle = FontStyle.Italic;
                titleFontStyle = FontStyle.BoldAndItalic;
                pageSize = 6;
                toolTipZ = -.22f;
                menuTitleZ = .125f;
            }
            else if (Mode[23] == 3) // thatguy sides
            {
                pageButtonSize = new Vector3(0.045f, 0.25f, 1f);
                pagePoss[0] = new Vector3(0.56f, 0.65f, -.1f);
                pagePoss[1] = new Vector3(0.56f, -0.65f, -.1f);
                menuSize = new Vector3(.1f, 1, 1);
                buttonSize = new Vector3(.09f, .8f, .08f);
                buttonPos = new Vector3(.56f, 0, .19f);
                buttonOffset = 1;
                //buttonTextPos = new Vector3(.064f, 0, .076f);
                //buttonTextOffset = 2.55f;
                buttonFontStyle = FontStyle.Italic;
                titleFontStyle = FontStyle.BoldAndItalic;
                pageSize = 6;
                toolTipZ = -.22f;
                menuTitleZ = .125f;
            }
            else if (Mode[23] == 4) // AZ
            {
                pageButtonSize = new Vector3(0.045f, 0.25f, 1f);
                pagePoss[0] = new Vector3(0.56f, 0.65f, -.1f);
                pagePoss[1] = new Vector3(0.56f, -0.65f, -.1f);
                menuSize = new Vector3(.1f, 1, 1.5f);
                buttonSize = new Vector3(.09f, .9f, .08f);
                buttonPos = new Vector3(.56f, 0, .28f);
                buttonOffset = 1.15f;
                settingButtonOffset = .4f;
                search = new Vector2(-.45f, -.925f);
                //buttonTextPos = new Vector3(.064f, 0f, .111f);
                //buttonTextOffset = 2.9f;
                buttonFontStyle = FontStyle.Italic;
                titleFontStyle = FontStyle.BoldAndItalic;
                pageSize = 8;
                toolTipZ = -.34f;
                menuTitleZ = .163f;
            }
            else if (Mode[23] == 5) // ii temp
            {
                parentSize = new Vector3(.1f, .3f, .3825f);
                pageButtonSize = new Vector3(.09f, .2f, .9f);
                pagePoss[0] = new Vector3(.56f, .65f, 0);
                pagePoss[1] = new Vector3(.56f, -.65f, 0);
                menuSize = new Vector3(.1f, 1f, 1f);
                menuPos = new Vector3(.05f, 0, 0);
                buttonSize = new Vector3(.09f, .9f, .08f);
                buttonPos = new Vector3(.56f, 0, .28f);
                search = new Vector2(-.45f, -.555f);
                buttonOffset = 1;
                //buttonTextPos = new Vector3(.064f, 0, .111f);
                //buttonTextOffset = 2.6f;
                buttonFontStyle = FontStyle.Italic;
                titleFontStyle = FontStyle.Italic;
                pageSize = 7;
                toolTipZ = -.17f;
                menuTitleZ = .165f;
                nextPageText = ">";
                lastPageText = "<";
                disconnectSize = new Vector3(.09f, .9f, .08f);
                disconnectPos = new Vector3(.56f, 0, .6f);
                internalButtonOffset = .1f;
            }
            else if (Mode[23] == 6) // kman temp
            {
                parentSize = new Vector3(.1f, .3f, .4f);
                pageButtonSize = new Vector3(.09f, .15f, .98f);
                pagePoss[0] = new Vector3(.56f, .5833f, -.13f);
                pagePoss[1] = new Vector3(.56f, -.5833f, -.13f);
                menuSize = new Vector3(.1f, .94f, 1.2f);
                menuPos = new Vector3(.05f, 0, -.03f);
                buttonSize = new Vector3(.09f, .8f, .08f);
                buttonPos = new Vector3(.56f, 0, .29f);
                search = new Vector2(-.42f, -.73f);
                buttonOffset = 1f;
                settingButtonOffset = .35f;
                //buttonTextPos = new Vector3(.064f, 0, .111f);
                //buttonTextOffset = 2.6f;
                buttonFontStyle = FontStyle.Italic;
                titleFontStyle = FontStyle.Italic;
                pageSize = 7;
                toolTipZ = -.25f;
                menuTitleZ = .175f;
                nextPageText = ">";
                lastPageText = "<";
                disconnectSize = new Vector3(.09f, .94f, .08f);
                disconnectPos = new Vector3(.56f, 0, .6f);
                internalButtonOffset = .13f;
            }
            else if (Mode[23] == 7) // mango / shiba temp
            {
                pageButtonSize = new Vector3(.09f, .8f, .08f);
                pagePoss[0] = new Vector3(.56f, 0, .28f);
                pagePoss[1] = new Vector3(.56f, 0, .15f);
                menuSize = new Vector3(.1f, 1, 1);
                menuPos = new Vector3(.05f, 0, 0);
                buttonSize = new Vector3(.09f, .8f, .08f);
                buttonPos = new Vector3(.56f, 0, .02f);
                search = new Vector2(-.45f, -.555f);
                buttonOffset = 1f;
                //buttonTextPos = new Vector3(.064f, 0, .111f);
                //buttonTextOffset = 2.6f;
                buttonFontStyle = FontStyle.Italic;
                titleFontStyle = FontStyle.Italic;
                pageSize = 4;
                toolTipZ = -.18f;
                menuTitleZ = .175f;
                nextPageText = $"Next >> [{pageNumber + 1}]";
                lastPageText = $"[{pageNumber - 1}] << Prev";
                disconnectSize = new Vector3(0.045f, 0.66f, 0.17f);
                disconnectPos = new Vector3(0.50f, -1.122f, .1f);
            }
            else if (Mode[23] == 8) // sial temp?
            {
                parentSize = new Vector3(.1f, .3f, .4f);
                pageButtonSize = new Vector3(.09f, .12f, .75f);
                pagePoss[0] = new Vector3(0.56f, .625f, 0);
                pagePoss[1] = new Vector3(0.56f, -.625f, 0);
                menuSize = new Vector3(.1f, 1, 1.1f);
                menuPos = new Vector3(.05f, 0, -.004f);
                buttonSize = new Vector3(.09f, .92f, .08f);
                buttonPos = new Vector3(.56f, 0, .28f);
                search = new Vector2(-.45f, -.615f);
                buttonOffset = 1f;
                settingButtonOffset = .41f;
                //buttonTextPos = new Vector3(.064f, 0f, .112f);
                //buttonTextOffset = 2.55f; // 3.05
                buttonFontStyle = FontStyle.Normal;
                titleFontStyle = FontStyle.Normal;
                pageSize = 7;
                toolTipZ = -.18f;
                menuTitleZ = .175f;
                nextPageText = ">";
                lastPageText = "<";
                disconnectSize = new Vector3(0.045f, 0.66f, 0.17f);
                disconnectPos = new Vector3(0.50f, -1.122f, .1f);
            }
            /*if (Mode[23] == 9) // paypal
            {
                parentSize = new Vector3(.1f, .3f, .4f);
                pageButtonSize = new Vector3(.09f, .2f, .1f);
                pagePoss[0] = new Vector3(.5f, .4f, 0.53f);
                pagePoss[1] = new Vector3(.5f, -.4f, 0.53f);
                menuSize = new Vector3(.1f, 1, .9f);
                menuPos = new Vector3(0.05f, 0, 0);
                buttonSize = new Vector3(.09f, .2f, .1f);
                buttonPos = new Vector3(0, .1f, 0);
                buttonOffset = 1f;
                internalButtonOffset = .1f;
                buttonFontStyle = FontStyle.Normal;
                titleFontStyle = FontStyle.Normal;
                pageSize = 8;
                toolTipZ = -.18f;
                menuTitleZ = .175f;
                nextPageText = "";
                lastPageText = "";
                disconnectSize = new Vector3(.09f, .5f, .1f);
                disconnectPos = new Vector3(.5f, 0, -.53f);
            }*/
            else if (Mode[23] == 9) // skelo temp
            {
                parentSize = new Vector3(.1f, .3f, .3825f);
                pageButtonSize = new Vector3(.09f, .9f, .08f);
                pagePoss[0] = new Vector3(.56f, 0, .28f);
                pagePoss[1] = new Vector3(.56f, 0, .18f);
                menuSize = new Vector3(.1f, 1.023f, .92f);
                menuPos = new Vector3(.05f, 0, 0);
                buttonSize = new Vector3(.09f, .9f, .08f);
                buttonPos = new Vector3(.56f, 0, .29f);
                search = new Vector2(-.45f, -.515f);
                buttonOffset = 1f;
                settingButtonOffset = .41f;
                internalButtonOffset = .1f;
                internalButtonAddOffset = .2f;
                buttonFontStyle = FontStyle.Normal;
                titleFontStyle = FontStyle.Normal;
                pageSize = 6;
                toolTipZ = -.18f;
                menuTitleZ = .171f;
                nextPageText = "------->";
                lastPageText = "<-------";
                disconnectSize = new Vector3(.045f, .66f, .17f);
                disconnectPos = new Vector3(.50f, -1.122f, .1f);
            }
            else if (Mode[23] == 10) // nxo template
            {
                parentSize = new Vector3(.1f, .2f, .3f);
                pageButtonSize = new Vector3(.005f, .25f, .08f);
                pagePoss[0] = new Vector3(.505f, .285f, -.31f);
                pagePoss[1] = new Vector3(.505f, -.285f, - .31f);
                menuSize = new Vector3(.01f, .925f, .90f);
                menuPos = new Vector3(.05f, 0, .025f);
                buttonSize = new Vector3(.005f, .82f, .08f);
                buttonPos = new Vector3(.505f, 0, .3250f);
                search = new Vector2(-.4f, -.415f);
                buttonOffset = 1f;
                settingButtonOffset = .36f;
                internalButtonOffset = .09f;
                internalButtonAddOffset = 0;
                buttonFontStyle = FontStyle.Normal;
                titleFontStyle = FontStyle.Normal;
                pageSize = 7;
                toolTipZ = -.18f;
                menuTitleZ = .135f;
                nextPageText = ">";
                lastPageText = "<";
                disconnectSize = new Vector3(.005f, .8975f, .0575f);
                disconnectPos = new Vector3(.5f, 0, .6f);
                returnButton = true;
                menuTextSize = new Vector2(.16f, .01725f);
                menuTitleSize = new Vector2(.19f, .04f);
                sideTextSize = new Vector2(.2f, .02f);
            }
            else if (Mode[23] == 11) // soup menu
            {
                parentSize = new Vector3(.1f, .3f, .3825f);
                pageButtonSize = new Vector3(.06f, .9f, .09f);
                pagePoss[0] = new Vector3(.56f, 0, -.39f);
                pagePoss[1] = new Vector3(.56f, 0, -.29f);
                menuSize = new Vector3(.1f, 1, .97f);
                menuPos = new Vector3(.05f, 0, 0);
                buttonSize = new Vector3(.06f, .9f, .09f);
                buttonPos = new Vector3(.56f, 0, .27f);
                search = new Vector2(-.45f, -.54f);
                buttonOffset = 1f;
                settingButtonOffset = .36f;
                internalButtonOffset = .11f;
                internalButtonAddOffset = 0;
                buttonFontStyle = FontStyle.Italic;
                titleFontStyle = FontStyle.Italic;
                pageSize = 5;
                toolTipZ = -.18f;
                menuTitleZ = .155f;
                nextPageText = ">>>>>";
                lastPageText = "<<<<<";
                disconnectSize = new Vector3(.09f, .9f, .08f);
                disconnectPos = new Vector3(.56f, 0, .56f);
                returnButton = false;
                menuTextSize = new Vector2(.2f, .03f);
                menuTitleSize = new Vector2(.28f, .05f);
                sideTextSize = new Vector2(.2f, .03f);
            }
            else if (Mode[23] == 12) // wide / skinny
            {
                parentSize = new Vector3(.1f, .2f, .4f);
                pageButtonSize = new Vector3(0.045f, 0.25f, 0.064295f);
                pagePoss[0] = new Vector3(0.56f, 0.65f, 0.45f);
                pagePoss[1] = new Vector3(0.56f, -0.65f, 0.45f);
                menuSize = new Vector3(.1f, 1.6f, 1f);
                menuPos = new Vector3(0.05f, 0f, -0.05f);
                buttonSize = new Vector3(.09f, 1.4f, .08f);
                buttonPos = new Vector3(.56f, 0, .23f);
                search = new Vector2(-.75f, -.68f);
                buttonOffset = 1.2f;
                settingButtonOffset = .65f;
                internalButtonOffset = .13f;
                internalButtonAddOffset = 0;
                //buttonTextPos = new Vector3(.064f, 0f, .089f);
                //buttonTextOffset = 3f;
                buttonFontStyle = FontStyle.Italic;
                titleFontStyle = FontStyle.BoldAndItalic;
                pageSize = 8;
                toolTipZ = -.24f;
                menuTitleZ = .133f;
                nextPageText = "";
                lastPageText = "";
                disconnectSize = new Vector3(.09f, .9f, .08f);
                disconnectPos = new Vector3(.56f, 0, .45f);
                menuTextSize = new Vector2(.2f, .03f);
                menuTitleSize = new Vector2(.28f, .05f);
                sideTextSize = new Vector2(.1f, .03f);
            }
            else if (Mode[23] == 13) // wide
            {
                parentSize = new Vector3(.1f, .3f, .4f);
                pageButtonSize = new Vector3(0.045f, 0.25f, 0.064295f);
                pagePoss[0] = new Vector3(0.56f, 0.65f, 0.45f);
                pagePoss[1] = new Vector3(0.56f, -0.65f, 0.45f);
                menuSize = new Vector3(.1f, 1.6f, 1f);
                menuPos = new Vector3(0.05f, 0f, -0.05f);
                buttonSize = new Vector3(.09f, 1.4f, .08f);
                buttonPos = new Vector3(.56f, 0, .23f);
                buttonOffset = 1.2f;
                settingButtonOffset = .65f;
                internalButtonOffset = .13f;
                internalButtonAddOffset = 0;
                //buttonTextPos = new Vector3(.064f, 0f, .089f);
                //buttonTextOffset = 3f;
                buttonFontStyle = FontStyle.Italic;
                titleFontStyle = FontStyle.BoldAndItalic;
                pageSize = 8;
                toolTipZ = -.24f;
                menuTitleZ = .133f;
                nextPageText = "";
                lastPageText = "";
                disconnectSize = new Vector3(.09f, .9f, .08f);
                disconnectPos = new Vector3(.56f, 0, .45f);
            }
            else if (Mode[23] == 14) // really wide
            {
                pageButtonSize = new Vector3(0.045f, 0.25f, 0.064295f);
                pagePoss[0] = new Vector3(0.56f, 0.65f, 0.45f);
                pagePoss[1] = new Vector3(0.56f, -0.65f, 0.45f);
                menuSize = new Vector3(.1f, 5f, 1f);
                buttonSize = new Vector3(.09f, 4.8f, .08f);
                buttonPos = new Vector3(.56f, 0, .23f);
                buttonOffset = 1.2f;
                settingButtonOffset = 2.351f;
                //buttonTextPos = new Vector3(.064f, 0f, .089f);
                //buttonTextOffset = 3f;
                pageSize = 8;
                toolTipZ = -.24f;
                menuTitleZ = .133f;
            }
            else if (Mode[23] == 15) // long
            {
                parentSize = new Vector3(.1f, .3f, .4f);
                pageButtonSize = new Vector3(0.045f, 0.25f, 0.064295f);
                pagePoss[0] = new Vector3(0.56f, 0.37f, 0.55f);
                pagePoss[1] = new Vector3(0.56f, -0.37f, 0.55f);
                menuSize = new Vector3(.1f, 1, 2f);
                menuPos = new Vector3(.05f, 0, -.205f);
                buttonSize = new Vector3(.09f, .8f, .08f);
                buttonPos = new Vector3(.56f, 0, .29f);
                search = new Vector2(-.565f, -.77f);
                buttonOffset = 1f;
                internalButtonOffset = .13f;
                settingButtonOffset = .35f;
                //buttonTextPos = new Vector3(.064f, 0f, .112f);
                //buttonTextOffset = 2.55f; // 3.05
                buttonFontStyle = FontStyle.Normal;
                titleFontStyle = FontStyle.Normal;
                pageSize = 13;
                toolTipZ = -.58f;
                menuTitleZ = .173f;
                nextPageText = "";
                lastPageText = "";
                disconnectSize = new Vector3(0.045f, 0.66f, 0.17f);
                disconnectPos = new Vector3(0.50f, -1.122f, .1f);
            }
        }

        public static void ColorKeyboard(bool doOutline)
        {
            var transform = keyboard.GetNamedChild("Keyboard").transform;
            var outline = transform.gameObject.GetNamedChild("Outline");
            if (doOutline)
            {
                outline.SetActive(true);
                for (int i = 0; i < outline.transform.childCount; i++)
                    outline.transform.GetChild(i).gameObject.ChangeColor(ButtonColorDisable);
            } else outline.SetActive(false);
            transform.gameObject.GetNamedChild("Back").ChangeColor(NormalColor);
            for (int i = 0; i < transform.childCount; i++)
            {
                var key = transform.GetChild(i).gameObject;
                if (key.name.Length == 1 && key.name[0] >= 'A' && key.name[0] <= 'Z')
                    key.ChangeColor(ButtonColorDisable);
            }
        }

        public static void SetUpKeyboard()
        {
            var board = keyboard.GetNamedChild("Keyboard");
            for (var letter = 'A'; letter <= 'Z'; letter++)
                board.GetNamedChild(letter.ToString()).GetOrAddComponent<KeyboardButton>();
            board.GetNamedChild("Exit").GetOrAddComponent<KeyboardButton>();
            board.GetNamedChild("Delete").GetOrAddComponent<KeyboardButton>();
            board.GetNamedChild("Next_Page").GetOrAddComponent<KeyboardButton>();
            board.GetNamedChild("Previous_Page").GetOrAddComponent<KeyboardButton>();
            board.GetNamedChild("Space").GetOrAddComponent<KeyboardButton>();
        }

        // thanks chatgpt
        public static string[] systemColorNames =
        {
            "Transparent", "AliceBlue", "AntiqueWhite", "Aqua", "Aquamarine", "Azure",
            "Beige", "Bisque", "Black", "BlanchedAlmond", "Blue", "BlueViolet",
            "Brown", "BurlyWood", "CadetBlue", "Chartreuse", "Chocolate", "Coral",
            "CornflowerBlue", "Cornsilk", "Crimson", "Cyan", "DarkBlue", "DarkCyan",
            "DarkGoldenrod", "DarkGray", "DarkGreen", "DarkKhaki", "DarkMagenta",
            "DarkOliveGreen", "DarkOrange", "DarkOrchid", "DarkRed", "DarkSalmon",
            "DarkSeaGreen", "DarkSlateBlue", "DarkSlateGray", "DarkTurquoise",
            "DarkViolet", "DeepPink", "DeepSkyBlue", "DimGray", "DodgerBlue",
            "Firebrick", "FloralWhite", "ForestGreen", "Fuchsia", "Gainsboro",
            "GhostWhite", "Gold", "Goldenrod", "Gray", "Green", "GreenYellow",
            "Honeydew", "HotPink", "IndianRed", "Indigo", "Ivory", "Khaki",
            "Lavender", "LavenderBlush", "LawnGreen", "LemonChiffon", "LightBlue",
            "LightCoral", "LightCyan", "LightGoldenrodYellow", "LightGray",
            "LightGreen", "LightPink", "LightSalmon", "LightSeaGreen",
            "LightSkyBlue", "LightSlateGray", "LightSteelBlue", "LightYellow",
            "Lime", "LimeGreen", "Linen", "Magenta", "Maroon", "MediumAquamarine",
            "MediumBlue", "MediumOrchid", "MediumPurple", "MediumSeaGreen",
            "MediumSlateBlue", "MediumSpringGreen", "MediumTurquoise",
            "MediumVioletRed", "MidnightBlue", "MintCream", "MistyRose",
            "Moccasin", "NavajoWhite", "Navy", "OldLace", "Olive", "OliveDrab",
            "Orange", "OrangeRed", "Orchid", "PaleGoldenrod", "PaleGreen",
            "PaleTurquoise", "PaleVioletRed", "PapayaWhip", "PeachPuff", "Peru",
            "Pink", "Plum", "PowderBlue", "Purple", "Red", "RosyBrown",
            "RoyalBlue", "SaddleBrown", "Salmon", "SandyBrown", "SeaGreen",
            "SeaShell", "Sienna", "Silver", "SkyBlue", "SlateBlue", "SlateGray",
            "Snow", "SpringGreen", "SteelBlue", "Tan", "Teal", "Thistle",
            "Tomato", "Turquoise", "Violet", "Wheat", "White", "WhiteSmoke",
            "Yellow", "YellowGreen"
        };

        public static SColor[] scolor =
        {
            SColor.Transparent, SColor.AliceBlue, SColor.AntiqueWhite, SColor.Aqua, SColor.Aquamarine, SColor.Azure,
            SColor.Beige, SColor.Bisque, SColor.Black, SColor.BlanchedAlmond, SColor.Blue, SColor.BlueViolet,
            SColor.Brown, SColor.BurlyWood, SColor.CadetBlue, SColor.Chartreuse, SColor.Chocolate, SColor.Coral,
            SColor.CornflowerBlue, SColor.Cornsilk, SColor.Crimson, SColor.Cyan, SColor.DarkBlue, SColor.DarkCyan,
            SColor.DarkGoldenrod, SColor.DarkGray, SColor.DarkGreen, SColor.DarkKhaki, SColor.DarkMagenta,
            SColor.DarkOliveGreen, SColor.DarkOrange, SColor.DarkOrchid, SColor.DarkRed, SColor.DarkSalmon,
            SColor.DarkSeaGreen, SColor.DarkSlateBlue, SColor.DarkSlateGray, SColor.DarkTurquoise,
            SColor.DarkViolet, SColor.DeepPink, SColor.DeepSkyBlue, SColor.DimGray, SColor.DodgerBlue,
            SColor.Firebrick, SColor.FloralWhite, SColor.ForestGreen, SColor.Fuchsia, SColor.Gainsboro,
            SColor.GhostWhite, SColor.Gold, SColor.Goldenrod, SColor.Gray, SColor.Green, SColor.GreenYellow,
            SColor.Honeydew, SColor.HotPink, SColor.IndianRed, SColor.Indigo, SColor.Ivory, SColor.Khaki,
            SColor.Lavender, SColor.LavenderBlush, SColor.LawnGreen, SColor.LemonChiffon, SColor.LightBlue,
            SColor.LightCoral, SColor.LightCyan, SColor.LightGoldenrodYellow, SColor.LightGray,
            SColor.LightGreen, SColor.LightPink, SColor.LightSalmon, SColor.LightSeaGreen,
            SColor.LightSkyBlue, SColor.LightSlateGray, SColor.LightSteelBlue, SColor.LightYellow,
            SColor.Lime, SColor.LimeGreen, SColor.Linen, SColor.Magenta, SColor.Maroon, SColor.MediumAquamarine,
            SColor.MediumBlue, SColor.MediumOrchid, SColor.MediumPurple, SColor.MediumSeaGreen,
            SColor.MediumSlateBlue, SColor.MediumSpringGreen, SColor.MediumTurquoise,
            SColor.MediumVioletRed, SColor.MidnightBlue, SColor.MintCream, SColor.MistyRose,
            SColor.Moccasin, SColor.NavajoWhite, SColor.Navy, SColor.OldLace, SColor.Olive, SColor.OliveDrab,
            SColor.Orange, SColor.OrangeRed, SColor.Orchid, SColor.PaleGoldenrod, SColor.PaleGreen,
            SColor.PaleTurquoise, SColor.PaleVioletRed, SColor.PapayaWhip, SColor.PeachPuff, SColor.Peru,
            SColor.Pink, SColor.Plum, SColor.PowderBlue, SColor.Purple, SColor.Red, SColor.RosyBrown,
            SColor.RoyalBlue, SColor.SaddleBrown, SColor.Salmon, SColor.SandyBrown, SColor.SeaGreen,
            SColor.SeaShell, SColor.Sienna, SColor.Silver, SColor.SkyBlue, SColor.SlateBlue, SColor.SlateGray,
            SColor.Snow, SColor.SpringGreen, SColor.SteelBlue, SColor.Tan, SColor.Teal, SColor.Thistle,
            SColor.Tomato, SColor.Turquoise, SColor.Violet, SColor.Wheat, SColor.White, SColor.WhiteSmoke,
            SColor.Yellow, SColor.YellowGreen
        };
    }
}
