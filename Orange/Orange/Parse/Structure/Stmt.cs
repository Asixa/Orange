using System.Reflection.Emit;
using Orange.Parse.New.Statements;
using Orange.Parse.Standard1._0.Statements;
using Orange.Parse.Statements;
using Orange.Tokenize;

namespace Orange.Parse
{
    public class Stmt : Node
    {
        public static Stmt Null = new Stmt();
        public static Stmt Enclosing = Null;
        public virtual void Create(ILGenerator generator) { }
        public static Stmt Match()
        {
            switch (_look.TagValue)
            {
                case ';':
                    Move();
                    return Null;
                case '{':
                    return Block.Match();
                case Tag.LET:
                    return Let.Match();
                case Tag.DEF:
                    return Variable.Defination();
                default:
                    return FuncCall.Match();
            }
        }
    }

    public class Stmts : Stmt
    {
        public Stmt stmt1, stmt2;

        public Stmts(Stmt s1, Stmt s2)
        {
            stmt1 = s1;
            stmt2 = s2;
        }

        public new static Stmt Match() => _look.TagValue == '}' ? Null : new Stmts(Stmt.Match(), Match());

        public override void Create(ILGenerator generator)
        {
            stmt1.Create(generator);
            stmt2.Create(generator);
        }
    }
}
