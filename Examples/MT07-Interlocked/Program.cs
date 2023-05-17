using System;
using System.Threading.Tasks;

namespace MT07_Interlocked
{
    class ExecutorSharedVariableWithoutLocks
    {
        static int _number = -0;
        const int LoopCnt = 10000;

        static void Thread_1()
        {
            for (int i = 0; i < LoopCnt; ++i)
                _number++;
        }

        static void Thread_2()
        {
            for (int i = 0; i < LoopCnt; ++i)
                _number--;
        }

        public static void Execute()
        {
            var t1 = new Task(Thread_1);
            var t2 = new Task(Thread_2);

            t1.Start();
            t2.Start();
            
            Task.WaitAll(t1, t2);

            Console.WriteLine(_number); // LoopCnt 가 클수록 이상한 값 출력.
        }
    }


    class Program
    {
        static void Main(string[] args)
        {
            ExecutorSharedVariableWithoutLocks.Execute();
        }
    }
}
