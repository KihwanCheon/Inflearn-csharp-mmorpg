using System;
using System.Threading;

namespace MT14_ReadWriteLockImpl
{
    /// <summary>
    /// 재귀적 락 허용 않음 (일단 간단하게 구현)
    /// 스핀락 정책 (5000번 -> Yield)
    /// </summary>
    class Lock
    {
        const int EMPTY_FLAG = 0x00000000;
        const int WRITE_FLAG = 0x7FFF0000;
        const int READ_FLAG  = 0x0000FFFF;
        const int MAX_SPIN_COUNT = 5000;

        // [0] unused
        // [1]~[15] Write Thread Id
        // [16]~[32] Read count
        private int _flag = EMPTY_FLAG;

        // 아무도 읽거나 쓰고 있지 않으면 쓰레드 id 를 기록한다.
        public void WriteLock()
        {
            int desired = (Thread.CurrentThread.ManagedThreadId  << 16)&WRITE_FLAG;
            while (true)
            {
                for (int i = 0; i < MAX_SPIN_COUNT; i++)
                {
                    if (Interlocked.CompareExchange(ref _flag, desired, EMPTY_FLAG) == EMPTY_FLAG)
                        return;

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
            Interlocked.Exchange(ref _flag, EMPTY_FLAG);
        }

        // 아무도 Write 하고 있지 않으면 ReadCount를 1 늘린다.
        public void ReadLock()
        {
            while (true)
            {
                for (int i = 0; i < MAX_SPIN_COUNT; i++)
                {
                    int expected = (_flag & READ_FLAG); // _flag 가 READ_FLAG 마스크 씌운 값과 같으면 WRITE 부분이 없다는 의미.
                    if (Interlocked.CompareExchange(ref _flag, expected + 1, expected) == expected)
                        return;

                    // if ((_flag & WRITE_FLAG) == 0) // 원자성 깨진 코드
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
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }
    }
}
