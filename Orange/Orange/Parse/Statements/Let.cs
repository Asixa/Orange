using System;
using System.Reflection.Emit;
using Orange.Generate;
using Orange.Parse.Core;
using Orange.Parse.Operation;
using Orange.Parse.Structure;
using static Orange.Debug.Debugger;
using static Tag;
using Type = Orange.Parse.Core.Type;

namespace Orange.Parse.Statements
{
    public class Let : Stmt
    {
        public  LogicNode target;
        public LogicNode value;

        private string typeName, FieldName;

        public static Stmt Match()
        {
            var let = new Let();
            Match(LET);
            let.target = Phrase.Match();
            Match('=');
            let.value = BoolTree.Match();
            Match(';');
            return let;
        }

        public override void Generate(Method generator)
        {
            value.Check(generator);
            value.Generate(generator);
            var morpheme = Phrase.GetEnd(target);
            Phrase.DoubleCheck(morpheme, MorphemeAttribute.Object);
            var index = generator.GetVar(morpheme.name,out var attribute);
            if(index==-1)Error(UnknownVariable,lex_line,lex_ch,morpheme.name);
            generator.AddCode(attribute==1?ISet.Storeloc:ISet.StoreField,index);
        }
    }
}
