using Orange.Parse.Core;

namespace Orange.Parse.Statements
{
    public class While : Stmt
    {
        public Expr Expr { get; private set; }
        public Stmt Stmt { get; private set; }

        public While()
        {
            Expr = null;
            Stmt = null;
        }

        public void Init(Expr expr, Stmt stmt)
        {
            Expr = expr;
            Stmt = stmt;
            if (Expr.Type != Type.Bool)
                Error("boolean requried in while");
        }

        public override void Gen(int begin, int after)
        {
            After = after;
            Expr.Jumping(0, after);
            var label = NewLable();
            EmitLabel(label);
            Stmt.Gen(label, begin);
            Emit("goto L" + begin);
        }
    }
}
