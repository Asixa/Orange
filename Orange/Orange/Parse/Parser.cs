using System;
using System.Collections.Generic;
using System.IO;
using Orange.Debug;
using Orange.Parse.Core;
using Orange.Parse.Statements;
using Orange.Tokenize;

namespace Orange.Parse
{
    public class Parser
    {
        public static Parser current;
        public static List<Snippet> snippets;
        #region Fields
        private Lexer _lexer;
        private Token _look;
        public Env Top;
        public int Used;
        public Snippet snippet;
        #endregion

        public Parser(Lexer lex)
        {
             Top = null;
             Used = 0;
             _lexer = lex;
             Move();
        }

        public void Move()
        {
            _look = _lexer.Scan();
        }

        public void Error(string msg) => Debug.Debugger.Error("[ERROR] line " + Lexer.Line + ": " + msg);

        public void Match(int tag)
        {
            if (_look.TagValue == tag)  Move();
            else  Error("syntax error:"+_look.TagValue+"-"+(char)tag);
        }

        public void Analyze()
        {
            snippet=new Snippet();
            Quotes();
            snippet.GetAllType();

            var a=(Class)Class();
            return;
            var blk =  Block();
            var begin = blk.NewLable();
            var after = blk.NewLable();
            blk.EmitLabel(begin);
            blk.Gen(begin, after);
            blk.EmitLabel(after);
            if(Orange.Program.debug)TAC.Print();
        }

        public void Finish()
        {
            snippets.Add(snippet);
        }

        #region Statements

        public Stmt Class()
        {
            var _class=new Class();
            if (_look.TagValue == Tag.PUBLIC)
            {
                Match(Tag.PUBLIC);
                _class.isPublic = true;
            }
            else if(_look.TagValue == Tag.PRIVATE)
            {
                Match(Tag.PRIVATE);
            }
            
            Match(Tag.CLASS);
            var tok = _look;
            Match(Tag.ID);
            _class.name = tok.ToString();
            Match('{');
            _class.methods.Add((Method)Method());
            Match('}');
            return _class;
        }

        public Stmt Method()
        {
            var  method=new Method();
            if (_look.TagValue == Tag.PUBLIC)
            {
                Match(Tag.PUBLIC);
                method.isPublic = true;
            }
            else if(_look.TagValue == Tag.PRIVATE)
            {
                Match(Tag.PRIVATE);
            }
            method.returnType=Type();
            method.Name = _look.ToString();
            Match(Tag.ID);
            method.p_params=ParamDecla();
            method.block=Block();
            return method;
        }

        public Stmt Block()
        {
             Match('{');
            var savedEnv =  Top;
             Top = new Env( Top);

             Declarations();
            var stmt =  Stmts();
             Match('}');
             Top = savedEnv;

            return stmt;
        }

        public void Quotes()
        {
            while (_look.TagValue == '#')  //D -> type ID
            {
                Match('#');
                if (_look.TagValue == '<')
                {
                    Match('<');
                    var tok = _look;
                    string _namespace="";
                    Match(Tag.ID);
                    _namespace += tok.ToString();

                    while (_look.TagValue=='.')
                    {
                        Match('.');
                        _namespace += ".";
                        tok = _look;
                        Match(Tag.ID);
                        _namespace += tok.ToString();
                    }

                    Match('>');
                    snippet.include.Add(new Quote(_namespace));
                }
                else
                {
                    Console.WriteLine(_look.TagValue);
                    Error("syntax error");
                }
            }
        }

        public FuncParamsDecla ParamDecla()
        {
            FuncParamsDecla p=new FuncParamsDecla();
            Match('(');
            while (_look.TagValue==',')
            {
                Match(',');
                var type = Type();
                var name = _look;
                Match(Tag.ID);
                p._params.Add(new Param(type, name.ToString()));
            }
            Match(')');
            return p;
        }

        public List<Expr> ParamInvoke()
        {
            List<Expr>e=new List<Expr>();
            Match('(');
            e.Add(Bool());
            while (_look.TagValue==',')
            {
                Match(',');
                e.Add(Bool());
            }
            Match(')');
            return e;
        }

        public void Declarations()
        {
            while(_look.TagValue == Tag.BASIC)  //D -> type ID
            {
                var type =  Type();
                var tok = _look;
                 Match(Tag.ID);
                 Match(';');

                var id = new Id(tok as Word, type,  Used);
                 Top.AddIdentifier(tok, id);
                 Used += type.Width;
            }
        }

        public Type ComplexType()
        {
            Type t=null;
            string id = "";
            id += _look.ToString();
            Match(Tag.ID);
            while (_look.TagValue=='.')
            {
                Match('.');
                id += _look.ToString();
                Match(Tag.ID);
            }
            t=new Type(id,Tag.ID,4);
            return _look.TagValue != '[' ? t : Dimension(t);
        }

        public Stmt FuncCall()
        {
            FuncCall func_call = new FuncCall
            {
                name = ComplexType(),
                _param = ParamInvoke()
            };
            Match(';');
            return func_call;
        }

   

        public Type Type()
        {
            Type type=null;
            if (_look.TagValue == Tag.BASIC)
            {
                type = _look as Type; //expect _look.tag == Tag.Basic
                Match(Tag.BASIC);
            }
            else if(_look.TagValue==Tag.ID)
            {
                
                var word = _look as Word;
                if (snippet.types.Contains(word.ToString()))
                {
                    type = new Type(word.Lexeme,Tag.ID,4);
                }
                else
                {
                    Error("未知的类型");
                }
                Match(Tag.ID);
            }
            return _look.TagValue != '[' ? type :  Dimension(type);
        }

        public Type Dimension(Type type)
        {
             Match('[');
            var tok = _look;
             Match(Tag.NUM);
             Match(']');

            if (_look.TagValue == '[')
                type =  Dimension(type);

            return new Array(((Int) tok).Value, type);
        }

        public Stmt Stmts()=>_look.TagValue == '}' ? Parse.Stmt.Null : new Seq( Stmt(),  Stmts());

        public Stmt Assign()
        {
            Stmt stmt;
            var tok = _look;

             Match(Tag.ID);
            var id = Top.Get(tok);
            if (id == null)
                 Error(tok + " undeclared");

            if(_look.TagValue == '=')
            {
                 Move();
                stmt = new Set(id,  Bool());
            }
            else 
            {
                var x =  Offset(id);
                 Match('=');
                stmt = new SetElem(x,  Bool());
            }

             Match(';');
            return stmt;
        }

        public Expr Bool()
        {
            var expr =  Join();
            while(_look.TagValue == Tag.OR)
            {
                 Move();
                expr = new Or(expr,  Join());
            }
            return expr;
        }

        public Expr Join()
        {
            var expr =  Equality();
            while(_look.TagValue == Tag.AND)
            {
                 Move();
                expr = new And(expr,  Equality());
            }
            return expr;
        }

        public Expr Equality()
        {
            var expr =  Rel();
            while(_look.TagValue == Tag.EQ || _look.TagValue == Tag.NE)
            {
                var tok = _look;
                 Move();
                expr = new Rel(tok, expr,  Rel());
            }
            return expr;
        }

        public Expr Rel()
        {
            var expr =  Expr();
            if ('<' != _look.TagValue && Tag.LE != _look.TagValue && Tag.GE != _look.TagValue &&
                '>' != _look.TagValue) return expr;
            var tok = _look;
            Move();
            return new Rel(tok, expr,  Expr());
        }

        public Expr Expr()
        {
            var expr =  Term();
            while(_look.TagValue == '+' || _look.TagValue == '-')
            {
                var tok = _look;
                 Move();
                expr = new Arith(tok, expr,  Term());
            }
            return expr;
        }

        public Expr Term()
        {
            var expr =  Unary();
            while(_look.TagValue == '*' || _look.TagValue == '/')
            {
                var tok = _look;
                 Move();
                expr = new Arith(tok, expr,  Unary());
            }
            return expr;
        }

        public Expr Unary()
        {
            switch (_look.TagValue)
            {
                case '-':
                    Move();
                    return new Unary(Word.minus,  Unary());
                case '!':
                    Move();
                    return new Not( Unary());
                default:
                    return  Factor();
            }
        }

        public Expr Factor()
        {
            Expr expr = null;
            switch(_look.TagValue)
            {
                case'(':
                     Move();
                    expr =  Bool();
                     Match(')');
                    return expr;

                case Tag.NUM:
                    expr = new Constant(_look, Parse.Type.Int);
                     Move();
                    return expr;

                case Tag.REAL:
                    expr = new Constant(_look, Parse.Type.Float);
                     Move();
                    return expr;

                case Tag.TRUE:
                    expr = Constant.True;
                     Move();
                    return expr;

                case Tag.FALSE:
                    expr = Constant.False;
                     Move();
                    return expr;
                case Tag.STRING:
                    expr=new Constant(_look,Parse.Type.String);
                    Move();
                    return expr;
                default:
                     Error("syntax error");
                    return expr;

                case Tag.ID:
                    var s = _look.ToString();
                    var id =  Top.Get(_look);
                    if (id == null)
                         Error(_look + " undeclared");
                     Move();
                    if (_look.TagValue != '[')
                        return id;
                    else
                        return  Offset(id);
            }
        }

        public Access Offset(Id a)
        {
            Expr i, w, t1, t2, loc;
            var type = a.Type;
             Match('[');
            i =  Bool();
             Match(']');
            type = (type as Array).Of;
            w = new Constant(type.Width);
            t1 = new Arith(new Token('*'), i, w);
            loc = t1;
            while(_look.TagValue == '[')
            {
                 Match('[');
                i =  Bool();
                 Match(']');
                type = (type as Array).Of;
                w = new Constant(type.Width);
                t1 = new Arith(new Token('*'), i, w);
                t2 = new Arith(new Token('+'), loc, t1);
                loc = t2;
            }
            return new Access(a, loc, type);
        }
        #endregion

        public Stmt Stmt()
        {
            Expr expr;
            Stmt s1, s2, savedStmt;
            switch (_look.TagValue)
            {
                case ';':
                    Move();
                    return Parse.Stmt.Null;

                case Tag.IF:
                    Match(Tag.IF);
                    Match('(');
                    expr = Bool();
                    Match(')');

                    s1 = Stmt();
                    if (_look.TagValue != Tag.ELSE)
                        return new If(expr, s1);

                    Match(Tag.ELSE);
                    s2 = Stmt();
                    return new IfElse(expr, s1, s2);

                case Tag.WHILE:
                    var whileNode = new While();
                    savedStmt = Parse.Stmt.Enclosing;
                    Parse.Stmt.Enclosing = whileNode;
                    Match(Tag.WHILE);
                    Match('(');
                    expr = Bool();
                    Match(')');
                    s1 = Stmt();
                    whileNode.Init(expr, s1);
                    Parse.Stmt.Enclosing = savedStmt;
                    return whileNode;

                case Tag.DO:
                    var doNode = new Do();
                    savedStmt = Parse.Stmt.Enclosing;
                    Parse.Stmt.Enclosing = doNode;
                    Match(Tag.DO);
                    s1 = Stmt();
                    Match(Tag.WHILE);
                    Match('(');
                    expr = Bool();
                    Match(')');
                    Match(';');
                    doNode.Init(s1, expr);
                    Parse.Stmt.Enclosing = savedStmt;
                    return doNode;

                case Tag.BREAK:
                    Match(Tag.BREAK);
                    Match(';');
                    return new Break();
                case Tag.PRINT:
                    Match(Tag.PRINT);
                    Match('(');
                    expr = Bool();
                    Match(')');
                    return new Print(expr);

                case '{':
                    return Block();

                default:
                    return Assign();
            }
        }
    }


}
