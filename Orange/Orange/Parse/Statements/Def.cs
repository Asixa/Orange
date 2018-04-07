using System.Reflection.Emit;
using Orange.Generate;
using Orange.Parse.Core;
using Orange.Parse.Operation;
using Orange.Parse.Structure;
using Orange.Tokenize;
using static Tag;

namespace Orange.Parse.Statements
{
    public class Def : Stmt
    {
        public string name;
        public LogicNode MatchType;
        public Core.Type type;

        public LocalBuilder builder;

        public Def(string n, Core.Type t,LogicNode match)
        {
            name = n;
            type = t;
            MatchType = match;
        }

        public static Stmt Defination()
        {
            Match(DEF);
            var match = Type.Match();
            var tok = Look;
            Match(ID);
            Match(';');
            var id = new Def(tok.ToString(), null,match);
        //    Top.AddIdentifier(tok, id);
            //Parser.current.used += match.width;
            return id;
        }

        public override void Generate(Method generator)
        {
           
        }
    }
}
