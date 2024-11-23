﻿using MysticClient.Utils;
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
using Photon.Voice;
using GorillaTagScripts;

namespace MysticClient.Classes
{
    [Obsolete("This Class Is Not Meant To Be Used It's Just Here To Hold Old Or Random Mods")]
    internal class UnUsed : MonoBehaviour
    {
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
        private static void ProjSpammer() // this wont work... but
        {
            if (PhotonSystem.InRoom)
                new PhotonView().RPC("OnHandTapRPC", RpcTarget.All, new object[]
                {
                    32,
                    false,
                    1f,
                    1f
                });
        }
        private static void TagEffect(Player target)
        {
            if (PhotonSystem.IsMasterClient)
            {
                new PhotonView().RPC("DeserializePlayerEffect", target, new object[] { 1, RigUtils.GetViewFromPlayer(RigUtils.GetNetFromPlayer(target)).ViewID });
            }
        }
        private static void LaunchPlayer(NetPlayer target, Vector3 velo)
        {
            SendEvent(8, velo, target, false);
        }
        private static void LaunchPlayer(Player target, Vector3 velo)
        {
            if (PhotonSystem.IsMasterClient)
            {
                new PhotonView().RPC("DeserializePlayerLaunched", target, new object[] { velo });
            }
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