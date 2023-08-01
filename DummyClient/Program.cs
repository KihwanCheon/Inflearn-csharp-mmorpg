using System;
using System.Net;
using System.Text;
using ServerCore;

namespace DummyClient
{
        class Packet
    {
        public ushort size;
        public ushort packatId;
    }

    
    class GameSession : Session
    {
        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected: {endPoint}");

            // 보낸다.
            for (int i = 0; i < 5; i++)
            {
                ArraySegment<byte> openSegment = SendBufferHelper.Open(4096);

                var packet = new Packet { size = 4, packatId = 7 };
                byte[] buffer = BitConverter.GetBytes(packet.size);
                byte[] buffer2 = BitConverter.GetBytes(packet.packatId);

                Array.Copy(buffer, 0, openSegment.Array, openSegment.Offset, buffer.Length);
                Array.Copy(buffer2, 0, openSegment.Array, openSegment.Offset + buffer.Length, buffer2.Length);

                ArraySegment<byte> sendBuff = SendBufferHelper.Close(packet.size);

                Send(sendBuff);
            }
        }

        public override int OnRecv(ArraySegment<byte> buffer)
        {
            if (buffer == null || buffer.Array == null)
            {
                Console.WriteLine($"[From Server] null array");
                return -1;
            }
            string recvData = Encoding.UTF8.GetString(buffer.Array, buffer.Offset, buffer.Count);
            Console.WriteLine($"[From Server] {recvData}");
            return buffer.Count;
        }

        public override void OnSend(int numOfBytes)
        {
            Console.WriteLine($"To Server Transferred bytes: {numOfBytes}");
        }

        public override void OnDisconnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnDisconnected: {endPoint}");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

            Connector connector = new Connector();
            connector.Connect(endPoint, () => new GameSession());

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
