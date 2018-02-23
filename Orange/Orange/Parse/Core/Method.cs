using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orange.Parse.Statements;

namespace Orange.Parse.Core
{
    class Method:Stmt
    {
        public bool isPublic;
        public string Name;
        public Type returnType;
        public Stmt block;
        public FuncParamsDecla p_params;
    }
}
