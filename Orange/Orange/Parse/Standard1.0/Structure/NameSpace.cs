using System.Collections.Generic;
using Orange.Parse.Structure;
using Orange.Tokenize;

namespace Orange.Parse.New.Structure
{
    public class NameSpace:Stmt
    {
        public string name;
        public List<Obj> objs=new List<Obj>();
        public new static NameSpace Match()
        {
            var space=new NameSpace();
            Match('&');
            space.name = _look.ToString();
            Match(Tag.ID);
            Match('-');
            Match('>');
            if (_look.TagValue == '[')
            {
                Match('[');
                while (_look.TagValue == Tag.OBJ) space.objs.Add(Obj.Match());
                Match(']');
            }
            else space.objs.Add(Obj.Match());
            return space;
        }

        public void GenerateIL()
        {
            foreach (var t in objs)
            {
                t._namespace = name;
                t.GenerateIL();
            }
        }
    }
}
