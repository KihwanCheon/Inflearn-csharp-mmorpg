using System;
using System.Net;
using System.Threading;
using ServerCore;

namespace Server
{
    class Packet
    {
        public ushort size;
        public ushort packatId;
    }


    class GameSession : PacketSession
    {
        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected: {endPoint}");

            /*ArraySegment<byte> openSegment = SendBufferHelper.Open(4096);

            var packet = new Packet { size = 4, packatId = 7 };
            byte[] buffer = BitConverter.GetBytes(packet.size);
            byte[] buffer2 = BitConverter.GetBytes(packet.packatId);

            Array.Copy(buffer, 0, openSegment.Array, openSegment.Offset, buffer.Length);
            Array.Copy(buffer2, 0, openSegment.Array, openSegment.Offset + buffer.Length, buffer2.Length);

            ArraySegment<byte> sendBuff = SendBufferHelper.Close(buffer.Length + buffer2.Length);
            Send(sendBuff);*/

            Thread.Sleep(1000);

            // 내보낸다.
            Disconnect();
            Disconnect(); // 실수로 연달아 호출?
        }

        public override void OnRecvPacket(ArraySegment<byte> buffer)
        {
            ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
            ushort id = BitConverter.ToUInt16(buffer.Array, buffer.Offset + HeaderSize);

            Console.WriteLine($"Recv packetId: {id}, size: {size}");
        }


        public override void OnSend(int numOfBytes)
        {
            Console.WriteLine($"To Client Transferred bytes: {numOfBytes}");
        }

        public override void OnDisconnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnDisconnected: {endPoint}");
        }
    }

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
                _listener.Init(endPoint, () => new GameSession());
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
