using Orange.Tokenize;

namespace Orange.Parse.New.Operation
{
    public class LogicNode:Node
    {
        public Token Op { get; set; }
        public Type Type { get; set; }

        public LogicNode(Token tok, Type type)
        {
            Op = tok;
            Type = type;
        }

        public override string ToString()=>Op.ToString();
    }

}
