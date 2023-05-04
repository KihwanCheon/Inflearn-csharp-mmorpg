using System;
using System.Threading;
using System.Threading.Tasks;

namespace CompilerOptimization01
{
    internal class Program
    {
        private static bool _stop = false;

        static void ThreadMain()
        {
            Console.WriteLine("스레드 시작!");

            while (!_stop)
            {
                // wait stop == false
            }

            Console.WriteLine("스레드 종료!");
        }


        static void Main(string[] args)
        {

            var t = new Task(ThreadMain);

            t.Start();

            Thread.Sleep(1000);

            _stop = true;

            Console.WriteLine("Stop 호출");
            Console.WriteLine("종료 대기중");

            t.Wait();
            
            Console.WriteLine("종료 성공!");
        }
    }
}
