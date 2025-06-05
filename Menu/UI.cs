using MysticClient.Notifications;
using MysticClient.Utils;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace MysticClient.Menu
{
    public class UI : MonoBehaviour
    {
        void OnGUI()
        {
            if (Main.GetEnabled("Array List"))
            {
                var pix = Main.GetEnabled("Small Array List") ? 0 : 5;
                GUI.backgroundColor = MenuSettings.NormalColor;
                GUI.contentColor = MenuSettings.NormalColor;
                GUILayout.BeginHorizontal();
                GUILayout.Space(pix);
                GUILayout.BeginVertical();
                GUILayout.Space(pix);
                foreach (var btnss in Buttons.buttons)
                    foreach (var btns in btnss)
                        foreach (var btn in btns)
                            if (btn.enabled)
                            {
                                GUILayout.BeginHorizontal();
                                if (Main.GetEnabled("Small Array List"))
                                    GUILayout.Label(btn.buttonText, new GUIStyle(GUI.skin.label)
                                    {
                                        normal = { textColor = MenuSettings.NormalColor },
                                        fontStyle = FontStyle.BoldAndItalic
                                    });
                                else
                                    GUILayout.Label($"| {btn.buttonText}", new GUIStyle(GUI.skin.label)
                                    {
                                        normal = { textColor = MenuSettings.NormalColor },
                                        fontSize = 20,
                                        fontStyle = FontStyle.BoldAndItalic
                                    });
                                if (Main.GetEnabled("Array List Buttons"))
                                    if (GUILayout.Button("Disable"))
                                        btn.enabled = false;
                                GUILayout.EndHorizontal();
                            }
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
            }
        }
    }
}