using System.Reflection.Emit;
using Orange.Debug;
using Orange.Parse.Core;
using Orange.Parse.New.Operation;
using Orange.Tokenize;
using Type = Orange.Parse.Core.Type;

namespace Orange.Parse.Standard1._0.Statements
{
    public class Let : Stmt
    {
        public Identitifer target;
        public LogicNode value;

        private string typeName, FieldName;

        public static Stmt Match()
        {
            var let = new Let();
            Match(Tag.LET);
            let.target = Type.Match();
            Match('=');
            let.value = BoolTree.Match();
            Match(';');
            return let;
        }

        public override void Create(ILGenerator generator)
        {                                                                                       //TODO 这里需要支持其他的类型和进行表达式运算
            if (value.Type == Type.String)                                                      //赋值String时候用的
            {
                generator.Emit(OpCodes.Ldstr, value.ToString());                                //将String值置入栈中
            }
            else if(value.Type == Type.Int)
            {
                generator.Emit(OpCodes.Ldc_I4,value.ToString());
            }

            target.Check();                                                                     //语义分析被赋值标识符
            var split = target.name.Split('.');                                                 //分割被赋值标识符
            FieldName = split[split.Length - 1];                                                //字段名称为标识符最后一个元素
            typeName = target.type.name_space + "." +                                           //类型名称为标识符除了最后以外的其他元素
                target.name.Substring(0, target.name.LastIndexOf("."));

            var type = System.Type.GetType(typeName);
            var field = type.GetField(FieldName);
            if (field != null)                                                                  //*赋值的是字段
            {
                generator.Emit(OpCodes.Stsfld, field);
            }
            else                                                                                //*赋值的是属性
            {
                var property = type.GetProperty(FieldName);
                if (property == null)
                {
                    Error(Debugger.Errors.UnknownField);
                    return;
                }

                var call = System.Type.GetType(typeName)
                    ?.GetMethod("set_" + FieldName, new[] {value.Type.system});
                generator.Emit(OpCodes.Call, call);
                generator.Emit(OpCodes.Nop);
            }
        }
    }
}
