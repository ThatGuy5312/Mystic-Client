using MysticClient.Classes;
using Photon.Pun;
using static MysticClient.Menu.Main;
using UnityEngine;
using MysticClient.Menu;
using System.Reflection;
using Viveport;
using MysticClient.Mods;
using System.Runtime.CompilerServices;
using Unity.XR.CoreUtils;

namespace MysticClient.Utils
{
    public class ProjectileLib : GunLib
    {
        public static bool rainbowColor;
        public static bool funnyRGB;
        public static float projSize = 1;
        private static void LaunchProjectile(object[] args)
        {
            var projectile = (SlingshotProjectile)GetProjectile((int)args[0]);
            if ((int)args[1] != -1)
            {
                var trail = (SlingshotProjectileTrail)GetProjectile((int)args[1]);
                trail.AttachTrail(projectile.gameObject, false, false);
            }
            int counter = 0;
            projectile.Launch((Vector3)args[2], (Vector3)args[3], RigUtils.MyNetPlayer, false, false, counter++, (float)args[5], true, (Color)args[4]);
        }
        public static void LaunchProjectile(ProjectileData projectileData)
        {
            LaunchProjectile(new object[]
            {
                projectileData.projectile, // 0 proj hash
                projectileData.trail, // 1 trail hash
                projectileData.position, // 2 spawn pos
                projectileData.velocity, // 3 velo
                projectileData.color, // 4 color
                projectileData.size // 5 size
            });
        }
        public class ProjectileData
        {
            public ProjectileData(int projectile, int trail, Vector3 position, Vector3 velocity, Color color, float size)
            {
                this.projectile = projectile;
                this.trail = trail;
                this.position = position;
                this.velocity = velocity;
                this.color = color;
                this.size = size;
            }
            public ProjectileData() { }
            public int projectile = -675036877;
            public int trail = 1432124712;
            public Vector3 position;
            public Vector3 velocity;
            public Color color;
            public float size;
        }
        public class ServerSided
        {
            private static GorillaVelocityEstimator VelocityEstimator;
            private static float projDelay;

            public static void LaunchProjectile(ProjData data)
            {
                VelocityEstimator ??= new GameObject("GVE").AddComponent<GorillaVelocityEstimator>();
                SnowballMaker.leftHandInstance.TryCreateSnowball(data.makerId, out var projectile);
                if (Time.time > projDelay)
                {
                    projectile.transform.position = data.position;
                    projectile.randomizeColor = true;
                    var velo = RigUtils.MyVelocity;
                    RigUtils.MyOfflineRig.SetThrowableProjectileColor(true, data.color);
                    projectile.velocityEstimator = VelocityEstimator;
                    typeof(SnowballThrowable).GetMethod("PerformSnowballThrowAuthority", NonPublicInstance).Invoke(projectile, null);
                    RigUtils.MyVelocity = velo;
                    projDelay = Time.time + .14f;
                }
            }

            public class ProjData
            {
                public Vector3 position;
                public Vector3 velocity;
                public string name;
                public string id;
                public int makerId;
                public Color color;
            }
        }
        public class Other
        {
            private static float projDelay;
            private static float projDelayType;
            public static void LaunchProjectile(object[] args)
            {
                var ppt = (PaperPlaneThrowable)GetProjectile((int)args[0]);
                if (Time.time > projDelay)
                {
                    projDelay = Time.time + projDelayType;
                    var pos = (Vector3)args[1];
                    var rot = (Quaternion)args[2];
                    var vel = (Vector3)args[3];
                    typeof(PaperPlaneThrowable).GetMethod("LaunchProjectile", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(ppt, new object[] { pos, rot, vel });
                    int eventid = StaticHash.Compute("PaperPlaneThrowable", "LaunchProjectile");
                    var LRPC = typeof(PaperPlaneThrowable).GetField("gLaunchRPC", BindingFlags.NonPublic | BindingFlags.Static);
                    var PE = (PhotonEvent)LRPC.GetValue(ppt);
                    object[] objs = { eventid, pos, rot, vel };
                    PE.RaiseAll(objs);
                }
            }
        }
        public class Elf
        {
            public static void BetaLaunchElf(object[] args)
            {
                var _events = (RubberDuckEvents)typeof(ElfLauncher).GetField("_events", NonPublicInstance).GetValue(null);
                _events.Activate.RaiseAll(new object[]
                {
                    (Vector3)args[0], // position
                    (Vector3)args[1] // velocity
                });
            }
        }
    }
}