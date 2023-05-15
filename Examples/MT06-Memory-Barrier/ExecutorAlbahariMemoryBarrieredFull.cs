using System;
using System.Threading;
using System.Threading.Tasks;

namespace MT06_Memory_Barrier
{
    // C# 완벽가이드 소개 예제.
    // https://www.albahari.com/threading/part4.aspx

    class ExecutorAlbahariMemoryBarrieredFull
    {
        static int _answer; 
        static bool _complete;
        static int _countOfCompleteInB;
        static float _d;
        
        static void A()
        {
            _answer = 123;
            Thread.MemoryBarrier();     // barrier 1
            _complete = true;
            Thread.MemoryBarrier();     // barrier 2
        }

        static void B()
        {
            Thread.MemoryBarrier();     // barrier 3    // 동시 실행시키면 barrier 2 보다 먼저 막아서 false 인 경우에만 실행 안되는 듯...
            if (_complete)
            {
                Thread.MemoryBarrier(); // barrier 4
                // Console.WriteLine(_answer);
                if (0 == _answer)
                    Console.WriteLine($"{_answer}");

                // Console.WriteLine($"{_d} = 1 / {_answer}");
                ++_countOfCompleteInB;

            }
        }

        public static void Execute()
        {
            int loopCnt = 10000;

            for (int i = 0; i < loopCnt; ++i)
            {
                Thread.MemoryBarrier();
                _answer = 0;
                _complete = false;
                
                var t1 = new Task(A);
                var t2 = new Task(B);

                t1.Start();
                t2.Start();

                Task.WaitAll(t1, t2);

                // Console.WriteLine($"ExecutorAlbahariMemoryBarrieredFull {i}th : answer: {_answer} // complete : {_complete} ");
            }

            Console.WriteLine($"ExecutorAlbahariMemoryBarrieredFull loop: {loopCnt}, print: {_countOfCompleteInB}");
        }
    }
}