using Orange.Compile;
using Orange.Parse.New.Statements;
using Orange.Parse.Statements;
using Orange.Tokenize;

namespace Orange.Parse.New.Structure
{
    public class Func:Stmt
    {
        public bool isPublic;
        public string name;
        public Type returnType;
        public Stmt block;
        public FuncParamsDecla p_params;

        public static Func Match()
        {
            var function = new Func();
            Match(Tag.FUNC);
            function.name = _look.ToString();
            Match(Tag.ID);
            Match('<');
            Match('<');
            function.returnType=Type.Void;
            Match('|');
            Match('>');
            Match('>');
            Match('[');
            function.block =Stmts.Match();
            Match(']');
            return function;
        }

        public void GenerateIL()
        {
            Compiler.AddLine(".method private hidebysig static void  "+name+"() cil managed");
            Compiler.AddLine("{");

            if (name == "Main") Compiler.AddLine(" .entrypoint");
            Compiler.AddLine(".maxstack  8");

            Compiler.AddLine("}");
        }
    }
}
