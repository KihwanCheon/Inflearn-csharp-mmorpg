using System;
using DummyClient;
using ServerCore;

public class PacketHandler
{
    public static void S_BroadcastEnterGameHandler(PacketSession pSession, IPacket ipkt)
    {
        var pkt = ipkt as S_BroadcastEnterGame;
        var session = pSession as ServerSession;
    }

    public static void S_BroadcastLeaveGameHandler(PacketSession pSession, IPacket ipkt)
    {
        var pkt = ipkt as S_BroadcastLeaveGame;
        var session = pSession as ServerSession;
    }

    public static void S_PlayerListHandler(PacketSession pSession, IPacket ipkt)
    {
        var pkt = ipkt as S_PlayerList;
        var session = pSession as ServerSession;
    }

    public static void S_BroadcastMoveHandler(PacketSession pSession, IPacket ipkt)
    {
        var pkt = ipkt as S_BroadcastMove;
        var session = pSession as ServerSession;
    }
}
