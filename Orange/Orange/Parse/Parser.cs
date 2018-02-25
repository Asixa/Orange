using System;
using System.Collections.Generic;
using Orange.Parse.Core;
using Orange.Parse.New.Operation.Boolean;
using Orange.Parse.New.Operation.Math;
using Orange.Parse.Structure;
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
            Console.WriteLine(((BoolTree)BoolTree.Match()).Op);
           
            Console.WriteLine("OK");
            Console.ReadKey();
           snippet =new Snippet();
            Quote.Match();
            snippet.GetAllType();

            while (_look.TagValue=='&')snippet.name_spaces.Add(NameSpace.Match());
            snippets.Add(snippet);
        }
    }
}
