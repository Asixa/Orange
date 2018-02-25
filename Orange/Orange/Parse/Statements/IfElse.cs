namespace Orange.Parse.Statements
{
    public class IfElse : Stmt
    {
        public Expr Expr { get; private set; }
        public Stmt Stmt1 { get; private set; }
        public Stmt Stmt2 { get; private set; }

        public IfElse(Expr expr, Stmt stmt1, Stmt stmt2)
        {
            Expr = expr;
            Stmt1 = stmt1;
            Stmt2 = stmt2;
            if (Expr.Type != Type.Bool)
                Error("boolean required in if");
        }

        public override void Gen(int beginning, int after)
        {
            var label1 = NewLable();
            var lable2 = NewLable();
            Expr.Jumping(0, lable2);

            EmitLabel(label1);
            Stmt1.Gen(label1, after);
            Emit("goto L" + after);

            EmitLabel(lable2);
            Stmt2.Gen(lable2, after);
        }
    }
}
