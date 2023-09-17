using Server;
using ServerCore;


public class PacketHandler
{
    public static void C_LeaveGameHandler(PacketSession session, IPacket packet)
    {
        var clientSession = session as ClientSession;
        if (clientSession?.Room == null)
            return;
        
        var room = clientSession.Room;
        room.Push(() => room.Leave(clientSession));
    }

    public static void C_MoveHandler(PacketSession session, IPacket packet)
    {
        var clientSession = session as ClientSession;
        if (clientSession?.Room == null)
            return;

        var movePkt = packet as C_Move;
        var room = clientSession.Room;

        room.Push( () => room.Move(clientSession, movePkt));
    }
}
