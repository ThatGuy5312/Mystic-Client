using MysticClient.Classes;
using MysticClient.Menu;
using Photon.Realtime;
using System;
using System.Collections;
using UnityEngine;
using Viveport;

namespace MysticClient.Utils
{
    public static class MExt
    {
        public static bool TriggerDown(this float triggerFloat) => triggerFloat > .5f;
        public static GameObject InstantiateSelf(this GameObject instantiatingObject) => MUtils.Instantiate(instantiatingObject);
        public static NetPlayer GetNetPlayer(this Player player) => RigUtils.GetNetFromPlayer(player);
        public static void Destroy(this UnityEngine.Object obj) => UnityEngine.Object.Destroy(obj);
        public static void Destroy(this UnityEngine.Object obj, float t) => UnityEngine.Object.Destroy(obj, t);
        public static void Destroy(this GameObject obj, Type type) => UnityEngine.Object.Destroy(obj.GetComponent(type));
        public static T Destroy<T>(this GameObject obj) where T : Component
        {
            T comp = obj.GetComponent<T>();
            if (comp != null) UnityEngine.Object.Destroy(comp);
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
    }
}