using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityAsync;
using UnityEngine.Events;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;
using System.Linq;
using System.Threading;

namespace StudioVR.Streamdeck
{
    [AddComponentMenu("Streamdeck/StreamdeckClient")]
    public class StreamdeckClient : MonoBehaviour
    {
        [Header("Streamdeck")]
        public StreamdeckEvent OnKeyUp;
        public StreamdeckEvent OnKeyDown;
        public StreamdeckEvent OnWillAppear;
        public StreamdeckEvent OnWillDisappear;

        [Header("Server")]
        public UnityEvent OnConnect;
        public UnityEvent OnDisconnect;

        [Header("Settings")]
        public string IpAddress = "127.0.0.1";
        public int Port = 28768;
        public float TickPerSeconds = 20;

        public bool IsConnected
        {
            get { return _isConnected; }
            set
            {
                if (value != _isConnected)
                {
                    if (value)
                        OnConnection();
                    else
                        OnDisconnection();

                    _isConnected = value;
                }
            }
        }
        private bool _isConnected = false;

        private TcpClient _tcp;
        private NetworkStream _stream;

        private DateTime nextFrame;

        // Start is called before the first frame update
        void Start()
        {
            _ = Task.Run(() =>
            {
                ClientInit();

                while (true)
                {
                    ClientLoop();
                }
            });
        }

        /// <summary>
        /// Set the image of a key from a file
        /// </summary>
        /// <param name="path">The path of the image</param>
        /// <param name="keyID">The keyID to set the image to</param>
        public void SetImagePath(string path, int keyID)
        {
            SendMessage(new StreamdeckMesssageData<string>() { type = "SetImagePath", keyID = keyID, data = path });
        }
        /// <summary>
        /// Set the image of a key from a base64 image
        /// </summary>
        /// <param name="base64Image">The base64 image with a mime header</param>
        /// <param name="keyID">The keyID to set the image to</param>
        public void SetImage(string base64Image, int keyID)
        {
            SendMessage(new StreamdeckMesssageData<string>() { type = "SetImage", keyID = keyID, data = base64Image });
        }
        /// <summary>
        /// Set the title of a key
        /// </summary>
        /// <param name="title">The title to set on the key</param>
        /// <param name="keyID">The keyID to set the image to</param>
        public void SetTitle(string title, int keyID)
        {
            SendMessage(new StreamdeckMesssageData<string>() { type = "SetTitle", keyID = keyID, data = title });
        }

        private async Task RunOnUnityThread(Action action)
        {
            try
            {
                await new WaitForFrames(1);
                action.Invoke();
            }
            catch (Exception e) { Debug.LogError($"{e.Message}:{e.StackTrace}"); }
        }

        private void ClientInit()
        {
            nextFrame = DateTime.MinValue;
            Connect();
        }
        private void ClientLoop()
        {
            while (DateTime.Now >= nextFrame)
            {
                nextFrame = DateTime.Now.AddSeconds(1 / TickPerSeconds);

                IsConnected = (_tcp != null) && _tcp.Connected;
                if (IsConnected)
                {
                    SendMessage(new StreamdeckMessage() { type = "Heartbeat" });

                    while (_stream.DataAvailable)
                    {
                        HandleMessage();
                    }
                }
                else
                {
                    Connect();
                }
            }

            Thread.Sleep(nextFrame - DateTime.Now);
        }

        private void Connect()
        {
            try
            {
                _tcp = new TcpClient(IpAddress, Port);
                _stream = _tcp.GetStream();

                IsConnected = _tcp.Connected;
            }
            catch (Exception e) { Debug.LogError(e.Message); }
        }

        private void OnConnection()
        {
            _ = RunOnUnityThread(() => OnConnect.Invoke());
        }
        private void OnDisconnection()
        {
            _ = RunOnUnityThread(() => OnDisconnect.Invoke());
        }

        private void HandleMessage()
        {
            try
            {
                var buffer = new byte[4];
                _stream.Read(buffer, 0, 4);

                var byteToRead = BitConverter.ToInt32(buffer, 0);

                buffer = new byte[byteToRead];
                _stream.Read(buffer, 0, byteToRead);

                var message = JsonConvert.DeserializeObject<StreamdeckMessage>(Encoding.UTF8.GetString(buffer));

                switch (message.type)
                {
                    case "KeyUp":
                        _ = RunOnUnityThread(() => OnKeyUp.Invoke(message.keyID));
                        break;

                    case "KeyDown":
                        _ = RunOnUnityThread(() => OnKeyDown.Invoke(message.keyID));
                        break;

                    case "OnWillAppear":
                        _ = RunOnUnityThread(() => OnWillAppear.Invoke(message.keyID));
                        break;

                    case "OnWillDisappear":
                        _ = RunOnUnityThread(() => OnWillDisappear.Invoke(message.keyID));
                        break;
                }
            }
            catch (Exception e) { Debug.LogError(e.Message); }
        }
        private void SendMessage(StreamdeckMessage message)
        {
            var json = JsonConvert.SerializeObject(message);

            var buffer = Encoding.UTF8.GetBytes(json);
            var size = BitConverter.GetBytes(buffer.Length);

            var data = size.Concat(buffer).ToArray();

            try
            {
                _stream.Write(data, 0, data.Length);
            }
            catch
            {
                IsConnected = false;
            }
        }

        private class StreamdeckMessage
        {
            public string type;
            public int keyID;
        }
        private class StreamdeckMesssageData<T> : StreamdeckMessage
        {
            public T data;
        }

        [System.Serializable]
        public class StreamdeckEvent : UnityEvent<int> { }
    }
}