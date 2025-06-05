using System.Collections;
using TMPro;
using UnityEngine;

namespace MysticClient.UISetting
{
    public class Interface
    {
        public static TextMeshPro Create(string name, ref GameObject parent, TextAlignmentOptions alignment)
        {
            parent = new GameObject(name);
            var text = parent.AddComponent<TextMeshPro>();
            var rect = parent.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(1.75f, 1.75f);
            text.lineSpacing = 25;
            text.font = GameObject.Find("motdtext").GetComponent<TMP_Text>().font;
            text.alignment = alignment;
            text.fontSize = .5f;
            parent.transform.LookAt(Camera.main.transform);
            parent.transform.Rotate(0, 180, 0);
            return text;
        }
    }
}