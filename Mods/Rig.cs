using GorillaNetworking;
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
            bool toggled = GetEnabled("Toggle Wacky Monke (RT)") && Controller.rightControllerIndexFloat > 0.3f ^ UserInput.GetMouseButton(0);
            if (toggled || !toggled)
            {
                GorillaTagger.Instance.offlineVRRig.head.rigTarget.eulerAngles = new Vector3(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f));
                GorillaTagger.Instance.offlineVRRig.leftHand.rigTarget.eulerAngles = new Vector3(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f));
                GorillaTagger.Instance.offlineVRRig.rightHand.rigTarget.eulerAngles = new Vector3(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f));
                GorillaTagger.Instance.offlineVRRig.head.rigTarget.eulerAngles = new Vector3(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f));
                GorillaTagger.Instance.offlineVRRig.leftHand.rigTarget.eulerAngles = new Vector3(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f));
                GorillaTagger.Instance.offlineVRRig.rightHand.rigTarget.eulerAngles = new Vector3(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f));
            }
        }
        public static void SetArmLength(Vector3 Length) => RigUtils.MyPlayer.transform.localScale = Length;
        public static void HeadTask(string task)
        {
            bool toggled = GetEnabled("Toggle Head Spin (RT)") && Controller.rightControllerIndexFloat > 0.3f ^ UserInput.GetMouseButton(0);
            float spinSpeed = 12f;
            if (task == "HeadSpinX")
            {
                if (toggled || !toggled)
                {
                    RigUtils.MyOfflineRig.head.trackingRotationOffset.x += spinSpeed;
                }
            }
            else if (task == "HeadSpinY")
            {
                if (toggled || !toggled)
                {
                    RigUtils.MyOfflineRig.head.trackingRotationOffset.y += spinSpeed;
                }
            }
            else if (task == "HeadSpinZ")
            {
                if (toggled || !toggled)
                {
                    RigUtils.MyOfflineRig.head.trackingRotationOffset.z += spinSpeed;
                }
            }
            else if (task == "180 Head")
            {
                if (toggled || !toggled)
                {
                    RigUtils.MyOfflineRig.head.trackingRotationOffset.z = 180f;
                }
            }
            else if (task == "180 Y Head")
            {
                if (toggled || !toggled)
                {
                    RigUtils.MyOfflineRig.head.trackingRotationOffset.y = 180f;
                }
            }
            else if (task == "Fix Head")
            {
                RigUtils.MyOfflineRig.head.trackingRotationOffset = new Vector3(0f, 0f, 0f);
            }
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
        //private static bool ghostToggle;
        //private static bool invisToggle;
        //private static bool buttonHitG;
        //private static bool buttonHitI;
        public static void GhostMonke()
        {
            bool rp = Controller.rightControllerPrimaryButton || UserInput.GetMouseButton(0);
            /*if (GetEnabled("Make Ghost/Invis Toggled"))
                if (rp && !buttonHitG)
                    ghostToggle = !ghostToggle;
            else
                RigUtils.MyOfflineRig.enabled = rp ? false : true;
            RigUtils.MyOfflineRig.enabled = ghostToggle ? false : true;
            buttonHitG = rp;*/
            RigUtils.MyOfflineRig.enabled = !rp;
        }
        public static void InvisMonke()
        {
            bool rp = Controller.rightControllerPrimaryButton || UserInput.GetMouseButton(0);
            /*if (GetEnabled("Make Ghost/Invis Toggled"))
                if (rp && !buttonHitI)
                    invisToggle = !invisToggle;
            else
                RigUtils.MyOfflineRig.headBodyOffset = rp ? new Vector3(999f, 999f, 999f) : Vector3.zero;
            //RigUtils.MyOfflineRig.enabled = rp ? false : true;
            RigUtils.MyOfflineRig.headBodyOffset = invisToggle ? new Vector3(999f, 999f, 999f) : Vector3.zero;
            buttonHitI = rp;*/
            RigUtils.MyOfflineRig.transform.position = rp ? new Vector3(999f, 999f, 999f) : Vector3.zero;
            RigUtils.MyOfflineRig.enabled = !rp;
        }
    }
}