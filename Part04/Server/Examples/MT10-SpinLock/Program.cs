﻿using System;
using System.Threading;
using System.Threading.Tasks;

namespace MT10_SpinLock
{
    class SpinLock
    {
        private volatile int _locked = 0;
        private readonly bool UseExchange = false;

        public void Acquire()
        {
            while (true)
            {
                if (UseExchange)
                {
                    int origin = Interlocked.Exchange(ref _locked, 1); // use atomic.
                    if (origin == 0)
                        break;
                }
                else // CAS
                {
                    const int expected = 0, desired = 1;
                    if (Interlocked.CompareExchange(ref _locked, desired, expected) == expected)
                        break;
                }
            }
        }

        public void Release()
        {
            _locked = 0;
        }
    }


    internal class Program
    {
        private static int _num = 0;
        private static SpinLock _lock = new SpinLock();

        private const int LoopCnt = 1000000;


        static void Thread_01()
        {
            for (int i = 0; i < LoopCnt; i++)
            {
                _lock.Acquire();
                ++_num;
                _lock.Release();
            }
        }
        
        static void Thread_02()
        {
            for (int i = 0; i < LoopCnt; i++)
            {
                _lock.Acquire();
                --_num;
                _lock.Release();
            }
        }

        static void Main(string[] args)
        {
            var t1 = new Task(Thread_01);
            var t2 = new Task(Thread_02);

            t1.Start();
            t2.Start();

            Task.WaitAll(t1, t2);
            
            Console.WriteLine($"Hello World! {_num}");
        }
    }
}
