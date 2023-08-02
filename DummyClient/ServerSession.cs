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

    /// <summary>서버 대리자</summary>
    public class ServerSession : Session
    {
        /**
         * C++ 처럼 포인터를 직접적으로 사용하기.
         * TryWriteBytes 대용으로...
         * unsafe!
         */
        static unsafe void ToBytes(byte[] array, int offset, ulong value)
        {
            fixed (byte* ptr = &array[offset])
                *(ulong*)ptr = value;
        }

        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected: {endPoint}");

            var packet = new PlayerInfoReq { packatId = (ushort)PacketID.PlayerInfoReq, playerId = 333};

            ArraySegment<byte> s = SendBufferHelper.Open(4096);
            bool success = true;
            ushort count = 0;
            // success &= BitConverter.TryWriteBytes(new Span<byte>(s.Array, s.Offset + count, s.Count - count), packet.size);
            count += 2;
            success &= BitConverter.TryWriteBytes(new Span<byte>(s.Array, s.Offset + count, s.Count - count), packet.packatId);
            count += 2;
            success &= BitConverter.TryWriteBytes(new Span<byte>(s.Array, s.Offset + count, s.Count - count), packet.playerId);
            count += 8;

            // write count at last, after packet counted
            success &= BitConverter.TryWriteBytes(new Span<byte>(s.Array, s.Offset, s.Count), count);

            ArraySegment<byte> sendBuff = SendBufferHelper.Close(count);

            if (success)
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