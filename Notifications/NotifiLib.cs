using System;
using System.Globalization;
using System.Linq;
using BepInEx;
using MysticClient.Menu;
using MysticClient.Utils;
using UnityEngine;
using UnityEngine.UI;
using static MysticClient.Menu.MenuSettings;

namespace MysticClient.Notifications
{
    [BepInPlugin("org.thatguy.notifications.com", "NotifiLib", "1.0.0")]
    public class NotifiLib : BaseUnityPlugin
    {
        private void Init()
        {
            MainCamera = GameObject.Find("Main Camera");
            HUDObj = new GameObject();
            HUDObj2 = new GameObject();
            HUDObj2.name = "NOTIFICATIONLIB_HUD_OBJ";
            HUDObj.name = "NOTIFICATIONLIB_HUD_OBJ";
            HUDObj.AddComponent<Canvas>();
            HUDObj.AddComponent<CanvasScaler>();
            HUDObj.AddComponent<GraphicRaycaster>();
            HUDObj.GetComponent<Canvas>().enabled = true;
            HUDObj.GetComponent<Canvas>().renderMode = RenderMode.WorldSpace;
            HUDObj.GetComponent<Canvas>().worldCamera = MainCamera.GetComponent<Camera>();
            HUDObj.GetComponent<RectTransform>().sizeDelta = new Vector2(5f, 5f);
            HUDObj.GetComponent<RectTransform>().position = new Vector3(MainCamera.transform.position.x, MainCamera.transform.position.y, MainCamera.transform.position.z);
            HUDObj2.transform.position = new Vector3(MainCamera.transform.position.x, MainCamera.transform.position.y, MainCamera.transform.position.z - 4.6f);
            HUDObj.transform.parent = HUDObj2.transform;
            HUDObj.GetComponent<RectTransform>().localPosition = new Vector3(0f, 0f, 1.6f);
            Vector3 eulerAngles = HUDObj.GetComponent<RectTransform>().rotation.eulerAngles;
            eulerAngles.y = -270f;
            HUDObj.transform.localScale = new Vector3(1f, 1f, 1f);
            HUDObj.GetComponent<RectTransform>().rotation = Quaternion.Euler(eulerAngles);
            Testtext = new GameObject
            {
                transform =
                {
                    parent = HUDObj.transform
                }
            }.AddComponent<Text>();
            Testtext.text = "";
            Testtext.fontSize = 30;
            Testtext.font = currentFont;
            Testtext.rectTransform.sizeDelta = new Vector2(450f, 210f);
            Testtext.alignment = TextAnchor.LowerLeft;
            Testtext.rectTransform.localScale = new Vector3(0.00333333333f, 0.00333333333f, 0.33333333f);
            Testtext.rectTransform.localPosition = new Vector3(-1f, -1f, -0.5f);
            Testtext.material = AlertText;
            NotifiText = Testtext;
        }

        private void FixedUpdate()
        {
            if (!HasInit && GameObject.Find("Main Camera") != null)
            {
                Init();
                HasInit = true;
            }
            HUDObj2.transform.position = new Vector3(MainCamera.transform.position.x, MainCamera.transform.position.y, MainCamera.transform.position.z);
            HUDObj2.transform.rotation = MainCamera.transform.rotation;
            if (Testtext.text != "")
            {
                NotificationDecayTimeCounter++;
                if (NotificationDecayTimeCounter > NotificationDecayTime)
                {
                    Notifilines = null;
                    newtext = "";
                    NotificationDecayTimeCounter = 0;
                    Notifilines = Testtext.text.Split(Environment.NewLine.ToCharArray()).Skip(1).ToArray();
                    foreach (string text in Notifilines)
                    {
                        if (text != "")
                        {
                            newtext = newtext + text + "\n";
                        }
                    }
                    Testtext.text = newtext;
                }
            }
            else
            {
                NotificationDecayTimeCounter = 0;
            }
        }

        public static void SendNotification(string NotificationText, bool playSound = true)
        {
            if (!disableNotifications)
            {
                try
                {
                    if (IsEnabled && PreviousNotifi != NotificationText)
                    {
                        if (!NotificationText.Contains(Environment.NewLine))
                        {
                            NotificationText += Environment.NewLine;
                        }
                        NotifiText.text = NotifiText.text + NotificationText;
                        NotifiText.supportRichText = true;
                        PreviousNotifi = NotificationText;
                        if (Main.GetEnabled("Dynamic Sounds") && playSound)
                        {
                            if (NotificationText.Contains("ERROR"))
                                Loaders.PlayAudio(Main.AudioClips[7]);
                            else
                                Loaders.PlayAudio(Main.AudioClips[2]);
                        }
                    }
                }
                catch
                {
                    Debug.LogError("Notification failed, object probably null due to third person | " + NotificationText);
                }
            }
        }
        private static float notifDelay = 0f;
        public static void SendNotification(string NotificationText, float delay, bool playSound = true)
        {
            if (!disableNotifications)
            {
                try
                {
                    if (IsEnabled && PreviousNotifi != NotificationText && Time.time > notifDelay)
                    {
                        if (!NotificationText.Contains(Environment.NewLine))
                        {
                            NotificationText += Environment.NewLine;
                        }
                        NotifiText.text = NotifiText.text + NotificationText;
                        NotifiText.supportRichText = true;
                        PreviousNotifi = NotificationText;
                        notifDelay = Time.time + delay;
                        if (Main.GetEnabled("Dynamic Sounds") && playSound)
                        {
                            if (NotificationText.Contains("ERROR"))
                                Loaders.PlayAudio(Main.AudioClips[7]);
                            else
                                Loaders.PlayAudio(Main.AudioClips[2]);
                        }
                    }
                }
                catch
                {
                    Debug.LogError("Notification failed, object probably null due to third person | " + NotificationText);
                }
            }
        }

        public static void ClearAllNotifications() => NotifiText.text = "";

        public static void ClearPastNotifications(int amount)
        {
            string text = "";
            foreach (string text2 in NotifiText.text.Split(Environment.NewLine.ToCharArray()).Skip(amount).ToArray())
            {
                if (text2 != "")
                {
                    text = text + text2 + "\n";
                }
            }
            NotifiText.text = text;
        }

        private GameObject HUDObj;

        private GameObject HUDObj2;

        private GameObject MainCamera;

        private Text Testtext;

        private Material AlertText = new Material(Shader.Find("GUI/Text Shader"));

        private int NotificationDecayTime = 144;

        private int NotificationDecayTimeCounter;

        public static int NoticationThreshold = 30;

        private string[] Notifilines;

        private string newtext;

        public static string PreviousNotifi;

        public static bool disableNotifications;

        private bool HasInit;

        private static Text NotifiText;

        public static bool IsEnabled = true;
    }
}