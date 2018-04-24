using System;
using System.Collections.Generic;
using Orange.Parse.Operation;
using Orange.Parse.Structure;
using static Tag;
using String = Orange.Tokenize.String;

namespace Orange.Parse.Statements
{
    class DotNet:Stmt
    {
        public string code;
        public Type @return;
        private readonly List<LogicNode> @params = new List<LogicNode>();
        public DotNet(string c)
        {
            code = c;
        }

        public static Stmt Match()
        {
            Match(DOTNET);
            Match('<');
            var tok = Look;
            var dot_net = new DotNet((tok as String)?.value);
            Match(STRING);
            Match('>');
            Match('(');
            if (!Check(')'))
            {
                dot_net.@params.Add(BoolTree.Match());
                while (Check(','))
                {
                    Move();
                    dot_net.@params.Add(BoolTree.Match());
                }
            }
            Match(')');
            Match(';');
            
            return null;
        }


        public override void Generate(Method generator)
        {
            //generator.AddCode(attribute == 1 ? ISet.Storeloc : ISet.StoreField, index);
        }
    }
}
