using MysticClient.Utils;
using static MysticClient.Menu.Main;
using MysticClient.Menu;
using UnityEngine;
using Photon.Pun;
using MysticClient.Classes;
using MysticClient.Notifications;
using UnityEngine.Animations.Rigging;

namespace MysticClient.Mods
{
    public class Overpowered : GunLib
    {
        public static void GliderBlindGun()
        {
            if (CreateGun(out VRRig rig))
                foreach (var gliders in Gliders())
                    if (gliders.GetView.Owner == RigUtils.MyRealtimePlayer)
                    {
                        gliders.transform.position = rig.transform.position;
                        gliders.transform.rotation = Quaternion.Euler(new Vector3(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f)));
                        LockOnRig(rig);
                    }
                    else
                        gliders.OnHover(null, null);
        }
        public static void GliderBlindAll()
        {
            foreach (var people in PhotonSystem.PlayerListOthers)
            {
                var glider = Gliders()[people.ActorNumber];
                if (glider.GetView.Owner == RigUtils.MyRealtimePlayer)
                {
                    glider.transform.position = RigUtils.GetRigFromPlayer(people).transform.position;
                    glider.transform.rotation = Quaternion.Euler(new Vector3(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f)));
                }
                else
                    glider.OnHover(null, null);
            }
        }
        /*public static void SlowGun(string tooltip)
        {
            if (PhotonSystem.InRoom)
            {
                if (PhotonSystem.IsMasterClient)
                {
                    if (CreateGun(out VRRig rig))
                    {
                        LockOnRig(rig);
                        RPCManager.SlowEvent(RigUtils.GetPlayerFromNet(RigUtils.GetPlayerFromRig(rig)));
                    }
                }
                else
                {
                    NotifiLib.SendNotification(NotifUtils.Error() + "You Are Not Master Client");
                    GetToolTip(tooltip).enabled = false;
                }
            }
            else
            {
                NotifiLib.SendNotification(NotifUtils.Error() + "YOU ARE NOT IN A ROOM");
                GetToolTip(tooltip).enabled = false;
            }
        }
        public static void VibrateGun(string tooltip)
        {
            if (PhotonSystem.InRoom)
            {
                if (PhotonSystem.IsMasterClient)
                {
                    if (CreateGun(out VRRig rig))
                    {
                        LockOnRig(rig);
                        RPCManager.VibrateEvent(RigUtils.GetPlayerFromNet(RigUtils.GetPlayerFromRig(rig)));
                    }
                }
                else
                {
                    NotifiLib.SendNotification(NotifUtils.Error() + "You Are Not Master Client");
                    GetToolTip(tooltip).enabled = false;
                }
            }
            else
            {
                NotifiLib.SendNotification(NotifUtils.Error() + "YOU ARE NOT IN A ROOM");
                GetToolTip(tooltip).enabled = false;
            }
        }
        public static void SlowAll(string tooltip)
        {
            if (PhotonSystem.InRoom)
            {
                if (PhotonSystem.IsMasterClient)
                {
                    if (Controller.rightControllerIndexFloat > 0.3f)
                    {
                        RPCManager.SlowEvent(NetEventOptions.RecieverTarget.others);
                    }
                }
                else
                {
                    NotifiLib.SendNotification(NotifUtils.Error() + "You Are Not Master Client");
                    GetToolTip(tooltip).enabled = false;
                }
            }
            else
            {
                NotifiLib.SendNotification(NotifUtils.Error() + "YOU ARE NOT IN A ROOM");
                GetToolTip(tooltip).enabled = false;
            }
        }
        public static void VibrateAll(string tooltip)
        {
            if (PhotonSystem.InRoom)
            {
                if (PhotonSystem.IsMasterClient)
                {
                    if (Controller.rightControllerIndexFloat > 0.3f)
                    {
                        RPCManager.VibrateEvent(NetEventOptions.RecieverTarget.others);
                    }
                }
                else
                {
                    NotifiLib.SendNotification(NotifUtils.Error() + "You Are Not Master Client");
                    GetToolTip(tooltip).enabled = false;
                }
            }
            else
            {
                NotifiLib.SendNotification(NotifUtils.Error() + "YOU ARE NOT IN A ROOM");
                GetToolTip(tooltip).enabled = false;
            }
        }*/
        public static void TagAura(string tooltip)
        {
            if (PhotonSystem.InRoom)
            {
                if (!IsTagged(PhotonSystem.LocalPlayer))
                {
                    foreach (var rigs in RigUtils.VRRigs)
                    {
                        if (!IsTagged(RigUtils.GetNetFromRig(rigs)))
                        {
                            if (PhotonSystem.IsMasterClient)
                            {
                                int players = PhotonSystem.AllNetPlayers.Length;
                                if (players == 1 || players == 2 || players == 3)
                                    AddInfected(RigUtils.GetNetFromRig(RigUtils.GetClosestVRRig(3f)));
                                else
                                    AddInfected(RigUtils.GetNetFromRig(RigUtils.GetClosestVRRig(int.MaxValue)));
                            } else RigUtils.MyPlayer.rightControllerTransform.position = RigUtils.GetClosestVRRig(3f).transform.position;
                        }
                    }
                }
                else
                {
                    NotifiLib.SendNotification(NotifUtils.Warning() + "You Must Be Tagged To Use This");
                    GetToolTip(tooltip).enabled = false;
                }
            }
            else
            {
                NotifiLib.SendNotification(NotifUtils.Error() + "YOU ARE NOT IN A ROOM");
                GetToolTip(tooltip).enabled = false;
            }
        }
        public static void SpamTagSelf(string tooltip)
        {
            if (PhotonSystem.InRoom)
            {
                if (PhotonSystem.IsMasterClient)
                {
                    SpamInfected(PhotonSystem.LocalPlayer);
                }
                else
                {
                    NotifiLib.SendNotification(NotifUtils.Error() + "You Are Not Master Client");
                    GetToolTip(tooltip).enabled = false;
                }
            }
            else
            {
                NotifiLib.SendNotification(NotifUtils.Error() + "YOU ARE NOT IN A ROOM");
                GetToolTip(tooltip).enabled = false;
            }
        }
        public static void SpamTagGun(string tooltip)
        {
            if (PhotonSystem.InRoom)
            {
                if (PhotonSystem.IsMasterClient)
                {
                    if (CreateGun(out VRRig rig))
                    {
                        SpamInfected(RigUtils.GetNetFromRig(rig));
                    }
                }
                else
                {
                    NotifiLib.SendNotification(NotifUtils.Error() + "You Are Not Master Client");
                    GetToolTip(tooltip).enabled = false;
                }
            }
            else
            {
                NotifiLib.SendNotification(NotifUtils.Error() + "YOU ARE NOT IN A ROOM");
                GetToolTip(tooltip).enabled = false;
            }
        }
        public static void SpamTagAll(string tooltip)
        {
            if (PhotonSystem.InRoom)
            {
                if (PhotonSystem.IsMasterClient)
                {
                    foreach (var players in PhotonSystem.AllNetPlayers)
                    {
                        SpamInfected(players);
                    }
                }
                else
                {
                    NotifiLib.SendNotification(NotifUtils.Error() + "You Are Not Master Client");
                    GetToolTip(tooltip).enabled = false;
                }
            }
            else
            {
                NotifiLib.SendNotification(NotifUtils.Error() + "YOU ARE NOT IN A ROOM");
                GetToolTip(tooltip).enabled = false;
            }
        }
        public static void UnTagGun(string tooltip)
        {
            if (PhotonSystem.InRoom)
            {
                if (CreateGun(out VRRig rig))
                {
                    if (PhotonSystem.IsMasterClient)
                    {
                        if (IsTagged(RigUtils.GetNetFromRig(rig)))
                        {
                            RemoveInfected(RigUtils.GetNetFromRig(rig));
                            NotifiLib.SendNotification(NotifUtils.Success() + $"Player {RigUtils.GetNetFromRig(rig).NickName} Has Been Un-Tagged");
                        }
                        else
                        {
                            NotifiLib.SendNotification(NotifUtils.Warning() + $"Player {RigUtils.GetNetFromRig(rig).NickName} Is Already Un-Tagged");
                        }
                    }
                    else
                    {
                        NotifiLib.SendNotification(NotifUtils.Error() + "You Are Not Master Client");
                        GetToolTip(tooltip).enabled = false;
                    }
                }
            }
            else
            {
                NotifiLib.SendNotification(NotifUtils.Error() + "YOU ARE NOT IN A ROOM");
                GetToolTip(tooltip).enabled = false;
            }
        }
        public static void TagGun(string tooltip)
        {
            if (PhotonSystem.InRoom)
            {
                if (CreateGun(out VRRig rig))
                {
                    if (PhotonSystem.IsMasterClient)
                    {
                        if (!IsTagged(RigUtils.GetNetFromRig(rig)))
                        {
                            AddInfected(RigUtils.GetNetFromRig(rig));
                            NotifiLib.SendNotification(NotifUtils.Success() + $"Player {rig.playerName} Has Been Tagged");
                        }
                        else
                        {
                            NotifiLib.SendNotification(NotifUtils.Success() + $"Player {rig.playerName} Is Already Tagged");
                        }
                    }
                    else
                    {
                        if (!IsTagged(RigUtils.GetNetFromRig(rig)))
                        {
                            LockOnRig(rig);
                            RigUtils.MyOfflineRig.enabled = false;
                            RigUtils.MyOfflineRig.transform.position = rig.transform.position - new Vector3(0, 3, 0);
                            RigUtils.MyNetworkView.transform.position = rig.transform.position - new Vector3(0, 3, 0);
                            RigUtils.MyPlayer.rightControllerTransform.position = rig.transform.position;
                        }
                        else
                        {
                            NotifiLib.SendNotification(NotifUtils.Success() + $"Player {rig.playerName} Has Been Tagged Or Already Was Tagged");
                        }
                    }
                }
            }
            else
            {
                NotifiLib.SendNotification(NotifUtils.Error() + "YOU ARE NOT IN A ROOM");
                GetToolTip(tooltip).enabled = false;
            }
        }
        public static void UnTagAll(string tooltip)
        {
            if (PhotonSystem.InRoom)
            {
                if (PhotonSystem.IsMasterClient)
                {
                    foreach (var players in PhotonSystem.PlayerListOthers)
                    {
                        if (IsTagged(players))
                        {
                            RemoveInfected(players);
                            GetToolTip(tooltip).enabled = false;
                            NotifiLib.SendNotification(NotifUtils.Success() + "Everyone Has Been Un-Tagged");
                        }
                    }
                }
                else
                {
                    NotifiLib.SendNotification(NotifUtils.Error() + "You Are Not Master Client");
                    GetToolTip(tooltip).enabled = false;
                }
            }
            else
            {
                NotifiLib.SendNotification(NotifUtils.Error() + "YOU ARE NOT IN A ROOM");
                GetToolTip(tooltip).enabled = false;
            }
        }
        public static void TagAll(string tooltip)
        {
            if (PhotonSystem.InRoom)
            {
                if (PhotonSystem.IsMasterClient)
                {
                    foreach (var players in PhotonSystem.PlayerListOthers)
                    {
                        if (!IsTagged(players))
                        {
                            AddInfected(players);
                            GetToolTip(tooltip).enabled = false;
                            NotifiLib.SendNotification(NotifUtils.Success() + "Everyone Has Been Tagged");
                        }
                    }
                }
                else
                {
                    if (!IsTagged(PhotonSystem.LocalPlayer))
                    {
                        GetToolTip(tooltip).enabled = false;
                        NotifiLib.SendNotification(NotifUtils.Error() + "YOU MUST BE TAGGED OR MASTER");
                    }
                    else
                    {
                        foreach (var players in PhotonSystem.PlayerListOthers)
                        {
                            foreach (var rigs in RigUtils.VRRigs)
                            {
                                bool allTagged = false;
                                if (!IsTagged(players))
                                {
                                    allTagged = true;
                                    break;
                                }
                                if (allTagged)
                                {
                                    if (!IsTagged(players))
                                    {
                                        RigUtils.MyOfflineRig.enabled = false;
                                        RigUtils.MyOfflineRig.transform.position = rigs.transform.position;
                                        RigUtils.MyNetworkView.transform.position = rigs.transform.position;
                                        RigUtils.MyPlayer.rightControllerTransform.position = rigs.transform.position;
                                    }
                                }
                                else
                                {
                                    RigUtils.MyOfflineRig.enabled = true;
                                    NotifiLib.SendNotification(NotifUtils.Success() + "Everyone Has Been Tagged Or They Were Already Tagged");
                                    GetToolTip(tooltip).enabled = false;
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                NotifiLib.SendNotification(NotifUtils.Error() + "YOU ARE NOT IN A ROOM");
                GetToolTip(tooltip).enabled = false;
            }
        }
        public static void UnTagSelf(string tooltip)
        {
            if (PhotonSystem.InRoom)
            {
                if (PhotonSystem.IsMasterClient)
                {
                    if (IsTagged(PhotonSystem.LocalPlayer))
                    {
                        RemoveInfected(PhotonSystem.LocalPlayer);
                        NotifiLib.SendNotification(NotifUtils.Success() + "You Have Been Un-Tagged");
                        GetToolTip(tooltip).enabled = false;
                    }
                    else
                    {
                        NotifiLib.SendNotification(NotifUtils.Error() + "You Are Already Un-Tagged");
                        GetToolTip(tooltip).enabled = false;
                    }
                }
                else
                {
                    NotifiLib.SendNotification(NotifUtils.Warning() + "You Must Be Master Client");
                    GetToolTip(tooltip).enabled = false;
                }
            }
            else
            {
                NotifiLib.SendNotification(NotifUtils.Error() + "YOU ARE NOT IN A ROOM");
                GetToolTip(tooltip).enabled = false;
            }
        }
        private static bool tagCheck;
        public static void TagSelf(string tooltip)
        {
            if (PhotonSystem.InRoom)
            {
                if (!PhotonSystem.IsMasterClient)
                {
                    if (!IsTagged(PhotonSystem.LocalPlayer))
                    {
                        foreach (var players in PhotonSystem.AllNetPlayers)
                        {
                            var rig = RigUtils.GetRigFromPlayer(players);
                            if (rig != RigUtils.MyOfflineRig)
                            {
                                if (IsTagged(players))
                                {
                                    tagCheck = true;
                                    RigUtils.MyOfflineRig.enabled = false;
                                    RigUtils.MyOfflineRig.transform.position = rig.rightHandTransform.position;
                                    RigUtils.MyNetworkView.transform.position = rig.rightHandTransform.position;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (tagCheck)
                        {
                            NotifiLib.SendNotification(NotifUtils.Success() + "You Have Been Tagged Or You Were Already Tagged");
                            tagCheck = false;
                            RigUtils.MyOfflineRig.enabled = true;
                            GetToolTip(tooltip).enabled = false;
                        }
                        else
                        {
                            NotifiLib.SendNotification(NotifUtils.Warning() + "You Are Already Tagged");
                            GetToolTip(tooltip).enabled = false;
                        }
                    }
                }
                else
                {
                    AddInfected(RigUtils.MyNetPlayer);
                    NotifiLib.SendNotification(NotifUtils.Success() + "You Have Been Tagged");
                    GetToolTip(tooltip).enabled = false;
                }
            }
            else
            {
                NotifiLib.SendNotification(NotifUtils.Error() + "YOU ARE NOT IN A ROOM");
                GetToolTip(tooltip).enabled = false;
            }
        }
    }
}