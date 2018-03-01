using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Orange.Parse;

namespace Orange.Compile
{
    class Compiler
    {
        public class CompileSetting
        {
            public string AssemblyName, ModuleName;
        }
        public static CompileSetting setting=new CompileSetting();                  //编译器设置
        public static List<string>dlls=new List<string>();                          //所有添加的dll

        public static AssemblyBuilder assembly;                                     //生成的程序集
        public static ModuleBuilder module;                                         //module

        public static void Init()
        {
            assembly = AppDomain.CurrentDomain.DefineDynamicAssembly(
                new AssemblyName(setting.AssemblyName),
                AssemblyBuilderAccess.RunAndSave);                                  //创建程序集
            
            module = assembly.DefineDynamicModule(
                setting.ModuleName,
                setting.AssemblyName + ".exe");                                     //定义模块
        }
        public static void Generate()
        {
            foreach (var snippet in Parser.snippets)
            foreach (var name_space in snippet.namespace_define)
                name_space.Create();                                                //遍历所有声明的命名空间结构，生成IL
            assembly.Save(setting.AssemblyName + ".exe");                           //将程序集保存
        }
    }
}
