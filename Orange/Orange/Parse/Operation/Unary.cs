using Orange.Debug;
using Orange.Parse.Core;
using Orange.Tokenize;

namespace Orange.Parse.Operation
{
    public class Unary:LogicNode
    {
        private LogicNode Expr { get; }

        private Unary(Token tok, LogicNode expr) : base(tok, Type.Max(Type.Int, expr.Type))
        {
            if (Type == null)Error(Debugger.Errors.TypeError);
            Expr = expr;
        }
        public override string ToString() => Op+ " " + Expr;
        public static LogicNode Match()
        {
            switch (_look.tag_value)
            {
                case '-':
                    Move();
                    return new Unary(Word.Minus,Match());
                case '!':
                    Move();
                    return new Unary(Word.Not, Match());
                default:
                    return Factor.Match();
            }
        }
    }
}
