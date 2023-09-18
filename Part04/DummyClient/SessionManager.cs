using System;
using System.Collections.Generic;

namespace DummyClient
{
    public class SessionManager
    {
        public static SessionManager Instance { get; } = new SessionManager();

        readonly List<ServerSession> _sessions = new List<ServerSession>();

        readonly object _lock = new object();

        public ServerSession Generate()
        {
            lock (_lock)
            {
                var session = new ServerSession(_sessions.Count + 1);
                _sessions.Add(session);
                return session;
            }
        }

        readonly Random random = new Random();

        public void SendForEach()
        {
            lock (_lock)
            {
                foreach (var session in _sessions)
                {
                    var move = new C_Move
                    {
                        posX = random.Next(-50, 50),
                        posY = 0,
                        posZ = random.Next(-50, 50),
                    };
                    session.Send(move.Write());
                }
            }
        }
    }
}