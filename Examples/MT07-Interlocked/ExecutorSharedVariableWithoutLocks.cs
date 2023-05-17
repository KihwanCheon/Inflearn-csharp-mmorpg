using System;
using System.Threading.Tasks;

namespace MT07_Interlocked
{
    public class ExecutorSharedVariableWithoutLocks
    {
        int _number = -0;
        const int LoopCnt = 10000;

        void Thread_1()
        {
            for (int i = 0; i < LoopCnt; ++i)
                _number++;
        }

        void Thread_2()
        {
            for (int i = 0; i < LoopCnt; ++i)
                _number--;
        }

        public void Execute()
        {
            var t1 = new Task(Thread_1);
            var t2 = new Task(Thread_2);

            t1.Start();
            t2.Start();
            
            Task.WaitAll(t1, t2);

            Console.WriteLine($"{GetType().Name} {_number}"); // LoopCnt 가 클수록 이상한 값 출력.
        }
    }
}