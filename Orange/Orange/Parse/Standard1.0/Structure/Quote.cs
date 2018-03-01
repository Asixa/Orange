using System;
using System.Collections.Generic;
using System.Reflection;
using Orange.Tokenize;

namespace Orange.Parse.New.Structure
{
    public class Quote:Stmt
    {
        public string name;
        public Quote(string name)
        {
            this.name = name;
            if(!AvaliableNamespaces.Contains(name))Debug.Debugger.Error("未知的命名空间");
        }

        public static List<string>AvaliableNamespaces=new List<string>();
        

        public static void GetAllAvaliableNameSpace()
        {
            foreach (var t in Compile.Compiler.dlls)
            {
                
                var asm = Assembly.LoadFile(t);
                var types = asm.GetTypes();
                foreach (var type in types)
                {
                    var lvs = GetNameSpaceLevels(type.Namespace);
                    if (lvs.Count <= 0) continue;
                    foreach (var t1 in lvs)
                    {
                        if(!AvaliableNamespaces.Contains(t1))
                        AvaliableNamespaces.Add(t1);
                    }
                }
            }
        }

        private static List<string> GetNameSpaceLevels(string nameSpace)
        {
            var str_copy = nameSpace;
            var list = new List<string>();
            if (str_copy == null)
            {
                return list;
            }
            while (true)
            {
                var dot_pos = str_copy.LastIndexOf('.');
                if (dot_pos < 0)
                {
                    break;
                }
                var current_space = str_copy.Substring(0, dot_pos);
                list.Insert(0, current_space);
                str_copy = current_space;
            }
            list.Add(nameSpace);
            return list;
        }

        public new static void Match()
        {
            Match(Tag.IMPORT);
            Match('<');
            snippet.include.Add(new Quote(MatchSingleNamespace()));
            while (_look.TagValue==',')
            {
                
                Match(',');
                snippet.include.Add(new Quote(MatchSingleNamespace()));
            }
            
            Match('>');
        }

        public static string MatchSingleNamespace()
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
