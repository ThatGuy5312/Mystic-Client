using HarmonyLib;
using MysticClient.Utils;
using UnityEngine;

namespace MysticClient.Patches
{
    [HarmonyPatch(typeof(VRRig), "OnDisable")]
    internal class GhostPatch : MonoBehaviour
    {
        public static bool Prefix(VRRig __instance)
        {
            return !(__instance == RigUtils.MyOfflineRig);
        }
    }
}
