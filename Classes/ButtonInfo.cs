using System;

namespace MysticClient.Classes
{
    public class ButtonInfo
    {
        public string buttonText = "-";
        public Action method = null;
        public Action disableMethod = null;
        public bool enabled = false;
        public bool isTogglable = true;
        public string toolTip = "This button doesn't have a tooltip/tutorial.";
        public Action settingAction = null;
    }
}
