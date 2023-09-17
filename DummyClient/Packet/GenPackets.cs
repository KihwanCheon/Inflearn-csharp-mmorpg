using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using ServerCore;

public enum PacketID
{
    C_Chat = 1,
	S_Chat = 2,
	
}

public interface IPacket
{
    ushort Protocol { get; }
    void Read(ArraySegment<byte> segment);
    ArraySegment<byte> Write();
}


public class C_Chat : IPacket
{
    public string chat;

    public ushort Protocol { get { return (ushort)PacketID.C_Chat; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;
        count += sizeof(ushort);    // for size
        count += sizeof(ushort);    // for PacketID
        ushort chatLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
        count += sizeof(ushort);    // for chatLen
        this.chat = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, chatLen);
        count += chatLen;           // for this.chat
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;
        
        count += sizeof(ushort);    // for size
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.C_Chat), 0, segment.Array, segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);    // for PacketID

        // string
        ushort chatLen = (ushort)Encoding.Unicode.GetBytes(chat, 0, this.chat.Length, segment.Array, segment.Offset + count + sizeof(ushort));
        Array.Copy(BitConverter.GetBytes(chatLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));;
        count += sizeof(ushort);
        count += chatLen;

        // write count at last, after packet counted
        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));
        
        return SendBufferHelper.Close(count);
    }
}

public class S_Chat : IPacket
{
    public int playerId;
    public string chat;

    public ushort Protocol { get { return (ushort)PacketID.S_Chat; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;
        count += sizeof(ushort);    // for size
        count += sizeof(ushort);    // for PacketID
        this.playerId = BitConverter.ToInt32(segment.Array, segment.Offset + count);
        count += sizeof(int);
        ushort chatLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
        count += sizeof(ushort);    // for chatLen
        this.chat = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, chatLen);
        count += chatLen;           // for this.chat
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;
        
        count += sizeof(ushort);    // for size
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.S_Chat), 0, segment.Array, segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);    // for PacketID

        Array.Copy(BitConverter.GetBytes(playerId), 0, segment.Array, segment.Offset + count, sizeof(int));
        count += sizeof(int);
        // string
        ushort chatLen = (ushort)Encoding.Unicode.GetBytes(chat, 0, this.chat.Length, segment.Array, segment.Offset + count + sizeof(ushort));
        Array.Copy(BitConverter.GetBytes(chatLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));;
        count += sizeof(ushort);
        count += chatLen;

        // write count at last, after packet counted
        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));
        
        return SendBufferHelper.Close(count);
    }
}


