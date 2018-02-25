using Orange.Tokenize;

namespace Orange.Parse.Statements
{
    public class IDOper:Stmt
    {
        public static Stmt Assign()
        {
            Stmt stmt;
            var tok = _look;

            Match(Tag.ID);
            var id = Top.Get(tok);
            if (id == null)
                ErrorWithLine(tok + " undeclared");

            if (_look.TagValue == '=')
            {
                Move();
                stmt = new Set(id,ExprC.Bool());
            }
            else
            {
                var x = ExprC.Offset(id);
                Match('=');
                stmt = new SetElem(x, ExprC.Bool());
            }

            Match(';');
            return stmt;
        }

    }
}
