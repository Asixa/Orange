using Orange.Parse.Core;
namespace Orange.Parse.Statements
{
    public class Block:Stmt
    {
        public new static Stmt Match()
        {
            Match('[');
            var saved_env = Top;
            Parser.current.top = new Env(Top);
            var stmt =Stmts.Match();
            Match(']');
            Parser.current.top = saved_env;
            return stmt;
        }
    }
}
