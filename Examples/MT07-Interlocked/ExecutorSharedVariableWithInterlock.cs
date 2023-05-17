using System;
using System.Threading;
using System.Threading.Tasks;

namespace MT07_Interlocked
{
    class ExecutorSharedVariableWithInterlock
    {
        int _number = -0;
        const int LoopCnt = 10000;

        void Thread_1()
        {
            for (int i = 0; i < LoopCnt; ++i)
                Interlocked.Increment(ref _number);
        }

        void Thread_2()
        {
            for (int i = 0; i < LoopCnt; ++i)
                Interlocked.Decrement(ref _number);
        }

        public void Execute()
        {
            var t1 = new Task(Thread_1);
            var t2 = new Task(Thread_2);

            t1.Start();
            t2.Start();
            
            Task.WaitAll(t1, t2);

            Console.WriteLine($"{GetType().Name} {_number}"); // LoopCnt가 커도 항상 0 출력.
        }
    }
}