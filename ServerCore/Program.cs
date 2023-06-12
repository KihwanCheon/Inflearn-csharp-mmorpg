using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ServerCore
{
    class Program
    {
        private static Listener _listener = new Listener();

        static void Main(string[] args)
        {
            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

            try
            {
                _listener.Init(endPoint);

                while (true)
                {
                    Console.WriteLine("Listening ....");

                    // 손님입장.
                    Socket client = _listener.Accept(); // blocking

                    // 받는다.
                    byte[] recvBuff = new byte[1024];
                    int recvBytes = client.Receive(recvBuff);
                    string recvData = Encoding.UTF8.GetString(recvBuff, 0, recvBytes);
                    Console.WriteLine($"[From Client] {recvData}");

                    // 보낸다.
                    byte[] sendBuff = Encoding.UTF8.GetBytes("Welcome to MMORPG Server!");
                    client.Send(sendBuff);          // blocking.

                    // 내보낸다.
                    client.Shutdown(SocketShutdown.Both);
                    client.Close();
                }

            }
            catch (Exception e)
            {
                Console.WriteLine($"{e}");
            }
        }
    }
}
