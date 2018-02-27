using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Orange.Tokenize;

namespace Orange.Parse.Core
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
        public System.Type SystemType;
        public string nameSpace;

        public Type(string typeName, char tag, int width,System.Type t): base(typeName, tag) 
        { 
            Width = width;
            SystemType = t;
        }

        public static readonly Type
            Void    =   new Type("void",Tag.BASIC,1,typeof(void)),
            Int     =   new Type("int",     Tag.BASIC, 4,typeof(int)),
            Float   =   new Type("float",   Tag.BASIC, 8,typeof(float)),
            Char    =   new Type("char",    Tag.BASIC, 1,typeof(char)),
            Bool    =   new Type("bool",    Tag.BASIC, 1,typeof(bool)),
            String  =   new Type("string",  Tag.BASIC,1,typeof(string));

        public static bool Numeric(Type type) 
        {
            return type == Char || type == Int || type == Float; 
        }

        public static Type Max(Type lhs, Type rhs)
        {
            if (lhs == String || rhs == String)return String;

            if (!Numeric(lhs) || !Numeric(rhs))return null;

            if (lhs == Float || rhs == Float)return Float;

            if (lhs == Int || rhs == Int)return Int;
           
            return Char;
        }


        public static Type Match()//可返回类型，也可返回字段，也可返回属性
        {
            Type t;
            if (Node._look.TagValue == Tag.BASIC)
            {
                t = Node._look as Type;
                Node.Match(Tag.BASIC);
            }
            else
            {
                var id = "";
                var _namespace = "";

                id += Node._look.ToString();
                Node.Match(Tag.ID);
                switch (CheckHeader(id))
                {
                    case 1:break;
                    case 2:break;
                    case 3:break;
                    case 4:
                        for (var i = 0; i < Parser.current.snippet.typesNamespace.Count; i++)
                        {
                            if (Parser.current.snippet.typesNamespace[i].Contains(id))
                                _namespace = Parser.current.snippet.include[i].name;
                        }
                        break;
                    default:break;
                }

                while (Node._look.TagValue == '.')
                {
                    Node.Match('.');
                    id += "." + Node._look;
                    Node.Match(Tag.ID);
                }

                t = new Type(id, Tag.ID, 4, System.Type.GetType(_namespace+id)) {nameSpace = _namespace};
            }

            return Node._look.TagValue != '[' ? t : Dimension(t);
        }

        private static int CheckHeader(string header)
        {
            //TODO 检查是不是变量               return 1;
            //TODO 检查是不是已经声明了的类      return 2;
            //TODO 检查是不是本地函数            return 3;
            if (Parser.current.snippet.types.Contains(header)) return 4;
            Error("未知的类型");
            return -1;
        }

        public static Type Dimension(Type type)
        {
            Node.Match('[');
            var tok = Stmt._look;
            Node.Match(Tag.INT);
            Node.Match(']');

            if (Node._look.TagValue == '[')
                type = Dimension(type);

            return new Array(((Int)tok).Value, type);
        }

        public static void Error(string msg) => Debug.Debugger.Error("[ERROR] line " + Lexer.Line + ": " + msg);
    }

    public class Array : Type
    {
        public Type Of;
        public int Size;
        
        public Array(int sz, Type type): base("[]",Tag.INDEX, sz * type.Width,typeof(Array))//TODO
        {
            Size = sz;
            Of = type;
        }

        public override string ToString()=>"[" + Size + "] " +Of;
    }
}
