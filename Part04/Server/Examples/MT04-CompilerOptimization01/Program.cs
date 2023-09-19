using System;
using System.Threading;
using System.Threading.Tasks;

/*
 * Release 모드로 실행하면서 비정상적인 동작 확인.
 */
namespace CompilerOptimization01
{
    internal class Program
    {
        /*
          volatile를 붙이면 최적화 하지 않아서 Release 모드에서도 정상동작하나 권장하지는 않음.
          volatile 최적화 하지 말고 캐시무시 말고 최신값을 가져다 사용하라는 의미, C#에서 이상하게 동작하니(?) 사용하지 않기를 권함. 
          volatile c++ 에도 있으나 의미가 다름. 
        */
        // volatile
        static bool _stop = false;


        static void ThreadMain()
        {
            Console.WriteLine("스레드 시작!");

            // Release 모드로 컴파일하면 최적화 됨. 메모리에서 한번만 가져와서 그 값만 비교 반복함.
            // Debug 모드로 가져오면, 메모리에서 매번 가져와서 비교함.
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
