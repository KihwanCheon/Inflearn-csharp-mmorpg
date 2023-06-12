using System;
using System.Threading;
using System.Threading.Tasks;

namespace MT15_ThreadLocalStorage
{
    internal class Program
    {
        // private static string ThreadName;
        private static ThreadLocal<string> ThreadName = new ThreadLocal<string>();
        
        static void WhoAmI()
        {
            // ThreadName = $"My Name is {Thread.CurrentThread.ManagedThreadId}";
            ThreadName.Value = $"My Name is {Thread.CurrentThread.ManagedThreadId}";


            Thread.Sleep(1000);
            // Console.WriteLine(ThreadName);
            Console.WriteLine(ThreadName.Value);
        }


        static void Main(string[] args)
        {
            ThreadPool.SetMinThreads(1, 1);
            ThreadPool.SetMaxThreads(3, 3);
            Parallel.Invoke(WhoAmI, WhoAmI, WhoAmI, WhoAmI, WhoAmI, WhoAmI);
        }
    }
}
