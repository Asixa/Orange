using Orange.Parse.Core;
namespace Orange.Parse.Statements
{
    public class Block:Stmt
    {
        public new static Stmt Match()
        {
            Match('[');
            var saved_env = Top;
            Parser.current.Top = new Env(Top);
            var stmt =Stmts.Match();
            Match(']');
            Parser.current.Top = saved_env;
            return stmt;
        }
    }
}
