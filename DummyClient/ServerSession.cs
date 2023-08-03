using System;
using System.Net;
using System.Text;
using ServerCore;

namespace DummyClient
{
    public abstract class  Packet
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

        public PlayerInfoReq()
        {
            packatId = (ushort)PacketID.PlayerInfoReq;
        }

        public override ArraySegment<byte> Write()
        {
            ArraySegment<byte> s = SendBufferHelper.Open(4096);
            bool success = true;
            ushort count = 0;
            // success &= BitConverter.TryWriteBytes(new Span<byte>(s.Array, s.Offset + count, s.Count - count), this.size);
            count += 2;
            success &= BitConverter.TryWriteBytes(new Span<byte>(s.Array, s.Offset + count, s.Count - count), this.packatId);
            count += 2;
            success &= BitConverter.TryWriteBytes(new Span<byte>(s.Array, s.Offset + count, s.Count - count), this.playerId);
            count += 8;

            // write count at last, after packet counted
            success &= BitConverter.TryWriteBytes(new Span<byte>(s.Array, s.Offset, s.Count), (ushort)4 /*count*/);

            if (!success)
                return null;

            return SendBufferHelper.Close(count);
        }

        public override void Read(ArraySegment<byte> s)
        {
            ushort count = 0;
            // ushort size = BitConverter.ToUInt16(s.Array, s.Offset);
            count += 2;
            // ushort id = BitConverter.ToUInt16(s.Array, s.Offset + count);
            count += 2;
            // playerId = BitConverter.ToInt64(s.Array, s.Offset + count);
            playerId = BitConverter.ToInt64(new ReadOnlySpan<byte>(s.Array, s.Offset + count, s.Count - count));
            count += 8;
        }
    }

    // class PlayerInfoOk: Packet
    // {
    //     public int hp;
    //     public int attack;
    // }

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

            var packet = new PlayerInfoReq { playerId = 333};
            
            ArraySegment<byte> s = packet.Write();

            if (s != null)
                Send(s);
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