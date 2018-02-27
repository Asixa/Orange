using System.Collections.Generic;
using Orange.Debug;
using Orange.Parse;

namespace Orange.Compile
{
    class Compiler
    {
        
        public static CompileSetting setting=new CompileSetting();
        public static List<string>includes=new List<string>();
    }

    public class CompileSetting
    {
        public string AssemblyName, ModuleName;
    }
}
