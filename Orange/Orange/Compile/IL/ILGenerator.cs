using System;
using System.Reflection;
using System.Reflection.Emit;
using Orange.Parse;

namespace Orange.Compile.IL
{
    class ILGenerator
    {
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
