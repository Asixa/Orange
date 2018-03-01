using Orange.Parse.Core;
using Orange.Parse.New.Statements;
using Orange.Tokenize;

namespace Orange.Parse.Statements
{
    public class Block:Stmt
    {
        public static Stmt Match()
        {
            Match('[');
            var savedEnv = Top;
            Parser.current.Top = new Env(Top);
            var stmt =Stmts.Match();
            Match(']');
            Parser.current.Top = savedEnv;
            return stmt;
        }
    }
}
