using GorillaNetworking;
using HarmonyLib;
using System;
using UnityEngine;

namespace MysticClient.Patches
{
    [HarmonyPatch(typeof(PlayFabAuthenticator), "ShowBanMessage")]
    public class BanMessage : MonoBehaviour
    {
        private static bool Prefix(PlayFabAuthenticator.BanInfo __instance)
        {
            try
            {
                if (__instance.BanExpirationTime != null && __instance.BanMessage != null)
                {
                    if (__instance.BanExpirationTime != "Indefinite")
                    {
                        int milliseconds = (int)(DateTime.Parse(__instance.BanExpirationTime) - DateTime.UtcNow).TotalMilliseconds;
                        int seconds = (int)(DateTime.Parse(__instance.BanExpirationTime) - DateTime.UtcNow).TotalSeconds;
                        int minutes = (int)(DateTime.Parse(__instance.BanExpirationTime) - DateTime.UtcNow).TotalMinutes;
                        int hours = (int)(DateTime.Parse(__instance.BanExpirationTime) - DateTime.UtcNow).TotalHours;
                        PlayFabAuthenticator.instance.gorillaComputer.GeneralFailureMessage(string.Concat(new string[]
                        {
                            "YOUR EXECUTION WILL HAPPIN IN\n ",
                            hours.ToString() + " | ",
                            minutes.ToString() + " | ",
                            seconds.ToString() + " | ",
                            milliseconds.ToString() + "\nREASON: ",
                            "COMMITING TERRISTIC CRIMES ON ANOTHER AXIOM."              
                        }));
                    }
                    else
                    {
                        PlayFabAuthenticator.instance.gorillaComputer.GeneralFailureMessage("YOUR ACCOUNT HAS BEEN BANNED INDEFINITELY.\nREASON: horrible");
                    }
                }
            }
            catch { }
            return false;
        }
    }
}