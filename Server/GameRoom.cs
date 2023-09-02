using System;
using System.Collections.Generic;
using ServerCore;

namespace Server
{
    public class GameRoom: IJobQueue
    {
        readonly List<ClientSession> _sessions = new List<ClientSession>();
        
        readonly JobQueue _jobQueue = new JobQueue();
        readonly TaskQueue _taskQueue = new TaskQueue();

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

            // Console.WriteLine(packet.chat);

            foreach (var s in _sessions)
                s.Send(segment);
        }

        public void Push(Action job)
        {
            _jobQueue.Push(job);
        }

        public void Push(ITask task)
        {
            _taskQueue.Push(task);
        }
    }
}