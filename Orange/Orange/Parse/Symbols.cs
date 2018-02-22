using System.Collections.Generic;
using Orange.Tokenize;

namespace Orange.Parse
{
    public class Env
    {
        private readonly Dictionary<Token, Id> SymbolTable;
        protected Env Prev;

        public Env(Env prev)
        {
            SymbolTable = new Dictionary<Token, Id>();
            Prev = prev;
        }

        public void AddIdentifier(Token tok, Id id)
        {
            SymbolTable.Add(tok, id);
        }

        public Id Get(Token tok)
        {
            for(var e = this; e != null; e = e.Prev)
                if (e.SymbolTable.ContainsKey(tok)) return e.SymbolTable[tok];
            return null;
        }
    }


    public class Type : Word
    {
        public int Width;

        public Type(string typeName, char tag, int width): base(typeName, tag) 
        { 
            Width = width; 
        }

        public static readonly Type
            Int     =   new Type("int",     Tag.BASIC, 4),
            Float   =   new Type("float",   Tag.BASIC, 8),
            Char    =   new Type("char",    Tag.BASIC, 1),
            Bool    =   new Type("bool",    Tag.BASIC, 1),
            String  =   new Type("string",  Tag.BASIC,1);

        public static bool Numeric(Type type) 
        {
            return type == Char || type == Int || type == Float; 
        }

        public static Type Max(Type lhs, Type rhs)
        {
            if (lhs == String || rhs == String)
                return String;
            if (!Numeric(lhs) || !Numeric(rhs))
                return null;
            if (lhs == Float || rhs == Float)
                return Float;
            if (lhs == Int || rhs == Int)
                return Int;
           
            return Char;
        }
    }

    public class Array : Type
    {
        public Type Of;
        public int Size;
        
        public Array(int sz, Type type): base("[]",Tag.INDEX, sz * type.Width)
        {
            Size = sz;
            Of = type;
        }

        public override string ToString()=>"[" + Size + "] " +Of;
    }
}
