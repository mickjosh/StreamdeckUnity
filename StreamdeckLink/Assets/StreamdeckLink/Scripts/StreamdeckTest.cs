using UnityEngine;

namespace StudioVR.Streamdeck
{
    [AddComponentMenu("Streamdeck/StreamdeckTest")]
    public class StreamdeckTest : MonoBehaviour
    {
        public StreamdeckClient client;

        private void Start()
        {
            client.OnConnect.AddListener(Connection);
            client.OnDisconnect.AddListener(Disconnection);

            client.OnKeyUp.AddListener(KeyUp);
            client.OnKeyDown.AddListener(KeyDown);

            client.OnWillAppear.AddListener(OnWillAppear);
            client.OnWillDisappear.AddListener(OnWillDisappear);
        }

        public void Connection()
        {
            print("Connected");
        }
        public void Disconnection()
        {
            print("Disconnected");
        }

        public void KeyUp(int KeyID)
        {
            print($"Key:{KeyID} pressed up!");
        }
        public void KeyDown(int KeyID)
        {
            print($"Key:{KeyID} pressed down!");
        }
    
        public void OnWillAppear(int KeyID)
        {
            print($"Key:{KeyID} just appeared!");
        }
        public void OnWillDisappear(int KeyID)
        {
            print($"Key:{KeyID} just disappeared!");
        }
    }
}