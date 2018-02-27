using System;
namespace a
{
    public class Program
    {
        public static int a;
        static void Main(string[] args)
        {
            c("HelloWorld");
            Console.WriteLine(Type.GetType("a.Program").GetField("a"));
            Console.ReadKey();
           
        }

        public static void c(string a)
        {
            Console.WriteLine(a);
        }
    }
}
