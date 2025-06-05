using static MysticClient.Menu.Main;
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
using SColor = System.Drawing.Color;
using System.Collections;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using GorillaExtensions;

namespace MysticClient.Mods
{
    public class Movement : GunLib
    {
        public static void AirstrikeGun()
        {
            if (CreateGun(out RaycastHit hit))
            {
                antiRepeat = false;
                return;
            }
            if (!antiRepeat) 
            {
                RigUtils.MyOnlineRig.transform.position = hit.point + (Vector3.up * 30f);
                RigUtils.MyVelocity = new Vector3(0, -20f, 0);     
                antiRepeat = true;
            }
        }

        // based on iiDks AutoWalk at https://github.com/iiDk-the-actual/iis.Stupid.Menu/blob/master/Mods/Movement.cs
        public static void WalkSim() // really upset that hands dont collide while on pc
        {
            if (UserInput.GetMouseButton(1))
            {
                var look = UserInput.mousePosition - Fun.oldMousePos;
                Camera.main.transform.localEulerAngles += new Vector3(-look.y * .3f, look.x * .3f, 0);
            }
            Fun.oldMousePos = UserInput.mousePosition;
            var tr = RigUtils.MyPlayer.bodyCollider.transform;
            var rb = RigUtils.MyPlayer.bodyCollider.attachedRigidbody;
            var joystick = SteamVR_Actions.gorillaTag_LeftJoystick2DAxis.GetAxis(SteamVR_Input_Sources.LeftHand);
            var stick = joystick.magnitude > .05f ? joystick : new Vector2(
                joystick.magnitude > .05f ? joystick.x : (UserInput.GetKey(KeyCode.D) ? 1 : 0) - (UserInput.GetKey(KeyCode.A) ? 1 : 0),
                joystick.magnitude > .05f ? joystick.y : (UserInput.GetKey(KeyCode.W) ? 1 : 0) - (UserInput.GetKey(KeyCode.S) ? 1 : 0));
            var armLength = .45f; var walkSpeed = 4.5f;
            var dirocity = (tr.forward * stick.y + tr.right * stick.x).normalized;
            if (SteamVR_Actions.gorillaTag_LeftJoystickClick.GetState(SteamVR_Input_Sources.LeftHand) || UserInput.GetKey(KeyCode.LeftShift)) walkSpeed *= 1.5f;
            if (stick.magnitude > .05f)
            {
                rb.velocity = new Vector3(dirocity.x * walkSpeed, rb.velocity.y, dirocity.z * walkSpeed);
                RigUtils.MyPlayer.leftControllerTransform.position = tr.position + tr.transform.forward * (Mathf.Sin(Time.time * walkSpeed) * (stick.y * armLength)) +
                    tr.right * ((Mathf.Sin(Time.time * walkSpeed) * (stick.x * armLength)) - .2f) + 
                    new Vector3(0, -.3f + (Mathf.Cos(Time.time * walkSpeed) * .2f), 0);
                RigUtils.MyPlayer.rightControllerTransform.position = tr.position + tr.forward * (-Mathf.Sin(Time.time * walkSpeed) * (stick.y * armLength)) + 
                    tr.right * ((-Mathf.Sin(Time.time * walkSpeed) * (stick.x * armLength)) + .2f) + 
                    new Vector3(0, -.3f + (Mathf.Cos(Time.time * walkSpeed) * -.2f), 0);
            }
            if (UserInput.GetKeyDown(KeyCode.Space) || MUtils.CheckOnce(Controller.rightControllerPrimaryButton))
            {
                rb.velocity += RigUtils.MyPlayer.headCollider.transform.up * Time.deltaTime;
                rb.AddForce(Vector3.up * 85f, ForceMode.Impulse);
            }
        }

        private static bool isRightGapple = false;
        private static bool isLeftGapple = false;
        private static Vector3 rightGrapplePoint;
        private static Vector3 leftGrapplePoint;
        private static SpringJoint rightJoint;
        private static SpringJoint leftJoint;
        private static GameObject spiderPointerRight = null;
        private static GameObject spiderPointerLeft = null;

        public static void SpiderMonkeOff() { spiderPointerRight?.SetActive(false); spiderPointerLeft?.SetActive(false); }
        public static void SpiderMonke()
        {
            var playerRB = RigUtils.MyOnlineRig.bodyCollider.attachedRigidbody;
            var rightGappleDuration = .2f; var leftGrappleDuration = .2f;
            if (Controller.rightGrab || UserInput.GetMouseButton(1))
            {
                spiderPointerRight.SetActive(false);
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
                if (Physics.Raycast(RigUtils.MyOnlineRig.rightHandTransform.position, RigUtils.MyOnlineRig.rightHandTransform.forward, out var hit))
                {
                    if (spiderPointerRight == null)
                        spiderPointerRight = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    spiderPointerRight.Destroy<Collider>();
                    spiderPointerRight.transform.localScale = new Vector3(.2f, .2f, .2f);
                    spiderPointerRight.transform.position = hit.point;
                    DrawLine(RigUtils.MyOnlineRig.rightHandTransform.position, hit.point, MenuSettings.NormalColor, .025f);
                    spiderPointerRight.GetRenderer().material = TransparentMaterial(GetChangeColorA(MenuSettings.NormalColor, .5f));
                    spiderPointerRight.transform.Rotate(1f, 1f, 1f);
                    spiderPointerRight.SetActive(true);
                }
            }
            if (Controller.leftGrab || UserInput.GetMouseButton(0))
            {
                spiderPointerLeft.SetActive(false);
                if (!isLeftGapple)
                {
                    isLeftGapple = true;
                    if (Physics.Raycast(RigUtils.MyOnlineRig.leftHandTransform.position, RigUtils.MyOnlineRig.leftHandTransform.forward, out var hit))
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
                if (Physics.Raycast(RigUtils.MyOnlineRig.leftHandTransform.position, RigUtils.MyOnlineRig.leftHandTransform.forward, out var hit))
                { 
                    if (spiderPointerLeft == null)
                        spiderPointerLeft = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    spiderPointerLeft.Destroy<Collider>();
                    spiderPointerLeft.transform.localScale = new Vector3(.2f, .2f, .2f);
                    spiderPointerLeft.transform.position = hit.point;
                    DrawLine(RigUtils.MyOnlineRig.leftHandTransform.position, hit.point, MenuSettings.NormalColor, .025f);
                    spiderPointerLeft.GetRenderer().material = TransparentMaterial(GetChangeColorA(MenuSettings.NormalColor, .5f));
                    spiderPointerLeft.transform.Rotate(1f, 1f, 1f);
                    spiderPointerLeft.SetActive(true);
                }
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
                    playerRB.AddForce(direction * (force / 2), ForceMode.Impulse);
                yield return null;
            }
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
            if (CreateGun(out RaycastHit hit))
            {
                antiRepeat = false;
                return;
            }
            if (!antiRepeat) // this is a really old thing people did i just still like it
            {
                RigUtils.MyOnlineRig.transform.position = hit.point;
                RigUtils.MyOnlineRig.GetComponent<Rigidbody>().velocity = Vector3.zero;
                antiRepeat = true;
            }
        }
        public static void IronMonke()
        {
            if (Controller.leftGrab || UserInput.GetMouseButton(0))
            {
                RigUtils.MyOfflineRig.PlayHandTapLocal(115, false, .1f);
                RigUtils.MyPlayer.bodyCollider.attachedRigidbody.AddForce(10f * -RigUtils.MyOnlineRig.leftHandTransform.right, ForceMode.Acceleration);
                RigUtils.MyOnlineRig.StartVibration(true, RigUtils.MyOnlineRig.tapHapticStrength / 50f * RigUtils.MyPlayer.bodyCollider.attachedRigidbody.velocity.magnitude, RigUtils.MyOnlineRig.tapHapticDuration);
                if (GetEnabled("Fire Particles"))
                    CreateParticles(RigUtils.MyOnlineRig.leftHandTransform.position, new ParticleSystem.MinMaxGradient(new Gradient
                    {
                        colorKeys = new GradientColorKey[]
                        {
                            new GradientColorKey(new Color(1, .5f, 0), 0),
                            new GradientColorKey(new Color(1, 0, 0), .5f),
                            new GradientColorKey(new Color(.5f, 0, 0), 1)
                        },
                        alphaKeys = new GradientAlphaKey[]
                        {
                            new GradientAlphaKey(1, 0),
                            new GradientAlphaKey(.5f, .5f),
                            new GradientAlphaKey(0, 1)
                        }
                    }));
                if (GetEnabled("Fire Trails"))
                    RigUtils.MyOnlineRig.leftHandTransform.gameObject.AttachTrail(.1f, new Material(DefaultShader) { color = MenuSettings.NormalColor });
            } else RigUtils.MyOnlineRig.leftHandTransform.gameObject.Destroy<TrailRenderer>();
            if (Controller.rightGrab || UserInput.GetMouseButton(1))
            {
                RigUtils.MyOfflineRig.PlayHandTapLocal(115, false, .1f);
                RigUtils.MyPlayer.bodyCollider.attachedRigidbody.AddForce(10f * RigUtils.MyOnlineRig.rightHandTransform.right, ForceMode.Acceleration);
                RigUtils.MyOnlineRig.StartVibration(false, RigUtils.MyOnlineRig.tapHapticStrength / 50f * RigUtils.MyPlayer.bodyCollider.attachedRigidbody.velocity.magnitude, RigUtils.MyOnlineRig.tapHapticDuration);
                if (GetEnabled("Fire Particles"))
                    CreateParticles(RigUtils.MyOnlineRig.rightHandTransform.position, new ParticleSystem.MinMaxGradient(new Gradient
                    {
                        colorKeys = new GradientColorKey[]
                        {
                            new GradientColorKey(new Color(1, .5f, 0), 0),
                            new GradientColorKey(new Color(1, 0, 0), .5f),
                            new GradientColorKey(new Color(.5f, 0, 0), 1)
                        },
                        alphaKeys = new GradientAlphaKey[]
                        {
                            new GradientAlphaKey(1, 0),
                            new GradientAlphaKey(.5f, .5f),
                            new GradientAlphaKey(0, 1)
                        }
                    }));
                if (GetEnabled("Fire Trails"))
                    RigUtils.MyOnlineRig.rightHandTransform.gameObject.AttachTrail(.1f, new Material(DefaultShader) { color = MenuSettings.NormalColor });
            } else RigUtils.MyOnlineRig.rightHandTransform.gameObject.Destroy<TrailRenderer>();
        }
        public static float flySpeed = 15f;
        public static void Fly()
        {
            if (Controller.leftControllerPrimaryButton || UserInput.GetMouseButton(0))
            {
                RigUtils.MyPlayer.transform.position += RigUtils.MyPlayer.headCollider.transform.forward * Time.deltaTime * flySpeed;
                RigUtils.MyPlayer.GetComponent<Rigidbody>().velocity = Vector3.zero;
            }
        }
        public static void VelocityFly()
        {
            if (Controller.leftControllerPrimaryButton || UserInput.GetMouseButton(0))
                RigUtils.MyVelocity += RigUtils.MyPlayer.headCollider.transform.forward * Time.deltaTime * flySpeed;
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
                    if (sticky) jump_right_local = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    else jump_right_local = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    if (invis) Destroy(jump_right_local.GetComponent<Renderer>());
                    jump_right_local.transform.localScale = plank ? new Vector3(.017f, .28f, .9999f) * RigUtils.MyPlayer.scale : scale *= RigUtils.MyPlayer.scale;
                    jump_right_local.transform.position = new Vector3(0, -.01f, 0) + RigUtils.MyPlayer.rightControllerTransform.position;
                    jump_right_local.transform.rotation = RigUtils.MyPlayer.rightControllerTransform.rotation;
                    jump_right_local.name = "Plats";
                    jump_right_local.layer = 0;
                    //if (GetEnabled("Round Menu")) RoundOtherObject(jump_right_local);
                    object[] eventContent = 
                    {
                     new Vector3(0, -.0100f, 0) + RigUtils.MyPlayer.rightControllerTransform.position,
                     RigUtils.MyPlayer.rightControllerTransform.rotation,
                     jump_right_local.transform.localScale, platfromSingleColor, platColorKeys, PlatColor, sticky, invis//, GetEnabled("Round Menu")
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
                    else jump_right_local.GetComponent<Renderer>().material.color = PlatColor;
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
                    if (sticky) jump_left_local = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    else jump_left_local = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    if (invis) Destroy(jump_left_local.GetComponent<Renderer>());
                    jump_left_local.transform.localScale = plank ? new Vector3(.017f, .28f, .9999f) * RigUtils.MyPlayer.scale : scale * RigUtils.MyPlayer.scale;
                    jump_left_local.transform.position = new Vector3(0, -.01f, 0) + RigUtils.MyPlayer.leftControllerTransform.position;
                    jump_left_local.transform.rotation = RigUtils.MyPlayer.leftControllerTransform.rotation;
                    jump_left_local.name = "Plats";
                    jump_left_local.layer = 0;
                    //if (GetEnabled("Round Menu")) RoundOtherObject(jump_left_local);
                    object[] eventContent =
                    {
                     new Vector3(0, -.0100f, 0) + RigUtils.MyPlayer.leftControllerTransform.position,
                     RigUtils.MyPlayer.leftControllerTransform.rotation,
                     jump_left_local.transform.localScale, platfromSingleColor, platColorKeys, PlatColor, sticky, invis//, GetEnabled("Round Menu")
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
                    else jump_left_local.GetComponent<Renderer>().material.color = PlatColor;
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

        public static Vector3 scale = new Vector3(.0125f, .28f, .3825f);
        public static bool once_left;
        public static bool once_right;
        public static bool once_left_false;
        public static bool once_right_false;
        public static GameObject[] jump_left_network = new GameObject[9999];
        public static GameObject[] jump_right_network = new GameObject[9999];
        public static GameObject[] platgun_network = new GameObject[9999];
        public static GameObject[] ballsgun_network = new GameObject[9999];
        public static GameObject jump_left_local = null;
        public static GameObject jump_right_local = null;   
    }
}