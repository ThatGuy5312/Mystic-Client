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
using System.Reflection;
using SColor = System.Drawing.Color;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using System.Threading.Tasks;
using UnityEngine.Video;

namespace MysticClient.Menu
{
    [HarmonyPatch(typeof(GorillaLocomotion.Player), "LateUpdate")]
    public class Main : MonoBehaviour
    {
        public static GorillaPaintbrawlManager GorillaPaintbrawlManager { get { return GameObject.Find("Gorilla Battle Manager").GetComponent<GorillaPaintbrawlManager>(); } }
        public static GorillaTagManager GorillaTagManager { get { return GameObject.Find("Gorilla Tag Manager").GetComponent<GorillaTagManager>(); } }
        public static GorillaHuntManager GorillaHuntManager { get { return GorillaGameManager.gameObject.GetComponent<GorillaHuntManager>(); } }
        public static GorillaGameManager GorillaGameManager { get { return GorillaGameManager.instance; } }
        public static NetworkSystem PhotonSystem { get { return NetworkSystem.Instance; } }
        public static PhotonNetworkController NetworkController { get { return PhotonNetworkController.Instance; } }
        public static ControllerInputPoller Controller { get { return ControllerInputPoller.instance; } }
        public static IInputSystem UserInput { get { return UnityInput.Current; } }

        public static void Prefix()
        {
            try
            {
                if (!inKeyboard)
                {
                    var toOpen = (!GetEnabled("Right Hand Menu") && Controller.leftControllerSecondaryButton) || (GetEnabled("Right Hand Menu") && Controller.rightControllerSecondaryButton);
                    var keyboardOpen = UserInput.GetKey(KeyCode.Q);
                    if (menu == null)
                    {
                        if (toOpen || keyboardOpen)
                        {
                            CreateMenu();
                            RecenterMenu(GetEnabled("Right Hand Menu"), keyboardOpen);
                            if (reference == null) CreateReference(GetEnabled("Right Hand Menu"));
                            if (GetEnabled("Dynamic Sounds")) Loaders.PlayAudio(AudioClips[5]);
                        }
                    }
                    else
                    {
                        if (toOpen || keyboardOpen)
                        {
                            RecenterMenu(GetEnabled("Right Hand Menu"), keyboardOpen);
                            if (reference == null) CreateReference(GetEnabled("Right Hand Menu"));
                            if (menu.GetComponent<Rigidbody>()) { Destroy(menu.GetComponent<Rigidbody>()); if (GetEnabled("Dynamic Sounds")) Loaders.PlayAudio(AudioClips[5]); }
                        }
                        else if (!(toOpen || keyboardOpen) && !menu.GetComponent<Rigidbody>())
                        {
                            if (physicSetting == 1 || physicSetting == 2)
                            {
                                var menuRB = menu.AddComponent(typeof(Rigidbody)) as Rigidbody;
                                menuRB.detectCollisions = true; menuRB.isKinematic = false;
                                var spin = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized * 175f;
                                var tracker = GetEnabled("Right Hand Menu") ? RigUtils.MyPlayer.rightHandCenterVelocityTracker : RigUtils.MyPlayer.leftHandCenterVelocityTracker;
                                menuRB.useGravity = !GetEnabled("Zero Gravity Menu");
                                menu.GetComponent<Rigidbody>().AddTorque(Random.insideUnitSphere * 275f);
                                menu.GetComponent<Rigidbody>().AddTorque(spin);
                                menuRB.velocity = tracker.GetAverageVelocity(true, 0); Destroy(reference); reference = null;
                                if (GetEnabled("Dynamic Sounds")) Loaders.PlayAudio(AudioClips[6]);
                                if (GetEnabled("Multi Create")) { Destroy(menu, destroyDelay); menu = null; }
                            }
                            else if (physicSetting == 0)
                            {
                                if (GetEnabled("Dynamic Sounds"))
                                    Loaders.PlayAudio(AudioClips[6]);
                                if (GetEnabled("Multi Create"))
                                {
                                    Destroy(menu, destroyDelay);
                                    menu = null;
                                    Destroy(reference);
                                    reference = null;
                                }
                                else DestroyMenu();
                            }
                        }
                    }
                }             
                try
                {
                    if (!RigUtils.MyOfflineRig.enabled)
                    {
                        if (Rig.GhostType == 1)
                        {
                            if (rightGhostHand == null)
                                rightGhostHand = GameObject.CreatePrimitive(PrimitiveType.Cube);
                            rightGhostHand.transform.position = RigUtils.MyOnlineRig.rightHandTransform.position;
                            rightGhostHand.transform.Rotate(1f, 1f, 1f);
                            rightGhostHand.transform.localScale = new Vector3(0.15f, 0.15f, 0.15f) * RigUtils.MyPlayer.scale;
                            rightGhostHand.transform.GetComponent<Renderer>().material = TransparentMaterial(GetChangeColorA(NormalColor, .15f));
                            if (leftGhostHand == null)
                                leftGhostHand = GameObject.CreatePrimitive(PrimitiveType.Cube);
                            leftGhostHand.transform.position = RigUtils.MyOnlineRig.leftHandTransform.position;
                            leftGhostHand.transform.Rotate(1f, 1f, 1f);
                            leftGhostHand.transform.localScale = new Vector3(0.15f, 0.15f, 0.15f) * RigUtils.MyPlayer.scale;
                            leftGhostHand.transform.GetComponent<Renderer>().material = TransparentMaterial(GetChangeColorA(NormalColor, .15f));
                            Destroy(rightGhostHand.GetComponent<Collider>());
                            Destroy(leftGhostHand.GetComponent<Collider>());
                        }
                        else if (Rig.GhostType == 2)
                        {
                            if (Ghost == null)
                            {
                                Ghost = Instantiate(RigUtils.MyOfflineRig, RigUtils.MyPlayer.transform.position, RigUtils.MyPlayer.transform.rotation);
                                Ghost.headBodyOffset = Vector3.zero;
                                Ghost.enabled = true;
                            }
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
                        if (Ghost.gameObject != null) 
                        { 
                            Destroy(Ghost.gameObject); 
                            Ghost = null; 
                        }
                        if (rightGhostHand != null || leftGhostHand != null)
                        {
                            Destroy(rightGhostHand);
                            rightGhostHand = null;
                            Destroy(leftGhostHand);
                            leftGhostHand = null;
                        }
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
                    if (UserInput.GetMouseButton(0) && !Chainloader.PluginInfos.ContainsKey("org.thatguy.gorillatag.mysticguiex"))
                    {
                        Physics.Raycast(mainCamera.ScreenPointToRay(UserInput.mousePosition), out var hit);
                        GameObject.Find("Player Objects/Player VR Controller/GorillaPlayer/TurnParent/LeftHandTriggerCollider").transform.position = hit.point;
                        GameObject.Find("Player Objects/Player VR Controller/GorillaPlayer/TurnParent/LeftHandTriggerCollider").GetComponent<TransformFollow>().enabled = false;
                    } else GameObject.Find("Player Objects/Player VR Controller/GorillaPlayer/TurnParent/LeftHandTriggerCollider").GetComponent<TransformFollow>().enabled = true;
                }
                catch { }
                try
                {
                    GameObject.Find("CodeOfConduct").GetComponent<TMP_Text>().text = "<color=blue>Credits</color>";
                    GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/motd (1)").GetComponent<TMP_Text>().text = "<color=blue>Updates</color>";
                    GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/motdtext").GetComponent<TMP_Text>().text = MOTDText;
                    GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/COC Text").GetComponent<TMP_Text>().text = Credits;
                    GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomBoundaryStones/BoundaryStoneSet_Forest/wallmonitorforestbg").GetComponent<Renderer>().material.color = Color.black;
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
                                boards.GetComponent<Renderer>().material.color = Color.black;
                        }
                    }
                    find = 0;
                    for (int i = 0; i < GameObject.Find("Environment Objects/LocalObjects_Prefab/Forest").transform.childCount; i++)
                    {
                        var boards = GameObject.Find("Environment Objects/LocalObjects_Prefab/Forest").transform.GetChild(i).gameObject;
                        if (boards.name.Contains("forestatlas"))
                        {
                            find++;
                            if (find == 2)
                                boards.GetComponent<Renderer>().material.color = Color.black;
                        }
                    }
                    foreach (var objs in GetGameObjects())
                        if (objs.name == "Wallmonitor_Small_Prefab")
                            objs.GetNamedChild("wallmonitorscreen_small").GetComponent<Renderer>().material.color = Color.black;
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
                    BundleObjects[0].transform.Rotate(0, .2f, 0); // idk im bored
                    keyboard = BundleObjects[1];
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
                    if (GetEnabled("Disable Stump Planet"))
                    {
                        planet.SetActive(false);
                        planetRing.SetActive(false);
                    }
                    else
                    {
                        planet.SetActive(true);
                        planetRing.SetActive(true);
                    }
                    //DateTimeTitle = DateTime.Now.ToString(); menuTitleZ = menuSize.z / 2f;
                } catch { }
            } 
            catch { }
            try
            {
                if (inKeyboard)
                {
                    CreateKeyboardReference();
                    if (menu == null)
                    {
                        CreateMenu();
                        menu.transform.position = RigUtils.MyOfflineRig.transform.forward;
                        menu.transform.LookAt(RigUtils.MyOfflineRig.transform.position);
                    }
                    var distance = Vector3.Distance(menu.transform.position, RigUtils.MyOfflineRig.transform.position);
                    if (distance > 5f)
                    {
                        menu.transform.position = RigUtils.MyOfflineRig.transform.forward;
                        menu.transform.LookAt(RigUtils.MyOfflineRig.transform.position);
                    }
                    Settings.SetUpKeyboard();
                    var foundMods = new List<ButtonInfo>();
                    foreach (var btnss in buttons)
                        foreach (var btns in btnss)
                            foreach (var btn in btns)
                                if (btn.buttonText.Contains(Settings.inputtext))
                                    foundMods.Add(btn);
                    var searchText = Settings.inputtext + ((Time.frameCount / 45 % 2) == 0 ? "|" : " ");
                    var top = new ButtonInfo { buttonText = searchText, isTogglable = false };
                    var btnArray = Settings.inputtext == "" ? foundMods.Skip(pageNumber * pageSize).Take(pageSize).ToArray() : buttons[easyPage][buttonsType].Skip(pageNumber * pageSize).Take(pageSize).ToArray();
                    CreateButton(0, top);
                    for (int i = 0; i < btnArray.Length; i++)
                        CreateButton((i + 1) * internalButtonOffset, btnArray[i]);
                }
                else
                {
                    GameObject.Find("buttonPresser_Right").Destroy();
                    GameObject.Find("buttonPresser_Left").Destroy();
                }
            } catch { }
            try
            {
                foreach (var btnss in buttons)
                    foreach (var btns in btnss)
                        foreach (var btn in btns)
                            if (btn.enabled)
                                if (btn.method != null)
                                {
                                    try { btn.method.Invoke(); }
                                    catch (Exception exc) { Debug.LogError(string.Format("{0} // Error with mod {1} at {2}: {3}", PluginInfo.Name, btn.buttonText, exc.StackTrace, exc.Message)); }
                                }
            } catch (Exception exc) { Debug.LogError(string.Format("{0} // Fatal Error {1} - {2}", PluginInfo.Name, exc.StackTrace, exc.Message)); }
        }

        public static AudioClip[] AudioClips = new AudioClip[999];
        public static Texture2D[] Textures = new Texture2D[999];
        public static GameObject[] BundleObjects = new GameObject[999];
        public static Texture2D[] MCTextures = new Texture2D[999];
        private static void Load()
        {
            for (int i = 0; i < Stringys[0].Length; i++)
            {
                if (AudioClips[i] == null)
                    AudioClips[i] = Loaders.GetAudioFromURL(Stringys[0][i]);
            }
            for (int i = 0; i < Stringys[1].Length; i++)
            {
                if (Textures[i] == null)
                    Textures[i] = Loaders.LoadImageFromURL(Stringys[1][i]);
            }
            for (int i = 0; i < Stringys[2].Length; i++)
            {
                if (BundleObjects[i] == null)
                    BundleObjects[i] = Loaders.LoadGameObject(Stringys[2][i].Split(':')[0], Stringys[2][i].Split(':')[1]);
            }
        }

        private static string[][] Stringys =
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
            },
            // fix paypal temp stuff (if i dont fix this and you see this on my github this is for a template that my friend made)
            new string[] // texture links 1
            {
                //"https://cdn.discordapp.com/attachments/1322758029325897738/1322866816745345044/Background-Gradient.png?ex=67726f3a&is=67711dba&hm=e6ade193fc44ee2707b5ee2625085253aa31959ba5368f3dd3e0746c7d6f6801&", // paypal grad
                //"https://cdn.discordapp.com/attachments/1322758029325897738/1322866956998672504/Paypal.png?ex=67726f5c&is=67711ddc&hm=5feccfe481cb069249d3b71e0c88a76e7840d92bcfb67d4cd21104528e2f530d&", // paypal
            },
            new string[] // asset bundle paths 2
            {
                //"MysticClient.Resources.adminplumbob:Admin Plumbob", //plumbob 0
                //"MysticClient.Resources.adminplanet:Planet", // planet 1
                "MysticClient.Resources.mcbundle:TextureBundle", // minecraft texture bundle 0
                "MysticClient.Resources.keyboard:Keyboard", // vr keyboard 1
            }
        };
        public static void CreateMenu()
        {
            if (physicSetting == 0 || physicSetting == 1) // i know i can simplify this but im too lazy
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
            menu.transform.localScale = GetEnabled("Annoying Menu") ? new Vector3(.1f, Random.Range(.3f, .5f), parentSize.z) : parentSize;
            menuBackground = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Destroy(menuBackground.GetComponent<Rigidbody>());
            Destroy(menuBackground.GetComponent<BoxCollider>());
            menuBackground.transform.parent = menu.transform;
            menuBackground.transform.rotation = Quaternion.identity;
            menuBackground.transform.localScale = GetEnabled("Annoying Menu") ? new Vector3(Random.Range(.1f, .3f), Random.Range(1f, 5f), Random.Range(1f, 5f)) : menuSize;
            /*if (GetEnabled("Make Menu Flash"))
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
            }*/
            menuBackground.GetComponent<Renderer>().material.color = NormalColor;
            menuBackground.transform.position = menuPos;
            if (GetEnabled("Outline Menu"))
                OutlineMenuObject(menuBackground);
            if (GetEnabled("Menu Trail"))
            {
                var trail = menu.AddComponent<TrailRenderer>();
                trail.material = TransparentMaterial(GetChangeColorA(GetEnabled("Make Menu Trail Color Follow Menu Color") ? NormalColor : menuTrailColor, .8f));
                trail.time = .3f;
                trail.startWidth = .03f;
                trail.endWidth = 0;
                trail.minVertexDistance = .1f;
            }
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
            text.text = menuTitle;
            text.fontSize = 1;
            text.supportRichText = true;
            text.fontStyle = titleFontStyle;
            text.alignment = TextAnchor.MiddleCenter;
            text.resizeTextForBestFit = true;
            text.resizeTextMinSize = 0;
            var component = text.GetComponent<RectTransform>();
            component.localPosition = Vector3.zero;
            component.sizeDelta = new Vector2(0.28f, 0.05f);
            component.position = new Vector3(0.06f, 0f, menuTitleZ);
            component.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));
            /*if (GetEnabled("Date Time"))
            {
                Text datetimetext = new GameObject
                {
                    transform =
                    {
                        parent = canvasObject.transform
                    }
                }.AddComponent<Text>();
                datetimetext.text = DateTimeTitle;
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
            }*/
            if (GetEnabled("Side Disconnect"))
            {
                var disconnectbutton = GameObject.CreatePrimitive(PrimitiveType.Cube);
                Destroy(disconnectbutton.GetComponent<Rigidbody>());
                disconnectbutton.GetComponent<BoxCollider>().isTrigger = true;
                disconnectbutton.transform.parent = menu.transform;
                disconnectbutton.transform.rotation = Quaternion.identity;
                disconnectbutton.transform.localScale = disconnectSize;
                disconnectbutton.transform.localPosition = disconnectPos;
                if (GetEnabled("Outline Menu"))
                    OutlineMenuObject(disconnectbutton);
                disconnectbutton.AddComponent<BtnCollider>().relatedText = "DisconnectingButton";
                /*if (GetEnabled("Make Menu Flash"))
                {
                    ColorChanger colorChanger = disconnectbutton.AddComponent<ColorChanger>();
                    colorChanger.colors = new Gradient
                    {
                        colorKeys = MenuColorKeys
                    };
                    colorChanger.loop = true;
                }
                else*/
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
                rectt.localPosition = disconnectbutton.transform.position + new Vector3(.01f, 0, 0);
                rectt.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));
            }

            if (GetEnabled("Destroy Networked Objects Side Button"))
            {
                var destoryButton = GameObject.CreatePrimitive(PrimitiveType.Cube);
                Destroy(destoryButton.GetComponent<Rigidbody>());
                destoryButton.GetComponent<BoxCollider>().isTrigger = true;
                destoryButton.transform.parent = menu.transform;
                destoryButton.transform.rotation = Quaternion.identity;
                destoryButton.transform.localScale = disconnectSize;
                destoryButton.transform.localPosition = disconnectPos + new Vector3(0, 0, .15f);
                if (GetEnabled("Outline Menu"))
                    OutlineMenuObject(destoryButton);
                destoryButton.AddComponent<BtnCollider>().relatedText = "DestroyButton";
                /*if (GetEnabled("Make Menu Flash"))
                {
                    ColorChanger colorChanger = destoryButton.AddComponent<ColorChanger>();
                    colorChanger.colors = new Gradient
                    {
                        colorKeys = MenuColorKeys
                    };
                    colorChanger.loop = true;
                }
                else*/
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
                recttt.localPosition = destoryButton.transform.position + new Vector3(.01f, 0, 0); // new Vector3(0.071f, -.331f, 0.12f)
                recttt.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));
            }
            if (GetToolTip("Changed Menu Theme").buttonText.Contains("AZ"))
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
                    if (GetEnabled("Outline Menu"))
                        OutlineMenuObject(disconnectbutton);
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

                    var rectt = discontext.GetComponent<RectTransform>();
                    rectt.localPosition = Vector3.zero;
                    rectt.sizeDelta = new Vector2(0.2f, 0.03f);
                    rectt.localPosition = new Vector3(0.064f, 0f, 0.21f);
                    rectt.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));
                }
            }

            if (GetToolTip("Changed Menu Theme").buttonText.Contains("PayPal"))
            {
                /*var rend = menuBackground.GetComponent<Renderer>();
                rend.material.shader = UniversalShader;
                rend.material.color = new Color32(10, 164, 192, byte.MaxValue);
                rend.material.mainTexture = Textures[0];
                var obj = new GameObject { transform = { parent = canvasObject.transform } };
                var image = obj.AddComponent<RawImage>();
                image.texture = Textures[1];
                var rect = obj.GetComponent<RectTransform>();
                rect.sizeDelta = new Vector2(.3f, .2f);
                rect.localPosition = new Vector3(.06f, 0, .29f);
                rect.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));*/
            }

            int lastPage = ((buttons[easyPage][buttonsType].Length + pageSize - 1) / pageSize) - 1;
            int next = pageNumber + 1; int last = pageNumber - 1;
            if (next > lastPage) next = 0; if (last < 0) last = lastPage;
            GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Destroy(gameObject.GetComponent<Rigidbody>());
            gameObject.GetComponent<BoxCollider>().isTrigger = true;
            gameObject.transform.parent = menu.transform;
            gameObject.transform.rotation = Quaternion.identity;
            gameObject.transform.localScale = pageButtonSize;
            gameObject.transform.localPosition = pagePoss[0];
            if (GetEnabled("Outline Menu"))
                OutlineMenuObject(gameObject);
            /*if (GetEnabled("Make Menu Flash") && !GetToolTip("Changed Menu Type").buttonText.Contains("AZ"))
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
            }*/
            gameObject.GetComponent<Renderer>().material.color = GetToolTip("Changed Menu Theme").buttonText.Contains("AZ") ? Color.black : NormalColor;
            gameObject.AddComponent<BtnCollider>().relatedText = "PreviousPage";
            text = new GameObject
            {
                transform =
                {
                    parent = canvasObject.transform
                }
            }.AddComponent<Text>();
            var usingTheme = GetToolTip("Changed Menu Theme").buttonText.Contains("Mango");
            text.text = usingTheme ? $"[{last}] << Prev" : lastPageText;
            text.font = currentFont;
            text.fontSize = 1;
            text.alignment = TextAnchor.MiddleCenter;
            text.resizeTextForBestFit = true;
            text.resizeTextMinSize = 0;
            component = text.GetComponent<RectTransform>();
            component.localPosition = Vector3.zero;
            component.sizeDelta = new Vector2(0.2f, 0.03f);
            component.localPosition = gameObject.transform.position + new Vector3(.01f, 0, 0); ;
            component.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));
            gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Destroy(gameObject.GetComponent<Rigidbody>());
            gameObject.GetComponent<BoxCollider>().isTrigger = true;
            gameObject.transform.parent = menu.transform;
            gameObject.transform.rotation = Quaternion.identity;
            gameObject.transform.localScale = pageButtonSize;
            gameObject.transform.localPosition = pagePoss[1];
            if (GetEnabled("Outline Menu"))
                OutlineMenuObject(gameObject);
            /*if (GetEnabled("Make Menu Flash") && !GetToolTip("Changed Menu Type").buttonText.Contains("AZ"))
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
            }*/
            gameObject.GetComponent<Renderer>().material.color = GetToolTip("Changed Menu Theme").buttonText.Contains("AZ") ? Color.black : NormalColor;
            gameObject.AddComponent<BtnCollider>().relatedText = "NextPage";
            text = new GameObject
            {
                transform =
                {
                    parent = canvasObject.transform
                }
            }.AddComponent<Text>();
            text.text = usingTheme ? $"Next >> [{next}]" : nextPageText;
            text.font = currentFont;
            text.fontSize = 1;
            text.alignment = TextAnchor.MiddleCenter;
            text.resizeTextForBestFit = true;
            text.resizeTextMinSize = 0;
            component = text.GetComponent<RectTransform>();
            component.localPosition = Vector3.zero;
            component.sizeDelta = new Vector2(0.2f, 0.03f);
            component.localPosition = gameObject.transform.position + new Vector3(.01f, 0, 0);
            component.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));
            var tooltipObj = new GameObject();
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
            var componenttooltip = tooltipObj.GetComponent<RectTransform>();
            componenttooltip.localPosition = Vector3.zero;
            componenttooltip.sizeDelta = new Vector2(0.2f, 0.03f);
            componenttooltip.position = new Vector3(0.06f, 0f, toolTipZ);
            componenttooltip.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));
            var activeButtons = buttons[easyPage][buttonsType].Skip(pageNumber * pageSize).Take(pageSize).ToArray();
            if (!inKeyboard)
                for (int i = 0; i < activeButtons.Length; i++)
                    CreateButton(i * internalButtonOffset, activeButtons[i]);
        }

        public static void CreateButton(float offset, ButtonInfo method)
        {
            var gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Destroy(gameObject.GetComponent<Rigidbody>());
            gameObject.GetComponent<BoxCollider>().isTrigger = true;
            gameObject.transform.parent = menu.transform;
            gameObject.transform.rotation = Quaternion.identity;
            gameObject.transform.localScale = GetEnabled("Annoying Menu") ? new Vector3(.09f, Random.Range(1f, 5f), Random.Range(.08f, .1f)) : buttonSize;
            gameObject.transform.localPosition = new Vector3(buttonPos.x, buttonPos.y, buttonPos.z - offset / buttonOffset);
            gameObject.AddComponent<BtnCollider>().relatedText = method.buttonText;
            if (GetEnabled("Outline Menu"))
                OutlineMenuObject(gameObject);
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
                gameObject.GetComponent<Renderer>().material.color = ButtonColorEnabled;
            else
                gameObject.GetComponent<Renderer>().material.color = ButtonColorDisable;
            text.alignment = TextAnchor.MiddleCenter;
            text.fontStyle = buttonFontStyle;
            text.resizeTextForBestFit = true;
            text.resizeTextMinSize = 0;
            RectTransform component = text.GetComponent<RectTransform>();
            component.localPosition = Vector3.zero;
            component.sizeDelta = new Vector2(.2f, .03f);
            //component.localPosition = new Vector3(buttonTextPos.x, buttonTextPos.y, buttonTextPos.z - offset / buttonTextOffset); // 2.6
            component.localPosition = gameObject.transform.position + new Vector3(.01f, 0, 0);
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

        private static void CreateKeyboardReference()
        {
            var click1 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            click1.transform.parent = RigUtils.MyOnlineRig.rightHandTransform;
            click1.GetComponent<Renderer>().material.color = new Color32(167, 17, 237, 28);
            click1.transform.localPosition = pointerPosition;
            click1.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
            click1.name = "buttonPresser_Right";
            var click2 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            click2.transform.parent = RigUtils.MyOnlineRig.leftHandTransform;
            click2.GetComponent<Renderer>().material.color = new Color32(167, 17, 237, 28);
            click2.transform.localPosition = pointerPosition;
            click2.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
            click2.name = "buttonPresser_Left";
            click1.Destroy(Time.deltaTime);
            click2.Destroy(Time.deltaTime);
        }

        private static void OutlineMenuObject(GameObject obj)
        {
            var outline = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Destroy(outline.GetComponent<Rigidbody>());
            Destroy(outline.GetComponent<BoxCollider>());
            outline.transform.parent = menu.transform;
            outline.transform.rotation = Quaternion.identity;
            outline.transform.localScale = new Vector3(obj.transform.localScale.x - .01f, obj.transform.localScale.y + .01f, obj.transform.localScale.z + .01f);
            outline.transform.position = obj.transform.position;
            outline.GetComponent<Renderer>().material.color = outlineColor;
        }

        public static void Toggle(string buttonText)
        {
            int lastPage = ((buttons[easyPage][buttonsType].Length + pageSize - 1) / pageSize) - 1;
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
            if (GetEnabled("Dynamic Sounds"))
            {
                var target = GetIndex(index);
                if (target != null)
                {
                    if (target.isTogglable)
                    {
                        if (!target.enabled)
                            menuHandAudio.PlayOneShot(AudioClips[0]); // enable 0
                        else
                            menuHandAudio.PlayOneShot(AudioClips[1]); // disable 1
                    }
                    else
                        if (target.method != null)
                        menuHandAudio.PlayOneShot(AudioClips[0]);
                }
                else
                {
                    if (index == "NextPage")
                        menuHandAudio.PlayOneShot(AudioClips[8]);
                    else if (index == "PreviousPage")
                        menuHandAudio.PlayOneShot(AudioClips[8]);
                    else if (index == "DisconnectingButton")
                        menuHandAudio.PlayOneShot(AudioClips[8]);
                    else if (index == "DestroyButton")
                        menuHandAudio.PlayOneShot(AudioClips[8]);
                    if (target == null && index != "NextPage" && index != "PreviousPage" && index != "DisconnectingButton" && index != "DestroyButton")
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
                        new PhotonView().RPC("RPC_PlayHandTap", RpcTarget.Others, new object[]
                        {
                         buttonSound,
                         GetEnabled("Right Hand Menu"),
                         99999f
                        });
                        RPCProtection(RigUtils.MyNetPlayer);
                    }
                    else RigUtils.MyOfflineRig.PlayHandTapLocal(buttonSound, GetEnabled("Right Hand Menu"), 0.4f);
                } else */
                RigUtils.MyOfflineRig.PlayHandTapLocal(buttonSound, GetEnabled("Right Hand Menu"), 0.4f);
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
                PhotonNetwork.OpRemoveCompleteCacheOfPlayer(RigUtils.MyNetPlayer.ActorNumber);
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
            foreach (var obj in GetObjectsByName(objectName))
                return obj;
            return null;
        }
        public static GameObject[] GetObjectsByName(string objectName)
        {
            var objects = new List<GameObject>();
            foreach (var obj in GetGameObjects())
                if (obj.name.Contains(objectName))
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
        public static GorillaKeyboardButton[] GetKeyboardButtons()
        {
            if (keyboardButtons == null)
                keyboardButtons = FindObjectsOfType<GorillaKeyboardButton>();
            return keyboardButtons;
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
            var time = Time.time;
            var r = Mathf.Sin(time * 2f) * 0.5f + 0.5f;
            var g = Mathf.Sin(time * 1.5f) * 0.5f + 0.5f;
            var b = Mathf.Sin(time * 2.5f) * 0.5f + 0.5f;
            return new Color(r, g, b, 1);
        }

        public static Color HardColor(int index) => Settings.colors[index % Settings.colors.Length];

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

        public static GameObject menu;
        public static GameObject menuBackground;
        public static GameObject reference;
        public static GameObject canvasObject;
        private static GameObject planet = null;
        private static GameObject planetRing = null;
        private static GameObject[] gameObjects = null;
        private static GameObject rightGhostHand = null;
        private static GameObject leftGhostHand = null;
        public static GameObject keyboard = null;

        public static Camera TPC;
        public static Camera mainCamera;

        public static bool inKeyboard;

        public static int pageNumber = 0;
        public static int buttonsType = 0;
        public static int pageSize = 8;
        public static int easyPage = 0;
        public static int destroyDelay = 0;

        private static float gridSize = 1f;

        private static Text tooltipText;

        public static Color boardColor = Color.black;

        private static AudioSource menuHandAudio { get { return GetEnabled("Right Hand Menu") ? RigUtils.MyOfflineRig.leftHandPlayer : RigUtils.MyOfflineRig.rightHandPlayer; } }

        private static GliderHoldable[] gliders = null;

        private static GorillaRopeSwing[] ropes = null;

        private static BuilderPiece[] builderPieces = null;

        private static GorillaKeyboardButton[] keyboardButtons = null;

        private static VRRig Ghost = null;

        //private static GradientColorKey[] MenuColorKeys = new GradientColorKey[4];
        public static Color buttonTextColor = Color.black;

        public static BindingFlags NonPublicInstance { get { return BindingFlags.NonPublic | BindingFlags.Instance; } }

        public static Shader UberShader { get { return Shader.Find("GorillaTag/UberShader"); } }

        public static Shader UniversalShader { get { return Shader.Find("Universal Render Pipeline/Lit"); } }

        public static Shader DefaultShader { get { return Shader.Find("Sprites/Default"); } }

        public static Shader TextShader { get { return Shader.Find("GUI/Text Shader"); } }

        public static string tooltipString;
        //private static string DateTimeTitle;

        private static string MOTDText =
        "Added Menu Saving, " +
        "Use System Colors, " +
        "141 New Setting Colors [System], " +
        "Dynamic Sounds, " +
        "Block Mods, " +
        "Minecraft, " +
        "Rope Mods, " +
        "Super Speed Boost, " +
        "Improved GunLib, " +
        "Get ID Gun, " +
        "Get Creation Date Gun, " +
        "Planet, " +
        "Voice Commands, " +
        "Menu Trail, " +
        "Annoying Menu, " +
        "Change Menu Font, " +
        "Array List, " +
        "Shiny Menu, " +
        "Disable Stump Planet, " +
        "Make Ghost/Invis Toggled, " +
        "Fixed Sound Spammers, " +
        "Spider Monke, " +
        "Pull Speed, " +
        "No Tag On Join, " +
        "More Visual Mods";


        private static string Credits =
            "ThatGuy [Main Developer / Owner]\n" +
            "Drew [Helper]\n" +
            "Mystic [The Real Owner]\n" +
            "Anthonyz [for being cool]" +
            "\n\n\n\n\n\n" +
            "You Are Currently Using Mystic Client Version [" + PluginInfo.Version + "] Check Discord Or Github For Any Future Updates";
    }



/*
leaf star -1291863839
dragon slingshot 526153556
fire ball proj 2111191893
cyber ninja star 1936767506
playing card proj -1299387895
*/
}
