namespace Orange.Parse.Statements
{
    public class Do : Stmt
    {
        public Expr Expr { get; private set; }
        public Stmt Stmt { get; private set; }

        public Do()
        {
            Expr = null;
            Stmt = null;
        }

        public void Init(Stmt stmt, Expr expr)
        {
            Expr = expr;
            Stmt = stmt;
            if (Expr.Type != Type.Bool)
                Expr.ErrorWithLine("boolean requried in do");
        }

        public override void Gen(int begin, int after)
        {
            After = after;
            var label = NewLable();
            Stmt.Gen(begin, label);
            EmitLabel(label);
            Expr.Jumping(begin, 0);
        }
    }

}
