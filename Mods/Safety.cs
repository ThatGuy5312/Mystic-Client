using GorillaNetworking;
using MysticClient.Notifications;
using MysticClient.Utils;
using Photon.Pun;
using System.Collections;
using static MysticClient.Menu.Main;
using UnityEngine;
using BepInEx;
using MysticClient.Menu;
using UnityEngine.XR;
using System;

namespace MysticClient.Mods
{
    public class Safety : MonoBehaviour
    {
        public static void MetaReport()
        {
            if (XRSettings.isDeviceActive)
                foreach (var metaReport in Resources.FindObjectsOfTypeAll<GorillaMetaReport>())
                    if (!metaReport.gameObject.activeSelf)
                    {
                        metaReport.enabled = true;
                        metaReport.gameObject.SetActive(true);
                        metaReport.Invoke("StartOverlay", .1f);
                    }
                    else NotifiLib.SendNotification(NotifUtils.Warning() + "You Are Already In The Meta Report Menu");
            else NotifiLib.SendNotification(NotifUtils.Error() + "You Are Not On Your VR Headset");
        }
        public static void AntiReport() // thanks drew
        {
            try
            {
                foreach (var lines in GorillaScoreboardTotalUpdater.allScoreboardLines)
                {
                    if (lines.linePlayer != RigUtils.MyNetPlayer)
                        continue;
                    var button = lines.reportButton.gameObject.transform;
                    foreach (var rigs in RigUtils.VRRigs)
                    {
                        if (rigs == RigUtils.MyOfflineRig)
                            continue;
                        var rightHand = Vector3.Distance(rigs.rightHandTransform.position, button.position);
                        var leftHand = Vector3.Distance(rigs.leftHandTransform.position, button.position);
                        if (rightHand < .5f || leftHand < .5f)
                        {
                            PhotonNetwork.Disconnect();
                            RPCProtection();
                            NotifiLib.SendNotification(NotifUtils.AntiReport() + "Player " + rigs.playerName + " Attempted To Report You, You Have Been Disconnected");
                            break;
                        }
                    }
                }
            } catch (Exception ex) { Debug.LogError($"AntiReport: Encountered An Error: {ex.Message}"); }
        }
    }
}