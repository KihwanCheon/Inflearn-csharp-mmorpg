using System;
using System.Threading;
using System.Threading.Tasks;

namespace MT11_ContextSwitching
{
    class SpinLock
    {
        private volatile int _locked = 0;
        

        public void Acquire()
        {
            while (true)
            {
                const int expected = 0, desired = 1;
                if (Interlocked.CompareExchange(ref _locked, desired, expected) == expected)
                    break;

                // Context switching!
                Thread.Sleep(1);    // 무조건 휴식 => 1ms 정도? 그보다 더 쉴수 있음.
                // Thread.Sleep(0);    // 조건부 양보 => priority 가 우선인 작업이 없으면 다시 스레드를 점유해서 사용.
                // Thread.Yield();     // 관대한 양보 => 실행 가능한 작업있으면 스레드 양보.

            }
        }

        public void Release()
        {
            _locked = 0;
        }
    }


    internal class Program
    {
        private static int _num = 0;
        private static SpinLock _lock = new SpinLock();

        private const int LoopCnt = 1000000;


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
