﻿using Orange.Debug;
using Orange.Parse.Core;
using Orange.Tokenize;

namespace Orange.Parse.Structure
{
    public class Node
    {
        readonly int _lexLine;

        public Node()=>_lexLine = Lexer.line; 
        //public void Error(string msg) => Debug.Debugger.Error("[ERROR] line " + _lexLine + ": " + msg);

        public static Token _look => Parser.current.look;
        public static void Move() => Parser.current.look = Parser.current.lexer.Scan();
        public static void Match(int tag)
        {
            if (_look.tag_value == tag) Move();
            else Error(Debugger.Errors.GrammarError +": "+ _look + " "+Debugger.Errors.ShouldBe+" " + (char)tag);
        }
        public static Env Top => Parser.current.top;
        public static Snippet snippet => Parser.current.snippet;
        public static void Error(string msg) => Debugger.Error(Debugger.Errors.Error+Debugger.Errors.Line + Lexer.line + ": " + msg);
    }
}