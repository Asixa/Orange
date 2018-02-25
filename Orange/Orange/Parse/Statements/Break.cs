namespace Orange.Parse.Statements
{
    public class Break : Stmt
    {
        public Stmt Stmt;

        public Break()
        {
            if (Enclosing == null)
                ErrorWithLine("unenclosed break");
            Stmt = Enclosing;
        }

        public override void Gen(int begin, int after)
        {
            Emit("goto L" + Stmt.After);
        }
    }
}
