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
        public string name;

        public PlayerInfoReq()
        {
            packatId = (ushort)PacketID.PlayerInfoReq;
        }
        
        public override void Read(ArraySegment<byte> segment)
        {
            ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
            ushort count = 0;
            count += sizeof(ushort);    // for this.size
            count += sizeof(ushort);    // for this.packetId
            playerId = BitConverter.ToInt64(s.Slice(count, s.Length - count));
            count += sizeof(long);      // for this.playerId

            // string
            ushort nameLen = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
            count += sizeof(ushort);    // for nameLen
            name = Encoding.Unicode.GetString(s.Slice(count, nameLen));
            count += nameLen;           // for this.name
        }

        public override ArraySegment<byte> Write()
        {
            ArraySegment<byte> segment = SendBufferHelper.Open(4096);
            Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);
            bool success = true;
            ushort count = 0;

            // success &= BitConverter.TryWriteBytes(new Span<byte>(s.Array, s.Offset + count, s.Count - count), size);
            count += sizeof(ushort);    // for this.size
            success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), packatId);
            count += sizeof(ushort);    // for this.packetId
            success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), playerId);
            count += sizeof(long);      // for this.playerId

            // string name len [2], byte[]
            ushort nameLen = (ushort)Encoding.Unicode.GetByteCount(name); // use utf16
            success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), nameLen);
            count += sizeof(ushort);
            Array.Copy(Encoding.Unicode.GetBytes(name), 0, segment.Array, count, nameLen);
            count += nameLen;

            // write count at last, after packet counted
            success &= BitConverter.TryWriteBytes(s, count);

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

            var packet = new PlayerInfoReq { playerId = 333, name = "TestId"};
            
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