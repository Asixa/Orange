using Orange.Debug;
using Orange.Parse.Core;
using Orange.Tokenize;

namespace Orange.Parse.New.Operation
{
    public class Unary:LogicNode
    {
        public LogicNode Expr { get; }
        public Unary(Token tok, LogicNode expr) : base(tok, Type.Max(Type.Int, expr.Type))
        {
            if (Type == null)Error(Debugger.Errors.TypeError);
            Expr = expr;
        }
        public override string ToString() => Op+ " " + Expr;
        public static LogicNode Match()
        {
            switch (_look.TagValue)
            {
                case '-':
                    Move();
                    return new Unary(Word.minus,Match());
                case '!':
                    Move();
                    return new Unary(Word.Not, Match());
                default:
                    return Factor.Match();
            }
        }
    }
}
