using UnityEngine;
using MysticClient.Classes;
using System;
using static Photon.Pun.PhotonNetwork;
using static UnityEngine.Application;
using static MysticClient.Mods.Movement;
using static MysticClient.Mods.Projectiles;
using static MysticClient.Mods.Rig;
using static MysticClient.Mods.Safety;
using static MysticClient.Mods.Settings;
using static MysticClient.Mods.Overpowered;
using static MysticClient.Menu.MenuSettings;
using static MysticClient.Mods.Fun;
using static MysticClient.Mods.Visuals;
using static MysticClient.Mods.Miscellaneous;
using static MysticClient.Notifications.NotifiLib;
using static MysticClient.Menu.Main;
using static MysticClient.Classes.Networking;
using MysticClient.Utils;
using Photon.Pun;
using MysticClient.Mods;
using System.Diagnostics;
using GorillaNetworking;
using OculusSampleFramework;
using MysticClient.Patches;


namespace MysticClient.Menu
{
    public class Buttons : MonoBehaviour
    {
        // AddButton("", false, ""),
        // AddButton("", ()=> , true, false, ""),
        // AddButton("", ()=> , ()=> , true, false, ""),
        public static ButtonInfo[][] buttons = new ButtonInfo[][]
        {
            new ButtonInfo[] // selections 0
            {
                AddButton("Join Discord", ()=> Process.Start("https://discord.gg/AEB2huHmvT"), false, false, "Joined Discord"),
                AddButton("Settings", ()=> EnterPage(1), false, false, "Opened Settings"),
                AddButton("Important Mods", ()=> EnterPage(8), false, false, "Opened Important Mods"),
                AddButton("Movement Mods", ()=> EnterPage(9), false, false, "Opened Movement Mods"),
                AddButton("Rig Mods", ()=> EnterPage(10), false, false, "Opened Rig Mods"),
                AddButton("Spammer Mods", ()=> EnterPage(11), false, false, "Opened Spammer Mods"),
                AddButton("Fun Mods", ()=> EnterPage(15), false, false, "Opened Fun Mods"),
                AddButton("Animal Mods", ()=> EnterPage(16), false, false, "Opened Animal Mods"),
                AddButton("Overpowered Mods", ()=> EnterPage(17), false, false, "Opened Overpowered Mods"),
                AddButton("Visual Mods", ()=> EnterPage(18), false, false, "Opened Visual Mods"),
                AddButton("Miscellaneous Mods", ()=> EnterPage(19), false, false, "Opened Miscellaneous Mods"),
                AddButton("Safety Mods", ()=> EnterPage(20), false, false, "Opened Movement Mods"),
            },

            new ButtonInfo[] // settings 1
            {
                AddButton("Exit Settings", ()=> EnterPage(0), false, false, "Closed Settings"),
                AddButton("Save Settings", ()=> Save(), false, false, "Saved Buttons"),
                AddButton("Load Settings", ()=> Load(), false, false, "Loaded Last Button Save"),
                AddButton("Menu Settings", ()=> EnterPage(2), false, false, "Opened Menu Settings"),
                AddButton("Rig Settings", ()=> EnterPage(3), false, false, "Opened Rig Settings"),
                AddButton("Notification Settings", ()=> EnterPage(4), false, false, "Opened Notification Settings"),
                AddButton("Mod Settings", ()=> EnterPage(5), false, false, "Opened Mod Settings"),
                AddButton("Projectile Settings", ()=> EnterPage(6), false, false, "Opened Projectile Settings"),
                AddButton("Gun Settings", ()=> EnterPage(7), false, false, "Opened Gun Settings"),
            },

            new ButtonInfo[] // menu settings 2
            {
                AddButton("Exit Menu Settings", ()=> EnterPage(1), false, false, "Closed Menu Settings"),
                AddButton("Right Hand Menu", false, "Put Menu On Right Hand"),
                AddButton("Side Disconnect", false, "Loaded Last Button Save"),
                AddButton("Menu Color: Black", ()=> ChangeMenuColor("Changed Menu Color"), false, false, "Changed Menu Color"),
                AddButton("Make Menu Flash", false, "Makes Menu Flash"),
                AddButton("Menu First Color: Black", ()=> ChangeMenuFirstColor("Changed Menu First Color"), false, false, "Changed Menu First Color"),
                AddButton("Menu Second Color: Black", ()=> ChangeMenuSecondColor("Changed Menu Second Color"), false, false, "Changed Menu Second Color"),
                AddButton("Button Color Enbaled: Gray", ()=> ChangeMenuButtonOnColor("Changed Button Color Enabled"), false, false, "Changed Button Color Enabled"),
                AddButton("Button Color Disabled: Cyan", ()=> ChangeMenuButtonOffColor("Changed Button Color Disabled"), false, false, "Changed Button Color Disabled"),
                AddButton("Button Text Color: Black", ()=> ChangeButtonTextColor("Changed Button Text Color"), true, false, "Changed Button Text Color"),
                AddButton("Menu Physics: None", ()=> ChangeMenuPhysics("Changed Menu Physics"), false, false, "Changed Menu Physics"),
                AddButton("Zero Gravity Menu", false, "Makes Menu Zero Gravity"),
                AddButton("Watch Menu", false, "Put Menu Above Your Rist"),
                AddButton("Pointer Position: Steam", ()=> ChangePointerPosition("Changed Pointer Position"), false, false, "Changed Pointer Position"),
                AddButton("Page Type: Top", ()=> ChangePageType("Changed Page Type"), false, false, "Changed Page Type"),
                AddButton("Menu Type: Mystic", ()=> ChangeMenuType("Changed Menu Type"), false, false, "Changed Menu Type"),
                AddButton("Multi Create", false, "Make The Menu Duplicate"),
                AddButton("Destroy Time: Instant", ()=> ChangeMenuDestroyTime("Change Menu Destroy Time"), false, false, "Change Menu Destroy Time"),
                AddButton("Destroy Menus", ()=> DestroyObjectsByName("MysticClientModMenu"), false, false, "Destroyed Any Mod Menus"),
                AddButton("Button Sound: Click", ()=> ChangeButtonSound("Changed Button Sound"), false, false, "Changed Button Sound"),
                AddButton("Date Time", false, "Shows Date Time Above Menu"),
                //AddButton("ServerSided Button Sounds", false, "Makes Button Sounds Server Sided"),
            },

            new ButtonInfo[] // rig settings 3
            {
                AddButton("Exit Rig Settings", ()=> EnterPage(1), false, false, "Closed Rig Settings"),
                AddButton("Ghost Type: None", ()=> ChangeGhostType("Changed Ghost Type"), false, false, "Changed Ghost Type"),
                //AddButton("Make Ghost/Invis Toggled", false, "Made Mods Toggled"), for the life of me i cant make a toggleable mod like this
            },

            new ButtonInfo[] // notification settings 4
            {
                AddButton("Exit Notification Settings", ()=> EnterPage(1), false, false, "Closed Notification Settings"),
                AddButton("Clear Notifications", ()=> ClearAllNotifications(), false, false, "Cleared Notifications"),
                AddButton("Disable Notifications", ()=> IsEnabled = false, ()=> IsEnabled = true, true, false, "Disabled Notifications"),
            },

            new ButtonInfo[] // mod settings 5
            {
                AddButton("Exit Mod Settings", ()=> EnterPage(1), false, false, "Closed Random Settings"),
                AddButton("Networking", ()=> EnterPage(21), false, false, "Closed Random Settings"),
                AddButton("Make Platforms Rigid", ()=> platformPhysics = true, ()=> platformPhysics = false, true, false, "Made Platforms Have Physics"),
                AddButton("Platform First Color: Purple", ()=> ChangePlatformFirstColor("Changed Platform First Color"), true, false, "Changed Platform First Color"),
                AddButton("Platform Second Color: Blue", ()=> ChangePlatformSecondColor("Changed Platform Second Color"), true, false, "Changed Platform Second Color"),
                AddButton("Make Platforms Single Color", ()=> platfromSingleColor = true, ()=> platfromSingleColor = false, true, false, "Makes Platforms A Single Color"),
                AddButton("Platform Color: Black", ()=> ChangePlatformColor("Changed Platform Color"), true, false, "Changed Platform Color"),
                AddButton("Fly Speed: Normal", ()=> ChangeFlySpeed("Changed Fly Speed"), true, false, "Changed Fly Speed"),
                AddButton("Speed Boost Speed: Normal", ()=> ChangeSpeedBoostSpeed("Changed Speed Boost Speed"), true, false, "Changed Speed Boost Speed"),
                AddButton("Time Of Day: Untouched", ()=> ChangeTimeOfDay("Changed Time Of Day"), true, false, "Changed Time Of Day"),
                AddButton("Disable Wind", ()=> SetWind(false), ()=> SetWind(true), true, false, "Disabled Wind"),
            },

            new ButtonInfo[] // projectile settings 6
            {
                AddButton("Exit Projectile Settings", ()=> EnterPage(1), false, false, "Closed Projectile Settings"),
                AddButton("Projectile: Snowball", ()=> ChangeProjectile("Changed Projectile"), false, false, "Changed Projectile"),
                AddButton("Trail: None", ()=> ChangeTrail("Changed Trail"), false, false, "Changed Trail"),
                AddButton("Shoot Speed: Normal", ()=> ChangeSpeed("Changed Shoot Speed"), false, false, "Changed Shoot Speed"),
                AddButton("Color: White", ()=> ChangeColor("Changed Color"), false, false, "Changed Color"),
                AddButton("Rain Bow", ()=> Projectiles.rainbowColor = true, ()=> Projectiles.rainbowColor = false, true, false, "Made Projectile Color Rainbow"),
                AddButton("Weird RGB", ()=> Projectiles.funnyRGB = true, ()=> Projectiles.funnyRGB = false, true, false, "Made Projectile Color Weird"),
                AddButton("Hand Type: Right Hand", ()=> ChangeHandType("Changed Hand Type"), false, false, "Changed Hand Type"),
                AddButton("Cycle Projectiles & Trails", ()=> Cycle(), true, false, "Cycles Through Projectiles And Trails"),
                AddButton("Projectile Size: Normal", ()=> ChangeProjectileScale("Changed Projectile Scale"), true, false, "Changed Projectile Scale"),
            },

            new ButtonInfo[] // gun settings 7
            {
                AddButton("Exit Gun Settings", ()=> EnterPage(1), false, false, "Closed Gun Settings"),
                AddButton("Gun Shape: Cube", ()=> ChangeGunType("Changed Gun Shape"), true, false, "Changed Gun Shape"),
                AddButton("Left Hand Gun", ()=> GunLib.leftHandGun = true, ()=> GunLib.leftHandGun = false, true, false, "Put Guns On Left Hand"),
                AddButton("Gun Disabled Color: Black", ()=> ChangeGunDisabled("Changed Disabled Color"), false, false, "Changed Disabled Color"),
                AddButton("Gun Enabled Color: Black", ()=> ChangeGunEnabled("Changed Enabled Color"), false, false, "Changed Enabled Color"),
                AddButton("Normal Gun", ()=> GunLib.normalGun = true, ()=> GunLib.normalGun = false, true, false, "Makes The Gun Not Do The Funny Little Bendy Thingy"),
            },

            new ButtonInfo[]  // important mods 8
            {
                AddButton("Exit Important Mods", ()=> EnterPage(0), false, false, "Closed Important Mods"),
                AddButton("Quit Game", ()=> Quit(), false, false, "BYEEE!"),
                AddButton("Disconnect", ()=> Disconnect("Disconnected"), true, false, "Disconnected"),
                AddButton("Join Random", ()=> JoinRandomRoom(), false, false, "Joined Random"),
                AddButton("Disable Network Triggers", ()=> GameObject.Find("Environment Objects/TriggerZones_Prefab/JoinRoomTriggers_Prefab").SetActive(false), ()=> GameObject.Find("Environment Objects/TriggerZones_Prefab/JoinRoomTriggers_Prefab").SetActive(true), true, false, "Disabled Network Triggers"),
                AddButton("Connect To US[W?]", ()=> ConnectToRegion("us"), true, false, "Attemting To Connect To US Region"),
                AddButton("Connect To USW[W?]", ()=> ConnectToRegion("usw"), true, false, "Attemting To Connect To USW Region"),
                AddButton("Connect To UE[W?]", ()=> ConnectToRegion("eu"), true, false, "Attemting To Connect To EU Region"),
                AddButton("MetaReportMenu", ()=> MetaReport(), false, false, "Enabled Meta Report Menu"),
                AddButton("Disable AFK Kick", ()=> PhotonNetworkController.Instance.disableAFKKick = true, ()=> PhotonNetworkController.Instance.disableAFKKick = false, true, false, "Disables AFK Kick"),

            },

            new ButtonInfo[] // movement 9
            {
                AddButton("Exit Movement Mods", ()=> EnterPage(0), false, false, "Closed Movement Mods"),
                AddButton("Platforms[G]", ()=> Platforms(false, false, false), true, false, "Grip For Platforms"),
                AddButton("InvisPlats[G]", ()=> Platforms(true, false, false), true, false, "Grip For Invis Platforms"),
                //AddButton("StickyPlats[G]", ()=> Platforms(false, true, false), true, false, "Grip For Sticky Platforms"), they dont work :(
                AddButton("PlankPlats[G]", ()=> Platforms(false, false, true), true, false, "Grip For Plank Platforms"),
                AddButton("NoClip[RT]", ()=> NoClip(), true, false, "Right Trigger For NoClip"),
                AddButton("Long Arms", ()=> SetArmLength(new Vector3(1.2f, 1.2f, 1.2f)), ()=> SetArmLength(new Vector3(1f, 1f, 1f)), true, false, "Your Still Short"),
                AddButton("Moon Gravity", ()=> SetGravity(new Vector3(0f, -3f, 0f)), ()=> SetGravity(new Vector3(0f, -9.81f, 0f)), true, false, "Sets Gravity To Low"),
                AddButton("Jupiter Gravity", ()=> SetGravity(new Vector3(0f, -20f, 0f)), ()=> SetGravity(new Vector3(0f, -9.81f, 0f)), true, false, "Sets Gravity To High"),
                AddButton("Space Gravity", ()=> SetGravity(new Vector3(0f, 0f, 0f)), ()=> SetGravity(new Vector3(0f, -9.81f, 0f)), true, false, "Sets Gravity To Nothing"),
                AddButton("Fly[A]", ()=> Fly(flySpeed), true, false, "A To Fly Forward"),
                AddButton("Speed Boost", ()=> SetSpeedBoost(speedBoostSpeed), true, false, "Set Speed To " + speedBoostSpeed.ToString()),
                AddButton("TPGun[RG][RT]", ()=> TPGun(), true, false, "Right Grip And Trigger To TP To Gun Point"),
                AddButton("EnderPearl[RG]", ()=> EnderPearl(), true, false, "Right Grip To Spawn Pearl"),
                AddButton("Bark Fly[J]", ()=> BarkFly(), true, false, "Joystick To Fly"),
                AddButton("IronMonke[G]", ()=> IronMonke(), true, false, "Grips To Use Iron Monke"),
                AddButton("Frozone[G]", ()=> Frozone(), true, false, "Grips To Slide On Platforms"),
                AddButton("To Tag Freeze", ()=> RigUtils.MyPlayer.disableMovement = false, ()=> RigUtils.MyPlayer.disableMovement = true, true, false, "Makes It Where You Can Move When You Get Tagged"),
            },

            new ButtonInfo[] // rig 10
            {
                AddButton("Exit Rig Mods", ()=> EnterPage(0), false, false, "Closed Rig Mods"),
                AddButton("GhostMonke[A]", ()=> GhostMonke(), true, false, "A For GhostMonke"),
                AddButton("InvisMonke[A]", ()=> InvisMonke(), true, false, "A For InvisMonke"),
                AddButton("GrabRig[RG]", ()=> GrabRig(), true, false, "Right Grip To Grab Rig"),
                AddButton("HeadSpin[X]", ()=> HeadTask("HeadSpinX"), ()=> HeadTask("Fix Head"), true, false, "Spins Head On X Axis"),
                AddButton("HeadSpin[Y]", ()=> HeadTask("HeadSpinY"), ()=> HeadTask("Fix Head"), true, false, "Spins Head On Y Axis"),
                AddButton("HeadSpin[Z]", ()=> HeadTask("HeadSpinZ"), ()=> HeadTask("Fix Head"), true, false, "Spins Head On Z Axis"),
                AddButton("BackwardsHead", ()=> HeadTask("180 Y Head"), ()=> HeadTask("Fix Head"), true, false, "Turns Head 180 On Y Axis"),
                AddButton("180 Head", ()=> HeadTask("180 Head"), ()=> HeadTask("Fix Head"), true, false, "Turns Head 180 On Z Axis"),
                AddButton("RGBMonke[CS]", ()=> SetOfflineColor(RGBColor()), true, false, "Made Your Rig Color RGB"),
                AddButton("WackyMonke", ()=> WackyMonke(), true, false, "Made Your Rig Look Wacky"),
            },

            new ButtonInfo[] // spammers 11
            {
                AddButton("Exit Spammers", ()=> EnterPage(0), false, false, "Closed Spammers"),
                AddButton("Projectiles", ()=> EnterPage(12), false, false, "Closed Water Spams"),
                AddButton("Water", ()=> EnterPage(13), false, false, "Closed Water Spams"),
                AddButton("Sound", ()=> EnterPage(14), false, false, "Closed Water Spams"),
            },

            new ButtonInfo[] // projectiles 12
            {
                AddButton("Exit Projectile Spammers", ()=> EnterPage(11), false, false, "Closed Projectile Spams"),
                AddButton("Projectile Spammer[RG][CS]", ()=> ProjectileSpammer(), true, false, "Right Grip To Spam Projectiles"),
                AddButton("PissSpam[RT][CS]", ()=> Piss(), true, false, "Right Trigger To Piss"),
                AddButton("ShitSpam[RT][CS]", ()=> Shit(), true, false, "Right Trigger To Shit"),
                AddButton("CumSpam[RT][CS]", ()=> Cum(), true, false, "Right Trigger To Cum"),
                AddButton("ProjectileGun[RG][RT][CS]", ()=> ProjectileGun(), true, false, "Press Right Grip And Trigger To Spam Projectiles On Gun Point"),
                AddButton("Projectile Halo", ()=> ProjectileHalo(), true, false, "Halos The Projectiles Around you"),
                AddButton("Projectile Settings", ()=> EnterPage(6), false, false, "Opened Projectile Settings"),
            },

            new ButtonInfo[] // water spams 13
            {
                AddButton("Exit Water Spammers", ()=> EnterPage(11), false, false, "Closed Water Spams"),
                AddButton("WaterRight[RG]", ()=> WaterRight("Right Grip To Spam Water On Right Hand"), true, false, "Right Grip To Spam Water On Right Hand"),
                AddButton("WaterLeft[RG]", ()=> WaterLeft("Left Grip To Spam Water On Left Hand"), true, false, "Left Grip To Spam Water On Left Hand"),
                AddButton("WaterBender[RG][LG]", ()=> WaterBender("Right And Left Grip To Spam Water"), true, false, "Right And Left Grip To Spam Water"),
                AddButton("WaterGun[RG][RT]", ()=> WaterGun("Right Grip And Right Trigger To Spam Water On Gun"), true, false, "Right Grip And Right Trigger To Spam Water On Gun"),
            },

            new ButtonInfo[] // sound 14
            {
                AddButton("Exit Sound Spammers", ()=> EnterPage(11), false, false, "Closed Sound Spams"),
                AddButton("Random[RT]", ()=> PlaySound(UnityEngine.Random.Range(0, 259), "Random[RT]"), true, false, "Right Trigger For Sound Spam"),
                AddButton("Metal[RT]", ()=> PlaySound(18, "Metal[RT]"), true, false, "Right Trigger For Sound Spam"),
                AddButton("AK[RT]", ()=> PlaySound(203, "AK[RT]"), true, false, "Right Trigger For Sound Spam"),
                AddButton("Siren[RT]", ()=> PlaySound(48 + ((UnityEngine.Time.frameCount / 15) % 2) * 2, "Siren[RT]"), true, false, "Right Trigger For Sound Spam"),
                AddButton("Pan[RT]", ()=> PlaySound(248, "Pan[RT]"), true, false, "Right Trigger For Sound Spam"),
                AddButton("Big Crystal[RT]", ()=> PlaySound(213, "Big Crystal[RT]"), true, false, "Right Trigger For Sound Spam"),
                AddButton("Crystal[RT]", ()=> PlaySound(crystalIndex[UnityEngine.Random.Range(0, 1)], "Crystal[RT]"), true, false, "Right Trigger For Sound Spam"),
                AddButton("Ding[RT]", ()=> PlaySound(244, "Ding[RT]"), true, false, "Right Trigger For Sound Spam"),
                AddButton("Ear Rape[RT]", ()=> PlaySound(215, "Ear Rape[RT]"), true, false, "Right Trigger For Sound Spam"),
                AddButton("Cloud[RT]", ()=> PlaySound(176, "Cloud[RT]"), true, false, "Right Trigger For Sound Spam"),
                AddButton("Bee[RT]", ()=> PlaySound(191, "Bee[RT]"), true, false, "Right Trigger For Sound Spam"),
                AddButton("Frog[RT]", ()=> PlaySound(91, "Frog[RT]"), true, false, "Right Trigger For Sound Spam"),
                AddButton("Turkey[RT]", ()=> PlaySound(83, "Trukey[RT]"), true, false, "Right Trigger For Sound Spam"),
                AddButton("Cat[RT]", ()=> PlaySound(236, "Cat[RT]"), true, false, "Right Trigger For Sound Spam"),
                AddButton("Wolf[RT]", ()=> PlaySound(195, "Walk[RT]"), true, false, "Right Trigger For Sound Spam"),
                AddButton("Bass[RT]", ()=> PlaySound(68, "Bass[RT]"), true, false, "Right Trigger For Sound Spam"),
                AddButton("Squeak[RT]", ()=> PlaySound(75 + (UnityEngine.Time.frameCount % 2), "Spueak[RT]"), true, false, "Right Trigger For Sound Spam"),
                AddButton("Click[RT]", ()=> PlaySound(67, "Click[RT]"), true, false, "Right Trigger For Sound Spam"),
                AddButton("Key[RT]", ()=> PlaySound(66, "Key[RT]"), true, false, "Right Trigger For Sound Spam"),
                AddButton("V Master Sounds V", null, false, false, "Master Sounds Under This Button"),
                AddButton("Count[RT]", ()=> PlayTagSound(1, "Count[RT]"), true, false, "Right Trigger For Sound Spam"),
                AddButton("Brawl Count[RT]", ()=> PlayTagSound(6, "Brawl Count[RT]"), true, false, "Right Trigger For Sound Spam"),
                AddButton("Brawl Start[RT]", ()=> PlayTagSound(7, "Brawl Start[RT]"), true, false, "Right Trigger For Sound Spam"),
                AddButton("Tag[RT]", ()=> PlayTagSound(0, "Tag[RT]"), true, false, "Right Trigger For Sound Spam"),
                AddButton("Tag End[RT]", ()=> PlayTagSound(2, "Tag End[RT]"), true, false, "Right Trigger For Sound Spam"),
                AddButton("Bonk[RT]", ()=> PlayTagSound(4, "Bonk[RT]"), true, false, "Right Trigger For Sound Spam"),
            },

            new ButtonInfo[] // fun 15
            {
                AddButton("Exit Fun Mods", ()=> EnterPage(0), false, false, "Closed Fun Mods"),
                AddButton("PlatformGun[RG][RT]", ()=> PlatformGun(), true, false, "Press Right Grip And Trigger To Spam Plats On Gun"),
                AddButton("DestroyPlats", ()=> DestroyObjectsByName("Plats"), false, false, "Detroyed Plats"),
                AddButton("Size Changer[RT][LT][A]", ()=> SizeChanger(), true, false, "Change The Size Of Your Rig"),
                AddButton("RGBSnowballs", ()=> RGBSnowballs(true), ()=> RGBSnowballs(false), true, false, "Made Snowballs RGB"),
                AddButton("Balls Gun[RG][RT]", ()=> BallsGun(), true, false, "Press Right Grip And Trigger To Spawn Balls On Point"),
                AddButton("DestroyBalls", ()=> DestroyObjectsByName("BALLS"), false, false, "Detroyed Plats"),
                AddButton("Get Gliders[RG]", ()=> GetGliders(), true, false, "Right Grip To Grab All Gliders"),
                AddButton("Glider Gun[RG][RT]", ()=> GliderGun(), true, false, "Right Grip And Trigger To Put Gliders On Point"),
                AddButton("Respawn Gliders", ()=> RespawnGliders(), false, false, "Respawned Gliders"),
                AddButton("Glider Trails", ()=> GliderTrails(), ()=> GliderTrailsOff(), true, false, "Put Trails On All The Gliders"),
                AddButton("Glider Fling Gun[RG][RT]", ()=> FlingGliderGun(), true, false, "Right Grip And Trigger To Fling Gliders To Point"),
                AddButton("Glider Tracers", ()=> GliderTracers(), true, false, "Put Tracers To All The Gliders"),
                AddButton("TP To Random Glider", ()=> TPToGlider(Gliders()[UnityEngine.Random.Range(0, Gliders().Length)]), false, false, "TPd To A Random Glider"),
                AddButton("Spin Gliders[RT]", ()=> SpinGliders(), true, false, "Right Trigger To Spin Gliders"),
                AddButton("Spaz Glider[RT]", ()=> SpazGliders(), true, false, "Right Trigger To Spaz Gliders"),
                AddButton("Gain Velocity[T]", ()=> GainVelocity(), true, false, "Triggers To Gain Velocity Best Use For Gliders"),
            },

            new ButtonInfo[] // amimal 16
            {
                AddButton("Exit Animal Mods", ()=> EnterPage(0), false, false, "Closed Animal Mods"),
                AddButton("Get Animal Ownership", ()=> GetAnimalOnwership(), false, false, "Gave You Animal Ownership"),
                AddButton("Grab Bug[RG]", ()=> GrabBug(), true, false, "Right Grip To Grab Bug"),
                AddButton("Grab Bat[RG]", ()=> GrabBat(), true, false, "Right Grip To Grab Bat"),
                AddButton("Orbit Bug", ()=> BugOrbit(), true, false, "Orbits Bug Around Player"),
                AddButton("Orbit Bat", ()=> BatOrbit(), true, false, "Orbits Bat Around Player"),
                AddButton("Spaz Bug", ()=> BugSpaz(), true, false, "Spazez Bug"),
                AddButton("Spaz Bat", ()=> BatSpaz(), true, false, "Spazez Bat"),
                AddButton("Bug Gun[RG][RT]", ()=> BugGun(), true, false, "Right Trigger And Grip To Put Gun On Point"),
                AddButton("Bat Gun[RG][RT]", ()=> BatGun(), true, false, "Right Trigger And Grip To Put Bat On Point"),
            },

            new ButtonInfo[] // overpowered 17
            {
                AddButton("Exit Overpowered Mods", ()=> EnterPage(0), false, false, "Closed Overpowered Mods"),
                AddButton("Tag All", ()=> TagAll("Attempting To Tag All"), true, false, "Attempting To Tag All"),
                AddButton("Tag Self", ()=> TagSelf("Attempting To Tag Self"), true, false, "Attempting To Tag Self"),
                AddButton("Tag Aura", ()=> TagAll("Tags The People Around You"), true, false, "Tags The People Around You"),
                AddButton("Tag Gun[RG][RT]", ()=> TagGun("Press Right Grip And Trigger To Tag Player"), true, false, "Press Right Grip And Trigger To Tag Player"),
                AddButton("UnTag All[M]", ()=> UnTagAll("Attempting To Un-Tag All"), true, false, "Attempting To Un-Tag All"),
                AddButton("UnTag Self[M]", ()=> UnTagSelf("Attempting To Un-Tag Self"), true, false, "Attempting To Un-Tag Self"),
                AddButton("UnTagGun[RG][RT][M]", ()=> UnTagGun("Press Right Grip And Trigger To Un-Tag Player"), true, false, "Press Right Grip And Trigger To Un-Tag Player"),
                AddButton("SpamTagAll[M]", ()=> SpamTagAll("Attempting To Spam Tag All"), true, false, "Attempting To Spam Tag All"),
                AddButton("Spam Tag Self[M]", ()=> SpamTagSelf("Attempting To Spam Tag Self"), true, false, "Attempting To Spam Tag Self"),
                AddButton("Spam Tag Gun[RG][RT][M]", ()=> SpamTagGun("Press Right Grip And Trigger To Spam Tag Player"), true, false, "Press Right Grip And Trigger To Spam Tag Player"),
                AddButton("Vibrate All[M]", ()=> VibrateAll("Vibrates Everyone In Room"), true, false, "Vibrates Everyone In Room"),
                AddButton("Vibrate Gun[RG][RT][M]", ()=> VibrateGun("Right Grip And Trigger To Vibrate Target"), true, false, "Right Grip And Trigger To Vibrate Target"),
                AddButton("Slow All[M]", ()=> SlowAll("Slows Everyone In Room"), true, false, "Slows Everyone In Room"),
                AddButton("Slow Gun[RG][RT][M]", ()=> SlowGun("Right Grip And Trigger To Slow Target"), true, false, "Right Grip And Trigger To Slow Target"),
                AddButton("Glider Blind All", ()=> GliderBlindAll(), true, false, "Closed Overpowered Mods"),
                AddButton("Glider Blind Gun[RG][RT]", ()=> GliderBlindGun(), true, false, "Right Grip And Trigger To Vibrate Target"),
            },

            new ButtonInfo[] // visuals 18
            {
                AddButton("Exit Visual Mods", ()=> EnterPage(0), false, false, "Closed Visual Mods"),
                AddButton("ESP", ()=> ESP(), ()=> ESPOff(), true, false, "Makes It So You Can Always See Other Players"),
                AddButton("Tracers", ()=> Tracers(), true, false, "Put Tracers From Your Hand To Players"),
                AddButton("Beacons", ()=> Beacons(), true, false, "Put Beacons On Players"),
            },

            new ButtonInfo[] // miscellaneous 19
            {
                AddButton("Exit Miscellaneous Mods", ()=> EnterPage(0), false, false, "Closed Miscellaneous Mods"),
                AddButton("Save All IDs", ()=> SaveAllIDs(), false, false, "Saved IDs To MysticClient//Miscellaneous"),
            },

            new ButtonInfo[] // safety 20
            {
                AddButton("Exit Safety Mods", ()=> EnterPage(0), false, false, "Closed Safety Mods"),
                AddButton("AntiReport", ()=> AntiReport(), true, false, "When Someone Gets Close To Your Report Button You Disconnect"),
            },

            new ButtonInfo[] // networking 21
            {
                AddButton("Exit Networking", ()=> EnterPage(5), false, false, "Closed Networking"),
                AddButton("Destory Networked Objects", ()=> DestroyNetworkedObjects(), false, false, "Destroyed Networked Objects"),
                AddButton("Destroy Networked Objects Side Button", false, "Put A Button On The Side Of The Menu To Destory Networked Objects"),
                AddButton("Receive Platforms", ()=> NetworkPlatforms(), ()=> UnNetworkPlatforms(), true, false, "Enabled Your Platform Receiver"),
                AddButton("Receive Plat Guns", ()=> NetworkPlatGun(), ()=> UnNetworkPlatGun(), true, false, "Enabled Your Platform Gun Receiver"),
                AddButton("Receive Ball Guns", ()=> NetworkBalls(), ()=> UnNetworkBalls(), true, false, "Enabled Your Ball Gun Receiver"),
                AddButton("Receive Frozones", ()=> NetworkFrozone(), ()=> UnNetworkFrozone(), true, false, "Enabled Your Frozone Receiver"),
                AddButton("Receive Projectiles", ()=> NetworkProjectiles(), ()=> UnNetworkProjectiles(), true, false, "Enabled Your Projectile Receiver"),
                AddButton("Disable Platform Network Colliders", ()=> NetworkColliders = false, ()=> NetworkColliders = true, true, false, "Enabled Your Projectile Receiver"),
            },
        };












        private static ButtonInfo AddButton(string text, bool enabled = false, string toolTip = "")
        {
            return new ButtonInfo
            {
                buttonText = text,
                method = null,
                disableMethod = null,
                isTogglable = true,
                enabled = enabled,
                toolTip = toolTip
            };
        }
        private static ButtonInfo AddButton(string text, Action method, bool isToggleable = true, bool enabled = false, string toolTip = "")
        {
            return new ButtonInfo
            {
                buttonText = text,
                method = method,
                disableMethod = null,
                isTogglable = isToggleable,
                enabled = enabled,
                toolTip = toolTip
            };
        }
        private static ButtonInfo AddButton(string text, Action method, Action disableMethod, bool isToggleable = true, bool enabled = false, string toolTip = "")
        {
            return new ButtonInfo
            {
                buttonText = text,
                method = method,
                disableMethod = disableMethod,
                isTogglable = isToggleable,
                enabled = enabled,
                toolTip = toolTip
            };
        }
        private static int[] crystalIndex = new int[]
        {
            UnityEngine.Random.Range(40, 54),
            UnityEngine.Random.Range(214, 221)
        };
        private static void Disconnect(string tooltip)
        {
            if (GetEnabled("Toggle Disconnect (B)"))
            {
                if (Controller.rightControllerSecondaryButton)
                {
                    PhotonNetwork.Disconnect();
                }
            }
            else
            {
                PhotonNetwork.Disconnect();
                GetToolTip(tooltip).enabled = false;
            }
        }







































        /*public static ButtonInfo[][] buttonss = new ButtonInfo[][]
{
            // AddButton("", ()=> , true, false, ""),
            // AddButton("", ()=> , ()=> , true, false, ""),
            new ButtonInfo[] // main buttons [0]
            {
                AddButton("Settings", ()=> EnterPage(1), false, false, "Opened Settings"),
                AddButton("Quit Game", ()=> Quit(), false, false, "BYEEE!"),
                AddButton("Disconnect", ()=> Disconnect("Disconnected"), true, false, "Disconnected"),
                AddButton("Join Random", ()=> JoinRandomRoom(), false, false, "Joined Random"),
                AddButton("Platforms[G]", ()=> Platforms(false, false, false), true, false, "Grip For Platforms"),
                AddButton("InvisPlats[G]", ()=> Platforms(true, false, false), true, false, "Grip For Invis Platforms"),
                AddButton("StickyPlats[G]", ()=> Platforms(false, true, false), true, false, "Grip For Sticky Platforms"),
                AddButton("PlankPlats[G]", ()=> Platforms(false, false, true), true, false, "Grip For Plank Platforms"),
                AddButton("NoClip[RT]", ()=> NoClip(), true, false, "Right Trigger For NoClip"),
                AddButton("GhostMonke[A]", ()=> GhostMonke(), true, false, "A For GhostMonke"),
                AddButton("InvisMonke[A]", ()=> InvisMonke(), true, false, "A For InvisMonke"),
                AddButton("GrabRig[RG]", ()=> GrabRig(), true, false, "Right Grip To Grab Rig"),
                AddButton("HeadSpin[X]", ()=> HeadTask("HeadSpinX"), ()=> HeadTask("Fix Head"), true, false, "Spins Head On X Axis"),
                AddButton("HeadSpin[Y]", ()=> HeadTask("HeadSpinY"), ()=> HeadTask("Fix Head"), true, false, "Spins Head On Y Axis"),
                AddButton("HeadSpin[Z]", ()=> HeadTask("HeadSpinZ"), ()=> HeadTask("Fix Head"), true, false, "Spins Head On Z Axis"),
                AddButton("BackwardsHead", ()=> HeadTask("180 Y Head"), ()=> HeadTask("Fix Head"), true, false, "Turns Head 180 On Y Axis"),
                AddButton("180 Head", ()=> HeadTask("180 Head"), ()=> HeadTask("Fix Head"), true, false, "Turns Head 180 On Z Axis"),
                AddButton("Long Arms", ()=> SetArmLength(new Vector3(1.2f, 1.2f, 1.2f)), ()=> SetArmLength(new Vector3(1f, 1f, 1f)), true, false, "Your Still Short"),
                AddButton("Moon Gravity", ()=> SetGravity(new Vector3(0f, -3f, 0f)), ()=> SetGravity(new Vector3(0f, -9.81f, 0f)), true, false, "Sets Gravity To Low"),
                AddButton("Jupiter Gravity", ()=> SetGravity(new Vector3(0f, -20f, 0f)), ()=> SetGravity(new Vector3(0f, -9.81f, 0f)), true, false, "Sets Gravity To High"),
                AddButton("Space Gravity", ()=> SetGravity(new Vector3(0f, 0f, 0f)), ()=> SetGravity(new Vector3(0f, -9.81f, 0f)), true, false, "Sets Gravity To Nothing"),
                AddButton("Speed Boost", ()=> SetSpeedBoost(speedBoostSpeed), true, false, "Set Speed To " + speedBoostSpeed.ToString()),
                AddButton("No Tag Freeze", ()=> RigUtils.MyPlayer.disableMovement = false, true, false, "When Tagged You Can Still Move"),
                AddButton("Slide Control", ()=> SetSlideControl(0.02f), ()=> SetSlideControl(0f), true, false, "Set Slide Control Amount To 0.02"),
                AddButton("Fly[X]", ()=> Fly(flySpeed), true, false, "X To Fly Forward"),
                AddButton("IronMonke[G]", ()=> IronMonke(), true, false, "Grips For Iron Monke"),
                AddButton("AntiReport[RG]", ()=> AntiReport(), ()=> AntiReportInternal(), true, false, "Right Grip To Spawn Ball Turn Off To Enable"),
                AddButton("Projectile Spammer[RG][CS]", ()=> ProjectileSpammer(), true, false, "Right Grip To Spam Projectiles"),
                AddButton("PissSpam[RT][CS]", ()=> Piss(), true, false, "Right Trigger To Piss"),
                AddButton("ShitSpam[RT][CS]", ()=> Shit(), true, false, "Right Trigger To Shit"),
                AddButton("CumSpam[RT][CS]", ()=> Cum(), true, false, "Right Trigger To Cum"),
                AddButton("ProjectileGun[RG][RT][CS]", ()=> ProjectileGun(), true, false, "Press Right Grip And Trigger To Spam Projectiles On Gun Point"),
                AddButton("Sound Spammers", ()=> EnterPage(2), false, false, "Opened Sound Spams"),
                AddButton("WaterRight[RG]", ()=> WaterRight("Right Grip To Spam Water On Right Hand"), true, false, "Right Grip To Spam Water On Right Hand"),
                AddButton("WaterLeft[RG]", ()=> WaterLeft("Left Grip To Spam Water On Left Hand"), true, false, "Left Grip To Spam Water On Left Hand"),
                AddButton("WaterBender[RG][LG]", ()=> WaterBender("Right And Left Grip To Spam Water"), true, false, "Right And Left Grip To Spam Water"),
                AddButton("WaterGun[RG][RT]", ()=> WaterGun("Right Grip And Right Trigger To Spam Water On Gun"), true, false, "Right Grip And Right Trigger To Spam Water On Gun"),
                AddButton("RGBSnowballs", ()=> RGBSnowballs(true), ()=> RGBSnowballs(false), true, false, "Made Snowballs RGB"),
                AddButton("Disable Network Triggers", ()=> GameObject.Find("Environment Objects/TriggerZones_Prefab/JoinRoomTriggers_Prefab").SetActive(false), ()=> GameObject.Find("Environment Objects/TriggerZones_Prefab/JoinRoomTriggers_Prefab").SetActive(true), true, false, "Disabled Network Triggers"),
                AddButton("Connect To US[W?]", ()=> ConnectToRegion("us"), true, false, "Attemting To Connect To US Region"),
                AddButton("Connect To USW[W?]", ()=> ConnectToRegion("usw"), true, false, "Attemting To Connect To USW Region"),
                AddButton("Connect To UE[W?]", ()=> ConnectToRegion("eu"), true, false, "Attemting To Connect To EU Region"),
                AddButton("PlatformGun[RG][RT]", ()=> PlatformGun(), true, false, "Press Right Grip And Trigger To Spam Plats On Gun"),
                AddButton("DestroyPlats", ()=> DestroyObjectsByName("Plats"), false, false, "Detroyed Plats"),
                AddButton("ESP", ()=> ESP(), ()=> ESPOff(), true, false, "Makes It So You Can Always See Other Players"),
                AddButton("RGBMonke[CS]", ()=> SetOfflineColor(RGBColor()), true, false, "Made Your Rig Color RGB"),
                AddButton("WackyMonke", ()=> WackyMonke(), true, false, "Made Your Rig Look Wacky"),
                AddButton("Size Changer[RT][LT][A]", ()=> SizeChanger(), true, false, "Change The Size Of Your Rig"),
                AddButton("Tracers", ()=> Tracers(), true, false, "Put Tracers From Your Hand To Players"),
                AddButton("Beacons", ()=> Beacons(), true, false, "Put Beacons On Players"),
                AddButton("Save All IDs", ()=> SaveAllIDs(), false, false, "Saved IDs To MysticClient//Miscellaneous"),
                AddButton("TPGun[RG][RT]", ()=> TPGun(), true, false, "Right Grip And Trigger To TP To Gun Point"),
                AddButton("Tag All", ()=> TagAll("Attempting To Tag All"), true, false, "Attempting To Tag All"),
                AddButton("Tag Self", ()=> TagSelf("Attempting To Tag Self"), true, false, "Attempting To Tag Self"),
                AddButton("Tag Aura", ()=> TagAll("Tags The People Around You"), true, false, "Tags The People Around You"),
                AddButton("Tag Gun[RG][RT]", ()=> TagGun("Press Right Grip And Trigger To Tag Player"), true, false, "Press Right Grip And Trigger To Tag Player"),
                AddButton("UnTag All[M]", ()=> UnTagAll("Attempting To Un-Tag All"), true, false, "Attempting To Un-Tag All"),
                AddButton("UnTag Self[M]", ()=> UnTagSelf("Attempting To Un-Tag Self"), true, false, "Attempting To Un-Tag Self"),
                AddButton("UnTagGun[RG][RT][M]", ()=> UnTagGun("Press Right Grip And Trigger To Un-Tag Player"), true, false, "Press Right Grip And Trigger To Un-Tag Player"),
                AddButton("SpamTagAll[M]", ()=> SpamTagAll("Attempting To Spam Tag All"), true, false, "Attempting To Spam Tag All"),
                AddButton("Spam Tag Self[M]", ()=> SpamTagSelf("Attempting To Spam Tag Self"), true, false, "Attempting To Spam Tag Self"),
                AddButton("Spam Tag Gun[RG][RT][M]", ()=> SpamTagGun("Press Right Grip And Trigger To Spam Tag Player"), true, false, "Press Right Grip And Trigger To Spam Tag Player"),
                AddButton("MetaReportMenu", ()=> MetaReport(), false, false, "Enabled Meta Report Menu"),
                AddButton("Get Animal Ownership", ()=> GetAnimalOnwership(), false, false, "Gave You Animal Ownership"),
                AddButton("Grab Bug[RG]", ()=> GrabBug(), true, false, "Right Grip To Grab Bug"),
                AddButton("Grab Bat[RG]", ()=> GrabBat(), true, false, "Right Grip To Grab Bat"),
                AddButton("Orbit Bug", ()=> BugOrbit(), true, false, "Orbits Bug Around Player"),
                AddButton("Orbit Bat", ()=> BatOrbit(), true, false, "Orbits Bat Around Player"),
                AddButton("Spaz Bug", ()=> BugSpaz(), true, false, "Spazez Bug"),
                AddButton("Spaz Bat", ()=> BatSpaz(), true, false, "Spazez Bat"),
                AddButton("Bug Gun[RG][RT]", ()=> BugGun(), true, false, "Right Trigger And Grip To Put Gun On Point"),
                AddButton("Bat Gun[RG][RT]", ()=> BatGun(), true, false, "Right Trigger And Grip To Put Bat On Point"),

            },

            new ButtonInfo[] // settings [1]
            {
                AddButton("Exit Settings", ()=> EnterPage(0), false, false, "Closed Settings"),
                AddButton("Save Preferences", ()=> Save(), false, false, "Saved Buttons"),
                AddButton("Load Preferences", ()=> Load(), false, false, "Loaded Last Button Save"),
                AddButton("First Person Camera", ()=> FPC(), ()=> TPC(), true, false, "Loaded Last Button Save"),
                AddButton("Side Disconnect", null, true, false, "Loaded Last Button Save"),
                AddButton("Menu Color: Black", ()=> ChangeMenuColor("Changed Menu Color"), false, false, "Changed Menu Color"),
                AddButton("Make Menu Flash", null, true, false, "Makes Menu Flash"),
                AddButton("Menu First Color: Black", ()=> ChangeMenuFirstColor("Changed Menu First Color"), false, false, "Changed Menu First Color"),
                AddButton("Menu Second Color: Black", ()=> ChangeMenuSecondColor("Changed Menu Second Color"), false, false, "Changed Menu Second Color"),
                AddButton("Button Color Enbaled: Gray", ()=> ChangeMenuButtonOnColor("Changed Button Color Enabled"), false, false, "Changed Button Color Enabled"),
                AddButton("Button Color Disabled: Cyan", ()=> ChangeMenuButtonOffColor("Changed Button Color Disabled"), false, false, "Changed Button Color Disabled"),
                AddButton("Menu Physics: None", ()=> ChangeMenuPhysics("Changed Menu Physics"), false, false, "Changed Menu Physics"),
                AddButton("Zero Gravity Menu", null, true, false, "Makes Menu Zero Gravity"),
                AddButton("Ghost Type: None", ()=> ChangeGhostType("Changed Ghost Type"), false, false, "Changed Ghost Type"),
                AddButton("Make Ghost/Invis Toggled", false, "Made Mods Toggled"),
                AddButton("Projectile Settings", ()=> EnterPage(3), false, false, "Opened Projectile Settings"),
                AddButton("Right Hand Menu", null, true, false, "Put Menu On Right Hand"),
                AddButton("Clear Notifications", ()=> ClearAllNotifications(), false, false, "Cleared Notifications"),
                //AddButton("Toggle Settings", ()=> EnterPage(4), false, false, "Opened Toggle Settings"), // might be fixed in the future
                AddButton("Gun Settings", ()=> EnterPage(5), false, false, "Opened Gun Settings"),
                AddButton("Disable Wind", ()=> DisableWind(), ()=> EnableWind(), true, false, "Disabled Wind"),



            },

            new ButtonInfo[] // sound spammers [2]
            {
                AddButton("Main Mods", ()=> EnterPage(0), false, false, "Closed Sound Spams"),
                AddButton("Random[RT]", ()=> PlaySound(UnityEngine.Random.Range(0, 259), "Random[RT]"), true, false, "Right Trigger For Sound Spam"),
                AddButton("Metal[RT]", ()=> PlaySound(18, "Metal[RT]"), true, false, "Right Trigger For Sound Spam"),
                AddButton("AK[RT]", ()=> PlaySound(203, "AK[RT]"), true, false, "Right Trigger For Sound Spam"),
                AddButton("Siren[RT]", ()=> PlaySound(48 + ((UnityEngine.Time.frameCount / 15) % 2) * 2, "Siren[RT]"), true, false, "Right Trigger For Sound Spam"),
                AddButton("Pan[RT]", ()=> PlaySound(248, "Pan[RT]"), true, false, "Right Trigger For Sound Spam"),
                AddButton("Big Crystal[RT]", ()=> PlaySound(213, "Big Crystal[RT]"), true, false, "Right Trigger For Sound Spam"),
                AddButton("Crystal[RT]", ()=> PlaySound(crystalIndex[UnityEngine.Random.Range(0, 1)], "Crystal[RT]"), true, false, "Right Trigger For Sound Spam"),
                AddButton("Ding[RT]", ()=> PlaySound(244, "Ding[RT]"), true, false, "Right Trigger For Sound Spam"),
                AddButton("Ear Rape[RT]", ()=> PlaySound(215, "Ear Rape[RT]"), true, false, "Right Trigger For Sound Spam"),
                AddButton("Bee[RT]", ()=> PlaySound(191, "Bee[RT]"), true, false, "Right Trigger For Sound Spam"),
                AddButton("Frog[RT]", ()=> PlaySound(91, "Frog[RT]"), true, false, "Right Trigger For Sound Spam"),
                AddButton("Turkey[RT]", ()=> PlaySound(83, "Trukey[RT]"), true, false, "Right Trigger For Sound Spam"),
                AddButton("Cat[RT]", ()=> PlaySound(236, "Cat[RT]"), true, false, "Right Trigger For Sound Spam"),
                AddButton("Wolf[RT]", ()=> PlaySound(195, "Walk[RT]"), true, false, "Right Trigger For Sound Spam"),
                AddButton("Bass[RT]", ()=> PlaySound(68, "Bass[RT]"), true, false, "Right Trigger For Sound Spam"),
                AddButton("Squeak[RT]", ()=> PlaySound(75 + (UnityEngine.Time.frameCount % 2), "Spueak[RT]"), true, false, "Right Trigger For Sound Spam"),
                AddButton("Click[RT]", ()=> PlaySound(67, "Click[RT]"), true, false, "Right Trigger For Sound Spam"),
                AddButton("Key[RT]", ()=> PlaySound(66, "Key[RT]"), true, false, "Right Trigger For Sound Spam"),
                AddButton("V Master Sounds V", null, false, false, "Opened Normal Sounds"),
                AddButton("Count[RT]", ()=> PlayTagSound(1, "Count[RT]"), true, false, "Right Trigger For Sound Spam"),
                AddButton("Brawl Count[RT]", ()=> PlayTagSound(6, "Brawl Count[RT]"), true, false, "Right Trigger For Sound Spam"),
                AddButton("Brawl Start[RT]", ()=> PlayTagSound(7, "Brawl Start[RT]"), true, false, "Right Trigger For Sound Spam"),
                AddButton("Tag[RT]", ()=> PlayTagSound(0, "Tag[RT]"), true, false, "Right Trigger For Sound Spam"),
                AddButton("Tag End[RT]", ()=> PlayTagSound(2, "Tag End[RT]"), true, false, "Right Trigger For Sound Spam"),
                AddButton("Bonk[RT]", ()=> PlayTagSound(4, "Bonk[RT]"), true, false, "Right Trigger For Sound Spam"),
            },

            new ButtonInfo[] // projectile settings [3]
            {
                AddButton("Settings", ()=> EnterPage(1), false, false, "Returned To Settings"),
                AddButton("Projectile: Snowball", ()=> ChangeProjectile("Changed Projectile"), false, false, "Changed Projectile"),
                AddButton("Trail: None", ()=> ChangeTrail("Changed Trail"), false, false, "Changed Trail"),
                AddButton("Shoot Speed: Normal", ()=> ChangeSpeed("Changed Shoot Speed"), false, false, "Changed Shoot Speed"),
                AddButton("Color: White", ()=> ChangeColor("Changed Color"), false, false, "Changed Color"),
                AddButton("Rain Bow", ()=> Projectiles.rainbowColor = true, ()=> Projectiles.rainbowColor = false, true, false, "Made Projectile Color Rainbow"),
                AddButton("Hand Type: Right Hand", ()=> ChangeHandType("Changed Hand Type"), false, false, "Changed Hand Type"),
                AddButton("Cycle Projectiles & Trails", ()=> Cycle(), true, false, "Cycles Through Projectiles And Trails"),
            },

            new ButtonInfo[] // toggle settings [4]
            {
                AddButton("Settings", ()=> EnterPage(1), false, false, "Returned To Settings"),
                AddButton("Toggle Disconnect (B)", null, true, false, "Made Mod Toggled"),
                AddButton("Toggle Head Spin (RT)", null, true, false, "Made Mod Toggled"),
                AddButton("Toggle Wacky Monke (RT)", null, true, false, "Made Mod Toggled"),
                AddButton("Toggle ESP (RT)", null, true, false, "Made Mod Toggled"),
                AddButton("Toggle Tracers (RT)", null, true, false, "Made Mod Toggled"),
                AddButton("Toggle Beacons (RT)", null, true, false, "Made Mod Toggled"),
                AddButton("Toggle Tag Aura (RT)", null, true, false, "Made Mod Toggled"),
                AddButton("Toggle Tag All (A)", null, true, false, "Made Mod Toggled"),
                AddButton("Toggle Tag Self (A)", null, true, false, "Made Mod Toggled"),
                AddButton("Toggle Tag UnTag All (A)", null, true, false, "Made Mod Toggled"),
                AddButton("Toggle Tag UnTag Self (A)", null, true, false, "Made Mod Toggled"),
                AddButton("Toggle Tag Spam Tag All (RT)", null, true, false, "Made Mod Toggled"),
                AddButton("Toggle Tag Spam Tag Self (RT)", null, true, false, "Made Mod Toggled"),
            },

            new ButtonInfo[] // gun settings [5]
            {
                AddButton("Settings", ()=> EnterPage(1), false, false, "Returned To Settings"),
                AddButton("Gun Shape: Cube", ()=> ChangeGunType("Changed Gun Shape"), true, false, "Changed Gun Shape"),
                AddButton("Left Hand Gun", ()=> GunLib.leftHandGun = true, ()=> GunLib.leftHandGun = false, true, false, "Put Guns On Left Hand"),
                AddButton("Gun Disabled Color: Black", ()=> ChangeGunDisabled("Changed Disabled Color"), false, false, "Changed Disabled Color"),
                AddButton("Gun Enabled Color: Black", ()=> ChangeGunEnabled("Changed Enabled Color"), false, false, "Changed Enabled Color"),
            },
};*/
    }
}
