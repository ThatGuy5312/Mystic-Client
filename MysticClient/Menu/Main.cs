using BepInEx;
using GorillaLocomotion.Gameplay;
using GorillaNetworking;
using HarmonyLib;
using MysticClient.Classes;
using Unity.XR.CoreUtils;
using MysticClient.Utils;
using Photon.Pun;
using PlayFab.ClientModels;
using PlayFab;
using System;
using ExitGames.Client.Photon;
using Photon.Realtime;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using WebSocketSharp;
using static MysticClient.Menu.Buttons;
using static MysticClient.Menu.MenuSettings;
using MysticClient.Mods;
using TMPro;
using BepInEx.Bootstrap;

namespace MysticClient.Menu
{
    [HarmonyPatch(typeof(GorillaLocomotion.Player), "LateUpdate")]
    public class Main : MonoBehaviour
    {
        public static GorillaPaintbrawlManager GorillaPaintbrawlManager { get { return GameObject.Find("Gorilla Battle Manager").GetComponent<GorillaPaintbrawlManager>(); } }
        public static GorillaTagManager GorillaTagManager { get { return GameObject.Find("Gorilla Tag Manager").GetComponent<GorillaTagManager>(); } }
        public static NetworkSystem PhotonSystem { get { return NetworkSystem.Instance; } }
        public static ControllerInputPoller Controller { get { return ControllerInputPoller.instance; } }
        public static IInputSystem UserInput { get { return UnityInput.Current; } }

        private const string menuIncompatibility = "org.zyn.gorillatag.jigh";
        // my friend didnt want to use my menu but he wanted to use the gui while using his menu so i added this

        public static void Prefix()
        {
            try
            {
                bool toOpen = (!GetEnabled("Right Hand Menu") && Controller.leftControllerSecondaryButton) || (GetEnabled("Right Hand Menu") && Controller.rightControllerSecondaryButton);
                bool keyboardOpen = UserInput.GetKey(KeyCode.Q);

                if (menu == null && !Chainloader.PluginInfos.ContainsKey(menuIncompatibility))
                {
                    if (toOpen || keyboardOpen)
                    {
                        CreateMenu();
                        RecenterMenu(GetEnabled("Right Hand Menu"), keyboardOpen);
                        if (reference == null)
                        {
                            CreateReference(GetEnabled("Right Hand Menu"));
                        }
                    }
                }
                else
                {
                    if ((toOpen || keyboardOpen))
                    {
                        RecenterMenu(GetEnabled("Right Hand Menu"), keyboardOpen);
                        if (reference == null)
                        {
                            CreateReference(GetEnabled("Right Hand Menu"));
                        }
                    }
                    else if (menu.GetComponent<Rigidbody>() == null)
                    {
                        if (physicSetting == 1 || physicSetting == 2)
                        {
                            var menuRB = menu.AddComponent(typeof(Rigidbody)) as Rigidbody;
                            menuRB.detectCollisions = true;
                            menuRB.isKinematic = false;
                            var Spin = new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f)).normalized * 175f;
                            var tracker = GetEnabled("Right Hand Menu") ? RigUtils.MyPlayer.rightHandCenterVelocityTracker : RigUtils.MyPlayer.leftHandCenterVelocityTracker;
                            menuRB.useGravity = !GetEnabled("Zero Gravity Menu");
                            menu.GetComponent<Rigidbody>().AddTorque(UnityEngine.Random.insideUnitSphere * 275f);
                            menu.GetComponent<Rigidbody>().AddTorque(Spin);
                            menuRB.velocity = tracker.GetAverageVelocity(true, 0f);
                            Destroy(reference);
                            reference = null;
                            if (GetEnabled("Multi Create")) // fix this later
                            {
                                Destroy(menu, GetEnabled("Multi Create") ? destroyDelay : 0);
                                menu = null;
                            }
                        }
                        else if (physicSetting == 0)
                        {
                            DestroyMenu();
                        }
                    }
                }
                try
                {
                    if (!RigUtils.MyOfflineRig.enabled)
                    {
                        if (Rig.GhostType == 1)
                        {
                            var right = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                            right.transform.position = RigUtils.MyOnlineRig.rightHandTransform.position;
                            right.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f) * RigUtils.MyPlayer.scale;
                            right.transform.GetComponent<Renderer>().material.color = FirstColor;
                            Destroy(right.GetComponent<Collider>());
                            var left = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                            left.transform.position = RigUtils.MyOnlineRig.leftHandTransform.position;
                            left.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f) * RigUtils.MyPlayer.scale;
                            left.transform.GetComponent<Renderer>().material.color = SecondColor;
                            Destroy(left.GetComponent<Collider>());
                            Destroy(right, Time.deltaTime);
                            Destroy(left, Time.deltaTime);
                        }
                        else if (Rig.GhostType == 2)
                        {
                            if (Ghost == null)
                            {
                                Ghost = Instantiate(RigUtils.MyOfflineRig, RigUtils.MyPlayer.transform.position, RigUtils.MyPlayer.transform.rotation);
                                Ghost.headBodyOffset = Vector3.zero;
                                Ghost.enabled = true;
                                Ghost.transform.Find("VR Constraints/LeftArm/Left Arm IK/SlideAudio").gameObject.SetActive(false);
                                Ghost.transform.Find("VR Constraints/RightArm/Right Arm IK/SlideAudio").gameObject.SetActive(false);
                            }
                            var col = NormalColor;
                            col.a = .5f;
                            Ghost.mainSkin.material.color = col;
                            Ghost.mainSkin.material.shader = Shader.Find("GUI/Text Shader");
                            Ghost.headConstraint.transform.position = RigUtils.MyPlayer.headCollider.transform.position;
                            Ghost.headConstraint.transform.rotation = RigUtils.MyPlayer.headCollider.transform.rotation;
                            Ghost.rightHandTransform.transform.position = RigUtils.MyPlayer.rightControllerTransform.position;
                            Ghost.rightHandTransform.transform.rotation = RigUtils.MyPlayer.rightControllerTransform.rotation;
                            Ghost.leftHandTransform.transform.position = RigUtils.MyPlayer.leftControllerTransform.position;
                            Ghost.leftHandTransform.transform.rotation = RigUtils.MyPlayer.leftControllerTransform.rotation;
                            Ghost.transform.position = RigUtils.MyPlayer.transform.position;
                            Ghost.transform.rotation = RigUtils.MyPlayer.transform.rotation;
                        }
                    }
                    else { if (Ghost.gameObject != null) { Destroy(Ghost.gameObject); Ghost = null; } }
                }
                catch { }
                try
                {
                    // i know tpc does the same thing but this just works better and i got it from old mystic client and im not changing it
                    if (GameObject.Find("Third Person Camera"))
                        mainCamera = GameObject.Find("Shoulder Camera").GetComponent<Camera>();
                    else
                        mainCamera = Camera.main;
                }
                catch { }
                try
                {
                    if (UserInput.GetMouseButton(0) && !Chainloader.PluginInfos.ContainsKey("org.thatguy.gorillatag.mysticguiex"))
                    {
                        RaycastHit hit;
                        Physics.Raycast(mainCamera.ScreenPointToRay(UserInput.mousePosition), out hit);
                        GameObject.Find("Player Objects/Player VR Controller/GorillaPlayer/TurnParent/LeftHandTriggerCollider").transform.position = hit.point;
                        GameObject.Find("Player Objects/Player VR Controller/GorillaPlayer/TurnParent/LeftHandTriggerCollider").GetComponent<TransformFollow>().enabled = false;
                    } else GameObject.Find("Player Objects/Player VR Controller/GorillaPlayer/TurnParent/LeftHandTriggerCollider").GetComponent<TransformFollow>().enabled = true;
                }
                catch { }
                try
                {
                    GameObject.Find("CodeOfConduct").GetComponent<TMP_Text>().text = "Credits";
                    GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/motd (1)").GetComponent<TMP_Text>().text = "Updates";
                    GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/motdtext").GetComponent<TMP_Text>().text = MOTDText;
                    GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomBoundaryStones/BoundaryStoneSet_Forest/wallmonitorforestbg").GetComponent<Renderer>().material.color = Color.black;
                    GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/COC Text").GetComponent<TMP_Text>().text = Credits;
                    GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomInteractables/GorillaComputerObject/ComputerUI/monitor/monitorScreen").GetComponent<Renderer>().material.color = Color.black;
                    GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomInteractables/GorillaComputerObject/ComputerUI/monitor").GetComponent<Renderer>().material.color = Color.blue;
                    GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomInteractables/GorillaComputerObject/ComputerUI/keyboard (1)").GetComponent<Renderer>().material.color = Color.magenta;
                    int find = 0;
                    for (int i = 0; i < GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom").transform.childCount; i++)
                    {
                        var boards = GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom").transform.GetChild(i).gameObject;
                        if (boards.name.Contains("forestatlas"))
                        {
                            find++;
                            if (find == 2)
                            {
                                boards.GetComponent<Renderer>().material.color = Color.black;
                            }
                        }
                    }
                    find = 0;
                    for (int i = 0; i < GameObject.Find("Environment Objects/LocalObjects_Prefab/Forest").transform.childCount; i++)
                    {
                        var boards = GameObject.Find("Environment Objects/LocalObjects_Prefab/Forest").transform.GetChild(i).gameObject;
                        if (boards.name.Contains("forestatlas"))
                        {
                            find++;
                            if (find == 4)
                            {
                                boards.GetComponent<Renderer>().material.color = Color.black;
                            }
                        }
                    }
                    for (int i = 0; i < GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomInteractables/GorillaComputerObject/ComputerUI/keyboard (1)/Buttons/Keys").transform.childCount; i++)
                    {
                        var keys = GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomInteractables/GorillaComputerObject/ComputerUI/keyboard (1)/Buttons/Keys").transform.GetChild(i).gameObject;
                        keys.GetComponent<Renderer>().material.color = Color.cyan;
                        var btn = keys.GetComponent<GorillaKeyboardButton>().ButtonColorSettings;
                        btn.UnpressedColor = Color.cyan;
                        btn.PressedColor = Color.magenta;
                    }
                } catch { }
                try
                {
                    if (buttonEnableSound == null)
                        buttonEnableSound = Loaders.GetAudioFromURL("https://drive.google.com/uc?export=download&id=1ikd7poEUHG84V7mgwTwWZ7I1QPp_rDIL");
                    if (buttonDisableSound == null)
                        buttonDisableSound = Loaders.GetAudioFromURL("https://drive.google.com/uc?export=download&id=1KTBZ-I7WdyZDyOioReq2dMxgvcJEnvWe");
                    Movement.platColorKeys[0].color = Movement.PlatFirstColor;
                    Movement.platColorKeys[0].time = 0f;
                    Movement.platColorKeys[1].color = Movement.PlatSecondColor;
                    Movement.platColorKeys[1].time = 0.3f;
                    Movement.platColorKeys[2].color = Movement.PlatFirstColor;
                    Movement.platColorKeys[2].time = 0.6f;
                    Movement.platColorKeys[3].color = Movement.PlatSecondColor;
                    Movement.platColorKeys[3].time = 1f;
                }
                catch { }
            }
            catch { }
            try 
            {
                foreach (var btns in buttons)
                    foreach (var btn in btns)
                        if (btn.enabled)
                            if (btn.method != null)
                            {
                                try { btn.method.Invoke(); }
                                catch (Exception exc) { Debug.LogError(string.Format("{0} // Error with mod {1} at {2}: {3}", PluginInfo.Name, btn.buttonText, exc.StackTrace, exc.Message)); }
                            }
            }
            catch (Exception exc) { Debug.LogError(string.Format("{0} // Error with executing mods at {1}: {2}", PluginInfo.Name, exc.StackTrace, exc.Message)); }
        }
        private static AudioClip buttonEnableSound = null;
        private static AudioClip buttonDisableSound = null;
        //private static bool launched;
        /*private static void OnLaunch()
        {
            Debug.Log("OnLaunch has been called!");
            launched = true;
            var planet = Loaders.LoadAsset("MysticClient.Resources.adminplanet").LoadAsset<GameObject>("Planet"); i did have a asset in here but it wouldnt load so i got rig of it do to file size
            planet.transform.position = RigUtils.MyOfflineRig.rightHandTransform.position;
            planet.transform.SetParent(RigUtils.MyOfflineRig.rightHandTransform, false);
            var rig = GameObject.Find("Player Objects/Player VR Controller");
            Destroy(rig.GetComponent<XRRig>());
            var origin = rig.AddComponent(typeof(XROrigin)) as XROrigin;
            origin.Origin = rig;
            origin.CameraFloorOffsetObject = GameObject.Find("GorillaPlayer");
            origin.Camera = GameObject.Find("Main Camera").GetComponent<Camera>();
            origin.RequestedTrackingOriginMode = XROrigin.TrackingOriginMode.NotSpecified;
            origin.CameraYOffset = 0f;
        }*/
        public static void CreateMenu()
        {
            if (physicSetting == 0 || physicSetting == 1)
            {
                menu = GameObject.CreatePrimitive(PrimitiveType.Cube);
                Destroy(menu.GetComponent<Rigidbody>());
                Destroy(menu.GetComponent<BoxCollider>());
                Destroy(menu.GetComponent<Renderer>());
            }
            else if (physicSetting == 2)
            {
                menu = GameObject.CreatePrimitive(PrimitiveType.Cube);
                Destroy(menu.GetComponent<Rigidbody>());
                Destroy(menu.GetComponent<Renderer>());
            }
            menu.name = "MysticClientModMenu";
            menu.layer = 8;
            menu.transform.localScale = new Vector3(0.1f, 0.3f, 0.4f) * RigUtils.MyPlayer.scale;
            menuBackground = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Destroy(menuBackground.GetComponent<Rigidbody>());
            Destroy(menuBackground.GetComponent<BoxCollider>());
            menuBackground.transform.parent = menu.transform;
            menuBackground.transform.rotation = Quaternion.identity;
            menuBackground.transform.localScale = menuSize;
            if (GetEnabled("Make Menu Flash"))
            {
                MenuColorKeys[0].color = FirstColor;
                MenuColorKeys[0].time = 0f;
                MenuColorKeys[1].color = SecondColor;
                MenuColorKeys[1].time = 0.3f;
                MenuColorKeys[2].color = FirstColor;
                MenuColorKeys[2].time = 0.6f;
                MenuColorKeys[3].color = SecondColor;
                MenuColorKeys[3].time = 1f;
                ColorChanger colorChanger = menuBackground.AddComponent<ColorChanger>();
                colorChanger.colors = new Gradient
                {
                    colorKeys = MenuColorKeys
                };
                colorChanger.loop = true;
            }
            else
            {
                menuBackground.GetComponent<Renderer>().material.color = NormalColor;
            }
            menuBackground.transform.position = menuPos;
            canvasObject = new GameObject();
            canvasObject.transform.parent = menu.transform;
            Canvas canvas = canvasObject.AddComponent<Canvas>();
            CanvasScaler canvasScaler = canvasObject.AddComponent<CanvasScaler>();
            canvasObject.AddComponent<GraphicRaycaster>();
            canvas.renderMode = RenderMode.WorldSpace;
            canvasScaler.dynamicPixelsPerUnit = 1000f;
            Text text = new GameObject
            {
                transform =
                {
                    parent = canvasObject.transform
                }
            }.AddComponent<Text>();
            text.font = currentFont;
            text.text = menuTitle;
            text.fontSize = 1;
            text.supportRichText = true;
            text.fontStyle = titleFontStyle;
            text.alignment = TextAnchor.MiddleCenter;
            text.resizeTextForBestFit = true;
            text.resizeTextMinSize = 0;
            RectTransform component = text.GetComponent<RectTransform>();
            component.localPosition = Vector3.zero;
            component.sizeDelta = new Vector2(0.28f, 0.05f);
            component.position = new Vector3(0.06f, 0f, menuTitleZ);
            component.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));
            if (GetEnabled("Date Time"))
            {
                Text datetimetext = new GameObject
                {
                    transform =
                    {
                        parent = canvasObject.transform
                    }
                }.AddComponent<Text>();
                datetimetext.text = DateTime.Now.ToString();
                datetimetext.font = currentFont;
                datetimetext.fontSize = 2;
                datetimetext.alignment = TextAnchor.MiddleCenter;
                datetimetext.fontStyle = FontStyle.BoldAndItalic;
                datetimetext.resizeTextForBestFit = true;
                datetimetext.resizeTextMinSize = 0;
                RectTransform dateRect = datetimetext.GetComponent<RectTransform>();
                dateRect.localPosition = Vector3.zero;
                dateRect.sizeDelta = new Vector2(0.28f, 0.05f);
                dateRect.position = new Vector3(0.06f, 0f, dateTimeZ);
                dateRect.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));
            }
            if (GetEnabled("Side Disconnect"))
            {
                var disconnectbutton = GameObject.CreatePrimitive(PrimitiveType.Cube);
                Destroy(disconnectbutton.GetComponent<Rigidbody>());
                disconnectbutton.GetComponent<BoxCollider>().isTrigger = true;
                disconnectbutton.transform.parent = menu.transform;
                disconnectbutton.transform.rotation = Quaternion.identity;
                disconnectbutton.transform.localScale = new Vector3(0.045f, 0.66f, 0.17f);
                disconnectbutton.transform.localPosition = new Vector3(0.50f, disconnectYs[0], .1f);
                disconnectbutton.AddComponent<BtnCollider>().relatedText = "DisconnectingButton";
                if (GetEnabled("Make Menu Flash"))
                {
                    ColorChanger colorChanger = disconnectbutton.AddComponent<ColorChanger>();
                    colorChanger.colors = new Gradient
                    {
                        colorKeys = MenuColorKeys
                    };
                    colorChanger.loop = true;
                }
                else
                    disconnectbutton.GetComponent<Renderer>().material.color = NormalColor;
                Text discontext = new GameObject
                {
                    transform =
                    {
                        parent = canvasObject.transform
                    }
                }.AddComponent<Text>();
                discontext.text = "Disconnect";
                discontext.font = currentFont;
                discontext.fontSize = 2;
                discontext.alignment = TextAnchor.MiddleCenter;
                discontext.fontStyle = FontStyle.BoldAndItalic;
                discontext.resizeTextForBestFit = true;
                discontext.resizeTextMinSize = 0;

                RectTransform rectt = discontext.GetComponent<RectTransform>();
                rectt.localPosition = Vector3.zero;
                rectt.sizeDelta = new Vector2(0.1f, 0.03f);
                rectt.localPosition = new Vector3(0.071f, disconnectYs[1], .043f);
                rectt.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));
            }

            if (GetEnabled("Destroy Networked Objects Side Button"))
            {
                var destoryButton = GameObject.CreatePrimitive(PrimitiveType.Cube);
                Destroy(destoryButton.GetComponent<Rigidbody>());
                destoryButton.GetComponent<BoxCollider>().isTrigger = true;
                destoryButton.transform.parent = menu.transform;
                destoryButton.transform.rotation = Quaternion.identity;
                destoryButton.transform.localScale = new Vector3(0.045f, 0.66f, 0.17f);
                destoryButton.transform.localPosition = new Vector3(0.50f, -1.122f, 0.3f);
                destoryButton.AddComponent<BtnCollider>().relatedText = "DestroyButton";
                if (GetEnabled("Make Menu Flash"))
                {
                    ColorChanger colorChanger = destoryButton.AddComponent<ColorChanger>();
                    colorChanger.colors = new Gradient
                    {
                        colorKeys = MenuColorKeys
                    };
                    colorChanger.loop = true;
                }
                else
                    destoryButton.GetComponent<Renderer>().material.color = NormalColor;
                Text destroytext = new GameObject
                {
                    transform =
                        {
                            parent = canvasObject.transform
                        }
                }.AddComponent<Text>();
                destroytext.text = "Destory Networked Objects";
                destroytext.font = currentFont;
                destroytext.fontSize = 2;
                destroytext.alignment = TextAnchor.MiddleCenter;
                destroytext.fontStyle = FontStyle.BoldAndItalic;
                destroytext.resizeTextForBestFit = true;
                destroytext.resizeTextMinSize = 0;

                RectTransform recttt = destroytext.GetComponent<RectTransform>();
                recttt.localPosition = Vector3.zero;
                recttt.sizeDelta = new Vector2(0.1f, 0.03f);
                recttt.localPosition = new Vector3(0.071f, -.331f, 0.12f);
                recttt.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));
            }
            if (GetToolTip("Changed Menu Type").buttonText.Contains("AZ"))
            {
                var roominfo = GameObject.CreatePrimitive(PrimitiveType.Cube);
                Destroy(roominfo.GetComponent<Rigidbody>());
                Destroy(roominfo.GetComponent<BoxCollider>());
                roominfo.transform.parent = menu.transform;
                roominfo.transform.rotation = Quaternion.identity;
                roominfo.transform.localScale = new Vector3(.09f, .9f, .25f);
                roominfo.transform.localPosition = new Vector3(.56f, 0, -.7f);
                roominfo.GetComponent<Renderer>().material.color = Color.black;
                Text infotext = new GameObject
                {
                    transform =
                        {
                            parent = canvasObject.transform
                        }
                }.AddComponent<Text>();
                if (PhotonSystem.InRoom)
                    infotext.text = "Players in room: " + PhotonSystem.RoomPlayerCount + "\nRoom Name: " + PhotonSystem.RoomName;
                else
                    infotext.text = "<color=red>NOT IN ROOM</color>";
                infotext.font = currentFont;
                infotext.fontSize = 2;
                infotext.alignment = TextAnchor.MiddleCenter;
                infotext.fontStyle = FontStyle.BoldAndItalic;
                infotext.resizeTextForBestFit = true;
                infotext.resizeTextMinSize = 0;

                RectTransform recttt = infotext.GetComponent<RectTransform>();
                recttt.localPosition = Vector3.zero;
                recttt.sizeDelta = new Vector2(0.2f, 0.05f);
                recttt.localPosition = roominfo.transform.position + new Vector3(.01f, 0, 0);
                recttt.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));
                if (!GetEnabled("Side Disconnect"))
                {
                    var disconnectbutton = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    Destroy(disconnectbutton.GetComponent<Rigidbody>());
                    disconnectbutton.GetComponent<BoxCollider>().isTrigger = true;
                    disconnectbutton.transform.parent = menu.transform;
                    disconnectbutton.transform.rotation = Quaternion.identity;
                    disconnectbutton.transform.localScale = new Vector3(0.045f, 0.9f, 0.17f);
                    disconnectbutton.transform.localPosition = new Vector3(0.56f, 0f, .53f);
                    disconnectbutton.AddComponent<BtnCollider>().relatedText = "DisconnectingButton";
                    disconnectbutton.GetComponent<Renderer>().material.color = Color.black;
                    Text discontext = new GameObject
                    {
                        transform =
                    {
                        parent = canvasObject.transform
                    }
                    }.AddComponent<Text>();
                    discontext.text = "Disconnect";
                    discontext.font = currentFont;
                    discontext.fontSize = 1;
                    discontext.alignment = TextAnchor.MiddleCenter;
                    discontext.fontStyle = FontStyle.BoldAndItalic;
                    discontext.resizeTextForBestFit = true;
                    discontext.resizeTextMinSize = 0;

                    RectTransform rectt = discontext.GetComponent<RectTransform>();
                    rectt.localPosition = Vector3.zero;
                    rectt.sizeDelta = new Vector2(0.2f, 0.03f);
                    rectt.localPosition = new Vector3(0.064f, 0f, 0.21f);
                    rectt.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));
                }
            }
            GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Destroy(gameObject.GetComponent<Rigidbody>());
            gameObject.GetComponent<BoxCollider>().isTrigger = true;
            gameObject.transform.parent = menu.transform;
            gameObject.transform.rotation = Quaternion.identity;
            gameObject.transform.localScale = pageButtonSize;
            gameObject.transform.localPosition = pagePoss[0];
            if (GetEnabled("Make Menu Flash") && !GetToolTip("Changed Menu Type").buttonText.Contains("AZ"))
            {
                ColorChanger colorChanger = gameObject.AddComponent<ColorChanger>();
                colorChanger.colors = new Gradient
                {
                    colorKeys = MenuColorKeys
                };
                colorChanger.loop = true;
            }
            else
            {
                gameObject.GetComponent<Renderer>().material.color = GetToolTip("Changed Menu Type").buttonText.Contains("AZ") ? Color.black : NormalColor;
            }
            gameObject.AddComponent<BtnCollider>().relatedText = "PreviousPage";
            text = new GameObject
            {
                transform =
                {
                    parent = canvasObject.transform
                }
            }.AddComponent<Text>();
            text.font = currentFont;
            text.fontSize = 1;
            text.alignment = TextAnchor.MiddleCenter;
            text.resizeTextForBestFit = true;
            text.resizeTextMinSize = 0;
            component = text.GetComponent<RectTransform>();
            component.localPosition = Vector3.zero;
            component.sizeDelta = new Vector2(0.2f, 0.03f);
            component.localPosition = new Vector3(0.064f, 0f, 0.109f / 2.55f);
            component.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));
            gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Destroy(gameObject.GetComponent<Rigidbody>());
            gameObject.GetComponent<BoxCollider>().isTrigger = true;
            gameObject.transform.parent = menu.transform;
            gameObject.transform.rotation = Quaternion.identity;
            gameObject.transform.localScale = pageButtonSize;
            gameObject.transform.localPosition = pagePoss[1];
            if (GetEnabled("Make Menu Flash") && !GetToolTip("Changed Menu Type").buttonText.Contains("AZ"))
            {
                ColorChanger colorChanger = gameObject.AddComponent<ColorChanger>();
                colorChanger.colors = new Gradient
                {
                    colorKeys = MenuColorKeys
                };
                colorChanger.loop = true;
            }
            else
            {
                gameObject.GetComponent<Renderer>().material.color = GetToolTip("Changed Menu Type").buttonText.Contains("AZ") ? Color.black : NormalColor;
            }
            gameObject.AddComponent<BtnCollider>().relatedText = "NextPage";
            text = new GameObject
            {
                transform =
                {
                    parent = canvasObject.transform
                }
            }.AddComponent<Text>();
            text.font = currentFont;
            text.fontSize = 1;
            text.alignment = TextAnchor.MiddleCenter;
            text.resizeTextForBestFit = true;
            text.resizeTextMinSize = 0;
            component = text.GetComponent<RectTransform>();
            component.localPosition = Vector3.zero;
            component.sizeDelta = new Vector2(0.2f, 0.03f);
            component.localPosition = new Vector3(0.064f, 0f, 0.109f / 2.55f);
            component.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));
            GameObject tooltipObj = new GameObject();
            tooltipObj.transform.SetParent(canvasObject.transform);
            tooltipObj.transform.localPosition = new Vector3(0f, 0f, 1.2f);
            tooltipText = tooltipObj.GetComponent<Text>();
            if (tooltipText == null)
                tooltipText = tooltipObj.AddComponent<Text>();
            tooltipText.font = currentFont;
            tooltipText.text = tooltipString;
            tooltipText.fontSize = 18;
            tooltipText.alignment = TextAnchor.MiddleCenter;
            tooltipText.resizeTextForBestFit = true;
            tooltipText.resizeTextMinSize = 0;
            tooltipText.color = Color.white;
            RectTransform componenttooltip = tooltipObj.GetComponent<RectTransform>();
            componenttooltip.localPosition = Vector3.zero;
            componenttooltip.sizeDelta = new Vector2(0.2f, 0.03f);
            componenttooltip.position = new Vector3(0.06f, 0f, toolTipZ);
            componenttooltip.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));
            var activeButtons = buttons[buttonsType].Skip(pageNumber * pageSize).Take(pageSize).ToArray();
            for (int i = 0; i < activeButtons.Length; i++)
            {
                CreateButton(i * 0.13f, activeButtons[i]);
            }
        }

        public static void CreateButton(float offset, ButtonInfo method)
        {
            GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Destroy(gameObject.GetComponent<Rigidbody>());
            gameObject.GetComponent<BoxCollider>().isTrigger = true;
            gameObject.transform.parent = menu.transform;
            gameObject.transform.rotation = Quaternion.identity;
            gameObject.transform.localScale = buttonSize;
            gameObject.transform.localPosition = new Vector3(buttonPos.x, buttonPos.y, buttonPos.z - offset / buttonOffset);
            gameObject.AddComponent<BtnCollider>().relatedText = method.buttonText;
            Text text = new GameObject
            {
                transform =
                {
                    parent = canvasObject.transform
                }
            }.AddComponent<Text>();
            text.font = currentFont;
            text.text = method.buttonText;
            text.color = buttonTextColor;
            text.supportRichText = true;
            text.fontSize = 1;
            if (method.enabled)
            {
                gameObject.GetComponent<Renderer>().material.color = ButtonColorEnabled;
            }
            else
            {
                gameObject.GetComponent<Renderer>().material.color = ButtonColorDisable;
            }
            text.alignment = TextAnchor.MiddleCenter;
            text.fontStyle = buttonFontStyle;
            text.resizeTextForBestFit = true;
            text.resizeTextMinSize = 0;
            RectTransform component = text.GetComponent<RectTransform>();
            component.localPosition = Vector3.zero;
            component.sizeDelta = new Vector2(.2f, .03f);
            component.localPosition = new Vector3(buttonTextPos.x, buttonTextPos.y, buttonTextPos.z - offset / buttonTextOffset); // 2.6
            component.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));
        }
        public static void DestroyMenu()
        {
            Destroy(menu);
            Destroy(canvasObject);
            Destroy(reference);
            menu = null;
            menuBackground = null;
            canvasObject = null;
            reference = null;
        }
        public static void RecreateMenu()
        {
            if (menu != null)
            {
                Destroy(menu);
                menu = null;
                CreateMenu();
                RecenterMenu(GetEnabled("Right Hand Menu"), UserInput.GetKey(KeyCode.Q));
            }
        }
        public static void RecenterMenu(bool isRightHanded, bool isKeyboardCondition)
        {
            if (!isKeyboardCondition)
            {
                if (!isRightHanded)
                {
                    if (GetEnabled("Watch Menu"))
                    {
                        menu.transform.position = new Vector3(0, .3f, 0) + RigUtils.MyOnlineRig.leftHandTransform.position;
                        menu.transform.rotation = RigUtils.MyOnlineRig.bodyCollider.transform.rotation;
                        menu.transform.RotateAround(menu.transform.position, menu.transform.forward, 90f);
                        menu.transform.RotateAround(menu.transform.position, menu.transform.up, 90f);
                        return;
                    }
                    menu.transform.position = RigUtils.MyOnlineRig.leftHandTransform.position;
                    menu.transform.rotation = RigUtils.MyOnlineRig.leftHandTransform.rotation;
                    return;
                }
                if (GetEnabled("Watch Menu"))
                {
                    menu.transform.position = new Vector3(0, .3f, 0) + RigUtils.MyOnlineRig.rightHandTransform.position;
                    menu.transform.rotation = RigUtils.MyOnlineRig.bodyCollider.transform.rotation;
                    menu.transform.RotateAround(menu.transform.position, menu.transform.forward, 90f);
                    menu.transform.RotateAround(menu.transform.position, menu.transform.up, 90f);
                    return;
                }
                menu.transform.position = RigUtils.MyOnlineRig.rightHandTransform.position;
                menu.transform.rotation = RigUtils.MyOnlineRig.rightHandTransform.rotation;
                menu.transform.RotateAround(menu.transform.position, menu.transform.forward, 180f);
            }
            else
            {
                try { TPC = GameObject.Find("Player Objects/Third Person Camera/Shoulder Camera").GetComponent<Camera>(); } catch { }
                if (TPC != null)
                {
                    TPC.transform.position = new Vector3(100f, 100f, 100f);
                    TPC.transform.LookAt(RigUtils.MyOnlineRig.transform.position);
                    Destroy(menu.GetComponent<Rigidbody>());
                    menu.transform.parent = TPC.transform;
                    menu.transform.position = (TPC.transform.position + (Vector3.Scale(TPC.transform.forward, (GetToolTip("Changed Menu Type").buttonText.Contains("AZ") && !GetToolTip("Changed Page Type").buttonText.Contains("Sides")) ? new Vector3(0.7f, 0.68f, 0.7f) : new Vector3(0.6f, 0.545f, 0.6f)))) + (Vector3.Scale(TPC.transform.up, new Vector3(-0.02f, .03f, -0.02f)));
                    menu.transform.LookAt(TPC.transform.position);
                    menu.transform.RotateAround(menu.transform.position, menu.transform.forward, 90f);
                    menu.transform.RotateAround(menu.transform.position, menu.transform.up, 90f);
                    menu.transform.RotateAround(menu.transform.position, menu.transform.forward, 180f);
                    if (reference != null)
                        if (UserInput.GetMouseButton(0) || UserInput.GetMouseButton(2))
                        {
                            RaycastHit hit;
                            if (Physics.Raycast(TPC.ScreenPointToRay(Mouse.current.position.ReadValue()), out hit, 100))
                                hit.transform.gameObject.GetComponent<BtnCollider>()?.OnTriggerEnter(reference.GetComponent<Collider>());
                        }
                        else
                            reference.transform.position = new Vector3(999f, -999f, -999f);
                    else
                        CreateReference(GetEnabled("Right Hand Menu"));
                }
            }
        }

        public static void CreateReference(bool isRightHanded)
        {
            reference = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            reference.transform.parent = isRightHanded ? RigUtils.MyOnlineRig.leftHandTransform : RigUtils.MyOnlineRig.rightHandTransform;
            reference.GetComponent<Renderer>().material.color = new Color32(167, 17, 237, 28);
            reference.transform.localPosition = pointerPosition;
            reference.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
            reference.name = "buttonPresser";
        }

        public static void Toggle(string buttonText)
        {
            int lastPage = ((buttons[buttonsType].Length + pageSize - 1) / pageSize) - 1;
            if (buttonText == "PreviousPage")
            {
                pageNumber--;
                if (pageNumber < 0)
                {
                    pageNumber = lastPage;
                }
            } 
            else
            {
                if (buttonText == "NextPage")
                {
                    pageNumber++;
                    if (pageNumber > lastPage)
                    {
                        pageNumber = 0;
                    }
                } 
                else
                {
                    if (buttonText == "DisconnectingButton")
                    {
                        PhotonNetwork.Disconnect();
                    }
                    else
                    {
                        if (buttonText == "DestroyButton")
                        {
                            Networking.DestroyNetworkedObjects();
                        }
                        else
                        {
                            var target = GetIndex(buttonText);
                            if (target != null)
                            {
                                if (target.isTogglable)
                                {
                                    target.enabled = !target.enabled;
                                    if (target.enabled)
                                    {
                                        if (target.method != null)
                                        {
                                            try { target.method.Invoke(); } catch { }
                                        }
                                    }
                                    else
                                    {
                                        if (target.disableMethod != null)
                                        {
                                            try { target.disableMethod.Invoke(); } catch { }
                                        }
                                    }
                                }
                                else
                                {
                                    if (target.method != null)
                                    {
                                        try { target.method.Invoke(); } catch { }
                                    }
                                }
                                tooltipString = target.toolTip;
                            }
                            else { Debug.LogError(buttonText + " does not exist"); }
                        }
                    }
                }
            }
            RecreateMenu();
        }
        public static void PlayButtonSound(string index)
        {
            if (GetToolTip("Changed Button Sound").buttonText.Contains("Dynamic"))
            {
                var target = GetIndex(index);
                if (target != null)
                {
                    if (target.isTogglable)
                    {
                        if (!target.enabled)
                            menuHandAudio.PlayOneShot(buttonEnableSound);
                        else
                            menuHandAudio.PlayOneShot(buttonDisableSound);
                    }
                    else
                        if (target.method != null)
                            menuHandAudio.PlayOneShot(buttonEnableSound);
                    menuHandAudio.volume = .5f;
                }
                else
                {
                    if (index == "NextPage")
                    {
                        menuHandAudio.PlayOneShot(buttonEnableSound);
                    }
                    else if (index == "PreviousPage")
                    {
                        menuHandAudio.PlayOneShot(buttonEnableSound);
                    }
                    else if (index == "DisconnectingButton")
                    {
                        menuHandAudio.PlayOneShot(buttonDisableSound);
                    }
                    else if (index == "DestroyButton")
                    {
                        menuHandAudio.PlayOneShot(buttonEnableSound);
                    }
                    if (target == null && index != "NextPage" && index != "PreviousPage" && index != "DisconnectingButton" && index != "DestroyButton")
                        Debug.LogError($"button {index} does not exist");
                    menuHandAudio.volume = .5f;
                }
            }
            else
            {
                /*if (GetEnabled("ServerSided Button Sounds")) this made the button not be able to disable idk why
                {
                    if (PhotonSystem.InRoom)
                    {
                        RigUtils.MyOfflineRig.PlayHandTapLocal(buttonSound, GetEnabled("Right Hand Menu"), 0.4f);
                        new PhotonView().RPC("RPC_PlayHandTap", RpcTarget.Others, new object[]
                        {
                         buttonSound,
                         GetEnabled("Right Hand Menu"),
                         99999f
                        });
                        RPCProtection(RigUtils.MyNetPlayer);
                    }
                    else RigUtils.MyOfflineRig.PlayHandTapLocal(buttonSound, GetEnabled("Right Hand Menu"), 0.4f);
                } else */RigUtils.MyOfflineRig.PlayHandTapLocal(buttonSound, GetEnabled("Right Hand Menu"), 0.4f);
            }
        }
        public static ButtonInfo GetIndex(string buttonText)
        {
            foreach (var buttons in buttons)
                foreach (var button in buttons)
                    if (button.buttonText == buttonText)
                        return button;
            return null;
        }
        public static ButtonInfo GetToolTip(string tooltip)
        {
            foreach (var buttons in buttons)
                foreach (var button in buttons)
                    if (button.toolTip == tooltip)
                        return button;
            return null;
        }
        public static bool GetEnabled(string Btnname)
        {
            foreach (var buttons in buttons)
                foreach (var button in buttons)
                    if (button.buttonText == Btnname)
                        return button.enabled;
            return false;
        }
        private static float bypassDelay = 0;
        private static float bypassCooldown = 5;
        private static bool hasBypassed;
        public static void RPCProtection(bool better = false)
        {
            if (better)
            {
                if (Time.time - bypassDelay < bypassCooldown) // thanks to @drew008278 for this 
                    return;
                bypassDelay = Time.time;
                try
                {
                    if (!hasBypassed)
                    {
                        hasBypassed = true;
                        PhotonNetwork.OpRemoveCompleteCacheOfPlayer(RigUtils.MyNetPlayer.ActorNumber);
                        var instanceNot = GorillaNot.instance;
                        instanceNot.logErrorMax = int.MaxValue;
                        instanceNot.rpcErrorMax = int.MaxValue;
                        instanceNot.rpcCallLimit = int.MaxValue;
                        PhotonNetwork.MaxResendsBeforeDisconnect = 5;
                        PhotonNetwork.QuickResends = 2;
                        instanceNot.OnPlayerLeftRoom(RigUtils.MyNetPlayer);
                    }
                }
                catch (Exception ex) { Debug.LogError($"Error in RPCProtection: {ex.Message}"); }
            }
            else
            {
                GorillaNot.instance.rpcErrorMax = int.MaxValue;
                GorillaNot.instance.rpcCallLimit = int.MaxValue;
                GorillaNot.instance.logErrorMax = int.MaxValue;
                PhotonNetwork.MaxResendsBeforeDisconnect = int.MaxValue;
                PhotonNetwork.QuickResends = int.MaxValue;
                PhotonNetwork.RemoveRPCs(RigUtils.GetPlayerFromNet(RigUtils.MyNetPlayer));
                PhotonNetwork.OpCleanActorRpcBuffer(RigUtils.MyNetPlayer.ActorNumber);
                PhotonNetwork.RemoveBufferedRPCs(RigUtils.MyPhotonView.ViewID, null, null);
                PhotonNetwork.RemoveRPCsInGroup(int.MaxValue);
                PhotonNetwork.SendAllOutgoingCommands();
                GorillaNot.instance.OnPlayerLeftRoom(RigUtils.MyNetPlayer);
            }
        }
        public static void CopyToClipboard(string text)
        {
            if (!text.IsNullOrEmpty())
            {
                GUIUtility.systemCopyBuffer = text;
                Debug.Log("Copyed " + text + " To System Clipboard");
            } else Debug.LogError("Text Could Not be Copyed Do To It Being Empty");
        }
        public static void LegacySendEvent(byte code, object evData, RaiseEventOptions reo, bool reliable)
        {
            if (PhotonNetwork.InRoom)
                try { PhotonNetwork.RaiseEvent(code, evData, reo, reliable ? SendOptions.SendReliable : SendOptions.SendUnreliable);
                } catch (Exception ex) { Debug.LogError($"Error with sending event {code}, {ex}"); }
            else Debug.LogWarning("Could Not Raise Event" + code.ToString() + " Do To Not Being Connected To Photon Server");
        }
        public static void LegacySendEvent(in byte code, in object evData, Photon.Realtime.Player target, bool reliable)
        {
            LegacySendEvent(code, evData, new RaiseEventOptions { TargetActors = new int[1] { target.ActorNumber } }, reliable);
        }
        public static void SendEvent(in byte code, in object evData, in NetEventOptions neo, bool reliable)
        {
            object[] objects = new object[] { NetworkSystem.Instance.ServerTimestamp, code, evData };
            NetworkSystemRaiseEvent.RaiseEvent(3, objects, neo, reliable);
        }
        public static void SendEvent(in byte code, in object evData, in NetPlayer target, bool reliable)
        {
            NetworkSystemRaiseEvent.neoTarget.TargetActors[0] = target.ActorNumber;
            SendEvent(code, evData, NetworkSystemRaiseEvent.neoTarget, reliable);
        }
        public static void SendRPC(string method, RpcTarget target, object[] args)
        {
            new PhotonView().RPC(method, target, args);
        }
        public static void SendRPC(string method, NetPlayer target, object[] args)
        {
            new PhotonView().RPC(method, RigUtils.GetPlayerFromNet(target), args);
        }
        public static void DestroyObjectsByName(string name)
        {
            for (int i = 1; i > 0; i++)
                GameObject.Find(name).SetActive(false);
        }
        public static void DestroyObjects(GameObject[] objects)
        {
            for (int i = 0; i < objects.Length; i++)
                objects[i].SetActive(false);
        }
        public static string GetObjectPath(Transform transform)
        {
            string path = "";
            while (transform.parent != null)
            {
                transform = transform.parent;
                path = string.IsNullOrEmpty(path) ? transform.name : transform.name + "/" + path;
            }
            return path;
        }
        public static GameObject GetObjectByName(string objectName)
        {
            foreach (GameObject obj in GetObjectsByName(objectName))
            {
                return obj;
            }
            return null;
        }
        public static GameObject[] GetObjectsByName(string objectName)
        {
            GameObject[] objects = null;
            foreach (GameObject obj in GetGameObjects())
            {
                if (obj.name.Contains(objectName))
                {
                    objects.AddItem(obj);
                }
            }
            return objects;
        }
        public static GameObject GetObjectByNames(string[] objectNames)
        {
            foreach (GameObject gameObject in GetGameObjects())
                foreach (string names in objectNames)
                    if (gameObject.name == names)
                        return gameObject;
            return null;
        }
        public static GliderHoldable[] Gliders()
        {
            if (gliders == null)
                gliders = FindObjectsOfType<GliderHoldable>();
            return gliders;
        }
        public static GameObject[] GetGameObjects()
        {
            if (gameObjects == null)
                gameObjects = FindObjectsOfType<GameObject>();
            return gameObjects;
        }
        public static BuilderPiece[] GetPieces()
        {
            if (builderPieces == null)
                builderPieces = FindObjectsOfType<BuilderPiece>();
            return builderPieces;
        }
        public static void JoinRoom(string name, JoinType joinType)
        {
            PhotonNetworkController.Instance.AttemptToJoinSpecificRoom(name, joinType);
        }
        public static bool AddInfected(NetPlayer player)
        {
            if (PhotonSystem.InRoom)
                if (PhotonSystem.IsMasterClient)
                    if (!GorillaTagManager.currentInfected.Contains(player))
                    {
                        GorillaTagManager.AddInfectedPlayer(player);
                        return true;
                    }
                    else return false;
                else return false;
            return false;
        }
        public static bool RemoveInfected(NetPlayer player)
        {
            if (PhotonSystem.InRoom)
                if (PhotonSystem.IsMasterClient)
                    if (GorillaTagManager.currentInfected.Contains(player))
                    {
                        GorillaTagManager.currentInfected.Remove(player);
                        return true;
                    }
                    else return false;
                else return false;
            return false;
        }
        public static bool SpamInfected(NetPlayer player)
        {
            if (PhotonSystem.InRoom)
                if (PhotonSystem.IsMasterClient)
                    if (GorillaTagManager.currentInfected.Contains(player))
                    {
                        GorillaTagManager.currentInfected.Remove(player);
                        return true;
                    }
                    else GorillaTagManager.AddInfectedPlayer(player);
                else return false;
            else return false;
            return false;
        }
        public static bool IsTagged(NetPlayer player)
        {
            if (GorillaTagManager.currentInfected.Contains(player))
                return true;
            return false;
        }
        public static string GetInfoFromPlayer(NetPlayer player)
        {
            var info = "";
            var realtime = RigUtils.GetPlayerFromNet(player);
            var request = new GetAccountInfoRequest { PlayFabId = realtime.UserId };
            PlayFabClientAPI.GetAccountInfo(request, delegate (GetAccountInfoResult result)
            {
                info = string.Format("{0}\r\n{1:yyyy/MM/dd}", realtime.NickName, result.AccountInfo.Created);
            }, null, null, null);
            return info;
        }
        public static GorillaRopeSwing[] GetRopes()
        {
            if (ropes == null)
                ropes = FindObjectsOfType<GorillaRopeSwing>();
            return ropes;
        }
        public static object GetProjectile(int hash)
        {
            var obj = ObjectPools.instance.Instantiate(hash);
            if (obj.GetComponent<SnowballThrowable>() && GetObjectPath(obj.transform.parent) == "Player Objects/Local VRRig/Local Gorilla Player/Holdables")
            {
                return obj.GetComponent<SnowballThrowable>();
            }
            else if (obj.GetComponent<PaperPlaneThrowable>() && GetObjectPath(obj.transform.parent) == "Player Objects/Local VRRig/Local Gorilla Player/RigAnchor/rig/body/shoulder.R/upper_arm.R/forearm.R/TransferrableItemRightArm/DropZoneAnchor")
            {
                return obj.GetComponent<PaperPlaneThrowable>();
            }
            else if (obj.GetComponent<SlingshotProjectile>())
            {
                return obj.GetComponent<SlingshotProjectile>();
            }
            else if (obj.GetComponent<SlingshotProjectileTrail>())
            {
                return obj.GetComponent<SlingshotProjectileTrail>();
            }
            return null;
        }
        public static Color RGBColor()
        {
            float time = Time.time;
            float r = Mathf.Sin(time * 2f) * 0.5f + 0.5f;
            float g = Mathf.Sin(time * 1.5f) * 0.5f + 0.5f;
            float b = Mathf.Sin(time * 2.5f) * 0.5f + 0.5f;
            return new Color(r, g, b, 1);
        }
        public static Color HardColor(int index)
        {
            return Settings.colors[index % Settings.colors.Length];
        }
        public static string ATS(string[] array)
        {
            foreach (var i in array)
                return i;
            return null;
        }
        public static GameObject menu;
        public static GameObject menuBackground;   
        public static GameObject reference;
        public static GameObject canvasObject;
        public static Camera TPC;
        public static int pageNumber = 0;
        public static int buttonsType = 0;
        public static int pageSize = 8;
        private static Text tooltipText;
        public static string tooltipString;
        public static int destroyDelay = 0;
        public static Color boardColor = Color.black;
        private static AudioSource menuHandAudio { get { return GetEnabled("Right Hand Menu") ? RigUtils.MyOfflineRig.leftHandPlayer : RigUtils.MyOfflineRig.rightHandPlayer; } }
        /*
        leaf star -1291863839
        dragon slingshot 526153556
        yumi bow proj -2146558070
        samurai bow proj 1844846002
        fire ball proj 2111191893
        cyber ninja star 1936767506
        rotten punkin proj -767500737
        playing card proj -1299387895
        bat swarm proj -1444634975
        */
        private static GliderHoldable[] gliders = null;
        private static GorillaRopeSwing[] ropes = null;
        private static GameObject[] gameObjects = null;
        private static BuilderPiece[] builderPieces = null;
        private static VRRig Ghost = null;
        private static GradientColorKey[] MenuColorKeys = new GradientColorKey[4];
        public static Camera mainCamera;
        public static Color buttonTextColor = Color.black;
        private static string MOTDText =
        "Remade The Whole Menu Using II Stupids Template Instead Of Shibas";
        private static string Credits =
            "ThatGuy [Main Developer / Owner]\n" +
            "Drew [Helper]\n" +
            "Mystic [The Real Owner]\n" +
            "\n\n\n\n\n\n\n" +
            "You Are Currently Using Mystic Client Version [" + PluginInfo.Version + "] Check Discord Or Github For Any Future Updates";
    }
}
