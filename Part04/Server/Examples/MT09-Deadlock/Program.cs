using System;
using System.Threading.Tasks;

namespace MT09_Deadlock
{
    class SessionManager
    {
        private static readonly object Lock = new object();

        public static void Test()
        {
            lock (Lock)
            {
                // 이것 저것 하다가 유저도 테스트 했다하자.
                UserManager.TestUser();
            }
        }

        public static void TestSession()
        {
            lock (Lock)
            {
                // 
            }
        }
    }

    class UserManager
    {
        private static readonly object Lock = new object();

        public static void Test()
        {
            lock (Lock)
            {
                // 이것 저것 하다가 세션도 테스트 했다하자.
                SessionManager.TestSession();
            }
        }

        public static void TestUser()
        {
            lock (Lock)
            {
                //
            }
        }
    }
    
    class Program
    {
        private const int LoopCnt = 10000;

        static void Thread_1()
        {
            for (int i = 0; i < LoopCnt; ++i)
            {
                SessionManager.Test();
            }
        }

        static void Thread_2()
        {
            for (int i = 0; i < LoopCnt; ++i)
            {
                UserManager.Test();
            }
        }

        static void Main(string[] args)
        {
            var t1 = new Task(Thread_1);
            var t2 = new Task(Thread_2);

            t1.Start();
            t2.Start();

            Task.WaitAll(t1, t2);

            Console.WriteLine("end!");
        }
    }
}
