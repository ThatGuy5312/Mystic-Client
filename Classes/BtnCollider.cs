using MysticClient.Menu;
using MysticClient.Utils;
using Photon.Pun;
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
                buttonCooldown = Time.time + 0.2f;
                RigUtils.MyOnlineRig.StartVibration(GetEnabled("Right Hand Menu"), GorillaTagger.Instance.tagHapticStrength / 2f, GorillaTagger.Instance.tagHapticDuration / 2f);
                //RigUtils.MyOfflineRig.PlayHandTapLocal(buttonSound, GetEnabled("Right Hand Menu"), 0.4f);
				PlayButtonSound(relatedText);
				Toggle(relatedText);
            }
		}
	}
}
