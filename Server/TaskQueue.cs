using System.Collections.Generic;

namespace Server
{
    /// <summary>
    /// JobQueue with Action 과 비교용.
    /// 명시적은 ITask 클래스(Command + Data) 생성해서 Queue 에 넣고 사용.
    /// </summary>
    public interface ITask
    {
        public void Execute();
    }

    class BroadCastTask : ITask
    {
        readonly GameRoom _room;
        readonly ClientSession _session;
        readonly string _chat;

        public BroadCastTask(GameRoom room, ClientSession session, string chat)
        {
            _room = room;
            _session = session;
            _chat = chat;
        }

        public void Execute()
        {
            _room.Broadcast(_session, _chat);
        }
    }

    public class TaskQueue
    {
        readonly Queue<ITask> _queue = new Queue<ITask>();
        readonly object _lock = new object();
        bool _flushing = false;
        
        public void Push(ITask task)
        {
            bool flush = false;
            lock (_lock)
            {
                _queue.Enqueue(task);
                if (_flushing == false)
                    flush = _flushing = true;
            }

            if (flush)
                Flush();
        }

        void Flush()
        {
            while (true)
            {
                var task = Pop();
                if (task == null)
                    return;
                
                task.Execute();
            }
        }

        ITask Pop()
        {
            lock (_lock)
            {
                if (_queue.Count == 0)
                {
                    _flushing = false;
                    return null;
                }
                return _queue.Dequeue();
            }
        }
    }
}