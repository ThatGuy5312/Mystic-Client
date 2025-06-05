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
using static MysticClient.Mods.Fun;
using static MysticClient.Mods.Visuals;
using static MysticClient.Mods.Miscellaneous;
using static MysticClient.Notifications.NotifiLib;
using static MysticClient.Menu.Main;
using static MysticClient.Classes.Networking;
using static MysticClient.Classes.Saving;
using MysticClient.Utils;
using MysticClient.Mods;
using System.Diagnostics;
using GorillaNetworking;
using PlayFab.GroupsModels;
using Steamworks;
using MysticClient.Notifications;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Unity.XR.CoreUtils;


namespace MysticClient.Menu
{
    public class Buttons : MonoBehaviour
    {
        // AddButton("", false, ""),
        // AddButton("", ()=> , true, false, ""),
        // AddButton("", ()=> , ()=> , true, false, ""),
        // AddButton("", ()=> , false, ""), | for voice commands
        public static ButtonInfo[][][] buttons = new ButtonInfo[][][]
        {
            new ButtonInfo[][] // main mods 0
            {
                new ButtonInfo[] // selections 0
                {
                    AddButton("Join Discord", ()=> Process.Start("https://discord.gg/78d7rUVV9J"), false, false, "Joined Discord"),
                    AddButton("Settings", ()=> EnterPage(1), false, false, "Opened Settings"),
                    AddButton("Important Mods", ()=> EnterPage(2), false, false, "Opened Important Mods"),
                    AddButton("Movement Mods", ()=> EnterPage(3), false, false, "Opened Movement Mods"),
                    AddButton("Rig Mods", ()=> EnterPage(4), false, false, "Opened Rig Mods"),
                    AddButton("Spammer Mods", ()=> EnterPage(5), false, false, "Opened Spammer Mods"),
                    AddButton("Fun Mods", ()=> EnterPage(6), false, false, "Opened Fun Mods"),
                    AddButton("Animal Mods", ()=> EnterPage(7), false, false, "Opened Animal Mods"),
                    AddButton("Overpowered Mods", ()=> EnterPage(8), false, false, "Opened Overpowered Mods"),
                    AddButton("Visual Mods", ()=> EnterPage(9), false, false, "Opened Visual Mods"),
                    AddButton("Miscellaneous Mods", ()=> EnterPage(10), false, false, "Opened Miscellaneous Mods"),
                    AddButton("Safety Mods", ()=> EnterPage(11), false, false, "Opened Safety Mods"),
                    //AddButton("Enable keyboard", ()=> inKeyboard = true, ()=> inKeyboard = false, false, false, "puts a skidibi keyboard infront of the menu"),
                },

                new ButtonInfo[] // settings 1
                {
                    AddButton("Exit Settings", ()=> EnterPage(0), false, false, "Closed Settings"),
                    AddButton("Saving", ()=> EnterPage(7, 1), false, false, "Opened Saving"),
                    AddButton("Menu Settings", ()=> EnterPage(0, 1), false, false, "Opened Menu Settings"),
                    AddButton("Rig Settings", ()=> EnterPage(1, 1), false, false, "Opened Rig Settings"),
                    AddButton("Notification Settings", ()=> EnterPage(2, 1), false, false, "Opened Notification Settings"),
                    AddButton("Mod Settings", ()=> EnterPage(3, 1), false, false, "Opened Mod Settings"),
                    AddButton("Projectile Settings", ()=> EnterPage(4, 1), false, false, "Opened Projectile Settings"),
                    AddButton("Gun Settings", ()=> EnterPage(5, 1), false, false, "Opened Gun Settings"),
                },

                new ButtonInfo[]  // important mods 2
                {
                    AddButton("Exit Important Mods", ()=> EnterPage(0), false, false, "Closed Important Mods"),
                    AddButton("Quit Game", ()=> QuitGame(), false, false, "BYEEE!"),
                    AddButton("Disconnect", ()=> Disconnect(), false, false, "Disconnected"),
                    AddButton("Create Public", ()=> CreateRoom(RandomText(4)), false, false, "Joined Random"),
                    AddButton("Disable Network Triggers", ()=> GameObject.Find("Environment Objects/TriggerZones_Prefab/JoinRoomTriggers_Prefab").SetActive(false), ()=> GameObject.Find("Environment Objects/TriggerZones_Prefab/JoinRoomTriggers_Prefab").SetActive(true), true, false, "Disabled Network Triggers"),
                    AddButton("Connect To US[W?]", ()=> ConnectToRegion("us"), true, false, "Attemting To Connect To US Region"),
                    AddButton("Connect To USW[W?]", ()=> ConnectToRegion("usw"), true, false, "Attemting To Connect To US West Region"),
                    AddButton("Connect To UE[W?]", ()=> ConnectToRegion("eu"), true, false, "Attemting To Connect To EU Region"),
                    AddButton("MetaReportMenu", ()=> MetaReport(), false, false, "Enabled Meta Report Menu"),
                    AddButton("Disable AFK Kick", ()=> NetworkController.disableAFKKick = true, ()=> NetworkController.disableAFKKick = false, true, false, "Disables AFK Kick"),

                },

                new ButtonInfo[] // movement 3
                {
                    AddButton("Exit Movement Mods", ()=> EnterPage(0), false, false, "Closed Movement Mods"),
                    AddButton("Platforms[G]", ()=> Platforms(false, false, false), true, false, "Grip For Platforms"),
                    AddButton("InvisPlats[G]", ()=> Platforms(true, false, false), true, false, "Grip For Invis Platforms"),
                    //AddButton("StickyPlats[G]", ()=> Platforms(false, true, false), true, false, "Grip For Sticky Platforms"),
                    AddButton("PlankPlats[G]", ()=> Platforms(false, false, true), true, false, "Grip For Plank Platforms"),
                    AddButton("NoClip[RT]", ()=> NoClip(), true, false, "Right Trigger For NoClip"),
                    AddButton("Long Arms", ()=> SetArmLength(new Vector3(1.2f, 1.2f, 1.2f)), ()=> SetArmLength(new Vector3(1f, 1f, 1f)), true, false, "Your Still Short"),
                    AddButton("Moon Gravity", ()=> SetGravity(new Vector3(0f, -3f, 0f)), ()=> SetGravity(new Vector3(0f, -9.81f, 0f)), true, false, "Sets Gravity To Low"),
                    AddButton("Jupiter Gravity", ()=> SetGravity(new Vector3(0f, -20f, 0f)), ()=> SetGravity(new Vector3(0f, -9.81f, 0f)), true, false, "Sets Gravity To High"),
                    AddButton("Reverse Gravity", ()=> SetGravity(new Vector3(0f, 9.81f, 0f)), ()=> SetGravity(new Vector3(0f, -9.81f, 0f)), true, false, "Reverses Your Gravity"),
                    AddButton("Space Gravity", ()=> SetGravity(Vector3.zero), ()=> SetGravity(new Vector3(0f, -9.81f, 0f)), true, false, "Sets Gravity To Nothing"),
                    AddButton("Fly[X]", ()=> Fly(), true, false, "X To Fly Forward"),
                    AddButton("Velocity Fly[X]", ()=> VelocityFly(), true, false, "X To Gain A Forward Velocity"),
                    AddButton("Speed Boost", ()=> SetSpeedBoost(speedBoostSpeed), true, false, "Set Speed To " + speedBoostSpeed.ToString()),
                    AddButton("TPGun[RG][RT]", ()=> TPGun(), true, false, "Right Grip And Trigger To TP To Gun Point"),
                    AddButton("EnderPearl[RG]", ()=> EnderPearl(), true, false, "Right Grip To Spawn Pearl"),
                    AddButton("Bark Fly[J]", ()=> BarkFly(), true, false, "Joystick To Fly"),
                    AddButton("IronMonke[G]", ()=> IronMonke(), true, false, "Grips To Use Iron Monke", ()=> EnterPage(6, 3)),
                    AddButton("Frozone[G]", ()=> Frozone(), true, false, "Grips To Slide On Platforms"),
                    AddButton("No Tag Freeze", ()=> RigUtils.MyPlayer.disableMovement = false, true, false, "Makes It Where You Can Move When You Get Tagged"),
                    AddButton("SpiderMonke[RG][LG]", ()=> SpiderMonke(), ()=> SpiderMonkeOff(), true, false, "Grips To Use Grapplers"),
                    //AddButton("Pull Speed[RG]", ()=> Pull(), true, false, "When You Stop Touching The Ground And Your Holding Right Grip You Go Forward (thx Zav)"), zav skidded it
                    // AddButton("Fire Monke", ()=> IronMonkeN(), true, false, "Hawk Tuah"),
                    AddButton("Walk Sim[LJ][WASD]", ()=> WalkSim(), true, false, "Left Joystick Or WASD To Walk With Your Rig"),
                    AddButton("Airstrike Gun[RG][RT]", ()=> AirstrikeGun(), true, false, "Right Grip And Trigger To Air-Strike Yourself Into The Ground"),
                },

                new ButtonInfo[] // rig 4
                {
                    AddButton("Exit Rig Mods", ()=> EnterPage(0), false, false, "Closed Rig Mods"),
                    AddButton("GhostMonke[A]", ()=> GhostMonke(), true, false, "A For GhostMonke"),
                    AddButton("InvisMonke[A]", ()=> InvisMonke(), true, false, "A For InvisMonke"),
                    AddButton("GrabRig[RG]", ()=> GrabRig(), true, false, "Right Grip To Grab Rig"),
                    AddButton("ThrowRig[RG][A]", ()=> ThrowRig(), true, false, "Right Grip To Grab Rig And Release To Throw And A To Reset"),
                    AddButton("RigGun[RG][RT]", ()=> RigGun(), true, false, "Right Grip And Trigger To Put Rig Up Ponter"),
                    AddButton("HeadSpin[X]", ()=> HeadTask("HeadSpinX"), ()=> HeadTask("Fix Head"), true, false, "Spins Head On X Axis"),
                    AddButton("HeadSpin[Y]", ()=> HeadTask("HeadSpinY"), ()=> HeadTask("Fix Head"), true, false, "Spins Head On Y Axis"),
                    AddButton("HeadSpin[Z]", ()=> HeadTask("HeadSpinZ"), ()=> HeadTask("Fix Head"), true, false, "Spins Head On Z Axis"),
                    AddButton("BackwardsHead", ()=> HeadTask("180 Y Head"), ()=> HeadTask("Fix Head"), true, false, "Turns Head 180 On Y Axis"),
                    AddButton("180 Head", ()=> HeadTask("180 Head"), ()=> HeadTask("Fix Head"), true, false, "Turns Head 180 On Z Axis"),
                    AddButton("RGBMonke[CS]", ()=> SetOfflineColor(RGBColor()), true, false, "Made Your Rig Color RGB"),
                    AddButton("WackyMonke", ()=> WackyMonke(), true, false, "Made Your Rig Look Wacky"),
                    //AddButton("Leg Mod", ()=> LegMod(true), ()=> LegMod(false), true, false, "Made Your Arms Turn Into Legs"),
                },

                new ButtonInfo[] // spammers 5
                {
                    AddButton("Exit Spammers", ()=> EnterPage(0), false, false, "Closed Spammers"),
                    AddButton("Projectiles", ()=> EnterPage(0, 2), false, false, "Opended Projectile Spammers"),
                    AddButton("Water", ()=> EnterPage(1, 2), false, false, "Opened Water Spammers"),
                    AddButton("Sound", ()=> EnterPage(2, 2), false, false, "Opened Sound Spammers"),
                    //AddButton("Block", ()=> EnterPage(3, 2), false, false, "Opened Block Spammers"),
                },

                new ButtonInfo[] // fun 6
                {
                    AddButton("Exit Fun Mods", ()=> EnterPage(0), false, false, "Closed Fun Mods"),
                    AddButton("Minecraft", ()=> EnterPage(4, 2), false, false, "Opened Mincraft Section"),
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
                    AddButton("Glider Halo", ()=> GliderHalo(), true, false, "Halos Gliders Above Your Head"),
                    AddButton("Gain Velocity[T]", ()=> GainVelocity(), true, false, "Triggers To Gain Velocity Best Use For Gliders"),
                    AddButton("Car Monke[RJ]", ()=> CarMonke(), ()=> car.SetActive(false), true, false, "Right Joystick To Drive Car"),
                    AddButton("Spam Trigger", ()=> SpamGrab(), true, false, "Spams A Snowball When In Grab Zones"),
                },

                new ButtonInfo[] // amimal 7
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

                new ButtonInfo[] // overpowered 8
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
                    AddButton("No Tag On Join", ()=> TagJoin("nope", false), ()=> TagJoin("done", true), true, false, "Make It Where Your Not Tagged When You Join A Infection Lobby"),
                    //AddButton("Vibrate All[M][W?]", ()=> VibrateAll("Vibrates Everyone In Room"), true, false, "Vibrates Everyone In Room"),
                    //AddButton("Vibrate Gun[RG][RT][M][W?]", ()=> VibrateGun("Right Grip And Trigger To Vibrate Target"), true, false, "Right Grip And Trigger To Vibrate Target"),
                    //AddButton("Slow All[M][W?]", ()=> SlowAll("Slows Everyone In Room"), true, false, "Slows Everyone In Room"),
                    //AddButton("Slow Gun[RG][RT][M][W?]", ()=> SlowGun("Right Grip And Trigger To Slow Target"), true, false, "Right Grip And Trigger To Slow Target"),
                    AddButton("Glider Blind All", ()=> GliderBlindAll(), true, false, "Closed Overpowered Mods"),
                    AddButton("Glider Blind Gun[RG][RT]", ()=> GliderBlindGun(), true, false, "Right Grip And Trigger To Vibrate Target"),
                    AddButton("Ropes Up[RT]", ()=> RopeUp("Ropes Up[RT]"), true, false, "Right Trigger To Make Ropes Go Up"),
                    //AddButton("Ropes Up Gun[RG][RT]", ()=> RopeUpGun("Ropes Up Gun[RG][RT]"), true, false, "Right Grap And Trigger To Make Target Rope Go Up"),
                    AddButton("Ropes Down[RT]", ()=> RopeDown("Ropes Down[RT]"), true, false, "Right Trigger To Make Ropes Go Down"),
                    //AddButton("Ropes Down Gun[RG][RT]", ()=> RopeDownGun("Ropes Down Gun[RG][RT]"), true, false, "Right Grap And Trigger To Make Target Rope Go Down"),
                    AddButton("Ropes Spaz[RT]", ()=> RopeSpaz("Ropes Spaz[RT]"), true, false, "Right Trigger To Make Ropes Spaz"),
                    //AddButton("Ropes Spaz Gun[RG][RT]", ()=> SpazRopeGun("Ropes Spaz Gun[RG][RT]"), true, false, "Right Grap And Trigger To Make Target Rope Spaz"),
                    //AddButton("Kick Gun[RG][RT]", ()=> KickGun(), true, false, "Right Grip And Trigger To Kick Target Player"), // you really thought
                    //AddButton("Lag Gun[RG][RT]", ()=> LagGun(), true, false, "Right Grip And Trigger To Lag Target Player"),
                },
                // AddButton("", ()=> , ()=> , true, false, ""),
                new ButtonInfo[] // visuals 9
                {
                    AddButton("Exit Visual Mods", ()=> EnterPage(0), false, false, "Closed Visual Mods"),
                    //AddButton("Rain", ()=> Rain(), ()=> RainOff(), true, false, "Enables The Rainy Weather"),
                    AddButton("Beacons", ()=> Beacons(), true, false, "Put Beacons On Players"),

                    AddButton("Infection Tracers", ()=> InfectionTracers(), true, false, "Makes A Line To Whos Infected And Whos Not"),
                    AddButton("Casual Tracers", ()=> CasualTracers(), true, false, "Makes A Line From Your Right Hand To Everyone"),
                    //AddButton("Hunt Tracers", ()=> HuntTracers("Makes A Line To Your Hunt Target"), true, false, "Makes A Line To Your Hunt Target"),

                    AddButton("Infection ESP", ()=> InfectionESP(), ()=> ESPOff(), true, false, "Makes You See Whos Infected And Whos Not"),
                    AddButton("Casual ESP", ()=> CasualESP(), ()=> ESPOff(), true, false, "Makes You See Everyone No Matter Where They Are"),
                    //AddButton("Hunt ESP", ()=> HuntESP("Makes You See Who Your Target Is In Hunt"), ()=> ESPOff(), true, false, "Makes You See Who Your Target Is In Hunt"),
                    //AddButton("Voice ESP", ()=> VoiceESP(), ()=> ESPOff(), true, false, "Makes You See How Load Someone Is Talking Using A Through There Color Clearness"),
                    
                    //AddButton("NameTags", ()=> NameTags(), true, false, "Puts A NameTag On Top Of Everyone"),
                },

                new ButtonInfo[] // miscellaneous 10
                {
                    AddButton("Exit Miscellaneous Mods", ()=> EnterPage(0), false, false, "Closed Miscellaneous Mods"),
                    AddButton("Save All IDs", ()=> SaveAllIDs("Save All IDs"), false, false, "Saved IDs To MysticClient//Miscellaneous"),
                    AddButton("Get ID Gun[RG][RT]", ()=> SaveIDGun("Get ID Gun[RG][RT]"), true, false, "Right Grip And Trigger While Pointer Is On Target To Save ID"),
                    AddButton("Get Creation Date Gun[RG][RT]", ()=> GetCreationDateGun("Get Creation Date Gun[RG][RT]"), true, false, "Right Grip And Trigger While Pointer Is On Target To Get Creation Date"),
                },

                new ButtonInfo[] // safety 11
                {
                    AddButton("Exit Safety Mods", ()=> EnterPage(0), false, false, "Closed Safety Mods"),
                    AddButton("AntiReport", ()=> AntiReport(), true, false, "When Someone Gets Close To Your Report Button You Disconnect"),
                },
            },







            new ButtonInfo[][] // setting pages 1
            {
                new ButtonInfo[] // menu settings 0
                {
                    AddButton("Exit Menu Settings", ()=> EnterPage(1), false, false, "Closed Menu Settings"),
                    AddButton("Right Hand Menu", false, "Put Menu On Right Hand"),
                    AddButton("Side Disconnect", false, "Puts A Disconnect Button On Menus Side"),
                    AddButton("Return Button", false, "Puts A Return Button Next To The Search Button"),
                    AddButton("Menu Color: Black", ()=> ChangeMenuColor("Changed Menu Color"), false, false, "Changed Menu Color"),
                    AddButton("Outline Menu", false, "Puts An Outline On The Menu"),
                    AddButton("Outline Color: Black", ()=> ChangeMenuOutlineColor("Changed Outline Color"), false, false, "Changed Outline Color"),
                    //AddButton("Make Menu Flash", false, "Makes Menu Flash"),
                    //AddButton("Menu First Color: Black", ()=> ChangeMenuFirstColor("Changed Menu First Color"), false, false, "Changed Menu First Color"),
                    //AddButton("Menu Second Color: Black", ()=> ChangeMenuSecondColor("Changed Menu Second Color"), false, false, "Changed Menu Second Color"),
                    AddButton("Button Color Enbaled: Gray", ()=> ChangeMenuButtonOnColor("Changed Button Color Enabled"), false, false, "Changed Button Color Enabled"),
                    AddButton("Button Color Disabled: Cyan", ()=> ChangeMenuButtonOffColor("Changed Button Color Disabled"), false, false, "Changed Button Color Disabled"),
                    AddButton("Button Text Color: Black", ()=> ChangeButtonTextColor("Changed Button Text Color"), true, false, "Changed Button Text Color"),
                    AddButton("Menu Physics: None", ()=> ChangeMenuPhysics("Changed Menu Physics"), false, false, "Changed Menu Physics"),
                    AddButton("Zero Gravity Menu", false, "Makes Menu Zero Gravity"),
                    AddButton("Watch Menu", false, "Put Menu Above Your Rist"),
                    AddButton("Pointer Position: Steam", ()=> ChangePointerPosition("Changed Pointer Position"), false, false, "Changed Pointer Position"),
                    //AddButton("Page Type: Top", ()=> ChangePageType("Changed Page Type"), false, false, "Changed Page Type"),
                    //AddButton("Menu Type: Mystic", ()=> ChangeMenuType("Changed Menu Type"), false, false, "Changed Menu Type"),
                    AddButton("Menu Theme: Mystic", ()=> ChangeMenuTheme("Changed Menu Theme"), false, false, "Changed Menu Theme"),
                    AddButton("Multi Create", false, "Make The Menu Duplicate"),
                    AddButton("Destroy Time: Instant", ()=> ChangeMenuDestroyTime("Changed Menu Destroy Time"), false, false, "Changed Menu Destroy Time"),
                    AddButton("Destroy Menus", ()=> DestroyObjectsByName("MysticClientModMenu"), false, false, "Destroyed Any Mod Menus"),
                    AddButton("Button Sound: Click", ()=> ChangeButtonSound("Changed Button Sound"), false, false, "Changed Button Sound"),
                    //AddButton("Date Time", false, "Shows Date Time Above Menu"), doesnt look that good
                    //AddButton("ServerSided Button Sounds", false, "Makes Button Sounds Server Sided"),
                    AddButton("Dynamic Sounds", ()=> { if (fastLoad) { GetIndex("Dynamic Sounds").enabled = false; SendNotification(NotifUtils.Warning() + "Config 'Fast Load' is enabled this setting is now unuseable"); } }, true, false, "Gives The Menu A Nice User Sounds"),
                    AddButton("Menu Trail", false, "Makes The Menu Have A Trail That Follows It", ()=> EnterPage(4, 3)),
                    AddButton("Use System Colors", false, "Make Color Changers Use System.Drawing.Color Instead Of UnityEngine.Color"),
                    AddButton("Annoying Menu", false, "You Would Never Get Annoyed Of This Menu. Right?"),
                    AddButton("Menu Font: Arial", ()=> ChangeFont("Changed Menu Font"), false, false, "Changed Menu Font"),
                    AddButton("Array List", false, "Puts An Array Of All Your Enabled Mods On Screen", ()=> EnterPage(5, 3)),
                    AddButton("Shiny Menu", false, "Makes The Menu Have A Shiny Look"),
                    AddButton("Disable Stump Planet", false, "Disables The Planet In Stump"),
                    AddButton("No Button Colliders", false, "Makes It So You Cant Use The Menu As A Platform"),
                    AddButton("Face Menu", false, "Makes The Menu Show Infront Of Your Face"),
                    AddButton("Semi-Transparent Menu", false, "Makes The Menu Semi-Transparent"),
                    AddButton("Round Menu", false, "Rounds The Menu"),
                    AddButton("Text UI Menu", false, "Adds A Text UI To Use Instead Of The Menu"),
                    AddButton("Current Catagory Searching", false, "Makes It Where When You Search It Will Only Show The Mods That Are In Your Current Catagory"),
                },

                new ButtonInfo[] // rig settings 1
                {
                    AddButton("Exit Rig Settings", ()=> EnterPage(1), false, false, "Closed Rig Settings"),
                    AddButton("Ghost Type: None", ()=> ChangeGhostType("Changed Ghost Type"), false, false, "Changed Ghost Type"),
                    AddButton("Make Ghost/Invis Toggled", false, "Makes GhostMonke And InvisMonke Toggled With A"),
                },

                new ButtonInfo[] // notification settings 2
                {
                    AddButton("Exit Notification Settings", ()=> EnterPage(1), false, false, "Closed Notification Settings"),
                    AddButton("Clear Notifications", ()=> ClearAllNotifications(), false, false, "Cleared Notifications"),
                    AddButton("Disable Notifications", ()=> disableNotifications = true, ()=> disableNotifications = false, true, false, "Disabled Notifications"),
                },

                new ButtonInfo[] // mod settings 3
                {
                    AddButton("Exit Mod Settings", ()=> EnterPage(1), false, false, "Closed Random Settings"),
                    AddButton("Voice Commands", ()=> EnableVoiceCommands(), ()=> StopVoiceCommands(), true, false, "Enabled Voice Commands"),
                    AddButton("Networking", ()=> EnterPage(6, 1), false, false, "Opened Networking Settings"),
                    AddButton("Make Platforms Rigid", ()=> platformPhysics = true, ()=> platformPhysics = false, true, false, "Made Platforms Have Physics"),
                    AddButton("Platform First Color: Purple", ()=> ChangePlatformFirstColor("Changed Platform First Color"), true, false, "Changed Platform First Color"),
                    AddButton("Platform Second Color: Blue", ()=> ChangePlatformSecondColor("Changed Platform Second Color"), true, false, "Changed Platform Second Color"),
                    AddButton("Make Platforms Single Color", ()=> platfromSingleColor = true, ()=> platfromSingleColor = false, true, false, "Makes Platforms A Single Color"),
                    AddButton("Platform Color: Black", ()=> ChangePlatformColor("Changed Platform Color"), true, false, "Changed Platform Color"),
                    AddButton("Fly Speed: Normal", ()=> ChangeFlySpeed("Changed Fly Speed"), true, false, "Changed Fly Speed"),
                    AddButton("Speed Boost Speed: Normal", ()=> ChangeSpeedBoostSpeed("Changed Speed Boost Speed"), true, false, "Changed Speed Boost Speed"),
                    AddButton("Super Speed Boost", false, "Makes The Speed Boost Mod Super"),
                    AddButton("Time Of Day: Untouched", ()=> ChangeTimeOfDay("Changed Time Of Day"), true, false, "Changed Time Of Day"),
                    AddButton("Load Audios[LAG SPIKE!]", ()=> LoadAudios("Load Audios[LAG SPIKE!]"), true, false, "Loads All Audios When Comfirmed"),
                    //AddButton("Disable Wind", ()=> SetWind(false), ()=> SetWind(true), true, false, "Disabled Wind"), not working
                },

                new ButtonInfo[] // projectile settings 4
                {
                    AddButton("Exit Projectile Settings", ()=> EnterPage(1), false, false, "Closed Projectile Settings"),
                    AddButton("Projectile: Snowball", ()=> ChangeProjectile("Changed Projectile"), false, false, "Changed Projectile"),
                    AddButton("Trail: None", ()=> ChangeTrail("Changed Projectile Trail"), false, false, "Changed Projectile Trail"),
                    AddButton("Shoot Speed: Normal", ()=> ChangeSpeed("Changed Projectile Shoot Speed"), false, false, "Changed Projectile Shoot Speed"),
                    AddButton("Color: White", ()=> ChangeColor("Changed Projectile Color"), false, false, "Changed Projectile Color"),
                    AddButton("Rain Bow", ()=> Projectiles.rainbowColor = true, ()=> Projectiles.rainbowColor = false, true, false, "Made Projectile Color Rainbow"),
                    AddButton("Weird RGB", ()=> Projectiles.funnyRGB = true, ()=> Projectiles.funnyRGB = false, true, false, "Made Projectile Color Weird"),
                    AddButton("Hand Type: Right Hand", ()=> ChangeHandType("Changed Projectile Hand Type"), false, false, "Changed Projectile Hand Type"),
                    AddButton("Cycle Projectiles & Trails", ()=> Cycle(), true, false, "Cycles Through Projectiles And Trails"),
                    AddButton("Projectile Size: Normal", ()=> ChangeProjectileScale("Changed Projectile Scale"), true, false, "Changed Projectile Scale"),
                },

                new ButtonInfo[] // gun settings 5
                {
                    AddButton("Exit Gun Settings", ()=> EnterPage(1), false, false, "Closed Gun Settings"),
                    AddButton("Gun Shape: Cube", ()=> ChangeGunShape("Changed Gun Shape"), false, false, "Changed Gun Shape"),
                    AddButton("Left Hand Gun", ()=> GunLib.leftHandGun = true, ()=> GunLib.leftHandGun = false, true, false, "Put Guns On Left Hand"),
                    AddButton("Gun Disabled Color: Black", ()=> ChangeGunDisabled("Changed Disabled Color"), false, false, "Changed Disabled Color"),
                    AddButton("Gun Enabled Color: Black", ()=> ChangeGunEnabled("Changed Enabled Color"), false, false, "Changed Enabled Color"),
                    AddButton("Gun Type: Sharp", ()=> ChangeGunType("Changed Gun Type"), false, false, "Changed Gun Type"),
                    AddButton("Old Gun Direction", ()=> GunLib.oldRayDirection = true, ()=> GunLib.oldRayDirection = false, true, false, "Make The Gun Go Down Instead On Up"),
                },

                new ButtonInfo[] // networking 6
                {
                    AddButton("Exit Networking", ()=> EnterPage(3, 1), false, false, "Closed Networking"),
                    AddButton("Destory Networked Objects", ()=> DestroyNetworkedObjects(), false, false, "Destroyed Networked Objects"),
                    AddButton("Destroy Networked Objects Side Button", false, "Put A Button On The Side Of The Menu To Destory Networked Objects"),
                    AddButton("Receive Platforms", ()=> NetworkPlatforms(), ()=> UnNetworkPlatforms(), true, false, "Enabled Your Platform Receiver"),
                    AddButton("Receive Plat Guns", ()=> NetworkPlatGun(), ()=> UnNetworkPlatGun(), true, false, "Enabled Your Platform Gun Receiver"),
                    AddButton("Receive Ball Guns", ()=> NetworkBalls(), ()=> UnNetworkBalls(), true, false, "Enabled Your Ball Gun Receiver"),
                    AddButton("Receive Frozones", ()=> NetworkFrozone(), ()=> UnNetworkFrozone(), true, false, "Enabled Your Frozone Receiver"),
                    AddButton("Receive Projectiles", ()=> NetworkProjectiles(), ()=> UnNetworkProjectiles(), true, false, "Enabled Your Projectile Receiver"),
                    AddButton("Receive Minecraft", ()=> NetworkMinecraft(), ()=> UnNetworkMinecraft(), true, false, "Enabled Your Minecraft Receiver"),
                    AddButton("Disable Platform Network Colliders", ()=> NetworkColliders = false, ()=> NetworkColliders = true, true, false, "Disables Platform Networks Colliders"),
                },

                new ButtonInfo[] // saving 7
                {
                    AddButton("Exit Saving", ()=> EnterPage(1), false, false, "Closed Saving"),
                    AddButton("Save Settings", ()=> Save(), false, false, "Saved Settings"),
                    AddButton("Load Settings", ()=> Load(), false, false, "Loaded Last Settings Save"),
                    AddButton("Auto Load Save", false, "Auto Loads Save On Startup"),
                },

                new ButtonInfo[] // spammer settings 8
                {
                    //AddButton("Exit Spammer Settings", ()=> EnterPage(1), false, false, "Closed Spammer Settings"),
                    //AddButton("Projectile Settings", ()=> EnterPage(4, 1), false, false, "Opened Projectile Settings"),
                    //AddButton("Block Settings", ()=> EnterPage(9, 1), false, false, "Opened Block Settings"),
                },

                new ButtonInfo[] // block spammer settings 9
                {
                    //AddButton("Exit Block Settings", ()=> EnterPage(8, 1), false, false, "Closed Block Settings"),
                    //AddButton("Spam Delay: .2", ()=> EnterPage(8, 1), false, false, "Changed Block Spam Delay"),
                },
            },








            new ButtonInfo[][] // spammers 2
            {
                new ButtonInfo[] // projectiles 0
                {
                    AddButton("Exit Projectile Spammers", ()=> EnterPage(5), false, false, "Closed Projectile Spams"),
                    AddButton("Projectile Spammer[RG][CS]", ()=> ProjectileSpammer(), true, false, "Right Grip To Spam Projectiles"),
                    AddButton("PissSpam[RT][CS]", ()=> Piss(), true, false, "Right Trigger To Piss"),
                    AddButton("ShitSpam[RT][CS]", ()=> Shit(), true, false, "Right Trigger To Shit"),
                    AddButton("CumSpam[RT][CS]", ()=> Cum(), true, false, "Right Trigger To Cum"),
                    AddButton("ProjectileGun[RG][RT][CS]", ()=> ProjectileGun(), true, false, "Press Right Grip And Trigger To Spam Projectiles On Gun Point"),
                    AddButton("Projectile Halo", ()=> ProjectileHalo(), true, false, "Halos The Projectiles Around you"),
                    AddButton("Projectile Settings", ()=> EnterPage(6), false, false, "Opened Projectile Settings"),
                },

                new ButtonInfo[] // water spams 1
                {
                    AddButton("Exit Water Spammers", ()=> EnterPage(5), false, false, "Closed Water Spams"),
                    AddButton("WaterRight[RG]", ()=> WaterRight("WaterRight[RG]"), true, false, "Right Grip To Spam Water On Right Hand"),
                    AddButton("WaterLeft[RG]", ()=> WaterLeft("WaterLeft[RG]"), true, false, "Left Grip To Spam Water On Left Hand"),
                    AddButton("WaterBender[RG][LG]", ()=> WaterBender("WaterBender[RG][LG]"), true, false, "Right And Left Grip To Spam Water"),
                    AddButton("WaterGun[RG][RT]", ()=> WaterGun("WaterGun[RG][RT]"), true, false, "Right Grip And Trigger To Spam Water On Gun"),
                    //AddButton("GiveWaterBender[RG][RT]", ()=> GiveWaterBender("GiveWaterBender[RG][RT]"), true, false, "Right Grip And Trigger To Spam Water On Targets Hands"),
                },

                new ButtonInfo[] // sound 2
                {
                    AddButton("Exit Sound Spammers", ()=> EnterPage(5), false, false, "Closed Sound Spams"),
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
                    AddButton("Count[RT][M]", ()=> PlayTagSound(1, "Count[RT]"), true, false, "Right Trigger For Sound Spam"),
                    AddButton("Brawl Count[RT][M]", ()=> PlayTagSound(6, "Brawl Count[RT]"), true, false, "Right Trigger For Sound Spam"),
                    AddButton("Brawl Start[RT][M]", ()=> PlayTagSound(7, "Brawl Start[RT]"), true, false, "Right Trigger For Sound Spam"),
                    AddButton("Tag[RT][M]", ()=> PlayTagSound(0, "Tag[RT]"), true, false, "Right Trigger For Sound Spam"),
                    AddButton("Tag End[RT][M]", ()=> PlayTagSound(2, "Tag End[RT]"), true, false, "Right Trigger For Sound Spam"),
                    AddButton("Bonk[RT][M]", ()=> PlayTagSound(4, "Bonk[RT]"), true, false, "Right Trigger For Sound Spam"),
                    AddButton("Freeze[RT][M]", ()=> PlayTagSound(11, "Freeze[RT]"), true, false, "Right Trigger For Sound Spam"),
                },

                new ButtonInfo[] // blocks :3
                {
                    AddButton("Exit Block Spammers", ()=> EnterPage(5), false, false, "Closed Block Spams"),
                    AddButton("Block Spammer[RG]", ()=> SpamBlock("Right Grip To Spam Block Piece"), true, false, "Right Grip To Spam Block Piece"),
                    AddButton("Block Gun[RG][RT]", ()=> SpamBlockGun("Right Grip And Trigger To Spam Blocks On Gun"), true, false, "Right Grip And Trigger To Spam Blocks On Gun"),
                    AddButton("Block Halo", ()=> BlockHalo("Halos Blocks Around You"), true, false, "Halos Blocks Around You"),
                    AddButton("Random Block Spammer[RG]", ()=> SpamBlockRandom("Right Grip To Spam Random Block Piece"), true, false, "Right Grip To Spam Random Block Piece"),
                    AddButton("Copy Block ID Gun[RG][RT]", ()=> GetBlockIDGun("Right Grip And Trigger To Copy Target Piece ID"), true, false, "Right Grip And Trigger To Copy Target Piece ID"),
                    AddButton("Recycle Block Gun[RG][RT][M?]", ()=> DestroyBlockGun("Right Grip And Trigger To Recycle Target Piece"), true, false, "Right Grip And Trigger To Recycle Target Piece"),
                },


                // this isnt spammers but i dont want to make another array
                new ButtonInfo[] // minecraft 4
                {
                    AddButton("Exit Minecraft", ()=> EnterPage(6), false, false, "Closed Minecraft"),
                    AddButton("Minecraft Mod[RG][RT]", ()=> Minecraft(), true, false, "Right Grip And Trigger To Spawn Block On Grid", ()=> EnterPage(3, 3)),
                    AddButton("Pickaxe", false,  "Puts A Pickaxe On Your Hand Which You Can Break Block With", ()=> EnterPage(1, 3)),
                    AddButton("Destroy Minecraft Cubes[A]", ()=> ClearMCCubes("Destroy Minecraft Cubes[A]"), true, false, "Destroys Minecraft Cubes If You Press A"),
                    AddButton("Destroy Minecraft Cubes Gun[RG][RT]", ()=> DestroyMCBlockGun(), true, false, "Right Grip And Trigger To Destroy Minecraft Cube"),
                    AddButton("Remove Minecraft Block Collider Gun[RG][RT]", ()=> RemoveMCBlockCollider(), true, false, "Right Grip And Trigger To Remove Minecraft Block Collider"),
                    AddButton("Add Minecraft Block Collider[RG][RT]", ()=> AddMCBlockCollider(), true, false, "Right Grip And Trigger To Add Minecraft Block Collider"),

                    AddButton("Mincraft Cube Texture: Grass", ()=> ChangeMCTexture("Changed Minecraft Block Texture"), false, false, "Changed Minecraft Block Texture",  ()=> EnterPage(0, 3)),

                    AddButton("Minecraft Song: Living Mice", ()=> ChangeMCSong("Changed Minecraft Song"), false, false, "Changed Minecraft Song", ()=> EnterPage(2, 3)),

                    AddButton("Play Song", ()=> { if (fastLoad) { GetIndex("Play Song").enabled = false; SendNotification(NotifUtils.Warning() + "Config 'Fast Load' is enabled this mod is now unuseable"); } else PlayMinecraftSong(false); }, false, false, "Playing Current Set Minecraft Song"),
                    AddButton("Stop Song", ()=> PlayMinecraftSong(true), false, false, "Stopped Current Set Minecraft Song"),
                    AddButton("Raise Volume", ()=> Loaders.MCObject.volume += .1f, false, false, "Raised Minecraft Music Volume"),
                    AddButton("Lower Volume", ()=> Loaders.MCObject.volume -= .1f, false, false, "Lowered Minecraft Music Volume"),

                    AddButton("Save Current Minecraft Build", ()=> SaveMinecraftCubes(), false, false, "Saved Save File To MysticClient/Saving/LastMinecraftCubes.json"),
                    AddButton("Loaded Last Minecraft Build Save", ()=> LoadMinecraftCubes(), false, false, "Loaded Save File From MysticClient/Saving/LastMinecraftCubes.json"),

                    //AddButton("Loop Audio", ()=> Loaders.MCObjectLoop = true, ()=> Loaders.MCObjectLoop = false, true, false, "Makes The Minecraft Music Loop"),
                },
            },


            new ButtonInfo[][] // other settings 3
            {
                new ButtonInfo[] // minecraft blocks 0
                {
                    AddButton("Exit MC Texture Settings", ()=> EnterPage(4, 2), false, false, "Closed MC Texture Settings"),
                    AddButton("Make Texture Grass", ()=> {mcBlockTexture = MCTextures[0]; Settings.Mode[26] = 0; }, false, false, "Guess What Just Happened"),
                    AddButton("Make Texture Dirt", ()=> {mcBlockTexture = MCTextures[1]; Settings.Mode[26] = 1; }, false, false, "Guess What Just Happened"),
                    AddButton("Make Texture Wood", ()=> {mcBlockTexture = MCTextures[2]; Settings.Mode[26] = 2; }, false, false, "Guess What Just Happened"),
                    AddButton("Make Texture Leaf", ()=> {mcBlockTexture = MCTextures[3]; Settings.Mode[26] = 3; }, false, false, "Guess What Just Happened"),
                    AddButton("Make Texture Plank", ()=> {mcBlockTexture = MCTextures[4]; Settings.Mode[26] = 4; }, false, false, "Guess What Just Happened"),
                    AddButton("Make Texture Stone", ()=> {mcBlockTexture = MCTextures[5]; Settings.Mode[26] = 5; }, false, false, "Guess What Just Happened"),
                    AddButton("Make Texture Cobblestone", ()=> {mcBlockTexture = MCTextures[6]; Settings.Mode[26] = 6; }, false, false, "Guess What Just Happened"),
                    AddButton("Make Texture HayBale", ()=> {mcBlockTexture = MCTextures[7]; Settings.Mode[26] = 7; }, false, false, "Guess What Just Happened"),
                    AddButton("Make Texture Glass", ()=> {mcBlockTexture = MCTextures[8]; Settings.Mode[26] = 8; }, false, false, "Guess What Just Happened"),
                    AddButton("Make Texture Obsidian", ()=> {mcBlockTexture = MCTextures[9]; Settings.Mode[26] = 9; }, false, false, "Guess What Just Happened"),
                    AddButton("Make Texture Water", ()=> {mcBlockTexture = MCTextures[10]; Settings.Mode[26] = 10; }, false, false, "Guess What Just Happened"),
                    AddButton("Make Texture TrapDoor", ()=> {mcBlockTexture = MCTextures[11]; Settings.Mode[26] = 11; }, false, false, "Guess What Just Happened"),
                },

                new ButtonInfo[] // minecraft pickaxe settings 1
                {
                    AddButton("Exit Pickaxe Settings", ()=> EnterPage(4, 2), false, false, "Closed Pickaxe Settings"),
                    AddButton("Max Strike Count: 4", ()=> ChangeMinecraftPickaxeMaxStrike("Changed Max Pickaxe Strike Count"), false, false, "Changed Max Pickaxe Strike Count"),
                    AddButton("No Hit Delay", false, "Removes The Hit Delay On The Pickaxe"),
                },

                new ButtonInfo[] // minecraft songs 2
                {
                    AddButton("Exit MC Song Settings", ()=> EnterPage(4, 2), false, false, "Closed MC Song Settings"),
                    AddButton("Make Song Living Mice", ()=> {mcSongClip = mcSongClip = AudioClips[9]; Settings.Mode[27] = 9; }, false, false, "Guess What Just Happened"),
                    AddButton("Make Song Clark", ()=> {mcSongClip = mcSongClip = AudioClips[10]; Settings.Mode[27] = 10; }, false, false, "Guess What Just Happened"),
                    AddButton("Make Song Danny", ()=> {mcSongClip = mcSongClip = AudioClips[11]; Settings.Mode[27] = 11; }, false, false, "Guess What Just Happened"),
                    AddButton("Make Song Oxygene", ()=> {mcSongClip = mcSongClip = AudioClips[12]; Settings.Mode[27] = 12; }, false, false, "Guess What Just Happened"),
                    AddButton("Make Song Key", ()=> {mcSongClip = mcSongClip = AudioClips[13]; Settings.Mode[27] = 13; }, false, false, "Guess What Just Happened"),
                    AddButton("Make Song Droopy Likes Your Face", ()=> {mcSongClip = mcSongClip = AudioClips[14]; Settings.Mode[27] = 14; }, false, false, "Guess What Just Happened"),
                    AddButton("Make Song Moog City", ()=> {mcSongClip = mcSongClip = AudioClips[15]; Settings.Mode[27] = 15; }, false, false, "Guess What Just Happened"),
                    AddButton("Make Song Moog City 2", ()=> {mcSongClip = mcSongClip = AudioClips[16]; Settings.Mode[27] = 16; }, false, false, "Guess What Just Happened"),
                    AddButton("Make Song Subwoofer Lullaby", ()=> {mcSongClip = mcSongClip = AudioClips[17]; Settings.Mode[27] = 17; }, false, false, "Guess What Just Happened"),
                    AddButton("Make Song Dog", ()=> {mcSongClip = mcSongClip = AudioClips[18]; Settings.Mode[27] = 18; }, false, false, "Guess What Just Happened"),
                    AddButton("Make Song Cat", ()=> {mcSongClip = mcSongClip = AudioClips[19]; Settings.Mode[27] = 19; }, false, false, "Guess What Just Happened"),
                    AddButton("Make Song Aria Math", ()=> {mcSongClip = mcSongClip = AudioClips[20]; Settings.Mode[27] = 20; }, false, false, "Guess What Just Happened"),
                    AddButton("Make Song Haggstorm", ()=> {mcSongClip = mcSongClip = AudioClips[21]; Settings.Mode[27] = 21; }, false, false, "Guess What Just Happened"),
                    AddButton("Make Song Pigstep", ()=> {mcSongClip = mcSongClip = AudioClips[22]; Settings.Mode[27] = 22; }, false, false, "Guess What Just Happened"),
                    AddButton("Make Song Pigstep (Alan Becker)", ()=> {mcSongClip = mcSongClip = AudioClips[23]; Settings.Mode[27] = 23; }, false, false, "Guess What Just Happened"),
                },

                new ButtonInfo[] // minecraft mod setting :3
                {
                    AddButton("Exit MC Mod Settings", ()=> EnterPage(4, 2), false, false, "Closed MC Mod Settings"),
                    AddButton("Left Hand", false, "Puts Minecraft Mod On Your Left Hand"),
                },

                new ButtonInfo[] // menu trail settings 4
                {
                    AddButton("Exit Menu Trail Settings", ()=> EnterPage(0, 1), false, false, "Closed Menu Trail Settings"),
                    AddButton("Menu Trail Color: Black", ()=> ChangeMenuTrailColor("Changed Menu Trail Color"), false, false, "Changed Menu Trail Color"),
                    AddButton("Make Menu Trail Color Follow Menu Color", false, "Makes The Menu Trail Color Follow The Menu Color"),
                },

                new ButtonInfo[] // array list setting 5
                {
                    AddButton("Exit Array List Settings", ()=> EnterPage(0, 1), false, false, "Closed Array List Settings"),
                    AddButton("Array List Buttons", false, "Adds A Disable Button Next To The Array List Text"),
                    AddButton("Small Array List", false, "Makes The Array List Small"),
                },

                new ButtonInfo[] // iron monke settings 6
                {
                    AddButton("Exit IronMonke Settings", ()=> EnterPage(0, 3), false, false, "Closed IronMonke Settings"),
                    AddButton("Fire Trails", false, "Makes IronMonke Have Trails When You Fly"),
                    AddButton("Fire Particles", false, "Makes IronMonke Have Particles When You Fly"),
                },
            },



            // voice commands
            AddButton("Next Page", ()=> Toggle("NextPage"), false, "Switched Page"),
            AddButton("Last Page", ()=> Toggle("PreviousPage"), false, "Switched Page"),
            AddButton("Destroy Networked Objects", ()=> Toggle("DestroyButton"), false, "Destroyed Networked Objects"),
            AddButton("Take A Screenshot", ()=> { SteamScreenshots.TriggerScreenshot(); SendNotification(NotifUtils.Menu() + "Saved Screenshot Using Steam"); }, false, "Took A Screenshot Using Steam"),
            AddButton("Search", ()=> Toggle("Search"), false, "Started Search"),
            AddButton("Stop Search", ()=> inKeyboard = false, false, "Stopped Search"),
            // voice commands
        };








        public static ButtonInfo AddButton(string text, bool enabled = false, string toolTip = "PlaceHolder")
        {
            return new ButtonInfo
            {
                buttonText = text,
                method = null,
                disableMethod = null,
                isTogglable = true,
                enabled = enabled,
                toolTip = toolTip,
                settingAction = null
            };
        }
        public static ButtonInfo AddButton(string text, bool isTogglable = true, bool enabled = false, string toolTip = "PlaceHolder")
        {
            return new ButtonInfo
            {
                buttonText = text,
                method = null,
                disableMethod = null,
                isTogglable = isTogglable,
                enabled = enabled,
                toolTip = toolTip,
                settingAction = null
            };
        }
        public static ButtonInfo AddButton(string text, Action method, bool isToggleable = true, bool enabled = false, string toolTip = "PlaceHolder")
        {
            return new ButtonInfo
            {
                buttonText = text,
                method = method,
                disableMethod = null,
                isTogglable = isToggleable,
                enabled = enabled,
                toolTip = toolTip,
                settingAction = null,
            };
        }
        public static ButtonInfo AddButton(string text, Action method, Action disableMethod, bool isToggleable = true, bool enabled = false, string toolTip = "PlaceHolder")
        {
            return new ButtonInfo
            {
                buttonText = text,
                method = method,
                disableMethod = disableMethod,
                isTogglable = isToggleable,
                enabled = enabled,
                toolTip = toolTip,
                settingAction = null
            };
        }
        public static ButtonInfo[][] AddButton(string text, Action method, bool isToggleable = true, string toolTip = "PlaceHolder") // this if for voice commands
        {
            return new ButtonInfo[][]
            {
                new ButtonInfo[]
                {
                    new ButtonInfo
                    {
                        buttonText = text,
                        method = method,
                        disableMethod = null,
                        isTogglable = isToggleable,
                        enabled = false,
                        toolTip = toolTip,
                        settingAction = null
                    }
                }
            };
        }

        public static ButtonInfo AddButton(string text, Action method, bool isToggleable = true, bool enabled = false, string toolTip = "PlaceHolder", Action settingAction = null)
        {
            return new ButtonInfo
            {
                buttonText = text,
                method = method,
                disableMethod = null,
                isTogglable = isToggleable,
                enabled = enabled,
                toolTip = toolTip,
                settingAction = settingAction
            };
        }
        public static ButtonInfo AddButton(string text, Action method, Action disableMethod, bool isToggleable = true, bool enabled = false, string toolTip = "PlaceHolder", Action settingAction = null)
        {
            return new ButtonInfo
            {
                buttonText = text,
                method = method,
                disableMethod = disableMethod,
                isTogglable = isToggleable,
                enabled = enabled,
                toolTip = toolTip,
                settingAction = settingAction
            };
        }
        public static ButtonInfo AddButton(string text, bool enabled = false, string toolTip = "PlaceHolder", Action settingAction = null)
        {
            return new ButtonInfo
            {
                buttonText = text,
                method = null,
                disableMethod = null,
                isTogglable = true,
                enabled = enabled,
                toolTip = toolTip,
                settingAction = settingAction
            };
        }

        public static int[] crystalIndex = new int[]
        {
            UnityEngine.Random.Range(40, 54),
            UnityEngine.Random.Range(214, 221)
        };
        public static void QuitGame()
        {
            Disconnect();
            Quit();
        }


        public static void CreateRoom(string roomName)
        {
            var config = new RoomConfig
            {
                createIfMissing = true,
                isJoinable = true,
                isPublic = true,
                MaxPlayers = NetworkController.GetRoomSize(NetworkController.currentJoinTrigger.networkZone),
                CustomProps = new Hashtable 
                {
                    { "gameMode", NetworkController.currentJoinTrigger.GetFullDesiredGameModeString() },
                    { "platform", (string)typeof(PhotonNetworkController).GetField("platformTag", NonPublicInstance).GetValue(NetworkController) },
                    { "queueName", GorillaComputer.instance.currentQueue }
                }
            };
            PhotonSystem.ConnectToRoom(roomName, config);
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
