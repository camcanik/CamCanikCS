using System;
using System.Collections.Generic;

namespace BigPrimeOnesZeros
{
    class Program
    {
        static bool HasOne(int x)
        {
            string numS = x.ToString();
            if (numS.Contains("1"))
                return true;

            return false;
        }
        static bool HasZero(int x)
        {
            string numS = x.ToString();
            if (numS.Contains("0"))
                return true;

            return false;
        }
        static bool HasOtherNum(int x)
        {
            string[] badNums = new string[] { "2", "3", "4", "5", "6", "7", "8", "9" };

            string numS = x.ToString();
            char[] digit = new char[numS.Length];
            digit = numS.ToCharArray();

            for (int i = 0; i < numS.Length; i++)
            {
                foreach (string badNum in badNums)
                {
                    if (digit[i].ToString() == badNum)
                        return true;
                }
            }
            return false;
        }
        static bool isPrime(int x)
        {
            int halfway = (x / 2) + 1;
            for (int i = 2; i <= halfway; i++)
            {
                if (x % i == 0)
                    return false;
            }
            return true;
        }

        static void Main(string[] args)
        {
            Console.WriteLine("This is gonna take a minute. Go grab a snack!\n");
            List<int> bigOnes = new List<int>();
            for (int i = 100000000; i <= 111111111; i++)
            {
                //Console.WriteLine(i);
                if (!HasOtherNum(i) && HasZero(i) && HasOne(i))
                    bigOnes.Add(i);
            }

            Console.WriteLine("\n================\nThe Primes\n================\n");
            foreach (int num in bigOnes)
            {
                if (isPrime(num))
                    Console.WriteLine(num);
            }

            Console.Read();
        }
    }
}