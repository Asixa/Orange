using Orange.Parse;
using Orange.Tokenize;

namespace Orange
{
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
