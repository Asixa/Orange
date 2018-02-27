using Orange.Parse;
using Orange.Parse.Core;
using Orange.Tokenize;

namespace Orange
{
    public class ExprC:Stmt
    {
        
        public static  Expr Bool()
        {
            var expr = Join();
            while (_look.TagValue == Tag.OR)
            {
                Move();
                expr = new Or(expr, Join());
            }
            return expr;
        }

        public static Expr Join()
        {
            var expr = Equality();
            while (_look.TagValue == Tag.AND)
            {
                Move();
                expr = new And(expr, Equality());
            }
            return expr;
        }

        public static Expr Equality()
        {
            var expr = Rel();
            while (_look.TagValue == Tag.EQ || _look.TagValue == Tag.NE)
            {
                var tok = _look;
                Move();
                expr = new Rel(tok, expr, Rel());
            }
            return expr;
        }

        public static Expr Rel()
        {
            var expr = Expr();
            if ('<' != _look.TagValue && Tag.LE != _look.TagValue && Tag.GE != _look.TagValue &&
                '>' != _look.TagValue) return expr;
            var tok = _look;
            Move();
            return new Rel(tok, expr, Expr());
        }

        public static Expr Expr()
        {
            var expr = Term();
            while (_look.TagValue == '+' || _look.TagValue == '-')
            {
                var tok = _look;
                Move();
                expr = new Arith(tok, expr, Term());
            }
            return expr;
        }

        public static Expr Term()
        {
            var expr = Unary();
            while (_look.TagValue == '*' || _look.TagValue == '/')
            {
                var tok = _look;
                Move();
                expr = new Arith(tok, expr, Unary());
            }
            return expr;
        }

        public static Expr Unary()
        {
            switch (_look.TagValue)
            {
                case '-':
                    Move();
                    return new Unary(Word.minus, Unary());
                case '!':
                    Move();
                    return new Not(Unary());
                default:
                    return Factor();
            }
        }

        public static Expr Factor()
        {
            Expr expr = null;
            switch (_look.TagValue)
            {
                case '(':
                    Move();
                    expr = Bool();
                    Match(')');
                    return expr;

                case Tag.INT:
                    expr = new Constant(_look, Type.Int);
                    Move();
                    return expr;

                case Tag.FLOAT:
                    expr = new Constant(_look, Type.Float);
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
                    expr = new Constant(_look, Type.String);
                    Move();
                    return expr;
                default:
                    Error("syntax error");
                    return expr;

                case Tag.ID:
                    var s = _look.ToString();
                    var id = Top.Get(_look);
                    if (id == null)
                        Error(_look + " undeclared");
                    Move();
                    if (_look.TagValue != '[')
                        return id;
                    else
                        return Offset(id);
            }
        }

        public static Access Offset(Id a)
        {
            Expr i, w, t1, t2, loc;
            var type = a.Type;
            Match('[');
            i = Bool();
            Match(']');
            type = (type as Array).Of;
            w = new Constant(type.Width);
            t1 = new Arith(new Token('*'), i, w);
            loc = t1;
            while (_look.TagValue == '[')
            {
                Match('[');
                i = Bool();
                Match(']');
                type = (type as Array).Of;
                w = new Constant(type.Width);
                t1 = new Arith(new Token('*'), i, w);
                t2 = new Arith(new Token('+'), loc, t1);
                loc = t2;
            }
            return new Access(a, loc, type);
        }
    }

    public class Expr : Node
    {
        public Token Op { get; set; }
        public Type Type { get; set; }

        public Expr(Token tok, Type type) 
        {
            Op = tok;
            Type = type;
        }

        public virtual Expr Gen()
        {
            return this;
        }

        public virtual Expr Reduce()
        {
            return this;
        }

        public virtual void Jumping(int lineForTrue, int lineForFalse)
        {
            EmitJumps(ToString(), lineForTrue, lineForFalse);
        }

        public void EmitJumps(string test, int lineForTrue, int lineForFalse)
        {
            if (lineForTrue != 0 && lineForFalse != 0)
            {
                Emit("if " + test + " goto L" + lineForTrue);
                Emit("goto L" + lineForFalse);
            }
            else if (lineForTrue != 0)
            {
                Emit("if " + test + " goto L" + lineForTrue);
            }
            else if (lineForFalse != 0)
            {
                Emit("iffalse " + test + " goto L" + lineForFalse);
            }
            else
            {
                ;
            }
        }

        public override string ToString()
        {
            return Op.ToString();
        }

    }

    public class Id : Expr
    {
        public int Offset { get; set; }
        public Id(Word id, Type type, int offset)
            : base(id, type)
        {
            Offset = offset;
        }
    }

    public class Temp : Expr
    {
        static int Count = 0;
        public int Number { get; private set; }

        public Temp(Type type) : base(Word.temp, type)
        {
            Number = ++Count;
        }

        public override string ToString()
        {
            return "t" + Number;
        }
    }

    public class Op : Expr
    {
        public Op(Token op, Type type): base(op, type)
        {
        }

        public override Expr Reduce()
        {
            var expr = Gen();
            var temp = new Temp(Type);
            Emit(temp + " = " + expr);
            return temp;
        }
    }

    public class Arith : Op
    {
        public Expr ExprLeft;
        public Expr ExprRight;

        public Arith(Token op, Expr lhs, Expr rhs) : base(op, null)
        {
            ExprLeft = lhs;
            ExprRight = rhs;
            Type = Type.Max(ExprLeft.Type, ExprRight.Type);
            if (Type == null)
                Error("type error");
        }

        public override Expr Gen()
        {
            return new Arith(Op, ExprLeft.Reduce(), ExprRight.Reduce());
        }

        public override string ToString()
        {
            return ExprLeft.ToString() + " " + Op.ToString() + " " + ExprRight.ToString();
        }
    }

    public class Unary : Op
    {
        public Expr Expr { get; private set; }

        public Unary(Token op, Expr expr): base(op, Type.Max(Type.Int, expr.Type))
        {
            if (Type == null)
                Error("type error");
            Expr = expr;
        }

        public override Expr Gen()=>new Unary(Op, Expr.Reduce());

        public override string ToString()=>Op.ToString() + " " + Expr.ToString();
    }
}
