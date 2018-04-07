using Orange.Generate;
using Orange.Parse.Structure;
using Orange.Tokenize;
using static Tag;
using Type = Orange.Parse.Core.Type;
using static Orange.Debug.Debugger;
namespace Orange.Parse.Operation
{
    public class Factor:LogicNode
    {
        public Factor(Token tok, Type type) : base(tok, type){}
        private static readonly Factor
            True = new Factor(Word.True, Type.Bool),
            False = new Factor(Word.False, Type.Bool);

        public override void Generate(Method method)
        {
            method.AddCode(ISet.Push_value,Op);
        }

        public static LogicNode Match()
        {
            LogicNode factor;
            switch (Look.tag_value)
            {
                case INT:factor=new Factor(Look,Type.Int);break;
                case FLOAT:factor = new Factor(Look, Type.Float);break;
                case TRUE:factor = True;break;
                case FALSE: factor = False;break;
                case STRING:factor = new Factor(Look,Type.String);break;
                case '(':
                    Move();
                    factor = BoolTree.Match();
                    Match(')');
                    return factor;
                case ID:return Phrase.Match();
                default:
                    Error(GrammarError);
                    return null;
            }
            Move();
            return factor;
        }
    }
}
