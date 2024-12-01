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

namespace MysticClient.Mods
{
    public class Settings
    {
        private static int[] Mode = new int[9999];
        private static PrimitiveType[] shapes = { PrimitiveType.Cube, PrimitiveType.Sphere, PrimitiveType.Capsule, PrimitiveType.Cylinder, PrimitiveType.Plane, PrimitiveType.Quad }; // iidk told me to do (PrimitiveType)Mode[i] but it breaks so im not
        public static Color[] colors = { Color.black, new Color(0.541f, 0.027f, 0.761f, 0.004f), Color.blue, Color.yellow, new Color(1f, 0.51f, 0f, 0.004f), Color.white, Color.cyan, Color.green, Color.red, new Color32(135, 0, 0, 1), Color.magenta, Color.gray };
        public static Color[] unityc = { Color.black, Color.blue, Color.yellow, Color.white, Color.cyan, Color.green, Color.red, Color.magenta, Color.gray };
        private static int[] flySpeeds = { 15, 10, 25 };
        private static int[] speedBoostSpeeds = { 12, 9, 16, 25, int.MaxValue };
        private static int[] times = { 3, 6, 0, 1 };
        private static int[] destroyDelays = { 0, 2, 5, 10, int.MaxValue };
        private static Action[] menuTypes = { ()=>MysticTemplate(), ()=>ThatGuyTemplate(), ()=>AZTemplate(), ()=>KFJTemplate() };
        private static Action[] pageTypes = { ()=>BottomPageButtons(), ()=>SidePageButtons(), ()=>TopPageButtons() };
        private static int[] buttonSounds = { 67, 66, 176, 8, 18, 244, 221, 0 };
        private static Vector3[] pointerPoses = { new Vector3(0, -.1f, -.15f), new Vector3(0, .1f, 0), new Vector3(0, -.1f, 0) };
        public static string[][] Names =
        {
            new string[] { "Black", "Purple", "Blue", "Yellow", "Orange", "White", "Cyan", "Green", "Red", "Blood", "Magenta", "Gray" }, // 0
            new string[] { "None", "Trowable", "Throwable And Collidable" }, // 1
            new string[] { "None", "Hands", "Body" }, // 2
            new string[] { "Normal", "Slow", "Fast" }, // 3
            new string[] { "Normal", "Small", "Big", "Huge", "???" }, // 4
            new string[] { "Day", "Dawn", "Hight", "Sun Rise", "Untouched" }, // 5
            new string[] { "Bottom", "Sides", "Top" }, // 6
            new string[] { "Quest", "Steam", "Steam / Quest" }, // 7
            new string[] { "Mystic", "ThatGuy", "AZ", "KFJ" }, // 8
            new string[] { "Black", "Blue", "Yellow", "White", "Cyan", "Green", "Red", "Magenta", "Gray" }, // 9
            new string[] { "Instant", "2s", "5s", "10s", "Never" }, // 10
            new string[] { "Click", "Key", "Cloud", "Wood", "Metal", "Ding", "Glass", "Dynamic" }, // 11
            new string[] { "Sharp", "Boring", "Bendy" }, // 12
        };

        public static void ChangeGunType(string tooltip)
        {
            Mode[22]++;
            if (Mode[22] >= 3) { Mode[22] = 0; }
            GunLib.gunType = Mode[22];
            string[] text = GetToolTip(tooltip).buttonText.Split(':');
            GetToolTip(tooltip).buttonText = text[0] + ": " + Names[12][Mode[22]];
            GetToolTip(tooltip).enabled = false;
        }

        public static void ChangeButtonSound(string tooltip)
        {
            Mode[21]++;
            if (Mode[21] >= colors.Length) { Mode[21] = 0; }
            buttonSound = buttonSounds[Mode[21]];
            string[] text = GetToolTip(tooltip).buttonText.Split(':');
            GetToolTip(tooltip).buttonText = text[0] + ": " + Names[11][Mode[21]];
            GetToolTip(tooltip).enabled = false;
        }
        public static void ChangePlatformSecondColor(string tooltip)
        {
            Mode[20]++;
            if (Mode[20] >= colors.Length) { Mode[20] = 0; }
            Movement.PlatSecondColor = colors[Mode[20]];
            string[] text = GetToolTip(tooltip).buttonText.Split(':');
            GetToolTip(tooltip).buttonText = text[0] + ": " + Names[0][Mode[20]];
            GetToolTip(tooltip).enabled = false;
        }
        public static void ChangePlatformFirstColor(string tooltip)
        {
            Mode[19]++;
            if (Mode[19] >= colors.Length) { Mode[19] = 0; }
            Movement.PlatFirstColor = colors[Mode[19]];
            string[] text = GetToolTip(tooltip).buttonText.Split(':');
            GetToolTip(tooltip).buttonText = text[0] + ": " + Names[0][Mode[19]];
            GetToolTip(tooltip).enabled = false;
        }
        public static void ChangePlatformColor(string tooltip)
        {
            Mode[18]++;
            if (Mode[18] >= colors.Length) { Mode[18] = 0; }
            Movement.PlatColor = colors[Mode[18]];
            string[] text = GetToolTip(tooltip).buttonText.Split(':');
            GetToolTip(tooltip).buttonText = text[0] + ": " + Names[0][Mode[18]];
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
        public static void ChangeMenuType(string tooltip)
        {
            Mode[15]++;
            if (Mode[15] >= menuTypes.Length) { Mode[15] = 0; }
            menuTypes[Mode[15]].Invoke();
            string[] text = GetToolTip(tooltip).buttonText.Split(':');
            GetToolTip(tooltip).buttonText = text[0] + ": " + Names[8][Mode[15]];
            GetToolTip(tooltip).enabled = false;
            if (GetToolTip("Changed Page Type").buttonText.Contains("Bottom"))
                BottomPageButtons();
            else if (GetToolTip("Changed Page Type").buttonText.Contains("Sides"))
                SidePageButtons();
            else if (GetToolTip("Changed Page Type").buttonText.Contains("Top"))
                TopPageButtons();
        }
        public static void ChangePageType(string tooltip)
        {
            Mode[14]++;
            if (Mode[14] >= pageTypes.Length) { Mode[14] = 0; }
            pageTypes[Mode[14]].Invoke();
            string[] text = GetToolTip(tooltip).buttonText.Split(':');
            GetToolTip(tooltip).buttonText = text[0] + ": " + Names[6][Mode[14]];
            GetToolTip(tooltip).enabled = false;
        }
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
            if (Mode[4] >= colors.Length) { Mode[4] = 0; }
            ButtonColorDisable = colors[Mode[4]];
            string[] text = GetToolTip(tooltip).buttonText.Split(':');
            GetToolTip(tooltip).buttonText = text[0] + ": " + Names[0][Mode[4]];
            GetToolTip(tooltip).enabled = false;
        }
        public static void ChangeMenuButtonOnColor(string tooltip)
        {
            Mode[3]++;
            if (Mode[3] >= colors.Length) { Mode[3] = 0; }
            ButtonColorEnabled = colors[Mode[3]];
            string[] text = GetToolTip(tooltip).buttonText.Split(':');
            GetToolTip(tooltip).buttonText = text[0] + ": " + Names[0][Mode[3]];
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
            if (Mode[0] >= colors.Length) { Mode[0] = 0; }
            NormalColor = colors[Mode[0]];
            string[] text = GetToolTip(tooltip).buttonText.Split(':');
            GetToolTip(tooltip).buttonText = text[0] + ": " + Names[0][Mode[0]];
            GetToolTip(tooltip).enabled = false;
        }
        public static void SetWind(bool enabled)
        {
            foreach (var volumes in GetForceVolumes())
                volumes.enabled = enabled;
        }
        private static ForceVolume[] forceVolumes = null;
        public static ForceVolume[] GetForceVolumes()
        {
            if (forceVolumes == null)
                forceVolumes = Resources.FindObjectsOfTypeAll<ForceVolume>();
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
        public static void EnterPage(int page)
        {
            buttonsType = page;
            pageNumber = 0;
        }
        public static void Save2()
        {
            foreach (var buttons in Buttons.buttons)
                foreach (var button in buttons)
                    if (!button.toolTip.Contains("Changed"))
                        PlayerPrefs.SetString(button.buttonText, JsonUtility.ToJson(button.enabled));
                    else
                    {
                        PlayerPrefs.SetString("Menu Color", JsonUtility.ToJson(colors[Mode[0]]));
                        PlayerPrefs.SetString("Menu First Color", JsonUtility.ToJson(colors[Mode[1]]));
                        PlayerPrefs.SetString("Menu Second Color", JsonUtility.ToJson(colors[Mode[2]]));
                        PlayerPrefs.SetString("Button Color Enbaled", JsonUtility.ToJson(colors[Mode[3]]));
                        PlayerPrefs.SetString("Button Color Disabled", JsonUtility.ToJson(colors[Mode[4]]));
                        PlayerPrefs.SetString("Menu Physics", JsonUtility.ToJson(physicSetting));
                        PlayerPrefs.SetString("Ghost Type", JsonUtility.ToJson(Rig.GhostType));
                        PlayerPrefs.SetString("Projectile", JsonUtility.ToJson(projectile));
                        PlayerPrefs.SetString("Trail", JsonUtility.ToJson(trail));
                        PlayerPrefs.SetString("Proj Shoot Speed", JsonUtility.ToJson(projSpeeds[Projectiles.Mode[4]]));
                        PlayerPrefs.SetString("Projectile Color", JsonUtility.ToJson(colors[Projectiles.Mode[3]]));
                        PlayerPrefs.SetString("Proj Hand Type", JsonUtility.ToJson(Projectiles.Mode[0]));
                        PlayerPrefs.SetString("Gun Type", JsonUtility.ToJson(GunLib.gunShape));
                        PlayerPrefs.SetString("Gun Disabled Color", JsonUtility.ToJson(colors[Mode[9]]));
                        PlayerPrefs.SetString("Gun Enabled Color", JsonUtility.ToJson(colors[Mode[8]]));
                        PlayerPrefs.SetString("Projectile Scale", JsonUtility.ToJson(sizes[Projectiles.Mode[5]]));
                    }
        }
        public static void Save()
        {
            GetIndex("Save Preferences").enabled = false;
            Directory.CreateDirectory("MysticClient\\ButtonSave");
            List<string> enabled = new List<string>();
            foreach (var buttons in Buttons.buttons)
                foreach (var button in buttons)
                    if (button.enabled)
                        enabled.Add(button.buttonText);
            File.WriteAllLines("MysticClient\\ButtonSave\\Save.txt", enabled);
        }
        public static void Load()
        {
            GetIndex("Load Preferences").enabled = false;
            string[] lines = File.ReadAllLines("MysticClient\\Buttons\\Save.txt");
            foreach (string line in lines)
                foreach (var buttons in Buttons.buttons)
                    foreach (var button in buttons)
                        if (button.buttonText == line)
                            button.enabled = true;
        }
        public static void Load2()
        {
            foreach (var buttons in Buttons.buttons)
                foreach (var button in buttons)
                    if (!button.toolTip.Contains("Changed"))
                        button.enabled = PlayerPrefs.HasKey(button.buttonText) ? JsonUtility.FromJson<bool>(PlayerPrefs.GetString(button.buttonText)) : false;
                    else
                    {
                        NormalColor = PlayerPrefs.HasKey("Menu Color") ? JsonUtility.FromJson<Color>(PlayerPrefs.GetString("Menu Color")) : Color.black;
                        FirstColor = PlayerPrefs.HasKey("Menu First Color") ? JsonUtility.FromJson<Color>(PlayerPrefs.GetString("Menu First Color")) : Color.black;
                        SecondColor = PlayerPrefs.HasKey("Menu Second Color") ? JsonUtility.FromJson<Color>(PlayerPrefs.GetString("Menu Second Color")) : Color.black;
                        ButtonColorEnabled = PlayerPrefs.HasKey("Button Color Enabled") ? JsonUtility.FromJson<Color>(PlayerPrefs.GetString("Button Color Enabled")) : Color.gray;
                        ButtonColorDisable = PlayerPrefs.HasKey("Button Color Disabled") ? JsonUtility.FromJson<Color>(PlayerPrefs.GetString("Button Color Disabled")) : Color.cyan;
                        physicSetting = PlayerPrefs.HasKey("Menu Physics") ? JsonUtility.FromJson<int>(PlayerPrefs.GetString("Menu Physics")) : 0;
                        projectile = PlayerPrefs.HasKey("Projectile") ? JsonUtility.FromJson<int>(PlayerPrefs.GetString("Projectile")) : -675036877;
                        trail = PlayerPrefs.HasKey("Trail") ? JsonUtility.FromJson<int>(PlayerPrefs.GetString("Trail")) : -1;
                        projectileSpeed = PlayerPrefs.HasKey("Proj Shoot Speed") ? JsonUtility.FromJson<int>(PlayerPrefs.GetString("Proj Shoot Speed")) : 1;
                        projectileColor = PlayerPrefs.HasKey("Projectile Color") ? JsonUtility.FromJson<Color>(PlayerPrefs.GetString("Projectile Color")) : Color.white;
                        Projectiles.Mode[0] = PlayerPrefs.HasKey("Proj Hand Type") ? JsonUtility.FromJson<int>(PlayerPrefs.GetString("Proj Hand Speed")) : 0;
                        GunLib.gunShape = PlayerPrefs.HasKey("Gun Type") ? JsonUtility.FromJson<PrimitiveType>(PlayerPrefs.GetString("Gun Type")) : PrimitiveType.Cube;
                        GunLib.disabledColor = PlayerPrefs.HasKey("Gun Disabled Color") ? JsonUtility.FromJson<Color>(PlayerPrefs.GetString("Gun Disabled Color")) : Color.black;
                        GunLib.enabledColor = PlayerPrefs.HasKey("Gun Enabled Color") ? JsonUtility.FromJson<Color>(PlayerPrefs.GetString("Gun Enabled Color")) : Color.black;
                        Projectiles.projSize = PlayerPrefs.HasKey("Projectile Scale") ? JsonUtility.FromJson<int>(PlayerPrefs.GetString("Projectile Scale")) : 1;
                    }
        }

        private static void BottomPageButtons()
        {
            if (Mode[15] == 0)
            {
                pageButtonSize = new Vector3(0.045f, 0.25f, 0.064295f);
                pagePoss[0] = new Vector3(0.56f, 0.37f, -0.78f);
                pagePoss[1] = new Vector3(0.56f, -0.37f, -0.78f);
            }
            if (Mode[15] == 1)
            {
                pageButtonSize = new Vector3(0.045f, 0.25f, 0.064295f);
                pagePoss[0] = new Vector3(0.56f, 0.37f, -0.68f);
                pagePoss[1] = new Vector3(0.56f, -0.37f, -0.68f);
            }
            if (Mode[15] == 2)
            {
                pageButtonSize = new Vector3(0.045f, 0.25f, 0.064295f);
                pagePoss[0] = new Vector3(0.56f, 0.37f, -.95f);
                pagePoss[1] = new Vector3(0.56f, -0.37f, -.95f);
            }
            if (Mode[15] == 3)
            {
                pageButtonSize = new Vector3(0.045f, 0.25f, 0.064295f);
                pagePoss[0] = new Vector3(0.56f, 0.65f, -0.68f);
                pagePoss[1] = new Vector3(0.56f, -0.65f, -0.68f);
            }
        }
        private static void SidePageButtons()
        {
            if (Mode[15] != 3)
            {
                pageButtonSize = new Vector3(0.045f, 0.25f, 1f);
                pagePoss[0] = new Vector3(0.56f, 0.65f, -.1f); // -.571f
                pagePoss[1] = new Vector3(0.56f, -0.65f, -.1f);
                return;
            }
            pageButtonSize = new Vector3(0.045f, 0.25f, 1f);
            pagePoss[0] = new Vector3(0.56f, .9f, -.1f); // -.571f
            pagePoss[1] = new Vector3(0.56f, -.9f, -.1f);
        }
        private static void TopPageButtons()
        {
            if (Mode[15] == 0)
            {
                pageButtonSize = new Vector3(0.045f, 0.25f, 0.064295f);
                pagePoss[0] = new Vector3(0.56f, 0.37f, 0.55f);
                pagePoss[1] = new Vector3(0.56f, -0.37f, 0.55f);
            }
            if (Mode[15] == 1)
            {
                pageButtonSize = new Vector3(0.045f, 0.25f, 0.064295f);
                pagePoss[0] = new Vector3(0.56f, 0.37f, 0.45f);
                pagePoss[1] = new Vector3(0.56f, -0.37f, 0.45f);
            }
            if (Mode[15] == 2)
            {
                pageButtonSize = new Vector3(0.045f, 0.25f, 0.064295f);
                pagePoss[0] = new Vector3(0.56f, 0.37f, 0.68f);
                pagePoss[1] = new Vector3(0.56f, -0.37f, 0.68f);
            }
            if (Mode[15] == 3)
            {
                pageButtonSize = new Vector3(0.045f, 0.25f, 0.064295f);
                pagePoss[0] = new Vector3(0.56f, 0.65f, 0.45f);
                pagePoss[1] = new Vector3(0.56f, -0.65f, 0.45f);
            }
        }
        private static void ThatGuyTemplate()
        {
            menuSize = new Vector3(.1f, 1, 1);
            buttonSize = new Vector3(.09f, .8f, .08f);
            buttonPos = new Vector3(.56f, 0, .19f);
            buttonOffset = 1;
            buttonTextPos = new Vector3(.064f, 0, .076f);
            buttonTextOffset = 2.55f;
            buttonFontStyle = FontStyle.Italic;
            titleFontStyle = FontStyle.BoldAndItalic;
            pageSize = 6;
            toolTipZ = -.22f;
            menuTitleZ = .125f;
        }
        private static void MysticTemplate()
        {
            menuSize = new Vector3(.1f, 1, 1.2f);
            buttonSize = new Vector3(.09f, .8f, .08f);
            buttonPos = new Vector3(.56f, 0, .29f);
            buttonOffset = 1f;
            buttonTextPos = new Vector3(.064f, 0f, .111f);
            buttonTextOffset = 2.6f; // 3.05
            buttonFontStyle = FontStyle.Normal;
            titleFontStyle = FontStyle.Normal;
            pageSize = 8;
            toolTipZ = -.27f;
            menuTitleZ = .173f;
        }
        public static void AZTemplate()
        {
            menuSize = new Vector3(.1f, 1, 1.5f);
            buttonSize = new Vector3(.09f, .9f, .08f);
            buttonPos = new Vector3(.56f, 0, .28f);
            buttonOffset = 1.15f;
            buttonTextPos = new Vector3(.064f, 0f, .111f);
            buttonTextOffset = 2.9f;
            buttonFontStyle = FontStyle.Italic;
            titleFontStyle = FontStyle.BoldAndItalic;
            pageSize = 8;
            toolTipZ = -.34f;
            menuTitleZ = .163f;
        }
        public static void KFJTemplate() // fix this
        {
            menuSize = new Vector3(.1f, 1.6f, 1f);
            buttonSize = new Vector3(.09f, 1.4f, .08f);
            buttonPos = new Vector3(.56f, 0, .23f);
            buttonOffset = 1.2f;
            buttonTextPos = new Vector3(.064f, 0f, .089f);
            buttonTextOffset = 3f;
            buttonFontStyle = FontStyle.Italic;
            titleFontStyle = FontStyle.BoldAndItalic;
            pageSize = 8;
            toolTipZ = -.24f;
            menuTitleZ = .133f;
        }
    }
}
