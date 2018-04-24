using Orange.Parse.Structure;
using Orange.Tokenize;
using Type = Orange.Parse.Core.Type;
using static Orange.Debug.Debugger;
using static Tag;

namespace Orange.Parse.Operation
{
    public class BoolTree:LogicNode
    {
        private readonly LogicNode left,right;
        public BoolTree(Token op, LogicNode lhs, LogicNode rhs) : base(op, null)
        {
            left = lhs;
            right = rhs;
        }

        public override void Generate(Method method)
        {
            left.Generate(method);
            right.Generate(method);
            method.AddCode(
                Op.tag_value == OR ? ISet.Or :
                Op.tag_value == AND ?ISet.And :
                Op.tag_value == EQ|| Op.tag_value == NE ? ISet.Equal :
                Op.tag_value == LE ||Op.tag_value == '>'? ISet.Greater :ISet.Less
            );
            if (Op.tag_value == NE || Op.tag_value == LE || Op.tag_value == GE)
                method.AddCode(ISet.Negate);
        }
        public override Type Check(Method method)
        {
            type = Check(left.Check(method), right.Check(method));
            if (type == null) Error(TypeError, lex_line, lex_ch, "");
            return type;
        }

        private Type Check(Type lft, Type rht)
        {
            switch (Op.tag_value)
            {
                case OR:
                case AND: return (lft == Type.Bool && rht == Type.Bool) ? Type.Bool : null;
                default: return lft == rht ? Type.Bool : null;
            }
        }
        public override string ToString() => left + " " + Op + " " + right;
        public static LogicNode Match() => MatchTemplate<BoolTree>(MatchSingleBool, new[] { OR, AND });
        private static LogicNode MatchSingleBool() => MatchTemplate<BoolTree>(MatchCompare, new[] { EQ, NE });
        private static LogicNode MatchCompare() => MatchTemplate<BoolTree>(MathTree.Match, new[] { '<', '>', LE, GE }, false);
    }
}
