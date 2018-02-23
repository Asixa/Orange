using System.Collections.Generic;

namespace Orange.Compile
{
    class Compiler
    {
        public static string IL_Code;
        public static CompileSetting setting=new CompileSetting();
        public static List<string>includes=new List<string>();
        
        public static void Clear() => IL_Code = "";

        public static void Gen_MANIFEST()
        {
            AddLine(".assembly extern mscorlib");
            AddLine("{");
            AddLine("   .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )");
            AddLine("   .ver 4:0:0:0");
            AddLine("}");

            AddLine(".assembly "+setting.AssemblyName);
            AddLine("{");
            AddLine("   .hash algorithm 0x00008004");
            AddLine("   .ver 1:0:0:0");
            AddLine("}");

            AddLine(".module " + setting.ModuleName);
            AddLine(".imagebase           0x00400000");
            AddLine(".file alignment      0x00000200");
            AddLine(".stackreserve        0x00100000");
            AddLine(".subsystem           0x0003    ");
            AddLine(".corflags            0x00020003");
        }

        public static void AddLine(string code) => IL_Code += code + "\n";
    }

    public class CompileSetting
    {
        public string AssemblyName, ModuleName;
    }
}
