using System.Globalization;

namespace Orange.Tokenize
{
    public class Tag
    {
        public const char
            AND     = (char)256,
            BASIC   = (char)257,
            BREAK   = (char)258,
            DO      = (char)259,
            ELSE    = (char)260,
            EQ      = (char)261,
            FALSE   = (char)262,
            GE      = (char)263,
            ID      = (char)264,
            IF      = (char)265,
            INDEX   = (char)266,    //used in syntax tree
            LE      = (char)267,    //used in syntax tree
            MINUS   = (char)268,    
            NE      = (char)269,
            INT     = (char)270,
            OR      = (char)271,
            FLOAT    = (char)272,
            TEMP    = (char)273,    //used in syntax tree
            TRUE    = (char)274,
            WHILE   = (char)275,
            PRINT   = (char)276,
            STRING  = (char)277,
            PUBLIC  = (char)278,
            PRIVATE = (char)279,
            OBJ   = (char)280,
            FUNC    = (char)281,
            NOT     =(char)282,
            LET    =(char)283,
            DEF     =(char)284,
            IMPORT  =(char)285,
            NAMESPACE=(char)286,
            CALL    =(char)287
        ;
    }

    public class Token
    {
        public char TagValue;

        public Token(char tag)
        {
            TagValue = tag;
        }

        public override string ToString()=>TagValue.ToString();
    }

    public class Int : Token
    {
        public int value;

        public Int(int val): base(Tag.INT)
        {
            value = val;
        }

        public override string ToString()=> value.ToString();
    }

    public class Word : Token
    {
        public string lexeme;

        public Word(string lexeme, char tag): base(tag)
        {
            this.lexeme = lexeme;
        }

        public override string ToString()=>lexeme;

        public static readonly Word
            And     = new Word("&&", Tag.AND),
            Or      = new Word("||", Tag.OR),
            Equal      = new Word("==", Tag.EQ),
            NotEqual      = new Word("!=", Tag.NE),
            Less      = new Word("<=", Tag.LE),
            Greater      = new Word(">=", Tag.GE),
            minus   = new Word("minus", Tag.MINUS),
            True    = new Word("true", Tag.TRUE),
            False   = new Word("false", Tag.FALSE),
            Not=new Word("!",Tag.NOT),
            temp    = new Word("t", Tag.TEMP);
    }

    public class Float : Token
    {
        public float value;

        public Float(float val): base(Tag.FLOAT)
        {
            value = val;
        }

        public override string ToString()
        {
            return  value.ToString(CultureInfo.InvariantCulture);
        }
    }

    public class String : Token
    {
        public string value;

        public String(string val) : base(Tag.STRING)
        {
            value = val;
        }

        public override string ToString()=>value;
    }
}
