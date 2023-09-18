using System.Collections.Generic;
using System.Linq;

public class PacketQueue
{
    public static PacketQueue Instance { get; } = new PacketQueue();

    readonly Queue<IPacket> _packets = new Queue<IPacket>();
    readonly object _lock = new object();

    public void Push(IPacket packet)
    {
        lock (_lock)
        {
            _packets.Enqueue(packet);
        }
    }

    public IPacket Pop()
    {
        lock (_lock)
        {
            if (_packets.Count == 0)
                return null;

            return _packets.Dequeue();
        }
    }

    public List<IPacket> PopAll()
    {
        lock (_lock)
        {
            if (_packets.Count == 0)
                return new List<IPacket>();
            
            var list = new List<IPacket>(_packets);
            _packets.Clear();
            return list;
        }
    }
}