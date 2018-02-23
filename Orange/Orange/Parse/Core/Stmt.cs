using Orange.Parse.Statements;
using Orange.Tokenize;

namespace Orange.Parse
{
    public class Stmt : Node
    {
        public int After { get; protected set; }

        public Stmt()
        {
            After = 0;
        }

        public virtual void Gen(int begin, int after)
        {
        }

        public static Stmt Null = new Stmt();

        public static Stmt Enclosing = Null;

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

        public static Stmt Stmts() => _look.TagValue == '}' ? Null : new Seq(Match(), Stmts());
        public static Stmt Match()
        {
            Expr expr;
            Stmt s1, s2, savedStmt;
            switch (_look.TagValue)
            {
                case ';':
                    Move();
                    return Null;

                case Tag.IF:
                    Match(Tag.IF);
                    Match('(');
                    expr = ExprC.Bool();
                    Match(')');

                    s1 = Match();
                    if (_look.TagValue != Tag.ELSE)
                        return new If(expr, s1);

                    Match(Tag.ELSE);
                    s2 = Match();
                    return new IfElse(expr, s1, s2);

                case Tag.WHILE:
                    var whileNode = new While();
                    savedStmt = Enclosing;
                    Enclosing = whileNode;
                    Match(Tag.WHILE);
                    Match('(');
                    expr = ExprC.Bool();
                    Match(')');
                    s1 = Match();
                    whileNode.Init(expr, s1);
                    Enclosing = savedStmt;
                    return whileNode;

                case Tag.DO:
                    var doNode = new Do();
                    savedStmt = Enclosing;
                    Enclosing = doNode;
                    Match(Tag.DO);
                    s1 = Match();
                    Match(Tag.WHILE);
                    Match('(');
                    expr = ExprC.Bool();
                    Match(')');
                    Match(';');
                    doNode.Init(s1, expr);
                    Enclosing = savedStmt;
                    return doNode;

                case Tag.BREAK:
                    Match(Tag.BREAK);
                    Match(';');
                    return new Break();
                case Tag.PRINT:
                    Match(Tag.PRINT);
                    Match('(');
                    expr = ExprC.Bool();
                    Match(')');
                    return new Print(expr);

                case '{':
                    return Block.Match();

                default:
                    return IDOper.Assign();
            }
        }
    }
}
