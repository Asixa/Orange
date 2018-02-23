using System.Collections.Generic;
namespace Orange.Parse.Statements
{
    public class FuncCall : Stmt
    {
        public Type name;
        public List<Expr> param;

        public new static Stmt Match()
        {
            var func_call = new FuncCall
            {
                name = Type.ComplexType(),
                param = ParamInvoke()
            };
            Match(';');
            return func_call;
        }

        public static List<Expr> ParamInvoke()
        {
            var e = new List<Expr>();
            Match('(');
            e.Add(ExprC.Bool());
            while (_look.TagValue == ',')
            {
                Match(',');
                e.Add(ExprC.Bool());
            }
            Match(')');
            return e;
        }
    }
}
