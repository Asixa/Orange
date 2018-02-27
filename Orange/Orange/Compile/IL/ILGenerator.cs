using System;
using System.Reflection;
using System.Reflection.Emit;
using Orange.Parse;

namespace Orange.Compile.IL
{
    class ILGenerator
    {
        public static string Code;

        public static void AddLine(string code) => Code += code + "\n";

        public static void Gen_MANIFEST()
        {
            AddLine(".assembly extern mscorlib");
            AddLine("{");
            AddLine("   .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )");
            AddLine("   .ver 4:0:0:0");
            AddLine("}");

            AddLine(".assembly " + Compiler.setting.AssemblyName);
            AddLine("{");
            AddLine("   .hash algorithm 0x00008004");
            AddLine("   .ver 1:0:0:0");
            AddLine("}");

            AddLine(".module " + Compiler.setting.ModuleName);
            AddLine(".imagebase           0x00400000");
            AddLine(".file alignment      0x00000200");
            AddLine(".stackreserve        0x00100000");
            AddLine(".subsystem           0x0003    ");
            AddLine(".corflags            0x00020003");
        }


        public class TagGenerater
        {
            public static int index;
            public static void Clear() => index = 0;

            public static string Get()
            {
                var result = "IL_" + To36(index).ToLower().PadLeft(4, '0');
                index++;
                return result;
            }

            public static string To36(int a)
            {
                int to = 36, i = 0;
                var array = new char[64];
                while (a != 0)
                {
                    array[i] = (char) (a % to);
                    if (array[i] >= 10)
                        array[i] = (char) (array[i] - 10 + 'A');
                    else
                        array[i] = (char) (array[i] + 48);
                    a /= to;
                    i++;
                }

                i--;
                var result = "";
                while (i >= 0)
                {
                    result += array[i];
                    i--;
                }

                return result;
            }
        }

        public static AssemblyBuilder assembly;
        public static MethodBuilder method;
        public static ModuleBuilder module;
        public static TypeBuilder Object;
        public static System.Reflection.Emit.ILGenerator IL;

        public static void Generate()
        {
            assembly = AppDomain.CurrentDomain.DefineDynamicAssembly(
                new AssemblyName(Compiler.setting.AssemblyName),
                AssemblyBuilderAccess.RunAndSave); //创建程序集
            module = assembly.DefineDynamicModule(
                Compiler.setting.ModuleName,
                Compiler.setting.AssemblyName+".exe"); //定义模块

            foreach (var t in Parser.snippets)
            {
                foreach (var t2 in t.NamespaceDefine)
                {
                    t2.GenerateIL();
                }
            }

            Finish();
        }



        public static void NewClass(string name)
        {
            Object = module.DefineType(name, TypeAttributes.Public); //定义类
        }

        public static void NewFunction(string name, MethodAttributes attributes, Type type, params Type[] paramType)
        {
            var method = Object.DefineMethod(name, attributes, type, paramType); //定义方法
            IL = method.GetILGenerator(); //获取il生成器
        }

        public static void Finish()
        {
            assembly.SetEntryPoint(Object.GetMethod("Main"));
            assembly.Save(Compiler.setting.AssemblyName + ".exe");
        }

    }
}
