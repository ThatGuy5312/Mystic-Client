using UnityEngine;
using HarmonyLib;
using GorillaTagScripts;

namespace MysticClient.Patches
{
	[HarmonyPatch(typeof(BuilderTableNetworking), "OnLeftRoom")]
	public class LeaveBuilderPatch : MonoBehaviour
	{
		private static bool Prefix()
		{
			return false;
		}
	}
}