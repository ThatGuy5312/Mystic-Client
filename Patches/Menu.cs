using System.Reflection;
using HarmonyLib;
using UnityEngine;

namespace MysticClient.Patches
{
    public class Menu : MonoBehaviour
    {
        public static bool IsPatched { get; private set; }

        internal static void ApplyHarmonyPatches()
        {
            if (!IsPatched)
            {
                instance ??= new Harmony(PluginInfo.GUID);
                instance.PatchAll(Assembly.GetExecutingAssembly());
                IsPatched = true;
            }
        }

        internal static void RemoveHarmonyPatches()
        {
            if (instance != null && IsPatched)
            {
                IsPatched = false;
            }
        }

        private static Harmony instance;
    }
}
