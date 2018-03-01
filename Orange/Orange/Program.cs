using System;
using System.IO;
using Orange.Compile;
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
            var watch = new System.Diagnostics.Stopwatch(); watch.Start();                                                                      //开始计时

            path = args.Length == 2 && args[0] == "-c" ? args[1] : "Sample.org";                                                                //载入指令
            Compiler.dlls.Add("C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.5\\System.dll");          //添加系统dll引用
            Compiler.dlls.Add("C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.5\\mscorlib.dll");
            Compiler.setting.AssemblyName = Compiler.setting.ModuleName = Path.GetFileNameWithoutExtension(path);                               //设置编译器配置
            Quote.GetAllAvaliableNameSpace();                                                                                                   //分析所有可用的命名空间

            Compiler.Init();                                                                                                                    //初始化编译器
            new Parser(new Lexer(new StreamReader(path))).Analyze();                                                                            //语法词法分析
            Compiler.Generate();                                                                                                                //生成IL程序集
            
            Debug.Debugger.Message("编译完成，耗时" + watch.Elapsed.TotalMilliseconds + "毫秒", ConsoleColor.Green);                              //输出计时
            Console.ReadKey();
        }
    }
}
