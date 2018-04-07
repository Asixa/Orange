using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Reflection;
using System.Xml;
using Orange.Tokenize;

// ReSharper disable InconsistentNaming
namespace Orange.Debug
{
    public static class Debugger
    {
        public static readonly Dictionary<string, string> debug_text = new Dictionary<string, string>();

        public static void Init(string file)
        {
            var xmldoc = new XmlDocument();
            xmldoc.Load(
                Assembly.GetExecutingAssembly()
                    .GetManifestResourceStream("Orange.Debug.Localization." + file + ".xml") ??
                throw new InvalidOperationException());
            foreach (XmlElement element in xmldoc.GetElementsByTagName("Text"))
                debug_text.Add(element.GetAttribute("tag"), element.GetAttribute("content"));
        }

        public static void Error(string msg)
        {
            Message(msg,ConsoleColor.Red);
            if (Program.Debug) Console.ReadKey();
            Environment.Exit(0);
        }
        public static void Error(string msg, int line,int ch, params object[] param) //向屏幕输出错误提示，并直接退出
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(debug_text["Error"]+"["+line+":"+ch+"]:"+debug_text[msg], param);
            Console.ForegroundColor = ConsoleColor.White;
            var header = debug_text["Line"] + " " + line + ":"; var code = Lexer.linesOfFile[line - 1];
            var code_notab =code.Replace("\t","");
            Console.WriteLine(code_notab);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(new string(' ',ch-(code.Length-code_notab.Length)*4) + "↑");
            if (Program.Debug) Console.ReadKey();
            Environment.Exit(0);
        }

        public static void Message(string msg, ConsoleColor color=ConsoleColor.Green) //向屏幕输出文字，可设置颜色
        {
            Console.ForegroundColor = color;
            Console.WriteLine(msg);
            Console.ForegroundColor = ConsoleColor.White;
        }

        public const string
            TypeError = "TypeError",
            UnknownVariable = "UnknownVariable",
            GrammarError = "GrammarError",
            UnknownFunction = "UnknownFunction",
            UnknownField = "UnknownField",
            UnkownType = "UnkownType",
            ImpossibleError = "ImpossibleError",
            ShouldBe = "ShouldBe",
            Errors = "Error",
            Line = "Line";

        public static string GetTag(char tag) => (int) tag < 200 ? tag.ToString() : Tag.Get(tag);
    }
}
