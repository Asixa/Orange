using System.Collections.Generic;

namespace Orange.Parse.New.Statements
{
    public class FuncCall : Stmt
    {
        public Type name;
        public List<Expr> param;

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

        public new static Stmt Match()
        {
            var func_call = new FuncCall
            {
                name = Type.ComplexType(),
            };
            Match('<');
            Match('<');
            if (_look.TagValue == '#')
            {
                Match('#');
                Match(';');
                return func_call;
            }
            Match(';');
            return func_call;
        }
    }
}
