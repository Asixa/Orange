using System.Reflection.Emit;
using Orange.Parse.Statements;
using Orange.Parse.Structure;
using static Tag;

namespace Orange.Parse
{
    public class Stmt : Node
    {
        protected static readonly Stmt Null = new Stmt();
        public static Stmt Enclosing = Null;
        public virtual void Create(ILGenerator generator) { }

        protected static Stmt Match()
        {
            switch (_look.tag_value)
            {
                case ';':
                    Move();
                    return Null;
                case '{':
                    return Block.Match();
                case LET:
                    return Let.Match();
                case DEF:
                    return Variable.Defination();
                default:
                    return FuncCall.Match();
            }
        }
    }

    public class Stmts : Stmt
    {
        private readonly Stmt stmt1,stmt2;

        private Stmts(Stmt s1, Stmt s2)
        {
            stmt1 = s1;
            stmt2 = s2;
        }

        public new static Stmt Match() => _look.tag_value == '}' ? Null : new Stmts(Stmt.Match(), Match());

        public override void Create(ILGenerator generator)
        {
            stmt1.Create(generator);
            stmt2.Create(generator);
        }
    }
}
