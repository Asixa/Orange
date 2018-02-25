using System;
namespace Orange.Debug
{
    public class Debugger
    {
        
        public static void Error(string msg)
        {
            Message(msg,ConsoleColor.Red);
            if(Program.debug)Console.ReadKey();
            Environment.Exit(0);
        }

        public static void Message(string msg,ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(msg);
            Console.ForegroundColor = ConsoleColor.White;
        }

        public class Errors
        {
            public static string
                TypeError = "类型错误";
        }
    }
}
