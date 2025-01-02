using MysticClient.Menu;
using static MysticClient.Menu.MenuSettings;
using static MysticClient.Mods.Settings;
using MysticClient.Utils;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Proj = MysticClient.Mods.Projectiles;
using MysticClient.Notifications;
using System.Runtime.CompilerServices;

namespace MysticClient.Classes
{
    public class Saving : MonoBehaviour
    {
        private static string[] SaveValues 
        { 
            get 
            {
                return new string[]
                {
                    $"MenuColor:{Mode[0]},0",
                    $"ButtonOnColor:{Mode[3]},3",
                    $"ButtonOffColor:{Mode[4]},4",
                    $"PhysicsMode:{Mode[5]},5",
                    $"GhostMode:{Mode[6]},6",
                    $"GunShape:{Mode[7]},7",
                    $"GunEnabledColor:{Mode[8]},8",
                    $"GunDisabledColor:{Mode[9]},9",
                    $"FlySpeed:{Mode[10]},10",
                    $"SpeedBoostMode:{Mode[11]},11",
                    $"GameDayTime:{Mode[12]},12",
                    $"HandPointerPosMode:{Mode[13]},13",
                    $"ButtonTextColor:{Mode[16]},16",
                    $"MenuDestroyTime:{Mode[17]},17",
                    $"PlatColor:{Mode[18]},18",
                    $"PlatFirstColor:{Mode[19]},19",
                    $"PlatSecondColor:{Mode[20]},20",
                    $"ButtonSoundMode:{Mode[21]},21",
                    $"GunLookType:{Mode[22]},22",
                    $"MenuTheme:{Mode[23]},23",
                    $"MenuOutlineColor:{Mode[24]},24",
                    $"HandTrailColor:{Mode[25]},25",
                    $"MinecraftSetBlock{Mode[26]},26",
                    $"MinecraftSetSong:{Mode[27]},27",
                    $"MenuFont:{Mode[28]},28",
                    $"MenuTrailColor:{Mode[29]},29",
                };
            } 
        }

        private static string[] ProjectileValues
        {
            get
            {
                return new string[]
                {
                    $"Projectile:{Proj.Mode[1]},1",
                    $"Trail:{Proj.Mode[2]},2",
                    $"ProjColor:{Proj.Mode[3]},3",
                    $"ProjSpeed:{Proj.Mode[4]},4",
                    $"ProjSize:{Proj.Mode[5]},5",
                };
            }
        }
        private static void SaveChangeValue()
        {
            Directory.CreateDirectory("MysticClient\\Buttons");
            File.WriteAllLines("MysticClient\\Buttons\\ChangeSaveValue.txt", SaveValues);
            File.WriteAllLines("MysticClient\\Buttons\\ChangeSaveProjValue.txt", ProjectileValues);
        }
        private static void LoadChangeProjValue()
        {
            if (!File.Exists("MysticClient\\Buttons\\ChangeSaveProjValue.txt"))
                return;

            var lines = File.ReadAllLines("MysticClient\\Buttons\\ChangeSaveProjValue.txt");

            foreach (var line in lines)
            {
                var parts = line.Split(':', ',');
                if (parts.Length != 3) continue;
                if (int.TryParse(parts[2], out int saveKey) && int.TryParse(parts[1], out int saveValue))
                {
                    Proj.Mode[saveKey] = saveValue;
                    Proj.RefreshProjSettings();
                }
                else Debug.LogWarning($"Failed to parse save data: {line}");
            }
        }
        private static void LoadChangeValue() // this shit took too long
        {
            if (!File.Exists("MysticClient\\Buttons\\ChangeSaveValue.txt")) 
            { 
                NotifiLib.SendNotification(NotifUtils.Error() + "Could Not Find Save File Did You Make A Save Yet?"); 
                return; 
            };

            var lines = File.ReadAllLines("MysticClient\\Buttons\\ChangeSaveValue.txt");

            foreach (var line in lines)
            {
                var parts = line.Split(':', ',');
                if (parts.Length != 3) continue;
                if (int.TryParse(parts[2], out int saveKey) && int.TryParse(parts[1], out int saveValue))
                {
                    Mode[saveKey] = saveValue;
                    RefreshSettings();
                } else Debug.LogWarning($"Failed to parse save data: {line}");
            }
        }
        private static void LoadChangeButtonNames()
        {
            if (!File.Exists("MysticClient\\Buttons\\ChangeSave.txt")) return;
            var lines = File.ReadAllLines("MysticClient\\Buttons\\ChangeSave.txt");
            foreach (var line in lines)
                foreach (var buttonss in Buttons.buttons)
                    foreach (var buttons in buttonss)
                        foreach (var button in buttons)
                            if (button.buttonText.Split(':')[0] == line.Split(':')[0])
                                button.buttonText = line;
        }

        private static void SaveChangeSettings()
        {
            var enabled = new List<string>();
            foreach (var buttonss in Buttons.buttons)
                foreach (var buttons in buttonss)
                    foreach (var button in buttons)
                        if (button.buttonText.Contains(":") && button.toolTip.Contains("Changed"))
                            enabled.Add(button.buttonText);
            Directory.CreateDirectory("MysticClient\\Buttons");
            File.WriteAllLines("MysticClient\\Buttons\\ChangeSave.txt", enabled);
        }

        private static void SaveEnabledButtons()
        {
            var enabled = new List<string>();
            foreach (var buttonss in Buttons.buttons)
                foreach (var buttons in buttonss)
                    foreach (var button in buttons)
                        if (button.enabled)
                            enabled.Add(button.buttonText);
            Directory.CreateDirectory("MysticClient\\Buttons");
            File.WriteAllLines("MysticClient\\Buttons\\Save.txt", enabled);
        }

        private static void LoadEnabledButtons()
        {
            if (!File.Exists("MysticClient\\Buttons\\Save.txt")) return;
            var lines = File.ReadAllLines("MysticClient\\Buttons\\Save.txt");
            foreach (var line in lines)
                foreach (var buttonss in Buttons.buttons)
                    foreach (var buttons in buttonss)
                        foreach (var button in buttons)
                            if (button.buttonText == line)
                                button.enabled = true;
        }

        public static void Save()
        {
            SaveEnabledButtons();
            SaveChangeSettings();
            SaveChangeValue();
        }

        public static void Load()
        {
            LoadChangeButtonNames();
            LoadEnabledButtons();
            LoadChangeValue();
            LoadChangeProjValue();
        }
    }
}