using System;
using Server;
using ServerCore;


public class PacketHandler
{
    public static void C_LeaveGameHandler(PacketSession pSession, IPacket ipkt)
    {
        var session = pSession as ClientSession;
        if (session?.Room == null)
            return;
        
        var room = session.Room;
        room.Push(() => room.Leave(session));
    }

    public static void C_MoveHandler(PacketSession pSession, IPacket ipkt)
    {
        var session = pSession as ClientSession;
        if (session?.Room == null)
            return;

        var movePkt = ipkt as C_Move;
        if (movePkt == null)
            return;

        Console.WriteLine($"player {session.SessionId} move ({movePkt.posX}, {movePkt.posY}, {movePkt.posZ})");

        var room = session.Room;
        room.Push( () => room.Move(session, movePkt));
    }
}
