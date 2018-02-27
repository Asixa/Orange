using Orange.Parse.Core;

namespace Orange.Parse.Statements
{
    public class Print : Stmt
    {
        private readonly Expr expr;
        public Print(Expr e) => expr = e;

        public override void Gen(int begin, int after)
        {
            var t = new Temp(Type.Int);
            Emit(t + " = " + expr.Gen());
            Emit("Print " + t);
        }
    }
}
