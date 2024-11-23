using ExitGames.Client.Photon;
using MysticClient.Menu;
using MysticClient.Mods;
using MysticClient.Utils;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using UnityEngine;
using static MysticClient.Mods.Movement;

namespace MysticClient.Classes
{
    public class Networking : MonoBehaviour
    {
        public static bool NetworkColliders;
        private static bool[] networked = new bool[9999];
        private static void AddNetwork(Action<EventData> function)
        {
            PhotonNetwork.NetworkingClient.EventReceived += function;
        }
        private static void RemoveNetwork(Action<EventData> function)
        {
            PhotonNetwork.NetworkingClient.EventReceived -= function;
        }
        public static void NetworkPlatforms()
        {
            if (!networked[0])
            {
                AddNetwork(PlatformNetwork);
                networked[0] = true;
                networked[1] = false;
            }
        }
        public static void UnNetworkPlatforms()
        {
            if (!networked[1])
            {
                RemoveNetwork(PlatformNetwork);
                networked[1] = true;
                networked[0] = false;
            }
        }
        public static void NetworkPlatGun()
        {
            if (!networked[2])
            {
                AddNetwork(PlatformGunNetwork);
                networked[2] = true;
                networked[3] = false;
            }
        }
        public static void UnNetworkPlatGun()
        {
            if (!networked[3])
            {
                RemoveNetwork(PlatformGunNetwork);
                networked[3] = true;
                networked[2] = false;
            }
        }
        public static void NetworkBalls()
        {
            if (!networked[4])
            {
                AddNetwork(BallGunNetwork);
                networked[4] = true;
                networked[5] = false;
            }
        }
        public static void UnNetworkBalls()
        {
            if (!networked[4])
            {
                RemoveNetwork(BallGunNetwork);
                networked[4] = true;
                networked[5] = false;
            }
        }
        public static void NetworkFrozone()
        {
            if (!networked[6])
            {
                AddNetwork(FrozoneNetwork);
                networked[6] = true;
                networked[7] = false;
            }
        }
        public static void UnNetworkFrozone()
        {
            if (!networked[7])
            {
                RemoveNetwork(FrozoneNetwork);
                networked[7] = true;
                networked[6] = false;
            }
        }
        public static void NetworkProjectiles()
        {
            if (!networked[8])
            {
                AddNetwork(ProjectileNetwork);
                networked[8] = true;
                networked[9] = false;
            }
        }
        public static void UnNetworkProjectiles()
        {
            if (!networked[9])
            {
                RemoveNetwork(ProjectileNetwork);
                networked[9] = true;
                networked[8] = false;
            }
        }
        public static void DestroyNetworkedObjects()
        {
            string[] names = { "PlatNetwork", "PlatGunNetwork", "BALLSNetwork" };
            var objects = Main.GetObjectByNames(names);
            if (objects.activeSelf)
                objects.SetActive(false);
        }
        /*public static void RaiseNetwork(string code, object content, RaiseEventOptions reo, bool reliable)
        {
            if (Main.PhotonSystem.InRoom)
            {
                object[] data = new object[] { code, content };
                Main.LegacySendEvent(126, data, reo, reliable);
            }
        }*/
        private static void PlatformNetwork(EventData eventData)
        {
            byte code = eventData.Code;
            if (code == 69)
            {
                object[] array = (object[])eventData.CustomData;
                jump_left_network[eventData.Sender] = GameObject.CreatePrimitive(PrimitiveType.Cube);
                jump_left_network[eventData.Sender].GetComponent<Collider>().enabled = NetworkColliders;
                jump_left_network[eventData.Sender].name = "PlatNetwork";
                jump_left_network[eventData.Sender].transform.localScale = (Vector3)array[2];
                jump_left_network[eventData.Sender].transform.position = (Vector3)array[0];
                jump_left_network[eventData.Sender].transform.rotation = (Quaternion)array[1];
                if (!(bool)array[3])
                {
                    var colorChanger = jump_left_network[eventData.Sender].AddComponent<ColorChanger>();
                    colorChanger.colors = new Gradient
                    {
                        colorKeys = (GradientColorKey[])array[4],
                    };
                    colorChanger.loop = true;
                } else jump_left_network[eventData.Sender].GetComponent<Renderer>().material.color = (Color)array[5];
            }
            if (code == 70)
            {
                object[] array = (object[])eventData.CustomData;
                jump_right_network[eventData.Sender] = GameObject.CreatePrimitive(PrimitiveType.Cube);
                jump_right_network[eventData.Sender].GetComponent<Collider>().enabled = NetworkColliders;
                jump_right_network[eventData.Sender].name = "PlatNetwork";
                jump_right_network[eventData.Sender].transform.localScale = (Vector3)array[2];
                jump_right_network[eventData.Sender].transform.position = (Vector3)array[0];
                jump_right_network[eventData.Sender].transform.rotation = (Quaternion)array[1];
                if (!(bool)array[3])
                {
                    var colorChanger = jump_right_network[eventData.Sender].AddComponent<ColorChanger>();
                    colorChanger.colors = new Gradient
                    {
                        colorKeys = (GradientColorKey[])array[4],
                    };
                    colorChanger.loop = true;
                } else jump_right_network[eventData.Sender].GetComponent<Renderer>().material.color = (Color)array[5];
            }
            if (code == 71)
            {
                object[] array = (object[])eventData.CustomData;
                if ((bool)array[1])
                {
                    var comp = jump_left_network[eventData.Sender].AddComponent(typeof(Rigidbody)) as Rigidbody;
                    comp.velocity = (Vector3)array[0];
                }
                else
                {
                    Destroy(jump_left_network[eventData.Sender]);
                    jump_left_network[eventData.Sender] = null;
                }
            }
            if (code == 72)
            {
                object[] array = (object[])eventData.CustomData;
                if ((bool)array[1])
                {
                    var comp = jump_right_network[eventData.Sender].AddComponent(typeof(Rigidbody)) as Rigidbody;
                    comp.velocity = (Vector3)array[0];
                    return;
                }
                else
                {
                    Destroy(jump_right_network[eventData.Sender]);
                    jump_right_network[eventData.Sender] = null;
                }
            }
        }
        private static void ProjectileNetwork(EventData eventData)
        {
            byte code = eventData.Code;
            if (code == 111)
            {
                ProjectileLib.LaunchProjectile((ProjectileLib.ProjectileData)eventData.CustomData);
            }
        }
        private static void PlatformGunNetwork(EventData eventData)
        {
            byte code = eventData.Code;
            if (code == 110)
            {
                object[] array = (object[])eventData.CustomData;
                platgun_network[eventData.Sender] = GameObject.CreatePrimitive(PrimitiveType.Cube);
                platgun_network[eventData.Sender].GetComponent<Collider>().enabled = NetworkColliders;
                platgun_network[eventData.Sender].layer = 8;
                platgun_network[eventData.Sender].name = "PlatGunNetwork";
                platgun_network[eventData.Sender].transform.localScale = scale;
                platgun_network[eventData.Sender].transform.position = (Vector3)array[0];
                platgun_network[eventData.Sender].transform.rotation = (Quaternion)array[1];
                if (!(bool)array[4])
                {
                    var colorChanger = platgun_network[eventData.Sender].AddComponent<ColorChanger>();
                    colorChanger.colors = new Gradient
                    {
                        colorKeys = (GradientColorKey[])array[5],
                    };
                    colorChanger.loop = true;
                } else platgun_network[eventData.Sender].GetComponent<Renderer>().material.color = (Color)array[6];
                if ((bool)array[3])
                {
                    var platRB = platgun_network[eventData.Sender].AddComponent(typeof(Rigidbody)) as Rigidbody;
                    platRB.velocity = (Vector3)array[2];
                }
            }
        }
        private static void BallGunNetwork(EventData eventData)
        {
            byte code = eventData.Code;
            if (code == 100)
            {
                object[] array = (object[])eventData.CustomData;
                ball_network[eventData.Sender] = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                ball_network[eventData.Sender].layer = 8;
                ball_network[eventData.Sender].name = "BALLSNetwork";
                ball_network[eventData.Sender].transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                ball_network[eventData.Sender].transform.position = (Vector3)array[0];
                ball_network[eventData.Sender].transform.rotation = (Quaternion)array[1];
                ball_network[eventData.Sender].GetComponent<Renderer>().material.color = (Color)array[3];
                var trail = ball_network[eventData.Sender].AddComponent<TrailRenderer>();
                trail.material = new Material(Shader.Find("Sprites/Default"));
                trail.time = 1;
                trail.startWidth = .1f;
                trail.endWidth = 0;
                trail.minVertexDistance = 1;
                trail.material.color = (Color)array[3];
                var ballRB = ball_network[eventData.Sender].AddComponent(typeof(Rigidbody)) as Rigidbody;
                ballRB.velocity = (Vector3)array[2];
            }
        }
        private static void FrozoneNetwork(EventData eventData)
        {
            byte code = eventData.Code;
            if (code == 90)
            {
                object[] array = (object[])eventData.CustomData;
                frozone_right_network[eventData.Sender] = GameObject.CreatePrimitive(PrimitiveType.Cube);
                frozone_right_network[eventData.Sender].transform.localScale = scale;
                frozone_right_network[eventData.Sender].GetComponent<Collider>().enabled = NetworkColliders;
                frozone_right_network[eventData.Sender].transform.position = (Vector3)array[0];
                frozone_right_network[eventData.Sender].transform.rotation = (Quaternion)array[1];
                if (!(bool)array[2])
                {
                    var colorChanger = frozone_right_network[eventData.Sender].AddComponent<ColorChanger>();
                    colorChanger.colors = new Gradient
                    {
                        colorKeys = (GradientColorKey[])array[3],
                    };
                    colorChanger.loop = true;
                } else frozone_right_network[eventData.Sender].GetComponent<Renderer>().material.color = (Color)array[4];
                Destroy(frozone_right_network[eventData.Sender], 1);
            }
            if (code == 91)
            {
                object[] array = (object[])eventData.CustomData;
                frozone_left_network[eventData.Sender] = GameObject.CreatePrimitive(PrimitiveType.Cube);
                frozone_left_network[eventData.Sender].transform.localScale = scale;
                frozone_left_network[eventData.Sender].GetComponent<Collider>().enabled = NetworkColliders;
                frozone_left_network[eventData.Sender].transform.position = (Vector3)array[0];
                frozone_left_network[eventData.Sender].transform.rotation = (Quaternion)array[1];
                if ((bool)array[2])
                {
                    var colorChanger = frozone_left_network[eventData.Sender].AddComponent<ColorChanger>();
                    colorChanger.colors = new Gradient
                    {
                        colorKeys = (GradientColorKey[])array[3],
                    };
                    colorChanger.loop = true;
                } else frozone_left_network[eventData.Sender].GetComponent<Renderer>().material.color = (Color)array[4];
                Destroy(frozone_left_network[eventData.Sender], 1);
            }
        }
    }
}