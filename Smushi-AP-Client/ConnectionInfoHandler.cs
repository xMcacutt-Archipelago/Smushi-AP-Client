using System.IO;
using System.Net.Mime;
using Newtonsoft.Json;
using UnityEngine;

namespace Smushi_AP_Client
{
    public class ConnectionInfo
    {
        public string Server { get; set; }
        public string Port { get; set; }
        public string Slot { get; set; }
        public string Password { get; set; }
    }

    public class ConnectionInfoHandler
    {
        private static readonly string fileName = "connection_info.json";

        public static void Save(string server, string port, string slot, string password)
        {
            var connectionInfo = new ConnectionInfo();
            connectionInfo.Server = server;
            connectionInfo.Port = port;
            connectionInfo.Slot = slot;
            connectionInfo.Password = password;
            var text = JsonConvert.SerializeObject(connectionInfo);
            File.WriteAllText(Application.persistentDataPath + fileName, text);
        }

        public static void Load(ref string server, ref string port, ref string slotName, ref string password)
        {
            if (!File.Exists(Application.persistentDataPath + fileName))
                Save("archipelago.gg", "65535", "Player1", "");
            var json = File.ReadAllText(Application.persistentDataPath + fileName);
            var connectionInfo = JsonConvert.DeserializeObject<ConnectionInfo>(json);
            server = connectionInfo.Server;
            port = connectionInfo.Port;
            slotName = connectionInfo.Slot;
            password = connectionInfo.Password;
        }
    }
}