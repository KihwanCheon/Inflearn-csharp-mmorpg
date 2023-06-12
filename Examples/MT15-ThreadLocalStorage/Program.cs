using System;
using System.Threading;
using System.Threading.Tasks;

namespace MT15_ThreadLocalStorage
{
    internal class Program
    {
        
        private static ThreadLocal<string> ThreadName = new ThreadLocal<string>(() => {
            return $"My Name is {Thread.CurrentThread.ManagedThreadId}";
        });
        
        static void WhoAmI()
        {
            if (ThreadName.IsValueCreated)
                Console.WriteLine($"{ThreadName.Value} repeated");
            else
                Console.WriteLine(ThreadName.Value);
        }


        static void Main(string[] args)
        {
            ThreadPool.SetMinThreads(1, 1);
            ThreadPool.SetMaxThreads(3, 3);
            Parallel.Invoke(WhoAmI, WhoAmI, WhoAmI, WhoAmI, WhoAmI, WhoAmI);

            ThreadName.Dispose();
        }
    }
}
