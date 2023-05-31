using System;
using System.Threading;
using System.Threading.Tasks;

namespace MT12_AutoResetEvent
{
    class Lock
    {
        // ResetEvent 들은 커널모드라서 느림.
        AutoResetEvent _available = new AutoResetEvent(true);
        // ManualResetEvent _available = new ManualResetEvent(true);    // _available.Reset() 이 필요함. WaitOne, Reset 이 한번에 원자적으로 동작하지 않음.
        // ManualResetEvent _available = new ManualResetEvent(false);   // 대기중인 스레드를 동시 실행시키는 등의 용도로 활용 // https://learn.microsoft.com/ko-kr/dotnet/api/system.threading.manualresetevent?view=net-7.0

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
