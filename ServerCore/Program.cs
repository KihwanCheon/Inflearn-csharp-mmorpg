using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ServerCore
{
    class Program
    {
        private static Listener _listener = new Listener();

        static void OnAcceptHandler(Socket clientSocket)
        {
            try
            {
                // 받는다.
                Session session = new Session();
                session.Init(clientSocket);

                // 보낸다.
                byte[] sendBuff = Encoding.UTF8.GetBytes("Welcome to MMORPG Server!");
                session.Send(sendBuff);          // blocking.

                Thread.Sleep(1000);

                // 내보낸다.
                session.Disconnect();
            }
            catch (Exception e)
            {
                Console.WriteLine($"OnAcceptHandler {e}");
            }
        }

        static void Main(string[] args)
        {
            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

            try
            {
                _listener.Init(endPoint, OnAcceptHandler);
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
