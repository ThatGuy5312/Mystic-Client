using UnityEngine;
using MysticClient.Utils;
using static MysticClient.Menu.Main;
using Photon.Realtime;
using OculusSampleFramework;
using MysticClient.Classes;

namespace MysticClient.Mods
{
    public class Projectiles : ProjectileLib
    {
        public static int[] Mode = new int[9999];
        public static int[] projSpeeds = { 1, 0, 10, 25, 50, 100 };
        public static string[] speedNames = { "Normal", "Slow", "Moderate", "Fast", "Very Fast", "Super Fast" };
        public static string[] sizeNames = { "Normal", ".2x", ".5x", "2x", "3x", "4x", "5x" };
        public static float[] sizes = { 1, .2f, .5f, 2, 3, 4, 5 };
        public static string[] HandTypeName = { "Right Hand", "Left Hand", "Both Hands" };
        public static int[] projHash = { -675036877, -1674517839, -1671677000, -820530352, 693334698, 1511318966, 825718363, 1705139863, -622368518, -1280105888, -790645151, -666337545, -160604350, -1433633837, 2061412059, -1433634409, -716425086, -1509512060, 1077936051, -1444634975, -364693752, -829423852, -1140939310, -2146558070, -235083827, 1918888813 };
        public static int[] trailHash = { -1, 1432124712, -1277271056, 163790326, 16948542, 1848916225, -67783235, -748577108, -1232128945, -393062454, 1844846002, -748577108 };
        public static string[] projNames = { "Snowball", "Balloon", "Ice", "Slingshot", "Deadshot", "Cloud", "Cupid", "Elf", "Rock", "Pepper", "Spider", "Square Gift", "Round Gift", "Roll Gift", "Candy Cane", "Coal", "Mentos", "Fish Food", "Candy Corn", "Bat Swarm", "Cartoon Bomb", "Eyeball", "Devil Bow", "Yumi Bow", "Voting Rock", "Apple" };
        public static string[] trailNames = { "None", "Slingshot", "Ice", "Deadshot", "Cloud", "Cupid", "Elf", "Pepper", "Spider", "Devil", "Samurai", "Rock" };
        public static int projectileSpeed = 1;
        public static int projectile = -675036877;
        public static int trail = -1;
        public static Color projectileColor = Color.white;

        public static void ChangeProjectileScale(string tooltip)
        {
            Mode[5]++;
            if (Mode[5] >= sizes.Length) { Mode[5] = 0; }
            projSize = sizes[Mode[5]];
            string[] text = GetToolTip(tooltip).buttonText.Split(':');
            GetToolTip(tooltip).buttonText = text[0] + ": " + sizeNames[Mode[5]];
            GetToolTip(tooltip).enabled = false;
        }
        public static void Cycle()
        {
            ChangeProjectile("Changed Projectile");
            ChangeTrail("Changed Projectile Trail");
        }
        public static void ChangeSpeed(string tooltip)
        {
            Mode[4]++;
            if (Mode[4] >= projSpeeds.Length){ Mode[4] = 0; }
            projectileSpeed = projSpeeds[Mode[4]];
            string[] text = GetToolTip(tooltip).buttonText.Split(':');
            GetToolTip(tooltip).buttonText = text[0] + ": " + speedNames[Mode[4]];
            GetToolTip(tooltip).enabled = false;
        }
        public static void ChangeColor(string tooltip)
        {
            Mode[3]++;
            var len = GetEnabled("Use System Colors") ? Settings.scolor.Length : Settings.colors.Length;
            if (Mode[3] >= len) { Mode[3] = 0; }
            var name = GetEnabled("Use System Colors") ? Settings.systemColorNames[Mode[3]] : Settings.Names[0][Mode[3]];
            projectileColor = GetEnabled("Use System Colors") ? SCToUC(Settings.scolor[Mode[3]]) : Settings.colors[Mode[3]];
            string[] text = GetToolTip(tooltip).buttonText.Split(':');
            GetToolTip(tooltip).buttonText = text[0] + ": " + name;
            GetToolTip(tooltip).enabled = false;
        }
        public static void ChangeTrail(string tooltip)
        {
            Mode[2]++;
            if (Mode[2] >= trailHash.Length) { Mode[2] = 0; }
            trail = trailHash[Mode[2]];
            string[] text = GetToolTip(tooltip).buttonText.Split(':');
            GetToolTip(tooltip).buttonText = text[0] + ": " + trailNames[Mode[2]];
            GetToolTip(tooltip).enabled = false;
        }
        public static void ChangeProjectile(string tooltip)
        {
            Mode[1]++;
            if (Mode[1] >= projHash.Length) { Mode[1] = 0; }
            projectile = projHash[Mode[1]];
            string[] text = GetToolTip(tooltip).buttonText.Split(':');
            GetToolTip(tooltip).buttonText = text[0] + ": " + projNames[Mode[1]];
            GetToolTip(tooltip).enabled = false;
        }
        public static void ChangeHandType(string tooltip)
        {
            Mode[0]++;
            if (Mode[0] >= 3) { Mode[0] = 0; }
            string[] text = GetToolTip(tooltip).buttonText.Split(':');
            GetToolTip(tooltip).buttonText = text[0] + ": " + HandTypeName[Mode[0]];
            GetToolTip(tooltip).enabled = false;
        }

        public static void RefreshProjSettings()
        {
            projectile = projHash[Mode[1]];
            trail = trailHash[Mode[2]];
            projectileColor = GetEnabled("Use System Colors") ? SCToUC(Settings.scolor[Mode[3]]) : Settings.colors[Mode[3]];
            projectileSpeed = projSpeeds[Mode[4]];
            projSize = sizes[Mode[5]];
        }

        public static void ProjectileSpammer()
        {
            if (GetEnabled("ServerSided Projectiles"))
            {
                if (Mode[0] == 0)
                {
                    if (Controller.rightGrab || UserInput.GetMouseButton(0))
                    {
                        ServerSided.FireProjectile(RightHand);
                        LegacySendEvent(111, RightHand, new RaiseEventOptions { Receivers = ReceiverGroup.Others }, true);
                    }
                }
                else if (Mode[0] == 1)
                {
                    if (Controller.leftGrab || UserInput.GetMouseButton(1))
                    {
                        ServerSided.FireProjectile(LeftHand);
                        LegacySendEvent(111, LeftHand, new RaiseEventOptions { Receivers = ReceiverGroup.Others }, true);
                    }
                }
                else if (Mode[0] == 2)
                {
                    if (Controller.rightGrab || UserInput.GetMouseButton(0))
                    {
                        ServerSided.FireProjectile(RightHand);
                        LegacySendEvent(111, RightHand, new RaiseEventOptions { Receivers = ReceiverGroup.Others }, true);
                    }
                    if (Controller.leftGrab || UserInput.GetMouseButton(1))
                    {
                        ServerSided.FireProjectile(LeftHand);
                        LegacySendEvent(111, LeftHand, new RaiseEventOptions { Receivers = ReceiverGroup.Others }, true);
                    }
                }
            }
            else
            {
                if (Mode[0] == 0)
                {
                    if (Controller.rightGrab || UserInput.GetMouseButton(0))
                    {
                        LaunchProjectile(RightHand);
                        LegacySendEvent(111, RightHand, new RaiseEventOptions { Receivers = ReceiverGroup.Others }, true);
                    }
                }
                else if (Mode[0] == 1)
                {
                    if (Controller.leftGrab || UserInput.GetMouseButton(1))
                    {
                        LaunchProjectile(LeftHand);
                        LegacySendEvent(111, LeftHand, new RaiseEventOptions { Receivers = ReceiverGroup.Others }, true);
                    }
                }
                else if (Mode[0] == 2)
                {
                    if (Controller.rightGrab || UserInput.GetMouseButton(0))
                    {
                        LaunchProjectile(RightHand);
                        LegacySendEvent(111, RightHand, new RaiseEventOptions { Receivers = ReceiverGroup.Others }, true);
                    }
                    if (Controller.leftGrab || UserInput.GetMouseButton(1))
                    {
                        LaunchProjectile(LeftHand);
                        LegacySendEvent(111, LeftHand, new RaiseEventOptions { Receivers = ReceiverGroup.Others }, true);
                    }
                }
            }
        }
        public static void Cum()
        {
            if (GetEnabled("ServerSided Projectiles"))
            {
                if (Controller.rightControllerIndexFloat > 0.5f || UserInput.GetMouseButton(0))
                {
                    var data = new ProjectileData(-675036877, trail, RigUtils.MyOfflineRig.transform.position - new Vector3(0f, 0.3f, 0f), RigUtils.MyOfflineRig.transform.forward.normalized * 5f, Color.white, projSize);
                    ServerSided.FireProjectile(data, true);
                    LegacySendEvent(111, data, new RaiseEventOptions { Receivers = ReceiverGroup.Others }, true);
                }
            }
            else
            {
                if (Controller.rightControllerIndexFloat > 0.5f || UserInput.GetMouseButton(0))
                {
                    var data = new ProjectileData(-820530352, trail, RigUtils.MyOfflineRig.transform.position - new Vector3(0f, 0.3f, 0f), RigUtils.MyOfflineRig.transform.forward.normalized * 5f, Color.white, projSize);
                    LaunchProjectile(data);
                    LegacySendEvent(111, data, new RaiseEventOptions { Receivers = ReceiverGroup.Others }, true);
                }
            }
        }
        public static void Piss()
        {
            if (GetEnabled("ServerSided Projectiles"))
            {
                if (Controller.rightControllerIndexFloat > 0.5f || UserInput.GetMouseButton(0))
                {
                    var data = new ProjectileData(-675036877, trail, RigUtils.MyOfflineRig.transform.position - new Vector3(0f, 0.3f, 0f), RigUtils.MyOfflineRig.transform.forward.normalized * 5f, Color.yellow, projSize);
                    ServerSided.FireProjectile(data, true);
                    LegacySendEvent(111, data, new RaiseEventOptions { Receivers = ReceiverGroup.Others }, true);
                }
            }
            else
            {
                if (Controller.rightControllerIndexFloat > 0.5f || UserInput.GetMouseButton(0))
                {
                    var data = new ProjectileData(-820530352, trail, RigUtils.MyOfflineRig.transform.position - new Vector3(0f, 0.3f, 0f), RigUtils.MyOfflineRig.transform.forward.normalized * 5f, Color.yellow, projSize);
                    LaunchProjectile(data);
                    LegacySendEvent(111, data, new RaiseEventOptions { Receivers = ReceiverGroup.Others }, true);
                }
            }
        }
        public static void Shit()
        {
            var data = new ProjectileData(-675036877, trail, RigUtils.MyOnlineRig.bodyCollider.transform.position, Vector3.zero, new Color(99f / 255f, 45f / 255f, 0f), projSize);
            if (GetEnabled("ServerSided Projectiles"))
            {
                if (Controller.rightControllerIndexFloat > 0.5f || UserInput.GetMouseButton(0))
                {
                    //var data = new ProjectileData(-675036877, trail, RigUtils.MyOnlineRig.bodyCollider.transform.position, Vector3.zero, new Color(99f/255f, 45f/255f, 0f));
                    ServerSided.FireProjectile(data, true);
                    LegacySendEvent(111, data, new RaiseEventOptions { Receivers = ReceiverGroup.Others }, true);
                }
            }
            else
            {
                if (Controller.rightControllerIndexFloat > 0.5f || UserInput.GetMouseButton(0))
                {
                    //var data = new ProjectileData(-675036877, trail, RigUtils.MyOnlineRig.bodyCollider.transform.position, Vector3.zero, new Color(99f/255f, 45f/255f, 0f));
                    LaunchProjectile(data);
                    LegacySendEvent(111, data, new RaiseEventOptions { Receivers = ReceiverGroup.Others }, true);
                }
            }
        }
        public static void ProjectileGun()
        {
            if (CreateGun())
            {
                var data = new ProjectileData(projectile, trail, pointer.transform.position, Vector3.zero, rainbowColor ? RGBColor() : funnyRGB ? HardColor(Random.Range(0, 10)) : projectileColor, projSize);
                if (GetEnabled("ServerSided Projectiles"))
                    ServerSided.FireProjectile(data);
                else
                    LaunchProjectile(data);
                LegacySendEvent(111, data, new RaiseEventOptions { Receivers = ReceiverGroup.Others }, true);
            }
        }
        public static void ProjectileHalo()
        {
            var rad = 1.2f;
            var count = 6;
            for (int i = 0; i < count; i++)
            {
                float angle = (i * 360f / count) + Time.time * 90f; // did this so if your using the mystic gui free cam this will still work
                var pos = RigUtils.MyOnlineRig.rigidbody.useGravity ? RigUtils.MyPlayer.transform.position : RigUtils.MyOfflineRig.transform.position + new Vector3(
                    Mathf.Cos(angle * Mathf.Deg2Rad) * rad, .7f, Mathf.Sin(angle * Mathf.Deg2Rad) * rad);
                var data = new ProjectileData(projectile, trail, pos, Vector3.zero, rainbowColor ? RGBColor() : funnyRGB ? HardColor(i) : projectileColor, projSize);
                if (GetEnabled("ServerSided Projectiles"))
                    ServerSided.FireProjectile(data);
                else
                    LaunchProjectile(data);
                LegacySendEvent(111, data, new RaiseEventOptions { Receivers = ReceiverGroup.Others }, true);
            }
            // anti lag stuff
            var obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            obj.transform.localScale = new Vector3(5, .5f, 5);
            obj.transform.position = RigUtils.MyOnlineRig.rigidbody.useGravity ? RigUtils.MyPlayer.transform.position : RigUtils.MyOfflineRig.transform.position - new Vector3(0, 2, 0);
            Destroy(obj.GetComponent<Renderer>());
            Destroy(obj, Time.deltaTime);
        }
        public static ProjectileData RightHand { get {
            return new ProjectileData(
                projectile,
                trail,
                RigUtils.MyPlayer.rightControllerTransform.position,
                Vector3.up + RigUtils.MyPlayer.rightControllerTransform.forward * projectileSpeed + RigUtils.MyPlayer.rightHandCenterVelocityTracker.GetAverageVelocity(true, 0f, false),
                rainbowColor ? RGBColor() : funnyRGB ? HardColor(Random.Range(0, 10)) : projectileColor, projSize);
            }
        }
        public static ProjectileData LeftHand { get {
            return new ProjectileData(
                projectile,
                trail,
                RigUtils.MyPlayer.leftControllerTransform.position,
                Vector3.up + RigUtils.MyPlayer.leftControllerTransform.forward * projectileSpeed + RigUtils.MyPlayer.leftHandCenterVelocityTracker.GetAverageVelocity(true, 0f, false),
                rainbowColor ? RGBColor() : funnyRGB ? HardColor(Random.Range(0, 10)) : projectileColor, projSize);
            }
        }
    }
}