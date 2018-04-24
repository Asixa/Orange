using System.Collections.Generic;
using Orange.Parse.Operation;
using Orange.Parse.Statements;
using static Orange.Generate.Generator;
using static Tag;

namespace Orange.Parse.Structure
{
    public class Class:Stmt
    {
        public string name;
        public ElementAtttibute atttibute;
       
        public readonly List<Def> defs = new List<Def>();
        public List<Variable> public_field=new List<Variable>();

        public  Method current_method;
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
                while (Check(DEF)) @class.defs.Add(Def.Match()as Def);
            }

            Match('}');
            return @class;
        }

        public void Generate(Namespace name_space)
        {
            name = name_space.name + "." + name;
            @this = new Morpheme(name, MorphemeAttribute.Class, this, null, new Core.Type(name, Tag.BASIC, 2));
            foreach (var def in defs)def.Generate(this);
            foreach (var method in methods)method.Generate(this);
        }

        public void Serialize()
        {
            binary_writer.Write(name);
            binary_writer.Write((int)atttibute);
            binary_writer.Write(methods.Count);
            foreach (var method in methods) method.Serialize();
        }


    }

    public class Variable:Stmt
    {
        public Core.Type type;
        //public LogicNode type_match;
        public string name;
        public Variable(Core.Type type,string name)
        {
            this.type = type;
            //this.type_match = type_match;
            this.name = name;
        }
    }
}
