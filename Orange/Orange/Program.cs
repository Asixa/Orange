using System;
using System.IO;
using Orange.Compile;
using Orange.Parse;
using Orange.Parse.Structure;
using Orange.Tokenize;

namespace Orange
{
    internal static class Program
    {
        public const bool Debug = true;
        private static string path;

        private static void Main(string[] args)
        {
            var watch = new System.Diagnostics.Stopwatch(); watch.Start();                                                                      //开始计时
            path = args.Length == 2 && args[0] == "-c" ? args[1] : "Sample.org";                                                                //载入指令                                                                                                           //初始化编译器
            new Parser(new Lexer(new StreamReader(path))).Analyze();                                                                            //语法词法分析
            Compiler.Generate();                                                                                                                //生成IL程序集

            Orange.Debug.Debugger.Message("编译完成，耗时" + watch.Elapsed.TotalMilliseconds + "毫秒", ConsoleColor.Green);                              //输出计时
            Console.ReadKey();
        }
    }
}
