using UnityEngine;
using ExitGames.Client.Photon;
using System.IO;
using MysticClient.Utils;
using System;
using BepInEx;
using MysticClient.Mods;

namespace MysticClient.Patches
{
    [BepInPlugin("org.thatguy.networking.com", "PhotonRegister", "1.0.0")] // why
    public class PhotonRegister : BaseUnityPlugin
    {
        void Awake() => RegisterCustomDataTypes();

        public static void RegisterCustomDataTypes()
        {
            PhotonPeer.RegisterType(typeof(GradientColorKey), 110, SerializeGradientColorKey, DeserializeGradientColorKey);
            PhotonPeer.RegisterType(typeof(ProjectileLib.ProjectileData), 111, SerializeProjectileData, DeserializeProjectileData);
            PhotonPeer.RegisterType(typeof(Color), 112, SerializeColor, DeserializeColor);
            PhotonPeer.RegisterType(typeof(Fun.CubeManager), 113, SerializeCube, DeserializeCube);
            Debug.Log("Registered custom data types [UnityEngine.GradientColorKey, MysticClient.Utils.ProjectileLib.ProjectileData, UnityEngine.Color, MysticClient.Mods.Fun.CubeManager] to photon");
        }

        private static byte[] SerializeGradientColorKey(object customObject)
        {
            var gradientkey = (GradientColorKey)customObject;
            var bytes = new byte[16];
            int offset = 0;

            Protocol.Serialize(gradientkey.color.r, bytes, ref offset);
            Protocol.Serialize(gradientkey.color.g, bytes, ref offset);
            Protocol.Serialize(gradientkey.color.b, bytes, ref offset);
            Protocol.Serialize(gradientkey.time, bytes, ref offset);

            return bytes;
        }

        private static object DeserializeGradientColorKey(byte[] data)
        {
            if (data == null || data.Length < 16)
            {
                Debug.LogError("DeserializeGradientColorKey: Data is null or insufficient langth");
                return null;
            }
            var colorKey = new GradientColorKey();
            int offset = 0;
            try
            {
                Protocol.Deserialize(out colorKey.color.r, data, ref offset);
                Protocol.Deserialize(out colorKey.color.g, data, ref offset);
                Protocol.Deserialize(out colorKey.color.b, data, ref offset);
                Protocol.Deserialize(out colorKey.time, data, ref offset);
            }
            catch (Exception ex) { Debug.LogError($"DeserializeGradientColorKey: Error during Deserialization {ex.Message}"); return null; }
            return colorKey;
        }

        private static byte[] SerializeProjectileData(object customObject)
        {
            var data = (ProjectileLib.ProjectileData)customObject;
            using var stream = new MemoryStream();
            using (var writer = new BinaryWriter(stream))
            {
                writer.Write(data.projectile); // projectile hash
                writer.Write(data.trail); // trail hash

                writer.Write(data.position.x); // x pos
                writer.Write(data.position.y); // y pos
                writer.Write(data.position.z); // z pos

                writer.Write(data.velocity.x); // x velo
                writer.Write(data.velocity.y); // y velo
                writer.Write(data.velocity.z); // z velo

                writer.Write(data.color.r); // r color value
                writer.Write(data.color.g); // g color value
                writer.Write(data.color.b); // b color value
                writer.Write(data.color.a); // a color value

                writer.Write(data.size); // projectile size
            }
            return stream.ToArray();
        }

        private static object DeserializeProjectileData(byte[] data)
        {
            using var stream = new MemoryStream(data);
            using var reader = new BinaryReader(stream);

            var projectile = reader.ReadInt32();

            var trail = reader.ReadInt32();

            var position = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());

            var velocity = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());

            var color = new Color(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());

            return new ProjectileLib.ProjectileData(projectile, trail, position, velocity, color, reader.ReadSingle());
        }

        private static byte[] SerializeColor(object customObject)
        {
            var color = (Color)customObject;
            var bytes = new byte[16];
            int offset = 0;

            Protocol.Serialize(color.r, bytes, ref offset);
            Protocol.Serialize(color.g, bytes, ref offset);
            Protocol.Serialize(color.b, bytes, ref offset);
            Protocol.Serialize(color.a, bytes, ref offset);

            return bytes;
        }

        private static object DeserializeColor(byte[] data)
        {
            if (data == null || data.Length < 16)
            {
                Debug.LogError("DeserializeColor: Data is null or insufficient langth");
                return null;
            }
            var color = new Color();
            int offset = 0;
            try
            {
                Protocol.Deserialize(out color.r, data, ref offset);
                Protocol.Deserialize(out color.g, data, ref offset);
                Protocol.Deserialize(out color.b, data, ref offset);
                Protocol.Deserialize(out color.a, data, ref offset);
            }
            catch (Exception ex) { Debug.LogError($"DeserializeColor: Error during Deserialization - {ex.Message}"); return null; }
            return color;
        }

        private static byte[] SerializeCube(object customObject)
        {
            var data = (Fun.CubeManager)customObject;
            using var stream = new MemoryStream();
            using (var writer = new BinaryWriter(stream))
                writer.Write(data.cubeID);
            return stream.ToArray();
        }

        private static object DeserializeCube(byte[] data)
        {
            using var stream = new MemoryStream(data);
            using var reader = new BinaryReader(stream);
            var cubeID = reader.ReadString();
            return new Fun.CubeManager { cubeID = cubeID };
        }
    }
}