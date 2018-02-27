using Orange.Parse.Core;
using Orange.Parse.New.Statements;
using Orange.Tokenize;

namespace Orange.Parse.Statements
{
    public class Block:Stmt
    {
        public static Stmt Match()
        {
            Match('[');
            var savedEnv = Top;
            Parser.current.Top = new Env(Top);

            Declarations();
            var stmt =Stmts.Match();
            Match(']');
            Parser.current.Top = savedEnv;
            return stmt;
        }

        public static void Declarations()
        {
            while (_look.TagValue == Tag.BASIC)  //D -> type ID
            {
                var type = Type.Match();
                var tok = _look;
                Match(Tag.ID);
                Match(';');

                var id = new Id(tok as Word, type, Parser.current.Used);
                Top.AddIdentifier(tok, id);
                Parser.current.Used += type.Width;
            }
        }
    }
}
