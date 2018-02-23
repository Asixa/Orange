using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Type = Orange.Parse.Type;

namespace Orange.Tokenize
{
    public class Lexer
    {
        public StreamReader _reader;
        private char peek = ' ';
        public bool EofReached { get; private set; }
        public static int Line = 1;
        public Dictionary<string, Word> KeyWords = new Dictionary<string, Word>();

        private void Reserve(Word word)=>KeyWords.Add(word.Lexeme, word);
        
        public Lexer(StreamReader reader)
        {
            _reader = reader;
            Reserve();
        }

        public void Error(string msg) => Debug.Debugger.Error("[ERROR] line " + Line + ": " + msg);

        private bool ReadChar()
        {
            try
            {
                if (-1 == _reader.Peek())
                {
                    EofReached = true;
                    return false;
                }

                peek = (char) _reader.Read();
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
            //white spaces
            for (; !EofReached; ReadChar())
            {
                if (peek == ' ' || peek == '\t')
                {
                }
                else if (peek == '\r')
                {
                    ReadChar();
                    ++Line;
                }
                else
                    break;
            }

            if (EofReached) return null;
            //注释
            if (peek == '/') //开始检测注释
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
                            Line++;
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

            //operators
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

            //strings
            if (peek == '"')
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

            //numbers
            if (char.IsDigit(peek))
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

            //identifiers
            if (char.IsLetter(peek))
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
            Reserve(new Word("class",Tag.CLASS));
            Reserve(Word.True);
            Reserve(Word.False);
            Reserve(Parse.Type.Int);
            Reserve(Parse.Type.Char);
            Reserve(Parse.Type.Bool);
            Reserve(Parse.Type.Float);
            Reserve(Parse.Type.String);
        }
    }
}
