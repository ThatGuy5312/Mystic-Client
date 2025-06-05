using MysticClient.Utils;
using UnityEngine;
using Controller = OVRInput.Controller;

namespace MysticClient.Classes
{
    // i have a feeling they will change the input system again just like when they did on 9 / 11 of 2023 so im just being safe by creating a easy to switch backup one in case of a switch over
    public class BackupInputs : MonoBehaviour
    {
        public static volatile BackupInputs instance;

        public float leftControllerIndexFloat;
        public float leftControllerGripFloat;
        public float rightControllerIndexFloat;
        public float rightControllerGripFloat;
        public float leftControllerIndexTouch;
        public float rightControllerIndexTouch;

        public Vector3 rightControllerPosition;
        public Vector3 leftControllerPosition;
        public Vector3 rightControllerVelocity;
        public Vector3 leftControllerVelocity;
        public Vector3 rightControllerAngularVelocity;
        public Vector3 leftControllerAngularVelocity;

        public Quaternion leftControllerRotation;
        public Quaternion rightControllerRotation;

        public bool leftControllerPrimaryButton;
        public bool leftControllerSecondaryButton;
        public bool rightControllerPrimaryButton;
        public bool rightControllerSecondaryButton;
        public bool leftControllerPrimaryButtonTouch;
        public bool leftControllerSecondaryButtonTouch;
        public bool rightControllerPrimaryButtonTouch;
        public bool rightControllerSecondaryButtonTouch;
        public bool leftGrab;
        public bool rightGrab;

        public Vector2 leftControllerPrimary2DAxis;
        public Vector2 rightControllerPrimary2DAxis;

        void Awake() => instance = this;

        void Update()
        {
            rightControllerPosition = OVRInput.GetLocalControllerPosition(Controller.RHand);
            leftControllerPosition = OVRInput.GetLocalControllerPosition(Controller.LHand);

            rightControllerRotation = OVRInput.GetLocalControllerRotation(Controller.RHand);
            leftControllerRotation = OVRInput.GetLocalControllerRotation(Controller.LHand);

            rightControllerPrimary2DAxis = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, Controller.RHand);
            leftControllerPrimary2DAxis = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, Controller.LHand);

            rightControllerPrimaryButton = OVRInput.Get(OVRInput.NearTouch.PrimaryThumbButtons, Controller.RHand);
            rightControllerSecondaryButton = OVRInput.Get(OVRInput.NearTouch.SecondaryThumbButtons, Controller.RHand);

            leftControllerPrimaryButton = OVRInput.Get(OVRInput.NearTouch.PrimaryThumbButtons, Controller.LHand);
            leftControllerSecondaryButton = OVRInput.Get(OVRInput.NearTouch.SecondaryThumbButtons, Controller.LHand);

            rightControllerPrimaryButtonTouch = OVRInput.Get(OVRInput.NearTouch.PrimaryThumbButtons, Controller.RTouch);
            rightControllerSecondaryButtonTouch = OVRInput.Get(OVRInput.NearTouch.SecondaryThumbButtons, Controller.RTouch);

            leftControllerPrimaryButtonTouch = OVRInput.Get(OVRInput.NearTouch.PrimaryThumbButtons, Controller.LTouch);
            leftControllerSecondaryButtonTouch = OVRInput.Get(OVRInput.NearTouch.SecondaryThumbButtons, Controller.LTouch);

            rightControllerGripFloat = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, Controller.RHand);
            leftControllerGripFloat = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, Controller.LHand);

            rightControllerIndexFloat = OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, Controller.RHand);
            leftControllerIndexFloat = OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, Controller.LHand);

            rightGrab = OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, Controller.RHand).TriggerDown();
            leftGrab = OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, Controller.LHand).TriggerDown();

            rightControllerVelocity = OVRInput.GetLocalControllerVelocity(Controller.RHand);
            leftControllerVelocity = OVRInput.GetLocalControllerVelocity(Controller.LHand);

            rightControllerAngularVelocity = OVRInput.GetLocalControllerAngularVelocity(Controller.RHand);
            leftControllerAngularVelocity = OVRInput.GetLocalControllerAngularVelocity(Controller.LHand);
        }
    }
}