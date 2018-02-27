using Orange.Parse;
using Orange.Parse.Core;
using Orange.Tokenize;

namespace Orange
{
    public class Constant : Expr
    {
        public Constant(Token tok, Type type) : base(tok, type){}

        public Constant(int i) : base(new Int(i), Type.Int){}

        public static readonly Constant
            True = new Constant(Word.True, Type.Bool),
            False = new Constant(Word.False, Type.Bool);

        public override void Jumping(int lineForTrue, int lineForFalse)
        {
            if (this == True && lineForTrue != 0)
                Emit("goto L" + lineForTrue);
            else if (this == False && lineForFalse != 0)
                Emit("goto L" + lineForFalse);
        }
    }

    public class Logical : Expr
    {
        public Expr Lhs { get; private set; }
        public Expr Rhs { get; private set; }

        public Logical(Token op, Expr lhs, Expr rhs): base(op, null)
        {
            Lhs = lhs;
            Rhs = rhs;
            Type = Check(lhs.Type, rhs.Type);
            if (null == Type)
                Error("type error");
        }

        protected virtual Type Check(Type lhs, Type rhs)
        {
            if (lhs == Type.Bool && rhs == Type.Bool)
                return Type.Bool;
            return null;
        }

        public override Expr Gen()
        {
            var falseExit = NewLable();
            var after = NewLable();
            var temp = new Temp(Type);
            Jumping(0, falseExit);
            Emit(temp + " = true");
            Emit("goto L" + after);
            EmitLabel(falseExit);
            Emit(temp + " = false");
            EmitLabel(after);
            return temp;
        }

        public override string ToString()=>Lhs + " " + Op + " " + Rhs;
    }


    /// <summary>
    ///  B = Lhs || Rhs
    /// </summary>
    public class Or : Logical
    {
        public Or(Expr lhs, Expr rhs): base(Word.Or, lhs, rhs)
        {
        }

        public override void Jumping(int trueExit, int falseExit)
        {
            var label = trueExit != 0 ? trueExit : NewLable();
            Lhs.Jumping(label, 0); 
            Rhs.Jumping(trueExit, falseExit); 
            if (trueExit == 0)
                EmitLabel(label);
        }
    }


    /// <summary>
    /// B = Lhs && Rhs 
    /// </summary>
    public class And : Logical
    {

        public And(Expr lhs, Expr rhs): base(Word.And, lhs, rhs)
        {
        }

        public override void Jumping(int trueExit, int falseExit)
        {
            var label = falseExit != 0 ? falseExit : NewLable();
            Lhs.Jumping(0, label);
            Rhs.Jumping(trueExit, falseExit);
            if (falseExit == 0)
                EmitLabel(label);
        }
    }

    /// <summary>
    /// B = !Rhs
    /// </summary>
    public class Not : Logical
    {
        public Not(Expr expr): base(new Token('!'), expr, expr)
        {
        }
        public override void Jumping(int trueExit, int falseExit)
        {
            Rhs.Jumping(falseExit, trueExit);
        }

        public override string ToString()
        {
            return Op.ToString() + " " + Rhs.ToString();
        }
    }

    public class Rel : Logical
    {
        public Rel(Token op, Expr lhs, Expr rhs)
            : base(op, lhs, rhs)
        {
        }

        protected override Type Check(Type lft, Type rht)
        {
            if (lft is Array || rht is Array)
                return null;
            return lft == rht ? Type.Bool : null;
        }

        public override void Jumping(int trueExit, int falseExit)
        {
            Temp t=new Temp(Type.Bool);
            Emit(t+ " = " +Lhs.Reduce() + " " + Op + " " + Rhs.Reduce());
            EmitJumps(t.ToString(), trueExit, falseExit);
        }
    }

    public class Access : Op
    {
        public Id Array;
        public Expr Index;

        public Access(Id arr, Expr idx, Type type)
            : base(new Word("[]", Tag.INDEX), type)
        {
            Array = arr;
            Index = idx;
        }

        public override Expr Gen()
        {
            return new Access(Array, Index.Reduce(), Type);
        }

        public override void Jumping(int trueExit, int falseExit)
        {
            EmitJumps(Reduce().ToString(), trueExit, falseExit);
        }

        public override string ToString()=>Array + " [ " + Index + " ]";
    }
}
