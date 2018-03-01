using System.Reflection.Emit;
using Orange.Tokenize;

namespace Orange.Parse.New.Statements
{
    public class Variable : Stmt
    {
        public string name;
        public Core.Type type;

        public LocalBuilder builder;

        public Variable(string n, Core.Type t)
        {
            name = n;
            type = t;
        }

        public static Stmt Defination()
        {
            Match(Tag.DEF);
            var identitifer = Core.Type.Match();
            var tok = _look;
            Match(Tag.ID);
            Match(';');
            var id = new Variable(tok.ToString(), identitifer.Check());
        //    Top.AddIdentifier(tok, id);
            Parser.current.Used += identitifer.type.width;
            return id;
        }

        public override void Create(ILGenerator generator)
        {
            builder = generator.DeclareLocal(type.system);
        }
    }
}
