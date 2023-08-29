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

        public void SendForEach()
        {
            lock (_lock)
            {
                foreach (var session in _sessions)
                {
                    var chatPacket = new C_Chat { chat = $"Hello Server! {session.Id}" };

                    var segment = chatPacket.Write();
                    session.Send(segment);
                }
            }
        }
    }
}