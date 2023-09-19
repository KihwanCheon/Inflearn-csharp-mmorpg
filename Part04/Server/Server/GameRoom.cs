using System;
using System.Collections.Generic;
using ServerCore;

namespace Server
{
    public class GameRoom : IJobQueue
    {
        readonly List<ClientSession> _sessions = new List<ClientSession>();
        readonly JobQueue _jobQueue = new JobQueue();
        readonly List<ArraySegment<byte>> _pendingList = new List<ArraySegment<byte>>();

        public void Push(Action job)
        {
            _jobQueue.Push(job);
        }

        public void Flush()
        {
            if (_pendingList.Count == 0)
                return;

            foreach (var s in _sessions)
                s.Send(_pendingList);

            // Console.WriteLine($"Flushed {_pendingList.Count} items");
            _pendingList.Clear();
        }

        public void Broadcast(ArraySegment<byte> segment)
        {
            _pendingList.Add(segment);
        }

        public void Enter(ClientSession session)
        {
            // add player
            _sessions.Add(session);
            session.Room = this;

            // send to newbie list of players
            var players = new S_PlayerList();
            foreach (var s in _sessions)
            {
                players.player.Add(new S_PlayerList.Player
                {
                    isSelf = s == session, playerId = s.SessionId, posX = s.PosX, posY = s.PosY, posZ = s.PosZ
                });
            }

            session.Send(players.Write());

            // send to all player what newbie entered
            var enter = new S_BroadcastEnterGame
            {
                playerId = session.SessionId, posX = 0, posY = 0, posZ = 0
            };

            Broadcast(enter.Write());
        }

        public void Leave(ClientSession session)
        {
            // remove player
            _sessions.Remove(session);

            // send to all players that a player left
            var leave = new S_BroadcastLeaveGame { playerId = session.SessionId };
            Broadcast(leave.Write());
        }

        public void Move(ClientSession session, C_Move inPkt)
        {
            session.PosX = inPkt.posX;
            session.PosY = inPkt.posY;
            session.PosZ = inPkt.posZ;

            // send to all
            var move = new S_BroadcastMove
            {
                playerId = session.SessionId, posX = inPkt.posX, posY = inPkt.posY, posZ = inPkt.posZ
            };
            Broadcast(move.Write());
        }
    }
}