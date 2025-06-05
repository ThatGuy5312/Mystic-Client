using HarmonyLib;
using UnityEngine;

namespace MysticClient.Patches
{
    public class RigPatch : MonoBehaviour
    {
        [HarmonyPatch(typeof(VRRig), "OnDisable")]
        private static bool Prefix(VRRig __instance) => !__instance.isOfflineVRRig;

        [HarmonyPatch(typeof(VRRigJobManager), "DeregisterVRRig")]
        private static bool Prefix(VRRigJobManager __instance, VRRig rig) => !rig.isOfflineVRRig;
    }
}
