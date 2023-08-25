using System;
using ServerCore;


public class PacketHandler
{
    public static void C_PlayerInfoReqHandler(PacketSession session, IPacket packet)
    {
        var req = packet as C_PlayerInfoReq;

        Console.WriteLine($"PlayerInfoReq : {req.playerId}, {req.name}");

        foreach (var skill in req.skills)
        {
            Console.WriteLine($"skill {skill.id}, {skill.level}, {skill.duration}, attrs: {skill.attrs.Count}, (fire: {(skill.attrs.Count > 0 ? skill.attrs[0].fire : 0)})");
        }
    }
}
