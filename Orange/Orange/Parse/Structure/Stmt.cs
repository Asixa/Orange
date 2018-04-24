using System;
using System.Reflection.Emit;
using Orange.Generate;
using Orange.Parse.Statements;
using Orange.Parse.Structure;
using static Tag;

namespace Orange.Parse
{
    public class Stmt : Node
    {
        protected static readonly Stmt Null = new Stmt();
        public static Stmt Enclosing = Null;
        public virtual void Generate(Method generator) { }

        protected static Stmt Match()
        {
            switch (Look.tag_value)
            {
                case ';':
                    Move();
                    return Null;
                case '{':
                    return Block.Match();
                case LET:
                    return Let.Match();
                case DEF:
                    return Def.Match();
                case DOTNET:
                    return DotNet.Match();
                default:
                {
                        return FuncCall.Match();
                }
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

        public new static Stmt Match() => Check('}') ? Null : new Stmts(Stmt.Match(), Match());

        public override void Generate(Method generator)
        {
            stmt1.Generate(generator);
            stmt2.Generate(generator);
        }
    }
}
