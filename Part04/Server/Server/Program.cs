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

        static void FlushRoom()
        {
            Room.Push(Room.Flush); //
            JobTimer.Instance.Push(FlushRoom, 250);
        }

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

                // FlushRoom();
                JobTimer.Instance.Push(FlushRoom);

                while (true)
                {
                    JobTimer.Instance.Flush();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"{e}");
            }
        }
    }
}
