using System;
using System.Threading.Tasks;

namespace MT08_Lock_101
{
    class ExecutorWithLock
    {
        static int _number = 0;
        static object _obj = new object();
        const int LoopCnt = 10000;

        static void Thread_1()
        {
            for (int i = 0; i < LoopCnt; ++i)
            {
                lock (_obj) // 잠금. // This uses Monitor internally.
                {
                    _number++;
                }
            }
        }

        static void Thread_2()
        {
            for (int i = 0; i < LoopCnt; ++i)
            {
                lock(_obj) // 잠금.
                    _number--;
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