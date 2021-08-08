using System;
using UnityEngine;

namespace StudioVR.Streamdeck
{
    [AddComponentMenu("Streamdeck/StreamdeckImages")]
    public class StreamdeckImages : MonoBehaviour
    {
        public StreamdeckClient client;
        public Texture2D[] Images = new Texture2D[16];

        private void Start()
        {
            client.OnConnect.AddListener(OnConnect);
        }

        public void OnConnect()
        {
            try
            {
                for (int i = 0; i < Images.Length; i++)
                {
                    if (Images[i] != null)
                    {
                        client.SetImage(Images[i].ToBase64String(), i);
                    }
                }
            }
            catch (Exception e) { print(e.Message); }
        }

        public void SetImage(Texture2D tex, int keyID)
        {
            Images[keyID] = tex;
            client.SetImage(tex.ToBase64String(), keyID);
        }
    }
}
