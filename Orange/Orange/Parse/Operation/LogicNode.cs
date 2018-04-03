using System.Linq;
using Orange.Parse.Structure;
using Orange.Tokenize;
using Type = Orange.Parse.Core.Type;

namespace Orange.Parse.Operation
{
    public class LogicNode : Node
    {
        protected Token Op { get; }
        public Type Type { get; protected set; }

        protected LogicNode(Token tok, Type type)
        {
            Op = tok;
            Type = type;
        }
        public override string ToString() => Op.ToString();

        protected delegate LogicNode Rule();

        protected static LogicNode MatchTemplate<T>
            (Rule rule, char[] operators, bool loop = true) where T : LogicNode
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
            if (loop)while (operators.Contains(_look.tag_value))expr= single.Invoke();
            else if (operators.Contains(_look.tag_value))return single.Invoke();
            return expr;
        }
    }
}
