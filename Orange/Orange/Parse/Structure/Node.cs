using Orange.Debug;
using Orange.Parse.Core;
using Orange.Tokenize;
using static Orange.Debug.Debugger;
namespace Orange.Parse.Structure
{
    public class Node
    {
        public readonly int lex_line,lex_ch;

        protected Node()
        {
            lex_line = Lexer.line;
            lex_ch = Lexer.ch;
        }

        public static Token Look => Parser.current.look;
        public static bool Check(char tag) => Look.tag_value == tag;
        public static void Move() => Parser.current.look = Parser.current.lexer.Scan();
        public static void Match(char tag)
        {
            if (Check(tag)) Move();
            else Error(ShouldBe,Lexer.line,Lexer.ch,Look,tag);
        }
        protected static Env Top => Parser.current.top;
        protected static Snippet Snippet => Parser.current.snippet;
    }
}
