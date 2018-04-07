using System.Collections.Generic;
using Orange.Debug;
using Orange.Parse.Operation;
using Orange.Parse.Statements;
using Orange.Parse.Structure;
using Orange.Tokenize;
using static Orange.Debug.Debugger;
using static Tag;

namespace Orange.Parse.Core
{
    public class Env
    {
        private readonly Dictionary<Token, Def> symbol_table;
        private readonly Env prev;

        public Env(Env prev)
        {
            symbol_table = new Dictionary<Token, Def>();
            this.prev = prev;
        }

        public void AddIdentifier(Token tok, Def id)
        {
            symbol_table.Add(tok, id);
        }

        public Def Get(Token tok)
        {
            for(var e = this; e != null; e = e.prev)
                if (e.symbol_table.ContainsKey(tok)) return e.symbol_table[tok];
            return null;
        }
    }

    public class Type : Word
    {
        public readonly int width;
        public string name_space;

        public Type(string type_name, char tag, int width): base(type_name, tag) 
        { 
            this.width = width;
        }

        public static readonly Type
            Void    =   new Type("void",BASIC,1),
            Int     =   new Type("int",     BASIC, 4),
            Float   =   new Type("float",   BASIC, 4),
            Char    =   new Type("char",    BASIC, 1),
            Bool    =   new Type("bool",    BASIC, 1),
            String  =   new Type("string",  BASIC,0);

        private static bool Numeric(Type type) =>type == Char || type == Int || type == Float; 
        public static Type Max(Type lhs, Type rhs)
        {
            if (lhs == String || rhs == String)return String;

            if (!Numeric(lhs) || !Numeric(rhs))return null;

            if (lhs == Float || rhs == Float)return Float;

            if (lhs == Int || rhs == Int)return Int;
           
            return Char;
        }

        public static LogicNode Match()
        {
            if (Node.Check(BASIC))
            {
                var tok = Node.Look;
                Node.Move();
                switch ((tok as Word)?.lexeme)
                {
                    case "void": return new Factor(tok, Void);
                    case "int": return new Factor(tok, Int);
                    case "float": return new Factor(tok, Float);
                    case "char": return new Factor(tok, Char);
                    case "bool": return new Factor(tok, Bool);
                    case "string": return new Factor(tok, String);
                }
            }
            return Phrase.Match();
        }
    }
}
