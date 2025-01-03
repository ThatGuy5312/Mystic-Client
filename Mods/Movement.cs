﻿using static MysticClient.Menu.Main;
using UnityEngine;
using ExitGames.Client.Photon;
using MysticClient.Classes;
using Photon.Pun;
using Photon.Realtime;
using System.Net;
using MysticClient.Utils;
using MysticClient.Menu;
using BepInEx;
using Valve.VR;
using Steamworks;
using UnityEngine.UIElements;
using System.Collections;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using GorillaExtensions;

namespace MysticClient.Mods
{
    public class Movement : GunLib
    {
        private static bool isRightGapple = false;
        private static bool isLeftGapple = false;
        private static Vector3 rightGrapplePoint;
        private static Vector3 leftGrapplePoint;
        private static SpringJoint rightJoint;
        private static SpringJoint leftJoint;

        public static void SpiderMonke()
        {
            var playerRB = RigUtils.MyOnlineRig.bodyCollider.attachedRigidbody;
            var rightGappleDuration = .2f; var leftGrappleDuration = .2f;
            if (Controller.rightGrab || UserInput.GetMouseButton(1))
            {
                if (!isRightGapple)
                {
                    isRightGapple = true;
                    if (Physics.Raycast(RigUtils.MyOnlineRig.rightHandTransform.position, RigUtils.MyOnlineRig.rightHandTransform.forward, out var hit))
                    {
                        rightGrapplePoint = hit.point;
                        rightJoint = RigUtils.MyOnlineRig.gameObject.GetOrAddComponent<SpringJoint>();
                        rightJoint.autoConfigureConnectedAnchor = false;
                        rightJoint.connectedAnchor = rightGrapplePoint;
                        var distance = Vector3.Distance(playerRB.position, rightGrapplePoint);
                        rightJoint.maxDistance = distance * .8f;
                        rightJoint.minDistance = distance * .25f;
                        rightJoint.spring = 10f;
                        rightJoint.damper = 50f;
                        rightJoint.massScale = 12f;
                        RigUtils.MyOnlineRig.StartCoroutine(ApllyTorque(playerRB, rightGrapplePoint, rightGappleDuration, 50f));
                        RigUtils.MyOfflineRig.PlayHandTapLocal(89, false, 999);
                    }
                }
                DrawLine(RigUtils.MyOnlineRig.rightHandTransform.position, rightGrapplePoint, Color.white, .025f);
            }
            else
            {
                isRightGapple = false;
                rightJoint.Destroy();
            }
            if (Controller.leftGrab || UserInput.GetMouseButton(0))
            {
                if (!isLeftGapple)
                {
                    isLeftGapple = true;
                    if (Physics.Raycast(RigUtils.MyOnlineRig.rightHandTransform.position, RigUtils.MyOnlineRig.rightHandTransform.forward, out var hit))
                    {
                        leftGrapplePoint = hit.point;
                        leftJoint = RigUtils.MyOnlineRig.gameObject.GetOrAddComponent<SpringJoint>();
                        leftJoint.autoConfigureConnectedAnchor = false;
                        leftJoint.connectedAnchor = leftGrapplePoint;
                        var distance = Vector3.Distance(playerRB.position, leftGrapplePoint);
                        leftJoint.maxDistance = distance * .8f;
                        leftJoint.minDistance = distance * .25f;
                        leftJoint.spring = 10f;
                        leftJoint.damper = 50f;
                        leftJoint.massScale = 12f;
                        RigUtils.MyOnlineRig.StartCoroutine(ApllyTorque(playerRB, leftGrapplePoint, leftGrappleDuration, 50f));
                        RigUtils.MyOfflineRig.PlayHandTapLocal(89, true, 999);
                    }
                }
                DrawLine(RigUtils.MyOnlineRig.leftHandTransform.position, leftGrapplePoint, Color.white, .025f);
            }
            else
            {
                isLeftGapple = false;
                leftJoint.Destroy();
            }
        }

        private static IEnumerator ApllyTorque(Rigidbody playerRB, Vector3 point, float duration, float force)
        {
            var timeElaped = 0f;
            while (timeElaped < duration)
            {
                timeElaped += Time.deltaTime;
                var direction = (point - playerRB.position).normalized;
                playerRB.AddForce(direction * force, ForceMode.Force);
                if (timeElaped < duration / 2)
                    playerRB.AddForce(direction * (force / 2), ForceMode.Force);
                yield return null;
            }
        }

        private static bool lasttouchleft;
        private static bool lasttouchright;
        public static void Pull() // thx Zav (@12swoe)
        {
            if (((!RigUtils.MyPlayer.wasLeftHandTouching && lasttouchleft) || (!RigUtils.MyPlayer.wasRightHandTouching && lasttouchright)) && Controller.rightGrab)
            {
                var pullPower = .25f;
                var vel = RigUtils.MyPlayer.GetComponent<Rigidbody>().velocity;
                RigUtils.MyPlayer.transform.position += new Vector3(vel.x * pullPower, 0f, vel.z * pullPower);
            }
            lasttouchleft = RigUtils.MyPlayer.wasLeftHandTouching;
            lasttouchright = RigUtils.MyPlayer.wasRightHandTouching;
        }
        public static void TagJoin(string did, bool alsodid)
        {
            PlayerPrefs.SetString("tutorial", did);
            PlayerPrefs.SetString("didTutorial", did);
            var hash = new Hashtable { { "didTutorial", alsodid } };
            RigUtils.MyRealtimePlayer.SetCustomProperties(hash, null, null);
            PlayerPrefs.Save();
        }

        public static GameObject[] frozone_right_network = new GameObject[9999];
        public static GameObject[] frozone_left_network = new GameObject[9999];
        public static void Frozone()
        {
            if (Controller.rightGrab || UserInput.GetMouseButton(1))
            {
                var rplat = GameObject.CreatePrimitive(PrimitiveType.Cube);
                rplat.name = "Plats";
                rplat.transform.localScale = scale;
                rplat.transform.position = RigUtils.MyPlayer.rightControllerTransform.position + (RigUtils.MyPlayer.rightControllerTransform.right * -.05f);
                rplat.transform.rotation = RigUtils.MyPlayer.rightControllerTransform.rotation;
                rplat.AddComponent<GorillaSurfaceOverride>().overrideIndex = 61;
                if (!platfromSingleColor)
                {
                    var cc = rplat.AddComponent<ColorChanger>();
                    cc.colors = new Gradient { colorKeys = platColorKeys };
                    cc.loop = true;
                } else rplat.GetComponent<Renderer>().material.color = PlatColor;
                var eventContent = new object[]
                {
                    rplat.transform.position, rplat.transform.rotation,
                    platfromSingleColor, platColorKeys, PlatColor
                };
                LegacySendEvent(90, eventContent, new RaiseEventOptions { Receivers = ReceiverGroup.Others }, true);
                Destroy(rplat, 1);
            }
            if (Controller.leftGrab || UserInput.GetMouseButton(0))
            {
                var lplat = GameObject.CreatePrimitive(PrimitiveType.Cube);
                lplat.name = "Plats";
                lplat.transform.localScale = scale;
                lplat.transform.position = RigUtils.MyPlayer.leftControllerTransform.position + (RigUtils.MyPlayer.leftControllerTransform.right * .05f);
                lplat.transform.rotation = RigUtils.MyPlayer.leftControllerTransform.rotation;
                lplat.AddComponent<GorillaSurfaceOverride>().overrideIndex = 61;
                if (!platfromSingleColor)
                {
                    var cc = lplat.AddComponent<ColorChanger>();
                    cc.colors = new Gradient { colorKeys = platColorKeys };
                    cc.loop = true;
                } else lplat.GetComponent<Renderer>().material.color = PlatColor;
                var eventContent = new object[]
                {
                    lplat.transform.position, lplat.transform.rotation,
                    platfromSingleColor, platColorKeys, PlatColor
                };
                LegacySendEvent(91, eventContent, new RaiseEventOptions { Receivers = ReceiverGroup.Others }, true);
                Destroy(lplat, 1);
            }
        }
        public static void BarkFly()
        {
            var aR = RigUtils.MyPlayer.bodyCollider.attachedRigidbody;
            aR.AddForce(-Physics.gravity * aR.mass * RigUtils.MyPlayer.scale);
            var axis = SteamVR_Actions.gorillaTag_LeftJoystick2DAxis.axis;
            var y = SteamVR_Actions.gorillaTag_RightJoystick2DAxis.axis.y;
            var vector = new Vector3(axis.x, y, axis.y);
            var forward = RigUtils.MyPlayer.bodyCollider.transform.forward;
            forward.y = 0f;
            var right = RigUtils.MyPlayer.bodyCollider.transform.right;
            right.y = 0f;
            var velo = vector.x * right + y * Vector3.up + vector.z * forward;
            velo *= RigUtils.MyPlayer.scale * flySpeed;
            aR.velocity = Vector3.Lerp(aR.velocity, velo, .01f);
        }
        private static bool antiRepeat;
        public static void TPGun()
        {
            if (CreateGun())
            {
                antiRepeat = false;
                return;
            }
            if (!antiRepeat) // this is a really old thing people did i just still like it
            {
                RigUtils.MyOnlineRig.transform.position = pointer.transform.position;
                RigUtils.MyOnlineRig.GetComponent<Rigidbody>().velocity = Vector3.zero;
                antiRepeat = true;
            }
        }
        public static void IronMonke()
        {
            if (Controller.leftGrab || UserInput.GetMouseButton(0))
            {
                RigUtils.MyOnlineRig.offlineVRRig.PlayHandTapLocal(115, false, 0.1f);
                RigUtils.MyPlayer.bodyCollider.attachedRigidbody.AddForce(10f * -RigUtils.MyOnlineRig.leftHandTransform.right, ForceMode.Acceleration);
                RigUtils.MyOnlineRig.StartVibration(true, RigUtils.MyOnlineRig.tapHapticStrength / 50f * RigUtils.MyPlayer.bodyCollider.attachedRigidbody.velocity.magnitude, RigUtils.MyOnlineRig.tapHapticDuration);
            }
            if (Controller.rightGrab || UserInput.GetMouseButton(1))
            {
                RigUtils.MyOnlineRig.offlineVRRig.PlayHandTapLocal(115, false, 0.1f);
                RigUtils.MyPlayer.bodyCollider.attachedRigidbody.AddForce(10f * RigUtils.MyOnlineRig.rightHandTransform.right, ForceMode.Acceleration);
                RigUtils.MyOnlineRig.StartVibration(false, RigUtils.MyOnlineRig.tapHapticStrength / 50f * RigUtils.MyPlayer.bodyCollider.attachedRigidbody.velocity.magnitude, RigUtils.MyOnlineRig.tapHapticDuration);
            }
        }
        public static float flySpeed = 15f;
        public static void Fly(float speed)
        {
            if (Controller.leftControllerPrimaryButton || UserInput.GetMouseButton(0))
            {
                RigUtils.MyPlayer.transform.position += RigUtils.MyPlayer.headCollider.transform.forward * Time.deltaTime * speed;
                RigUtils.MyPlayer.GetComponent<Rigidbody>().velocity = Vector3.zero;
            }
        }
        public static float speedBoostSpeed = 12f;

        public static void SetSlideControl(float control) => RigUtils.MyPlayer.slideControl = control;

        public static void SetSpeedBoost(float speed) 
        { 
            RigUtils.MyPlayer.maxJumpSpeed = speed; 
            if (GetEnabled("Super Speed Boost")) 
                RigUtils.MyPlayer.jumpMultiplier = speed;
        }

        public static void SetGravity(Vector3 gravity) => Physics.gravity = gravity;

        public static void NoClip() // i made this way more simple and it worked on other menus but not on mine so i had to do all this
        {
            if (Controller.rightControllerIndexFloat.TriggerDown() || UserInput.GetKey(KeyCode.LeftShift))
                foreach (var mesh in Resources.FindObjectsOfTypeAll<MeshCollider>())
                    mesh.enabled = false;
            else
                foreach (var mesh in Resources.FindObjectsOfTypeAll<MeshCollider>())
                    mesh.enabled = true;
        }

        public static bool platformPhysics;
        public static bool platfromSingleColor;
        public static Color PlatFirstColor = new Color32(167, 17, 237, 28);
        public static Color PlatSecondColor = Color.blue;
        public static Color PlatColor = Color.black;
        public static GradientColorKey[] platColorKeys = new GradientColorKey[4];

        public static void Platforms(bool invis, bool sticky, bool plank)
        {
            if (Controller.rightGrab || UserInput.GetMouseButton(1))
            {
                if (!once_right && jump_right_local == null)
                {
                    if (sticky)
                        jump_right_local = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    else
                        jump_right_local = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    if (invis)
                        Destroy(jump_right_local.GetComponent<Renderer>());
                    jump_right_local.transform.localScale = plank ? new Vector3(0.017f, 0.28f, 0.9999f) * RigUtils.MyPlayer.scale : scale * RigUtils.MyPlayer.scale;
                    jump_right_local.transform.position = new Vector3(0f, -0.01f, 0f) + RigUtils.MyPlayer.rightControllerTransform.position;
                    jump_right_local.transform.rotation = RigUtils.MyPlayer.rightControllerTransform.rotation;
                    jump_right_local.name = "Plats";
                    jump_right_local.layer = 0;
                    object[] eventContent = new object[]
                    {
                     new Vector3(0f, -0.0100f, 0f) + RigUtils.MyPlayer.rightControllerTransform.position,
                     RigUtils.MyPlayer.rightControllerTransform.rotation,
                     jump_right_local.transform.localScale, platfromSingleColor, platColorKeys, PlatColor, sticky, invis
                    };
                    LegacySendEvent(70, eventContent, new RaiseEventOptions { Receivers = ReceiverGroup.Others }, true);
                    once_right = true;
                    once_right_false = false;
                    if (!platfromSingleColor)
                    {
                        var colorChanger = jump_right_local.AddComponent<ColorChanger>();
                        colorChanger.colors = new Gradient { colorKeys = platColorKeys };
                        colorChanger.loop = true;
                    }
                    else
                        jump_right_local.GetComponent<Renderer>().material.color = PlatColor;
                }
            }
            else if (!once_right_false && jump_right_local != null && platformPhysics)
            {
                var prb = jump_right_local.AddComponent(typeof(Rigidbody)) as Rigidbody;
                prb.velocity = RigUtils.MyPlayer.rightHandCenterVelocityTracker.GetAverageVelocity(true, 0);
                jump_right_local.layer = 8;
                jump_right_local = null;
                once_right = false;
                once_right_false = true;
                LegacySendEvent(72, new object[] { RigUtils.MyPlayer.rightHandCenterVelocityTracker.GetAverageVelocity(true, 0), platformPhysics }, new RaiseEventOptions { Receivers = ReceiverGroup.Others }, true);
            }
            else if (!once_right_false && jump_right_local != null && !platformPhysics)
            {
                Destroy(jump_right_local);
                jump_right_local = null;
                once_right = false;
                once_right_false = true;
                LegacySendEvent(72, new object[] { RigUtils.MyPlayer.rightHandCenterVelocityTracker.GetAverageVelocity(true, 0), platformPhysics }, new RaiseEventOptions { Receivers = ReceiverGroup.Others }, true);
            }
            if (Controller.leftGrab || UserInput.GetMouseButton(0))
            {
                if (!once_left && jump_left_local == null)
                {
                    if (sticky)
                        jump_left_local = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    else
                        jump_left_local = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    if (invis)
                        Destroy(jump_left_local.GetComponent<Renderer>());
                    jump_left_local.transform.localScale = plank ? new Vector3(0.017f, 0.28f, 0.9999f) * RigUtils.MyPlayer.scale : scale * RigUtils.MyPlayer.scale;
                    jump_left_local.transform.position = new Vector3(0f, -0.01f, 0f) + RigUtils.MyPlayer.leftControllerTransform.position;
                    jump_left_local.transform.rotation = RigUtils.MyPlayer.leftControllerTransform.rotation;
                    jump_left_local.name = "Plats";
                    jump_left_local.layer = 0;
                    var eventContent = new object[]
                    {
                     new Vector3(0f, -0.0100f, 0f) + RigUtils.MyPlayer.leftControllerTransform.position,
                     RigUtils.MyPlayer.leftControllerTransform.rotation,
                     jump_left_local.transform.localScale, platfromSingleColor, platColorKeys, PlatColor, sticky, invis
                    };
                    LegacySendEvent(69, eventContent, new RaiseEventOptions { Receivers = ReceiverGroup.Others }, true);
                    once_left = true;
                    once_left_false = false;
                    if (!platfromSingleColor)
                    {
                        var colorChanger = jump_left_local.AddComponent<ColorChanger>();
                        colorChanger.colors = new Gradient { colorKeys = platColorKeys };
                        colorChanger.loop = true;
                    }
                    else
                        jump_left_local.GetComponent<Renderer>().material.color = PlatColor;
                }
            }
            else if (!once_left_false && jump_left_local != null && platformPhysics)
            {
                var prb = jump_left_local.AddComponent(typeof(Rigidbody)) as Rigidbody;
                prb.velocity = RigUtils.MyPlayer.leftHandCenterVelocityTracker.GetAverageVelocity(true, 0);
                jump_left_local.layer = 8;
                jump_left_local = null;
                once_left = false;
                once_left_false = true;
                LegacySendEvent(71, new object[] { RigUtils.MyPlayer.leftHandCenterVelocityTracker.GetAverageVelocity(true, 0), platformPhysics }, new RaiseEventOptions { Receivers = ReceiverGroup.Others }, true);
            }
            else if (!once_left_false && jump_left_local != null && !platformPhysics)
            {
                Destroy(jump_left_local);
                jump_left_local = null;
                once_left = false;
                once_left_false = true;
                LegacySendEvent(71, new object[] { RigUtils.MyPlayer.leftHandCenterVelocityTracker.GetAverageVelocity(true, 0), platformPhysics }, new RaiseEventOptions { Receivers = ReceiverGroup.Others }, true);
            }
        }

        public static Vector3 scale = new Vector3(0.0125f, 0.28f, 0.3825f);
        public static bool once_left;
        public static bool once_right;
        public static bool once_left_false;
        public static bool once_right_false;
        public static GameObject[] jump_left_network = new GameObject[9999];
        public static GameObject[] jump_right_network = new GameObject[9999];
        public static GameObject[] ball_network = new GameObject[9999];
        public static GameObject[] platgun_network = new GameObject[9999];
        public static GameObject[] ballsgun_network = new GameObject[9999];
        public static GameObject jump_left_local = null;
        public static GameObject jump_right_local = null;   
    }
}