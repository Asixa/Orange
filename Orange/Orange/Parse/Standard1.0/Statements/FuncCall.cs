using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Orange.Parse.New.Operation;
using ILGenerator = Orange.Compile.IL.ILGenerator;
using Type = Orange.Parse.Core.Type;

namespace Orange.Parse.New.Statements
{
    public class FuncCall : Stmt
    {
        public Type name;
        public List<LogicNode> _params;

        private string typeName,MethodName;

        public static List<Expr> ParamInvoke()
        {
            var e = new List<Expr>();
            Match('(');
            e.Add(ExprC.Bool());
            while (_look.TagValue == ',')
            {
                Match(',');
                e.Add(ExprC.Bool());
            }
            Match(')');
            return e;
        }

        public new static Stmt Match()
        {
            var func_call = new FuncCall
            {
                name = Type.Match(),
            };
            Match('<');
            Match('<');
            if (_look.TagValue == '#')
            {
                Match('#');
                Match(';');
                func_call._params=new List<LogicNode>();
                return func_call;
            }

            func_call._params = new List<LogicNode> {BoolTree.Match()};
            while (_look.TagValue==',')
            {
                Match(',');
                func_call._params.Add(BoolTree.Match());
            }
            Match(';');
            return func_call;
        }

        public override void GenerateIL()
        {
            analyze();
            MethodInfo method;
            if (_params.Count != 0)
            {
                ILGenerator.IL.Emit(OpCodes.Ldstr, _params[0].ToString());
                var param = new System.Type[_params.Count];
                for (var i = 0; i < param.Length; i++) param[i] = _params[i].Type.SystemType;
             
                method= System.Type.GetType(typeName)?.GetMethod(MethodName,param);
            }
            else
            {
               
                method = System.Type.GetType(typeName)?.GetMethod(MethodName);
            }
            ILGenerator.IL.Emit(OpCodes.Call,method);
            ILGenerator.IL.Emit(method.ReturnType == typeof(void)? OpCodes.Nop:OpCodes.Pop);
        }

        public void analyze()
        {
            var t = name.ToString();
            var p = t.Split('.');
            MethodName = p[p.Length - 1];
            typeName =name.nameSpace+"."+ t.Substring(0, t.LastIndexOf("."));
        }

    }
}
