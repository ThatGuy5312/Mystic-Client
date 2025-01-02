using HarmonyLib;
using UnityEngine;
using MysticClient.Menu;

namespace MysticClient.Patches
{
    [HarmonyPatch(typeof(GameObject), "CreatePrimitive")]
    public class ShaderFix : MonoBehaviour
    {
        private static void Postfix(GameObject __result) => __result.GetComponent<Renderer>().material.shader = Main.GetEnabled("Shiny Menu") ? Main.UniversalShader : Main.UberShader;
    }
}