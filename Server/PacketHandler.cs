using System;
using Server;
using ServerCore;


public class PacketHandler
{
    public static void C_ChatHandler(PacketSession session, IPacket packet)
    {
        var clientSession = session as ClientSession;
        var chatPkt = packet as C_Chat;

        if (clientSession?.Room == null)
            return;

        if (chatPkt?.chat == null) return;

        clientSession.Room.Broadcast(clientSession, chatPkt.chat);
    }
}
