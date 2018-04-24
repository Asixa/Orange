using System;
using System.Collections.Generic;
using static Orange.Generate.Generator;
using static Orange.Interprete.Interpreter;
namespace Orange.Interprete.Runtime
{
    class RTMethod
    {
        public ElementAtttibute atttibute;
        public string name;
        public readonly List<OpCode> codes = new List<OpCode>();
        public void AddCode(ISet op, object param = null) => codes.Add(new OpCode(op, param));
        public static RTMethod Deserialize()
        {
            var method = new RTMethod
            {
                name = binary_reader.ReadString(),
                atttibute = (ElementAtttibute) binary_reader.ReadInt32()
            };
            var count = binary_reader.ReadInt32();
            for (var i = 0; i <count ; i++)
            {
                var iset = (ISet) binary_reader.ReadInt32();
                var val = binary_reader.ReadString();
                method.AddCode(iset, val == "" ? null : val);
            }

            return method;
        }
    }
}
