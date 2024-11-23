using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace MysticClient.Notifications
{
    public class NotifiLib : MonoBehaviour
    {
        GameObject HUDObj;
        GameObject HUDObj2;
        GameObject MainCamera;
        Text Testtext;
        Material AlertText = new Material(Shader.Find("GUI/Text Shader"));
        int NotificationDecayTime = 150;
        int NotificationDecayTimeCounter = 150;
        string[] Notifilines;
        string newtext;
        public static string PreviousNotifi;
        bool HasInit = false;
        static Text NotifiText;
        public static bool IsEnabled;

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
            HUDObj2.transform.SetParent(HUDObj2.transform);
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
            Testtext.font = Font.CreateDynamicFontFromOSFont("Agency FB", 24);
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
                    foreach (string Line in Notifilines)
                    {
                        if (Line != "")
                        {
                            newtext = newtext + Line + "\n";
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

        public static float ropedelay;

        public static void SendNotification(string NotificationText)
        {
            if (ropedelay < Time.time)
            {
                ropedelay = Time.time + 0.05f;
                if (IsEnabled)
                {
                    if (!NotificationText.Contains(Environment.NewLine)) { NotificationText = NotificationText + Environment.NewLine; }
                    NotifiText.text = NotifiText.text + NotificationText;
                    PreviousNotifi = NotificationText;
                    Debug.Log(NotificationText);
                }
            }
        }
        public static void ClearAllNotifications()
        {
            NotifiText.text = "";
        }

        public static void ClearPastNotifications(int amount)
        {
            string[] Notifilines = null;
            string newtext = "";
            Notifilines = NotifiText.text.Split(Environment.NewLine.ToCharArray()).Skip(amount).ToArray();
            foreach (string Line in Notifilines)
            {
                if (Line != "")
                {
                    newtext = newtext + Line + "\n";
                }
            }

            NotifiText.text = newtext;
        }
    }
}
