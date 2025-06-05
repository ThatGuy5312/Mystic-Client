using MysticClient.Utils;
using static MysticClient.Menu.Main;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Collections;
using UnityEngine.Networking;
using Unity.XR.CoreUtils;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using GorillaExtensions;
using System;
using OVR.OpenVR;

namespace MysticClient.Menu
{
    public class MysticModManager : MonoBehaviour
    {
        private GameObject mm_Prefab;
        private GameObject collider;
        private GameObject rightHandCollider;
        private GameObject leftHandCollider;

        static Texture2D currentImage;

        IEnumerator afterLoad;

        static int currentModIndex = 0;

        static List<Extention> MenuExtentions = new List<Extention>
        {
            MysticGUI,
            MysticGUI2
        };

        void Start()
        {
            Directory.CreateDirectory("BepInEx/Plugins/MysticMenuPlugins");
            afterLoad = AfterLoad();
            StartCoroutine(afterLoad);
        }

        void Update()
        {
            mm_Prefab.GetNamedChild("Prev").GetOrAddComponent<BtnCollider>();
            mm_Prefab.GetNamedChild("Next").GetOrAddComponent<BtnCollider>();
            mm_Prefab.GetNamedChild("Download").GetOrAddComponent<BtnCollider>();

            var Title = 
$"Mystic Plugin Installer\n{MenuExtentions[currentModIndex].name}\nBy {MenuExtentions[currentModIndex].creator}\n_______________________________________________________";

            var Description = MenuExtentions[currentModIndex].description;

            mm_Prefab.GetNamedChild("Monitor").GetNamedChild("screen").GetNamedChild("Title").GetComponent<Text>().text = Title;
            mm_Prefab.GetNamedChild("Monitor").GetNamedChild("screen").GetNamedChild("Description").GetComponent<Text>().text = Description;
            mm_Prefab.GetNamedChild("Monitor").GetNamedChild("screen").GetNamedChild("Image").GetComponent<RawImage>().texture = currentImage;



            if (UserInput.GetMouseButton(0) || UserInput.GetMouseButton(2))
                if (Physics.Raycast(mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue()), out var hit, 100))
                    hit.transform.gameObject.GetComponent<BtnCollider>()?.OnTriggerEnter(collider.GetComponent<Collider>());
        }

        static void SetTexture(Extention extention) => MUtils.RunCoroutine(LoadTexture(extention, texture => { currentImage = texture; }));

        static IEnumerator DownloadExtention(Extention extention)
        {
            using var webRequest = UnityWebRequest.Get(extention.downloadLink);
            yield return webRequest.SendWebRequest();
            if (webRequest.result != UnityWebRequest.Result.Success) Debug.Log($"Failed to download: {webRequest.error}"); else
            {
                var filePath = Path.Combine("BepInEx/Plugins/MysticMenuPlugins", $"{extention.fileName}.dll");
                File.WriteAllBytes(filePath, webRequest.downloadHandler.data);
                Debug.Log($"Download to: {filePath}");
            }
        }

        static IEnumerator LoadTexture(Extention extention, Action<Texture2D> callback)
        {
            using var webRequest = UnityWebRequestTexture.GetTexture(extention.imageLink);
            yield return webRequest.SendWebRequest();
            if (webRequest.result != UnityWebRequest.Result.Success) { Debug.Log($"Failed to load image for {extention.name}: {webRequest.error}"); callback?.Invoke(null); } else
            {
                var image = DownloadHandlerTexture.GetContent(webRequest);
                callback?.Invoke(image);
                Debug.Log($"Loaded image for {extention.name}");
            }
        }

        static Extention MysticGUI => new Extention
        {
            name = "Mystic GUI Extention",
            creator = "ThatGuy (Menu Owner)",
            description = "A mod that add a GUI to the Mystic Client menu",
            fileName = "MysticGUIEX",
            downloadLink = "https://drive.google.com/uc?export=download&id=1H51Imqwz0hwwwA_5NS9o6domE8h6LMZ5",
            imageLink = "https://drive.google.com/uc?export=download&id=1GxIcW2Ap6PC9Mh3iA6KO4ZurR5zAXmeb"
        };

        static Extention MysticGUI2 => new Extention
        {
            name = "Funny Story",
            creator = "ThatGuy",
            description = "so like yesterday this kid got suspended for yelling in class after someone made a noice saying 'that sounded like someone pulling out a butt plug at max velocity' and when the teacher told him to stop talking he kept on saying i have rizz and then the teacher made him stay after class",
            fileName = "",
            downloadLink = "https://cdn.discordapp.com/attachments/1323017142911766558/1350381591918153738/menugui.png?ex=67d6885c&is=67d536dc&hm=cb991f0e1214d51b7f482aa0b8ccba56a7f799117b4040ffa19d5aaaff87bdd8&",
        };

        IEnumerator AfterLoad()
        {
            yield return new WaitForSeconds(3);
            if (mm_Prefab == null)
                mm_Prefab = BundleObjects[2];
            mm_Prefab.transform.position = new Vector3(-69.3f, 12, -83.5f);
            mm_Prefab.transform.rotation = Quaternion.Euler(new Vector3(9.77f, 73.884f, 0));
            collider = GameObject.CreatePrimitive(PrimitiveType.Cube);
            collider.name = "buttonPresser";
            collider.transform.position = new Vector3(999, 999, -999);

            rightHandCollider = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            rightHandCollider.name = "buttonPresser_Right";
            rightHandCollider.transform.localScale = new Vector3(.01f, .01f, .01f);
            rightHandCollider.transform.parent = RigUtils.MyOnlineRig.rightHandTransform;
            rightHandCollider.transform.localPosition = new Vector3(0, -.1f, 0);
            rightHandCollider.Destroy<Renderer>();
            rightHandCollider.layer = 2;

            leftHandCollider = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            leftHandCollider.name = "buttonPresser_Left";
            leftHandCollider.transform.localScale = new Vector3(.01f, .01f, .01f);
            leftHandCollider.transform.parent = RigUtils.MyOnlineRig.leftHandTransform;
            leftHandCollider.transform.localPosition = new Vector3(0, -.1f, 0);
            leftHandCollider.Destroy<Renderer>();
            leftHandCollider.layer = 2;

            SetTexture(MenuExtentions[currentModIndex]);

            StopCoroutine(afterLoad);
        }

        public class Extention
        {
            public string name;
            public string creator;
            public string description;
            public string downloadLink;
            public string fileName;
            public string imageLink;
        }

        public class BtnCollider : MonoBehaviour
        {
            private static float pressDelay = 0f;

            public void OnTriggerEnter(Collider other)
            {
                if (other.name.Contains("buttonPresser"))
                if (Time.time >= pressDelay)
                {
                    pressDelay = Time.time + .2f;
                    var isLeft = other.name.Contains("Left");
                    RigUtils.MyOnlineRig.StartVibration(isLeft, RigUtils.MyOnlineRig.tagHapticStrength / 2f, RigUtils.MyOnlineRig.tagHapticDuration / 2f);
                    RigUtils.MyOfflineRig.PlayHandTapLocal(67, GetEnabled("Right Hand Menu"), .4f);

                    if (gameObject.name == "Prev")
                    {
                        currentModIndex--;
                        if (currentModIndex < 0)
                            currentModIndex = MenuExtentions.Count - 1;

                        SetTexture(MenuExtentions[currentModIndex]);
                    }

                    if (gameObject.name == "Next")
                    {
                        currentModIndex++;
                        if (currentModIndex >= MenuExtentions.Count)
                            currentModIndex = 0;

                        SetTexture(MenuExtentions[currentModIndex]);
                    }


                    if (gameObject.name == "Download")
                            StartCoroutine(DownloadExtention(MenuExtentions[currentModIndex]));
                }
            }
        }
    }
}
