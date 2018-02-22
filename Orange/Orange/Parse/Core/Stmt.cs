namespace Orange.Parse
{
    public class Stmt : Node
    {
        public int After { get; protected set; }

        public Stmt() 
        {
            After = 0;
        }

        public virtual void Gen(int begin, int after) { }

        public static Stmt Null = new Stmt();

        public static Stmt Enclosing = Stmt.Null;

    }
}
