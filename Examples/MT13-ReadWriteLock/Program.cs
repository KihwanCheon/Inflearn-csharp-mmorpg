using System;
using System.Threading;

namespace MT13_ReadWriteLock
{
    class Program
    {
        // 락 종류  
        // 1. 근성  Monitor(object _lock, lock(_lock))
        // 2. 양보  SpinLock(계속 시도하다 특정횟수 넘으면 yield)
        // 3. 갑질  Event

        // object _lock, SpinLock는 상호 배제
        static object _lock = new object();
        static SpinLock _lock2 = new SpinLock();

        static ReaderWriterLockSlim _lock3 = new ReaderWriterLockSlim();

        class Reward {}

        // 99.999999 비율로 호출
        static Reward GetReward()
        {
            // lock (_lock)    // 좀 락잡는게 아깝다...
            // {
            //     return null;
            // }

            _lock3.EnterReadLock();

            _lock3.ExitReadLock();
            return null;
        }

        
        // 0.0000001 비율로 호출
        static void AddReward(Reward reward)
        {
            // lock (_lock)
            // {
            // add...
            // }

            _lock3.EnterWriteLock();
            // add...
            _lock3.ExitWriteLock();
        }


        static void Main(string[] args)
        {
            lock (_lock)
            {
            }

            bool lockTaken = false;
            _lock2.Enter(ref lockTaken);
            _lock2.Exit();
        

            Console.WriteLine("Hello World!");
        }
    }
}
