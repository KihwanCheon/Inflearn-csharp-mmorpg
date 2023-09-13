using System;
using System.Net;
using System.Threading;
using ServerCore;

namespace Server
{

    class Program
    {
        private static readonly Listener _listener = new Listener();
        public static GameRoom Room { get; } = new GameRoom();

        static void Main(string[] args)
        {
            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

            try
            {
                _listener.Init(endPoint, SessionManager.Instance.Generate);
                Console.WriteLine("Listening ....");

                int roomTick = 0;
                while (true)
                {
                    int now = Environment.TickCount;
                    if (roomTick < now)
                    {
                        Room.Push(Room.Flush); //
                        roomTick = now + 250;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"{e}");
            }
        }
    }
}
