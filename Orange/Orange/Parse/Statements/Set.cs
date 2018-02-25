namespace Orange.Parse.Statements
{
    public class Set : Stmt
    {
        public Id Id;
        public Expr Expr;

        public Set(Id id, Expr expr)
        {
            Id = id;
            Expr = expr;
            if (null == Check(Id.Type, Expr.Type))
                Error("type error");
        }

        public Type Check(Type lhs, Type rhs)
        {
            if (Type.Numeric(lhs) && Type.Numeric(rhs))
                return rhs;
            if (lhs == Type.Bool && rhs == Type.Bool)
                return rhs;
            if (lhs == Type.String && rhs == Type.String)
                return rhs;
            return null;
        }

        public override void Gen(int begin, int after) => Emit(Id + " = " + Expr.Gen());
    }
}
