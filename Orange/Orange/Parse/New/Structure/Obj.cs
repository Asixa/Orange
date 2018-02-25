using System.Collections.Generic;
using Orange.Compile;
using Orange.Parse.New.Structure;
using Orange.Tokenize;

namespace Orange.Parse.Structure
{
    public class Obj:Stmt
    {
        public string _namespace;
        public bool isPublic;
        public string name;
        public List<Func> functions=new List<Func>();

        public new static Obj Match()
        {
            var obj=new Obj();
            Match(Tag.OBJ);
            obj.name = _look.ToString();
            Match(Tag.ID);
            Match('-');
            Match('>');
            Match('['); 
            while (_look.TagValue==Tag.FUNC)obj.functions.Add(Func.Match());   
            Match(']');
            return obj;
        }

        public void GenerateIL()
        {
            Compiler.AddLine(".class private auto ansi beforefieldinit "+_namespace+"."+name+ " extends [mscorlib]System.Object");
            Compiler.AddLine("{");
            //构造函数
            Compiler.AddLine(@"//***********"+_namespace+"."+name+"构造函数开始***************");
            Compiler.AddLine(".method public hidebysig specialname rtspecialname instance void  .ctor() cil managed");
            Compiler.AddLine("{");
            Compiler.AddLine(".maxstack  8");
            Compiler.AddLine("IL_0000: ldarg.0");
            Compiler.AddLine("IL_0001: call instance void [mscorlib]System.Object::.ctor()");
            Compiler.AddLine("IL_0006: nop");
            Compiler.AddLine("IL_0007:  ret");
            Compiler.AddLine("}");
            Compiler.AddLine(@"//***********" + _namespace + "." + name + "构造函数结束***************");
            foreach (var t in functions)
            {
                t.GenerateIL();
            }
            Compiler.AddLine("}");
        }
    }
}
