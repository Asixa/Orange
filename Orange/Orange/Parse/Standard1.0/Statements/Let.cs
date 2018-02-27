using System;
using System.Reflection.Emit;
using Orange.Parse.New.Operation;
using Orange.Tokenize;
using ILGenerator = Orange.Compile.IL.ILGenerator;
using Type = Orange.Parse.Core.Type;

namespace Orange.Parse.Standard1._0.Statements
{
    public class Let:Stmt
    {
        public Type target;
        public LogicNode value;

        private string typeName, FieldName;
        public static Stmt Match()
        {
            var let=new Let();
            Match(Tag.LET);
            let.target = Type.Match();
            Match('=');
            let.value = BoolTree.Match();
            Match(';');
            return let;
        }

        public override void GenerateIL()
        {
            if (value.Type == Type.String)
            {
                ILGenerator.IL.Emit(OpCodes.Ldstr, value.ToString());
                analyze();

                var type = System.Type.GetType(typeName);
                var field = type.GetField(FieldName);
                if (field != null)
                {
                    ILGenerator.IL.Emit(OpCodes.Stsfld, field);
                }
                else
                {
                    var property = type.GetProperty(FieldName);
                    if (property == null) return;
                    var call = System.Type.GetType(typeName)
                        ?.GetMethod("set_" + FieldName, new[] {value.Type.SystemType});
                    ILGenerator.IL.Emit(OpCodes.Call, call);
                    ILGenerator.IL.Emit(OpCodes.Nop);
                }
            }
        }

        public void analyze()
        {
            var t = target.ToString();
            var p = t.Split('.');
            FieldName = p[p.Length - 1];
            typeName = target.nameSpace + "." + t.Substring(0, t.LastIndexOf("."));
        }
    }
}
