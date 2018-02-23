using System;
using System.Collections.Generic;
using System.Reflection;
using Orange.Tokenize;

namespace Orange.Parse.Core
{
    public class Quote:Stmt
    {
        public string name;
        public Quote(string name)
        {
            this.name = name;
            if(!namespaces.Contains(name))Debug.Debugger.Error("未知的命名空间");
        }

        public static List<string>namespaces=new List<string>();

        public static void GetAllAvaliableNameSpace()
        {
            foreach (var t in Compile.Compiler.includes)
            {
                
                var asm = Assembly.LoadFile(t);
                var types = asm.GetTypes();
                foreach (var type in types)
                {
                    var lvs = GetNameSpaceLevels(type.Namespace);
                    if (lvs.Count <= 0) continue;
                    foreach (var t1 in lvs)
                    {
                        if(!namespaces.Contains(t1))
                        namespaces.Add(t1);
                    }
                }
            }
        }

        static List<string> GetNameSpaceLevels(string nameSpace)
        {
            var strCopy = nameSpace;
            var list = new List<string>();
            if (strCopy == null)
            {
                return list;
            }
            while (true)
            {
                var dotPos = strCopy.LastIndexOf('.');
                if (dotPos < 0)
                {
                    break;
                }
                var currentSpace = strCopy.Substring(0, dotPos);
                list.Insert(0, currentSpace);
                strCopy = currentSpace;
            }
            list.Add(nameSpace);
            return list;
        }

        public new static void Match()
        {
            while (_look.TagValue == '#') 
            {
                Match('#');
                if (_look.TagValue == '<')
                {
                    Match('<');
                    var tok = _look;
                    string _namespace = "";
                    Match(Tag.ID);
                    _namespace += tok.ToString();

                    while (_look.TagValue == '.')
                    {
                        Match('.');
                        _namespace += ".";
                        tok = _look;
                        Match(Tag.ID);
                        _namespace += tok.ToString();
                    }

                    Match('>');
                    snippet.include.Add(new Quote(_namespace));
                }
                else
                {
                    Console.WriteLine(_look.TagValue);
                    Error("syntax error");
                }
            } while (_look.TagValue == '#')  //D -> type ID
            {
                Match('#');
                if (_look.TagValue == '<')
                {
                    Match('<');
                    var tok = _look;
                    string _namespace = "";
                    Match(Tag.ID);
                    _namespace += tok.ToString();

                    while (_look.TagValue == '.')
                    {
                        Match('.');
                        _namespace += ".";
                        tok = _look;
                        Match(Tag.ID);
                        _namespace += tok.ToString();
                    }

                    Match('>');
                    snippet.include.Add(new Quote(_namespace));
                }
                else
                {
                    Console.WriteLine(_look.TagValue);
                    Error("syntax error");
                }
            }
        }
    }
}
