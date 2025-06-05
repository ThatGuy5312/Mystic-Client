using BepInEx;
using GorillaLocomotion.Gameplay;
using GorillaNetworking;
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
using static MysticClient.Menu.Buttons;
using static MysticClient.Menu.MenuSettings;
using MysticClient.Mods;
using TMPro;
using System.Reflection;
using SColor = System.Drawing.Color;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using System.Threading.Tasks;
using MysticClient.Notifications;
using UnityEngine.XR;
using GorillaExtensions;

namespace MysticClient.Menu
{
    //[HarmonyPatch(typeof(GorillaLocomotion.Player), "LateUpdate")]
    public class Main : MonoBehaviour
    {
        public static GorillaPaintbrawlManager GorillaPaintbrawlManager => GameObject.Find("Gorilla Battle Manager").GetComponent<GorillaPaintbrawlManager>();
        public static GorillaTagManager GorillaTagManager => GameObject.Find("Gorilla Tag Manager").GetComponent<GorillaTagManager>();
        public static GorillaHuntManager GorillaHuntManager => GorillaGameManager.gameObject.GetComponent<GorillaHuntManager>();
        public static GorillaGameManager GorillaGameManager => GorillaGameManager.instance;
        public static NetworkSystem PhotonSystem => NetworkSystem.Instance;
        public static PhotonNetworkController NetworkController => PhotonNetworkController.Instance;
        public static ControllerInputPoller Controller => ControllerInputPoller.instance;
        public static IInputSystem UserInput => UnityInput.Current;

        void Update()
        {
            try
            {
                var toOpen = (!GetEnabled("Right Hand Menu") && Controller.leftControllerSecondaryButton) || (GetEnabled("Right Hand Menu") && Controller.rightControllerSecondaryButton);
                var keyboardOpen = UserInput.GetKey(KeyCode.Q);
                if (menu == null)
                {
                    if ((toOpen || keyboardOpen) && !GetEnabled("Text UI Menu") && !inKeyboard)
                    {
                        CreateMenu();
                        RecenterMenu(GetEnabled("Right Hand Menu"), keyboardOpen);
                        if (reference == null) CreateReference(GetEnabled("Right Hand Menu"));
                        if (GetEnabled("Dynamic Sounds")) Loaders.PlayAudio(AudioClips[5]);
                    }
                }
                else
                {
                    if ((toOpen || keyboardOpen) && !inKeyboard)
                    {
                        RecenterMenu(GetEnabled("Right Hand Menu"), keyboardOpen);
                        if (reference == null) CreateReference(GetEnabled("Right Hand Menu"));
                        if (menu.GetComponent<Rigidbody>()) { Destroy(menu.GetComponent<Rigidbody>()); if (GetEnabled("Dynamic Sounds")) Loaders.PlayAudio(AudioClips[5]); }
                    }
                    else if (!(toOpen || keyboardOpen) && !menu.GetComponent<Rigidbody>())
                    {
                        if (!inKeyboard)
                        {
                            TPC.gameObject.GetNamedChild("CM vcam1").SetActive(true);
                            if (physicSetting == 1 || physicSetting == 2)
                            {
                                var menuRB = menu.AddComponent<Rigidbody>();
                                menuRB.useGravity = !GetEnabled("Zero Gravity Menu");
                                var tracker = GetEnabled("Right Hand Menu") ? RigUtils.MyPlayer.rightHandCenterVelocityTracker : RigUtils.MyPlayer.leftHandCenterVelocityTracker;
                                menuRB.velocity = tracker.GetAverageVelocity(true, 0);
                                menuRB.angularVelocity = MUtils.VelocityHandEstimator(GetEnabled("Right Hand Menu") ? "RightHand Controller" : "LeftHand Controller").angularVelocity;
                                reference.Destroy(); reference = null;
                                referenceOther?.Destroy(); referenceOther ??= null;
                                if (GetEnabled("Dynamic Sounds")) Loaders.PlayAudio(AudioClips[6]);
                                if (GetEnabled("Multi Create")) { menu.Destroy(destroyDelay); menu = null; }
                            }
                            else if (physicSetting == 0)
                            {
                                if (GetEnabled("Dynamic Sounds")) Loaders.PlayAudio(AudioClips[6]);
                                if (GetEnabled("Multi Create"))
                                {
                                    menu.Destroy(destroyDelay);
                                    menu = null;
                                    reference.Destroy();
                                    reference = null;
                                    referenceOther.Destroy();
                                    referenceOther = null;
                                }
                                else DestroyMenu();
                            } else physicSetting = 1;
                        } else RecenterMenu(GetEnabled("Right Hand Menu"), true);
                    }
                }
                try
                {
                    if (!RigUtils.MyOfflineRig.enabled)
                    {
                        if (Rig.GhostType == 1)
                        {
                            rightGhostHand ??= GameObject.CreatePrimitive(PrimitiveType.Cube);
                            rightGhostHand.transform.position = RigUtils.MyOnlineRig.rightHandTransform.position;
                            rightGhostHand.transform.Rotate(1, 1, 1);
                            rightGhostHand.transform.localScale = new Vector3(.15f, .15f, .15f) * RigUtils.MyPlayer.scale;
                            rightGhostHand.transform.GetComponent<Renderer>().material = TransparentMaterial(GetChangeColorA(NormalColor, .15f));
                            leftGhostHand ??= GameObject.CreatePrimitive(PrimitiveType.Cube);
                            leftGhostHand.transform.position = RigUtils.MyOnlineRig.leftHandTransform.position;
                            leftGhostHand.transform.Rotate(1, 1, 1);
                            leftGhostHand.transform.localScale = new Vector3(.15f, .15f, .15f) * RigUtils.MyPlayer.scale;
                            leftGhostHand.transform.GetComponent<Renderer>().material = TransparentMaterial(GetChangeColorA(NormalColor, .15f));
                            rightGhostHand.Destroy<Collider>();
                            leftGhostHand.Destroy<Collider>();
                        }
                        else if (Rig.GhostType == 2)
                        {
                            if (Ghost == null)
                            {
                                Ghost = Instantiate(RigUtils.MyOfflineRig, RigUtils.MyPlayer.transform.position, RigUtils.MyPlayer.transform.rotation);
                                Ghost.headBodyOffset = Vector3.zero;
                                Ghost.enabled = true;
                            }
                            Ghost.gameObject.SetActive(true);
                            Ghost.mainSkin.material = TransparentMaterial(GetChangeColorA(NormalColor, .5f));
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
                    else 
                    { 
                        Ghost?.gameObject?.SetActive(false);
                        rightGhostHand?.Destroy();
                        leftGhostHand?.Destroy();
                    }
                }
                catch { }
                try
                {
                    // i know tpc does the same thing but this just works better and i got it from old mystic client and im not changing it
                    if (GameObject.Find("Third Person Camera"))
                        mainCamera = GameObject.Find("Shoulder Camera").GetComponent<Camera>();
                    else mainCamera = Camera.main;
                }
                catch { }
                try
                {
                    if (UserInput.GetMouseButton(0)) // so much shorter than what ever everyone else does
                    {
                        Physics.Raycast(mainCamera.ScreenPointToRay(UserInput.mousePosition), out var hit);
                        GameObject.Find("LeftHandTriggerCollider").transform.position = hit.point;
                        GameObject.Find("LeftHandTriggerCollider").GetComponent<TransformFollow>().enabled = false;
                    } else GameObject.Find("LeftHandTriggerCollider").GetComponent<TransformFollow>().enabled = true;
                }
                catch { }
                try
                {
                    var material = new Material(UberShader) { color = NormalColor };
                    GameObject.Find("CodeOfConduct").GetComponent<TMP_Text>().text = "<color=blue>Credits</color>";
                    GameObject.Find("motd (1)").GetComponent<TMP_Text>().text = "<color=blue>Updates</color>";
                    GameObject.Find("motdtext").GetComponent<TMP_Text>().text = MOTDText;
                    GameObject.Find("COC Text").GetComponent<TMP_Text>().text = Credits;
                    GameObject.Find("wallmonitorforestbg").ChangeMaterial(material);
                    GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomInteractables/GorillaComputerObject/ComputerUI/monitor/monitorScreen").ChangeMaterial(material);
                    GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomInteractables/GorillaComputerObject/ComputerUI/monitor").ChangeColor(outlineColor);
                    GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomInteractables/GorillaComputerObject/ComputerUI/keyboard (1)").ChangeColor(ButtonColorDisable);
                    int find = 0;
                    if (!foundBoards)
                    {
                        for (int i = 0; i < GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom").transform.childCount; i++)
                        {
                            var boards = GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom").transform.GetChild(i).gameObject;
                            if (boards.name.Contains("UnityTempFile"))
                            {
                                find++;
                                if (find == 2) // sigma code right here
                                {
                                    boardName = boards.name;
                                    Debug.Log($"Found board name: {boards.name}");
                                }
                            }
                        }
                        foundBoards = true;
                    }
                    if (foundBoards)
                    {
                        find = 0;
                        for (int i = 0; i < GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom").transform.childCount; i++)
                        {
                            var boards = GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom").transform.GetChild(i).gameObject;
                            if (boards.name.Contains(boardName)) // 8e79918dcea7d684f8d517406813ed80 2/8/2025
                            {
                                find++;
                                if (find == 2)
                                    boards.ChangeMaterial(material);
                            }
                        }
                        find = 0;
                        for (int i = 0; i < GameObject.Find("Environment Objects/LocalObjects_Prefab/Forest").transform.childCount; i++)
                        {
                            var boards = GameObject.Find("Environment Objects/LocalObjects_Prefab/Forest").transform.GetChild(i).gameObject;
                            if (boards.name.Contains(boardName))
                            {
                                find++;
                                if (find == 4)
                                    boards.ChangeMaterial(material);
                            }
                        }
                    }
                    for (int i = 0; i < GameObject.Find("Environment Objects").transform.childCount; i++)
                    {
                        var boards = GameObject.Find("Environment Objects").transform.GetChild(i).gameObject;
                        if (boards.name == "wallmonitorscreen_small(clone)")
                            boards.ChangeMaterial(material);
                    }
                    /*find = 0;
                    for (int i = 0; i < GameObject.Find("Environment Objects/LocalObjects_Prefab/City_WorkingPrefab").transform.childCount; i++)
                    {
                        var boards = GameObject.Find("Environment Objects/LocalObjects_Prefab/City_WorkingPrefab").transform.GetChild(i).gameObject;
                        if (boards.name.Contains("db6d743600b8c3547ada9400d1affb93"))
                        {
                            find++;
                            if (find == 3)
                                boards.ChangeMaterial(material);
                        }
                    }*/
                    foreach (var buttons in GetKeyboardButtons())
                    {
                        var colorSetting = buttons.ButtonColorSettings;
                        colorSetting.UnpressedColor = ButtonColorDisable;
                        colorSetting.PressedColor = ButtonColorEnabled;
                        gorillaKeys = buttons;
                        if (!pressedKeys)
                        {
                            buttons.PressButtonColourUpdate();
                            pressedKeys = true;
                        }
                    }
                    if (!foundSmallBoards)
                    {
                        foreach (var objs in GetGameObjects())
                            if (objs.name == "wallmonitorscreen_small")
                            {
                                var obj = Instantiate(objs, objs.transform);
                                obj.ChangeMaterial(material);
                                obj.transform.localScale = obj.transform.localScale + new Vector3(.001f, .001f, .001f);
                                obj.transform.parent = GameObject.Find("Environment Objects").transform;
                            }
                        foundSmallBoards = true;
                    }
                    for (int i = 0; i < GameObject.Find("Environment Objects").transform.childCount; i++)
                    {
                        var board = GameObject.Find("Environment Objects").transform.GetChild(i).gameObject;
                        if (board.name.Contains("wallmonitorscreen_small"))
                            board.ChangeMaterial(material);
                    }
                } catch { }
                try // loading and setting
                {
                    Load();
                    MCTextures[0] = (Texture2D)BundleObjects[0].GetNamedChild("Grass").GetComponent<Renderer>().material.mainTexture;
                    MCTextures[1] = (Texture2D)BundleObjects[0].GetNamedChild("Dirt").GetComponent<Renderer>().material.mainTexture;
                    MCTextures[2] = (Texture2D)BundleObjects[0].GetNamedChild("Wood").GetComponent<Renderer>().material.mainTexture;
                    MCTextures[3] = (Texture2D)BundleObjects[0].GetNamedChild("Leaf").GetComponent<Renderer>().material.mainTexture;
                    MCTextures[4] = (Texture2D)BundleObjects[0].GetNamedChild("Plank").GetComponent<Renderer>().material.mainTexture;
                    MCTextures[5] = (Texture2D)BundleObjects[0].GetNamedChild("Stone").GetComponent<Renderer>().material.mainTexture;
                    MCTextures[6] = (Texture2D)BundleObjects[0].GetNamedChild("Cobblestone").GetComponent<Renderer>().material.mainTexture;
                    MCTextures[7] = (Texture2D)BundleObjects[0].GetNamedChild("HayBale").GetComponent<Renderer>().material.mainTexture;
                    MCTextures[8] = (Texture2D)BundleObjects[0].GetNamedChild("Glass").GetComponent<Renderer>().material.mainTexture;
                    MCTextures[9] = (Texture2D)BundleObjects[0].GetNamedChild("Obsidian").GetComponent<Renderer>().material.mainTexture;
                    MCTextures[10] = (Texture2D)BundleObjects[0].GetNamedChild("Water").GetComponent<Renderer>().material.mainTexture;
                    MCTextures[11] = (Texture2D)BundleObjects[0].GetNamedChild("TrapDoor").GetComponent<Renderer>().material.mainTexture;
                    Fun.car ??= BundleObjects[0].GetNamedChild("Cyber truck");
                    BundleObjects[0].transform.Rotate(0, .2f, 0); // idk im bored
                    keyboard ??= BundleObjects[1];
                    Movement.platColorKeys[0].color = Movement.PlatFirstColor;
                    Movement.platColorKeys[0].time = 0f;
                    Movement.platColorKeys[1].color = Movement.PlatSecondColor;
                    Movement.platColorKeys[1].time = 0.3f;
                    Movement.platColorKeys[2].color = Movement.PlatFirstColor;
                    Movement.platColorKeys[2].time = 0.6f;
                    Movement.platColorKeys[3].color = Movement.PlatSecondColor;
                    Movement.platColorKeys[3].time = 1f;
                    if (planet == null && planetRing == null)
                    {
                        var planetRingMat = TransparentMaterial(GetChangeColorA(Color.blue, .3f));
                        planet = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                        Destroy(planet.GetComponent<Collider>());
                        planet.transform.localScale = new Vector3(.6f, .6f, .6f);
                        planet.transform.position = new Vector3(-66.9f, 12.2f, -82.6f);
                        planet.GetComponent<Renderer>().material = TransparentMaterial(new Color(.541f, .027f, .761f, .3f));
                        planetRing = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                        Destroy(planetRing.GetComponent<Collider>());
                        planetRing.transform.localScale = new Vector3(.1f, .1f, .1f);
                        planetRing.GetComponent<Renderer>().material = planetRingMat;
                        var trail = planetRing.AddComponent<TrailRenderer>();
                        trail.material = planetRingMat;
                        trail.time = 1f;
                        trail.startWidth = .1f;
                        trail.endWidth = 0;
                        trail.minVertexDistance = .1f;
                    }
                    var radius = .8f; var speed = 90f; var tilt = 30f; planetRing.transform.position = planet.transform.position + new Vector3(Mathf.Cos(Time.frameCount / speed) * radius, Mathf.Sin(Time.frameCount / speed) * radius * Mathf.Sin(Mathf.Deg2Rad * tilt), Mathf.Sin(Time.frameCount / speed) * Mathf.Cos(Mathf.Deg2Rad * tilt) * radius);
                    if (GetEnabled("Disable Stump Planet")) { planet.SetActive(false); planetRing.SetActive(false); } else { planet.SetActive(true); planetRing.SetActive(true); }

                    //DateTimeTitle = DateTime.Now.ToString(); menuTitleZ = menuSize.z / 2f;
                }
                catch { }
                try
                {
                    Fun.pickaxe ??= BundleObjects[0].GetNamedChild("Pickaxe");
                    Fun.pickaxe.transform.localScale = new Vector3(.7f, .7f, .7f);
                    if (GetEnabled("Pickaxe"))
                    {
                        Fun.pickaxe.transform.parent = RigUtils.MyOnlineRig.rightHandTransform;
                        Fun.pickaxe.layer = 2;
                        Fun.pickaxe.transform.localPosition = Vector3.zero;
                        Fun.pickaxe.transform.localRotation = DevUI.pickRot;
                        Fun.pickaxe.SetActive(true);
                    } else Fun.pickaxe.SetActive(false);
                } catch { }
            } 
            catch { }
            if (inKeyboard)
            {
                keyboard.SetActive(true);
                if (!XRSettings.isDeviceActive)
                {
                    for (var letter = KeyCode.A; letter <= KeyCode.Z; letter++)
                        if (UserInput.GetKeyDown(letter))
                        {
                            KeyboardInput += letter.ToString();
                            RigUtils.MyOfflineRig.PlayHandTapLocal(66, GetEnabled("Right Hand Menu"), .4f);
                            RecreateMenu();
                        }

                    if (UserInput.GetKeyDown(KeyCode.Backspace) && KeyboardInput.Length > 0)
                    {
                        try { KeyboardInput = KeyboardInput[..^1]; } catch { }
                        RigUtils.MyOfflineRig.PlayHandTapLocal(66, GetEnabled("Right Hand Menu"), .4f);
                        RecreateMenu();
                    }

                    if (UserInput.GetKeyDown(KeyCode.Space))
                    {
                        KeyboardInput += " ";
                        RigUtils.MyOfflineRig.PlayHandTapLocal(66, GetEnabled("Right Hand Menu"), .4f);
                        RecreateMenu();
                    }

                    if (UserInput.GetKeyDown(KeyCode.Return))
                    {
                        RecreateMenu();
                        RigUtils.MyOfflineRig.PlayHandTapLocal(66, GetEnabled("Right Hand Menu"), .4f);
                    }

                    if (UserInput.GetKeyDown(KeyCode.Escape))
                    {
                        RigUtils.MyOfflineRig.PlayHandTapLocal(66, GetEnabled("Right Hand Menu"), .4f);
                        inKeyboard = false;
                        RecreateMenu();
                    }
                } 
                else
                {
                    Settings.ColorKeyboard(GetEnabled("Outline Menu"));
                    Settings.SetUpKeyboard();
                }
                inputTextObject.text = KeyboardInput + ((Time.frameCount / 45 % 2) == 0 ? "|" : "");
            } else keyboard.SetActive(false);
            try
            {
                foreach (var btnss in buttons)
                    foreach (var btns in btnss)
                        foreach (var btn in btns)
                            if (btn.enabled)
                            {
                                if (btn.method != null)
                                    try { btn.method.Invoke(); }
                                    catch (Exception exc) { Debug.LogError($"{PluginInfo.Name} // Error with mod {btn.buttonText} at {exc.StackTrace}: {exc.Message}"); }
                            }
            } catch (Exception exc) { Debug.LogError($"<b>{PluginInfo.Name} | Fatal Error<b> at {exc.StackTrace} - {exc.Message}"); }
        }

        public static AudioClip[] AudioClips = new AudioClip[999];
        public static Texture2D[] LinkTextures = new Texture2D[999];
        public static Texture2D[] PathTextures = new Texture2D[999];
        public static GameObject[] BundleObjects = new GameObject[999];
        public static Texture2D[] MCTextures = new Texture2D[999];
        private static void Load()
        {
            if (!fastLoad)
            {
                for (int i = 0; i < Stringys[0].Length; i++)
                    AudioClips[i] ??= Loaders.GetAudioFromURL(Stringys[0][i]);
                for (int i = 0; i < Stringys[1].Length; i++)
                    LinkTextures[i] ??= Loaders.LoadImageFromURL(Stringys[1][i]);
            }
            for (int i = 0; i < Stringys[2].Length; i++)
                BundleObjects[i] ??= Loaders.LoadGameObject(Stringys[2][i].Split(':')[0], Stringys[2][i].Split(':')[1]);
            for (int i = 0; i < Stringys[3].Length; i++)
                PathTextures[i] ??= Loaders.LoadTexture(Stringys[3][i]);

            /*for (int i = 0; i < Stringys[4].Length; i++) well it was a good try and i dont think a menu being 84mb would look good anyway
                AudioClips[i] ??= Loaders.GetAudioClip(Stringys[4][i]);*/
        }

        public static string[][] Stringys =
        {
            new string[] // audio links 0
            {
                "https://drive.google.com/uc?export=download&id=1ikd7poEUHG84V7mgwTwWZ7I1QPp_rDIL", // enable button sound 0
                "https://drive.google.com/uc?export=download&id=1KTBZ-I7WdyZDyOioReq2dMxgvcJEnvWe", // disable button sound 1
                "https://drive.google.com/uc?export=download&id=14dAJ-FJl4jk3cJQ3LL9SByb30s8NJ-nQ", // notification sound 2
                "https://drive.google.com/uc?export=download&id=1w6qXUvebN15_pPoXvPJpHJ7CHI6fvvMW", // voice command listening sound 3
                "https://drive.google.com/uc?export=download&id=1LW_Hx6UjsKjS2lEySHiUx4dKohYwhxsN", // voice command stop listening sound 4
                "https://drive.google.com/uc?export=download&id=1n7hvtQbjyUGANoNm7ef1PYd5ZD9nviv_", // menu open sound 5
                "https://drive.google.com/uc?export=download&id=1of4T10GLBGcyy5zexLJ2bpyhA-bQRAH4", // menu close sound 6
                "https://drive.google.com/uc?export=download&id=1v7QQZun4xfMIbhe7aAYwQ-FSHImTavlS", // error sound 7
                "https://drive.google.com/uc?export=download&id=1uv0YZ8CtIqWe-e4WkJ7TXC4Axk3EPRcD", // page button sound 8
                "https://drive.google.com/uc?export=download&id=1JiS7vopdMjk6y0u2ZRoTzlq0yi5CyT_I", // living mice 9
                "https://drive.google.com/uc?export=download&id=1mHwNQOkl1KuIDZzkomqJx6Xi-O4CBibM", // clark 10
                "https://drive.google.com/uc?export=download&id=102oiIdZYJsiwvgJkbAulijsQH428XmRl", // danny 11
                "https://drive.google.com/uc?export=download&id=1oeRM8TDKBwW_HBdR0tWH_UDIJtInxp3o", // oxygene 12
                "https://drive.google.com/uc?export=download&id=1KvPZNXGrqptFwkh-yd1zgKXM_aEJaBMU", // key 13
                "https://drive.google.com/uc?export=download&id=1o6jZukZ8AS1wyywunyXZFN3rlnVvnIFG", // droopy likes your face 14
                "https://drive.google.com/uc?export=download&id=1PYRzcSQ7V1YCEmsRt3ueja_zH4U0xiGJ", // moog city 15
                "https://drive.google.com/uc?export=download&id=1ES_g5EzyOvfGsT7O8Za_59N9An8IuXm1", // moog city2 16
                "https://drive.google.com/uc?export=download&id=1Cxa1I-eygQoj-ktplYnls87y8i4Z2F0I", // subwoofer lullaby 17
                "https://drive.google.com/uc?export=download&id=1wIsTclGBBnBuTp2iaRPuOPlUb_fmzLZ4", // dog 18
                "https://drive.google.com/uc?export=download&id=1l6S2YAhNvl8YPqlxSzi1TvuM3ZJNPOMZ", // cat 19
                "https://drive.google.com/uc?export=download&id=1t57tp4j3jXir3NZZvLAx8Imu2HyEMsI9", // aria math 20
                "https://drive.google.com/uc?export=download&id=1YjJAyh2tYWzbdc1H7OBBq6dC2_4wJ6ym", // haggstorm 21
                "https://drive.google.com/uc?export=download&id=19sSkt2lR66Bp5gNfMxJ4MfPCGClIzuLU", // pigstep 22
                "https://drive.google.com/uc?export=download&id=1QzM51uwVEvWTunn8ZIPfLRhq5OlYTP7r", // pigstep (alan becker) 23
                "https://drive.google.com/uc?export=download&id=1-mKtFaEvtTDHOwhTIh8gsjQOkFSlJkvr", // minecraft block break 24
                "https://drive.google.com/uc?export=download&id=1HDEsDEUTgbpjxZJQZmJayxDTBVln-r7D", // minecraft block hit 25
                "https://drive.google.com/uc?export=download&id=1hRZMcsqMtvo-lpgoO7SSytLRISUUcLiW", // moon 2 (minecraft remake) 26
            },
            // fix paypal temp stuff (if i dont fix this and you see this on my github this is for a template that my friend made)
            new string[] // texture links 1
            {
                //"https://cdn.discordapp.com/attachments/1322758029325897738/1322866816745345044/Background-Gradient.png?ex=67726f3a&is=67711dba&hm=e6ade193fc44ee2707b5ee2625085253aa31959ba5368f3dd3e0746c7d6f6801&", // paypal grad
                //"https://cdn.discordapp.com/attachments/1322758029325897738/1322866956998672504/Paypal.png?ex=67726f5c&is=67711ddc&hm=5feccfe481cb069249d3b71e0c88a76e7840d92bcfb67d4cd21104528e2f530d&", // paypal
            },
            new string[] // asset bundle paths 2
            {
                //"MysticClient.Resources.adminplumbob:Admin Plumbob", // plumbob 0
                //"MysticClient.Resources.adminplanet:Planet", // planet 1
                "MysticClient.Resources.menubundle:MenuBundle", // minecraft texture bundle 0
                //"MysticClient.Resources.audioobjects:AudioObjects", menu audios 1
                "MysticClient.Resources.keyboard:Keyboard", // vr keyboard 1
                "MysticClient.Resources.modmanager:ModManager", // mod manager object 2
            },
            new string[] // menu image paths :3
            {
                "MysticClient.Resources.SettingIcon.png", // setting icon 0
                "MysticClient.Resources.WarningIcon.png", // warning icon 1
                "MysticClient.Resources.Search.png", // search icon 2
                "MysticClient.Resources.HomeIcon.png", // home icon 3
            },
            new string[] // audio object names 4
            {
                "ButtonEnable",
                "ButtonDisable",
                "MenuOpen",
                "MenuClose",
                "Notifications",
                "PageButton",
                "Error",
                "CommandOn",
                "CommandOff",
                "Living Mice",
                "Clark",
                "Danny",
                "Oxygene",
                "Key",
                "Droopy",
                "Moog City",
                "Moog City 2",
                "Subwoofer Lullaby",
                "Dog",
                "Cat",
                "Aria Math",
                "Haggstorm",
                "Pigstep (Stereo Mix)",
                "PigStep (feat. Aaron Grooves)",
                "MCBlockBreak",
                "MCBlockHit",
                "Moon2",
            },
        };
        public static void CreateMenu()
        {
            menu = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Destroy(menu.GetComponent<Rigidbody>());
            Destroy(menu.GetComponent<Renderer>());
            if (physicSetting != 2)
                Destroy(menu.GetComponent<BoxCollider>());
            menu.name = "MysticClientModMenu";
            menu.layer = 8;
            menu.transform.localScale = GetEnabled("Annoying Menu") ? new Vector3(.1f, Random.Range(.3f, .5f), parentSize.z) : parentSize;
            menuBackground = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Destroy(menuBackground.GetComponent<Rigidbody>());
            Destroy(menuBackground.GetComponent<BoxCollider>());
            menuBackground.transform.parent = menu.transform;
            menuBackground.transform.rotation = Quaternion.identity;
            menuBackground.transform.localScale = GetEnabled("Annoying Menu") ? new Vector3(Random.Range(.1f, .3f), Random.Range(1f, 5f), Random.Range(1f, 5f)) : menuSize;
            menuBackground.GetComponent<Renderer>().material = GetEnabled("Semi-Transparent Menu") ? TransparentMaterial(GetChangeColorA(NormalColor, .3f)) : NormalMaterial(NormalColor);
            menuBackground.transform.position = menuPos;
            if (GetEnabled("Outline Menu")) OutlineMenuObject(menuBackground);
            if (GetEnabled("Menu Trail"))
            {
                var trail = menu.AddComponent<TrailRenderer>();
                trail.material = TransparentMaterial(GetChangeColorA(GetEnabled("Make Menu Trail Color Follow Menu Color") ? NormalColor : menuTrailColor, GetEnabled("Semi-Transparent Menu") ? .5f : .8f));
                trail.time = .3f;
                trail.startWidth = .03f;
                trail.endWidth = 0;
                trail.minVertexDistance = .1f;
            }
            if (GetEnabled("Round Menu")) RoundMenuObject(menuBackground);
            canvasObject = new GameObject();
            canvasObject.transform.parent = menu.transform;
            var canvas = canvasObject.AddComponent<Canvas>();
            var canvasScaler = canvasObject.AddComponent<CanvasScaler>();
            canvasObject.AddComponent<GraphicRaycaster>();
            canvas.renderMode = RenderMode.WorldSpace;
            canvasScaler.dynamicPixelsPerUnit = 1000f;
            var text = new GameObject
            {
                transform =
                {
                    parent = canvasObject.transform
                }
            }.AddComponent<Text>();
            text.font = currentFont;
            text.text = GetEnabled("Annoying Menu") ? DzongkhaMenuName : menuTitle;
            text.fontSize = 1;
            text.supportRichText = true;
            text.fontStyle = titleFontStyle;
            text.alignment = TextAnchor.MiddleCenter;
            text.resizeTextForBestFit = true;
            text.resizeTextMinSize = 0;
            var component = text.GetComponent<RectTransform>();
            component.localPosition = Vector3.zero;
            component.sizeDelta = menuTitleSize;
            component.position = new Vector3(0.06f, 0f, menuTitleZ);
            component.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));
            if (GetEnabled("Side Disconnect"))
            {
                var matr = GetEnabled("Semi-Transparent Menu") ? TransparentMaterial(GetChangeColorA(NormalColor, .3f)) : NormalMaterial(NormalColor);
                CreateButton("Disconnect", "DisconnectingButton", disconnectPos, disconnectSize, sideTextSize, matr);
            }
            if (GetEnabled("Destroy Networked Objects Side Button"))
            {
                var matr = GetEnabled("Semi-Transparent Menu") ? TransparentMaterial(GetChangeColorA(NormalColor, .3f)) : NormalMaterial(NormalColor);
                CreateButton("Destroy Networked Objects", "DestroyButton", disconnectPos + new Vector3(0, 0, .18f), disconnectSize, sideTextSize, matr);
            }
            if (GetToolTip("Changed Menu Theme").buttonText.Contains("AZ"))
            {
                var txt = PhotonSystem.InRoom ? $"Players in room: {PhotonSystem.RoomPlayerCount}\nRoom Name: {PhotonSystem.RoomName}" : "<color=red>NOT IN ROOM</color>";
                var mat = GetEnabled("Semi-Transparent Menu") ? TransparentMaterial(GetChangeColorA(Color.black, .3f)) : NormalMaterial(Color.black);
                CreateButton(txt, "", new Vector3(.56f, 0f, -.7f), new Vector3(.09f, .9f, .25f), sideTextSize, mat, false);
                if (!GetEnabled("Side Disconnect"))
                {
                    var mater = GetEnabled("Semi-Transparent Menu") ? TransparentMaterial(GetChangeColorA(Color.black, .3f)) : NormalMaterial(Color.black);
                    CreateButton("Disconnect", "DisconnectingButton", new Vector3(.56f, 0, .53f), new Vector3(.045f, .9f, .17f), sideTextSize, mater);
                }
            }
            if (GetEnabled("Return Button"))
            {
                var a = GetToolTip("Changed Menu Theme").buttonText.Contains("NXO");
                var btn = GameObject.CreatePrimitive(PrimitiveType.Cube);
                btn.Destroy<Rigidbody>();
                if (!UserInput.GetKey(KeyCode.Q)) btn.layer = GetEnabled("No Button Colliders") ? 2 : 0;
                btn.GetComponent<BoxCollider>().isTrigger = true;
                btn.transform.parent = menu.transform;
                btn.transform.rotation = Quaternion.identity;
                btn.transform.localScale = a ? new Vector3(.045f, .30625f, .08f) : new Vector3(.09f, .102f, .08f);
                btn.transform.localPosition = a ? new Vector3(.505f, 0, -.31f) : new Vector3(.56f, search.x + returnAddAmount.x, search.y + returnAddAmount.y);
                btn.AddComponent<BtnCollider>().relatedText = "Return";
                btn.ChangeMaterial(GetEnabled("Semi-Transparent Menu") ? TransparentMaterial(GetChangeColorA(NormalColor, .3f)) : NormalMaterial(NormalColor));
                if (GetEnabled("Outline Menu")) OutlineMenuObject(btn);
                if (GetEnabled("Round Menu")) RoundMenuObject(btn);
                ImageMenuButton(btn, PathTextures[3]);

                //var mat = GetEnabled("Semi-Transparent Menu") ? TransparentMaterial(GetChangeColorA(NormalColor, .3f)) : NormalMaterial(NormalColor);
                //CreateButton("Return", "Return", new Vector3(.505f, 0, -.31f), new Vector3(.045f, .30625f, .08f), menuTextSize, mat);
            }

            var button = GameObject.CreatePrimitive(PrimitiveType.Cube);
            button.Destroy<Rigidbody>();
            if (!UserInput.GetKey(KeyCode.Q)) button.layer = GetEnabled("No Button Colliders") ? 2 : 0;
            button.GetComponent<BoxCollider>().isTrigger = true;
            button.transform.parent = menu.transform;
            button.transform.rotation = Quaternion.identity;
            button.transform.localScale = new Vector3(.09f, .102f, .08f);
            button.transform.localPosition = new Vector3(.56f, search.x, search.y);
            button.AddComponent<BtnCollider>().relatedText = "Search";
            button.ChangeMaterial(GetEnabled("Semi-Transparent Menu") ? TransparentMaterial(GetChangeColorA(NormalColor, .3f)) : NormalMaterial(NormalColor));
            if (GetEnabled("Outline Menu")) OutlineMenuObject(button);
            if (GetEnabled("Round Menu")) RoundMenuObject(button);
            ImageMenuButton(button, PathTextures[2]);


            int lastPage = ((buttons[easyPage][buttonsType].Length + pageSize - 1) / pageSize) - 1;
            int next = pageNumber + 1; int last = pageNumber - 1;
            if (next > lastPage) next = 0; if (last < 0) last = lastPage;
            var pageColor = GetToolTip("Changed Menu Theme").buttonText.Contains("AZ") ? Color.black : NormalColor;
            var pageMat = GetEnabled("Semi-Transparent Menu") ? TransparentMaterial(GetChangeColorA(pageColor, .3f)) : NormalMaterial(pageColor);
            var usingTheme = GetToolTip("Changed Menu Theme").buttonText.Contains("Mango");
            CreateButton(usingTheme ? $"[{last}] << Prev" : lastPageText, "PreviousPage", pagePoss[0], pageButtonSize, menuTextSize, pageMat);
            CreateButton(usingTheme ? $"Next >> [{next}]" : nextPageText, "NextPage", pagePoss[1], pageButtonSize, menuTextSize, pageMat);

            var tooltipObj = new GameObject();
            tooltipObj.transform.SetParent(canvasObject.transform);
            tooltipObj.transform.localPosition = new Vector3(0, 0, 1.2f);
            tooltipText = tooltipObj.GetComponent<Text>();
            if (tooltipText == null) tooltipText = tooltipObj.AddComponent<Text>();
            tooltipText.font = currentFont;
            tooltipText.text = tooltipString;
            tooltipText.fontSize = 18;
            tooltipText.alignment = TextAnchor.MiddleCenter;
            tooltipText.resizeTextForBestFit = true;
            tooltipText.resizeTextMinSize = 0;
            tooltipText.color = Color.white;
            var componenttooltip = tooltipObj.GetComponent<RectTransform>();
            componenttooltip.localPosition = Vector3.zero;
            componenttooltip.sizeDelta = menuTextSize;
            componenttooltip.position = new Vector3(.06f, 0, toolTipZ);
            componenttooltip.rotation = Quaternion.Euler(new Vector3(180, 90, 90));
            var activeButtons = buttons[easyPage][buttonsType].Skip(pageNumber * pageSize).Take(pageSize).ToArray();
            for (int i = 0; i < activeButtons.Length; i++)
                if (!inKeyboard)
                    CreateModButton(i * internalButtonOffset + internalButtonAddOffset, activeButtons[i]);

            if (inKeyboard)
            {
                var gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
                gameObject.Destroy<Rigidbody>();
                if (!UserInput.GetKey(KeyCode.Q)) gameObject.layer = GetEnabled("No Button Colliders") ? 2 : 0;
                gameObject.GetComponent<BoxCollider>().isTrigger = true;
                gameObject.transform.parent = menu.transform;
                gameObject.transform.rotation = Quaternion.identity;
                gameObject.transform.localScale = GetEnabled("Annoying Menu") ? new Vector3(.09f, Random.Range(1f, 5f), Random.Range(.08f, .1f)) : buttonSize;
                gameObject.transform.localPosition = new Vector3(buttonPos.x, buttonPos.y, buttonPos.z);
                if (GetEnabled("Outline Menu")) OutlineMenuObject(gameObject);
                if (GetEnabled("Round Menu")) RoundMenuObject(gameObject);
                inputTextObject = new GameObject
                {
                    transform =
                    {
                        parent = canvasObject.transform
                    }
                }.AddComponent<Text>();
                inputTextObject.font = currentFont;
                inputTextObject.color = buttonTextColor;
                inputTextObject.supportRichText = true;
                inputTextObject.fontSize = 1;
                gameObject.GetComponent<Renderer>().material = GetEnabled("Semi-Transparent Menu") ? TransparentMaterial(GetChangeColorA(NormalColor, .3f)) : NormalMaterial(NormalColor);
                inputTextObject.alignment = TextAnchor.MiddleCenter;
                inputTextObject.fontStyle = buttonFontStyle;
                inputTextObject.resizeTextForBestFit = true;
                inputTextObject.resizeTextMinSize = 0;
                var componentino = inputTextObject.GetComponent<RectTransform>();
                componentino.sizeDelta = menuTextSize;
                componentino.localPosition = gameObject.transform.position + new Vector3(GetEnabled("Semi-Transparent Menu") ? .02f : .01f, 0, 0);
                componentino.rotation = Quaternion.Euler(new Vector3(180, 90, 90));
            }

            var searchedMods = new List<ButtonInfo> { };
            if (inKeyboard)
            foreach (var btnss in buttons)
                foreach (var btns in btnss)
                    foreach (var btn in btns)
                    {
                        try
                        {
                            if (btn.buttonText.ToLower().Contains(KeyboardInput.ToLower()))
                                searchedMods.Add(btn);
                        } catch { }
                    }
            var foundButtons = searchedMods.Skip(pageNumber * (pageSize - 1)).Take(pageSize - 1).ToArray();
            if (inKeyboard) for (int i = 0; i < foundButtons.Length; i++)
                    CreateModButton((i + 1) * internalButtonOffset + internalButtonAddOffset, foundButtons[i]);
        }

        public static void CreateButton(string buttonText, string relatedText, Vector3 position, Vector3 scale, Vector2 textSize, Material material, bool hasCollider = true)
        {
            var button = GameObject.CreatePrimitive(PrimitiveType.Cube);
            button.Destroy<Rigidbody>();
            if (!UserInput.GetKey(KeyCode.Q)) button.layer = GetEnabled("No Button Colliders") ? 2 : 0;
            if (!hasCollider) button.Destroy<Collider>();
            if (hasCollider) button.GetComponent<BoxCollider>().isTrigger = true;
            button.transform.parent = menu.transform;
            button.transform.rotation = Quaternion.identity;
            button.transform.localScale = scale;
            button.transform.localPosition = position;
            if (hasCollider) button.AddComponent<BtnCollider>().relatedText = relatedText;
            button.GetComponent<Renderer>().material = GetEnabled("Semi-Transparent Menu") ? TransparentMaterial(GetChangeColorA(NormalColor, .3f)) : NormalMaterial(NormalColor);
            if (GetEnabled("Outline Menu")) OutlineMenuObject(button);
            if (GetEnabled("Round Menu")) RoundMenuObject(button);
            var text = new GameObject
            {
                transform =
                {
                    parent = canvasObject.transform
                }
            }.AddComponent<Text>();
            text.text = buttonText;
            text.font = currentFont;
            text.fontSize = 1;
            text.alignment = TextAnchor.MiddleCenter;
            text.fontStyle = FontStyle.BoldAndItalic;
            text.resizeTextForBestFit = true;
            text.resizeTextMinSize = 0;
            var rect = text.GetComponent<RectTransform>();
            rect.sizeDelta = textSize;
            rect.localPosition = button.transform.position + new Vector3(.01f, 0, 0);
            rect.rotation = Quaternion.Euler(new Vector3(180, 90, 90));
        }

        public static void CreateModButton(float offset, ButtonInfo method)
        {
            var gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            gameObject.Destroy<Rigidbody>();
            if (!UserInput.GetKey(KeyCode.Q)) gameObject.layer = GetEnabled("No Button Colliders") ? 2 : 0;
            gameObject.GetComponent<BoxCollider>().isTrigger = true;
            gameObject.transform.parent = menu.transform;
            gameObject.transform.rotation = Quaternion.identity;
            if (method.settingAction != null)
            {
                gameObject.transform.localScale = new Vector3(buttonSize.x, buttonSize.y - .15f, buttonSize.z);
                gameObject.transform.localPosition = new Vector3(buttonPos.x, buttonPos.y + .075f, buttonPos.z - offset / buttonOffset);
                var settingObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                settingObj.Destroy<Rigidbody>();
                //settingObj.ChangeMaterial(new Material(DefaultShader) { mainTexture = PathTextures[0] });
                settingObj.ChangeMaterial(GetEnabled("Semi-Transparent Menu") ? TransparentMaterial(GetChangeColorA(NormalColor, .3f)) : NormalMaterial(NormalColor));
                if (!UserInput.GetKey(KeyCode.Q)) settingObj.layer = GetEnabled("No Button Colliders") ? 2 : 0;
                settingObj.GetComponent<BoxCollider>().isTrigger = true;
                settingObj.transform.parent = menu.transform;
                settingObj.transform.localScale = new Vector3(buttonSize.x, .1f, buttonSize.z);
                settingObj.transform.localPosition = new Vector3(buttonPos.x, buttonPos.y - settingButtonOffset, buttonPos.z - offset / buttonOffset);
                settingObj.transform.rotation = Quaternion.identity;
                if (GetEnabled("Outline Menu"))  OutlineMenuObject(settingObj);
                if (GetEnabled("Round Menu")) RoundMenuObject(settingObj);
                settingObj.AddComponent<BtnCollider>().relatedText = $"{method.buttonText}_SettingSide";

                if (GetEnabled("Round Menu")) RoundMenuObject(settingObj);
                var image = new GameObject { transform = { parent = canvasObject.transform } }.AddComponent<Image>();
                var materer = new Material(image.material);
                image.material = materer;
                image.material.SetTexture("_MainTex", PathTextures[0]);
                var recter = image.GetComponent<RectTransform>();
                recter.sizeDelta = new Vector2(.03f, .03f);
                recter.localPosition = settingObj.transform.position + new Vector3(.01f, 0, 0);
                recter.rotation = Quaternion.Euler(new Vector3(180, 90, 90));
            }
            else
            {
                gameObject.transform.localScale = GetEnabled("Annoying Menu") ? new Vector3(.09f, Random.Range(1f, 5f), Random.Range(.08f, .1f)) : buttonSize;
                gameObject.transform.localPosition = new Vector3(buttonPos.x, buttonPos.y, buttonPos.z - offset / buttonOffset);
            }
            gameObject.AddComponent<BtnCollider>().relatedText = method.buttonText;
            if (GetEnabled("Outline Menu")) OutlineMenuObject(gameObject);
            if (GetEnabled("Round Menu")) RoundMenuObject(gameObject);
            var text = new GameObject
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
            var color = method.enabled ? ButtonColorEnabled : ButtonColorDisable;
            gameObject.GetComponent<Renderer>().material = GetEnabled("Semi-Transparent Menu") ? TransparentMaterial(GetChangeColorA(color, .3f)) : NormalMaterial(color);
            text.alignment = TextAnchor.MiddleCenter;
            text.fontStyle = buttonFontStyle;
            text.resizeTextForBestFit = true;
            text.resizeTextMinSize = 0;
            var component = text.GetComponent<RectTransform>();
            //component.localPosition = Vector3.zero;
            component.sizeDelta = menuTextSize;
            //component.localPosition = new Vector3(buttonTextPos.x, buttonTextPos.y, buttonTextPos.z - offset / buttonTextOffset); // 2.6
            component.localPosition = gameObject.transform.position + new Vector3(GetEnabled("Semi-Transparent Menu") ? .02f : .01f, 0, 0);
            component.rotation = Quaternion.Euler(new Vector3(180, 90, 90));
        }
        public static void DestroyMenu()
        {
            menu.Destroy();
            canvasObject.Destroy();
            reference.Destroy();
            referenceOther.Destroy();
            menu = null;
            menuBackground = null;
            canvasObject = null;
            reference = null;
            referenceOther = null;
        }
        public static void RecreateMenu()
        {
            if (menu != null)
            {
                menu.Destroy();
                menu = null;
                CreateMenu();
                RecenterMenu(GetEnabled("Right Hand Menu"), UserInput.GetKey(KeyCode.Q));
            }
        }
        public static void RecenterMenu(bool isRightHanded, bool isKeyboardCondition)
        {
            if (!isKeyboardCondition)
            {
                if (!inKeyboard)
                {
                    if (!GetEnabled("Face Menu"))
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
                        return;
                    }
                    menu.transform.position = RigUtils.MyOnlineRig.headCollider.transform.position + Vector3.Scale(RigUtils.MyOnlineRig.headCollider.transform.forward, new Vector3(.75f, .75f, .75f));
                    menu.transform.LookAt(RigUtils.MyPlayer.headCollider.transform.position);
                    menu.transform.RotateAround(menu.transform.position, menu.transform.forward, 90f);
                    menu.transform.RotateAround(menu.transform.position, menu.transform.up, 90f);
                    menu.transform.RotateAround(menu.transform.position, menu.transform.forward, 180f);
                    return;
                }
                else
                {
                    if (Vector3.Distance(keyboard.transform.position, RigUtils.MyOnlineRig.bodyCollider.transform.position) > 5)
                    {
                        menu.transform.position = RigUtils.MyOnlineRig.headCollider.transform.position + Vector3.Scale(RigUtils.MyOnlineRig.headCollider.transform.forward, new Vector3(.75f, .75f, .75f)) + (Vector3.up / .5f);
                        menu.transform.LookAt(RigUtils.MyPlayer.headCollider.transform.position);
                        menu.transform.RotateAround(menu.transform.position, menu.transform.forward, 90f);
                        menu.transform.RotateAround(menu.transform.position, menu.transform.up, 90f);
                        menu.transform.RotateAround(menu.transform.position, menu.transform.forward, 180f);

                        keyboard.transform.position = RigUtils.MyOnlineRig.bodyCollider.transform.position;
                        keyboard.transform.rotation = RigUtils.MyOnlineRig.bodyCollider.transform.rotation;
                    }
                }
            }
            else
            {
                try { TPC = GameObject.Find("Player Objects/Third Person Camera/Shoulder Camera").GetComponent<Camera>(); } catch { }
                TPC.gameObject.GetNamedChild("CM vcam1").SetActive(false);
                if (TPC != null || inKeyboard)
                {
                    TPC.transform.position = new Vector3(100, 100, 100);
                    TPC.transform.LookAt(RigUtils.MyOnlineRig.transform.position);
                    menu.transform.parent = TPC.transform;
                    menu.transform.position = TPC.transform.position + Vector3.Scale(TPC.transform.forward, GetToolTip("Changed Menu Theme").buttonText.Contains("AZ") ? new Vector3(0.7f, 0.68f, 0.7f) : new Vector3(0.6f, 0.545f, 0.6f)) + Vector3.Scale(TPC.transform.up, new Vector3(-0.02f, .03f, -0.02f));
                    menu.transform.LookAt(TPC.transform.position);
                    menu.transform.RotateAround(menu.transform.position, menu.transform.forward, 90f);
                    menu.transform.RotateAround(menu.transform.position, menu.transform.up, 90f);
                    menu.transform.RotateAround(menu.transform.position, menu.transform.forward, 180f);
                    if (reference != null)
                        if (UserInput.GetMouseButton(0) || UserInput.GetMouseButton(2))
                        {
                            if (Physics.Raycast(TPC.ScreenPointToRay(Mouse.current.position.ReadValue()), out var hit, 100))
                                hit.transform.gameObject.GetComponent<BtnCollider>()?.OnTriggerEnter(reference.GetComponent<Collider>());
                        } else reference.transform.position = new Vector3(999, -999, -999);
                    else CreateReference(GetEnabled("Right Hand Menu"));
                }
            }
        }

        public static void CreateReference(bool isRightHanded)
        {
            if (GetEnabled("Face Menu") || inKeyboard)
            {
                reference = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                reference.transform.parent = RigUtils.MyOnlineRig.rightHandTransform;
                reference.GetComponent<Renderer>().material = GetEnabled("Semi-Transparent Menu") ? TransparentMaterial(GetChangeColorA(new Color32(167, 17, 237, 28), .3f)) : NormalMaterial(new Color32(167, 17, 237, 28));
                reference.transform.localPosition = pointerPosition;
                reference.transform.localScale = new Vector3(.01f, .01f, .01f);
                reference.name = "buttonPresser";
                referenceOther = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                referenceOther.transform.parent = RigUtils.MyOnlineRig.leftHandTransform;
                referenceOther.GetComponent<Renderer>().material = GetEnabled("Semi-Transparent Menu") ? TransparentMaterial(GetChangeColorA(new Color32(167, 17, 237, 28), .3f)) : NormalMaterial(new Color32(167, 17, 237, 28));
                referenceOther.transform.localPosition = pointerPosition;
                referenceOther.transform.localScale = new Vector3(.01f, .01f, .01f);
                referenceOther.name = "buttonPresser_Left";
                return;
            }
            reference = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            reference.transform.parent = isRightHanded ? RigUtils.MyOnlineRig.leftHandTransform : RigUtils.MyOnlineRig.rightHandTransform;
            reference.GetComponent<Renderer>().material = GetEnabled("Semi-Transparent Menu") ? TransparentMaterial(GetChangeColorA(new Color32(167, 17, 237, 28), .3f)) : NormalMaterial(new Color32(167, 17, 237, 28));
            reference.transform.localPosition = pointerPosition;
            reference.transform.localScale = new Vector3(.01f, .01f, .01f);
            reference.name = "buttonPresser";
        }

        private static void OutlineMenuObject(GameObject obj)
        {
            if (!GetEnabled("Semi-Transparent Menu")) // looks like shit when on
            {
                var outline = GameObject.CreatePrimitive(PrimitiveType.Cube);
                outline.Destroy<Rigidbody>();
                outline.Destroy<Collider>();
                outline.transform.parent = menu.transform;
                outline.transform.rotation = Quaternion.identity;
                var sub = GetToolTip("Changed Menu Theme").buttonText.Contains("NXO") ? .008f : .01f;
                outline.transform.localScale = new Vector3(obj.transform.localScale.x - sub, obj.transform.localScale.y + sub, obj.transform.localScale.z + sub);
                outline.transform.position = obj.transform.position;
                outline.GetComponent<Renderer>().material = NormalMaterial(outlineColor);
                if (GetEnabled("Round Menu")) RoundMenuObject(outline);
            }
        }

        public static void Toggle(string buttonText)
        {
            int lastPage = ((buttons[easyPage][buttonsType].Length + pageSize - 1) / pageSize) - 1;
            if (buttonText == "PreviousPage")
            {
                pageNumber--;
                if (pageNumber < 0)
                    pageNumber = lastPage;
            }
            else
            {
                if (buttonText == "NextPage")
                {
                    pageNumber++;
                    if (pageNumber > lastPage)
                        pageNumber = 0;
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
                            if (buttonText == "Return")
                            {
                                Settings.EnterPage(0, 0, 0);
                            }
                            else
                            {
                                if (buttonText == "Search")
                                {
                                    inKeyboard = true;
                                    if (!GetEnabled("Current Catagory Searching"))
                                        Settings.EnterPage(0, 0, 0);
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
                                                if (target.buttonText == "RGBSnowballs")
                                                    throwables = null;
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
                                    } else { if (buttonText.Contains("SettingSide")) GetIndex(buttonText.Split('_')[0]).settingAction.Invoke(); else Debug.LogError(buttonText + " does not exist"); }
                                }
                            }
                        }          
                    }
                }
            }
            RecreateMenu();
        }
        public static void PlayButtonSound(string index)
        {
            if (GetEnabled("Dynamic Sounds"))
            {
                var target = GetIndex(index);
                if (target != null)
                {
                    if (target.isTogglable)
                    {
                        if (!target.enabled)
                            menuHandAudio.PlayOneShot(AudioClips[0]); // enable 0
                        else menuHandAudio.PlayOneShot(AudioClips[1]); // disable 1
                    }
                    else if (target.method != null)
                        menuHandAudio.PlayOneShot(AudioClips[0]);
                }
                else
                {
                    if (index == "NextPage") menuHandAudio.PlayOneShot(AudioClips[8]);
                    else if (index == "PreviousPage") menuHandAudio.PlayOneShot(AudioClips[8]);
                    else if (index == "DisconnectingButton") menuHandAudio.PlayOneShot(AudioClips[8]);
                    else if (index == "DestroyButton") menuHandAudio.PlayOneShot(AudioClips[8]);
                    else if (index.Contains("SettingSide")) menuHandAudio.PlayOneShot(AudioClips[8]);
                    else if (index.Contains("Return")) menuHandAudio.PlayOneShot(AudioClips[8]);
                    if (target == null && index != "NextPage" && index != "PreviousPage" && index != "DisconnectingButton" && index != "DestroyButton" && !index.Contains("SettingSide") && index != "Return")
                        Debug.LogError($"button {index} does not exist");
                }
                menuHandAudio.volume = .3f;
            }
            else
            {
                /*if (GetEnabled("ServerSided Button Sounds")) this made the button not be able to disable idk why
                {
                    if (PhotonSystem.InRoom)
                    {
                        RigUtils.MyOfflineRig.PlayHandTapLocal(buttonSound, GetEnabled("Right Hand Menu"), 0.4f);
                        RPCManager.SoundEvent(RpcTarget.Others, new object[]
                        {
                         buttonSound,
                         GetEnabled("Right Hand Menu"),
                         99999f
                        });
                        RPCProtection(RigUtils.MyNetPlayer);
                    }
                    else RigUtils.MyOfflineRig.PlayHandTapLocal(buttonSound, GetEnabled("Right Hand Menu"), .4f);
                } else */
                RigUtils.MyOfflineRig.PlayHandTapLocal(buttonSound, GetEnabled("Right Hand Menu"), .4f);
            }
        }
        public static ButtonInfo GetIndex(string buttonText)
        {
            foreach (var buttonss in buttons)
                foreach (var buttons in buttonss)
                    foreach (var button in buttons)
                        if (button.buttonText == buttonText)
                            return button;
            return null;
        }
        public static ButtonInfo GetToolTip(string tooltip)
        {
            foreach (var buttonss in buttons)
                foreach (var buttons in buttonss)
                    foreach (var button in buttons)
                        if (button.toolTip == tooltip)
                            return button;
            return null;
        }
        public static bool GetEnabled(string Btnname)
        {
            foreach (var buttonss in buttons)
                foreach (var buttons in buttonss)
                    foreach (var button in buttons)
                        if (button.buttonText == Btnname)
                            return button.enabled;
            return false;
        }
        private static float bypassDelay = 0;
        private static bool hasBypassed;
        public static void RPCProtection(bool better = false)
        {
            if (better)
            {
                if (Time.time - bypassDelay < 5) // thanks to @drew008278 for this 
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
                } catch (Exception ex) { Debug.LogError($"Error in RPCProtection: {ex.Message}"); }
            }
            else
            {
                GorillaNot.instance.rpcErrorMax = int.MaxValue;
                GorillaNot.instance.rpcCallLimit = int.MaxValue;
                GorillaNot.instance.logErrorMax = int.MaxValue;
                PhotonNetwork.MaxResendsBeforeDisconnect = int.MaxValue;
                PhotonNetwork.QuickResends = int.MaxValue;
                PhotonNetwork.RemoveRPCs(RigUtils.MyRealtimePlayer);
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
        public static void LegacySendEvent(in byte code, in object evData, Photon.Realtime.Player target, bool reliable) => LegacySendEvent(code, evData, new RaiseEventOptions { TargetActors = new int[1] { target.ActorNumber } }, reliable);
        public static void SendEvent(in byte code, in object evData, in NetEventOptions neo, bool reliable)
        {
            object[] objects = { NetworkSystem.Instance.ServerTimestamp, code, evData };
            NetworkSystemRaiseEvent.RaiseEvent(3, objects, neo, reliable);
        }
        public static void SendEvent(in byte code, in object evData, in NetPlayer target, bool reliable)
        {
            NetworkSystemRaiseEvent.neoTarget.TargetActors[0] = target.ActorNumber;
            SendEvent(code, evData, NetworkSystemRaiseEvent.neoTarget, reliable);
        }
        public static void SendRPC(PhotonView view, string method, RpcTarget target, object[] args) => view.RPC(method, target, args);
        public static void SendRPC(PhotonView view, string method, NetPlayer target, object[] args) => view.RPC(method, RigUtils.GetPlayerFromNet(target), args);
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
            var path = "";
            while (transform.parent != null)
            {
                transform = transform.parent;
                path = string.IsNullOrEmpty(path) ? transform.name : transform.name + "/" + path;
            }
            return path;
        }
        public static GameObject GetObjectByName(string objectName)
        {
            foreach (var obj in GetObjectsByName(objectName))
                return obj;
            return null;
        }
        public static GameObject[] GetObjectsByName(string objectName)
        {
            var objects = new List<GameObject>();
            foreach (var obj in GetGameObjects())
                if (obj.name == objectName)
                    objects.Add(obj);
            return objects.ToArray();
        }
        public static GameObject GetObjectByNames(string[] objectNames)
        {
            foreach (var gameObject in GetGameObjects())
                foreach (string names in objectNames)
                    if (gameObject.name == names)
                        return gameObject;
            return null;
        }
        public static GliderHoldable[] Gliders()
        {
            gliders ??= FindObjectsOfType<GliderHoldable>(true);
            return gliders;
        }
        public static GameObject[] GetGameObjects()
        {
            gameObjects ??= FindObjectsOfType<GameObject>(true);
            return gameObjects;
        }
        public static BuilderPiece[] GetPieces()
        {
            builderPieces ??= FindObjectsOfType<BuilderPiece>();
            return builderPieces;
        }
        public static GorillaKeyboardButton[] GetKeyboardButtons()
        {
            keyboardButtons ??= FindObjectsOfType<GorillaKeyboardButton>();
            return keyboardButtons;
        }
        public static SnowballThrowable[] GetThrowables()
        {
            throwables ??= FindObjectsOfType<SnowballThrowable>();
            return throwables;
        }
        public static GorillaRopeSwing[] GetRopes()
        {
            ropes ??= FindObjectsOfType<GorillaRopeSwing>();
            return ropes;
        }
        public static void JoinRoom(string name, JoinType joinType) => NetworkController.AttemptToJoinSpecificRoom(name, joinType);
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
        public static bool IsTagged(VRRig rig)
        {
            if (GorillaTagManager.currentInfected.Contains(RigUtils.GetNetFromRig(rig)))
                return true;
            return false;
        }

        public static async Task<string> GetInfoFromPlayer(NetPlayer player)
        {
            var realtime = RigUtils.GetPlayerFromNet(player);
            var request = new GetAccountInfoRequest { PlayFabId = realtime.UserId };
            var tsc = new TaskCompletionSource<string>();
            PlayFabClientAPI.GetAccountInfo(request, (GetAccountInfoResult result) =>
            {
                var info = string.Format("{0}\r\n{1:yyyy/MM/dd}", realtime.NickName, result.AccountInfo.Created);
                tsc.SetResult(info);
            }, error => { tsc.SetException(new Exception("Failed to get account info")); });
            return await tsc.Task;
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
            var time = Time.time;
            var r = Mathf.Sin(time * 2f) * .5f + .5f;
            var g = Mathf.Sin(time * 1.5f) * .5f + .5f;
            var b = Mathf.Sin(time * 2.5f) * .5f + .05f;
            return new Color(r, g, b, 1);
        }
        public static Color HardColor(int index) 
        {
            if (GetEnabled("Use System Colors"))
                return SCToUC(Settings.scolor[index % Settings.scolor.Length]);
            else return Settings.colors[index % Settings.colors.Length];
        }

        public static Material TransparentMaterial(Color color)
        {
            var material = new Material(DefaultShader) { color = color };
            material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            material.SetInt("_ZWrite", 0);
            material.renderQueue = 3000;
            return material;
        }

        public static Color SCToUC(SColor color) => new Color32(color.R, color.G, color.B, color.A);

        public static Color GetChangeColorA(Color original, float a)
        {
            var col = original;
            col.a = a;
            return col;
        }

        public static Material NormalMaterial(Color color) => new Material(GetEnabled("Shiny Menu") ? UniversalShader : UberShader) { color = color };

        public static Vector3 RoundToGrid(Vector3 position)
        {
            var x = Mathf.Round(position.x / gridSize) * gridSize;
            var y = Mathf.Round(position.y / gridSize) * gridSize;
            var z = Mathf.Round(position.z / gridSize) * gridSize;
            return new Vector3(x, y, z);
        }

        public static void CreateText(string text, Vector3 pos, Quaternion rot)
        {
            var textObj = new GameObject($"New_Text[{text}]").AddComponent<TextMesh>();
            textObj.fontSize = 100;
            textObj.fontStyle = buttonFontStyle;
            textObj.font = currentFont;
            textObj.characterSize = .1f;
            textObj.anchor = TextAnchor.MiddleCenter;
            textObj.alignment = TextAlignment.Center;
            textObj.color = NormalColor;
            textObj.transform.position = pos;
            textObj.transform.rotation = rot;
            textObj.text = text;
            textObj.Destroy(Time.deltaTime);
        }

        public static void DrawLine(Vector3 startPos, Vector3 endPos, Color color, float width)
        {
            var line = new GameObject("Line");
            var liner = line.AddComponent<LineRenderer>();
            liner.SetWidth(width);
            liner.positionCount = 2;
            liner.useWorldSpace = true;
            liner.SetPosition(0, startPos);
            liner.SetPosition(1, endPos);
            liner.material = new Material(UberShader) { color = color };
            line.Destroy(Time.deltaTime);
        }

        public static string RandomText(int length)
        {
            var text = "";
            for (int i = 0; i < length; i++)
                text += NetworkSystem.roomCharacters.Substring(Random.Range(0, NetworkSystem.roomCharacters.Length), 1);
            if (GorillaComputer.instance.CheckAutoBanListForName(text)) return text;
            return RandomText(length);
        }

        private static void RoundMenuObject(GameObject obj, float bevel = .02f)
        {
            var roundRend = obj.GetComponent<Renderer>();
            var oldScale = obj.transform.localScale;
            var oldPos = obj.transform.localPosition;
            GameObject CreatePrimitive(PrimitiveType type, Vector3 pos, Vector3 scale, Quaternion rot)
            {
                var gameObj = GameObject.CreatePrimitive(type);
                var renderer = gameObj.GetComponent<Renderer>();
                renderer.enabled = roundRend.enabled;
                Destroy(gameObj.GetComponent<Collider>());
                gameObj.transform.parent = menu.transform;
                gameObj.transform.localPosition = pos;
                gameObj.transform.localScale = scale;
                gameObj.transform.rotation = rot;
                return gameObj;
            }
            var scaleA = oldScale + new Vector3(0, bevel * -2.55f, 0);
            var scaleB = oldScale + new Vector3(0, 0, -bevel * 2);
            var baseA = CreatePrimitive(PrimitiveType.Cube, oldPos, scaleA, Quaternion.identity);
            var baseB = CreatePrimitive(PrimitiveType.Cube, oldPos, scaleB, Quaternion.identity);
            var halfY = oldScale.y / 2f;
            var halfZ = oldScale.z / 2f;
            var cornerScale = new Vector3(bevel * 2.55f, oldScale.x / 2f, bevel * 2f);
            var cornerRot = Quaternion.Euler(0, 0, 90);
            var corners = new GameObject[]
            {
                CreatePrimitive(PrimitiveType.Cylinder, oldPos + new Vector3(0, halfY - (bevel * 1.275f), halfZ - bevel), cornerScale, cornerRot),
                CreatePrimitive(PrimitiveType.Cylinder, oldPos + new Vector3(0, -halfY + (bevel * 1.275f), halfZ - bevel), cornerScale, cornerRot),
                CreatePrimitive(PrimitiveType.Cylinder, oldPos + new Vector3(0, halfY - (bevel * 1.275f), -halfZ + bevel), cornerScale, cornerRot),
                CreatePrimitive(PrimitiveType.Cylinder, oldPos + new Vector3(0, -halfY + (bevel * 1.275f), -halfZ + bevel), cornerScale, cornerRot)
            };
            var allObjs = new GameObject[] { baseA, baseB }.Concat(corners).ToArray();
            foreach (var objs in allObjs)
            {
                var changer = objs.AddComponent<ColorChanger.Clamper>();
                changer.target = roundRend;
                changer.Start();
            }
            roundRend.enabled = false;
        }

        // from zyro at https://github.com/zyroyz/AetherPadTemp/blob/main/AetherTemp/Menu/Particles.cs in CreateFireAtPosition and modifyed by me
        public static void CreateParticles(Vector3 position, ParticleSystem.MinMaxGradient gradient)
        {
            var fire = new GameObject("FireParticle");
            fire.transform.position = position;
            var particles = fire.AddComponent<ParticleSystem>();
            var module = particles.main;

            module.startColor = gradient;
            module.startSize = .05f;
            module.startSpeed = 2f;
            module.startLifetime = 1.5f;
            module.loop = true;
            module.simulationSpace = ParticleSystemSimulationSpace.World;
            module.maxParticles = 30;

            var renderer = particles.GetComponent<ParticleSystemRenderer>();
            renderer.material = new Material(ParticleShader);
            renderer.material.SetColor("_Color", Color.black);

            var emission = particles.emission;
            emission.rateOverTime = 20f;

            var shape = particles.shape;
            shape.shapeType = ParticleSystemShapeType.Cone;
            shape.angle = 20f;
            shape.radius = .1f;

            fire.Destroy(.5f);
        }

        /*public static void AddTrailToObject(ref GameObject obj, float width, Material material)
        {
            var trail = obj.GetOrAddComponent<TrailRenderer>();
            trail.material = material;
            trail.time = 1f;
            trail.startWidth = width;
            trail.endWidth = 0;
            trail.minVertexDistance = .1f;
        }*/

        public static void LoadAudios(string btnname)
        {
            if (comfirmLoad)
            {
                for (int i = 0; i < Stringys[0].Length; i++)
                {
                    if (AudioClips[i] == null)
                        AudioClips[i] = Loaders.GetAudioFromURL(Stringys[0][i]);
                    else NotifiLib.SendNotification(NotifUtils.Warning() + "All Audios Have Already Been Loaded", 1f);
                }
                for (int i = 0; i < Stringys[1].Length; i++)
                {
                    if (LinkTextures[i] == null)
                        LinkTextures[i] = Loaders.LoadImageFromURL(Stringys[1][i]);
                }
                fastLoad = false;
                GetIndex(btnname).enabled = false;
            }
            else
            {
                NotifiLib.SendNotification(NotifUtils.Warning() + "This Will Make Your Game Have A HEAVY Lag Spike For Around A Minute If You Wish To Continue Press A And If You Wish To Cancel Press B", 5f);
                if (Controller.rightControllerPrimaryButton) comfirmLoad = true;
                if (Controller.rightControllerSecondaryButton) GetIndex(btnname).enabled = false;
            }
        }

        private static void ImageMenuButton(GameObject button, Texture2D imageTX)
        {
            var image = new GameObject { transform = { parent = canvasObject.transform } }.AddComponent<Image>();
            var materer = new Material(image.material);
            image.material = materer;
            image.material.SetTexture("_MainTex", imageTX);
            var recter = image.GetComponent<RectTransform>();
            recter.sizeDelta = new Vector2(.03f, .03f);
            recter.localPosition = button.transform.position + new Vector3(.01f, 0, 0);
            recter.rotation = Quaternion.Euler(new Vector3(180, 90, 90));
        }

        public static GameObject menu;
        public static GameObject menuBackground;
        public static GameObject reference;
        public static GameObject referenceOther;
        public static GameObject canvasObject;
        private static GameObject planet = null;
        private static GameObject planetRing = null;
        private static GameObject[] gameObjects = null;
        private static GameObject rightGhostHand = null;
        private static GameObject leftGhostHand = null;
        public static GameObject keyboard = null;

        public static Camera TPC;
        public static Camera mainCamera;

        public static bool inKeyboard = false;
        private static bool foundBoards;
        private static bool foundSmallBoards;
        private static bool pressedKeys;
        public static bool fastLoad;
        private static bool comfirmLoad = false;

        public static int pageNumber = 0;
        public static int buttonsType = 0;
        public static int pageSize = 8;
        public static int easyPage = 0;
        public static int destroyDelay = 0;

        private static float gridSize = 1f;

        private static Text tooltipText;
        private static Text inputTextObject;

        public static Color boardColor = Color.black;

        private static AudioSource menuHandAudio { get { return GetEnabled("Right Hand Menu") ? RigUtils.MyOfflineRig.leftHandPlayer : RigUtils.MyOfflineRig.rightHandPlayer; } }

        private static GliderHoldable[] gliders = null;

        private static GorillaRopeSwing[] ropes = null;

        private static BuilderPiece[] builderPieces = null;

        private static GorillaKeyboardButton[] keyboardButtons = null;
        public static GorillaKeyboardButton gorillaKeys;

        public static SnowballThrowable[] throwables = null;

        private static VRRig Ghost = null;

        //private static GradientColorKey[] MenuColorKeys = new GradientColorKey[4];
        public static Color buttonTextColor = Color.black;

        public static BindingFlags NonPublicInstance => BindingFlags.NonPublic | BindingFlags.Instance;

        public static RaiseEventOptions Others => new RaiseEventOptions { Receivers = ReceiverGroup.Others };

        public static Shader UberShader => Shader.Find("GorillaTag/UberShader");
        public static Shader UniversalShader => Shader.Find("Universal Render Pipeline/Lit");
        public static Shader DefaultShader => Shader.Find("Sprites/Default");
        public static Shader ParticleShader => Shader.Find("Universal Render Pipeline/Particles/Unlit");
        public static Shader TextShader => Shader.Find("GUI/Text Shader");

        public static string tooltipString;
        public static string DzongkhaMenuName = $"<color=magenta>མཚན་མོ།</color> <color=blue>ལས༌མགྲོན</color> <color=cyan>ཐོ{PluginInfo.Version}</color>";
        //private static string DateTimeTitle;
        private static string boardName;
        public static string KeyboardInput = "";

        private static string MOTDText =
        "Added No Button Colliders, Improved Spider Monke, And Fixed Some Stuff";


        private static string Credits =
            "ThatGuy [Main Developer / Owner]\n" +
            "Drew [Helper]\n" +
            "Mystic [The Real Owner]\n" +
            "Anthonyz [for being cool]" +
            "\n\n\n\n\n\n" +
            $"You Are Currently Using Mystic Client Version {PluginInfo.Version} With A Mod Count Of {Plugin.MenuButtonCount}, Check Discord Or Github For Any Future Updates";
    }



/*
leaf star -1291863839
dragon slingshot 526153556
fire ball proj 2111191893
cyber ninja star 1936767506
playing card proj -1299387895
*/
}
