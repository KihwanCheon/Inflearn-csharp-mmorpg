using System;
using System.Collections.Generic;
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

    public class PlayerInfoReq : Packet
    {
        public long playerId;
        public string name;

        public struct SkillInfo
        {
            public int id;
            public short level;
            public float duration;

            public bool Write(Span<byte> s, ref ushort count)
            {
                bool success = true;

                success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), id);
                count += sizeof(int);
                success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), level);
                count += sizeof(short);
                success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), duration);
                count += sizeof(float);

                return success;
            }

            public void Read(ReadOnlySpan<byte>s, ref ushort count)
            {
                id = BitConverter.ToInt32(s.Slice(count, s.Length - count));
                count += sizeof(int);
                level = BitConverter.ToInt16(s.Slice(count, s.Length - count));
                count += sizeof(short);
                duration = BitConverter.ToSingle(s.Slice(count, s.Length - count));
                count += sizeof(float);
            }
        }

        public List<SkillInfo> skills = new List<SkillInfo>();

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

            // string name
            ushort nameLen = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
            count += sizeof(ushort);    // for nameLen
            name = Encoding.Unicode.GetString(s.Slice(count, nameLen));
            count += nameLen;           // for this.name

            // skill list
            skills.Clear();
            ushort skillLen = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
            count += sizeof(ushort);

            for (int i = 0; i < skillLen; i++)
            {
                SkillInfo skill = new SkillInfo();
                skill.Read(s, ref count);
                skills.Add(skill);
            }
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

            // string name
            ushort nameLenCount = sizeof(ushort);
            ushort nameLen = (ushort)Encoding.Unicode.GetBytes(name, 0, name.Length, segment.Array, segment.Offset + count + nameLenCount);
            success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), nameLen);
            count += nameLenCount;
            count += nameLen;

            // skill list
            success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)skills.Count);
            count += sizeof(ushort);
            foreach (var skill in skills)
                success &= skill.Write(s, ref count);
            
            // write count at last, after packet counted
            success &= BitConverter.TryWriteBytes(s, count);

            if (!success)
                return null;

            return SendBufferHelper.Close(count);
        }
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

            var packet = new PlayerInfoReq { playerId = 333, name = "TestId"};
            packet.skills.Add(new PlayerInfoReq.SkillInfo{id = 101, level = 1, duration = 3.0f});
            packet.skills.Add(new PlayerInfoReq.SkillInfo{id = 201, level = 2, duration = 4.0f});
            packet.skills.Add(new PlayerInfoReq.SkillInfo{id = 301, level = 3, duration = 5.0f});
            packet.skills.Add(new PlayerInfoReq.SkillInfo{id = 401, level = 4, duration = 6.0f});

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