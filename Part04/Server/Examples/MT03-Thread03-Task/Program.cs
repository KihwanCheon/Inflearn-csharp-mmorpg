using System;
using System.Threading;
using System.Threading.Tasks;

namespace Thread03_Task
{
    internal class Program
    {

        static void MainThreadForPool(object state)
        {
            var currTrd = Thread.CurrentThread;

            for (int i = 0; i < 5; ++i)
            {
                Console.WriteLine($"Hello {state}. ThreadPool!" +
                                  $" ThreadId: {currTrd.ManagedThreadId}" +
                                  $", isThreadPoolThread: {currTrd.IsThreadPoolThread}" +
                                  $", isBackground: {currTrd.IsBackground} ");
            }
        }

        static void Main(string[] args)
        {
            const int maxThreadCnt = 5;
            ThreadPool.SetMinThreads(1, 1);
            ThreadPool.SetMaxThreads(maxThreadCnt, maxThreadCnt);

            for (int i = 0; i < maxThreadCnt; ++i)        // 오래 걸리는 작업으로 스레드 풀을 점유한다.
            {
                var t = new Task(() => { while (true) { } }, TaskCreationOptions.LongRunning); // 스레드 풀에서 스레드 가져다 쓰지 않음.
                // var t = new Task(() => { while (true) { } });  // 스레드 풀에서 스레드 가져다 씀.
                t.Start();
            }

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
