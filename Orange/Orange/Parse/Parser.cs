using System.Collections.Generic;
using Orange.Parse.Core;
using Orange.Parse.Statements;
using Orange.Tokenize;

namespace Orange.Parse
{
    public class Parser
    {
        public static Parser current;
        public static List<Snippet> snippets=new List<Snippet>();

        public Lexer _lexer;
        public Token _look;
        public Env Top;
        public int Used;
        public Snippet snippet;

        public Parser(Lexer lex)
        {
            current = this;
            Top = null;
             Used = 0;
             _lexer = lex;
             Stmt.Move();
        }

        public void Analyze()
        {
            snippet=new Snippet();
            Quote.Match();
            snippet.GetAllType();

            var a=(Class)Class.Match();
            snippets.Add(snippet);
            return;

            var blk =  Block.Match();
            var begin = blk.NewLable();
            var after = blk.NewLable();
            blk.EmitLabel(begin);
            blk.Gen(begin, after);
            blk.EmitLabel(after);
            if(Orange.Program.debug)TAC.Print();
        }
    }
}
