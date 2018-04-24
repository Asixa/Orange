
using System;
using System.Collections.Generic;
using static Orange.Generate.Generator;
using static Orange.Interprete.Interpreter;
namespace Orange.Interprete.Runtime
{
    class RTClass
    {
        public string name;
        public ElementAtttibute atttibute;
        public readonly List<RTMethod>methods=new List<RTMethod>();


        public static RTClass Deserialize()
        {
            var @class = new RTClass
            {
                name = binary_reader.ReadString(),
                atttibute = (ElementAtttibute) binary_reader.ReadInt32()
            };
            var count = binary_reader.ReadInt32();
            for (var i = 0; i < count; i++)
                @class.methods.Add(RTMethod.Deserialize());
            return @class;
        }
    }
}
