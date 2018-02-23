using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orange.Parse.Statements
{
    public class FuncParamsDecla
    {
        public List<Param> _params=new List<Param>();
    }

    public class Param
    {
        public Type type;
        public string name;

        public Param(Type t, string n)
        {
            type = t;
            name = n;
        }
    }
}
