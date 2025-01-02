using System.Collections;
using UnityEngine;

namespace MysticClient.Notifications
{
    public class NotifUtils : MonoBehaviour
    {
        public static string ColorText(string text, string color)
        {
            return "<color=" + color + ">" + text + "</color>";
        }
        public static string Menu()
        {
            return "<color=grey>[</color><color=purple>Menu</color><color=grey>]</color> ";
        }
        public static string Voice()
        {
            return "<color=grey>[</color><color=purple>VOICE</color><color=grey>]</color> ";
        }
        public static string Success()
        {
            return "<color=grey>[</color><color=blue>SUCCESS</color><color=grey>]</color> ";
        }
        public static string Error()
        {
            return "<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> ";
        }
        public static string Warning()
        {
            return "<color=grey>[</color><color=orange>Warning</color><color=grey>]</color> ";
        }
        public static string AntiReport()
        {
            return "<color=grey>[</color><color=purple>ANTI-REPORT</color><color=grey>]</color> ";
        }
        public static string Room()
        {
            return "<color=grey>[</color><color=purple>ROOM</color><color=grey>]</color> ";
        }
    }
}