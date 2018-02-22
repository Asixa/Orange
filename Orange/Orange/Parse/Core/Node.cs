using System;
using Orange.Parse;
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

        public void Error(string msg) => Debug.Debugger.Error("[ERROR] line " + _lexLine + ": " + msg);

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
