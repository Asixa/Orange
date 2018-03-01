using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Orange.Debug;
using Orange.Parse.Core;
using Orange.Parse.New.Operation;
using Type = Orange.Parse.Core.Type;

namespace Orange.Parse.New.Statements
{
    public class FuncCall : Stmt
    {
        public Identitifer function;
        public List<LogicNode> _params;

        private string typeName,MethodName;

        public new static Stmt Match()
        {
            var func_call = new FuncCall
            {
                function = Type.Match(),
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

        public override void Create(ILGenerator generator)
        {
            //处理ID，处理类型名称和方法名称
            function.Check();                                                                           //语义分析标识符

            if (function.reflect != null)
            {                            
                var builder = (MethodBuilder) function.reflect;                                         //获取这个ID对应的函数builder
                generator.Emit(OpCodes.Call, builder);                                                  //生成调用函数的中间码
                generator.Emit(builder.ReturnType == typeof(void)                                       //TODO 这个是测试用的，不对函数返回值作任何操作
                    ? OpCodes.Nop
                    : OpCodes.Pop);  
                return;
            }
            else                                                                                        //如果匹配到的的标识符不是本地函数
            {
                MethodInfo method;
                var split = function.name.Split('.');                                                   //分割标识符
                MethodName = split[split.Length - 1];                                                   //方法名称为标识符最后一个元素
                typeName = split.Length == 1 ? "" : function.type.name_space +
                    "." + function.name.Substring(0, function.name.LastIndexOf("."));                   //类型名称为除了最后一个元素的其他元素
                if (_params.Count != 0)                                                                 //如果这个函数有参数
                {
                    generator.Emit(OpCodes.Ldstr, _params[0].ToString());                               //TODO 将String参数值置入栈中

                    var param = new System.Type[_params.Count];                                         //参数类型数组
                    for (var i = 0; i < param.Length; i++) param[i] = _params[i].Type.system;           //将_param数组转换为系统类型数组
                    method = System.Type.GetType(typeName)?.GetMethod(MethodName, param);               //获取目标类型
                }
                else
                {
                    method = System.Type.GetType(typeName)?.GetMethod(MethodName);                      //直接获取目标类型
                }
                if (method == null) Error(Debugger.Errors.UnknownFunction);                                                   //如果没有找到这个函数，说明有错误
                generator.Emit(OpCodes.Call, method);                                                   //生成调用函数的中间码
                generator.Emit(method.ReturnType == typeof(void) ? OpCodes.Nop : OpCodes.Pop);          //TODO 这个是测试用的，不对函数返回值作任何操作
            }
        }
    }
}
