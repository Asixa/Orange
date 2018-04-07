using Orange.Parse.Core;
using Orange.Parse.Structure;
using Orange.Tokenize;
using static Orange.Debug.Debugger;
using static Orange.Generate.Generator;
namespace Orange.Parse.Operation
{
    public class MathTree:LogicNode
    {
        public readonly LogicNode left,right;
        public MathTree(Token op, LogicNode lhs, LogicNode rhs) : base(op, null)
        {
            left = lhs;
            right = rhs;
        }
        public override Type Check()
        {
            type = Type.Max(left.Check(), right.Check());
            if (type == null) Error(TypeError, lex_line, lex_ch, "");
            return type;
        }

        public override void Generate(Method method)
        {
            left.Generate(method);
            right.Generate(method);
            method.AddCode(
                Op.tag_value == '+' ? ISet.Add :
                Op.tag_value == '-' ? ISet.Minus :
                Op.tag_value == '*' ? ISet.Multiply : ISet.Divide
                );
        }

        public override string ToString()=>left + " " + Op+ " " + right;
        public static LogicNode Match() => MatchTemplate<MathTree>(Match_MD, new[] { '+', '-' });
        private static LogicNode Match_MD() => MatchTemplate<MathTree>(Unary.Match, new[] { '*', '/' });
    }
}
