using static MysticClient.Menu.Main;
using UnityEngine;
using MysticClient.Utils;
using ExitGames.Client.Photon;
using Photon.Realtime;
using Photon.Pun;
using MysticClient.Menu;
using MysticClient.Classes;
using UnityEngine.Animations.Rigging;
using System.ComponentModel.Design;
using MysticClient.Notifications;

namespace MysticClient.Mods
{
    public class Visuals : MonoBehaviour
    {
        public static void NameTags()
        {
            foreach (var rigs in RigUtils.VRRigs)
                CreateText(rigs.playerName, rigs.transform.position + new Vector3(0, .5f, 0), rigs.transform.position.ToLookQuat(RigUtils.MyPlayer.headCollider.transform.position));
        }
        public static void RainOff() // instiantate on startup (spelling fr)
        {
            BetterDayNightManager.instance.weatherCycle[BetterDayNightManager.instance.currentWeatherIndex] = BetterDayNightManager.WeatherType.None;
            BetterDayNightManager.instance.CurrentWeather();
            if (rainPos != Vector3.zero)
                GameObject.Find("Environment Objects/LocalObjects_Prefab/Forest/Environment/WeatherDayNight/rain").transform.position = rainPos;
            GameObject.Find("Environment Objects/LocalObjects_Prefab/Forest/Environment/WeatherDayNight/rain").transform.parent = GameObject.Find("Environment Objects/LocalObjects_Prefab/Forest/Environment/WeatherDayNight").transform;
        }
        private static Vector3 rainPos = Vector3.zero;
        public static void Rain()
        {
            BetterDayNightManager.instance.weatherCycle[BetterDayNightManager.instance.currentWeatherIndex] = BetterDayNightManager.WeatherType.Raining;
            BetterDayNightManager.instance.CurrentWeather();
            if (rainPos == Vector3.zero)
                rainPos = GameObject.Find("Environment Objects/LocalObjects_Prefab/Forest/Environment/WeatherDayNight/rain").transform.position;
            GameObject.Find("Environment Objects/LocalObjects_Prefab/Forest/Environment/WeatherDayNight/rain").transform.parent = RigUtils.MyPlayer.transform;
            GameObject.Find("Environment Objects/LocalObjects_Prefab/Forest/Environment/WeatherDayNight/rain").transform.position = RigUtils.MyPlayer.transform.position + new Vector3(0, 5, 0);
        }
        public static void VoiceESP()
        {
            foreach (var rigs in RigUtils.VRRigs)
                if (rigs != RigUtils.MyOfflineRig)
                {
                    rigs.mainSkin.material.shader = Shader.Find("GUI/Text Shader");
                    var gpl = rigs.GetComponent<GorillaSpeakerLoudness>();
                    var main = new Color(rigs.playerColor.r, rigs.playerColor.g, rigs.playerColor.b, gpl.Loudness * .3f);
                }
        }
        public static Color handTrailColor = Color.black;
        private static TrailRenderer righthandTrail = null;
        public static void HandTrail()
        {
            if (righthandTrail == null)
            {
                righthandTrail = RigUtils.MyPlayer.rightControllerTransform.AddComponent<TrailRenderer>();
                righthandTrail.material = new Material(Shader.Find("Sprites/Default"));
                righthandTrail.material.color = handTrailColor;
                righthandTrail.time = 1f;
                righthandTrail.startWidth = 1;
                righthandTrail.endWidth = 0;
                righthandTrail.minVertexDistance = 6f;
            }
        }
        public static void HandTrailOff()
        {
            if (righthandTrail != null)
            {
                Destroy(righthandTrail);
                righthandTrail = null;
            }
        }
        public static void Beacons()
        {
            foreach (var rigs in RigUtils.VRRigs)
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
        public static void HuntTracers(string tooltip)
        {
            if (Main.GorillaGameManager.GameType() == GorillaGameModes.GameModeType.Hunt)
            {
                foreach (var players in RigUtils.NetPlayers)
                    if (players == Main.GorillaHuntManager.GetTargetOf(RigUtils.MyNetPlayer))
                    {
                        var line = new GameObject();
                        var lineRend = line.AddComponent<LineRenderer>();
                        lineRend.material.color = RigUtils.GetRigFromPlayer(players).playerColor;
                        lineRend.startWidth = .01f;
                        lineRend.endWidth = .01f;
                        lineRend.positionCount = 2;
                        lineRend.useWorldSpace = true;
                        lineRend.SetPosition(0, RigUtils.MyOnlineRig.rightHandTransform.position);
                        lineRend.SetPosition(1, RigUtils.GetRigFromPlayer(players).transform.position);
                        lineRend.material.shader = Shader.Find("GUI/Text Shader");
                        Destroy(lineRend, Time.deltaTime);
                        Destroy(line, Time.deltaTime);
                    }
            }
            else { NotifiLib.SendNotification(NotifUtils.Error() + "You Are Not In The Hunt GameMode"); GetToolTip(tooltip).enabled = false; }
        }
        public static void InfectionTracers()
        {
            foreach (var rigs in RigUtils.VRRigs)
            {
                var line = new GameObject();
                var lineRend = line.AddComponent<LineRenderer>();
                lineRend.startWidth = .01f;
                lineRend.endWidth = .01f;
                lineRend.positionCount = 2;
                lineRend.useWorldSpace = true;
                lineRend.SetPosition(0, RigUtils.MyOnlineRig.rightHandTransform.position);
                lineRend.SetPosition(1, rigs.transform.position);
                lineRend.material.shader = Shader.Find("GUI/Text Shader");
                Destroy(lineRend, Time.deltaTime);
                Destroy(line, Time.deltaTime);
                if (IsTagged(rigs))
                    lineRend.material.color = Color.red;
                else
                    lineRend.material.color = Color.green;
            }
        }
        public static void CasualTracers()
        {
            foreach (var rigs in RigUtils.VRRigs)
            {
                var line = new GameObject();
                var lineRend = line.AddComponent<LineRenderer>();
                lineRend.material.color = rigs.playerColor;
                lineRend.startWidth = .01f;
                lineRend.endWidth = .01f;
                lineRend.positionCount = 2;
                lineRend.useWorldSpace = true;
                lineRend.SetPosition(0, RigUtils.MyOnlineRig.rightHandTransform.position);
                lineRend.SetPosition(1, rigs.transform.position);
                lineRend.material.shader = Shader.Find("GUI/Text Shader");
                Destroy(lineRend, Time.deltaTime);
                Destroy(line, Time.deltaTime);
            }
        }
        public static void ESPOff()
        {
            foreach (var rigs in RigUtils.VRRigs)
            {
                rigs.mainSkin.material.shader = Shader.Find("GorillaTag/UberShader");
                var main = new Color(rigs.playerColor.r, rigs.playerColor.g, rigs.playerColor.b, 1f);
                rigs.mainSkin.material.color = main;
            }
        }
        public static void InfectionESP()
        {
            foreach (var rigs in RigUtils.VRRigs)
                if (rigs != RigUtils.MyOfflineRig)
                {
                    rigs.mainSkin.material.shader = Shader.Find("GUI/Text Shader");
                    if (rigs.mainSkin.material.name.Contains("fected") || rigs.mainSkin.material.name.Contains("it"))
                        rigs.mainSkin.material.color = Color.Lerp(Color.black, Color.red, Mathf.PingPong(Time.time, 1));
                    else
                        rigs.mainSkin.material.color = Color.Lerp(Color.black, Color.green, Mathf.PingPong(Time.time, 1));
                }
        }
        public static void CasualESP()
        {
            foreach (var rigs in RigUtils.VRRigs)
                if (rigs != RigUtils.MyOfflineRig)
                {
                    rigs.mainSkin.material.shader = Shader.Find("GUI/Text Shader");
                    var main = new Color(rigs.playerColor.r, rigs.playerColor.g, rigs.playerColor.b, 1f);
                    rigs.mainSkin.material.color = main;
                }
        }
        public static void HuntESP(string tooltip)
        {
            if (Main.GorillaGameManager.GameType() == GorillaGameModes.GameModeType.Hunt)
            {
                foreach (var players in RigUtils.NetPlayers)
                    if (players == Main.GorillaHuntManager.GetTargetOf(RigUtils.MyNetPlayer))
                        RigUtils.GetRigFromPlayer(players).mainSkin.material.shader = Shader.Find("GUI/Text Shader");
            } else { NotifiLib.SendNotification(NotifUtils.Error() + "You Are Not In The Hunt GameMode"); GetToolTip(tooltip).enabled = false; }
        }
    }
}