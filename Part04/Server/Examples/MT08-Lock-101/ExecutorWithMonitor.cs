using System;
using System.Threading;
using System.Threading.Tasks;

namespace MT08_Lock_101
{
    class ExecutorWithMonitor
    {
        static int _number = 0;
        static object _obj = new object();
        const int LoopCnt = 10000;

        static void Thread_1()
        {
            for (int i = 0; i < LoopCnt; ++i)
            {
                try // Monitor 를 사용하면. early-return 이나 중간에 발생하는 예외로 잠금하지 못하는 경우를 대비해야함.
                {
                    Monitor.Enter(_obj); // 잠금.
                    {
                        _number++;

                        if (false) // try-finally 로 감싸지 않고, true 면 다음번 Monitor.Enter를 못해서 프로그램 잠김.
                            return; // 
                    }
                }
                finally
                {
                    Monitor.Exit(_obj); // 잠금해제.
                }
            }
        }

        static void Thread_2()
        {
            for (int i = 0; i < LoopCnt; ++i)
            {
                Monitor.Enter(_obj); // 잠금.

                _number--;

                Monitor.Exit(_obj); // 잠금 해제.
            }
        }

        public static void Exec()
        {
            var t1 = new Task(Thread_1);
            var t2 = new Task(Thread_2);

            t1.Start();
            t2.Start();

            Task.WaitAll(t1, t2);

            Console.WriteLine($"{_number}");
        }
    }
}