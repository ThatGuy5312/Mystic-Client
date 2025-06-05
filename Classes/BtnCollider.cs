using System;
using MysticClient.Utils;
using UnityEngine;
using static MysticClient.Menu.Main;
using static MysticClient.Menu.MenuSettings;

namespace MysticClient.Classes
{
	public class BtnCollider : MonoBehaviour
	{
		public string relatedText;
		public static float buttonCooldown = 0f;		
		public void OnTriggerEnter(Collider other)
		{
			if (Time.time > buttonCooldown && other.name.Contains("buttonPresser") && menu != null)
			{
                buttonCooldown = Time.time + .2f;
                var isLeft = other.name.Contains("Left");
                if (GetEnabled("Face Menu"))
                    RigUtils.MyOnlineRig.StartVibration(isLeft, RigUtils.MyOnlineRig.tagHapticStrength / 2f, RigUtils.MyOnlineRig.tagHapticDuration / 2f);
                else RigUtils.MyOnlineRig.StartVibration(GetEnabled("Right Hand Menu"), RigUtils.MyOnlineRig.tagHapticStrength / 2f, RigUtils.MyOnlineRig.tagHapticDuration / 2f);
                //RigUtils.MyOfflineRig.PlayHandTapLocal(buttonSound, GetEnabled("Right Hand Menu"), .4f);
                PlayButtonSound(relatedText);
				Toggle(relatedText);
            }
		}
	}
}
public class KeyboardButton : MonoBehaviour
{
    private static float pressCooldown;

    public void OnTriggerEnter(Collider other)
    {
        if (Time.time > pressCooldown && other.name.Contains("buttonPresser"))
        {
            pressCooldown = Time.time + .2f;
            var isLeft = other.name.Contains("Left");
            RigUtils.MyOnlineRig.StartVibration(isLeft, RigUtils.MyOnlineRig.tagHapticStrength / 2f, RigUtils.MyOnlineRig.tagHapticDuration / 2f);
            RigUtils.MyOfflineRig.PlayHandTapLocal(66, GetEnabled("Right Hand Menu"), .4f);

            // thanks chatgpt
            if (gameObject.name == "A") KeyboardInput += "A";
            if (gameObject.name == "B") KeyboardInput += "B";
            if (gameObject.name == "C") KeyboardInput += "C";
            if (gameObject.name == "D") KeyboardInput += "D";
            if (gameObject.name == "E") KeyboardInput += "E";
            if (gameObject.name == "F") KeyboardInput += "F";
            if (gameObject.name == "G") KeyboardInput += "G";
            if (gameObject.name == "H") KeyboardInput += "H";
            if (gameObject.name == "I") KeyboardInput += "I";
            if (gameObject.name == "J") KeyboardInput += "J";
            if (gameObject.name == "K") KeyboardInput += "K";
            if (gameObject.name == "L") KeyboardInput += "L";
            if (gameObject.name == "M") KeyboardInput += "M";
            if (gameObject.name == "N") KeyboardInput += "N";
            if (gameObject.name == "O") KeyboardInput += "O";
            if (gameObject.name == "P") KeyboardInput += "P";
            if (gameObject.name == "Q") KeyboardInput += "Q";
            if (gameObject.name == "R") KeyboardInput += "R";
            if (gameObject.name == "S") KeyboardInput += "S";
            if (gameObject.name == "T") KeyboardInput += "T";
            if (gameObject.name == "U") KeyboardInput += "U";
            if (gameObject.name == "V") KeyboardInput += "V";
            if (gameObject.name == "W") KeyboardInput += "W";
            if (gameObject.name == "X") KeyboardInput += "X";
            if (gameObject.name == "Y") KeyboardInput += "Y";
            if (gameObject.name == "Z") KeyboardInput += "Z";
            // thanks chatgpt

            if (gameObject.name == "Space") KeyboardInput += " ";
            if (gameObject.name == "Delete" && KeyboardInput.Length > 0) try { KeyboardInput = KeyboardInput[..^1];/*<-what the sigma*/ } catch { }
            if (gameObject.name == "Next_Page") Toggle("NextPage");
            if (gameObject.name == "Previous_Page") Toggle("PreviousPage");
            if (gameObject.name == "Exit") inKeyboard = false;

            RecreateMenu();
        }
    }
}
