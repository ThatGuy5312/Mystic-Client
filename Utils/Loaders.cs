using MysticClient.Menu;
using MysticClient.Notifications;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Video;

namespace MysticClient.Utils
{
    public class Loaders : MonoBehaviour
    {
        public static float AudioVolume = .3f;
        public static bool loopAudio = false;
        public static AudioSource Object = null;
        public static AudioSource MCObject = null;
        public static Texture2D LoadTexture(string path)
        {
            var texture = new Texture2D(2, 2);
            using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(path);
            if (stream != null)
            {
                var data = new byte[stream.Length];
                stream.Read(data, 0, data.Length);
                texture.LoadImage(data);
                return texture;
            }
            return null;
        }
        public static GameObject LoadGameObject(string path, string name)
        {
            using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(path); if (stream == null) return null;
            return Instantiate(AssetBundle.LoadFromStream(stream).LoadAsset<GameObject>(name));
        }
        public static AssetBundle LoadAsset(string path)
        {
            using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(path); if (stream == null) return null;
            return AssetBundle.LoadFromStream(stream);
        }
        public static AudioClip GetAudioFromURL(string MP3Link)
        {
            using var webRequest = UnityWebRequestMultimedia.GetAudioClip(MP3Link, AudioType.MPEG);
            var operation = webRequest.SendWebRequest();
            while (!operation.isDone) { }
            if (webRequest.result != UnityWebRequest.Result.Success) { Debug.LogError($"Failed to download audio clip: {webRequest.error}"); }
            return DownloadHandlerAudioClip.GetContent(webRequest);
        }
        public static Texture2D LoadImageFromURL(string url)
        {
            using var webRequest = UnityWebRequestTexture.GetTexture(url);
            var operation = webRequest.SendWebRequest();
            while (!operation.isDone) { }
            if (webRequest.result != UnityWebRequest.Result.Success) { Debug.LogError($"Failed to download texture2d: {webRequest.error}"); }
            return DownloadHandlerTexture.GetContent(webRequest);
        }
        public static void PlayAudio(AudioClip clip)
        {
            if (Object == null) { Object = new GameObject().AddComponent<AudioSource>(); }
            Object.transform.position = RigUtils.MyPlayer.transform.position;
            Object.loop = loopAudio;
            Object.volume = AudioVolume;
            Object.PlayOneShot(clip);
        }
        public static void PlayAudio(string MP3Link)
        {
            if (Object == null) { Object = new GameObject().AddComponent<AudioSource>(); }
            Object.transform.position = RigUtils.MyPlayer.transform.position;
            Object.clip = GetAudioFromURL(MP3Link);
            Object.loop = loopAudio;
            Object.volume = AudioVolume;
            Object.Play();
        }
        public static void PlayMCAudio(AudioClip clip)
        {
            if (MCObject == null) { MCObject = new GameObject().AddComponent<AudioSource>(); }
            MCObject.transform.position = RigUtils.MyPlayer.transform.position;
            MCObject.loop = loopAudio;
            MCObject.volume = AudioVolume;
            MCObject.PlayOneShot(clip);
        }

        // this is not used for anything harmful it's used to download extentions for the menu like the gui
        public static void InstallPlugin(string URLPath, string FileName, string tooltip)
        {
            var client = new WebClient();
            if (!File.Exists($"BepInEx\\plugins\\{FileName}"))
            {
                client.DownloadFile(URLPath, FileName);
                File.Move(FileName, "BepInEx\\plugins");
                Main.GetToolTip(tooltip).enabled = false;
            } 
            else NotifiLib.SendNotification(NotifUtils.Warning() + $"File {FileName} already exist. try restarting your game to fix if the plugin if not working");
        }
    }
}