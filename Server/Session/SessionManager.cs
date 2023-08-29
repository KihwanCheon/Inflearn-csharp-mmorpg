using System;
using System.Collections.Generic;

namespace Server
{
    public class SessionManager
    {
        public static SessionManager Instance { get; } = new SessionManager();

        int _sessionId;
        readonly Dictionary<int, ClientSession> _sessions = new Dictionary<int, ClientSession>();
        readonly object _lock = new object();

        public ClientSession Generate()
        {
            lock (_lock)
            {
                var ret = new ClientSession(++_sessionId);
                _sessions.Add(ret.SessionId, ret);
                Console.WriteLine($"Connected: {ret.SessionId}");
                return ret; 
            }
        }

        public ClientSession Find(int sessionId)
        {
            lock (_lock)
            {
                return _sessions.TryGetValue(sessionId, out var session) 
                    ? session : null;
            }
        }

        public void Remove(ClientSession session)
        {
            lock (_lock)
            {
                _sessions.Remove(session.SessionId);
            }
        }
    }
}