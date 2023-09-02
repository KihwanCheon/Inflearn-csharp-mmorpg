using System;
using System.Collections;
using System.Collections.Generic;

namespace ServerCore
{

    public interface IJobQueue
    {
        void Push(Action job);
    }

    public class JobQueue: IJobQueue
    {
        readonly Queue<Action> _jobQueue = new Queue<Action>();
        readonly object _lock = new object();
        bool _flushing = false;

        public void Push(Action job)
        {
            bool flush = false;

            lock (_lock)
            {
                _jobQueue.Enqueue(job);
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
                var job = Pop();
                if (job == null)
                    return;

                job.Invoke();
            }
        }

        Action Pop()
        {
            lock (_lock)
            {
                if (_jobQueue.Count == 0)
                {
                    _flushing = false;
                    return null;
                }

                return _jobQueue.Dequeue();
            }
        }
    }
}