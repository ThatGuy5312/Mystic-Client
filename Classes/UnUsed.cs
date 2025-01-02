using MysticClient.Utils;
using System;
using Photon.Realtime;
using static MysticClient.Menu.Main;
using UnityEngine;
using Photon.Pun;
using Viveport;
using System.Reflection;
using GorillaNetworking;
using System.Collections.Generic;
using MysticClient.Notifications;
using Random = UnityEngine.Random;
using HarmonyLib;
using Unity.XR.CoreUtils;
using Photon.Voice.PUN.UtilityScripts;
using Fusion;

namespace MysticClient.Classes
{
    [Obsolete("This Class Is Not Meant To Be Used It's Just Here To Hold Old Or Random Mods")]
    internal class UnUsed : MonoBehaviour
    {
        public static void SetColor(Color color)
        {
            typeof(GorillaColorizableBase).GetMethod("SetColor", NonPublicInstance).Invoke(typeof(GorillaColorizableBase), new object[] { color });
        }
        public static void RaycastExample()
        {
            var startPos = GorillaTagger.Instance.rightHandTransform.position; // start position of raycast
            var direction = GorillaTagger.Instance.rightHandTransform.up; // direction of raycast
            // you dont need to do the var stuff you can just put it in like this
            // Physics.Raycast(GorillaTagger.Instance.rightHandTransform.position, GorillaTagger.Instance.rightHandTransform.up, out RaycastHit hit);
            Physics.Raycast(startPos, direction, out RaycastHit hit);
            //hit.point; // position of where the end point raycast is hitting
        }
        public static void UnUsedBundleStuff()
        {
            BundleObjects[0].transform.localScale = new Vector3(8, 8, 8);
            BundleObjects[0].transform.position = RigUtils.MyOfflineRig.transform.position + new Vector3(0, .75f + (Mathf.Sin(Time.time * Mathf.PI * 2 * .5f) * .05f), 0);
            BundleObjects[0].transform.Rotate(0, 0, 1);
            BundleObjects[1].transform.parent = RigUtils.MyOnlineRig.rightHandTransform;
            BundleObjects[1].transform.position = RigUtils.MyOnlineRig.rightHandTransform.position;
            BundleObjects[1].transform.rotation = RigUtils.MyOnlineRig.rightHandTransform.rotation;
            var dict = new Dictionary<int, string>
            {
                { 0, "Grass" }, { 1, "Dirt" }, { 2, "Wood" }, { 3, "Leaf" }, { 4, "Plank" }, { 5, "Stone" }, { 6, "Cobblestone" },
                { 7, "HeyBale" }, { 8, "Glass" }, { 9, "Obsidian" }, { 10, "Water" }, { 11, "TrapDoor" }
            };
            for (int i = 0; i < dict.Keys.Count; i++)
            {
                dict.TryGetValue(i, out var name);
                MCTextures[i] = (Texture2D)BundleObjects[0].GetNamedChild(name).GetComponent<Renderer>().material.mainTexture;
            }
        }
        public static void CorruptObject(GameObject obj) // it doesnt really corrupt anything i just thought it might me able to be used for a cool mod someday
        {
            obj.transform.position += new Vector3(
                Random.Range(-5f, 5f),
                Random.Range(-5f, 5f),
                Random.Range(-5f, 5f)
            );
            obj.transform.rotation = Random.rotation;
            obj.transform.localScale *= Random.Range(.5f, 2f);
            var rend = obj.GetComponent<Renderer>();
            if (rend != null)
            {
                var col = new Color(
                    Random.value,
                    Random.value,
                    Random.value
                );
                rend.material = TransparentMaterial(GetChangeColorA(col, Random.Range(.1f, 1f)));
                rend.material.mainTexture = MCTextures[Random.Range(0, 10)];
            }
            var rb = obj.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddForce(new Vector3(
                    Random.Range(-10f, 10f),
                    Random.Range(-10f, 10f),
                    Random.Range(-10f, 10f)
                ), ForceMode.Impulse);
            }
        }
        public static void LagGun()
        {
            if (GunLib.CreateGun(out VRRig rig) && PhotonNetwork.InRoom)
            {
                for (int i = 0; i < 100; i++)
                {
                    RPCManager.LagEvent(RigUtils.GetViewFromRig(rig).Owner);
                    RPCProtection();
                }
            }
        }
        public static void BetaLaunchProjectie(int ProjHash, int TrailHash, Vector3 pos, Vector3 vel)
        {
            Traverse.Create(GameObject.Find("PhotonMono").GetComponent<PhotonHandler>()).Field("nextSendTickCountOnSerialize").SetValue((int)(Time.realtimeSinceStartup * 9999f)); // this is from iidks menu
            typeof(ProjectileWeapon).GetField("GetLaunchPosition", NonPublicInstance).SetValue(RigUtils.MyOfflineRig.projectileWeapon, pos);
            typeof(ProjectileWeapon).GetField("GetLaunchVelocity", NonPublicInstance).SetValue(RigUtils.MyOfflineRig.projectileWeapon, vel);
            typeof(ProjectileWeapon).GetField("projectilePrefab", NonPublicInstance).SetValue(RigUtils.MyOfflineRig.projectileWeapon, MUtils.instance.Instantiate(ProjHash, false));
            typeof(ProjectileWeapon).GetField("projectileTrail", NonPublicInstance).SetValue(RigUtils.MyOfflineRig.projectileWeapon, MUtils.instance.Instantiate(TrailHash, false));
            typeof(ProjectileWeapon).GetMethod("LaunchProjectile", NonPublicInstance).Invoke(RigUtils.MyOfflineRig.projectileWeapon, null);
        }
        public static void MuteAll()
        {
            foreach (var rigs in RigUtils.VRRigs)
            {
                LegacySendEvent(0, null, new RaiseEventOptions
                {
                    CachingOption = EventCaching.DoNotCache,
                    Receivers = ReceiverGroup.Others,
                }, true);
                var v = RigUtils.GetNetViewFromRig(rigs).GetView;
                v.ControllerActorNr = PhotonSystem.LocalPlayer.ActorNumber;
                PhotonNetwork.Destroy(v); // und working no ban 100%
            }
        }
        public static void MuteGun()
        {
            if (GunLib.CreateGun(out VRRig rig))
            {
                LegacySendEvent(0, null, new RaiseEventOptions 
                {
                    CachingOption = EventCaching.DoNotCache,
                    TargetActors = new int[] { RigUtils.GetPlayerFromRig(rig).ActorNumber },
                }, true);
                var v = RigUtils.GetNetViewFromRig(rig).GetView; 
                v.ControllerActorNr = PhotonSystem.LocalPlayer.ActorNumber;
                PhotonNetwork.Destroy(v); // und working no ban 100%
            }
        }
        public static void VStumpKickAll()
        {
            SendRPC(GameObject.Find("NetworkObject").GetComponent<PhotonView>(), "SetRoomMapRPC", RpcTarget.All, new object[] { Random.Range(long.MinValue, long.MaxValue) });
            SendRPC(GameObject.Find("NetworkObject").GetComponent<PhotonView>(), "UnloadMapRPC", RpcTarget.All, null);
            RPCProtection();
        }
        public static void VStumpKickGun()
        {
            if (GunLib.CreateGun(out VRRig rig))
            {
                SendRPC(GameObject.Find("NetworkObject").GetComponent<PhotonView>(), "SetRoomMapRPC", RigUtils.GetViewFromRig(rig).Owner, new object[] { Random.Range(long.MinValue, long.MaxValue) });
                SendRPC(GameObject.Find("NetworkObject").GetComponent<PhotonView>(), "UnloadMapRPC", RigUtils.GetViewFromRig(rig).Owner, null);
                RPCProtection();
            }
        }
        // fuck you i got made this method when custom maps came out i just never got it to work but it was just do to not having the right photon view
        // https://cdn.discordapp.com/attachments/1111105512482820217/1312960859621822556/image.png?ex=674e6596&is=674d1416&hm=8039cc922755bf771337da04f1da5b7c8ecd909146b7b30ff7f4f3299b2a9e86 my code when i made it when they came out
        public static void UnloadCustomMap()
        {
            SendRPC(GameObject.Find("NetworkObject").GetComponent<PhotonView>(), "UnloadMapRPC", RpcTarget.All, null);
            RPCProtection();
        } 
        private static void CreatePlatform(Transform hand)
        {
            var obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            if (hand == GorillaTagger.Instance.rightHandTransform)
                obj.name = "Right_PLATFORM";
            else if (hand == GorillaTagger.Instance.leftHandTransform)
                obj.name = "Left_PLATFORM";
            obj.transform.localScale = new Vector3(0.0125f, 0.28f, 0.3825f) * GorillaLocomotion.Player.Instance.scale;
            obj.GetComponent<Renderer>().material.color = Color.blue; // color of platforms
            obj.transform.position = hand.position;
            obj.transform.rotation = hand.rotation;
        }

        // plats i made for someone use if you want

        private static bool on_right;
        public static bool on_left;
        private static bool on_right_false;
        public static bool on_left_false;
        public static void Plats()
        {
            if (ControllerInputPoller.instance.rightGrab)
            {
                if (!on_right)
                {
                    CreatePlatform(GorillaTagger.Instance.rightHandTransform);
                    on_right = true;
                    on_right_false = false;
                }
            }
            else if (!on_right_false)
            {
                Destroy(GameObject.Find("Right_PLATFORM"));
                on_right = false;
                on_right_false = true;
            }
            if (ControllerInputPoller.instance.leftGrab)
            {
                if (!on_left)
                {
                    CreatePlatform(GorillaTagger.Instance.leftHandTransform);
                    on_left = true;
                    on_left_false = false;
                }

            }
            else if (!on_left_false)
            {
                Destroy(GameObject.Find("Left_PLATFORM"));
                on_left = false;
                on_left_false = true;
            }
        }

        /*[HarmonyPatch(typeof(PlayFabAuthenticator), "ShowBanMessage")]
        public class BanMessage : MonoBehaviour
        {
            private static bool Prefix(PlayFabAuthenticator.BanInfo __instance)
            {
                try
                {
                    if (__instance.BanExpirationTime != null && __instance.BanMessage != null)
                    {
                        if (__instance.BanExpirationTime != "Indefinite")
                        {
                            int milliseconds = (int)(DateTime.Parse(__instance.BanExpirationTime) - DateTime.UtcNow).TotalMilliseconds;
                            int seconds = (int)(DateTime.Parse(__instance.BanExpirationTime) - DateTime.UtcNow).TotalSeconds;
                            int minutes = (int)(DateTime.Parse(__instance.BanExpirationTime) - DateTime.UtcNow).TotalMinutes;
                            int hours = (int)(DateTime.Parse(__instance.BanExpirationTime) - DateTime.UtcNow).TotalHours;
                            PlayFabAuthenticator.instance.gorillaComputer.GeneralFailureMessage(string.Concat(new string[]
                            {
                            "YOUR EXECUTION WILL HAPPIN IN\n ",
                            hours.ToString() + " | ",
                            minutes.ToString() + " | ",
                            seconds.ToString() + " | ",
                            milliseconds.ToString() + "\nREASON: ",
                            "COMMITING TERRISTIC CRIMES ON ANOTHER AXIOM."
                            }));
                        }
                        else
                        {
                            PlayFabAuthenticator.instance.gorillaComputer.GeneralFailureMessage("YOUR ACCOUNT HAS BEEN BANNED INDEFINITELY.\nREASON: horrible");
                        }
                    }
                }
                catch { }
                return false;
            }
        }*/

        private static GameObject AntiReportBall = null;
        private static void AntiReport() // my old anti report, stopped working
        {
            if (Controller.rightGrab || UserInput.GetMouseButton(0))
            {
                if (AntiReportBall == null)
                {
                    AntiReportBall = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    AntiReportBall.GetComponent<Renderer>().material.color = new Color32(167, 17, 237, 28);
                    Destroy(AntiReportBall.GetComponent<Collider>());
                    Destroy(AntiReportBall.GetComponent<Rigidbody>());
                    AntiReportBall.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                }
                AntiReportBall.transform.position = new Vector3(0f, 0f, 0.3f) + RigUtils.MyPlayer.rightControllerTransform.transform.position;
                AntiReportBall.transform.rotation = RigUtils.MyPlayer.rightControllerTransform.transform.rotation;
            }
        }
        private static void AntiReportInternal()
        {
            if (AntiReportBall != null)
                foreach (var rig in RigUtils.VRRigs)
                    if (rig != RigUtils.MyOfflineRig)
                    {
                        var TheirRightHand = Vector3.Distance(AntiReportBall.transform.position, rig.rightHandTransform.position);
                        var TheirLeftHand = Vector3.Distance(AntiReportBall.transform.position, rig.leftHandTransform.position);
                        if (TheirRightHand < .45f || TheirLeftHand < .45f)
                        {
                            PhotonNetwork.Disconnect();
                            NotifiLib.SendNotification(NotifUtils.AntiReport() + RigUtils.GetPlayerFromRig(rig).NickName + " Attempted To Report You, You Have Been Disconnected");
                            RPCProtection();
                        }
                    }
        }
        private static void UnUsedBoardStuff()
        {
            for (int i = 0; i < GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomInteractables/GorillaComputerObject/ComputerUI/keyboard (1)/Buttons/Keys").transform.childCount; i++)
            {
                var btns = GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomInteractables/GorillaComputerObject/ComputerUI/keyboard (1)/Buttons/Keys").transform.GetChild(i);
                if (btns.GetComponent<GorillaKeyboardButton>())
                {
                    var btn = btns.GetComponent<GorillaKeyboardButton>().ButtonColorSettings;
                    btn.UnpressedColor = Color.cyan;
                    btn.PressedColor = Color.magenta;
                }
            }
            var AllGNJT = (List<GorillaNetworkJoinTrigger>)typeof(PhotonNetworkController)
                .GetField("allJoinTriggers", BindingFlags.NonPublic | BindingFlags.Instance)
                .GetValue(PhotonNetworkController.Instance);
            foreach (var GNJT in AllGNJT)
            {
                var UI = (JoinTriggerUI)typeof(GorillaNetworkJoinTrigger).GetField("ui", BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(GNJT);
                var bmt = (JoinTriggerUITemplate)typeof(JoinTriggerUI).GetField("template", BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(UI);
                foreach (var field in bmt.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public))
                {
                    if (field.FieldType == typeof(Material))
                    {
                        Material mat = field.GetValue(bmt) as Material;
                        mat.color = boardColor;
                    }
                }
            }
        }
        private static void Spam(int index = 32) // this wont work... but
        {
            if (PhotonSystem.InRoom)
                SendRPC(RigUtils.MyPhotonView, "OnHandTapRPC", RpcTarget.All, new object[]
                {
                    index,
                    false,
                    1f,
                    VRRig.PackWorldPosForNetwork(RigUtils.MyOnlineRig.rightHandTransform.position)
                });
        }
        private static void TagEffect(Player target)
        {
            if (PhotonSystem.IsMasterClient)
                new PhotonView().RPC("DeserializePlayerEffect", target, new object[] { 1, RigUtils.GetViewFromPlayer(RigUtils.GetNetFromPlayer(target)).ViewID });
        }
        private static void LaunchPlayer(NetPlayer target, Vector3 velo)
        {
            SendEvent(8, velo, target, false);
        }
        private static void LaunchPlayer(Player target, Vector3 velo)
        {
            if (PhotonSystem.IsMasterClient)
                new PhotonView().RPC("DeserializePlayerLaunched", target, new object[] { velo });
        }
        private static void SendSound(int id, float vol)
        {
            if (PhotonSystem.InRoom)
            {
                new PhotonView().RPC("DeserializeSoundEffect", RpcTarget.All, new object[] { id, vol, RigUtils.MyPhotonView.ViewID });
            }
        }

        // the projectile code under this comment is public i just put it in here while making it

        private static void ExampleSpam()
        {
            if (ControllerInputPoller.instance.rightGrab)
            {
                int proj = -675036877; // projectile hash code
                int trail = 1432124712; // projectile trail hash code / set to -1 if you dont want a trail

                // the numbers that are set right now is a snow ball with a slingshot type trail

                var col = Color.white; // color of projectile
                Vector3 pos = GorillaLocomotion.Player.Instance.rightControllerTransform.position; // spawn position of projectile
                Vector3 vel = GorillaLocomotion.Player.Instance.rightHandCenterVelocityTracker.GetAverageVelocity(true, 0f); // velocity of projectile
                LaunchProjectile(proj, trail, pos, vel, col); // launches proj remember these are client sided

                // you can get rid of these // comments
            }
        }

        private static void LaunchProjectile(int projHash, int trailHash, Vector3 pos, Vector3 vel, Color col)
        {
            var projectile = ObjectPools.instance.Instantiate(projHash).GetComponent<SlingshotProjectile>();
            if (trailHash != -1)
            {
                var trail = ObjectPools.instance.Instantiate(trailHash).GetComponent<SlingshotProjectileTrail>();
                trail.AttachTrail(projectile.gameObject, false, false);
            }
            var counter = 0;
            projectile.Launch(pos, vel, NetworkSystem.Instance.LocalPlayer, false, false, counter++, 1, true, col);
        }
    }
}