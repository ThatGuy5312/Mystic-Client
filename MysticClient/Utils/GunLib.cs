using BepInEx;
using MysticClient.Menu;
using System.Collections;
using System.ComponentModel.Design;
using System.Runtime.CompilerServices;
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
        private static float pointerDelay = 10f;
        private static float bendAmount = .5f;
        public static bool normalGun;
        public static bool CreateGun(out VRRig rig)
        {
            bool gunExists = CreateGun(leftHandGun, Main.UserInput.GetMouseButton(1), gunShape, out RaycastResult);
            rig = (RaycastResult.collider.GetComponentInParent<VRRig>() && gunExists) ? RaycastResult.collider.GetComponentInParent<VRRig>() : null;
            if (rig && rig != RigUtils.MyOfflineRig && pointer.activeSelf)
                return CheckInputs();
            return false;
        }
        public static bool CreateGun(out RaycastHit hit, out VRRig rig)
        {
            bool gunExists = CreateGun(leftHandGun, Main.UserInput.GetMouseButton(1), gunShape, out hit);
            rig = (RaycastResult.collider.GetComponentInParent<VRRig>() && gunExists) ? RaycastResult.collider.GetComponentInParent<VRRig>() : null;
            if (rig && rig != RigUtils.MyOfflineRig && pointer.activeSelf)
                return CheckInputs();
            return false;
        }
        public static bool CreateGun() // this is so simple i love it
        {
            CreateGun(leftHandGun, Main.UserInput.GetMouseButton(1), gunShape, out RaycastResult);
            return pointer.activeSelf && CheckInputs();
        }
        public static bool CreateGun(out RaycastHit hit)
        {
            CreateGun(leftHandGun, Main.UserInput.GetMouseButton(1), gunShape, out hit);
            return pointer.activeSelf && CheckInputs();
        }
        public static void LockOnRig(VRRig rig)
        {
            pointer.transform.position = rig.head.headTransform.position;
        }
        private static bool CreateGun(bool leftHand, bool OnPC, PrimitiveType shape, out RaycastHit hit)
        {
            InitializeGun(shape);
            if (OnPC)
            {
                if (!Main.UserInput.GetMouseButton(1))
                {
                    pointer.SetActive(false);
                    LineRender.enabled = false;
                    Physics.Raycast(Main.mainCamera.ScreenPointToRay(Main.UserInput.mousePosition), out hit);
                    return false;
                }
                var hand = leftHand ? RigUtils.MyOnlineRig.leftHandTransform : RigUtils.MyOnlineRig.rightHandTransform;
                if (Physics.Raycast(Main.mainCamera.ScreenPointToRay(Main.UserInput.mousePosition), out hit))
                {
                    RenderGun(hit, hand);
                    return true;
                }
            }
            else
            {
                var hand = leftHand ? RigUtils.MyOnlineRig.leftHandTransform : RigUtils.MyOnlineRig.rightHandTransform;
                var ray = leftHand ? -hand.up : -hand.up;
                if (leftHand ? !Main.Controller.leftGrab : !Main.Controller.rightGrab)
                {
                    pointer.SetActive(false);
                    LineRender.enabled = false;
                    Physics.Raycast(hand.position, ray, out hit);
                    return false;
                }
                if (Physics.Raycast(hand.position, ray, out hit))
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
        private static void RenderGun(RaycastHit hit, Transform hand)
        {
            targetPos = hit.point;
            pointer.transform.position = normalGun ? hit.point : Vector3.Lerp(pointer.transform.position, targetPos, Time.deltaTime * pointerDelay);
            Vector3 midPos = Vector3.Lerp(hand.position, pointer.transform.position, 0.5f) + Vector3.up * bendAmount;
            LineRender.positionCount = normalGun ? 2 : 3;
            LineRender.SetPosition(0, hand.position);
            if(!normalGun)
                LineRender.SetPosition(1, midPos);
            LineRender.SetPosition(normalGun ? 1 : 2, pointer.transform.position);
            pointer.SetActive(true);
            LineRender.enabled = true;
            pointer.transform.Rotate(.8f, .8f, .8f);
            pointer.transform.localScale = Vector3.Lerp(new Vector3(0.2f, 0.2f, 0.2f), new Vector3(0.3f, 0.3f, 0.3f), Mathf.PingPong(Time.time, 1));
        }
        private static bool CheckInputs()
        {
            bool on = leftHandGun ? ControllerInputPoller.instance.leftControllerIndexFloat > 0.5f :
                ControllerInputPoller.instance.rightControllerIndexFloat > 0.5f ||
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
    }
}