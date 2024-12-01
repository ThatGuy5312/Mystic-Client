using HarmonyLib;
using MysticClient.Menu;
using System.Collections;
using UnityEngine;

namespace MysticClient.Patches
{
    [HarmonyPatch(typeof(GorillaLevelScreen), "Awake")]
    public class ScreenPatch : MonoBehaviour
    {
        private static void Prefix(GorillaLevelScreen __instance)
        {
            var mat = new Material(Shader.Find("GorillaTag/UberShader"));
            mat.color = Main.boardColor;
            __instance.goodMaterial = mat;
            __instance.badMaterial = mat;
            __instance.goodMaterial.color = Main.boardColor;
            __instance.badMaterial.color = Main.boardColor;
            __instance.GetComponent<Renderer>().material = mat;
        }
    }
}