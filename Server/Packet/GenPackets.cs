using System;
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
        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        ushort count = 0;
        count += sizeof(ushort);    // for size
        count += sizeof(ushort);    // for PacketID
        ushort chatLen = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
        count += sizeof(ushort);    // for chatLen
        this.chat = Encoding.Unicode.GetString(s.Slice(count, chatLen));
        count += chatLen;           // for this.chat
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);
        bool success = true;
        ushort count = 0;
        
        count += sizeof(ushort);    // for size
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.C_Chat);
        count += sizeof(ushort);    // for PacketID

        // string
        ushort chatLenCount = sizeof(ushort);
        ushort chatLen = (ushort)Encoding.Unicode.GetBytes(chat, 0, this.chat.Length, segment.Array, segment.Offset + count + chatLenCount);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), chatLen);
        count += chatLenCount;
        count += chatLen;

        // write count at last, after packet counted
        success &= BitConverter.TryWriteBytes(s, count);

        if (!success)
            return null;

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
        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        ushort count = 0;
        count += sizeof(ushort);    // for size
        count += sizeof(ushort);    // for PacketID
        this.playerId = BitConverter.ToInt32(s.Slice(count, s.Length - count));
        count += sizeof(int);
        ushort chatLen = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
        count += sizeof(ushort);    // for chatLen
        this.chat = Encoding.Unicode.GetString(s.Slice(count, chatLen));
        count += chatLen;           // for this.chat
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);
        bool success = true;
        ushort count = 0;
        
        count += sizeof(ushort);    // for size
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.S_Chat);
        count += sizeof(ushort);    // for PacketID

        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.playerId);
        count += sizeof(int);
        // string
        ushort chatLenCount = sizeof(ushort);
        ushort chatLen = (ushort)Encoding.Unicode.GetBytes(chat, 0, this.chat.Length, segment.Array, segment.Offset + count + chatLenCount);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), chatLen);
        count += chatLenCount;
        count += chatLen;

        // write count at last, after packet counted
        success &= BitConverter.TryWriteBytes(s, count);

        if (!success)
            return null;

        return SendBufferHelper.Close(count);
    }
}


