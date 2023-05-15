using System;
using System.Threading.Tasks;

namespace MT06_Memory_Barrier
{
    /**
     * r1, r2에 메모리에서 로드해서 대입하는 부분이 코드 재배치가 이루어져 r1 == 0 && r2 == 0 상태가 만들어진다
     */
    class HardwareOptimizedExecutor
    {
        static int x = 0;
        static int y = 0;
        static int r1 = 0;
        static int r2 = 0;

        // volatile 를 붙이던 안붙이던 상관없이 r1 == 0 && r2 == 0 상태가 만들어짐
        /*
        static volatile int x = 0;
        static volatile int y = 0;
        static volatile int r1 = 0;
        static volatile int r2 = 0;
        */

        static void Thread_1()
        {
            y = 1;      // store y
            r1 = x;     // load x
        }

        static void Thread_2()
        {
            x = 1;      // store x
            r2 = y;     // load y
        }

        public static void Execute()
        {
            int count = 0;
            while (true)
            {
                ++count;
                x = y = r1 = r2 = 0;

                var t1 = new Task(Thread_1);
                var t2 = new Task(Thread_2);

                t1.Start();
                t2.Start();

                Task.WaitAll(t1, t2);

                if (r1 == 0 && r2 == 0)
                    break;
            }

            Console.WriteLine($"HardwareOptimizedExecutor {count}번 만에 빠져나옴");
        }
    }
}