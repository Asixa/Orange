using Orange.Debug;
using Orange.Tokenize;
using Type = Orange.Parse.Core.Type;

namespace Orange.Parse.New.Operation
{
    public class BoolTree:LogicNode
    {
        public LogicNode left;
        public LogicNode right;
        public BoolTree(Token op, LogicNode lhs, LogicNode rhs) : base(op, null)
        {
            left = lhs;
            right = rhs;
            Type = Check(lhs.Type,rhs.Type);
            if (Type==null)Error(Debugger.Errors.TypeError);
        }
        protected Type Check(Type lft, Type rht)
        {
            switch (Op.TagValue)
            {
                case Tag.OR:
                case Tag.AND: return (lft == Type.Bool && rht == Type.Bool) ? Type.Bool : null;
                default: return lft == rht ? Type.Bool : null;
            }
        }
        public override string ToString() => left + " " + Op + " " + right;
        public static LogicNode Match() => MatchTemplate<BoolTree>(MatchSingleBool, new[] { Tag.OR, Tag.AND });
        public static LogicNode MatchSingleBool() => MatchTemplate<BoolTree>(MatchCompare, new[] { Tag.EQ, Tag.NE });
        private static LogicNode MatchCompare() => MatchTemplate<BoolTree>(MathTree.Match, new[] { '<', '>', Tag.LE, Tag.GE }, false);
    }
}
