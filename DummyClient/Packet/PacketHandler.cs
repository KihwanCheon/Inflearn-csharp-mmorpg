using System;
using DummyClient;
using ServerCore;

public class PacketHandler
{
    public static void S_ChatHandler(PacketSession session, IPacket packet)
    {
        var chatPacket = packet as S_Chat;
        var serverSession = session as ServerSession;

        // if (chatPacket?.playerId == 1)
            // Console.WriteLine($"{serverSession?.Id}] {chatPacket?.chat}");
    }
}
