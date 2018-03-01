using System;
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
                case '(':
                    Move();
                    factor = BoolTree.Match(); //最高表达式形态
                    Match(')');
                    return factor;
                case Tag.INT:
                    factor=new Factor(_look,Type.Int);
                    Move();
                    return factor;
                case Tag.FLOAT:
                    factor = new Factor(_look, Type.Float);
                    Move();
                    return factor;
                case Tag.TRUE:
                    factor = True;
                    Move();
                    return factor;
                case Tag.FALSE:
                    factor = False;
                    Move();
                    return factor;
                case Tag.STRING:
                    factor = new Factor(_look,Type.String);
                    Move();
                    return factor;
                case Tag.ID:
                    var tok = _look;
                    var identitifer = Type.Match();
                    var variable = Top.Get(_look);
                    if (variable == null)
                        Error(_look + Debugger.Errors.UnknownVariable);
                    Move();
                    return new Factor(tok, identitifer.check());
                default:
                    Error(Debugger.Errors.GrammarError+" "+_look);
                    return null;
            }
        }
    }
}
