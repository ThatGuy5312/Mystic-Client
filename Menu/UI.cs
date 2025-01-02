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
                GUI.backgroundColor = MenuSettings.NormalColor;
                GUI.contentColor = Main.buttonTextColor;
                GUILayout.BeginHorizontal();
                GUILayout.Space(5);
                GUILayout.BeginVertical();
                GUILayout.Space(5);
                foreach (var btnss in Buttons.buttons)
                    foreach (var btns in btnss)
                        foreach (var btn in btns)
                            if (btn.enabled)
                            {
                                GUILayout.BeginHorizontal();
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