using System;
using System.Collections.Generic;
using static Orange.Generate.Generator;
using static Tag;

namespace Orange.Parse.Structure
{
    public class Namespace:Stmt
    {
        public string name;
        public readonly List<Class> classes=new List<Class>();
        public readonly List<Namespace>sub_spaces=new List<Namespace>();
        public new static Namespace Match()
        {
            var space=new Namespace();
            Match(NAMESPACE);
            space.name = Look.ToString();
            Match(ID);
            if (Check('-'))
            {
                Match('-');
                Match('>');
                space.classes.Add(Class.Match(space));
            }
            else
            {
                Match('{');
                while (Check(OBJ)) space.classes.Add(Class.Match(space));
                Match('}');
            }
            name_spaces.Add(space);
            return space;
        }

        public void Generate()
        {
            foreach (var @class in classes)@class.Generate(this);
        }

        public void Serialize()
        {
            binary_writer.Write(name);
            binary_writer.Write(classes.Count);
            foreach (var @class in classes) @class.Serialize();
        }

        public static Namespace Deserialize()
        {
            var @namespace = new Namespace {name = binary_reader.ReadString()};
            for (var i = 0; i < binary_reader.ReadInt32(); i++)
            @namespace.classes.Add(Class.Deserialize());
            return @namespace;
        }
    }
}
