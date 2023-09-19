using System;
using System.Threading;
using System.Threading.Tasks;

namespace MT06_Memory_Barrier
{
    // C# 완벽가이드 소개 예제.

    class ExecutorAlbahariNoneBarriered
    {
        static int _answer; 
        static bool _complete;
        static int _countOfCompleteInB;
        static float _d;
        
        static void A()
        {
            _answer = 123;
            _complete = true;
        }

        static void B()
        {
            if (_complete)
            {
                if (0 ==_answer)
                    Console.WriteLine($"{_answer}");
                ++_countOfCompleteInB;
            }
        }

        /// <summary>
        /// 아래 실행으로는 위 Console.WriteLine($"{_answer}"); 가 출력 안됨..... 
        /// </summary>
        public static void Execute()
        {
            int loopCnt = 10000;

            for (int i = 0; i < loopCnt; ++i)
            {
                _complete = false;
                _answer = 0;

                var t1 = new Task(A);
                var t2 = new Task(B);

                t1.Start();
                t2.Start();

                Task.WaitAll(t1, t2);

                // Console.WriteLine($"ExecutorAlbahariNoneBarriered {i}th : answer: {_answer} // complete : {_complete} ");
            }

            Console.WriteLine($"ExecutorAlbahariNoneBarriered loop: {loopCnt}, print: {_countOfCompleteInB}");
        }
    }
}