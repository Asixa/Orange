using System.Collections.Generic;
using Orange.Tokenize;

namespace Orange.Parse.Core
{
    class Class:Stmt
    {
        public bool isPublic;
        public string name;
        public List<Method> methods=new List<Method>();

        public new static Stmt Match()
        {
            var _class = new Class();
            switch (_look.TagValue)
            {
                case Tag.PUBLIC:
                    Match(Tag.PUBLIC);
                    _class.isPublic = true;
                    break;
                case Tag.PRIVATE:
                    Match(Tag.PRIVATE);
                    break;
            }

            Match(Tag.CLASS);
            var tok = _look;
            Match(Tag.ID);
            _class.name = tok.ToString();
            Match('{');
            _class.methods.Add((Method)Method.Match());
            Match('}');
            return _class;
        }
    }
}
