using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using ServerCore;


public enum PacketID
{
    PlayerInfoReq = 1,
    Test = 2,

}


public class PlayerInfoReq
{
    public byte testByte;
    public sbyte testSByte;
    public long playerId;
    public string name;
    public class Skill
    {
        public int id;
        public short level;
        public float duration;
        public class Attribute
        {
            public int fire;

            public void Read(ReadOnlySpan<byte> s, ref ushort count)
            {
                this.fire = BitConverter.ToInt32(s.Slice(count, s.Length - count));
                count += sizeof(int);
            }

            public bool Write(Span<byte> s, ref ushort count)
            {
                bool success = true;
                success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.fire);
                count += sizeof(int);
                return success;
            }
        }

        public List<Attribute> attrs = new List<Attribute>();

        public void Read(ReadOnlySpan<byte> s, ref ushort count)
        {
            this.id = BitConverter.ToInt32(s.Slice(count, s.Length - count));
            count += sizeof(int);
            this.level = BitConverter.ToInt16(s.Slice(count, s.Length - count));
            count += sizeof(short);
            this.duration = BitConverter.ToSingle(s.Slice(count, s.Length - count));
            count += sizeof(float);
            // attrs list
            this.attrs.Clear();
            ushort attrsLen = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
            count += sizeof(ushort);

            for (int i = 0; i < attrsLen; i++)
            {
                Attribute element = new Attribute();
                element.Read(s, ref count);
                this.attrs.Add(element);
            }
        }

        public bool Write(Span<byte> s, ref ushort count)
        {
            bool success = true;
            success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.id);
            count += sizeof(int);
            success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.level);
            count += sizeof(short);
            success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.duration);
            count += sizeof(float);
            // attrs list
            success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)this.attrs.Count);
            count += sizeof(ushort);
            foreach (Attribute element in this.attrs)
                success &= element.Write(s, ref count);
            return success;
        }
    }

    public List<Skill> skills = new List<Skill>();

    public void Read(ArraySegment<byte> segment)
    {
        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        ushort count = 0;
        count += sizeof(ushort);    // for size
        count += sizeof(ushort);    // for PacketID
        this.testByte = (byte)segment.Array[segment.Offset + count];
        count += sizeof(byte);
        this.testSByte = (sbyte)segment.Array[segment.Offset + count];
        count += sizeof(sbyte);
        this.playerId = BitConverter.ToInt64(s.Slice(count, s.Length - count));
        count += sizeof(long);
        ushort nameLen = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
        count += sizeof(ushort);    // for nameLen
        this.name = Encoding.Unicode.GetString(s.Slice(count, nameLen));
        count += nameLen;           // for this.name
        // skills list
        this.skills.Clear();
        ushort skillsLen = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
        count += sizeof(ushort);

        for (int i = 0; i < skillsLen; i++)
        {
            Skill element = new Skill();
            element.Read(s, ref count);
            this.skills.Add(element);
        }
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);
        bool success = true;
        ushort count = 0;

        count += sizeof(ushort);    // for size
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.PlayerInfoReq);
        count += sizeof(ushort);    // for PacketID

        segment.Array[segment.Offset + count] = (byte)this.testByte;
        count += sizeof(byte);
        segment.Array[segment.Offset + count] = (byte)this.testSByte;
        count += sizeof(sbyte);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.playerId);
        count += sizeof(long);
        // string
        ushort nameLenCount = sizeof(ushort);
        ushort nameLen = (ushort)Encoding.Unicode.GetBytes(name, 0, this.name.Length, segment.Array, segment.Offset + count + nameLenCount);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), nameLen);
        count += nameLenCount;
        count += nameLen;
        // skills list
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)this.skills.Count);
        count += sizeof(ushort);
        foreach (Skill element in this.skills)
            success &= element.Write(s, ref count);

        // write count at last, after packet counted
        success &= BitConverter.TryWriteBytes(s, count);

        if (!success)
            return null;

        return SendBufferHelper.Close(count);
    }
}

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
                    Console.WriteLine($"PlayerInfoReq : {req.playerId}, {req.name}");

                    foreach (var skill in req.skills)
                    {
                        Console.WriteLine($"skill {skill.id}, {skill.level}, {skill.duration}, attrs: {skill.attrs.Count}, (fire: {(skill.attrs.Count > 0 ? skill.attrs[0].fire : 0)})");
                    }
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