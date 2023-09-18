using System;

namespace MT07_Interlocked
{
    class Program
    {
        static void Main(string[] args)
        {
            new ExecutorSharedVariableWithoutLocks().Execute();
            new ExecutorSharedVariableWithInterlock().Execute();
        }
    }
}
