using System;
using System.Threading;
using System.Threading.Tasks;

namespace MT06_Memory_Barrier
{
    // 메모리 배리어
    // A. 코드 재배치 억제
    // B. 가시성

    // 1) Full Memory Barrier (ASM MFENCE, C# Thread.MemoryBarrier) : Store/Load 둘 다 막음.
    // 2) Store Memory Barrier (ASM SFENCE) : Store만 막음.
    // 3) Load Memory Barrier (ASM LFENCE) : Load만 막음.

    /**
     * r1, r2에 메모리에서 로드해서 대입하는 부분 코드 재배치가 Thread.MemoryBarrier(); 로 막혀서 r1 == 0 && r2 == 0 상태가 만들어지지 않음.
     */
    class ExecutorMemoryBarriered
    {
        static int x = 0;
        static int y = 0;
        static int r1 = 0;
        static int r2 = 0;
        
        static void Thread_1()
        {
            y = 1;      // store y
            
            //-----------------------------------------------
            Thread.MemoryBarrier();

            r1 = x;     // load x
        }

        static void Thread_2()
        {
            x = 1;      // store x

            //-----------------------------------------------
            Thread.MemoryBarrier();

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

            Console.WriteLine($"ExecutorMemoryBarriered {count}번 만에 빠져나옴"); // This line will not be touched.
        }
    }
}