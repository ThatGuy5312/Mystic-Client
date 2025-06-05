using static MysticClient.Menu.Buttons;
using UnityEngine;
using System.Linq;
using BepInEx;

namespace MysticClient.Classes
{
    public class MysticMenuPlugin : BaseUnityPlugin
    {
        public ButtonInfo[] GetInner(int outer = 0, int inner = 0) => buttons[outer][inner];

        public ButtonInfo[][] GetOuter(int outer = 0) => buttons[outer];

        public ButtonInfo[] GetInnerBackup(int outer = 0, int inner = 0) => Plugin.buttonBackup[outer][inner];

        public ButtonInfo[][] GetOuterBackup(int outer = 0) => Plugin.buttonBackup[outer];
    }

    public static class PluginUtils
    {
        public static ButtonInfo[] AddInfo(this ButtonInfo[] buttons, ButtonInfo button, bool atEnd)
        {
            if (atEnd)
                return buttons.Concat(new[] { button }).ToArray();
            else return new[] { button }.Concat(buttons).ToArray();
        }

        public static ButtonInfo[][] AddInfo(this ButtonInfo[][] list, ButtonInfo[] buttons, bool atEnd)
        {
            if (atEnd)
                return list.Concat(new[] { buttons }).ToArray();
            else return new[] { buttons }.Concat(list).ToArray();
        }
    }
}