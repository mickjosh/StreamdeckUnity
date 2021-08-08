using SharpDeck;
using SharpDeck.Events.Received;
using SharpDeck.Manifest;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace UnityStreamdeckPlugin.Action
{
    public class UnityKey : StreamDeckAction
    {
        private static List<UnityKey>[] Keys = new List<UnityKey>[16]
        {
            new List<UnityKey>(),
            new List<UnityKey>(),
            new List<UnityKey>(),
            new List<UnityKey>(),
            new List<UnityKey>(),
            new List<UnityKey>(),
            new List<UnityKey>(),
            new List<UnityKey>(),
            new List<UnityKey>(),
            new List<UnityKey>(),
            new List<UnityKey>(),
            new List<UnityKey>(),
            new List<UnityKey>(),
            new List<UnityKey>(),
            new List<UnityKey>(),
            new List<UnityKey>()
        };

        public int KeyID;

        protected override Task OnKeyDown(ActionEventArgs<KeyPayload> args)
        {
            Server.instance.SendKeyDown(KeyID);

            return base.OnKeyDown(args);
        }

        protected override Task OnKeyUp(ActionEventArgs<KeyPayload> args)
        {
            Server.instance.SendKeyUp(KeyID);

            return base.OnKeyUp(args);
        }

        protected override Task OnWillAppear(ActionEventArgs<AppearancePayload> args)
        {
            Keys[KeyID].Add(this);
            return base.OnWillAppear(args);
        }
        protected override Task OnWillDisappear(ActionEventArgs<AppearancePayload> args)
        {
            Keys[KeyID].Remove(this);
            return base.OnWillDisappear(args);
        }
    
        public static async Task SetImagePath(string imagePath, int keyID)
        {
            var image64 = $"data:image/{Path.GetExtension(imagePath).Replace(".", "")};base64,{Convert.ToBase64String(await File.ReadAllBytesAsync(imagePath))}";

            foreach (var key in Keys[keyID])
            {
                await key.SetImageAsync(image64);
            }
        }
        public static async Task SetImage(string base64Image, int keyID)
        {
            foreach (var key in Keys[keyID])
            {
                await key.SetImageAsync(base64Image);
            }
        }
        public static async Task SetTitle(string title, int keyID)
        {
            foreach (var key in Keys[keyID])
            {
                await key.SetTitleAsync(title);
            }
        }
    }

    [StreamDeckAction("Key0", "com.studiovr.unitylink.key0")]
    class Key0 : UnityKey { public Key0() { KeyID = 0; } }

    [StreamDeckAction("Key1", "com.studiovr.unitylink.key1")]
    class Key1 : UnityKey { public Key1() { KeyID = 1; } }

    [StreamDeckAction("Key2", "com.studiovr.unitylink.key2")]
    class Key2 : UnityKey { public Key2() { KeyID = 2; } }

    [StreamDeckAction("Key3", "com.studiovr.unitylink.key3")]
    class Key3 : UnityKey { public Key3() { KeyID = 3; } }

    [StreamDeckAction("Key4", "com.studiovr.unitylink.key4")]
    class Key4 : UnityKey { public Key4() { KeyID = 4; } }

    [StreamDeckAction("Key5", "com.studiovr.unitylink.key5")]
    class Key5 : UnityKey { public Key5() { KeyID = 5; } }

    [StreamDeckAction("Key6", "com.studiovr.unitylink.key6")]
    class Key6 : UnityKey { public Key6() { KeyID = 6; } }

    [StreamDeckAction("Key7", "com.studiovr.unitylink.key7")]
    class Key7 : UnityKey { public Key7() { KeyID = 7; } }

    [StreamDeckAction("Key8", "com.studiovr.unitylink.key8")]
    class Key8 : UnityKey { public Key8() { KeyID = 8; } }

    [StreamDeckAction("Key9", "com.studiovr.unitylink.key9")]
    class Key9 : UnityKey { public Key9() { KeyID = 9; } }

    [StreamDeckAction("Key10", "com.studiovr.unitylink.key10")]
    class Key10 : UnityKey { public Key10() { KeyID = 10; } }

    [StreamDeckAction("Key11", "com.studiovr.unitylink.key11")]
    class Key11 : UnityKey { public Key11() { KeyID = 11; } }

    [StreamDeckAction("Key12", "com.studiovr.unitylink.key12")]
    class Key12 : UnityKey { public Key12() { KeyID = 12; } }

    [StreamDeckAction("Key13", "com.studiovr.unitylink.key13")]
    class Key13 : UnityKey { public Key13() { KeyID = 13; } }

    [StreamDeckAction("Key14", "com.studiovr.unitylink.key14")]
    class Key14 : UnityKey { public Key14() { KeyID = 14; } }

    [StreamDeckAction("Key15", "com.studiovr.unitylink.key15")]
    class Key15 : UnityKey { public Key15() { KeyID = 15; } }
}
