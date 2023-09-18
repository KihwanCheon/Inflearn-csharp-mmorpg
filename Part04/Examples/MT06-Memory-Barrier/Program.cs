namespace MT06_Memory_Barrier
{
    class Program
    {
        
        static void Main(string[] args)
        {
            ExecutorHardwareOptimized.Execute(); // loop will be broken.
            ExecutorAlbahariNoneBarriered.Execute(); 
            ExecutorAlbahariMemoryBarrieredFull.Execute(); 
            ExecutorMemoryBarriered.Execute();  // infinite looped.
        }
    }
}
