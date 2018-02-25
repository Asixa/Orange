using System;
using System.Collections.Generic;
using System.Linq;

namespace a
{

    delegate int test(int a, int b);
    class Program
    {
        static void Main(string[] args)
        {
            test b2 = testfunc;
            b2 += testfunc2;
            test t = (a, b) => a += b;
            Console.WriteLine(b2(1,2).ToString());
            Console.ReadKey();
        }

        public static int testfunc(int a, int b)
        {
            return 1000;
        }
        public static int testfunc2(int a, int b)
        {
            return 9999;
        }
    }
}
