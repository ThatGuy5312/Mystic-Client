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
            foreach (VRRig rigs in RigUtils.VRRigs)
            {
                if (rigs != RigUtils.MyOfflineRig)
                {
                    bool toggled = GetEnabled("Toggle Beacons (RT)") && Controller.rightControllerIndexFloat > 0.3f ^ UserInput.GetMouseButton(0);
                    if (toggled || !toggled)
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
        }
        public static void Tracers()
        {
            foreach (VRRig rigs in RigUtils.VRRigs)
            {
                bool toggled = GetEnabled("Toggle Tracers (RT)") && Controller.rightControllerIndexFloat > 0.3f ^ UserInput.GetMouseButton(0);
                if (toggled || !toggled)
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
        }
        public static bool ESPPlayerColor;
        public static void ESPOff()
        {
            foreach (VRRig rigs in RigUtils.VRRigs)
            {
                rigs.mainSkin.material.shader = Shader.Find("GorillaTag/UberShader");
                Color main = new Color(rigs.playerColor.r, rigs.playerColor.g, rigs.playerColor.b, 1f);
                rigs.mainSkin.material.color = main;
            }
        }
        public static void ESP()
        {
            foreach (VRRig rigs in RigUtils.VRRigs)
            {
                bool toggled = GetEnabled("Toggle ESP (RT)") && Controller.rightControllerIndexFloat > 0.3f ^ UserInput.GetMouseButton(0);
                if (rigs != RigUtils.MyOfflineRig && toggled || !toggled)
                {
                    rigs.mainSkin.material.shader = Shader.Find("GUI/Text Shader");
                    if (rigs.mainSkin.material.name.Contains("fected") || rigs.mainSkin.material.name.Contains("it"))
                    {
                        Color normal = Color.Lerp(Color.black, Color.red, Mathf.PingPong(Time.time, 1));
                        Color main = new Color(rigs.playerColor.r, rigs.playerColor.g, rigs.playerColor.b, 0.5f);
                        if (ESPPlayerColor)
                            rigs.mainSkin.material.color = main;
                        else
                            rigs.mainSkin.material.color = normal;
                    }
                    else
                    {
                        Color normal = Color.Lerp(Color.black, Color.green, Mathf.PingPong(Time.time, 1));
                        Color main = new Color(rigs.playerColor.r, rigs.playerColor.g, rigs.playerColor.b, 0.5f);
                        if (ESPPlayerColor)
                            rigs.mainSkin.material.color = main;
                        else
                            rigs.mainSkin.material.color = normal;
                    }
                }
                else { ESPOff(); }
            }
        }
    }
}