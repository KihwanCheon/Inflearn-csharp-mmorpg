using System;
using System.Threading;
using System.Threading.Tasks;

namespace MT14_ReadWriteLockImpl
{
    /// <summary>
    /// 재귀적 락 허용(Write->Write, Write->Read)
    /// 스핀락 정책 (5000번 -> Yield)
    /// </summary>
    class Lock
    {
        private const int EMPTY_FLAG = 0x00000000;
        private const int WRITE_MASK = 0x7FFF0000;
        private const int READ_MASK  = 0x0000FFFF;
        private const int MAX_SPIN_COUNT = 5000;

        // [0] unused
        // [1]~[15] Write Thread Id
        // [16]~[32] Read count
        private int _flag = EMPTY_FLAG;
        private int _writeCount = 0;

        // 아무도 읽거나 쓰고 있지 않으면 쓰레드 id 를 기록한다.
        public void WriteLock()
        {
            // 동일 스레드가 쓰기락 잡고 있으면 쓰기 카운트 올림.
            int lockThreadId = (_flag & WRITE_MASK) >> 16;
            if (Thread.CurrentThread.ManagedThreadId == lockThreadId)
            {
                ++_writeCount;
                return;
            }

            int desired = (Thread.CurrentThread.ManagedThreadId  << 16) & WRITE_MASK;

            while (true)
            {
                for (int i = 0; i < MAX_SPIN_COUNT; i++)
                {
                    if (Interlocked.CompareExchange(ref _flag, desired, EMPTY_FLAG) == EMPTY_FLAG)
                    {
                        ++_writeCount;
                        return;
                    }

                    // if (_flag == EMPTY_FLAG) // 원자성 깨진 코드
                    // {
                    //     _flag = desired;
                    //     return;
                    // }
                }
                Thread.Yield();
            }
        }

        public void WriteUnLock()
        {
            if (--_writeCount == 0)
                Interlocked.Exchange(ref _flag, EMPTY_FLAG);
        }

        // 아무도 Write 하고 있지 않으면 ReadCount를 1 늘린다.
        public void ReadLock()
        {
            // 동일 스레드가 쓰기락 잡고 있으면 단순히 읽기 카운트 올림.
            int lockThreadId = (_flag & WRITE_MASK) >> 16;
            if (Thread.CurrentThread.ManagedThreadId == lockThreadId)
            {
                Interlocked.Increment(ref _flag);
                return;
            }

            while (true)
            {
                for (int i = 0; i < MAX_SPIN_COUNT; i++)
                {
                    int expected = (_flag & READ_MASK); // _flag 가 READ_MASK 마스크 씌운 값과 같으면 WRITE 부분이 없다는 의미.
                    if (Interlocked.CompareExchange(ref _flag, expected + 1, expected) == expected)
                        return;

                    // if ((_flag & WRITE_MASK) == 0) // 원자성 깨진 코드
                    // {
                    //     _flag++;
                    //     return;
                    // }
                }
                Thread.Yield();
            }
        }

        public void ReadUnLock()
        {
            Interlocked.Decrement(ref _flag);
        }

    }
    

    internal class Program
    {
        private static volatile int count = 0;
        private static Lock _lock = new Lock();

        static void Main(string[] args)
        {
            var t1 = new Task(delegate()
            {
                for (int i = 0; i < 1000000; ++i)
                {
                    _lock.WriteLock();
                    _lock.WriteLock();
                    ++count;
                    _lock.WriteUnLock();
                    _lock.WriteUnLock();
                }
            });

            var t2 = new Task(delegate ()
            {
                for (int i = 0; i < 1000000; ++i)
                {
                    _lock.WriteLock();
                    --count;
                    _lock.WriteUnLock();
                }
            });

            t1.Start();
            t2.Start();

            Task.WaitAll(t1, t2);

            Console.WriteLine($"count {count}");
        }
    }
}
