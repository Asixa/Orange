using System;
using System.IO;
using Orange.Compile;
using Orange.Compile.IL;
using Orange.Parse;
using Orange.Parse.New.Structure;
using Orange.Tokenize;

namespace Orange
{
    class Program
    {
        public static bool debug=true;
        public static string path;
        static void Main(string[] args)
        {
            var watch = new System.Diagnostics.Stopwatch(); watch.Start();

            path = args.Length == 2 && args[0] == "-c" ? args[1] : "Sample.org";
            Compiler.includes.Add("C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.5\\System.dll");
            Compiler.includes.Add("C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.5\\mscorlib.dll");
            Compiler.setting.AssemblyName = Compiler.setting.ModuleName = Path.GetFileNameWithoutExtension(path);
            Quote.GetAllAvaliableNameSpace();

            new Parser(new Lexer(new StreamReader(path))).Analyze();
            ILGenerator.Generate();

            

            Debug.Debugger.Message("编译完成，耗时" + watch.Elapsed.TotalMilliseconds + "毫秒", ConsoleColor.Green);
            Console.ReadKey();
        }
    }
}
