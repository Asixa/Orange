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
            Match('&');
            Match('<');
            snippet.include.Add(new Quote(MatchNamespace()));
            while (_look.TagValue==',')
            {
                
                Match(',');
                snippet.include.Add(new Quote(MatchNamespace()));
            }
            
            Match('>');
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("引用命名空间:");
            foreach (var t in snippet.include)Console.Write(t.name+"|");
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static string MatchNamespace()
        {
            var _namespace = _look.ToString();
            Match(Tag.ID);
            while (_look.TagValue == '.')
            {
                Match('.');
                _namespace += "." + _look;
                Match(Tag.ID);
            }
            return _namespace;
        }
    }
}
