using System;
using System.Collections.Generic;

namespace Server
{
    public class GameRoom
    {
        readonly List<ClientSession> _sessions = new List<ClientSession>();
        readonly object _lock = new object();

        public void Enter(ClientSession session)
        {
            lock (_lock)
            {
                _sessions.Add(session);
                session.Room = this;
            }
        }

        public void Leave(ClientSession session)
        {
            lock (_lock)
            {
                _sessions.Remove(session); 
            }
        }

        public void Broadcast(ClientSession session, string chat)
        {
            var packet = new S_Chat { playerId = session.SessionId, chat = $"{chat} I am {session.SessionId}" };
            var segment = packet.Write();

            // Console.WriteLine(packet.chat);

            lock (_lock)
            {
                foreach (var s in _sessions)
                    s.Send(segment);
            }
        }
    }
}