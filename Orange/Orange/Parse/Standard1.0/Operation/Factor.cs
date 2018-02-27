using Orange.Parse.Core;
using Orange.Tokenize;

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
                    //var id =Type.Match();


                    //var s = _look.ToString();
                    //var id = Top.Get(_look);
                    //if (id == null)
                    //    Error(_look + " 未声明的标识符");
                    //Move();
                    //return id;

                    return null;
                default:
                    Error("语法错误=>Factor "+_look);
                    return null;
            }
        }
    }

    public class Identifier : LogicNode
    {
        public string name;
        public Identifier(string name,Type type) : base(null, type)
        {
            this.name = name;
        }
    }
}
