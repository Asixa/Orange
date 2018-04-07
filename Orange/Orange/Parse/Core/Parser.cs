using System;
using System.Collections.Generic;
using Orange.Parse.Core;
using Orange.Parse.Operation;
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

        public Parser Analyze()
        {
            //var a =BoolTree.Match();
            //Console.WriteLine(a);
            //a.Serialize();
            //Console.WriteLine("完成");
            //Console.ReadLine();


            snippet =new Snippet();
            //Quote.Match();
            //snippet.GetAllType();
            while (look.tag_value==NAMESPACE)snippet.namespace_define.Add(Namespace.Match());
            Snippets.Add(snippet);
            return this;
        }

        public void Check()
        {
            foreach (var snippet in Snippets)
            foreach (var name_space in snippet.namespace_define)
                name_space.Generate();
        }
    }
}
