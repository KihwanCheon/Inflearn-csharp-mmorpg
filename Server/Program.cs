using System;
using System.Net;
using ServerCore;

namespace Server
{

    class Program
    {
        private static Listener _listener = new Listener();

        static void Main(string[] args)
        {
            PacketManager.Instance.Register(); // before multi threading.

            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

            try
            {
                _listener.Init(endPoint, () => new ClientSession());
                Console.WriteLine("Listening ....");

                while (true)
                {
                    ; //
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"{e}");
            }
        }
    }
}
