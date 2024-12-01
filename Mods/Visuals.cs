using static MysticClient.Menu.Main;
using UnityEngine;
using MysticClient.Utils;
using ExitGames.Client.Photon;
using Photon.Realtime;
using Photon.Pun;
using MysticClient.Menu;

namespace MysticClient.Mods
{
    public class Visuals : MonoBehaviour
    {
        public static void Beacons()
        {
            foreach (var rigs in RigUtils.VRRigs)
            {
                if (rigs != RigUtils.MyOfflineRig)
                {
                    var beacon = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                    beacon.transform.localScale = new Vector3(0.04f, 200f, 0.04f);
                    beacon.GetComponent<Renderer>().material = rigs.mainSkin.material;
                    beacon.transform.rotation = rigs.transform.rotation;
                    beacon.transform.position = rigs.transform.position;
                    Destroy(beacon, Time.deltaTime);
                }
            }
        }
        /*public static void InfectionTracers()
        {
            foreach (VRRig rigs in RigUtils.VRRigs)
            {
                var line = new GameObject();
                var lineRend = line.AddComponent<LineRenderer>();
                lineRend.startWidth = 0.01f;
                lineRend.endWidth = 0.01f;
                lineRend.positionCount = 2;
                lineRend.useWorldSpace = true;
                lineRend.SetPosition(0, GorillaTagger.Instance.rightHandTransform.position);
                lineRend.SetPosition(1, rigs.transform.position);
                lineRend.material.shader = Shader.Find("GUI/Text Shader");
                Destroy(lineRend, Time.deltaTime);
                Destroy(line, Time.deltaTime);
                if (rigs.mainSkin.material.name.Contains("fected") || rigs.mainSkin.material.name.Contains("it"))
                {
                    lineRend.startColor = Color.red;
                    lineRend.endColor = Color.red;
                }
                else
                {
                    lineRend.startColor = Color.green;
                    lineRend.endColor = Color.green;
                }
            }
        }*/
        public static void Tracers()
        {
            foreach (var rigs in RigUtils.VRRigs)
            {
                var line = new GameObject();
                var lineRend = line.AddComponent<LineRenderer>();
                lineRend.startColor = MenuSettings.FirstColor;
                lineRend.endColor = MenuSettings.FirstColor;
                lineRend.startWidth = 0.01f;
                lineRend.endWidth = 0.01f;
                lineRend.positionCount = 2;
                lineRend.useWorldSpace = true;
                lineRend.SetPosition(0, RigUtils.MyOnlineRig.rightHandTransform.position);
                lineRend.SetPosition(1, rigs.transform.position);
                lineRend.material.shader = Shader.Find("GUI/Text Shader");
                Destroy(lineRend, Time.deltaTime);
                Destroy(line, Time.deltaTime);
            }
        }
        public static bool ESPPlayerColor;
        public static void ESPOff()
        {
            foreach (VRRig rigs in RigUtils.VRRigs)
            {
                rigs.mainSkin.material.shader = Shader.Find("GorillaTag/UberShader");
                var main = new Color(rigs.playerColor.r, rigs.playerColor.g, rigs.playerColor.b, 1f);
                rigs.mainSkin.material.color = main;
            }
        }
        public static void ESP()
        {
            foreach (VRRig rigs in RigUtils.VRRigs)
            {
                if (rigs != RigUtils.MyOfflineRig)
                {
                    rigs.mainSkin.material.shader = Shader.Find("GUI/Text Shader");
                    if (rigs.mainSkin.material.name.Contains("fected") || rigs.mainSkin.material.name.Contains("it"))
                    {
                        var normal = Color.Lerp(Color.black, Color.red, Mathf.PingPong(Time.time, 1));
                        var main = new Color(rigs.playerColor.r, rigs.playerColor.g, rigs.playerColor.b, 0.5f);
                        if (ESPPlayerColor)
                            rigs.mainSkin.material.color = main;
                        else
                            rigs.mainSkin.material.color = normal;
                    }
                    else
                    {
                        var normal = Color.Lerp(Color.black, Color.green, Mathf.PingPong(Time.time, 1));
                        var main = new Color(rigs.playerColor.r, rigs.playerColor.g, rigs.playerColor.b, 0.5f);
                        if (ESPPlayerColor)
                            rigs.mainSkin.material.color = main;
                        else
                            rigs.mainSkin.material.color = normal;
                    }
                }
            }
        }
    }
}