using System;
using System.Collections.Generic;
using Orange.Generate;
using Orange.Parse.Structure;
using Orange.Tokenize;
using Type = Orange.Parse.Core.Type;
using static Orange.Debug.Debugger;
using static Tag;

namespace Orange.Parse.Operation
{
    [Flags]
    public enum MorphemeAttribute
    {
        None = 1,Object = 2,Function = 4,Class = 8,Namespace = 16
    }

    public class Morpheme : Node
    {
        public readonly Type type;
        public readonly Stmt stmt;
        public readonly List<Morpheme> up_list;
        public readonly MorphemeAttribute attribute;
        private readonly string name;

        public Morpheme(string n, MorphemeAttribute a, Stmt stmt, List<Morpheme> list, Type t)
        {
            type = t;
            name = n;
            attribute = a;
            this.stmt = stmt;
            up_list = new List<Morpheme>(list ?? new List<Morpheme>()) {this};
        }

        public override string ToString()
        {
            var result = "";
            for (var index = 0; index < up_list.Count - 1; index++) result += up_list[index].name + ".";
            return result + name;
        }
    }
    public class Func : LogicNode
    {
        public Morpheme morpheme;
        private readonly LogicNode param;
        public Func(Token tok, LogicNode p) : base(tok, null){param = p;}

        public override Type Check()
        {
            param?.Check();
            morpheme = Phrase.GetEnd(this);
            var method= (Method) morpheme.stmt;
            var @return = method.@return ?? (method.@return = Phrase.GetEnd(method.return_match).type);
            type = @return;
            return type;
        }
        public override void Generate(Method method)
        {
            param?.Generate(method);
            method.AddCode(ISet.Call, morpheme.ToString());
        }

    }

    public class Params : LogicNode
    {
        private readonly LogicNode left, right;
        public override string ToString() => left + "," + right;
        public Params(Token op, LogicNode lhs, LogicNode rhs) : base(op, null) {left = lhs;right = rhs;}
        public override void Generate(Method method)
        {
            right.Generate(method);
            left.Generate(method);
        }

        public override Type Check()
        {
            left.Check();
            right.Check();
            return null;
        }
    }
    public class Phrase : LogicNode
    {
        private readonly LogicNode left,right;
        public Morpheme m_left,m_right;
        private Phrase parent;
        private readonly MorphemeAttribute attribute;
        public Phrase(Token op, LogicNode lhs, LogicNode rhs) : base(op, Type.Int)
        {
            left = lhs;
            right = rhs;
            if (right is Phrase phrase) phrase.parent = this;
        }
        public override string ToString() => left + " " + Op + " " + right;
        public static LogicNode Match() => MatchTemplate<Phrase>(Match_Func, new[] {'.'});
        private static LogicNode MatchParam() => MatchTemplate<Params>(BoolTree.Match, new[] {','});
        private static LogicNode Match_Func()
        {
            var tok = Look;
            Match(ID);
            if (!Check('(')) return new Factor(tok, Type.Void);
            Match('(');
            var func = Check(')') ? new Func(tok,null) : new Func(tok,MatchParam());
            Match(')');
            return func;
        }

        public override Type Check()
        {
            type = GetEnd(this).type;
            return type;
        }

        public static bool DoubleCheck(Morpheme morpheme,MorphemeAttribute attribute)
        {
            if (attribute.HasFlag(morpheme.attribute))return true;
            Error(TypeError,morpheme.lex_line,morpheme.lex_ch,morpheme.ToString());
            return false;
        }
        public static Morpheme GetEnd(LogicNode result)
        {
            if (result is Factor factor && factor.Op.tag_value == BASIC)return new Morpheme((factor.Op as Word)?.lexeme,MorphemeAttribute.Class,null,null,factor.type);
            
            if (!(result is Phrase phrase))return Analyze(null, result);
            while (phrase.right is Phrase)
            {
                phrase.m_left = Analyze(phrase.parent == null ? null : phrase.m_right, phrase.left);
                phrase.m_right = Analyze(phrase.m_left, phrase.right);
                phrase=(Phrase) phrase.right; 
            }
            return Analyze(phrase.m_left, phrase.right);
        }

        private static Morpheme Analyze(Morpheme last,LogicNode current)
        {
            var name = current.ToString();
            if (last == null)
            {
                foreach (var name_space in Generator.name_spaces)
                    if (name_space.name == name) return new Morpheme(name, MorphemeAttribute.Namespace, name_space, null,null);
                last = Generator.@this;
            }
            switch (last.attribute)
            {
                case MorphemeAttribute.None: break;
                case MorphemeAttribute.Object:
                    var @class__ = last.stmt as Class;
                    foreach (var method in @class__.methods)
                        if (method.name == name && (method.atttibute & ElementAtttibute.STATIC) == 0)
                            return new Morpheme(name, MorphemeAttribute.Function, method, last.up_list,method.@return);
                    break;
                case MorphemeAttribute.Function: //TODO
                    break;
                case MorphemeAttribute.Class:
                    var @class_ = last.stmt as Class;
                    foreach (var method in @class_.methods)
                        if (method.name == name) //&& (method.atttibute & ElementAtttibute.STATIC) != 0)
                        return new Morpheme(name, MorphemeAttribute.Function, method, last.up_list,method.@return);
                        
                    //TODO field

                    Error(UnknownFunction,last.lex_line,last.lex_ch,last.ToString());
                    break;
                case MorphemeAttribute.Namespace:
                    var @namespace = last.stmt as Namespace;
                    foreach (var name_space in @namespace.sub_spaces)
                        if (name_space.name == name) return new Morpheme(name, MorphemeAttribute.Namespace, name_space, last.up_list,null);
                    foreach (var @class in @namespace.classes)
                        if (@class.name == name) return new Morpheme(name, MorphemeAttribute.Class, @class, last.up_list,null);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            throw new ArgumentOutOfRangeException();
        }
    }
}
