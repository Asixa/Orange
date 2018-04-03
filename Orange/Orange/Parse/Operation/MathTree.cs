using Orange.Debug;
using Orange.Parse.Core;
using Orange.Tokenize;

namespace Orange.Parse.Operation
{
    public class MathTree:LogicNode
    {
        private readonly LogicNode left;
        private readonly LogicNode right;
        public MathTree(Token op, LogicNode lhs, LogicNode rhs) : base(op, null)
        {
            left = lhs;
            right = rhs;
            Type = Type.Max(left.Type, right.Type);
            if (Type == null)Error(Debugger.Errors.TypeError);
        }
        public override string ToString()=>left + " " + Op+ " " + right;
        public static LogicNode Match() => MatchTemplate<MathTree>(Match_MD, new[] { '+', '-' });
        private static LogicNode Match_MD() => MatchTemplate<MathTree>(Unary.Match, new[] { '*', '/' });
    }
}
