using MysticClient.Classes;
using Photon.Pun;
using static MysticClient.Menu.Main;
using UnityEngine;
using MysticClient.Menu;
using System.Reflection;
using Viveport;
using MysticClient.Mods;

namespace MysticClient.Utils
{
    public class ProjectileLib : GunLib
    {
        public static bool rainbowColor;
        public static bool funnyRGB;
        public static int projSize = 1;
        private static void LaunchProjectile(object[] args)
        {
            var projectile = (SlingshotProjectile)GetProjectile((int)args[0]);
            if ((int)args[1] != -1)
            {
                var trail = (SlingshotProjectileTrail)GetProjectile((int)args[1]);
                trail.AttachTrail(projectile.gameObject, false, false);
            }
            var color = Color.white;
            if (rainbowColor && !(bool)args[5])
            {
                color = RGBColor();
            }
            else if (funnyRGB && !rainbowColor && !(bool)args[5])
            {
                color = HardColor(Random.Range(0, 10));
            }
            else if (!funnyRGB && !rainbowColor && (Color)args[4] == null)
            {
                color = Color.white;
            }
            else if ((Color)args[4] != null || (bool)args[5])
            {
                color = (Color)args[4];
            }
            int counter = 0;
            projectile.Launch((Vector3)args[2], (Vector3)args[3], RigUtils.MyNetPlayer, false, false, counter++, projSize, true, color);
        }
        public static void LaunchProjectile(ProjectileData projectileData, bool forceColor = false)
        {
            LaunchProjectile(new object[]
            {
                projectileData.projectile,
                projectileData.trail,
                projectileData.position,
                projectileData.velocity,
                projectileData.color,
                forceColor
            });
        }
        public class ProjectileData
        {
            public ProjectileData(int projectile, int trail, Vector3 position, Vector3 velocity, Color color)
            {
                this.projectile = projectile;
                this.trail = trail;
                this.position = position;
                this.velocity = velocity;
                this.color = color;
            }
            public ProjectileData() { }
            public int projectile = -675036877;
            public int trail = 1432124712;
            public Vector3 position;
            public Vector3 velocity;
            public Color color;
        }
        public class ServerSided
        {
            public static ServerSided Internal { get; private set; }
            private GorillaVelocityEstimator VelocityEstimator = null;
            private float projDelay;
            private float projDelayType;
            public static void FireProjectile(ProjectileData projectileData, bool delay = false)
            {
                Internal.LaunchProjectile(new object[]
                {
                    projectileData.projectile,
                    projectileData.trail,
                    projectileData.position,
                    projectileData.velocity,
                    projectileData.color,
                    delay
                });
            }
            public void LaunchProjectile(object[] args)
            {
                if (VelocityEstimator == null)
                {
                    var GVE = new GameObject("New GVE");
                    VelocityEstimator = GVE.AddComponent<GorillaVelocityEstimator>();
                }
                VelocityEstimator.enabled = false;
                var projectile = (SnowballThrowable)GetProjectile((int)args[0]);
                //SlingshotProjectile component = projectile.AddComponent<SlingshotProjectile>();
                if (!projectile.gameObject.activeSelf)
                {
                    projectile.EnableSnowballLocal(true);
                    projectile.velocityEstimator = VelocityEstimator;
                    projectile.transform.position = (Vector3)args[2];
                }
                if ((int)args[1] != -1)
                {
                    ObjectPools.instance.Instantiate((int)args[1]).GetComponent<SlingshotProjectileTrail>().AttachTrail(projectile.gameObject, false, false);
                }
                if (Time.time > projDelay)
                {
                    try
                    {
                        var position = (Vector3)args[2];
                        var velocity = (Vector3)args[3];
                        RigUtils.MyOfflineRig.SetThrowableProjectileColor(true, (Color)args[4]);
                        RigUtils.MyOfflineRig.SetThrowableProjectileColor(false, (Color)args[4]);
                        var method = typeof(SnowballThrowable).GetMethod("LaunchSnowball", BindingFlags.Instance | BindingFlags.NonPublic);
                        method.Invoke(projectile, new object[] { });
                    }
                    catch { }
                    if (projDelayType > 0f && !(bool)args[5])
                    {
                        projDelay = Time.time + projDelayType + 0.05f;
                    }
                }
            }
        }
        public class Other
        {
            public static Other Internal { get; private set; }
            private float projDelay;
            private float projDelayType;
            public static void BetaLaunchThrowable(object[] args)
            {
                Internal.LaunchProjectile(args);
            }
            public void LaunchProjectile(object[] args)
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
                    PE.RaiseOthers(objs);
                }
            }
        }
    }
}