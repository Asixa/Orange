using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Orange.Parse.New.Statements;
using Orange.Parse.Statements;
using Orange.Tokenize;
using ILGenerator = Orange.Compile.IL.ILGenerator;
using Type = Orange.Parse.Core.Type;

namespace Orange.Parse.New.Structure
{
    public class Func:Stmt
    {
        public bool isPublic;
        public string name;
        public Type returnType;
        public Stmt block;
        public struct Param
        {
            public Type type;
            public string name;
        }
        public List<Param>_params=new List<Param>();

        public static Func Match()
        {
            var function = new Func();
            Match(Tag.FUNC);
            function.name = _look.ToString();
            Match(Tag.ID);
            Match('<');
            Match('<');
            if (_look.TagValue == Tag.BASIC || _look.TagValue == Tag.ID)
            {
                function.returnType =Type.Match();
            }
            else function.returnType=Type.Void;
            Match('|');
            if (_look.TagValue == Tag.BASIC || _look.TagValue == Tag.ID)
            {
                function._params.Add(match_param());
                while(_look.TagValue == ',')
                {
                    Match(',');
                    function._params.Add(match_param());
                }
            }
            Match('>');
            Match('>');
            Match('[');
            function.block =Stmts.Match();
            Match(']');
            return function;
        }

        private static  Param match_param()
        {
            var param=new Param
            {
                type = Type.Match(),
                name =_look.ToString()
            };
            Match(Tag.ID);
            return param;
        }


        public void GenerateIL()
        {
            var param = new System.Type[_params.Count];
            for (var i = 0; i < _params.Count; i++)
            {
                param[i] = _params[i].type.SystemType;
            }
            ILGenerator.NewFunction(name,MethodAttributes.Static|MethodAttributes.Public,returnType.SystemType,param);
            block.GenerateIL();
            ILGenerator.IL.Emit(OpCodes.Ret);
        }
    }
}
