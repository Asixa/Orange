using Orange.Parse.Statements;
using Orange.Tokenize;

namespace Orange.Parse.Core
{
    class Method:Stmt
    {
        public bool isPublic;
        public string Name;
        public Type returnType;
        public Stmt block;
        public FuncParamsDecla p_params;

        public static Stmt Match()
        {
            var method = new Method();
            if (_look.TagValue == Tag.PUBLIC)
            {
                Match(Tag.PUBLIC);
                method.isPublic = true;
            }
            else if (_look.TagValue == Tag.PRIVATE)
            {
                Match(Tag.PRIVATE);
            }
            method.returnType = Type.Match();
            method.Name = _look.ToString();
            Match(Tag.ID);
            method.p_params =FuncParamsDecla.Match();
            method.block = Block.Match();
            return method;
        }
    }
}
