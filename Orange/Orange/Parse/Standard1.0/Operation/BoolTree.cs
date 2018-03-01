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
            if (Type==null)Error(Debugger.Errors.TypeError+"1:"+lhs.Type+" - "+rhs.Type);
        }
        protected Type Check(Type lft, Type rht)
        {
            switch (Op.TagValue)
            {
                case Tag.OR:
                case Tag.AND: return (lft == Type.Bool && rht == Type.Bool) ? Type.Bool : null;
                default: return lft == rht ? Type.Bool : null;//如果两遍类型相等而返回布尔表达式
            }
        }
        public override string ToString() => left + " " + Op + " " + right;
        public static LogicNode Match() => MatchTemplate<BoolTree>(MatchSingleBool, new[] { Tag.OR, Tag.AND });
        public static LogicNode MatchSingleBool() => MatchTemplate<BoolTree>(MatchCompare, new[] { Tag.EQ, Tag.NE });
        private static LogicNode MatchCompare() => MatchTemplate<BoolTree>(MathTree.Match, new[] { '<', '>', Tag.LE, Tag.GE }, false);
        
        public static LogicNode Match2()
        {
            var expr = MatchSingleBool();
            while (_look.TagValue == Tag.OR||_look.TagValue==Tag.AND)
            {
                var tok = _look;
                Move();
                expr = new BoolTree(tok,expr, MatchSingleBool());
            }
            return expr;
        }
        public static LogicNode MatchSingleBool2()
        {
            var expr = MatchCompare();
            while (_look.TagValue == Tag.EQ || _look.TagValue == Tag.NE)
            {
                var tok = _look;
                Move();
                expr = new BoolTree(tok, expr, MatchCompare());
            }
            return expr;
        } 
        private static LogicNode MatchCompare2()
        {
            var expr = MathTree.Match();
            if (_look.TagValue != '<' &&
                _look.TagValue != Tag.LE &&
                _look.TagValue != Tag.GE &&
                _look.TagValue != '>')return expr; 
            var tok = _look;
            Move();
            return new BoolTree(tok,expr, MathTree.Match());
        }
    }
}
