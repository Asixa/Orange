using Orange.Debug;
using Orange.Parse.Core;
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
            if (Type == null)Error(Debugger.Errors.TypeError+"2 "+left.Type+left+" "+right.Type+right);
        }
        public override string ToString()=>left + " " + Op+ " " + right;
        public static LogicNode Match() => MatchTemplate<MathTree>(Match_Mul_Div, new[] { '+', '-' });
        public static LogicNode Match_Mul_Div() => MatchTemplate<MathTree>(Unary.Match, new[] { '*', '/' });
    }
}
