﻿using System;
using DummyClient;
using ServerCore;

public class PacketHandler
{
    public static void S_BroadcastEnterGameHandler(PacketSession pSession, IPacket ipkt)
    {
        var session = pSession as ServerSession;

        if (ipkt is S_BroadcastEnterGame pkt)
            Console.WriteLine($"player {pkt.playerId} entered");
    }

    public static void S_BroadcastLeaveGameHandler(PacketSession pSession, IPacket ipkt)
    {
        var session = pSession as ServerSession;

        if (ipkt is S_BroadcastLeaveGame pkt)
            Console.WriteLine($"player {pkt.playerId} left");
    }

    public static void S_PlayerListHandler(PacketSession pSession, IPacket ipkt)
    {
        var session = pSession as ServerSession;

        if (ipkt is S_PlayerList pkt)
            Console.WriteLine($"{pkt.player.Count} players exist");
    }

    public static void S_BroadcastMoveHandler(PacketSession pSession, IPacket ipkt)
    {
        var session = pSession as ServerSession;
        var pkt = ipkt as S_BroadcastMove;
        // if (pkt != null)
        //     Console.WriteLine($"player {pkt.playerId} move ({pkt.posX}, {pkt.posY}, {pkt.posZ})");// if (pkt != null)
        //     Console.WriteLine($"player {pkt.playerId} move ({pkt.posX}, {pkt.posY}, {pkt.posZ})");
    }
}
