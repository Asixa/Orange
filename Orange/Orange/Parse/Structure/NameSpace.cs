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
            Match(Tag.NAMESPACE);
            space.name = _look.ToString();
            Match(Tag.ID);
            if (_look.TagValue == '-')
            {
                Match('-');
                Match('>');
                space.objs.Add(Obj.Match(space));
            }
            else
            {
                Match('{');
                while (_look.TagValue == Tag.OBJ) space.objs.Add(Obj.Match(space));
                Match('}');
            }
            return space;
        }

        public void Create()
        {
            foreach (var obj in objs)obj.Create(this);
        }
    }
}
