using MysticClient.Utils;
using System.Collections;
using static MysticClient.Menu.Main;
using UnityEngine;
using MysticClient.Classes;
using Photon.Realtime;
using Photon.Pun;
using MysticClient.Notifications;
using MysticClient.Menu;
using GorillaTag;
using GorillaTagScripts;
using System.Reflection;
using System.Linq;
using OculusSampleFramework;
using MysticClient.Patches;
using static ThrowableBug;
using UnityEngine.Animations.Rigging;
using Valve.VR;
using System.Collections.Generic;
using Viveport;
using static SteamVR_Utils;
using WebSocketSharp;
using System.Diagnostics;
using UnityEngine.UI;

namespace MysticClient.Mods
{
    public class Fun : GunLib
    {
        // RigUtils.MyPlayer.transform.position + new Vector3(Mathf.Cos(Time.frameCount / 30f), 0f, Mathf.Sin(Time.frameCount / 30f)); orbit
        // Quaternion.Euler(new Vector3(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f))); spaz



        public static Texture2D mcBlockTexture = MCTextures[0];
        public static AudioClip mcSongClip = AudioClips[9];
        #region minecraft
        public static void PlayMinecraftSong(bool toStop)
        {
            if (toStop)
                Loaders.MCObject.Stop();
            else Loaders.PlayMCAudio(mcSongClip);
        }

        private static float addColliderDelay = 0f;
        public static void AddMCBlockCollider()
        {
            if (CreateGun(out RaycastHit hit))
            {
                var obj = hit.collider.gameObject;
                if (Time.time > addColliderDelay && obj && gridParent != null)
                {
                    try
                    {
                        if (obj.transform.IsChildOf(gridParent.transform))
                        {
                            obj.layer = 0;
                            addColliderDelay = Time.time + .2f;
                        }
                    }
                    catch { }
                }
            }
        }
        private static float destroyColliderDelay = 0f;
        public static void RemoveMCBlockCollider()
        {
            if (CreateGun(out RaycastHit hit))
            {
                var obj = hit.collider.gameObject;
                if (Time.time > destroyColliderDelay && obj && gridParent != null)
                {
                    try
                    {
                        if (obj.transform.IsChildOf(gridParent.transform))
                        {
                            obj.layer = 8;
                            destroyColliderDelay = Time.time + .2f;
                        }
                    }
                    catch { }
                }
            }
        }

        private static float destroyDelay = 0f;
        public static void DestroyMCBlockGun()
        {
            if (CreateGun(out RaycastHit hit))
            {
                var obj = hit.collider.gameObject;
                if (Time.time > destroyDelay && obj && gridParent != null)
                {
                    try // i dont like seeing errors
                    {
                        if (obj.transform.IsChildOf(gridParent.transform))
                        {
                            Destroy(obj);
                            destroyDelay = Time.time + .2f;
                        }
                    }
                    catch { }
                }
            }
        }
        private static List<GameObject> MinecraftCubes = new List<GameObject>();
        private static GameObject gridParent;
        private static GameObject placePos = null;
        private static float blockPlaceCooldown;
        private static LineRenderer mcLineRenderer;
        public static void ClearMCCubes(string btnname)
        {
            if (Controller.rightControllerPrimaryButton || UserInput.GetMouseButton(0))
                foreach (var obj in MinecraftCubes)
                    { Destroy(obj); GetIndex(btnname).enabled = false; }
            else if (Controller.rightControllerSecondaryButton || UserInput.GetMouseButton(1))
                GetIndex(btnname).enabled = false;
            else
                NotifiLib.SendNotification(NotifUtils.Menu() + "If You Want To Continue Press A To Comfirm Or Press B To Cancel", 5f);
        }
        public static void Minecraft()
        {
            if (Controller.rightGrab || UserInput.GetMouseButton(1))
            {
                if (mcLineRenderer == null)
                {
                    var obj = new GameObject("MCLineRenderer");
                    mcLineRenderer = obj.AddComponent<LineRenderer>();
                    mcLineRenderer.material = TransparentMaterial(GetChangeColorA(Color.Lerp(MenuSettings.NormalColor, Color.gray, Mathf.PingPong(Time.time, 1f)), .2f));
                    mcLineRenderer.startWidth = .05f;
                    mcLineRenderer.endWidth = .05f;
                }
                mcLineRenderer.material = TransparentMaterial(GetChangeColorA(Color.Lerp(MenuSettings.NormalColor, Color.gray, Mathf.PingPong(Time.time, 1f)), .2f));
                mcLineRenderer.SetPosition(0, RigUtils.MyPlayer.rightControllerTransform.position);
                mcLineRenderer.SetPosition(1, RigUtils.MyPlayer.rightControllerTransform.position + RigUtils.MyPlayer.rightControllerTransform.forward);
                if (gridParent == null)
                    gridParent = new GameObject("GridParent");
                var pos = RoundToGrid(mcLineRenderer.GetPosition(1));
                if (Time.time > blockPlaceCooldown)
                {
                    if (Controller.rightControllerIndexFloat.TriggerDown() || UserInput.GetMouseButton(0))
                    {
                        var block = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        if (mcBlockTexture == MCTextures[8])
                            block.GetComponent<Renderer>().material.shader = DefaultShader;
                        else if (mcBlockTexture == MCTextures[10])
                            block.GetComponent<Renderer>().material = TransparentMaterial(GetChangeColorA(Color.white, .5f));
                        else
                            block.GetComponent<Renderer>().material.shader = UniversalShader;
                        block.GetComponent<Renderer>().material.mainTexture = mcBlockTexture;
                        block.transform.position = pos;
                        block.transform.parent = gridParent.transform;
                        MinecraftCubes.Add(block);
                        blockPlaceCooldown = Time.time + .1f;
                    }
                }
                if (!Controller.rightControllerIndexFloat.TriggerDown() || !UserInput.GetMouseButton(0))
                {
                    if (placePos == null)
                        placePos = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    Destroy(placePos.GetComponent<Collider>());
                    placePos.GetComponent<Renderer>().material = TransparentMaterial(GetChangeColorA(Color.Lerp(Color.white, Color.gray, Mathf.PingPong(Time.time, 1f)), .2f));
                    placePos.GetComponent<Renderer>().material.mainTexture = mcBlockTexture;
                    placePos.transform.position = pos;
                    placePos.transform.parent = gridParent.transform;
                }
            }
            else
            {
                if (mcLineRenderer != null)
                    Destroy(mcLineRenderer.gameObject);
                if (placePos != null)
                    Destroy(placePos);
                mcLineRenderer = null;
                placePos = null;
            }
        }

        public class CubeManager : MonoBehaviour { public int cubeID; }

        #endregion

        public static void BlockHalo(string tooltip)
        {
            if (PhotonSystem.InRoom)
            {
                if (ZoneManagement.instance.IsZoneActive(GTZone.monkeBlocks))
                {
                    var rad = 1f;
                    var count = 8;
                    for (int i = 0; i < count; i++)
                    {
                        float angle = (i * 360f / count) + Time.time * 90f; // did this so if your using the mystic gui free cam this will still work
                        var pos = RigUtils.MyOnlineRig.rigidbody.useGravity ? RigUtils.MyPlayer.transform.position : RigUtils.MyOfflineRig.transform.position + new Vector3(
                            Mathf.Cos(angle * Mathf.Deg2Rad) * rad, 0, Mathf.Sin(angle * Mathf.Deg2Rad) * rad);
                        RPCManager.PieceEvent(gotPieceType, pos, RigUtils.MyOnlineRig.rightHandTransform.rotation);
                    }
                }
                else
                {
                    NotifiLib.SendNotification(NotifUtils.Error() + "You Are Not In The Monke Blocks Map");
                    GetToolTip(tooltip).enabled = false;
                }
            }
            else
            {
                NotifiLib.SendNotification(NotifUtils.Error() + "You Are Not In A Room");
                GetToolTip(tooltip).enabled = false;
            }
        }
        public static void SpamBlock(string tooltip)
        {
            if (PhotonSystem.InRoom)
            {
                if (ZoneManagement.instance.IsZoneActive(GTZone.monkeBlocks))
                {
                    if (UserInput.GetMouseButton(0) || Controller.rightGrab)
                    {
                        RPCManager.PieceEvent(gotPieceType, RigUtils.MyOnlineRig.rightHandTransform.position, RigUtils.MyOnlineRig.rightHandTransform.rotation);
                        RPCProtection(false);
                    }
                }
                else
                {
                    NotifiLib.SendNotification(NotifUtils.Error() + "You Are Not In The Monke Blocks Map");
                    GetToolTip(tooltip).enabled = false;
                }
            }
            else
            {
                NotifiLib.SendNotification(NotifUtils.Error() + "You Are Not In A Room");
                GetToolTip(tooltip).enabled = false;
            }
        }
        public static void SpamBlockGun(string tooltip)
        {
            if (PhotonSystem.InRoom)
            {
                if (ZoneManagement.instance.IsZoneActive(GTZone.monkeBlocks))
                {
                    if (CreateGun())
                    {
                        RPCManager.PieceEvent(gotPieceType, pointer.transform.position, pointer.transform.rotation);
                        RPCProtection(true);
                    }
                }
                else
                {
                    NotifiLib.SendNotification(NotifUtils.Error() + "You Are Not In The Monke Blocks Map");
                    GetToolTip(tooltip).enabled = false;
                }
            }
            else
            {
                NotifiLib.SendNotification(NotifUtils.Error() + "You Are Not In A Room");
                GetToolTip(tooltip).enabled = false;
            }
        }
        public static void SpamBlockRandom(string tooltip)
        {
            if (PhotonSystem.InRoom)
            {
                if (ZoneManagement.instance.IsZoneActive(GTZone.monkeBlocks))
                {
                    if (UserInput.GetMouseButton(0) || Controller.rightGrab)
                    {
                        RPCManager.PieceEvent(GetPieces().ToArray()[Random.Range(0, GetPieces().Length)].pieceType, RigUtils.MyOnlineRig.rightHandTransform.position, RigUtils.MyOnlineRig.rightHandTransform.rotation);
                        RPCProtection(true);
                    }
                }
                else
                {
                    NotifiLib.SendNotification(NotifUtils.Error() + "You Are Not In The Monke Blocks Map");
                    GetToolTip(tooltip).enabled = false;
                }
            }
            else
            {
                NotifiLib.SendNotification(NotifUtils.Error() + "You Are Not In A Room");
                GetToolTip(tooltip).enabled = false;
            }
        }
        public static void DestroyBlockGun(string tooltip)
        {
            if (PhotonSystem.InRoom)
            {
                if (ZoneManagement.instance.IsZoneActive(GTZone.monkeBlocks))
                {
                    if (CreateGun(out RaycastHit hit))
                    {
                        var piece = hit.collider.GetComponentInParent<BuilderPiece>();
                        if (piece)
                        {
                            BuilderTable.instance.RequestRecyclePiece(piece, true, 2);
                            RPCProtection(true);
                        }
                    }
                }
                else
                {
                    NotifiLib.SendNotification(NotifUtils.Error() + "You Are Not In The Monke Blocks Map");
                    GetToolTip(tooltip).enabled = false;
                }
            }
            else
            {
                NotifiLib.SendNotification(NotifUtils.Error() + "You Are Not In A Room");
                GetToolTip(tooltip).enabled = false;
            }
        }
        private static int gotPieceType = -566818631;
        public static void GetBlockIDGun(string tooltip)
        {
            if (PhotonSystem.InRoom)
            {
                if (ZoneManagement.instance.IsZoneActive(GTZone.monkeBlocks))
                {
                    if (CreateGun(out RaycastHit hit))
                    {
                        var piece = hit.collider.GetComponentInParent<BuilderPiece>();
                        if (piece) { gotPieceType = piece.pieceType; NotifiLib.SendNotification(NotifUtils.Success() + $"Copied Piece ID {gotPieceType} To Int32", 1f); }
                    }
                }
                else
                {
                    NotifiLib.SendNotification(NotifUtils.Error() + "You Are Not In The Monke Blocks Map");
                    GetToolTip(tooltip).enabled = false;
                }
            }
            else
            {
                NotifiLib.SendNotification(NotifUtils.Error() + "You Are Not In A Room");
                GetToolTip(tooltip).enabled = false;
            }
        }
        public static void GliderHalo()
        {
            var rad = 4f;
            var count = 8;
            for (int i = 0; i < count; i++)
            {
                float angle = (i * 360f / count) + Time.time * 90f; // did this so if your using the mystic gui free cam this will still work
                var pos = RigUtils.MyOnlineRig.rigidbody.useGravity ? RigUtils.MyPlayer.transform.position : RigUtils.MyOfflineRig.transform.position + new Vector3(
                    Mathf.Cos(angle * Mathf.Deg2Rad) * rad, 1.5f, Mathf.Sin(angle * Mathf.Deg2Rad) * rad);
                var glider = Gliders()[i];
                if (glider.GetView.Owner == RigUtils.MyRealtimePlayer)
                    glider.transform.position = pos;
                else
                    glider.OnHover(null, null);
            }
        }
        public static void TPToGlider(GliderHoldable glider)
        {
            RigUtils.MyPlayer.transform.position = glider.transform.position + new Vector3(0, .5f, 0);
        }
        public static void SpazGliders()
        {
            if (Controller.rightControllerIndexFloat.TriggerDown() || UserInput.GetMouseButton(0))
                foreach (var gliders in Gliders())
                    if (gliders.GetView.Owner == RigUtils.MyRealtimePlayer)
                    {
                        var gliderRB = gliders.GetComponent<Rigidbody>();
                        gliderRB?.AddForce(Random.insideUnitSphere * 8, ForceMode.Impulse);
                    }
                    else
                        gliders.OnHover(null, null);
        }
        public static void SpinGliders()
        {
            if (Controller.rightControllerIndexFloat.TriggerDown() || UserInput.GetMouseButton(0))
                foreach (var gliders in Gliders())
                    if (gliders.GetView.Owner == RigUtils.MyRealtimePlayer)
                    {
                        var gliderRB = gliders.GetComponent<Rigidbody>();
                        if (gliderRB != null)
                            gliderRB.angularVelocity = Vector3.up;
                    }
                    else
                        gliders.OnHover(null, null);
        }
        public static void GainVelocity()
        {
            if (Controller.rightControllerIndexFloat.TriggerDown() || UserInput.GetMouseButton(1))
            {
                RigUtils.MyPlayer.bodyCollider.attachedRigidbody.velocity += RigUtils.MyPlayer.headCollider.transform.forward * Time.deltaTime * Movement.flySpeed;
                var rigidbody = RigUtils.MyPlayer.bodyCollider.attachedRigidbody;
                rigidbody.AddForce(new Vector3(0f, 25f, 0f), ForceMode.Impulse);
            }
            if (Controller.leftControllerIndexFloat.TriggerDown() || UserInput.GetMouseButton(0))
            {
                RigUtils.MyPlayer.bodyCollider.attachedRigidbody.velocity -= RigUtils.MyPlayer.headCollider.transform.up * Time.deltaTime * Movement.flySpeed;
                var rigidbody = RigUtils.MyPlayer.bodyCollider.attachedRigidbody;
                rigidbody.AddForce(new Vector3(0f, -25f, 0f), ForceMode.Impulse);
            }
        }
        public static void GliderTracers()
        {
            foreach (var gliders in Gliders())
            {
                var line = new GameObject();
                var lineRend = line.AddComponent<LineRenderer>();
                lineRend.startColor = MenuSettings.FirstColor;
                lineRend.endColor = MenuSettings.FirstColor;
                lineRend.startWidth = 0.01f;
                lineRend.endWidth = 0.01f;
                lineRend.positionCount = 2;
                lineRend.useWorldSpace = true;
                lineRend.SetPosition(0, RigUtils.MyOnlineRig.rightHandTransform.position);
                lineRend.SetPosition(1, gliders.transform.position);
                lineRend.material.shader = Shader.Find("GUI/Text Shader");
                Destroy(lineRend, Time.deltaTime);
                Destroy(line, Time.deltaTime);
            }
        }
        public static void FlingGliderGun()
        {
            if (CreateGun())
                FlingGliders(pointer.transform.position, 2f, 2f);
        }
        public static void FlingGliders(Vector3 direction, float bringForce, float flingForce) // made by @drew008278 a while back and cleaned up by me
        {
            bool allAtTarget = true;
            foreach (var gliders in Gliders())
            if (gliders.GetView.Owner == RigUtils.MyRealtimePlayer)
            {
                var gliderRB = gliders.GetComponent<Rigidbody>();
                if (gliderRB != null)
                {
                    var directionToTarget = direction - gliderRB.position;
                    gliderRB.AddForce(directionToTarget.normalized * bringForce, ForceMode.Impulse);
                    if (directionToTarget.magnitude > 1f)
                        allAtTarget = false;
                }
            }
            else
                gliders.OnHover(null, null);
            if (allAtTarget)
            foreach (var gliders in Gliders())
            if (gliders.GetView.Owner == RigUtils.MyRealtimePlayer)
            {
                var gliderRB = gliders.GetComponent<Rigidbody>();
                if (gliderRB != null)
                {
                    gliderRB.AddForce(Vector3.up * flingForce, ForceMode.Impulse);
                }
            }
            else
                gliders.OnHover(null, null);
        }
        private static TrailRenderer gliderTrail = null;
        public static void GliderTrails()
        {
            foreach (var gliders in Gliders())
                if (gliderTrail == null)
                {
                    var gradient = new GradientColorKey[6];
                    gradient[0].color = Color.yellow;
                    gradient[0].time = 0;
                    gradient[1].color = Color.red;
                    gradient[1].time = .2f;
                    gradient[2].color = Color.magenta;
                    gradient[2].time = .4f;
                    gradient[3].color = Color.blue;
                    gradient[3].time = .6f;
                    gradient[4].color = Color.green;
                    gradient[4].time = .8f;
                    gradient[5].color = Color.yellow;
                    gradient[5].time = 1;
                    var chager = gliderTrail.AddComponent<ColorChanger>();
                    chager.colors = new Gradient { colorKeys = gradient };
                    chager.loop = true;
                    gliderTrail = gliders.AddComponent<TrailRenderer>();
                    gliderTrail.material = new Material(Shader.Find("Sprites/Default"));
                    gliderTrail.time = 1f;
                    gliderTrail.startWidth = 1;
                    gliderTrail.endWidth = 0;
                    gliderTrail.minVertexDistance = 6f;
                }
        }
        public static void GliderTrailsOff()
        {
            if (gliderTrail != null)
            {
                Destroy(gliderTrail);
                gliderTrail = null;
            }
        }
        public static void GliderGun()
        {
            if (CreateGun())
                foreach (var gliders in Gliders())
                    if (gliders.GetView.Owner == RigUtils.MyRealtimePlayer)
                    {
                        gliders.transform.position = pointer.transform.position;
                        gliders.transform.rotation = pointer.transform.rotation;
                    }
                    else
                        gliders.OnHover(null, null);
        }
        public static void RespawnGliders()
        {
            foreach (var gliders in Gliders())
                if (gliders.GetView.Owner == RigUtils.MyRealtimePlayer)
                {
                    gliders.Respawn();
                    RecreateMenu();
                }
                else
                    gliders.OnHover(null, null);
        }
        public static void GetGliders()
        {
            if (Controller.rightGrab || UserInput.GetMouseButton(0))
                foreach (var gliders in Gliders())
                    if (gliders.GetView.Owner == RigUtils.MyRealtimePlayer)
                    {
                        gliders.transform.position = RigUtils.MyPlayer.rightControllerTransform.position;
                        gliders.transform.rotation = RigUtils.MyPlayer.rightControllerTransform.rotation;
                    }
                    else
                        gliders.OnHover(null, null);
        }
        private static GameObject bluePortal = null;
        private static GameObject orangePortal = null;
        public static void PortalGunOff()
        {
            if (bluePortal != null && orangePortal != null)
            {
                var rig = Vector3.Distance(bluePortal.transform.position, RigUtils.MyPlayer.transform.position);
                if (rig < .35f)
                {
                    RigUtils.MyPlayer.transform.position = bluePortal.transform.position;
                }
                var rig2 = Vector3.Distance(orangePortal.transform.position, RigUtils.MyPlayer.transform.position);
                if (rig2 < .35f)
                {
                    RigUtils.MyPlayer.transform.position = orangePortal.transform.position;
                }
            }
        }
        public static void PortalGun()
        {
            if (Controller.leftGrab)
            {
                if (CreateGun(Controller.rightControllerPrimaryButton))
                {
                    if (bluePortal == null)
                    {
                        bluePortal = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                        bluePortal.name = "BALLS";
                        bluePortal.transform.localScale = new Vector3(.3f, .3f, .3f);
                        bluePortal.GetComponent<Renderer>().material.color = Settings.colors[2];
                        Destroy(bluePortal.GetComponent<Collider>());
                    }
                    bluePortal.transform.position = pointer.transform.position;
                }
                if (Controller.rightControllerSecondaryButton)
                {
                    if (orangePortal == null)
                    {
                        orangePortal = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                        orangePortal.name = "BALLS";
                        orangePortal.transform.localScale = new Vector3(.3f, .3f, .3f);
                        orangePortal.GetComponent<Renderer>().material.color = Settings.colors[4];
                        Destroy(orangePortal.GetComponent<Collider>());
                    }
                    orangePortal.transform.position = pointer.transform.position;
                }
            }
        }
        public static void BugGun()
        {
            if (CreateGun())
            {
                if (DougBug.GetComponent<ThrowableBug>().IsMyItem())
                    DougBug.transform.position = pointer.transform.position;
                else
                {
                    GetIndex("Get Animal Ownership").enabled = true;
                    RecreateMenu();
                }
            }
        }
        public static void BatGun()
        {
            if (CreateGun())
            {
                if (BatteryAcid.GetComponent<ThrowableBug>().IsMyItem())
                    BatteryAcid.transform.position = pointer.transform.position;
                else
                {
                    GetIndex("Get Animal Ownership").enabled = true;
                    RecreateMenu();
                }
            }
        }
        public static void BugSpaz()
        {
            if (DougBug.GetComponent<ThrowableBug>().IsMyItem())
                DougBug.transform.rotation = Quaternion.Euler(new Vector3(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f)));
            else
            {
                GetIndex("Get Animal Ownership").enabled = true;
                RecreateMenu();
            }
        }
        public static void BatSpaz()
        {
            if (BatteryAcid.GetComponent<ThrowableBug>().IsMyItem())
                BatteryAcid.transform.rotation = Quaternion.Euler(new Vector3(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f)));
            else
            {
                GetIndex("Get Animal Ownership").enabled = true;
                RecreateMenu();
            }
        }
        public static void BugOrbit()
        {
            if (DougBug.GetComponent<ThrowableBug>().IsMyItem())
            {
                var orbit = RigUtils.MyPlayer.transform.position + new Vector3(Mathf.Cos(Time.frameCount / 30f), 0f, Mathf.Sin(Time.frameCount / 30f));
                DougBug.transform.position = orbit;
            }
            else
            {
                GetIndex("Get Animal Ownership").enabled = true;
                RecreateMenu();
            }
        }
        public static void BatOrbit()
        {
            if (BatteryAcid.GetComponent<ThrowableBug>().IsMyItem())
            {
                var orbit = RigUtils.MyPlayer.transform.position + new Vector3(Mathf.Cos(Time.frameCount / 30f), 0f, Mathf.Sin(Time.frameCount / 30f));
                BatteryAcid.transform.position = orbit;
            }
            else
            {
                GetIndex("Get Animal Ownership").enabled = true;
                RecreateMenu();
            }
        }
        public static void GrabBug()
        {
            if (DougBug.GetComponent<ThrowableBug>().IsMyItem())
            {
                if (Controller.rightGrab || UserInput.GetMouseButton(0))
                    DougBug.transform.position = RigUtils.MyOnlineRig.rightHandTransform.position + new Vector3(0, .2f, 0);
            }
            else
            {
                GetIndex("Get Animal Ownership").enabled = true;
                RecreateMenu();
            }
        }
        public static void GrabBat()
        {
            if (BatteryAcid.GetComponent<ThrowableBug>().IsMyItem())
            {
                if (Controller.rightGrab || UserInput.GetMouseButton(0))
                    BatteryAcid.transform.position = RigUtils.MyOnlineRig.rightHandTransform.position + new Vector3(0, .2f, 0);
            }
            else
            {
                GetIndex("Get Animal Ownership").enabled = true;
                RecreateMenu();
            }
        }
        private static GameObject DougBug { get { return GameObject.Find("Floating Bug Holdable"); } }
        private static GameObject BatteryAcid { get { return GameObject.Find("Cave Bat Holdable"); } }
        public static void GetAnimalOnwership()
        {
            DougBug.GetComponent<ThrowableBug>().ownerRig = RigUtils.MyOfflineRig;
            DougBug.GetComponent<ThrowableBug>().allowWorldSharableInstance = true;
            DougBug.GetComponent<ThrowableBug>().WorldShareableRequestOwnership();
            BatteryAcid.GetComponent<ThrowableBug>().ownerRig = RigUtils.MyOfflineRig;
            BatteryAcid.GetComponent<ThrowableBug>().allowWorldSharableInstance = true;
            BatteryAcid.GetComponent<ThrowableBug>().WorldShareableRequestOwnership();
        }
        public static void SetOfflineColor(Color color) => RigUtils.MyOfflineRig.mainSkin.material.color = color;
        public static void PlatformGun()
        {
            if (CreateGun())
            {
                var plat = GameObject.CreatePrimitive(PrimitiveType.Cube);
                plat.name = "Plats";
                plat.transform.localScale = Movement.scale;
                plat.transform.position = pointer.transform.position;
                var content = new object[] 
                { 
                    plat.transform.position, plat.transform.rotation, Physics.gravity, 
                    Movement.platformPhysics, Movement.platfromSingleColor, Movement.platColorKeys, Movement.PlatColor
                };
                LegacySendEvent(110, content, new RaiseEventOptions { Receivers = ReceiverGroup.Others }, true);
                if (Movement.platformPhysics) { var platRB = plat.AddComponent(typeof(Rigidbody)) as Rigidbody; platRB.velocity = Physics.gravity; plat.layer = 8; }
                if (!Movement.platfromSingleColor)
                {
                    var colorChanger = plat.AddComponent<ColorChanger>();
                    colorChanger.colors = new Gradient { colorKeys = Movement.platColorKeys };
                    colorChanger.loop = true;
                } else plat.GetComponent<Renderer>().material.color = Movement.PlatColor;
            }
        }
        private static SnowballThrowable[] throwables = null;
        public static void RGBSnowballs(bool enable)
        {
            foreach (var throwable in GetThrowables())
                if (throwable != null)
                    throwable.randomizeColor = enable;
        }
        private static SnowballThrowable[] GetThrowables()
        {
            throwables ??= Resources.FindObjectsOfTypeAll<SnowballThrowable>();
            return throwables;
        }
        public static void GiveWaterBender(string btnname) // not working
        {
            if (PhotonSystem.InRoom)
            {
                if (CreateGun(out VRRig rig))
                {
                    RigUtils.MyOfflineRig.enabled = false;
                    RigUtils.MyOfflineRig.transform.position = -rig.transform.forward;
                    RigUtils.MyNetworkView.transform.position = -rig.transform.forward;
                    RPCManager.WaterEvent(RpcTarget.All, rig.rightHandTransform.position, rig.rightHandTransform.rotation);
                    RPCManager.WaterEvent(RpcTarget.All, rig.leftHandTransform.position, rig.leftHandTransform.rotation);
                    RPCProtection();
                } else RigUtils.MyOfflineRig.enabled = true;
            } else { GetIndex(btnname).enabled = false; NotifiLib.SendNotification(NotifUtils.Error() + "You Are Not Connected To A Lobby"); }
        }
        public static void WaterRight(string btnname)
        {
            if (PhotonSystem.InRoom)
            {
                if (Controller.rightGrab || UserInput.GetMouseButton(1))
                {
                    RPCManager.WaterEvent(RpcTarget.All, RigUtils.MyOnlineRig.rightHandTransform.position, RigUtils.MyOnlineRig.rightHandTransform.rotation);
                    RPCProtection();
                }
            } else { GetIndex(btnname).enabled = false; NotifiLib.SendNotification(NotifUtils.Error() + "You Are Not Connected To A Lobby"); }
        }
        public static void WaterLeft(string btnname)
        {
            if (PhotonSystem.InRoom)
            {
                if (Controller.leftGrab || UserInput.GetMouseButton(0))
                {
                    RPCManager.WaterEvent(RpcTarget.All, RigUtils.MyOnlineRig.leftHandTransform.position, RigUtils.MyOnlineRig.leftHandTransform.rotation);
                    RPCProtection();
                }
            } else { GetIndex(btnname).enabled = false; NotifiLib.SendNotification(NotifUtils.Error() + "You Are Not Connected To A Lobby"); }
        }
        public static void WaterBender(string btnname) { WaterRight(btnname); WaterLeft(btnname); }
        public static void WaterGun(string btnname)
        {
            if (PhotonSystem.InRoom)
            {
                if (CreateGun())
                {
                    RigUtils.MyOfflineRig.enabled = false;
                    RigUtils.MyOfflineRig.transform.position = pointer.transform.position - new Vector3(0, 2, 0);
                    RigUtils.MyNetworkView.transform.position = pointer.transform.position - new Vector3(0, 2, 0);
                    RPCManager.WaterEvent(RpcTarget.All, pointer.transform.position, pointer.transform.rotation);
                    RPCProtection();
                } else RigUtils.MyOfflineRig.enabled = true;
            } else { GetIndex(btnname).enabled = false; NotifiLib.SendNotification(NotifUtils.Error() + "You Are Not Connected To A Lobby"); }
        }
        public static void PlayTagSound(int index, string btnname)
        {
            if (Controller.rightControllerIndexFloat.TriggerDown() || UserInput.GetMouseButton(0))
            {
                RPCManager.TagSoundEvent(NetEventOptions.RecieverTarget.all, new object[]
                {
                    index,
                    float.MaxValue,
                    false
                });
                RPCProtection();
            }
        }
        public static void PlaySound(int index, string btnname)
        {
            if (Controller.rightControllerIndexFloat.TriggerDown() || UserInput.GetMouseButton(0))
            {
                RPCManager.SoundEvent(RpcTarget.All, index, true, float.MaxValue);
                RPCProtection();
            }
        }
        public static void BallsGun()
        {
            if (CreateGun())
            {
                var Balls = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                Balls.name = "BALLS";
                Balls.layer = 8;
                Balls.GetComponent<Renderer>().material.color = RGBColor();
                Balls.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                Balls.transform.position = pointer.transform.position;
                var eventContent = new object[] 
                { 
                    Balls.transform.position, Balls.transform.rotation, Physics.gravity, 
                    Balls.GetComponent<Renderer>().material.color
                };
                LegacySendEvent(100, eventContent, new RaiseEventOptions { Receivers = ReceiverGroup.Others }, true);
                var BallsRB = Balls.AddComponent(typeof(Rigidbody)) as Rigidbody;
                BallsRB.velocity = Physics.gravity;
                var trail = Balls.AddComponent<TrailRenderer>();
                trail.material.shader = Shader.Find("Sprites/Default");
                trail.time = 1f;
                trail.startWidth = 0.1f;
                trail.endWidth = 0f;
                trail.minVertexDistance = 1f;
                trail.material.color = Balls.GetComponent<Renderer>().material.color;
            }
        }
        public static GameObject EnderBall = null;
        public static GTColor.HSVRanges ColorRanges = new GTColor.HSVRanges(0f, 1f, .7f, 1f, 1f, 1f);
        public static void EnderPearl()
        {
            if (Controller.rightGrab || UserInput.GetMouseButton(0))
            {
                if (EnderBall == null)
                {
                    var col = GTColor.RandomHSV(ColorRanges);
                    EnderBall = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    EnderBall.name = "BALLS";
                    EnderBall.transform.localScale = new Vector3(.1f, .1f, .1f);
                    EnderBall.layer = 8;
                    EnderBall.GetComponent<SphereCollider>().isTrigger = true;
                    EnderBall.GetComponent<Renderer>().material.color = col;
                    var trail = EnderBall.AddComponent<TrailRenderer>();
                    trail.material = new Material(Shader.Find("Sprites/Default"));
                    trail.time = 1f;
                    trail.startWidth = .1f;
                    trail.endWidth = 0f;
                    trail.minVertexDistance = 1f;
                    trail.startColor = col;
                    trail.endColor = Color.clear;
                }
                else 
                {
                    EnderBall.transform.position = RigUtils.MyPlayer.rightControllerTransform.position;
                    EnderBall.transform.rotation = RigUtils.MyPlayer.rightControllerTransform.rotation;
                }
            }
            else if (EnderBall != null)
            {
                var ballRB = EnderBall.AddComponent(typeof(Rigidbody)) as Rigidbody;
                    ballRB.velocity = RigUtils.MyPlayer.rightHandCenterVelocityTracker.GetAverageVelocity(true, 0f);
                    EnderBall.AddComponent<EnderTrigger>();
            }
        }
        public class EnderTrigger : MonoBehaviour
        {
            void OnTriggerEnter()
            {
                RigUtils.MyOnlineRig.transform.position = EnderBall.transform.position;
                RigUtils.MyOnlineRig.GetComponent<Rigidbody>().velocity = Vector3.zero;
                RigUtils.MyOfflineRig.PlayHandTapLocal(84, true, 999);
                Destroy(EnderBall);
                EnderBall = null;
            }
        }
    }
}