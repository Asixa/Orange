using System.Collections.Generic;
using System.Reflection;
using static Orange.Debug.Debugger;
using static Tag;
namespace Orange.Parse.Structure
{
    public class Quote:Stmt
    {
        public readonly string name;
        private static readonly List<string> AvaliableNamespaces = new List<string>();

        private Quote(string name)
        {
            this.name = name;
            if(!AvaliableNamespaces.Contains(name))Error("未知的命名空间");
        }

        public static void GetAllAvaliableNameSpace()
        {
            //FIX ME 
            foreach (var dll in Compile.Compiler.Dlls)
            {
                foreach (var type in Assembly.LoadFile(dll).GetTypes())
                {
                    var levels = GetNameSpaceLevels(type.Namespace);
                    if (levels.Count <= 0) continue;
                    foreach (var item in levels)
                    {
                        if(!AvaliableNamespaces.Contains(item))
                        AvaliableNamespaces.Add(item);
                    }
                }
            }
        }
        private static List<string> GetNameSpaceLevels(string name_space)
        {
            var str_copy = name_space;
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
            list.Add(name_space);
            return list;
        }


        public new static void Match()
        {
            Match(IMPORT);
            Match('<');
            Snippet.include.Add(new Quote(MatchSingleNamespace()));
            while (Look.tag_value==',')
            {
                Match(',');
                Snippet.include.Add(new Quote(MatchSingleNamespace()));
            }
            Match('>');
        }

        private static string MatchSingleNamespace()
        {
            var _namespace = Look.ToString();
            Match(ID);
            while (Check('.'))
            {
                Move();
                _namespace += "." + Look;
                Match(ID);
            }
            return _namespace;
        }
    }
}
