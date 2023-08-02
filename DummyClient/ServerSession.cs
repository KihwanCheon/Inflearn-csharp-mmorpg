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

    public enum PacketID
    {
        PlayerInfoReq = 1,
        PlayerInfoOk = 2
    }

    class PlayerInfoReq : Packet
    {
        public long playerId;
    }

    class PlayerInfoOk: Packet
    {
        public int hp;
        public int attack;
    }

    /**
     * 서버의 대리자.
     */
    public class ServerSession : Session
    {
        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected: {endPoint}");

            var packet = new PlayerInfoReq { size = 12, packatId = (ushort)PacketID.PlayerInfoReq, playerId = 333};

            ArraySegment<byte> s = SendBufferHelper.Open(4096);
            
            byte[] size = BitConverter.GetBytes(packet.size);  // 2
            byte[] packetId = BitConverter.GetBytes(packet.packatId); // 2
            byte[] playerId = BitConverter.GetBytes(packet.playerId); // 8

            int count = 0;
            Array.Copy(size, 0, s.Array, s.Offset + 0, size.Length);
            count += size.Length;
            Array.Copy(packetId, 0, s.Array, s.Offset + count, packetId.Length);
            count += packetId.Length;
            Array.Copy(playerId, 0, s.Array, s.Offset + count, playerId.Length);
            count += playerId.Length;
            ArraySegment<byte> sendBuff = SendBufferHelper.Close(packet.size);

            Send(sendBuff);
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


}