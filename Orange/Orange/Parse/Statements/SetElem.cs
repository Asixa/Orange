namespace Orange.Parse.Statements
{

    public class SetElem : Stmt
    {
        public Id Array;
        public Expr Index;
        public Expr Expr;

        public SetElem(Access access, Expr expr)
        {
            Array = access.Array;
            Index = access.Index;
            Expr = expr;
            if (null == Check(access.Type, Expr.Type))
                Error("type error");
        }

        private Type Check(Type lhs, Type rhs)
        {
            if (lhs is Array || rhs is Array) return null;
            if (lhs == rhs) return rhs;
            if (Type.Numeric(lhs) && Type.Numeric(rhs)) return rhs;
            return null;
        }

        public override void Gen(int begin, int after)
        {
            var idx = Index.Reduce().ToString();
            var val = Expr.Reduce().ToString();
            Emit(Array + " [ " + idx + " ] = " + val);
        }
    }
}
