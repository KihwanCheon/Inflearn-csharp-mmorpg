using System;
using System.Threading;

namespace Thread01
{
    class Program
    {
        static void MainThread()
        {
            while(true)
            //for (int i = 0; i < 5; ++i)
                Console.WriteLine("Hello Thread!");
        }

        static void Main(string[] args)
        {
            Thread t = new Thread(MainThread);
            t.Name = "Test Thread";
            t.IsBackground = true; // 메인스레드 종료시 스레드도 종료.
            t.Start();
            
            Console.WriteLine("Waiting for Thread");
            t.Join();
            Console.WriteLine("Hello World!");

            while (true)
            {

            }
        }
    }
}
