using System;
using System.Threading;
using System.Threading.Tasks;

namespace MT12_AutoResetEvent
{
    class Lock
    {
        AutoResetEvent _available = new AutoResetEvent(true);
        
        public void Acquire()
        {
            _available.WaitOne();   // 입장시도.
            // _available.Reset(); // AutoResetEvent 에서는 WaitOne에 포함.
        }

        public void Release()
        {
            _available.Set();
        }
    }


    internal class Program
    {
        private static int _num = 0;
        private static Lock _lock = new Lock();

        private const int LoopCnt = 1000; // AutoResetEvent 는 Interlocked 와 다르게 커널까지 내려가므로 오래걸림.


        static void Thread_01()
        {
            for (int i = 0; i < LoopCnt; i++)
            {
                _lock.Acquire();
                ++_num;
                _lock.Release();
            }
        }

        static void Thread_02()
        {
            for (int i = 0; i < LoopCnt; i++)
            {
                _lock.Acquire();
                --_num;
                _lock.Release();
            }
        }

        static void Main(string[] args)
        {
            var t1 = new Task(Thread_01);
            var t2 = new Task(Thread_02);

            t1.Start();
            t2.Start();

            Task.WaitAll(t1, t2);

            Console.WriteLine($"Hello World! {_num}");
        }
    }
}
