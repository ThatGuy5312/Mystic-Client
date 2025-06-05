using ExitGames.Client.Photon;
using MysticClient.Menu;
using MysticClient.Mods;
using MysticClient.Utils;
using OculusSampleFramework;
using OVR.OpenVR;
using Photon.Pun;
using Photon.Realtime;
using PlayFab.GroupsModels;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static DevConsole.MessagePayload;
using static MysticClient.Mods.Movement;

namespace MysticClient.Classes
{
    public class Networking : MonoBehaviourPun
    {
        public static bool NetworkColliders;
        private static bool[] networked = new bool[9999];
        private static void AddNetwork(Action<EventData> function) => PhotonNetwork.NetworkingClient.EventReceived += function;
        private static void RemoveNetwork(Action<EventData> function) => PhotonNetwork.NetworkingClient.EventReceived -= function;

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
        public static void NetworkMinecraft()
        {
            if (!networked[10])
            {
                AddNetwork(MinecraftNetwork);
                networked[10] = true;
                networked[11] = false;
            }
        }
        public static void UnNetworkMinecraft()
        {
            if (!networked[11])
            {
                RemoveNetwork(MinecraftNetwork);
                networked[11] = true;
                networked[10] = false;
            }
        }
        public static void DestroyNetworkedObjects() // i know there is a way better way of doing this but this is the only one that works
        {
            foreach (var platsRight in jump_right_network) platsRight.Destroy();
            foreach (var platsLeft in jump_left_network) platsLeft.Destroy();
            foreach (var plats in platgun_network) plats.Destroy();
            foreach (var balls in ballsgun_network) balls.Destroy();
            foreach (var blocks in Fun.MinecraftCubes) { Fun.MinecraftCubes.Remove(blocks); blocks.Destroy(); }
        }
        /*public static void RaiseNetwork(string code, object content, RaiseEventOptions reo, bool reliable)
        {
            if (Main.PhotonSystem.InRoom)
            {
                object[] data = new object[] { code, content };
                Main.LegacySendEvent(126, data, reo, reliable);
            }
        }*/
        public static void PlatformNetwork(EventData eventData)
        {
            byte code = eventData.Code;
            if (code == 69)
            {
                object[] array = (object[])eventData.CustomData;
                jump_left_network[eventData.Sender] = GameObject.CreatePrimitive((bool)array[6] ? PrimitiveType.Sphere : PrimitiveType.Cube);
                if (NetworkColliders) // i know theres a better way but it doesnt work
                    jump_left_network[eventData.Sender].GetComponent<Collider>().enabled = true;
                else
                    jump_left_network[eventData.Sender].GetComponent<Collider>().enabled = false;
                jump_left_network[eventData.Sender].name = "PlatNetwork";
                jump_left_network[eventData.Sender].transform.localScale = (Vector3)array[2];
                jump_left_network[eventData.Sender].transform.position = (Vector3)array[0];
                jump_left_network[eventData.Sender].transform.rotation = (Quaternion)array[1];
                jump_left_network[eventData.Sender].GetComponent<Renderer>().enabled = !(bool)array[7];
                if (!(bool)array[3])
                {
                    var colorChanger = jump_left_network[eventData.Sender].AddComponent<ColorChanger>();
                    colorChanger.colors = new Gradient
                    {
                        colorKeys = (GradientColorKey[])array[4],
                    };
                    colorChanger.loop = true;
                } else jump_left_network[eventData.Sender].GetComponent<Renderer>().material.color = (Color)array[5];
                //if ((bool)array[8]) Main.RoundOtherObject(jump_left_network[eventData.Sender]);
            }
            if (code == 70)
            {
                object[] array = (object[])eventData.CustomData;
                jump_right_network[eventData.Sender] = GameObject.CreatePrimitive((bool)array[6] ? PrimitiveType.Sphere : PrimitiveType.Cube);
                if (NetworkColliders)
                    jump_right_network[eventData.Sender].GetComponent<Collider>().enabled = true;
                else
                    jump_right_network[eventData.Sender].GetComponent<Collider>().enabled = false;
                jump_right_network[eventData.Sender].name = "PlatNetwork";
                jump_right_network[eventData.Sender].transform.localScale = (Vector3)array[2];
                jump_right_network[eventData.Sender].transform.position = (Vector3)array[0];
                jump_right_network[eventData.Sender].transform.rotation = (Quaternion)array[1];
                jump_right_network[eventData.Sender].GetComponent<Renderer>().enabled = !(bool)array[7];
                if (!(bool)array[3])
                {
                    var colorChanger = jump_right_network[eventData.Sender].AddComponent<ColorChanger>();
                    colorChanger.colors = new Gradient
                    {
                        colorKeys = (GradientColorKey[])array[4],
                    };
                    colorChanger.loop = true;
                } else jump_right_network[eventData.Sender].GetComponent<Renderer>().material.color = (Color)array[5];
                //if ((bool)array[8]) Main.RoundOtherObject(jump_right_network[eventData.Sender]);
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
        public static void ProjectileNetwork(EventData eventData)
        {
            byte code = eventData.Code;
            if (code == 111)
                ProjectileLib.LaunchProjectile((ProjectileLib.ProjectileData)eventData.CustomData);
        }
        public static void PlatformGunNetwork(EventData eventData)
        {
            byte code = eventData.Code;
            if (code == 110)
            {
                object[] array = (object[])eventData.CustomData;
                platgun_network[eventData.Sender] = GameObject.CreatePrimitive(PrimitiveType.Cube);
                if (NetworkColliders)
                    platgun_network[eventData.Sender].GetComponent<Collider>().enabled = true;
                else
                    platgun_network[eventData.Sender].GetComponent<Collider>().enabled = false;
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
        public static void BallGunNetwork(EventData eventData)
        {
            byte code = eventData.Code;
            if (code == 100)
            {
                object[] array = (object[])eventData.CustomData;
                ballsgun_network[eventData.Sender] = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                ballsgun_network[eventData.Sender].layer = 8;
                ballsgun_network[eventData.Sender].name = "BALLSNetwork";
                ballsgun_network[eventData.Sender].transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                ballsgun_network[eventData.Sender].transform.position = (Vector3)array[0];
                ballsgun_network[eventData.Sender].transform.rotation = (Quaternion)array[1];
                ballsgun_network[eventData.Sender].GetComponent<Renderer>().material.color = (Color)array[3];
                var trail = ballsgun_network[eventData.Sender].AddComponent<TrailRenderer>();
                trail.material = new Material(Shader.Find("Sprites/Default"));
                trail.time = 1;
                trail.startWidth = .1f;
                trail.endWidth = 0;
                trail.minVertexDistance = 1;
                trail.material.color = (Color)array[3];
                var ballRB = ballsgun_network[eventData.Sender].AddComponent(typeof(Rigidbody)) as Rigidbody;
                ballRB.velocity = (Vector3)array[2];
            }
        }
        public static void FrozoneNetwork(EventData eventData)
        {
            byte code = eventData.Code;
            if (code == 90)
            {
                object[] array = (object[])eventData.CustomData;
                frozone_right_network[eventData.Sender] = GameObject.CreatePrimitive(PrimitiveType.Cube);
                frozone_right_network[eventData.Sender].transform.localScale = scale;
                if (NetworkColliders)
                    frozone_right_network[eventData.Sender].GetComponent<Collider>().enabled = true;
                else
                    frozone_right_network[eventData.Sender].GetComponent<Collider>().enabled = false;
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
                if (NetworkColliders)
                    frozone_left_network[eventData.Sender].GetComponent<Collider>().enabled = true;
                else
                    frozone_left_network[eventData.Sender].GetComponent<Collider>().enabled = false;
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
        public static void SendCube(Vector3 pos, Quaternion rot, int textureID, string cubeID)
        {
            var others = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
            object[] data = { pos, rot, textureID, cubeID, };
            Main.LegacySendEvent(112, data, others, true);
        }
        public static void RemoveOtherCube(string cubeID)
        {
            var others = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
            Main.LegacySendEvent(113, cubeID, others, true);
        }
        public static void ChangeOtherCollider(string cubeID, bool toRemove)
        {
            var others = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
            Main.LegacySendEvent(toRemove ? (byte)114 : (byte)115, cubeID, others, true);
        }
        public static void MinecraftNetwork(EventData eventData)
        {
            if (eventData.Code == 112)
            {
                var array = (object[])eventData.CustomData;
                var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.transform.position = (Vector3)array[0];
                cube.transform.rotation = (Quaternion)array[1];
                if ((int)array[2] == 8)
                    cube.GetComponent<Renderer>().material.shader = Main.DefaultShader;
                else if ((int)array[2] == 10)
                    cube.GetComponent<Renderer>().material = Main.TransparentMaterial(Main.GetChangeColorA(Color.white, .5f));
                else cube.GetComponent<Renderer>().material.shader = Main.UniversalShader;
                cube.GetComponent<Renderer>().material.mainTexture = Main.MCTextures[(int)array[2]];
                cube.AddComponent<Fun.CubeManager>().cubeID = (string)array[3];
                Fun.MinecraftCubes.Add(cube);
            }
            if (eventData.Code == 113)
            {
                var id = (string)eventData.CustomData;
                foreach (var cubes in Fun.MinecraftCubes)
                {
                    var cube = cubes.GetComponent<Fun.CubeManager>();
                    if (cube.cubeID == id)
                    { cubes.Destroy(); Fun.MinecraftCubes.Remove(cubes); }
                }
            }
            if (eventData.Code == 114)
            {
                var id = (string)eventData.CustomData;
                foreach (var cubes in Fun.MinecraftCubes)
                {
                    var cube = cubes.GetComponent<Fun.CubeManager>();
                    if (cube.cubeID == id)
                    { cubes.layer = 8; }
                }
            }
            if (eventData.Code == 115)
            {
                var id = (string)eventData.CustomData;
                foreach (var cubes in Fun.MinecraftCubes)
                {
                    var cube = cubes.GetComponent<Fun.CubeManager>();
                    if (cube.cubeID == id)
                        cubes.layer = 0;
                }
            }
        }

        public class Global : MonoBehaviour, IOnEventCallback
        {
            void Start() => PhotonNetwork.AddCallbackTarget(this);

            public static bool ReceiveGlobalPlatforms = false;
            public static bool ReceiveGlobalProjectiles = false;
            public static bool ReceiveGlobalPlatformGun = false;
            public static bool ReceiveGlobalBallGun = false;
            public static bool ReceiveGlobalFrozone = false;
            public static bool ReceiveGlobalMinecraft = false;

            public void OnEvent(EventData eventData)
            {
                if (ReceiveGlobalPlatforms) PlatformNetwork(eventData);
                if (ReceiveGlobalProjectiles) ProjectileNetwork(eventData);
                if (ReceiveGlobalPlatformGun) PlatformGunNetwork(eventData);
                if (ReceiveGlobalBallGun) BallGunNetwork(eventData);
                if (ReceiveGlobalFrozone) FrozoneNetwork(eventData);
                if (ReceiveGlobalMinecraft) MinecraftNetwork(eventData);
            }
        }
    }
}