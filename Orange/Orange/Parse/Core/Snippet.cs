using System.Collections.Generic;
using System.Reflection;
using Orange.Parse.Structure;

namespace Orange.Parse.Core
{
    public class Snippet
    {   
        public readonly List<Quote> include = new List<Quote>();                             //此片段引用的命名空间
        public readonly List<string> types = new List<string>();
        public readonly List<List<string>> types_namespace = new List<List<string>>();       //保存每个type所属的命名空间
        public readonly List<NameSpace>namespace_define=new List<NameSpace>();               //所有匹配到的命名空间结构

        public void GetAllType()
        {
            foreach (var quote in include)
            {
                var group = new List<string>();
                
                foreach (var dll in Compile.Compiler.Dlls)
                {
                    var asm = Assembly.LoadFile(dll);
                    foreach (var type in asm.GetTypes())
                    {
                        if (quote.name != type.Namespace) continue;
                        types.Add(type.Name);
                        @group.Add(type.Name);
                    }
                }
                types_namespace.Add(group);
            }
        }
    }
}