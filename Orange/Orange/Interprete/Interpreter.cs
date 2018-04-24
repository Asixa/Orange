using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Microsoft.CSharp;
using Orange.Interprete.Runtime;

namespace Orange.Interprete
{
    internal static class Interpreter
    {
        public static BinaryReader binary_reader;
        public static readonly List<RTNamespace> NameSpaces = new List<RTNamespace>();

        public static void Run()
        {
            Deserialize();
            foreach (var name_space in NameSpaces)
                foreach (var @class in name_space.classes)
                    if (@class.name == "Main")
                    {

                    }
                
            
            Error("没有找到Main函数");
        }

        public static void Error(string err)
        {

        }
        public static void Deserialize()
        {
            binary_reader = new BinaryReader(new FileStream("a.ore", FileMode.Open));
            var count= binary_reader.ReadInt32();
            for (var i = 0; i < count; i++) NameSpaces.Add(RTNamespace.Deserialize());
            binary_reader.Close();
        }

        public static Stack<object> stack=new Stack<object>();
        public static List<object>heap=new List<object>();

        public static object DotNet(string code)
        {
            var results = new CSharpCodeProvider().CompileAssemblyFromSource(new CompilerParameters(), BuildCode(code));
            if (results.Errors.Count <= 0)
                return results.CompiledAssembly.GetType("C").GetMethod("T")?.Invoke(null, null);
            foreach (var error in results.Errors) Console.WriteLine(error);
            return null;
        }

        private static string BuildCode(string code,params string[] @params)
        {
            var builder = new StringBuilder();
            builder.Append("using System;");
            builder.Append("public class C{ public static Object T("+"){");
            builder.Append(code+"("+");");
            builder.Append("return null;");
            builder.Append("}}");
            return builder.ToString();
        }
    }
}
