using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Orange.Compile;
using Orange.Generate;
using Orange.Parse.Core;
using Orange.Parse.Operation;
using static Orange.Generate.Generator;
using static Tag;

namespace Orange.Parse.Structure
{
    public class Class:Stmt
    {
        public ElementAtttibute atttibute;
        public string name;
        public readonly List<Method> methods=new List<Method>();
        public static Class Match(Namespace name_space)
        {
            var @class=new Class();
            Match(OBJ);
            @class.name = Look.ToString();
            Match(ID);
            Match('{');
            while (Check(DEF) || Check(FUNC))
            {
                while (Check(FUNC)) @class.methods.Add(Method.Match(@class));
                while (Check(DEF)) @class.methods.Add(Method.Match(@class));
            }

            Match('}');
            return @class;
        }

        public void Generate(Namespace name_space)
        {
            name = name_space.name + "." + name;
            @this = new Morpheme(name, MorphemeAttribute.Class, this, null, new Core.Type(name, Tag.BASIC, 2));
            foreach (var method in methods)method.Generate(this);
        }

        public void Serialize()
        {
            binary_writer.Write(name);
            binary_writer.Write((int)atttibute);
            binary_writer.Write(methods.Count);
            foreach (var method in methods) method.Serialize();
        }

        public static Class Deserialize()
        {
            var @class = new Class
            {
                name = binary_reader.ReadString(),
                atttibute = (ElementAtttibute) binary_reader.ReadInt32()
            };
            for (var i = 0; i < binary_reader.ReadInt32(); i++)
            @class.methods.Add(Method.Deserialize());
            return @class;
        }
    }
}
