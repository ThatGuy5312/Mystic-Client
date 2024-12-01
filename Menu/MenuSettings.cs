using MysticClient.Classes;
using UnityEngine;

namespace MysticClient.Menu
{
    public class MenuSettings : MonoBehaviour
    {
        public static int buttonSound = 67;
        public static int physicSetting;

        public static float[] disconnectYs = { -1.122f, -0.331f };
        public static Vector3 buttonTextPos = new Vector3(0.064f, 0f, 0.111f);
        public static float buttonTextOffset = 2.6f; // 3.05
        public static Vector3 buttonSize = new Vector3(0.09f, 0.8f, 0.08f);
        public static Vector3 buttonPos = new Vector3(0.56f, 0f, 0.29f);
        public static float buttonOffset = 1f; // 1.2
        public static Vector3 menuSize = new Vector3(0.1f, 1f, 1.2f);
        public static Vector3 menuPos = new Vector3(0.05f, 0f, -0.05f);
        public static Vector3 pageButtonSize = new Vector3(0.045f, 0.25f, 0.064295f);
        public static Vector3[] pagePoss = { new Vector3(0.56f, 0.37f, 0.540f), new Vector3(0.56f, -0.37f, 0.540f) };
        public static float toolTipZ = -0.28f;
        public static float menuTitleZ = 0.173f;
        public static float dateTimeZ = 0.2f;
        public static Vector3 pointerPosition = new Vector3(0f, -0.1f, 0f);
        public static FontStyle titleFontStyle = FontStyle.Normal;
        public static FontStyle buttonFontStyle = FontStyle.Normal;
        
        public static string menuTitle = "<color=magenta>Mystic</color> <color=blue>Client</color> <color=cyan>V0.8.5</color>";
        public static Font currentFont = Resources.GetBuiltinResource<Font>("Arial.ttf");

        public static Color FirstColor = Color.black;
        public static Color SecondColor = Color.black;
        public static Color NormalColor = Color.black;
        public static Color ButtonColorDisable = Color.cyan;
        public static Color ButtonColorEnabled = Color.gray;
    }
}
