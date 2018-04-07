using System;
using System.Collections.Generic;
using Orange.Generate;
using Orange.Parse.Core;
using Orange.Parse.Operation;
using Orange.Parse.Structure;
using Type = Orange.Parse.Core.Type;
using static Orange.Debug.Debugger;
namespace Orange.Parse.Statements
{
    public class FuncCall : Stmt
    {
        private LogicNode match;
        private Morpheme morpheme;
        public static Stmt Match()
        {
            var func_call = new FuncCall {match = Phrase.Match()};
            if (func_call.match is Phrase phrase)
                func_call.morpheme = phrase.m_right;
            else if(func_call.match is Func func)
                func_call.morpheme = func.morpheme;
            Match(';');
            return func_call;
        }

        public override void Generate(Method generator)
        {
            match.Check();
        }
    }
}
