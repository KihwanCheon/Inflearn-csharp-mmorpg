using System.Collections.Generic;
using UnityEngine;

public class PlayerManager
{
    MyPlayer _myPlayer;
    readonly Dictionary<int, Player> _players = new Dictionary<int, Player>();

    public static PlayerManager Instance { get; set; } = new PlayerManager();

    public void Add(S_PlayerList packet)
    {
        var obj = Resources.Load("Player");
        
        foreach (var p in packet.player)
        {
            var go = Object.Instantiate(obj) as GameObject;

            Player player;
            if (p.isSelf)
            {
                _myPlayer = go.AddComponent<MyPlayer>();
                player = _myPlayer;
            }
            else
            {
                player = go.AddComponent<Player>();
                _players.Add(p.playerId, player);
            }

            player.transform.position = new Vector3(p.posX, p.posY, p.posZ);
            player.PlayerId = p.playerId;
        }
    }

    public void EnterGame(S_BroadcastEnterGame pkt)
    {
        if (_myPlayer?.PlayerId == pkt.playerId)
        {
            Debug.LogWarning($"myPlayer {pkt.playerId} is already entered");
            return;
        }

        if (_players.ContainsKey(pkt.playerId))
        {
            Debug.LogWarning($"player {pkt.playerId} is already entered");
            return;
        }

        var obj = Resources.Load("Player");
        var go = Object.Instantiate(obj) as GameObject;

        var player = go.AddComponent<Player>();
        
        player.transform.position = new Vector3(pkt.posX, pkt.posY, pkt.posZ);
        player.PlayerId = pkt.playerId;
        _players.Add(pkt.playerId, player);
    }

    public void LeaveGame(S_BroadcastLeaveGame pkt)
    {
        if (pkt.playerId == _myPlayer?.PlayerId)
        {
            Object.Destroy(_myPlayer.gameObject);
            _myPlayer = null;
        }
        else if (_players.TryGetValue(pkt.playerId, out var player))
        {
            Object.Destroy(player.gameObject);
            _players.Remove(pkt.playerId);
        }
        else
        {
            Debug.LogWarning($"player {pkt.playerId} is already left");
        }
    }

    public void Move(S_BroadcastMove pkt)
    {
        if (_myPlayer?.PlayerId == pkt.playerId)
        {
            _myPlayer.transform.position = new Vector3(pkt.posX, pkt.posY, pkt.posZ);
        }
        else if (_players.TryGetValue(pkt.playerId, out var player))
        {
            player.transform.position = new Vector3(pkt.posX, pkt.posY, pkt.posZ);
        }
        else
        {
            Debug.LogWarning($"player {pkt.playerId} is already entered");
        }
    }
}
