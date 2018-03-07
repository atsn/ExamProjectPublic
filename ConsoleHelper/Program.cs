using System;

namespace ConsoleHelper
{
    class Program
    {
        static void Main(string[] args)
        {
            for (int i = 0; i < 1000; i++)
            {
                    Console.WriteLine(DateTime.Now.ToString(Guid.NewGuid().ToString()));

            }
        }


    }
}
