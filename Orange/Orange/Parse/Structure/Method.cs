using System;
using System.Collections.Generic;
using Orange.Debug;
using Orange.Generate;
using Orange.Parse.Core;
using Orange.Parse.Operation;
using static Orange.Generate.Generator;
using static Tag;
using Type = Orange.Parse.Core.Type;

namespace Orange.Parse.Structure
{
    public class Method : Stmt
    {
        private struct Param
        {
            public LogicNode match;
            public Type type;
            public string name;
        }

        public ElementAtttibute atttibute;
        public string name;
        public Type @return=null;
        public LogicNode return_match;
        private Stmt statements;
        private readonly List<Param> _params = new List<Param>();

        public readonly List<OpCode> codes = new List<OpCode>();
        public int CodeLine => codes.Count - 1;
        public void AddCode(ISet op,object param=null) => codes.Add(new OpCode(op, param));

        public static Method Match(Class @class)
        {
            var function = new Method();
            Match(FUNC);
            function.name = Look.ToString();
            Match(ID);
            Match('(');
            if (Check(BASIC) || Check(ID))
            {
                function.return_match = Type.Match();
            }
            else function.@return = Type.Void;

            Match('|');
            if (Check(BASIC) || Check(ID))
            {
                function._params.Add(match_param());
                while (Check(','))
                {
                    Match(',');
                    function._params.Add(match_param());
                }
            }

            Match(')');
            Match('{');
            function.statements = Stmts.Match();
            Match('}');
            return function;
        }
        private static Param match_param()
        {
            var param = new Param
            {
                match = Phrase.Match(),
                type =null,
                name = Look.ToString()
            };
            Match(ID);
            return param;
        }

        public void Generate(Class @class)
        {
            if (return_match != null&&@return==null) @return = Phrase.GetEnd(return_match).type;
            statements.Generate(this);
            Debugger.Message("-----"+name+"生成的字节码-------");
            foreach (var code in codes)
                Console.WriteLine(code.ToString());
            Debugger.Message("--------------------------");
        }

        public void Serialize()
        {
            binary_writer.Write(name);
            binary_writer.Write((int)atttibute);
            binary_writer.Write(name);

            binary_writer.Write(codes.Count);
            foreach (var code in codes)
            {
                binary_writer.Write((int)code.opcode);
                binary_writer.Write(code.value?.ToString() ?? "");
            }
        }

        public static Method Deserialize()
        {
            var method = new Method
            {
                name = binary_reader.ReadString(),
                atttibute = (ElementAtttibute) binary_reader.ReadInt32()
            };
            for (var i = 0; i < binary_reader.ReadInt32(); i++)
            {
                var iset = (ISet) binary_reader.ReadInt32();
                var val = binary_reader.ReadString();
                method.AddCode(iset,val==""?null:val);
            }

            return method;
        }
    }
}
