namespace Orange.Parse.Statements
{
    public class Seq : Stmt
    {
        public Stmt Stmt1;
        public Stmt Stmt2;

        public Seq(Stmt stmt1, Stmt stmt2)
        {
            Stmt1 = stmt1;
            Stmt2 = stmt2;
        }

        public override void Gen(int begin, int after)
        {
            if (Stmt1 == Null)
            {
                Stmt2.Gen(begin, after);
            }
            else if (Stmt2 == Null)
            {
                Stmt1.Gen(begin, after);
            }
            else
            {
                var label = NewLable();
                Stmt1.Gen(begin, label);
                EmitLabel(label);
                Stmt2.Gen(label, after);
            }
        }
    }
}
