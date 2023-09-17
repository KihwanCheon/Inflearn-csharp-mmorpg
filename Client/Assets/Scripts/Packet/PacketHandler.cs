using DummyClient;
using ServerCore;
using UnityEngine;

public class PacketHandler
{
    public static void S_BroadcastEnterGameHandler(PacketSession pSession, IPacket ipkt)
    {
        var session = pSession as ServerSession;
        var pkt = ipkt as S_BroadcastEnterGame;
        if (pkt == null)
            return;
        
        Debug.Log($"player {pkt.playerId} entered");
        PlayerManager.Instance.EnterGame(pkt);
    }

    public static void S_BroadcastLeaveGameHandler(PacketSession pSession, IPacket ipkt)
    {
        var session = pSession as ServerSession;
        var pkt = ipkt as S_BroadcastLeaveGame;
        if (pkt == null)
            return;
        
        Debug.Log($"player {pkt.playerId} left");
        PlayerManager.Instance.LeaveGame(pkt);
    }

    public static void S_PlayerListHandler(PacketSession pSession, IPacket ipkt)
    {
        var session = pSession as ServerSession;
        var pkt = ipkt as S_PlayerList;
        if (pkt == null)
            return;
        
        Debug.Log($"{pkt.player.Count} players exist");
        PlayerManager.Instance.Add(pkt);
    }

    public static void S_BroadcastMoveHandler(PacketSession pSession, IPacket ipkt)
    {
        var session = pSession as ServerSession;
        var pkt = ipkt as S_BroadcastMove;
        if (pkt == null)
            return;
        
        PlayerManager.Instance.Move(pkt);
    }
}
