using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSIL
{

    class Program
    {
        static void Main()
        {
            unsafe void ptr()
            {
                int* a;
                long b;
                a =nullptr+ b;
            }
            Console.WriteLine();
            Console.ReadLine();
        }
    }
}
