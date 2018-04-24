using System;
using System.Reflection.Emit;
using Orange.Generate;
using Orange.Parse.Operation;
using Orange.Parse.Structure;
using Orange.Tokenize;
using static Tag;
using Type = Orange.Parse.Core.Type;

namespace Orange.Parse.Statements
{
    public class Def : Stmt
    {
        public string name;
        public LogicNode MatchType;
        public Type type;

        public LocalBuilder builder;

        public Def(string n, Core.Type t,LogicNode match)
        {
            name = n;
            type = t;
            MatchType = match;
        }

        public static Stmt Match()
        {
            Match(DEF);
            var match = Type.Match();
            var tok = Look;
            Match(ID);
            Match(';');
            var id = new Def(tok.ToString(), null,match);
            return id;
        }

        public override void Generate(Method generator)
        {
            var morpheme =Phrase.GetEnd(MatchType);
            Phrase.DoubleCheck(morpheme, MorphemeAttribute.Class);
            generator.locals.Add(new Variable(morpheme.type,name));
        }

        public void Generate(Class generator)
        {
            var morpheme = Phrase.GetEnd(MatchType);
            Phrase.DoubleCheck(morpheme, MorphemeAttribute.Class);
            generator.public_field.Add(new Variable(morpheme.type, name));
        }
    }
}
