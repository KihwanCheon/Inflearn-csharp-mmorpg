using DummyClient;
using ServerCore;
using UnityEngine;

public class PacketHandler
{
    public static void S_ChatHandler(PacketSession session, IPacket packet)
    {
        var chatPacket = packet as S_Chat;
        var serverSession = session as ServerSession;

        // if (chatPacket?.playerId == 1)
        {
            Debug.Log($"{ serverSession?.Id}] { chatPacket?.chat}");

            var go = GameObject.Find("Player"); // not working in async thread(packet handling)
            
            if (go == null)
                Debug.Log($" Player not found");
            else
                Debug.Log($" Player found");
        }
    }
}
