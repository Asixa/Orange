using System;
using System.Reflection;
using System.Reflection.Emit;

namespace a
{
    public class Program
    {
        public  int a;
        static void Main(string[] args)
        {
            var t = Test<c2>();
            Console.ReadLine();
            
        }

        public static c2 Test<T>()
        {
            var type = typeof(T);
            object entity = type.Assembly.CreateInstance(type.FullName,false,BindingFlags.Default,null, new object[] { 2 }, null, null);
            return (c2) entity;
        }
    }

    public class c1
    {
        public c1(int a)
        {
            Console.WriteLine(a);
        }
        public static explicit operator c2(c1 v)
        {
            return null;
        }
    }

    public class c2
    {
        public c2(int a)
        {
            Console.WriteLine("Child" + a);
        }
    }
}
