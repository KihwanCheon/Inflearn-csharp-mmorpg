

using System;
using System.Collections.Generic;
using ServerCore;


public class PacketManager
{
    #region Singleton
    private static PacketManager _instance;
    public static PacketManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = new PacketManager();
            return _instance;
            ;
        }
    }
    #endregion

    
    private Dictionary<ushort, Action<PacketSession, ArraySegment<byte>>> _onRecv 
        = new Dictionary<ushort, Action<PacketSession, ArraySegment<byte>>>();

    private Dictionary<ushort, Action<PacketSession, IPacket>> _handler
        = new Dictionary<ushort, Action<PacketSession, IPacket>>();


    public void Register()
    {
                _onRecv.Add((ushort) PacketID.PlayerInfoReq, MakePacket<PlayerInfoReq>);
        _handler.Add((ushort) PacketID.PlayerInfoReq, PacketHandler.PlayerInfoReqHandler);
        _onRecv.Add((ushort) PacketID.Test, MakePacket<Test>);
        _handler.Add((ushort) PacketID.Test, PacketHandler.TestHandler);

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
