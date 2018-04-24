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
        private bool isval;
        public Factor(Token tok, Type type) : base(tok, type){}
        private static readonly Factor
            True = new Factor(Word.True, Type.Bool),
            False = new Factor(Word.False, Type.Bool);

        public override Type Check(Method method)
        {
            if (type != null) return type;
            isval = true;//local
            foreach (var local in method.locals)if (local.name == Op.ToString()) return local.type;
            foreach (var local in method.@class.public_field)if (local.name == Op.ToString()) return local.type;
            Error(UnknownField,lex_line,lex_ch,Op.ToString());
            return null;
        }

        public override void Generate(Method method)
        {
            if (isval)
            {
                var index = method.GetVar(Op.ToString(), out var attribute);
                method.AddCode(attribute==1 ? ISet.Push_local : ISet.Push_field, index);
            }
            else
            {
                method.AddCode(ISet.Push_value , Op);
            }
           
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
