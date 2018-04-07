using Orange.Debug;
using Orange.Parse.Core;
using Orange.Tokenize;
using static Orange.Debug.Debugger;
namespace Orange.Parse.Operation
{
    public class Unary:LogicNode
    {
        private LogicNode Expr { get; }

        private Unary(Token tok, LogicNode expr) : base(tok, null)
        {
            Expr = expr;
        }

        public override Type Check()
        {
           type=Type.Max(Type.Int, Expr.type);
           if (type == null) Error(TypeError, lex_line, lex_ch, "");
           return type;
        }

        public override string ToString() => Op+ " " + Expr;
        public static LogicNode Match()
        {
            switch (Look.tag_value)
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
