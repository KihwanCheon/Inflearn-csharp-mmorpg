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

    
    readonly Dictionary<ushort, Func<PacketSession, ArraySegment<byte>, IPacket>> _onRecv =
        new Dictionary<ushort, Func<PacketSession, ArraySegment<byte>, IPacket>>();

    readonly Dictionary<ushort, Action<PacketSession, IPacket>> _handler =
        new Dictionary<ushort, Action<PacketSession, IPacket>>();


    public void Register()
    {
        _onRecv.Add((ushort) PacketID.C_Chat, MakePacket<C_Chat>);
        _handler.Add((ushort) PacketID.C_Chat, PacketHandler.C_ChatHandler);

    }
    
    public void OnRecvPacket(PacketSession session
        , ArraySegment<byte> buffer
        , Action<PacketSession, IPacket> onRecvCallback = null)
    {
        ushort count = 0;
        ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
        count += 2;
        ushort id = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
        count += 2;

        if (_onRecv.TryGetValue(id, out var func))
        {
            var packet = func.Invoke(session, buffer);
            if (onRecvCallback != null)
                onRecvCallback.Invoke(session, packet);
            else
                HandlePacket(session, packet);
        }
        else
            Console.WriteLine($"packet id not found: {id}");
    }

    T MakePacket<T>(PacketSession session, ArraySegment<byte> buffer) where T : IPacket, new()
    {
        var packet = new T();
        packet.Read(buffer);
        
        return packet;
    }

    public void HandlePacket(PacketSession session, IPacket packet)
    {
        if (_handler.TryGetValue(packet.Protocol, out var action))
            action.Invoke(session, packet);
        else
            Console.WriteLine($"packet handler not found: {packet.GetType()}");
    }
}
