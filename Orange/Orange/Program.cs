using System;
using System.IO;
using System.Linq;
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
            Compile.Compiler.includes.Add("C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.5\\System.dll");
            Compile.Compiler.includes.Add("C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.5\\mscorlib.dll");
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
            if(debug)Console.ReadKey();

            Compile.Compiler.setting.AssemblyName = Compile.Compiler.setting.ModuleName = Path.GetFileNameWithoutExtension(Path.GetFileName(path));
            Compile.Compiler.Gen_MANIFEST();
            
            Console.WriteLine(Compile.Compiler.IL_Code);
            Console.ReadKey();
        }
    }
}
