using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orange.Parse.Core
{
    class Class:Stmt
    {
        public bool isPublic;
        public string name;
        public List<Method> methods=new List<Method>();
    }
}
