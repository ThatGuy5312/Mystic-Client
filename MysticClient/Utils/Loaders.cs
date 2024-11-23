using MysticClient.Menu;
using MysticClient.Notifications;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace MysticClient.Utils
{
    public class Loaders : MonoBehaviour
    {
        public static float AudioVolume = 0.2f;
        public static bool loopAudio = false;
        private static GameObject Object = null;
        private static GameObject GetObject = null;
        public static Texture2D LoadTexture(string path)
        {
            var texture = new Texture2D(2, 2);
            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(path);
            if (stream != null)
            {
                var data = new byte[stream.Length];
                stream.Read(data, 0, data.Length);
                texture.LoadImage(data);
            } else Debug.LogError($"Failed to load texure from resource {path}");
            return texture;
        }
        public static AssetBundle LoadAsset(string path)
        {
            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(path);
            var bundle = AssetBundle.LoadFromStream(stream);
            stream.Close();
            return bundle;
        }
        public static AudioClip GetAudioFromURL(string MP3Link)
        {
            using (var webRequest = UnityWebRequestMultimedia.GetAudioClip(MP3Link, AudioType.MPEG))
            {
                var operation = webRequest.SendWebRequest();
                while (!operation.isDone) { }
                if (webRequest.result != UnityWebRequest.Result.Success) { Debug.LogError($"Failed to download audio sclip: {webRequest.error}"); }
                return DownloadHandlerAudioClip.GetContent(webRequest);
            }
        }
        public static void PlayAudio(string MP3Link) // made by @anthonyzfrfr cleaned up by me
        {
            using (var webRequest = UnityWebRequestMultimedia.GetAudioClip(MP3Link, AudioType.MPEG))
            {
                webRequest.SendWebRequest();
                var Audio = DownloadHandlerAudioClip.GetContent(webRequest);
                if (Object == null) { Object = new GameObject("Object"); }
                Object.transform.position = RigUtils.MyOnlineRig.transform.position;
                var audioSource = Object.AddComponent<AudioSource>();
                audioSource.clip = Audio;
                audioSource.loop = loopAudio;
                audioSource.volume = AudioVolume;
                audioSource.Play();
            }
        }
        public static Texture2D LoadImage(string URLPath, string FileName)
        {
            var texture = new Texture2D(2, 2);
            var client = new WebClient();
            client.DownloadFile(URLPath, FileName);
            var textureByte = File.ReadAllBytes(FileName);
            texture.LoadImage(textureByte);
            return texture;
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
            else 
            {
                NotifiLib.SendNotification(NotifUtils.Warning() + $"File {FileName} already exist. try restarting your game to fix if the plugin if not working");
            }
        }
    }
}