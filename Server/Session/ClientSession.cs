using System;
using System.Net;
using ServerCore;

namespace Server
{
    /// <summary>클라 대리자</summary>
    public class ClientSession : PacketSession
    {
        public int SessionId { get; }
        public GameRoom Room { get; set; }

        public ClientSession(int sessionId)
        {
            SessionId = sessionId;
        }

        public override void OnConnected(EndPoint endPoint)
        {
            Room = Program.Room;
            Room.Enter(this);
            Console.WriteLine($"OnConnected: {endPoint}");
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
            if (Room != null)
            {
                Room.Leave(this);
                Room = null;
            }

            SessionManager.Instance.Remove(this);

            Console.WriteLine($"OnDisconnected: {endPoint}");
        }
    }
}