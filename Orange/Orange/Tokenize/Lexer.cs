using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Orange.Debug;
using Type = Orange.Parse.Core.Type;
using static Orange.Debug.Debugger;
using static Tag;

// ReSharper disable InconsistentNaming
namespace Orange.Tokenize
{
    public class Lexer
    {
        private readonly StreamReader reader;
        private char peek = ' ';
        private bool EofReached { get; set; }
        public static int line = 1,ch=1;
        private readonly Dictionary<string, Word> key_words = new Dictionary<string, Word>();
        public static string[] linesOfFile;
        private void Reserve(Word word)=>key_words.Add(word.lexeme, word);
        
        public Lexer(string path)
        {
            linesOfFile = File.ReadAllLines(path);
            reader = new StreamReader(path);
            Reserve();
        }

        private void ReadChar()
        {
            try
            {
                if (-1 == reader.Peek())
                {
                    EofReached = true;
                    return;
                }
                peek = (char) reader.Read();
                ch++;
            }
            catch (Exception e)
            {
                Message(e.Message,ConsoleColor.Blue);
            }
        }

        private bool ReadChar(char ch)
        {
            if (EofReached) return false;
            ReadChar();
            if (peek != ch) return false;
            peek = ' ';
            return true;
        }

        public Token Scan()
        {
            //remove spaces
            for (; !EofReached; ReadChar())
            {
                if (peek == ' ' || peek == '\t'){}
                else if (peek == '\r')
                {
                    ReadChar();
                    ++line;
                    ch = 0;
                }
                else break;
            }
            if (EofReached) return null;

            //remove comments
            if (peek == '/') 
            {
                ReadChar();
                switch (peek)
                {
                    case '/': for (; ; ReadChar()) if (peek == '\r' || peek == '\uffff') return Scan();
                    case '*':
                        for (ReadChar(); ; ReadChar())
                        {
                            switch (peek)
                            {
                                case '\r':
                                    line++;
                                    ch = 0;
                                    ReadChar();
                                    break;
                                case '*':
                                    ReadChar();
                                    if (peek == '/')
                                    {
                                        ReadChar();
                                        return Scan();
                                    }
                                    break;
                                case '\uffff':
                                    return Scan();
                            }
                        }
                }
                return new Token('/');
            }

            //Operators
            switch (peek)
            {
                case '&':return ReadChar('&') ? Word.And : new Token('&');
                case '|':return ReadChar('|') ? Word.Or : new Token('|');
                case '=':return ReadChar('=') ? Word.Equal : new Token('=');
                case '!':return ReadChar('=') ? Word.NotEqual : new Token('!');
                case '<':return ReadChar('=') ? Word.Less : new Token('<');
                case '>':return ReadChar('=') ? Word.Greater : new Token('>');
                case '"':
                    var s = "";
                    ReadChar();
                    for (; ; ReadChar())
                    {
                        switch (peek)
                        {
                            case '"':
                            {
                                ReadChar();
                                return new String(s);
                            }
                            case '\n':
                            {
                                Error("应输入\"\"\"");
                                ReadChar();
                                return new String(s);
                            }
                        }
                        s = s + peek;
                    }
            }

            //Digits
            if (char.IsDigit(peek))
            {
                var val = 0;
                do{val = 10 * val + (peek - '0');ReadChar();} while (char.IsDigit(peek));
                if (peek != '.') return new Int(val);

                float float_val = val;
                for (float d = 10;; d *= 10)
                {
                    ReadChar();
                    if (!char.IsDigit(peek)) break;
                    float_val += (peek - 48) / d;
                }
                return new Float(float_val);
            }

            //Identifiers
            if (char.IsLetter(peek))
            {
                var b = new StringBuilder();
                do
                {
                    b.Append(peek);
                    ReadChar();
                } while (char.IsLetterOrDigit(peek));

                var s = b.ToString();
                if (key_words.ContainsKey(s)) return key_words[s];
                return key_words[s] = new Word(s, ID);
            }

            //other symbols
            var tok = new Token(peek);
            if (!EofReached) peek = ' ';
            return tok;
        }

        private void Reserve()
        {
            Reserve(new Word("if", IF));
            Reserve(new Word("else", ELSE));
            Reserve(new Word("while", WHILE));
            Reserve(new Word("do", DO));
            Reserve(new Word("break", BREAK));
            Reserve(new Word("print", PRINT));
            Reserve(new Word("public",PUBLIC));
            Reserve(new Word("private", PRIVATE));
            Reserve(new Word("obj",OBJ));
            Reserve(new Word("func",FUNC));
            Reserve(new Word("let",LET));
            Reserve(new Word("def",DEF));
            Reserve(new Word("import",IMPORT));
            Reserve(new Word("namespace",NAMESPACE));
            Reserve(new Word("dotNet",DOTNET));
            Reserve(Word.True);
            Reserve(Word.False);
            Reserve(Type.Int);
            Reserve(Type.Char);
            Reserve(Type.Bool);
            Reserve(Type.Float);
            Reserve(Type.String);
        }
    }

    public class Token
    {
        public readonly char tag_value;
        public Token(char tag)
        {
            tag_value = tag;
        }
        public override string ToString() => tag_value.ToString();
    }

    public class Int : Token
    {
        private readonly int value;
        public Int(int val) : base(INT)
        {
            value = val;
        }
        public override string ToString() => value.ToString();
    }

    public class Word : Token
    {
        public readonly string lexeme;
        public Word(string lexeme, char tag) : base(tag)
        {
            this.lexeme = lexeme;
        }
        public override string ToString() => lexeme;

        public static readonly Word
            And = new Word("&&", AND),
            Or = new Word("||", OR),
            Equal = new Word("==", EQ),
            NotEqual = new Word("!=", NE),
            Less = new Word("<=", LE),
            Greater = new Word(">=", GE),
            Minus = new Word("-", MINUS),
            True = new Word("true", TRUE),
            False = new Word("false", FALSE),
            Not = new Word("!", NOT);
    }

    public class Float : Token
    {
        private readonly float value;
        public Float(float val) : base(FLOAT)
        {
            value = val;
        }
        public override string ToString() => value.ToString(System.Globalization.CultureInfo.InvariantCulture);
    }

    public class String : Token
    {
        public readonly string value;
        public String(string val) : base(STRING)
        {
            value = val;
        }
        public override string ToString() => value;
    }
}

public static class Tag
{
    public const char
        AND = (char) 256,
        BASIC = (char) 257,
        BREAK = (char) 258,
        DO = (char) 259,
        ELSE = (char) 260,
        EQ = (char) 261,
        FALSE = (char) 262,
        GE = (char) 263,
        ID = (char) 264,
        IF = (char) 265,
        INDEX = (char) 266,
        LE = (char) 267,
        MINUS = (char) 268,
        NE = (char) 269,
        INT = (char) 270,
        OR = (char) 271,
        FLOAT = (char) 272,
        TEMP = (char) 273,
        TRUE = (char) 274,
        WHILE = (char) 275,
        PRINT = (char) 276,
        STRING = (char) 277,
        PUBLIC = (char) 278,
        PRIVATE = (char) 279,
        OBJ = (char) 280,
        FUNC = (char) 281,
        NOT = (char) 282,
        LET = (char) 283,
        DEF = (char) 284,
        IMPORT = (char) 285,
        NAMESPACE = (char) 286,
        DOTNET=(char)287;

    public static string Get(char tag)
    {
        switch (tag)
        {
            case AND:
                return "&";
            case BASIC: return "basic";
            case BREAK: return "break";
            case DO: return "do";
            case ELSE: return "else";
            case EQ: return "==";
            case FALSE: return "false";
            case GE: return ">=";
            case ID: return "ID";
            case IF: return "if";
            case INDEX: return "index";
            case LE: return "<=";
            case MINUS: return "-";
            case NE: return "!=";
            case INT: return "int";
            case OR: return "||";
            case FLOAT: return "float";
            case TEMP: return "temp";
            case TRUE: return "true";
            case WHILE: return "while";
            case PRINT: return "print";
            case STRING: return "string";
            case PUBLIC: return "public";
            case PRIVATE: return "private";
            case OBJ: return "obj";
            case FUNC: return "func";
            case NOT: return "!";
            case LET: return "let";
            case DEF: return "def";
            case IMPORT: return "import";
            case NAMESPACE: return "namespace";
                default: return "ERROR";
        }
    }
}