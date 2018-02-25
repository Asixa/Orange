using System;
using System.IO;
using Orange.Compile;
using Orange.Parse;
using Orange.Parse.Core;
using Orange.Tokenize;

namespace Orange
{
    class Program
    {
        public static bool debug=true;
        public static string path;
        static void Main(string[] args)
        {
            Compiler.includes.Add("C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.5\\System.dll");
            Compiler.includes.Add("C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.5\\mscorlib.dll");
            Quote.GetAllAvaliableNameSpace();
            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();
            path = args.Length == 2 && args[0] == "-c" ? args[1] : "Sample.org";
            var lex = new Lexer(new StreamReader(path));
            var parse = new Parser(lex);
            parse.Analyze();

            TAC.Generate(Path.GetDirectoryName(path)+Path.GetFileNameWithoutExtension(path)+".ACEXE");
            var meantime = watch.Elapsed.TotalMilliseconds;
            Debug.Debugger.Message("编译完成，耗时" + meantime + "毫秒",ConsoleColor.Green);

            Compiler.setting.AssemblyName = Compiler.setting.ModuleName = Path.GetFileNameWithoutExtension(Path.GetFileName(path));
            Compiler.Gen_MANIFEST();
            Compiler.Gen();
            
            Console.WriteLine(Compiler.IL_Code);
            Console.ReadKey();
        }
    }
}
