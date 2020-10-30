using System;
using System.Collections.Generic;
using System.Text;

namespace TH1
{
    class Cau2
    {
        public static void call()
        {
            Console.WriteLine("===Cau2===");
            Console.Write("Enter n : ");
            int n = Convert.ToInt32(Console.ReadLine());
            int totalPrime = 0;

            for (int i = 2; i < n; i++)
            {
                if (Cau1.isPrime(i)) totalPrime += i;
            }

            Console.WriteLine("Total prime number smaller than n: " + totalPrime);
        }
    }
}
