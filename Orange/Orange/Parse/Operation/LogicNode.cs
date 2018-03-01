using System.Linq;
using Orange.Tokenize;
using Type = Orange.Parse.Core.Type;

namespace Orange.Parse.New.Operation
{
    public class LogicNode : Node
    {
        public Token Op { get; set; }
        public Type Type { get; set; }
        public LogicNode(Token tok, Type type)
        {
            Op = tok;
            Type = type;
        }
        public override string ToString() => Op.ToString();
        public delegate LogicNode Rule();
        public static LogicNode MatchTemplate<T>(Rule rule, char[] operators, bool loop = true) where T : LogicNode
        {
            var expr = rule.Invoke();
            Rule single = () =>
            {
                var tok = _look;
                Move();
                return typeof(T) == typeof(BoolTree)
                    ? new BoolTree(tok, expr, rule.Invoke()) as LogicNode
                    : new MathTree(tok, expr, rule.Invoke()) as LogicNode;
            };
            if (loop)while (operators.Contains(_look.TagValue))expr= single.Invoke();
            else if (operators.Contains(_look.TagValue))return single.Invoke();
            return expr;
        }
    }
}
