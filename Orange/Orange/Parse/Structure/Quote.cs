using System.Collections.Generic;
using System.Reflection;
using Orange.Tokenize;
namespace Orange.Parse.New.Structure
{
    public class Quote:Stmt
    {
        public string name;
        public static List<string> avaliable_namespaces = new List<string>();
        public Quote(string name)
        {
            this.name = name;
            if(!avaliable_namespaces.Contains(name))Debug.Debugger.Error("未知的命名空间");
        }

        public static void GetAllAvaliableNameSpace()
        {
            foreach (var dll in Compile.Compiler.dlls)
            {
                foreach (var type in Assembly.LoadFile(dll).GetTypes())
                {
                    var levels = GetNameSpaceLevels(type.Namespace);
                    if (levels.Count <= 0) continue;
                    foreach (var item in levels)
                    {
                        if(!avaliable_namespaces.Contains(item))
                        avaliable_namespaces.Add(item);
                    }
                }
            }
        }
        private static List<string> GetNameSpaceLevels(string nameSpace)
        {
            var str_copy = nameSpace;
            var list = new List<string>();
            if (str_copy == null)return list;
            while (true)
            {
                var dot_pos = str_copy.LastIndexOf('.');
                if (dot_pos < 0)break;
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
                Move();
                _namespace += "." + _look;
                Match(Tag.ID);
            }
            return _namespace;
        }
    }
}
