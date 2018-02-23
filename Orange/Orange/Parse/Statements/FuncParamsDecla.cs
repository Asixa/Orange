using System.Collections.Generic;
using Orange.Tokenize;

namespace Orange.Parse.Statements
{
    public class FuncParamsDecla:Stmt
    {
        public List<Param> _params=new List<Param>();

        public new static FuncParamsDecla Match()
        {
            FuncParamsDecla p = new FuncParamsDecla();
            Match('(');
            while (_look.TagValue == ',')
            {
                Match(',');
                var type = Type.Match();
                var name = _look;
                Match(Tag.ID);
                p._params.Add(new Param(type, name.ToString()));
            }
            Match(')');
            return p;
        }
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
