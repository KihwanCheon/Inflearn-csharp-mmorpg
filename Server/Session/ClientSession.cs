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
            Console.WriteLine($"OnConnected: {endPoint}");
            Room = Program.Room;
            Room.Push(() => Room.Enter(this));
        }

        public override void OnRecvPacket(ArraySegment<byte> buffer)
        {
            PacketManager.Instance.OnRecvPacket(this, buffer);
        }
        
        public override void OnSend(int numOfBytes)
        {
            //Console.WriteLine($"To Client Transferred bytes: {numOfBytes}");
        }

        public override void OnDisconnected(EndPoint endPoint)
        {
            SessionManager.Instance.Remove(this);
            if (Room != null)
            {
                var room = Room;
                room.Push(() => { room.Leave(this); });
                Room = null;
            }
            
            Console.WriteLine($"OnDisconnected: {endPoint}");
        }
    }
}