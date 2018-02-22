using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
