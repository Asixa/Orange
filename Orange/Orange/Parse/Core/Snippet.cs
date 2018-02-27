using System.Collections.Generic;
using System.Reflection;
using Orange.Parse.New.Structure;

namespace Orange.Parse.Core
{
    public class Snippet
    {   
        public List<Quote> include = new List<Quote>();
        public List<string> types = new List<string>();

        public List<List<string>> typesNamespace = new List<List<string>>();

        public List<NameSpace>NamespaceDefine=new List<NameSpace>();

        public void GetAllType()
        {
            foreach (var t in include)
            {
                var group = new List<string>();
                
                foreach (var t2 in Compile.Compiler.includes)
                {
                    var asm = Assembly.LoadFile(t2);
                    foreach (var t3 in asm.GetTypes())
                    {
                        if (t.name != t3.Namespace) continue;
                        types.Add(t3.Name);
                        @group.Add(t3.Name);
                    }
                }
                typesNamespace.Add(group);
            }
        }
    }
}
