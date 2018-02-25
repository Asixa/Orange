using Orange.Debug;
using Orange.Tokenize;

namespace Orange.Parse.New.Operation
{
    public class MathTree:LogicNode
    {
        public LogicNode left;
        public LogicNode right;

        public MathTree(Token op, LogicNode lhs, LogicNode rhs) : base(op, null)
        {
            left = lhs;
            right = rhs;
            Type = Type.Max(left.Type, right.Type);
            if (Type == null)Error(Debugger.Errors.TypeError);
        }
        public override string ToString()=>left + " " + Op+ " " + right;

        public static LogicNode Match()
        {
            var expr = Match_Mul_Div();
            while (_look.TagValue == '+' || _look.TagValue == '-')
            {
                var tok = _look;
                Move();
                expr = new MathTree(tok, expr, Match_Mul_Div());
            }
            return expr;
        }

        public static LogicNode Match_Mul_Div()
        {
            var expr = Unary.Match();
            while (_look.TagValue == '*' || _look.TagValue == '/')
            {
                var tok = _look;
                Move();
                expr = new MathTree(tok, expr, Unary.Match());
            }
            return expr;
        }
    }
}
