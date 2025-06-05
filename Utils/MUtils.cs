using GorillaExtensions;
using MysticClient.Classes;
using MysticClient.Menu;
using MysticClient.Mods;
using MysticClient.Notifications;
using OculusSampleFramework;
using Photon.Realtime;
using Photon.Voice.PUN.UtilityScripts;
using Steamworks;
using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using Viveport;
using Random = UnityEngine.Random;

namespace MysticClient.Utils
{
    public static class MExt
    {
        // dev stuff

        public static void ToSlider(this float value, float maxValue) => value = GUILayout.HorizontalSlider(value, 0, maxValue);
        public static void ToTextbox(this ref float value)
        {
            var text = GUILayout.TextArea(value.ToString());
            if (float.TryParse(text, out var newValue)) value = newValue;
        }
        public static void ToTextbox(this ref int value)
        {
            var text = GUILayout.TextArea(value.ToString());
            if (int.TryParse(text, out var newValue)) value = newValue;
        }
        public static void ToTextbox(this ref Vector3 value)
        {
            GUILayout.BeginHorizontal();
            var x = GUILayout.TextArea(value.x.ToString());
            var y = GUILayout.TextArea(value.y.ToString());
            var z = GUILayout.TextArea(value.z.ToString());
            if (float.TryParse(x, out var newValueX)) value.x = newValueX;
            if (float.TryParse(y, out var newValueY)) value.y = newValueY;
            if (float.TryParse(z, out var newValueZ)) value.z = newValueZ;
            GUILayout.EndHorizontal();
        }

        public static void ToTextbox(this ref Quaternion value)
        {
            GUILayout.BeginHorizontal();
            var x = GUILayout.TextArea(value.x.ToString());
            var y = GUILayout.TextArea(value.y.ToString());
            var z = GUILayout.TextArea(value.z.ToString());
            var w = GUILayout.TextArea(value.w.ToString());
            if (float.TryParse(x, out var newValueX)) value.x = newValueX;
            if (float.TryParse(y, out var newValueY)) value.y = newValueY;
            if (float.TryParse(z, out var newValueZ)) value.z = newValueZ;
            if (float.TryParse(w, out var newValueW)) value.w = newValueW;
            GUILayout.EndHorizontal();
        }

        public static void ToTextbox(this ref Vector2 value)
        {
            GUILayout.BeginHorizontal();
            var x = GUILayout.TextArea(value.x.ToString());
            var y = GUILayout.TextArea(value.y.ToString());
            if (float.TryParse(x, out var newValueX)) value.x = newValueX;
            if (float.TryParse(y, out var newValueY)) value.y = newValueY;
            GUILayout.EndHorizontal();
        }

        public static void ToTextbox(this ref Rect value)
        {
            GUILayout.BeginHorizontal();
            var x = GUILayout.TextArea(value.x.ToString());
            var y = GUILayout.TextArea(value.y.ToString());
            var width = GUILayout.TextArea(value.width.ToString());
            var height = GUILayout.TextArea(value.height.ToString());
            if (float.TryParse(x, out var newValueX)) value.x = newValueX;
            if (float.TryParse(y, out var newValueY)) value.y = newValueY;
            if (float.TryParse(width, out var newWidth)) value.width = newWidth;
            if (float.TryParse(height, out var newHeight)) value.height = newHeight;
            GUILayout.EndHorizontal();
        }

        // dev stuff
        public static bool TriggerDown(this float triggerFloat) => triggerFloat > .5f;
        public static GameObject InstantiateSelf(this GameObject instantiatingObject) => MUtils.Instantiate(instantiatingObject);
        public static NetPlayer GetNetPlayer(this Photon.Realtime.Player player) => RigUtils.GetNetFromPlayer(player);
        public static void Destroy(this UnityEngine.Object obj) => UnityEngine.Object.Destroy(obj);
        public static void Destroy(this UnityEngine.Object obj, float t) => UnityEngine.Object.Destroy(obj, t);
        public static void Destroy(this GameObject obj, Type type) => UnityEngine.Object.Destroy(obj.GetComponent(type));
        public static T Destroy<T>(this GameObject obj) where T : Component
        {
            T comp = obj.GetComponent<T>();
            if (comp != null) UnityEngine.Object.Destroy(comp);
            return comp;
        }
        public static void DestroyImmediate(this GameObject obj) => UnityEngine.Object.DestroyImmediate(obj);
        public static T DestroyImmediate<T>(this GameObject obj) where T : Component
        {
            T comp = obj.GetComponent<T>();
            if (comp != null) UnityEngine.Object.DestroyImmediate(comp);
            return comp;
        }
        public static Renderer GetRenderer(this GameObject obj) => obj.GetComponent<Renderer>();
        public static Vector3 Position(this GameObject obj) => obj.transform.position;
        public static Vector3 Position(this UnityEngine.Object obj) => obj.Position();
        public static void SetPosition(this GameObject obj, Vector3 nextVector) => obj.transform.position = nextVector;
        public static void SetPosition(this Transform transform, Vector3 nextVector) => transform.position = nextVector;
        public static Quaternion ToLookQuat(this Vector3 origin, Vector3 target)
        {
            var direction = target - origin;
            if (direction.sqrMagnitude == 0)
                return Quaternion.identity;
            return Quaternion.LookRotation(direction.normalized, Vector3.up);
        }
        public static void SetWidth(this LineRenderer line, float width) { line.startWidth = width; line.endWidth = width; }
        public static Color GetColorConstant(this GameObject obj)
        {
            if (obj.GetComponent<ColorChanger>())
                return obj.GetComponent<ColorChanger>().color;
            else return obj.GetComponent<Renderer>().material.color;
        }
        public static bool HasComponent(this GameObject obj, Type type) => obj.GetComponent(type) != null; // these are both useless but i like the name
        public static bool HasComponent<T>(this GameObject obj) where T : Component => obj.GetComponent<T>();
        public static void PlaySplashEffectLocal(this VRRig rig, Vector3 pos, Quaternion rot, float splashScale, float boundingRadius, bool bigSpalsh, bool enteringWater)
        {
            boundingRadius = Mathf.Clamp(boundingRadius, .0001f, .5f);
            ObjectPools.instance.Instantiate(RigUtils.MyPlayer.waterParams.rippleEffect, pos, rot, RigUtils.MyPlayer.waterParams.rippleEffectScale * boundingRadius * 2f).GetComponent<WaterRippleEffect>().PlayEffect();
            splashScale = Mathf.Clamp(splashScale, 1E-05f, 1f);
            ObjectPools.instance.Instantiate(RigUtils.MyPlayer.waterParams.splashEffect, pos, rot, splashScale).GetComponent<WaterSplashEffect>().PlayEffect(bigSpalsh, enteringWater, splashScale);
        }
        public static Vector3 GetRotationSpeedVelocity(this GameObject obj, float radius)
        {
            var angularVelocity = obj.GetComponent<Rigidbody>().angularVelocity;
            return Vector3.Cross(angularVelocity, obj.transform.right * radius);
        }
        public static void ChangeShader(this GameObject obj, Shader shader) => obj.GetComponent<Renderer>().material.shader = shader;
        public static void ChangeTexture(this GameObject obj, Texture2D texture) => obj.GetComponent<Renderer>().material.mainTexture = texture;
        public static void ChangeColor(this GameObject obj, Color color) => obj.GetComponent<Renderer>().material.color = color;
        public static void ChangeMaterial(this GameObject obj, Material material) => obj.GetComponent<Renderer>().material = material;
        public static string GetPath(this Transform transform)
        {
            var path = "";
            while (transform.parent != null)
            {
                transform = transform.parent;
                path = string.IsNullOrEmpty(path) ? transform.name : transform.name + "/" + path;
            }
            return path;
        }
        private static Vector3 GetPreviousPosition(GorillaVelocityEstimator velocityEstimator) => velocityEstimator.gameObject.transform.position - velocityEstimator.gameObject.transform.forward * .01f;
        public static Vector3 Velocity(this GorillaVelocityEstimator velocityEstimator) => (velocityEstimator.gameObject.transform.position - GetPreviousPosition(velocityEstimator)) / Time.deltaTime;
        public static bool IsNull(this UnityEngine.Object obj) => obj == null;
        public static string ToHex(this Color color) => $"#{(int)(color.r * 255):X2}{(int)(color.g * 255):X2}{(int)(color.b * 255):X2}";
        public static T AddComponentOnce<T>(this GameObject obj) where T : Component
        {
            T comp = obj.GetComponent<T>();
            if (comp != null) comp = obj.AddComponent<T>();
            return comp;
        }
        public static TrailRenderer AttachTrail(this GameObject obj, float startWidth, Material material)
        {
            var trail = obj.GetOrAddComponent<TrailRenderer>();
            trail.material = material;
            trail.time = 1f;
            trail.startWidth = startWidth;
            trail.endWidth = 0;
            trail.minVertexDistance = .1f;
            return trail;
        }
    }
    public class MUtils : MonoBehaviour
    {
        public static MUtils instance = null;
        void Awake() => instance = this;
        public static void RunCoroutine(IEnumerator enumerator) => instance.StartCoroutine(enumerator);
        public static void EndCoroutine(IEnumerator enumerator) => instance.StopCoroutine(enumerator);
        public static Coroutine _RunCoroutine(IEnumerator enumerator) => instance.StartCoroutine(enumerator);
        public static void _EndCoroutine(Coroutine coroutine) => instance.StopCoroutine(coroutine);
        public GameObject Instantiate(int hash, bool activeSelf) { var obj = ObjectPools.instance.Instantiate(hash); obj.SetActive(activeSelf); return obj; }
        public static int GetLayer(UnityLayer layer) => LayerMask.NameToLayer(layer.ToString());
        public static int GetLayer(UnityLayer[] layers)
        {
            for (int i = 0; i < layers.Length; i++)
                return LayerMask.GetMask(string.Concat(new string[] { layers[i].ToString(), "\n" }));
            return 0;
        }
        public static GorillaVelocityEstimator VelocityHandEstimator(string hand) => GameObject.Find($"Player Objects/Player VR Controller/GorillaPlayer/TurnParent/{hand}").GetOrAddComponent<GorillaVelocityEstimator>();
        public static ButtonInfo[] _Buttons
        {
            get
            {
                foreach (var btnss in Buttons.buttons)
                    foreach (var btns in btnss)
                        return btns;
                return null;
            }
        }
        public static Type GetClass(string className) => AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.GetName().Name == "Assembly-CSharp")?.GetType(className); 
        public static int GetRandomColorID
        {
            get
            {
                if (Main.GetEnabled("Use System Colors"))
                    return Random.Range(0, Settings.scolor.Length);
                else return Random.Range(0, Settings.colors.Length);
            }
        }
        public static string GetMCTextureFromID(int ID)
        {
            if (ID == 0) return "Grass";
            else if (ID == 1) return "Dirt";
            else if (ID == 2) return "Wood";
            else if (ID == 3) return "Leaf";
            else if (ID == 4) return "Plank";
            else if (ID == 5) return "Stone";
            else if (ID == 6) return "Cobblestone";
            else if (ID == 7) return "HayBale";
            else if (ID == 8) return "Glass";
            else if (ID == 9) return "Obsidian";
            else if (ID == 10) return "Water";
            else if (ID == 11) return "TrapDoor";
            else return "stop that i dont like it";
        }

        public static void RenameFile(string oldFile, string newFile)
        {
            if (File.Exists(oldFile))
            {
                File.Move(oldFile, newFile);
                Debug.Log($"Renamed file {oldFile} to {newFile}");
            } else Debug.Log($"File {oldFile} was not found");
        }

        private static bool once;
        public static bool CheckOnce(bool once)
        {
            if (once && MUtils.once)
            {
                MUtils.once = false;
                return true;
            }  
            else if (!once) MUtils.once = true;
            return false;
        }

        public static Texture2D CreateRounded(Color color, int width, int height, int radius)
        {
            var texture = new Texture2D(width, height);
            var pixels = new Color[width * height];
            for (int y = 0; y < texture.height; y++)
                for (int x = 0; x < texture.width; x++)
                {
                    var corner = (x < radius && y < radius && (x - radius) * (x - radius) + (y - radius) * (y - radius) > radius * radius) ||
                        (x > width - radius - 1 && y < radius && (x - (width - radius - 1)) * (x - (width - radius - 1)) + (y - radius) * (y - radius) > radius * radius) ||
                        (x < radius && y > height - radius - 1 && (x - radius) * (x - radius) + (y - (height - radius - 1)) * (y - (height - radius - 1)) > radius * radius) ||
                        (x > width - radius - 1 && y > height - radius - 1 && (x - (width - radius - 1)) * (x - (width - radius - 1)) + (y - (height - radius - 1)) * (y - (height - radius - 1)) > radius * radius);
                    pixels[x + y * width] = corner ? Color.clear : color;
                }
            texture.SetPixels(pixels);
            texture.Apply();
            return texture;
        }
    }
}