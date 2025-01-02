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
		public void OnTriggerEnter(Collider collider)
		{
			if (Time.time > buttonCooldown && collider.name == "buttonPresser" && menu != null)
			{
                buttonCooldown = Time.time + .2f;
                RigUtils.MyOnlineRig.StartVibration(GetEnabled("Right Hand Menu"), RigUtils.MyOnlineRig.tagHapticStrength / 2f, RigUtils.MyOnlineRig.tagHapticDuration / 2f);
                //RigUtils.MyOfflineRig.PlayHandTapLocal(buttonSound, GetEnabled("Right Hand Menu"), .4f);
				PlayButtonSound(relatedText);
				Toggle(relatedText);
            }
		}
	}
}
public class KeyboardButton : MonoBehaviour
{
    public string buttonText;
    public Action buttonAction;
    private static float pressCooldown;

    private void OnTriggerEnter(Collider other)
    {
        if (Time.time > pressCooldown && other.name.Contains("buttonPresser"))
        {
            pressCooldown = Time.time + 0.2f;
            var isRight = other.name.Contains("Right");
            RigUtils.MyOnlineRig.StartVibration(isRight, RigUtils.MyOnlineRig.tagHapticStrength / 2f, RigUtils.MyOnlineRig.tagHapticDuration / 2f);
            RigUtils.MyOfflineRig.PlayHandTapLocal(66, isRight, 0.4f);
            buttonAction?.Invoke();
        }
    }
}
