using System.CodeDom;
using System.Linq;
using Orange.Parse.Structure;
using Orange.Tokenize;
using static Orange.Generate.Generator;
using Type = Orange.Parse.Core.Type;

namespace Orange.Parse.Operation
{
    public class LogicNode : Node
    {
        public Token Op { get; }
        public Type type;

        protected LogicNode(Token tok, Type type)
        {
            Op = tok;
            this.type = type;
        }

        public virtual void Generate(Method method){}

        public virtual Type Check() {return type;}
        public override string ToString() => Op.ToString();

        protected delegate LogicNode Rule();
        protected static LogicNode MatchTemplate<T>
            (Rule rule, char[] operators, bool loop = true) where T : LogicNode
        {
            var expr = rule.Invoke();
            Rule single = () =>
            {
                var tok = Look;
                Move();
                return typeof(T) == typeof(Params)
                    ? new Params(tok, expr, rule.Invoke())
                    : typeof(T) == typeof(Phrase)
                        ? new Phrase(tok, expr, rule.Invoke())
                        : typeof(T) == typeof(BoolTree)
                            ? new BoolTree(tok, expr, rule.Invoke())
                            : new MathTree(tok, expr, rule.Invoke()) as LogicNode;
            };
            if (loop)
                while (operators.Contains(Look.tag_value))
                    expr = single.Invoke();
            else if (operators.Contains(Look.tag_value))
                return single.Invoke();
            return expr;
        }
    }
}
