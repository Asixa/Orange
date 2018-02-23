namespace Orange.Parse.Statements
{
    public class If : Stmt
    {
        public Expr Expr { get; private set; }

        public Stmt Stmt { get; private set; }

        public If(Expr expr, Stmt stmt)
        {
            Expr = expr;
            Stmt = stmt;
            if (Expr.Type != Type.Bool)
                Expr.Error("boolean required in if");
        }

        public override void Gen(int begin, int after)
        {
            var lable = NewLable();
            Expr.Jumping(0, after);
            EmitLabel(lable);
            Stmt.Gen(lable, after);
        }
    }
}
