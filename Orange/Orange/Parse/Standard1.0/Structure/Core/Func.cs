using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Orange.Parse.Core;
using Orange.Parse.New.Statements;
using Orange.Parse.Structure;
using Orange.Tokenize;
using Type = Orange.Parse.Core.Type;

namespace Orange.Parse.New.Structure
{
    public class Func:Stmt
    {
        public struct Param
        {
            public Identitifer type;
            public string name;
        }

        public bool isPublic;
        public string name;
        public Identitifer returnType;
        public Stmt block;
        public List<Param>_params=new List<Param>();

        public MethodBuilder builder;
        public ILGenerator generator;

        public static Func Match(Obj obj)
        {
            var function = new Func();
            Match(Tag.FUNC);
            function.name = _look.ToString();
            Match(Tag.ID);
            Match('(');
            
            if (_look.TagValue == Tag.BASIC || _look.TagValue == Tag.ID)
            {
                function.returnType =Type.Match();
            }
            else function.returnType=new Identitifer{Checked = true,type = Type.Void};
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
            Match(')');
            Match('{');
            function.block =Stmts.Match();
            Match('}');


            //声明一个空的函数
            var param = new System.Type[function._params.Count];
            for (var i = 0; i < function._params.Count; i++) param[i] = function._params[i].type.check().SystemType;
            function.builder = obj.builder.DefineMethod(function.name, MethodAttributes.Static | MethodAttributes.Public,
                function.returnType.check().SystemType, param);
            function.generator = function.builder.GetILGenerator();
            function.generator.Emit(OpCodes.Nop);

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


        public void Create(Obj obj)
        {
            generator.DeclareLocal(typeof(int));
            block.Create(generator);
            generator.Emit(OpCodes.Ret);
        }
    }
}
