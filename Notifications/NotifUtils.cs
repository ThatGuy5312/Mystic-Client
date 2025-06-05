using System.Collections;
using UnityEngine;

namespace MysticClient.Notifications
{
    public class NotifUtils : MonoBehaviour
    {
        public static string ColorText(string text, string color) => "<color=" + color + ">" + text + "</color>";
        public static string Menu() => "<color=grey>[</color><color=purple>Menu</color><color=grey>]</color> ";
        public static string Voice() => "<color=grey>[</color><color=purple>VOICE</color><color=grey>]</color> ";
        public static string Success() => "<color=grey>[</color><color=blue>SUCCESS</color><color=grey>]</color> ";
        public static string Error() => "<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> ";
        public static string Warning() => "<color=grey>[</color><color=orange>Warning</color><color=grey>]</color> ";
        public static string AntiReport() => "<color=grey>[</color><color=purple>ANTI-REPORT</color><color=grey>]</color> ";
        public static string Room() => "<color=grey>[</color><color=purple>ROOM</color><color=grey>]</color> ";

        public static string MessageText(MessageInfo info)
        {
            if (info == MessageInfo.None) return "";
            else if (info == MessageInfo.Success) return Success();
            else if (info == MessageInfo.Error) return Error();
            else if (info == MessageInfo.Warning) return Warning();
            else if (info == MessageInfo.Menu) return Menu();
            else if (info == MessageInfo.Voice) return Voice();
            else if (info == MessageInfo.AntiReport) return AntiReport();
            else if (info == MessageInfo.Room) return Room();
            else return "";
        }

        public enum MessageInfo
        {
            None,
            Success,
            Error,
            Warning,
            Menu,
            Voice,
            AntiReport,
            Room
        }
    }
}