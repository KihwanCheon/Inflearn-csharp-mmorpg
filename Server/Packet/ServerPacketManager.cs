

using System;
using System.Collections.Generic;
using ServerCore;


public class PacketManager
{
    #region Singleton

    public static PacketManager Instance { get; } = new PacketManager();

    PacketManager()
    {
        Register();
    }

    #endregion

    
    private Dictionary<ushort, Action<PacketSession, ArraySegment<byte>>> _onRecv 
        = new Dictionary<ushort, Action<PacketSession, ArraySegment<byte>>>();

    private Dictionary<ushort, Action<PacketSession, IPacket>> _handler
        = new Dictionary<ushort, Action<PacketSession, IPacket>>();


    public void Register()
    {
        _onRecv.Add((ushort) PacketID.C_Chat, MakePacket<C_Chat>);
        _handler.Add((ushort) PacketID.C_Chat, PacketHandler.C_ChatHandler);

    }
    
    public void OnRecvPacket(PacketSession session, ArraySegment<byte> buffer)
    {
        ushort count = 0;
        ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
        count += 2;
        ushort id = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
        count += 2;

        if (_onRecv.TryGetValue(id, out var action))
            action.Invoke(session, buffer);
        else
            Console.WriteLine($"packet id not found: {id}");
    }

    void MakePacket<T>(PacketSession session, ArraySegment<byte> buffer) where T : IPacket, new()
    {
        var packet = new T();
        packet.Read(buffer);

        if (_handler.TryGetValue(packet.Protocol, out var action))
            action.Invoke(session, packet);
        else
            Console.WriteLine($"packet handler not found: {packet.GetType()}");
    }
}
