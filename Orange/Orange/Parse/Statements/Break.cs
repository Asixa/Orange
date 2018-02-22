using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orange.Parse.Statements
{

    public class Break : Stmt
    {
        public Stmt Stmt;

        public Break()
        {
            if (Enclosing == null)
                Error("unenclosed break");
            Stmt = Enclosing;
        }

        public override void Gen(int begin, int after)
        {
            Emit("goto L" + Stmt.After);
        }
    }
}
