using System;
using System.Threading;

namespace Thread02_ThreadPool
{
    class Program
    {
        static void MainThreadForPool(object state)
        {
            var currTrd = Thread.CurrentThread;

            for (int i = 0; i < 5; ++i)
            {
                Console.WriteLine($"Hello {state}. ThreadPool!" +
                                  $" ThreadId: {currTrd.ManagedThreadId}" +
                                  $", isThreadPoolThread: {currTrd.IsThreadPoolThread}" +
                                  $", isBackground: {currTrd.IsBackground} " );
            }
        }

        static void Main(string[] args)
        {
            const int maxThreadCnt = 5;
            ThreadPool.SetMinThreads(1, 1);
            ThreadPool.SetMaxThreads(maxThreadCnt, maxThreadCnt);

            for (int i = 0; i < maxThreadCnt -1; ++i)        // 오래 걸리는 작업으로 스레드 풀을 점유한다.
                ThreadPool.QueueUserWorkItem((obj) => { while (true) { } });

            ThreadPool.QueueUserWorkItem(MainThreadForPool, "1"); // IsBackground.
            ThreadPool.QueueUserWorkItem(MainThreadForPool, "2"); // IsBackground.
            ThreadPool.QueueUserWorkItem(MainThreadForPool, "3"); // IsBackground.
            
            Console.WriteLine("Hello World!");

            while (true)
            {

            }
        }
    }
}
