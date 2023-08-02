using System;
using System.Net;
using ServerCore;

namespace DummyClient
{
    class Program
    {
        static void Main(string[] args)
        {
            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

            Connector connector = new Connector();
            connector.Connect(endPoint, () => new ServerSession());

            try
            {
                while (true)
                {
                    ;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
