using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Orange.Debug;
using Type = Orange.Parse.Core.Type;

namespace Orange.Tokenize
{
    public class Lexer
    {
        public StreamReader reader;
        private char peek = ' ';
        public bool EofReached { get; private set; }
        public static int line = 1;
        public Dictionary<string, Word> KeyWords = new Dictionary<string, Word>();

        private void Reserve(Word word)=>KeyWords.Add(word.lexeme, word);
        
        public Lexer(StreamReader reader)
        {
            this.reader = reader;
            Reserve();
        }

        public void Error(string msg) => Debugger.Error(Debugger.Errors.Error+Debugger.Errors.Line+ ": " + msg);

        private bool ReadChar()
        {
            try
            {
                if (-1 == reader.Peek())
                {
                    EofReached = true;
                    return false;
                }

                peek = (char) reader.Read();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return true;
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
            for (; !EofReached; ReadChar())//去除空白
            {
                if (peek == ' ' || peek == '\t')
                {
                }
                else if (peek == '\r')
                {
                    ReadChar();
                    ++line;
                }
                else
                    break;
            }

            if (EofReached) return null;

            if (peek == '/') //检测注释
            {
                ReadChar();
                if (peek == '/')
                {
                    for (; ; ReadChar())
                    {
                        switch (peek)
                        {
                            case '\r':
                            case '\uffff':
                                return Scan();
                            default:
                                break;
                        }
                    }
                }
                if (peek == '*')
                {
                    ReadChar();
                    for (; ; ReadChar())
                    {
                        if (peek == '\r')
                        {
                            line++;
                            ReadChar();
                        }
                        if (peek == '*')
                        {
                            ReadChar();
                            if (peek == '/')
                            {
                                ReadChar();
                                return Scan();
                            }
                        }
                        if (peek == '\uffff')
                        {
                            return Scan();
                        }
                    }
                }
                
                return new Token('/');
            }

            //操作符
            switch (peek)
            {
                case '&':
                    return ReadChar('&') ? Word.And : new Token('&');
                case '|':
                    return ReadChar('|') ? Word.Or : new Token('|');
                case '=':
                    return ReadChar('=') ? Word.Equal : new Token('=');
                case '!':
                    return ReadChar('=') ? Word.NotEqual : new Token('!');
                case '<':
                    return ReadChar('=') ? Word.Less : new Token('<');
                case '>':
                    return ReadChar('=') ? Word.Greater : new Token('>');
            }

            if (peek == '"')//分析字符串
            {
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
                        default:
                            break;
                    }
                    s = s + peek;
                }
            }

            if (char.IsDigit(peek))//分析数字
            {
                var v = 0;
                do
                {
                    v = 10 * v + (peek - '0');
                    ReadChar();
                } while (char.IsDigit(peek));

                if (peek != '.') return new Int(v);

                float f = v;
                for (float d = 10;; d *= 10)
                {
                    ReadChar();
                    if (!char.IsDigit(peek)) break;
                    f += (peek - 48) / d;
                }

                return new Float(f);
            }

            if (char.IsLetter(peek))//分析标识符
            {
                var b = new StringBuilder();
                do
                {
                    b.Append(peek);
                    ReadChar();
                } while (char.IsLetterOrDigit(peek));

                var s = b.ToString();
                if (KeyWords.ContainsKey(s)) return KeyWords[s];
                return KeyWords[s] = new Word(s, Tag.ID);
            }

            if(peek=='/')Console.WriteLine("wc");
            var tok = new Token(peek);
            if (!EofReached) peek = ' ';
            return tok;
        }

        private void Reserve()
        {
            Reserve(new Word("if", Tag.IF));
            Reserve(new Word("else", Tag.ELSE));
            Reserve(new Word("while", Tag.WHILE));
            Reserve(new Word("do", Tag.DO));
            Reserve(new Word("break", Tag.BREAK));
            Reserve(new Word("print", Tag.PRINT));
            Reserve(new Word("public",Tag.PUBLIC));
            Reserve(new Word("private", Tag.PRIVATE));
            Reserve(new Word("obj",Tag.OBJ));
            Reserve(new Word("func",Tag.FUNC));
            Reserve(new Word("let",Tag.LET));
            Reserve(new Word("def",Tag.DEF));
            Reserve(new Word("import",Tag.IMPORT));
            Reserve(new Word("namespace",Tag.NAMESPACE));
            Reserve(new Word("call",Tag.CALL));
            Reserve(Word.True);
            Reserve(Word.False);
            Reserve(Type.Int);
            Reserve(Type.Char);
            Reserve(Type.Bool);
            Reserve(Type.Float);
            Reserve(Type.String);
        }
    }
}
