using System;
using Orange.Parse;
using Orange.Parse.Core;
using Orange.Tokenize;

namespace Orange
{
    public class Node
    {
        readonly int _lexLine;
        static int _labels = 0;

        public Node() 
        { 
            _lexLine = Lexer.Line; 
        }

        //public void Error(string msg) => Debug.Debugger.Error("[ERROR] line " + _lexLine + ": " + msg);


        public static Token _look => Parser.current._look;
        public static void Move() => Parser.current._look = Parser.current._lexer.Scan();
        public static void Match(int tag)
        {
            if (_look.TagValue == tag) Move();
            else Error("syntax error:" + _look.TagValue + "-" + (char)tag);
        }
        public static Env Top => Parser.current.Top;
        public static Snippet snippet => Parser.current.snippet;
        public static void Error(string msg) => Debug.Debugger.Error("[ERROR] line " + Lexer.Line + ": " + msg);


        public int NewLable()=>++_labels;

        public void EmitLabel(int i)
        {
            if(Program.debug)Console.WriteLine("L" + i + ":");
            TAC.runtime.tags.Add(i, TAC.runtime.tacs.Count);
            TAC.Add(new TAC("TAG",i.ToString()));
           
        }

        public void Emit(string s)
        {
            if (Program.debug) Console.WriteLine("\t" + s);
            //DEBUG 后期删掉以提升效率
            s=s.Replace("*", "MUL")
                .Replace("/", "DIV")
                .Replace("+", "PLU")
                .Replace("-", "MIN")
                .Replace(">", "GTR")
                .Replace("<", "LES")
                .Replace(">=", "GEQ")
                .Replace("<=", "LEQ")
                .Replace("==", "EQU")
                .Replace("goto L","goto ^");
            //***********

            TAC.Add(new TAC(s.Split(' ')));
        }
    }
}
