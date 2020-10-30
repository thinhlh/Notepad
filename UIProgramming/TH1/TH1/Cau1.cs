using System;
using System.Collections.Generic;
using System.Text;

namespace TH1
{
    class Cau1
    {
        public static bool isPrime(int x)
        {
            if (x <= 1) return false;
            if (x <= 3) return true;
            else
            {
                for (int i = 2; i <= Math.Sqrt(x); i++)
                    if (x % i == 0) return false;
                return true;
            }
        }
        private static bool isSquareNumber(int x)
        {
            if (Math.Sqrt(x) * Math.Sqrt(x) == x) return true;
            return false;
        }
        public static void call()
        {
            Console.WriteLine("===Cau 1===");
            Console.Write("Enter number of elements: ");
            List<int> list = new List<int>();
            int n = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Enter elemts: ");

            while (n > 0)
            {
                list.Add(Convert.ToInt32(Console.ReadLine()));
                n--;
            }

            int total = 0, countPrime = 0, minSquareNumber = -1;

            foreach (int temp in list)
            {
                if (temp % 2 != 0) total += temp;
                if (isPrime(temp) == true) countPrime++;
                if (isSquareNumber(temp) == true && ((minSquareNumber == -1) || minSquareNumber > temp)) minSquareNumber = temp;
            }

            Console.WriteLine("Total of odd numbers: " + total);
            Console.WriteLine("THe number of prime numbers: " + countPrime);
            Console.WriteLine("The smallest square numbers: " + minSquareNumber);
        }

    }
}
