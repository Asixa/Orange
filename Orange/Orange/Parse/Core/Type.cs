using System.Collections.Generic;
using Orange.Debug;
using Orange.Parse.Statements;
using Orange.Parse.Structure;
using Orange.Tokenize;

namespace Orange.Parse.Core
{
    public class Env
    {
        private readonly Dictionary<Token, Variable> symbol_table;
        private readonly Env prev;

        public Env(Env prev)
        {
            symbol_table = new Dictionary<Token, Variable>();
            this.prev = prev;
        }

        public void AddIdentifier(Token tok, Variable id)
        {
            symbol_table.Add(tok, id);
        }

        public Variable Get(Token tok)
        {
            for(var e = this; e != null; e = e.prev)
                if (e.symbol_table.ContainsKey(tok)) return e.symbol_table[tok];
            return null;
        }
    }

    public class Type : Word
    {
        public readonly int width;
        public readonly System.Type system;
        public string name_space;

        public Type(string type_name, char tag, int width,System.Type t): base(type_name, tag) 
        { 
            this.width = width;
            system = t;
        }

        public static readonly Type
            Void    =   new Type("void",Tag.BASIC,1,typeof(void)),
            Int     =   new Type("int",     Tag.BASIC, 4,typeof(int)),
            Float   =   new Type("float",   Tag.BASIC, 8,typeof(float)),
            Char    =   new Type("char",    Tag.BASIC, 1,typeof(char)),
            Bool    =   new Type("bool",    Tag.BASIC, 1,typeof(bool)),
            String  =   new Type("string",  Tag.BASIC,1,typeof(string));

        private static bool Numeric(Type type) =>type == Char || type == Int || type == Float; 
        
        public static Type Max(Type lhs, Type rhs)
        {
            if (lhs == String || rhs == String)return String;

            if (!Numeric(lhs) || !Numeric(rhs))return null;

            if (lhs == Float || rhs == Float)return Float;

            if (lhs == Int || rhs == Int)return Int;
           
            return Char;
        }

        public static Identitifer Match()//可返回类型，也可返回字段，也可返回属性
        {
            var identitifer=new Identitifer();
            if (Node._look.tag_value == Tag.BASIC)
            {
                identitifer.element.Add(Node._look.ToString());
                identitifer.type = Node._look as Type;
                identitifer.Checked = true;
                identitifer.is_type = true;
                Node.Match(Tag.BASIC);
            }
            else
            {
                identitifer.element.Add(Node._look.ToString());
                Node.Match(Tag.ID);

                while (Node._look.tag_value == '.')
                {
                    Node.Match('.');
                    identitifer.element.Add(Node._look.ToString());
                    Node.Match(Tag.ID);
                }
            }

            return identitifer;
        }

        public static Type Dimension(Type type)
        {
            
            Node.Match('[');
            var tok = Stmt._look;
            Node.Match(Tag.INT);
            Node.Match(']');

            if (Node._look.tag_value == '[')
                type = Dimension(type);
            return null;
            //return new Array(((Int)tok).Value, type);
        }

        public static void Error(string msg) => Debugger.Error("[ERROR] line " + Lexer.line + ": " + msg);
    }

    public class Identitifer
    {
        public bool is_type,Checked;
        public string name;
        public Type type;
        public readonly List<string> element=new List<string>();                                                 //存着的所有段
       

        public object reflect;                                                                          // 这个存个奇怪的东西，有时候会用到
        public override string ToString() => "[" + name + "," + (is_type ? "类型" : "非类型") + "," + type.system+"]";

        public Type Check()
        {

            if (Checked) return type;
            var header = element[0];
            switch (CheckHeader(header, out var OUT))
            {
                case 1: break;                                                                          //这个标识符是个本地变量
                case 2: break;                                                                          //这个标识符是本地已经声明的类
                case 3:                                                                                 //这个标识符是本地已经声明的函数
                    if (element.Count > 1) Node.Error(Debugger.Errors.ImpossibleError);
                    else
                    {
                        name = element[0];
                        reflect = ((Func)OUT).builder;
                        type = ((Func)OUT).returnType.Check();
                        Checked = true;
                    }
                    break;
                case 4:                                                                                 //这个标识符是外部dll的
                    var _namespace = "";
                    name = header;
                    for (var i = 0; i < Parser.current.snippet.types_namespace.Count; i++)
                        if (Parser.current.snippet.types_namespace[i].Contains(header))
                            _namespace = Parser.current.snippet.include[i].name;
                    var head = System.Type.GetType(_namespace + "." + header);
                    var father = head;
                    for (var i = 1; i < element.Count; i++)
                    {
                        name += "." + element[i];
                        father = SystemCheckNext(father, element[i]);
                    }

                    type = new Type(name, Tag.BASIC, 4, father) { name_space = _namespace };
                    is_type = System.Type.GetType(_namespace + "." + name) != null;
                    Checked = true;
                    break;
                default:
                    break;
            }

            return type;
        }

        private static int CheckHeader(string header,out object OUT)
        {
            OUT = null;
                                                                             //TODO 检查是不是变量           return 1;
                                                                           
            foreach (var snippet in Parser.Snippets)                         //检查是不是本地函数            return 3;
            {
                foreach (var name_space in snippet.namespace_define)
                {
                    foreach (var obj in name_space.objs)
                    {
                        if (obj.name == header)
                        {
                            return 2;                                       // 检查是不是已经声明了的类      return 2;
                        }
                        foreach (var function in obj.functions)
                        {
                            if (function.name != header) continue;
                            OUT = function;
                            return 3;
                        }
                    }
                }
            }

            if (Parser.current.snippet.types.Contains(header))return 4;      // 检查是不是引用dll的的类      return 2;
            Node.Error(Debugger.Errors.UnkownType);
            return -1;
        }

        private static System.Type SystemCheckNext(System.Type father, string next)
        {
            var type = System.Type.GetType(father.FullName + "." + next);       //检测此段是否是 类型
            if (type != null)return type;
            
            foreach (var method_info in father.GetMethods())                    //检测此段是否是 函数
                if (method_info.Name == next)
                    return method_info.ReturnType;
            
            var field = father.GetField(next);                                  //检测此段是否是 字段
            if (field != null)return field.FieldType;

            var property = father.GetProperty(next);                            //检测此段是否是 属性
            if (property != null)return property.PropertyType;

            Node.Error(Debugger.Errors.UnkownType);                             //没有检测到，报错
            return null;
        }
    }
}
