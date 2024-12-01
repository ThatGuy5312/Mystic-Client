using BepInEx;
using MysticClient.Menu;
using System.Collections;
using System.ComponentModel.Design;
using System.Runtime.CompilerServices;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace MysticClient.Utils
{
    public class GunLib : MonoBehaviour
    {
        public static bool leftHandGun;
        public static PrimitiveType gunShape = PrimitiveType.Cube;
        public static GameObject pointer = null;
        public static RaycastHit RaycastResult;
        public static Color enabledColor = Color.black;
        public static Color disabledColor = Color.black;
        private static LineRenderer LineRender = null;
        private static Vector3 targetPos;
        private static float pointerDelay = 15f;
        private static float bendAmount = .5f;
        public static int gunType;
        public static bool oldRayDirection;
        public static bool CreateGun(out VRRig rig)
        {
            var created = CreateGun(gunShape, out RaycastResult);
            rig = (RaycastResult.collider.GetComponentInParent<VRRig>() && created) ? RaycastResult.collider.GetComponentInParent<VRRig>() : null;
            if (rig && rig != RigUtils.MyOfflineRig && pointer.activeSelf)
                return CheckInputs();
            return false;
        }
        public static bool CreateGun(out RaycastHit hit, out VRRig rig)
        {
            var created = CreateGun(gunShape, out hit);
            rig = (RaycastResult.collider.GetComponentInParent<VRRig>() && created) ? RaycastResult.collider.GetComponentInParent<VRRig>() : null;
            if (rig && rig != RigUtils.MyOfflineRig && pointer.activeSelf)
                return CheckInputs();
            return false;
        }
        public static bool CreateGun() // this is so simple i love it
        {
            CreateGun(gunShape, out RaycastResult);
            return pointer.activeSelf && CheckInputs();
        }
        public static bool CreateGun(out RaycastHit hit)
        {
            CreateGun(gunShape, out hit);
            return pointer.activeSelf && CheckInputs();
        }
        public static bool CreateGun(bool customInput)
        {
            CreateGun(gunShape, out RaycastResult);
            return pointer.activeSelf && customInput;
        }
        public static bool CreateGun(bool customInput, out RaycastHit hit)
        {
            CreateGun(gunShape, out hit);
            return pointer.activeSelf && customInput;
        }
        public static bool CreateGun(bool customInput, out VRRig rig)
        {
            var created = CreateGun(gunShape, out RaycastResult);
            rig = (RaycastResult.collider.GetComponentInParent<VRRig>() && created) ? RaycastResult.collider.GetComponentInParent<VRRig>() : null;
            if (rig && rig != RigUtils.MyOfflineRig && pointer.activeSelf)
                return customInput;
            return false;
        }
        public static bool CreateGun(bool customInput, out RaycastHit hit, out VRRig rig)
        {
            var created = CreateGun(gunShape, out hit);
            rig = (RaycastResult.collider.GetComponentInParent<VRRig>() && created) ? RaycastResult.collider.GetComponentInParent<VRRig>() : null;
            if (rig && rig != RigUtils.MyOfflineRig && pointer.activeSelf)
                return customInput;
            return false;
        }
        public static void LockOnRig(VRRig rig)
        {
            pointer.transform.position = rig.head.headTransform.position;
        }
        private static bool CreateGun(PrimitiveType shape, out RaycastHit hit)
        {
            InitializeGun(shape);
            if (Main.UserInput.GetMouseButton(1))
            {
                if (!Main.UserInput.GetMouseButton(1))
                {
                    pointer.SetActive(false);
                    LineRender.enabled = false;
                    Physics.Raycast(Main.mainCamera.ScreenPointToRay(Main.UserInput.mousePosition), out hit);
                    return false;
                }
                var hand = leftHandGun ? RigUtils.MyOnlineRig.leftHandTransform : RigUtils.MyOnlineRig.rightHandTransform;
                if (Physics.Raycast(Main.mainCamera.ScreenPointToRay(Main.UserInput.mousePosition), out hit))
                {
                    RenderGun(hit, hand);
                    return true;
                }
            }
            else
            {
                var hand = leftHandGun ? RigUtils.MyOnlineRig.leftHandTransform : RigUtils.MyOnlineRig.rightHandTransform;
                var newRay = leftHandGun ? hand.forward : hand.forward;
                var oldRay = leftHandGun ? -hand.up : -hand.up;
                if (leftHandGun ? !Main.Controller.leftGrab : !Main.Controller.rightGrab)
                {
                    pointer.SetActive(false);
                    LineRender.enabled = false;
                    Physics.Raycast(hand.position, oldRayDirection ? oldRay : newRay, out hit);
                    return false;
                }
                if (Physics.Raycast(hand.position, oldRayDirection ? oldRay : newRay, out hit))
                {
                    RenderGun(hit, hand);
                    return true;
                }
            }
            return false;
        }
        private static void InitializeGun(PrimitiveType shape)
        {
            if (pointer == null)
            {
                pointer = GameObject.CreatePrimitive(shape);
                pointer.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                pointer.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                Destroy(pointer.GetComponent<Rigidbody>());
                Destroy(pointer.GetComponent<Collider>());
                pointer.SetActive(false);
                LineRender = pointer.AddComponent<LineRenderer>();
                LineRender.startWidth = 0.025f;
                LineRender.endWidth = 0.025f;
                LineRender.positionCount = 3;
                LineRender.useWorldSpace = true;
                LineRender.material.shader = Shader.Find("GUI/Text Shader");
                LineRender.enabled = false;
            }
        }
        private static int pointCount = 50;
        private static void RenderGun(RaycastHit hit, Transform hand)
        {
            targetPos = hit.point;
            pointer.transform.position = (gunType == 1) ? hit.point : Vector3.Lerp(pointer.transform.position, targetPos, Time.deltaTime * pointerDelay);
            if (gunType == 1)
            {
                LineRender.positionCount = 2;
                LineRender.SetPosition(0, hand.position);
                LineRender.SetPosition(1, pointer.transform.position);
            }
            else if (gunType == 0)
            {
                Vector3 midPos = Vector3.Lerp(hand.position, pointer.transform.position, 0.5f) + Vector3.up * bendAmount;
                LineRender.positionCount = 3;
                LineRender.SetPosition(0, hand.position);
                LineRender.SetPosition(1, midPos);
                LineRender.SetPosition(2, pointer.transform.position);
            }
            else if (gunType == 2)
            {
                var curvePoints = new Vector3[pointCount];
                for (int i = 0; i < pointCount; i++)
                {
                    var t = i / (float)(pointCount - 1);
                    curvePoints[i] = CalculatePoint(t, hand.position, GetMiddle(hand.position + hit.point), pointer.transform.position);
                }
                LineRender.positionCount = pointCount;
                LineRender.SetPositions(curvePoints);
            }
            pointer.SetActive(true);
            LineRender.enabled = true;
            pointer.transform.Rotate(0.8f, 0.8f, 0.8f);
            pointer.transform.localScale = Vector3.Lerp(
                new Vector3(0.2f, 0.2f, 0.2f),
                new Vector3(0.3f, 0.3f, 0.3f),
                Mathf.PingPong(Time.time, 1));
        }
        private static bool CheckInputs()
        {
            bool on = leftHandGun ? Main.Controller.leftControllerIndexFloat > 0.5f :
                Main.Controller.rightControllerIndexFloat > 0.5f ||
                Main.UserInput.GetMouseButton(0);
            if (on)
            {
                SetGunColor(enabledColor);
            } else { SetGunColor(disabledColor); }
            return on;
        }
        private static void SetGunColor(Color color)
        {
            pointer.GetComponent<Renderer>().material.color = color;
            LineRender.startColor = color;
            LineRender.endColor = color;
            LineRender.material.color = color;
        }
        private static Vector3 CalculatePoint(float t, Vector3 start, Vector3 mid, Vector3 end) => Mathf.Pow(1 - t, 2) * start + 2 * (1 - t) * t * mid + Mathf.Pow(t, 2) * end;
        private static Vector3 GetMiddle(Vector3 vector) => new Vector3(vector.x / 2f, vector.y / 2f, vector.z / 2f);
    }
}