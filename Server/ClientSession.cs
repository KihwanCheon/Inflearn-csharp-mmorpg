using System;
using System.Net;
using System.Threading;
using ServerCore;

namespace Server
{
    public abstract class Packet
    {
        public ushort size;
        public ushort packatId;

        public abstract ArraySegment<byte> Write();
        public abstract void Read(ArraySegment<byte> s);
    }

    public enum PacketID
    {
        PlayerInfoReq = 1,
        PlayerInfoOk = 2
    }

    class PlayerInfoReq : Packet
    {
        public long playerId;
        public string name;

        public PlayerInfoReq()
        {
            packatId = (ushort)PacketID.PlayerInfoReq;
        }

        public override void Read(ArraySegment<byte> segment)
        {
            ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
            ushort count = 0;
            // ushort size = BitConverter.ToUInt16(s.Array, s.Offset);
            count += 2;
            // ushort id = BitConverter.ToUInt16(s.Array, s.Offset + count);
            count += 2;
            // playerId = BitConverter.ToInt64(s.Array, s.Offset + count);
            playerId = BitConverter.ToInt64(s.Slice(count, s.Length - count));
            count += 8;
        }

        public override ArraySegment<byte> Write()
        {
            ArraySegment<byte> segment = SendBufferHelper.Open(4096);
            Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);
            bool success = true;
            ushort count = 0;

            // success &= BitConverter.TryWriteBytes(new Span<byte>(s.Array, s.Offset + count, s.Count - count), size);
            count += sizeof(ushort);
            success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), packatId);
            count += sizeof(ushort);
            success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), playerId);
            count += sizeof(long);
            success &= BitConverter.TryWriteBytes(s, count); // write count at last, after packet counted

            if (!success)
                return null;

            return SendBufferHelper.Close(count);
        }
    }

    // class PlayerInfoOk: Packet
    // {
    //     public int hp;
    //     public int attack;
    // }

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
            ushort count = 0;
            ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
            count += HeaderSize;
            ushort id = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
            count += 2;

            switch ((PacketID) id)
            {
                case PacketID.PlayerInfoReq:
                {
                    var req = new PlayerInfoReq();
                    req.Read(buffer);
                    Console.WriteLine($"PlayerInfoReq : {req.playerId}");
                }
                break;
            }

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
}