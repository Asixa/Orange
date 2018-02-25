namespace Orange.Parse.New.Statements
{
    public class Stmts:Stmt
    {
        public Stmt stmt1, stmt2;

        public Stmts(Stmt s1, Stmt s2)
        {
            stmt1 = s1;
            stmt2 = s2;
        }

        public new static Stmt Match() => _look.TagValue == ']' ? Null : new Stmts(Stmt.Match(),Match());

    }
}
