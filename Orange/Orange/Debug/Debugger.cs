using System;
namespace Orange.Debug
{
    public class Debugger
    {
        public static void Error(string msg)                            //向屏幕输出错误提示，并直接退出
        {
            Message(msg,ConsoleColor.Red);
            if(Program.Debug)Console.ReadKey();
            Environment.Exit(0);
        }

        public static void Message(string msg,ConsoleColor color)       //向屏幕输出文字，可设置颜色
        {
            Console.ForegroundColor = color;
            Console.WriteLine(msg);
            Console.ForegroundColor = ConsoleColor.White;
        }

        public class Errors                                             //所有错误信息
        {
            public static string
                TypeError = "类型错误",
                UnknownVariable="未知的变量",
                GrammarError="语法错误",
                UnknownFunction="未知函数",
                UnknownField="未知的字段",
                UnkownType="未知的类型",
                ImpossibleError="这个错误本不应该存在，请联系Asixa",

                ShouldBe="应该为",
                Error="[错误]",
                Line="行号";
        }
    }
}
