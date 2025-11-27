using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Converters;
using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Helpers;
using Archipelago.MultiClient.Net.MessageLog.Messages;
using Archipelago.MultiClient.Net.Packets;

namespace Smushi_AP_Client
{
    public class ArchipelagoHandler
    {
        private const string GameName = "Smushi Come Home";
        public static bool IsConnected;
        public static bool IsConnecting;
        public Action<string, string> OnConnect;

        private readonly ConcurrentQueue<long> _locationsToCheck = new ConcurrentQueue<long>();

        private readonly Random _random = new Random();

        private string _lastDeath;
        private LoginSuccessful _loginSuccessful;
        private ArchipelagoSession _session;
        public SlotData SlotData;

        public ArchipelagoHandler(string server, int port, string slot, string password)
        {
            Server = server;
            Port = port;
            Slot = slot;
            Password = password;
        }

        public string Server { get; }
        public int Port { get; }
        public string Slot { get; }
        public string Password { get; }

        public string Seed { get; set; }
        private double SlotInstance { get; set; }

        private void CreateSession()
        {
            SlotInstance = DateTime.Now.ToUnixTimeStamp();
            _session = ArchipelagoSessionFactory.CreateSession(Server, Port);
            _session.MessageLog.OnMessageReceived += OnMessageReceived;
            _session.Socket.SocketClosed += OnSocketClosed;
            _session.Items.ItemReceived += ItemReceived;
        }

        private void OnSocketClosed(string reason)
        {
            APConsole.Instance.Log($"Connection closed ({reason}) Attempting reconnect...");
            IsConnected = false;
        }

        public void InitConnect()
        {
            IsConnecting = true;
            CreateSession();
            IsConnected = Connect();
            IsConnecting = false;
        }

        private bool Connect()
        {
            Seed = _session.ConnectAsync()?.Result?.SeedName;

            var result = _session.LoginAsync(
                GameName,
                Slot,
                ItemsHandlingFlags.AllItems,
                new Version(1, 0, 0), Array.Empty<string>(),
                password: Password
            ).Result;

            if (result.Successful)
            {
                _loginSuccessful = (LoginSuccessful)result;
                SlotData = new SlotData(_loginSuccessful.SlotData);
                PluginMain.GameHandler.InitOnConnect();
                ConnectionInfoHandler.Save(Server, Port.ToString(), Slot, Password);
                new Thread(RunCheckLocationsFromList).Start();
                OnConnect.Invoke(Seed, Slot);
                return true;
            }

            var failure = (LoginFailure)result;
            var errorMessage = $"Failed to Connect to {Server}:{Port} as {Slot}:";
            errorMessage = failure.Errors.Aggregate(errorMessage, (current, error) => current + $"\n    {error}");
            errorMessage = failure.ErrorCodes.Aggregate(errorMessage, (current, error) => current + $"\n    {error}");
            APConsole.Instance.Log(errorMessage);
            APConsole.Instance.Log("Attempting reconnect...");
            return false;
        }

        private void ItemReceived(ReceivedItemsHelper helper)
        {
            try
            {
                while (helper.Any())
                {
                    var itemIndex = helper.Index;
                    var item = helper.DequeueItem();
                    PluginMain.ItemHandler.HandleItem(itemIndex, item);
                }
            }
            catch (Exception ex)
            {
                APConsole.Instance.Log($"[ItemReceived ERROR] {ex}");
                throw;
            }
        }

        public void Release()
        {
            _session.SetGoalAchieved();
            _session.SetClientState(ArchipelagoClientState.ClientGoal);
        }

        public void CheckLocations(long[] ids)
        {
            ids.ToList().ForEach(id => _locationsToCheck.Enqueue(id));
        }

        public void CheckLocation(long id)
        {
            _locationsToCheck.Enqueue(id);
        }

        private void RunCheckLocationsFromList()
        {
            while (true)
                if (_locationsToCheck.TryDequeue(out var locationId))
                    _session.Locations.CompleteLocationChecks(locationId);
                else
                    Thread.Sleep(100);
        }

        public void Hint(long id)
        {
            _session.Hints.CreateHints(HintStatus.Unspecified, id);
        }

        public bool IsLocationChecked(long id)
        {
            return _session.Locations.AllLocationsChecked.Contains(id);
        }

        public int CountLocationsCheckedInRange(long start, long end)
        {
            var startId = start;
            var endId = end;
            return _session.Locations.AllLocationsChecked.Count(loc => loc >= startId && loc < endId);
        }

        public void UpdateTags(List<string> tags)
        {
            var packet = new ConnectUpdatePacket
            {
                Tags = tags.ToArray(),
                ItemsHandling = ItemsHandlingFlags.AllItems
            };
            _session.Socket.SendPacket(packet);
        }

        private static void OnMessageReceived(LogMessage message)
        {
            APConsole.Instance.Log(message.ToString() ?? string.Empty);
        }
    }
}