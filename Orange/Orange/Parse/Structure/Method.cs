using System;
using System.Collections.Generic;
using Orange.Debug;
using Orange.Generate;
using Orange.Parse.Core;
using Orange.Parse.Operation;
using Orange.Parse.Statements;
using static Orange.Generate.Generator;
using static Tag;
using Type = Orange.Parse.Core.Type;

namespace Orange.Parse.Structure
{
    public class Method : Stmt
    {
        public class Param
        {
            public LogicNode match;
            public Type type;
            public string name;
        }

        public string name;
        public ElementAtttibute atttibute;

        public Type @return=null;
        public LogicNode return_match;

        private Stmt statements;

        public readonly List<Param> @params = new List<Param>();
        public readonly List<Variable> locals = new List<Variable>();


        public readonly List<OpCode> codes = new List<OpCode>();
        public int CodeLine => codes.Count - 1;
        public void AddCode(ISet op,object param=null) => codes.Add(new OpCode(op, param));

        public Class @class;

        public void CheckParam()
        {
            foreach (var t in @params) t.type = t.match.Check(this);
        }
        public static Method Match(Class @class)
        {
            var function = new Method(){@class = @class};
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
                function.@params.Add(match_param());
                while (Check(','))
                {
                    Move();
                    function.@params.Add(match_param());
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
                match = Type.Match(),
                type =null,
                name = Look.ToString()
            };
            Match(ID);
            return param;
        }

        public int GetVar(string name, out int attribute)
        {
            var index = -1;
            attribute = 1;
            for (var i = 0; i < locals.Count; i++)
                if (locals[i].name == name)
                    index = i;
            if (index != -1) return index;

            attribute = 2;
            for (var i = 0; i < @class.public_field.Count; i++)
                if (@class.public_field[i].name == name)
                    index = i;
            return index;
        }

        public void Generate(Class @class)
        {
            @class.current_method = this;
            if (return_match != null&&@return==null) @return = Phrase.GetEnd(return_match).type;
            statements.Generate(this);
            Debugger.Message("-----"+name+"生成的字节码-------");
            for (var index = 0; index < locals.Count; index++) Debugger.Message(".["+index+"]"+locals[index].type+" "+locals[index].name);
            foreach (var code in codes)
                Debugger.Message(code.ToString());
            Debugger.Message("--------------------------");
        }

        public void Serialize()
        {
            binary_writer.Write(name);
            binary_writer.Write((int)atttibute);
            binary_writer.Write(codes.Count);
            foreach (var code in codes)
            {
                binary_writer.Write((int)code.opcode);
                binary_writer.Write(code.value?.ToString() ?? "");
            }
        }
    }
}
