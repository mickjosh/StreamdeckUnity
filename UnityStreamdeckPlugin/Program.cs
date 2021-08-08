using System;

namespace UnityStreamdeckPlugin
{
    class Program
    {
        static void Main(string[] args)
        {
            Server.Run();
            SharpDeck.StreamDeckPlugin.Run();
        }
    }
}
