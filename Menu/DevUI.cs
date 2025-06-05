using UnityEngine;
using UnityEngine.InputSystem;
using SColor = System.Drawing.Color;
using static MysticClient.Menu.MenuSettings;
using MysticClient.Utils;
using MysticClient.Notifications;
using OVR.OpenVR;

namespace MysticClient.Menu
{
	public class DevUI : MonoBehaviour
	{
		private const bool DeveloperBuild = true;

        private const int windowID = 2389457;

        private Rect windowRect = new Rect(20, 20, 450, 450);
        private Rect warningRect = new Rect(1470, 0, 95, 95);
        private Rect devLabelRect = new Rect(1550, 0, 500, 45);

        private Vector2 scroll = Vector2.zero;

        private bool inGUI = false;
        private bool inUIConfig;
        private bool inDevTesting;
        private bool sendingRPCs = false;

        private string testinput = "Test";
        private string rpcText = "FriendRequestCompletedRPC";

        private Color devBuildLabelColor = Color.black;

        void Update()
        {
            devBuildLabelColor = Color.Lerp(NormalColor, outlineColor, Mathf.PingPong(Time.time, 1f));
            if (Keyboard.current.f2Key.wasPressedThisFrame)
            {
                inGUI = !inGUI;
                windowRect.x = Event.current.mousePosition.x;
                windowRect.y = Event.current.mousePosition.y;
            }
        }

        void OnGUI()
        {
            GUI.backgroundColor = NormalColor;
            if (DeveloperBuild)
            {
                if (Main.PathTextures[0] != null)
                {
                    var matrix = GUI.matrix;
                    GUIUtility.RotateAroundPivot(Mathf.Sin(Time.time * 2f) * 10f, warningRect.center);
                    GUI.Label(warningRect, Main.PathTextures[1]);
                    GUI.matrix = matrix;
                }
                GUI.Label(devLabelRect, "Development Build", new GUIStyle(GUI.skin.label) { normal = { textColor = devBuildLabelColor }, fontSize = 40, fontStyle = FontStyle.BoldAndItalic });
                if (inGUI) windowRect = GUILayout.Window(windowID, windowRect, MainUI, "Developer UI");
            }
        }

        void MainUI(int windowID)
        {
            GUILayout.BeginVertical();
            scroll = GUILayout.BeginScrollView(scroll);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Menu UI Config")) { inUIConfig = false; inDevTesting = false; }
            if (GUILayout.Button("UI Dev Config")) { inUIConfig = true; inDevTesting = false; }
            if (GUILayout.Button("Dev Testing")) { inUIConfig = false; inDevTesting = true; }
            GUILayout.EndHorizontal();

            if (!inDevTesting)
            {
                if (!inUIConfig)
                {
                    GUILayout.Label("Parent Size");
                    parentSize.ToTextbox();

                    GUILayout.Label("Setting Button Offset");
                    settingButtonOffset.ToTextbox();

                    GUILayout.Label("Search Button Pos");
                    search.ToTextbox();

                    GUILayout.Label("Home Button Pos");
                    returnAddAmount.ToTextbox();
                }
                else
                {
                    GUILayout.Label("Dev Text [Top Right]");
                    devLabelRect.ToTextbox();

                    GUILayout.Label("Warning Icon [Top Right]");
                    warningRect.ToTextbox();
                }
            }
            else
            {
                testinput = GUILayout.TextArea(testinput);
                if (GUILayout.Button("Send OnScreen Notif")) ScreenNotifs.SendOnScreenNotif(testinput);
                GUILayout.Label("Screen Notif Y Pos");
                ScreenNotifs.YStartPos.ToTextbox();

                GUILayout.Label("Pickaxe Rotation");
                pickRot.ToTextbox();
            }

            GUILayout.EndScrollView();
            GUILayout.EndVertical();
            GUI.DragWindow();
        }



        // dev vars

        public static Quaternion pickRot = new Quaternion(-90f, 50f, 210f, 190f);
    }
}