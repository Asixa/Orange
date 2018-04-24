
using System;
using System.Collections.Generic;
using static Orange.Interprete.Interpreter;
namespace Orange.Interprete.Runtime
{
    class RTNamespace
    {
        public string name;
        public List<RTClass>classes=new List<RTClass>();
        public List<RTNamespace>subspaces=new List<RTNamespace>();

        public static RTNamespace Deserialize()
        {
            var @namespace = new RTNamespace {name = binary_reader.ReadString()};
            var count = binary_reader.ReadInt32();
            for (var i = 0; i < count; i++)
                @namespace.classes.Add(RTClass.Deserialize());
            return @namespace;
        }
    }
}
