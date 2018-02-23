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

        public static Type Match()
        {
            Type type = null;
            if (Stmt._look.TagValue == Tag.BASIC)
            {
                type = Stmt._look as Type; //expect _look.tag == Tag.Basic
                Stmt.Match(Tag.BASIC);
            }
            else if (Stmt._look.TagValue == Tag.ID)
            {

                var word = Stmt._look as Word;
                if (Stmt.snippet.types.Contains(word.ToString()))
                {
                    type = new Type(word.Lexeme, Tag.ID, 4);
                }
                else
                {
                    Error("未知的类型");
                }
                Stmt.Match(Tag.ID);
            }
            return Stmt._look.TagValue != '[' ? type : Dimension(type);
        }
        public static Type ComplexType()
        {
            Type t = null;
            string id = "";
            id += Stmt._look.ToString();
            Stmt.Match(Tag.ID);
            while (Stmt._look.TagValue == '.')
            {
                Stmt.Match('.');
                id += Stmt._look.ToString();
                Stmt.Match(Tag.ID);
            }
            t = new Type(id, Tag.ID, 4);
            return Stmt._look.TagValue != '[' ? t : Dimension(t);
        }

        public static Type Dimension(Type type)
        {
            Stmt.Match('[');
            var tok = Stmt._look;
            Stmt.Match(Tag.NUM);
            Stmt.Match(']');

            if (Stmt._look.TagValue == '[')
                type = Dimension(type);

            return new Array(((Int)tok).Value, type);
        }

        public static void Error(string msg) => Debug.Debugger.Error("[ERROR] line " + Lexer.Line + ": " + msg);
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
