using MysticClient.Menu;
using MysticClient.Notifications;
using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Video;

namespace MysticClient.Utils
{
    public class Loaders : MonoBehaviour
    {
        public static AudioSource Object = null;
        public static AudioSource MCObject = null;
        public static bool MCObjectLoop = false;
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

        public static IEnumerator GetAudioFromURL(string MP3Link, Action<AudioClip> onComplete)
        {
            using var webRequest = UnityWebRequestMultimedia.GetAudioClip(MP3Link, AudioType.MPEG);
            yield return webRequest.SendWebRequest();
            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                var clip = DownloadHandlerAudioClip.GetContent(webRequest);
                onComplete?.Invoke(clip);
            }
            else
            {
                Debug.LogError($"Failed to download audio clip: {webRequest.error}");
                onComplete?.Invoke(null);
            }
        }

        public static string GetTextFromURL(string link)
        {
            using var webRequest = UnityWebRequest.Get(link);
            var operation = webRequest.SendWebRequest();
            while (!operation.isDone) { }
            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Error with getting pastebin text: {webRequest.error}");
                return null;
            }
            else 
            { 
                Debug.Log($"Grabbed PasteBin Text: {webRequest.downloadHandler.text}"); 
                return webRequest.downloadHandler.text; 
            }
        }

        public static AudioClip LoadAudioClip(string linkandpath)
        {
            AudioClip clip = null;
            if (!File.Exists($"MysticClient/MenuSounds/{linkandpath.Split(';')[1]}"))
                DownloadAudioClip(linkandpath.Split(';')[0], linkandpath.Split(';')[1]);
            else clip = LoadAudioClipFromFile(linkandpath.Split(';')[1]);
            return clip;
        }
        public static AudioClip GetAudioClip(string objectName) => Main.BundleObjects[1].GetNamedChild(objectName).GetComponent<AudioSource>().clip;
        public static void DownloadAudioClip(string link, string filename)
        {
            using var webRequest = UnityWebRequest.Get(link);
            var operation = webRequest.SendWebRequest();
            while (!operation.isDone) { }
            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                var path = "MysticClient\\MenuSounds";
                if (!File.Exists(path))
                    Directory.CreateDirectory(path);
                File.WriteAllBytes(Path.Combine(path, filename), webRequest.downloadHandler.data);
            }
        }

        public static AudioClip LoadAudioClipFromFile(string path)
        {
            using var webRequest = UnityWebRequestMultimedia.GetAudioClip("file://MysticClient/MenuSounds/" + path, AudioType.MPEG);
            var operation = webRequest.SendWebRequest();
            while (!operation.isDone) { }
            if (webRequest.result != UnityWebRequest.Result.Success) Debug.LogError($"Failed to load audio clip: {webRequest.error}");
            return DownloadHandlerAudioClip.GetContent(webRequest);
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
            Object.loop = false;
            Object.PlayOneShot(clip);
        }
        public static void PlayAudio(string MP3Link)
        {
            if (Object == null) { Object = new GameObject().AddComponent<AudioSource>(); }
            Object.transform.position = RigUtils.MyPlayer.transform.position;
            Object.clip = GetAudioFromURL(MP3Link);
            Object.loop = false;
            Object.Play();
        }
        public static void PlayMCAudio(AudioClip clip)
        {
            if (MCObject == null) { MCObject = new GameObject().AddComponent<AudioSource>(); }
            MCObject.transform.position = RigUtils.MyPlayer.transform.position;
            MCObject.loop = MCObjectLoop;
            MCObject.PlayOneShot(clip);
        }

        // this is not used for anything harmful it's used to download extentions for the menu like the gui
        public static void InstallPlugin(string URLPath, string FileName)
        {
            var client = new WebClient();
            if (!File.Exists($"BepInEx\\plugins\\{FileName}"))
            {
                client.DownloadFile(URLPath, FileName);
                File.Move(FileName, "BepInEx\\plugins");
            } 
            else NotifiLib.SendNotification(NotifUtils.Warning() + $"File {FileName} already exist. try restarting your game to fix if the plugin if not working");
        }
    }
}