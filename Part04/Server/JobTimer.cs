using System;
using ServerCore;

namespace Server
{

    struct JobTimerElement : IComparable<JobTimerElement>
    {
        public int ExecTick;// 실행 시각.
        public Action Action;

        public int CompareTo(JobTimerElement other)
        {
            return other.ExecTick - ExecTick;
        }
    }
    public class JobTimer
    {
        PriorityQueue<JobTimerElement> _pq = new PriorityQueue<JobTimerElement>();
        object _lock = new object();

        public static JobTimer Instance { get; } = new JobTimer();

        public void Push(Action action, int tickAfter = 0)
        {
            JobTimerElement job;
            job.ExecTick = Environment.TickCount + tickAfter;
            job.Action = action;

            lock (_lock)
            {
                _pq.Push(job);
            }
        }

        public void Flush()
        {
            while (true)
            {
                int now = Environment.TickCount;
                JobTimerElement job;
                lock (_lock)
                {
                    if (_pq.Count == 0)
                        break;

                    job = _pq.Peek();
                    if (job.ExecTick > now)
                        break;

                    _pq.Pop();
                }

                job.Action.Invoke();
            }
        }
    }
}