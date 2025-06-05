using MysticClient.Utils;
using static MysticClient.Menu.Main;
using UnityEngine;
using UnityEngine.XR;
using Valve.VR;

namespace MysticClient.UISetting
{
    public class UIInputs : MonoBehaviour
    {
        public static volatile UIInputs instance;

        void Awake() => instance = this;

        public bool LeftStickDown
        {
            get
            {
                bool output;
                if (GameObject.Find("[SteamVR]") != null) output = SteamVR_Actions.gorillaTag_LeftJoystickClick.GetState(SteamVR_Input_Sources.LeftHand);
                else InputDevices.GetDeviceAtXRNode(XRNode.LeftHand).TryGetFeatureValue(CommonUsages.primary2DAxisClick, out output);
                return output;
            }
        }

        public Vector2 LeftStickAxis
        {
            get
            {
                Vector2 output;
                if (GameObject.Find("[SteamVR]") != null) output = SteamVR_Actions.gorillaTag_LeftJoystick2DAxis.axis;
                else InputDevices.GetDeviceAtXRNode(XRNode.LeftHand).TryGetFeatureValue(CommonUsages.primary2DAxis, out output);
                return output;
            }
        }

        public bool LeftGripDown => Controller.leftGrab;
        public bool LeftTriggerDown => Controller.leftControllerIndexFloat.TriggerDown();
        public bool LeftPrimaryDown => Controller.leftControllerPrimaryButton;
        public bool LeftSecondaryDown => Controller.leftControllerSecondaryButton;

        public bool RightStickDown
        {
            get
            {
                bool output;
                if (GameObject.Find("[SteamVR]") != null) output = SteamVR_Actions.gorillaTag_RightJoystickClick.GetState(SteamVR_Input_Sources.RightHand);
                else InputDevices.GetDeviceAtXRNode(XRNode.RightHand).TryGetFeatureValue(CommonUsages.primary2DAxisClick, out output);
                return output;
            }
        }

        public Vector2 RightStickAxis
        {
            get
            {
                Vector2 output;
                if (GameObject.Find("[SteamVR]") != null) output = SteamVR_Actions.gorillaTag_RightJoystick2DAxis.axis;
                else InputDevices.GetDeviceAtXRNode(XRNode.RightHand).TryGetFeatureValue(CommonUsages.primary2DAxis, out output);
                return output;
            }
        }

        public bool RightGripDown => Controller.rightGrab;
        public bool RightTriggerDown => Controller.rightControllerIndexFloat.TriggerDown();
        public bool RightPrimaryDown => Controller.rightControllerPrimaryButton;
        public bool RightSecondaryDown => Controller.rightControllerSecondaryButton;
    }
}