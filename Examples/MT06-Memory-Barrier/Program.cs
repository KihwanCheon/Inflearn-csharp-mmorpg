namespace MT06_Memory_Barrier
{
    class Program
    {
        
        static void Main(string[] args)
        {
            HardwareOptimizedExecutor.Execute(); // loop will be broken.

            MemoryBarrieredExecutor.Execute();  // infinite looped.
        }
    }
}
