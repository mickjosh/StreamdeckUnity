using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityStreamdeckPlugin.Action;

namespace UnityStreamdeckPlugin
{
    public class Server
    {
        public static Server instance;

        public const int Port = 28768;
        public const float TickPerSeconds = 30f;
        public const int HeartbeatTimeout = 5000;

        private TcpListener _listener;
        private List<Client> _clients = new List<Client>();
        private DateTime _nextFrame;

        public static void Run()
        {
            instance = new Server();

            Task.Run(() =>
            {
                instance.Init();

                while(true)
                {
                    instance.Loop();
                }
            });
        }

        private void Init()
        {
            _listener = new TcpListener(IPAddress.Any, Port);
            _listener.Start();
            _listener.BeginAcceptTcpClient(AcceptClient, null);

            _nextFrame = DateTime.MinValue;
        }

        private void Loop()
        {
            while (DateTime.Now >= _nextFrame)
            {
                var clientsToRemove = new List<Client>();

                foreach(var client in _clients)
                {
                    try
                    {
                        if (client.IsDead())
                        {
                            clientsToRemove.Add(client);
                            client.Dispose();
                            continue;
                        }

                        while (client.Stream.DataAvailable)
                        {
                            HandleMessage(client);
                        }
                    }
                    catch { }
                }

                clientsToRemove.ForEach(x => _clients.Remove(x));

                _nextFrame = DateTime.Now.AddSeconds(1 / TickPerSeconds);
            }

            Thread.Sleep(_nextFrame - DateTime.Now);
        }
    
        private void HandleMessage(Client client)
        {
            try
            {
                //Read the next incoming message
                //Header: 4 bytes (Int32), representing the lenght of the message (X)
                //Message: X bytes (Json), representing the message in a json format
                byte[] buffer = new byte[4];
                client.Stream.Read(buffer, 0, 4);

                int byteToRead = BitConverter.ToInt32(buffer, 0);

                buffer = new byte[byteToRead];
                client.Stream.Read(buffer, 0, byteToRead);

                var json = JObject.Parse(Encoding.UTF8.GetString(buffer));

                var keyID = json["keyID"].ToObject<int>();
                switch(json["type"].ToObject<string>())
                {
                    case "Heartbeat":
                        client.Heartbeat();
                        break;

                    case "SetImage":
                        _ = UnityKey.SetImage(json["data"].ToObject<string>(), keyID);
                        break;

                    case "SetImagePath":
                        _ = UnityKey.SetImagePath(json["data"].ToObject<string>(), keyID);
                        break;

                    case "SetTitle":
                        _ = UnityKey.SetTitle(json["data"].ToObject<string>(), keyID);
                        break;
                }
            }
            catch { }
        }
        private void SendMessage(string json)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(json);
            byte[] size = BitConverter.GetBytes(buffer.Length);

            byte[] data = size.Concat(buffer).ToArray();

            foreach (var client in _clients)
            {
                try
                {
                    _ = client.Stream.WriteAsync(data, 0, data.Length);
                }
                catch { }
            }
        }

        private void AcceptClient(IAsyncResult result)
        {
            try
            {
                TcpClient client = _listener.EndAcceptTcpClient(result);

                _clients.Add(new Client(client));
            }
            catch { }

            _listener.BeginAcceptTcpClient(AcceptClient, null);
        }

        public void SendKeyUp(int keyID)
        {
            var json = new JObject();

            json["type"] = "KeyUp";
            json["keyID"] = keyID;

            SendMessage(json.ToString());
        }
        public void SendKeyDown(int keyID)
        {
            var json = new JObject();

            json["type"] = "KeyDown";
            json["keyID"] = keyID;

            SendMessage(json.ToString());
        }

        public void SendOnWillAppear(int keyID)
        {
            var json = new JObject();

            json["type"] = "OnWillAppear";
            json["keyID"] = keyID;

            SendMessage(json.ToString());
        }
        public void SendOnWillDisappear(int keyID)
        {
            var json = new JObject();

            json["type"] = "OnWillDisappear";
            json["keyID"] = keyID;

            SendMessage(json.ToString());
        }

        private class Client
        {
            public Client(TcpClient tcp)
            {
                Tcp = tcp;
                Stream = tcp.GetStream();

                Heartbeat();
            }

            public TcpClient Tcp;
            public NetworkStream Stream;

            private DateTime _lastHeartbeat;

            public bool IsDead()
            {
                return (DateTime.UtcNow - _lastHeartbeat).TotalMilliseconds >= Server.HeartbeatTimeout;
            }
            public void Heartbeat()
            {
                _lastHeartbeat = DateTime.UtcNow;
            }

            public void Dispose()
            {
                Tcp.Dispose();
                Stream.Dispose();
            }
        }
    }
}
