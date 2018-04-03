using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Orange.Compile;
using Orange.Tokenize;

namespace Orange.Parse.Structure
{
    public class Obj:Stmt
    {
        //实例参数
        public bool isPublic;
        public string name;
        public List<Func> functions=new List<Func>();
        //实例反射
        public TypeBuilder builder;

        public static Obj Match(NameSpace name_space)
        {
            var obj=new Obj();
            Match(Tag.OBJ);
            obj.name = _look.ToString();
            Match(Tag.ID);
            Match('{');

            //生成一个类
            obj.builder = Compiler.module.DefineType(name_space.name +"."+ obj.name, TypeAttributes.Public);

            while (_look.tag_value==Tag.FUNC)obj.functions.Add(Func.Match(obj));   
            Match('}');
            return obj;
        }

        public void Create(NameSpace name_space)
        {
            foreach (var t in functions)t.Create(this);
            builder.CreateType();
            //检查主函数
            var Main = builder.GetMethod("Main");
            if(Main!=null)
            Compiler.assembly.SetEntryPoint(Main);
        }
    }
}
