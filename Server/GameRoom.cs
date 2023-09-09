using System;
using System.Collections.Generic;
using ServerCore;

namespace Server
{
    public class GameRoom: IJobQueue
    {
        readonly List<ClientSession> _sessions = new List<ClientSession>();
        readonly JobQueue _jobQueue = new JobQueue();
        readonly List<ArraySegment<byte>> _pendingList = new List<ArraySegment<byte>>();

        public void Enter(ClientSession session)
        {
            _sessions.Add(session);
            session.Room = this;
        }

        public void Leave(ClientSession session)
        {
            _sessions.Remove(session); 
        }

        public void Broadcast(ClientSession session, string chat)
        {
            var packet = new S_Chat { playerId = session.SessionId, chat = $"{chat} I am {session.SessionId}" };
            var segment = packet.Write();

            _pendingList.Add(segment);
        }

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

            Console.WriteLine($"Flushed {_pendingList.Count} items");
            _pendingList.Clear();
        }
    }
}