using System.Collections.Generic;
using System.Reflection.Emit;

namespace Orange.Compile
{
    class Compiler
    {
        public static CompileSetting setting=new CompileSetting();
        public static List<string>includes=new List<string>();

        public static AssemblyBuilder assembly;
        public static MethodBuilder method;
        public static ModuleBuilder module;
        public static TypeBuilder Object;

        public void Test()
        {
            assembly
        }
    }

    public class CompileSetting
    {
        public string AssemblyName, ModuleName;
    }
}
