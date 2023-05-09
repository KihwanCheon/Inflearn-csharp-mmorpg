using System;

namespace MT05_Cache
{
    internal class Program
    {
        static void Main(string[] args)
        {
            {
                int[,] arr = new int[10000, 10000];
                // [1][2][3][4]....[][][]
                // [][][][]....[][][]
                // [][][][]....[][][]
                // [][][][]....[][][]
                // [][][][]....[][][]
                int i = 0;
                long start = DateTime.Now.Ticks;
                for (int y = 0; y < 10000; ++y)
                    for (int x = 0; x < 10000; ++ x)
                        arr[y, x] = ++i;        // 인접 메모리를 캐시해서 접근하는데 시간이 적게 걸림.
                long end = DateTime.Now.Ticks;
                Console.WriteLine($"[y, x] 시간 {end - start}");
            }

            {
                int[,] arr = new int[10000, 10000];
                // [1][][][]....[][][]
                // [2][][][]....[][][]
                // [3][][][]....[][][]
                // [4][][][]....[][][]
                // [5][][][]....[][][]
                int i = 0;
                long start = DateTime.Now.Ticks;
                for (int y = 0; y < 10000; ++y)
                   for (int x = 0; x < 10000; ++x)
                        arr[x, y] = ++i;        // 매번 배열을 메모리에서 새로 가져옴 시간이 더 걸림.

                long end = DateTime.Now.Ticks;
                Console.WriteLine($"[x, y] 시간 {end - start}");
            }

        }
    }
}
