using System;
using System.Collections.Generic;
using System.Reflection;
using Orange.Parse.Core;

namespace Orange.Parse
{
    public class Snippet
    {   
        public List<Quote> include = new List<Quote>();
        public List<string> types = new List<string>();

        public void GetAllType()
        {
            foreach (var t in include)
            {
                foreach (var t2 in Compile.Compiler.includes)
                {
                    var asm = Assembly.LoadFile(t2);
                    foreach (var t3 in asm.GetTypes())
                    {
                        if (t.name == t3.Namespace)
                        {
                           // Console.WriteLine(t3.Name);
                            types.Add(t3.Name);
                        }
                    }
                }
            }
        }
    }
}
