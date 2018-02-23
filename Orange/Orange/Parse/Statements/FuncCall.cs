using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orange.Parse.Statements
{
    public class FuncCall:Stmt
    {
        public Type name;
        public List<Expr> _param;
    }
}
