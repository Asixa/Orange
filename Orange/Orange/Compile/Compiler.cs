using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Orange.Parse;

namespace Orange.Compile
{
    internal class Compiler
    {
        public class CompileSetting
        {
            public string assembly_name, module_name;
        }
        public static readonly CompileSetting Setting=new CompileSetting();                  //编译器设置
        public static readonly List<string>Dlls=new List<string>();                          //所有添加的dll

        public static AssemblyBuilder assembly;                                     //生成的程序集
        public static ModuleBuilder module;                                         //module
        
        public static void Init()
        {
            assembly = AppDomain.CurrentDomain.DefineDynamicAssembly(
                new AssemblyName(Setting.assembly_name),
                AssemblyBuilderAccess.RunAndSave);                                  //创建程序集
            
            module = assembly.DefineDynamicModule(
                Setting.module_name,
                Setting.assembly_name + ".exe");                                     //定义模块
        }
        public static void Generate()
        {
            foreach (var snippet in Parser.Snippets)
            foreach (var name_space in snippet.namespace_define)
                name_space.Generate();                                                //遍历所有声明的命名空间结构，生成IL
            assembly.Save(Setting.assembly_name + ".exe");                           //将程序集保存
        }
    }
}
