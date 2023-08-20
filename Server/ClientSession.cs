using System;
using System.Net;
using System.Threading;
using ServerCore;


namespace Server
{
    /// <summary>클라 대리자</summary>
    public class ClientSession : PacketSession
    {
        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected: {endPoint}");

            Thread.Sleep(1000);

            // 내보낸다.
            Disconnect();
            Disconnect(); // 실수로 연달아 호출?
        }

        public override void OnRecvPacket(ArraySegment<byte> buffer)
        {
            PacketManager.Instance.OnRecvPacket(this, buffer);
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
}