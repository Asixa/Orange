using Orange.Debug;
using Orange.Tokenize;
using Type = Orange.Parse.Core.Type;

namespace Orange.Parse.New.Operation
{
    public class Factor:LogicNode
    {
        public Factor(Token tok, Type type) : base(tok, type){}
        public Factor(int i) : base(new Int(i), Type.Int){ }
        public static readonly Factor
            True = new Factor(Word.True, Type.Bool),
            False = new Factor(Word.False, Type.Bool);
        public static LogicNode Match()
        {
            LogicNode factor;
            switch (_look.TagValue)
            {
                case Tag.INT:factor=new Factor(_look,Type.Int);break;
                case Tag.FLOAT:factor = new Factor(_look, Type.Float);break;
                case Tag.TRUE:factor = True;break;
                case Tag.FALSE: factor = False;break;
                case Tag.STRING:factor = new Factor(_look,Type.String);break;
                case '(':
                    Move();
                    factor = BoolTree.Match();
                    Match(')');
                    return factor;
                case Tag.ID:
                    var tok = _look;
                    var identitifer = Type.Match();
                    var variable = Top.Get(_look);
                    if (variable == null)
                        Error(_look + Debugger.Errors.UnknownVariable);
                    Move();
                    return new Factor(tok, identitifer.Check());
                default:
                    Error(Debugger.Errors.GrammarError+" "+_look);
                    return null;
            }
            Move();
            return factor;
        }
    }
}
