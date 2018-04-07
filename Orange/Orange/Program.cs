using System;
using System.IO;
using Orange.Debug;
using Orange.Generate;
using Orange.Parse;
using Orange.Tokenize;

namespace Orange
{
    internal static class Program
    {
        public const bool Debug = true;
        private static string path;

        private static void Main(string[] args)
        {
            //if (args.Length == 0)
            //{
            //    Debugger.Message("你就在此地不要走动,我去把橘子写完",ConsoleColor.Yellow);
            //    return;
            //}

            Debugger.Init("Chinese");
            var watch = new System.Diagnostics.Stopwatch(); watch.Start();                                                                      //开始计时
            path = args.Length == 2 && args[0] == "-c" ? args[1] : "Sample.org";                                                                //载入指令                                                                                                           //初始化编译器
            new Parser(new Lexer(path)).Analyze().Check();                                                                            //语法词法分析

            Debugger.Message("编译完成，耗时" + watch.Elapsed.TotalMilliseconds + "毫秒", ConsoleColor.Green);                              //输出计时
            Console.ReadKey();
        }
    }
}
