﻿using MysticClient.Utils;
using static MysticClient.Menu.Main;
using MysticClient.Menu;
using UnityEngine;
using Photon.Pun;
using MysticClient.Classes;
using MysticClient.Notifications;
using UnityEngine.Animations.Rigging;
using GorillaLocomotion.Gameplay;
using static ThrowableBug;
using PlayFab.ClientModels;

namespace MysticClient.Mods
{
    public class Overpowered : GunLib
    {
        public static void SpazRopeGun(string btnname)
        {
            if (PhotonSystem.InRoom)
            {
                if (CreateGun(out RaycastHit hit))
                {
                    var rope = hit.collider.GetComponentInParent<GorillaRopeSwing>();
                    if (rope)
                    {
                        var force = new Vector3(Random.Range(-50f, 50f), Random.Range(-50f, 50f), Random.Range(-50f, 50f));
                        RPCManager.RopeEvent(force, rope, RpcTarget.All);
                        RPCProtection();
                    }
                }
            }
            else
            {
                NotifiLib.SendNotification(NotifUtils.Error() + "YOU ARE NOT IN A ROOM");
                GetIndex(btnname).enabled = false;
            }
        }
        public static void RopeSpaz(string btnname)
        {
            if (PhotonSystem.InRoom)
            {
                if (Controller.rightControllerIndexFloat.TriggerDown() || UserInput.GetMouseButton(0))
                {
                    var force = new Vector3(Random.Range(-50f, 50f), Random.Range(-50f, 50f), Random.Range(-50f, 50f));
                    RPCManager.RopeEvent(force, RpcTarget.All);
                    RPCProtection();
                }
            }
            else
            {
                NotifiLib.SendNotification(NotifUtils.Error() + "YOU ARE NOT IN A ROOM");
                GetIndex(btnname).enabled = false;
            }
        }
        public static void RopeDownGun(string btnname)
        {
            if (PhotonSystem.InRoom)
            {
                if (CreateGun(out RaycastHit hit))
                {
                    var rope = hit.collider.GetComponentInParent<GorillaRopeSwing>();
                    if (rope)
                    {
                        var force = new Vector3(0, -50, 0);
                        RPCManager.RopeEvent(force, rope, RpcTarget.All);
                        RPCProtection();
                    }
                }
            }
            else
            {
                NotifiLib.SendNotification(NotifUtils.Error() + "YOU ARE NOT IN A ROOM");
                GetIndex(btnname).enabled = false;
            }
        }
        public static void RopeDown(string btnname)
        {
            if (PhotonSystem.InRoom)
            {
                if (Controller.rightControllerIndexFloat.TriggerDown() || UserInput.GetMouseButton(0))
                {
                    var force = new Vector3(0, -50, 0);
                    RPCManager.RopeEvent(force, RpcTarget.All);
                    RPCProtection();
                }
            }
            else
            {
                NotifiLib.SendNotification(NotifUtils.Error() + "YOU ARE NOT IN A ROOM");
                GetIndex(btnname).enabled = false;
            }
        }
        public static void RopeUpGun(string btnname)
        {
            if (PhotonSystem.InRoom)
            {
                if (CreateGun(out RaycastHit hit))
                {
                    var rope = hit.collider.GetComponentInParent<GorillaRopeSwing>();
                    if (rope)
                    {
                        var force = new Vector3(0, 50, 0);
                        RPCManager.RopeEvent(force, rope, RpcTarget.All);
                        RPCProtection();
                    }
                }
            }
            else
            {
                NotifiLib.SendNotification(NotifUtils.Error() + "YOU ARE NOT IN A ROOM");
                GetIndex(btnname).enabled = false;
            }
        }
        public static void RopeUp(string btnname)
        {
            if (PhotonSystem.InRoom)
            {
                if (Controller.rightControllerIndexFloat.TriggerDown() || UserInput.GetMouseButton(0))
                {
                    var force = new Vector3(0, 50, 0);
                    RPCManager.RopeEvent(force, RpcTarget.All);
                    RPCProtection();
                }
            }
            else
            {
                NotifiLib.SendNotification(NotifUtils.Error() + "YOU ARE NOT IN A ROOM");
                GetIndex(btnname).enabled = false;
            }
        }
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
        public static void SlowGun(string tooltip)
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
                    if (Controller.rightControllerIndexFloat.TriggerDown())
                        RPCManager.SlowEvent(NetEventOptions.RecieverTarget.others);
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
                    if (Controller.rightControllerIndexFloat.TriggerDown())
                        RPCManager.VibrateEvent(NetEventOptions.RecieverTarget.others);
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
                                AddInfected(RigUtils.GetNetFromRig(RigUtils.GetClosestVRRig(3f)));
                            else RigUtils.MyPlayer.rightControllerTransform.position = RigUtils.GetClosestVRRig(3f).transform.position;
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
                    SpamInfected(PhotonSystem.LocalPlayer);
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
                        SpamInfected(RigUtils.GetNetFromRig(rig));
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
                    foreach (var players in PhotonSystem.AllNetPlayers)
                        SpamInfected(players);
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
                            NotifiLib.SendNotification(NotifUtils.Success() + $"Player {RigUtils.GetNetFromRig(rig).NickName} Has Been Un-Tagged", 5f);
                        } else NotifiLib.SendNotification(NotifUtils.Warning() + $"Player {RigUtils.GetNetFromRig(rig).NickName} Is Already Un-Tagged", 5f);
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
                            NotifiLib.SendNotification(NotifUtils.Success() + $"Player {rig.playerNameVisible} Has Been Tagged", 5f);
                        } else NotifiLib.SendNotification(NotifUtils.Success() + $"Player {rig.playerNameVisible} Is Already Tagged", 5f);
                    }
                    else
                    {
                        if (!IsTagged(rig.Creator))
                        {
                            LockOnRig(rig);
                            RigUtils.MyOfflineRig.enabled = false;
                            RigUtils.MyOfflineRig.rightHandTransform.SetPosition(rig.Position());
                            RigUtils.MyOfflineRig.leftHandTransform.SetPosition(rig.Position());
                            RigUtils.MyOfflineRig.transform.SetPosition(rig.Position());
                        } else NotifiLib.SendNotification(NotifUtils.Success() + $"Player {rig.playerNameVisible} Has Been Tagged Or Already Was Tagged", 5f);
                    }
                } else { if (!PhotonSystem.IsMasterClient) RigUtils.MyOfflineRig.enabled = true; }
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
                            NotifiLib.SendNotification(NotifUtils.Success() + "Everyone Has Been Un-Tagged", 5f);
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
                    if (Controller.rightControllerPrimaryButton)
                    {   
                        if (!IsTagged(PhotonSystem.LocalPlayer))
                        {
                            GetToolTip(tooltip).enabled = false;
                            NotifiLib.SendNotification(NotifUtils.Error() + "YOU MUST BE TAGGED OR MASTER");
                        }
                        else
                            foreach (var rigs in RigUtils.VRRigs)
                            {
                                RigUtils.MyOfflineRig.enabled = false;
                                RigUtils.MyOfflineRig.transform.SetPosition(rigs.Position() - new Vector3(0, 2, 0));
                                RigUtils.MyOfflineRig.rightHandTransform.SetPosition(rigs.Position());
                                RigUtils.MyOfflineRig.leftHandTransform.SetPosition(rigs.Position());
                            }
                    } else RigUtils.MyOfflineRig.enabled = true;
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