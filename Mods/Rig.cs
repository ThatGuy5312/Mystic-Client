﻿using GorillaNetworking;
using MysticClient.Menu;
using MysticClient.Utils;
using static MysticClient.Menu.Main;
using Photon.Pun;
using System.Collections;
using UnityEngine;
using MysticClient.Classes;
using BepInEx;

namespace MysticClient.Mods
{
    public class Rig : GunLib
    {

        private static float MonkeSize;
        public static void SizeChanger()
        {
            if (Controller.rightControllerIndexFloat > 0.3f || UserInput.GetKey(KeyCode.Alpha1))
                MonkeSize += 0.1f;
            if (Controller.leftControllerIndexFloat > 0.3f || UserInput.GetKey(KeyCode.Alpha2))
                MonkeSize -= 0.1f;
            if (Controller.rightControllerPrimaryButton || UserInput.GetKey(KeyCode.Alpha3))
                MonkeSize = 1f;
            RigUtils.MyPlayer.scale = MonkeSize;
            RigUtils.MyOfflineRig.scaleFactor = MonkeSize;
        }
        public static void WackyMonke()
        {
            RigUtils.MyOfflineRig.head.rigTarget.eulerAngles = new Vector3(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f));
            RigUtils.MyOfflineRig.leftHand.rigTarget.eulerAngles = new Vector3(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f));
            RigUtils.MyOfflineRig.rightHand.rigTarget.eulerAngles = new Vector3(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f));
            RigUtils.MyOfflineRig.head.rigTarget.eulerAngles = new Vector3(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f));
            RigUtils.MyOfflineRig.leftHand.rigTarget.eulerAngles = new Vector3(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f));
            RigUtils.MyOfflineRig.rightHand.rigTarget.eulerAngles = new Vector3(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f));
        }
        public static void SetArmLength(Vector3 Length) => RigUtils.MyPlayer.transform.localScale = Length;
        public static void HeadTask(string task)
        {
            var spinSpeed = 12f;
            if (task == "HeadSpinX")
                RigUtils.MyOfflineRig.head.trackingRotationOffset.x += spinSpeed;
            else if (task == "HeadSpinY")
                RigUtils.MyOfflineRig.head.trackingRotationOffset.y += spinSpeed;
            else if (task == "HeadSpinZ")
                RigUtils.MyOfflineRig.head.trackingRotationOffset.z += spinSpeed;
            else if (task == "180 Head")
                RigUtils.MyOfflineRig.head.trackingRotationOffset.z = 180f;
            else if (task == "180 Y Head")
                RigUtils.MyOfflineRig.head.trackingRotationOffset.y = 180f;
            else if (task == "Fix Head")
                RigUtils.MyOfflineRig.head.trackingRotationOffset = new Vector3(0f, 0f, 0f);
        }
        public static void GrabRig()
        {
            if (Controller.rightGrab || UserInput.GetMouseButton(0))
            {
                RigUtils.MyOfflineRig.enabled = false;
                RigUtils.MyOfflineRig.transform.position = RigUtils.MyOnlineRig.rightHandTransform.position;
                RigUtils.MyOfflineRig.transform.rotation = RigUtils.MyOnlineRig.rightHandTransform.rotation;
            } else RigUtils.MyOfflineRig.enabled = true;
        }
        public static int GhostType;
        private static bool ghostToggled;
        private static bool invisToggled;
        public static void GhostMonke()
        {
            bool rp = Controller.rightControllerPrimaryButton || UserInput.GetMouseButton(0);
            if (GetEnabled("Make Ghost/Invis Toggled"))
            {
                if (rp)
                {
                    if (!ghostToggled && RigUtils.MyOfflineRig.enabled)
                    {
                        RigUtils.MyOfflineRig.enabled = false;
                        ghostToggled = true;
                        return;
                    }
                    if (!ghostToggled && !RigUtils.MyOfflineRig.enabled)
                    {
                        RigUtils.MyOfflineRig.enabled = true;
                        ghostToggled = true;
                        return;
                    }
                } else ghostToggled = false;
                return;
            }
            RigUtils.MyOfflineRig.enabled = !rp;
        }
        public static void InvisMonke()
        {
            bool rp = Controller.rightControllerPrimaryButton || UserInput.GetMouseButton(0);
            if (GetEnabled("Make Ghost/Invis Toggled"))
            {
                if (rp)
                {
                    if (!invisToggled && RigUtils.MyOfflineRig.enabled)
                    {
                        RigUtils.MyOfflineRig.enabled = false;
                        RigUtils.MyOfflineRig.transform.position = new Vector3(RigUtils.MyOfflineRig.transform.position.x, -100, RigUtils.MyOfflineRig.transform.position.z);
                        invisToggled = true;
                        return;
                    }
                    if (!invisToggled && !RigUtils.MyOfflineRig.enabled)
                    {
                        RigUtils.MyOfflineRig.enabled = true;
                        invisToggled = true;
                        return;
                    }
                } else invisToggled = false;
            }
            else
            {
                RigUtils.MyOfflineRig.transform.position = rp ? new Vector3(RigUtils.MyOfflineRig.transform.position.x, -100, RigUtils.MyOfflineRig.transform.position.z) : Vector3.zero;
                RigUtils.MyOfflineRig.enabled = !rp;
            }
        }
    }
}