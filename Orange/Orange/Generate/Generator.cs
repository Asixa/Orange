using System;
using System.Collections.Generic;
using System.IO;
using Orange.Parse;
using Orange.Parse.Operation;
using Orange.Parse.Structure;

// ReSharper disable InconsistentNaming
public enum ISet
{
    Jump,
    Class,
    Func,
    Class_end,
    Func_end,
    Add,
    Minus,
    Multiply,
    Divide,
    Call,
    Push_value,
    Push_local,
    Push_field,
    Equal,
    Less,
    Greater,
    Negate,
    Or,
    And,
    Storeloc,
    StoreField
}
namespace Orange.Generate
{
    
    public static class Generator
    {

        public static readonly List<Namespace>name_spaces=new List<Namespace>();
        public static BinaryWriter binary_writer;
        public static Morpheme @this;
        public static void Serialize()
        {
            binary_writer= new BinaryWriter(new FileStream("a.ore", FileMode.Create));
            binary_writer.Write(name_spaces.Count);
            foreach (var name_space in name_spaces)name_space.Serialize();
            binary_writer.Close();
        }



        [Flags]
        public enum ElementAtttibute
        {
            STATIC = 1,
            PUBLIC = 2,
            PRIVATE = 4
        }

        public class OpCode
        {
            public OpCode(ISet op, object val)
            {
                opcode = op;
                value = val;
            }

            public readonly ISet opcode;
            public readonly object value;
            public override string ToString() => opcode + " " + value;
        }
    }



}
