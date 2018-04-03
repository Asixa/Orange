using System;
using System.Collections.Generic;
using Orange.Parse.Core;
using Orange.Parse.Structure;
using Orange.Tokenize;
using static Tag;

namespace Orange.Parse
{
    public class Parser
    {
        public static Parser current;
        public static readonly List<Snippet> Snippets=new List<Snippet>();

        public readonly Lexer lexer;
        public Token look;
        public Env top;
        public int used;
        public Snippet snippet;

        public Parser(Lexer lex)
        {
            current = this;
            top = null;
             used = 0;
             lexer = lex;
            Node.Move();
        }

        public void Analyze()
        {
            snippet =new Snippet();
            Quote.Match();
            snippet.GetAllType();
            while (look.tag_value==NAMESPACE)snippet.namespace_define.Add(NameSpace.Match());
            Snippets.Add(snippet);
        }
    }
}
