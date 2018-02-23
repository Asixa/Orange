using System;
using System.Collections.Generic;
using System.IO;

namespace Orange.Parse
{
    [Serializable]
    public class TAC
    {
        [Serializable]
        public struct Runtime
        {
            public List<TAC> tacs;
            public Dictionary<int, int> tags;
        }

        public static Runtime runtime=new Runtime{tacs =new List<TAC>(),tags=new Dictionary<int, int>()};

        public static void Print()
        {
            Console.WriteLine();
            Console.WriteLine("____________格式化三地址码______________");
            foreach (var t in runtime.tacs)
            {
                Console.WriteLine(t.Op + " " + t.A1 + " " + t.A2 + " " + t.A3);
            }

            Console.WriteLine("___________标签列表___________");
            foreach (var t in runtime.tags)
            {
                Console.WriteLine("TAG" + t.Key + " : " + t.Value);
            }
        }

        public static void Add(TAC t) => runtime.tacs.Add(t);
        public string Op, A1, A2, A3;

        public void tac(string op, string a1, string a2 = "", string a3 = "")
        {
            Op = op;
            A1 = a1;
            A2 = a2;
            A3 = a3;
        }

        public TAC(string[] s)
        {
            switch (s[0])
            {
                case "if":
                    tac("IFY", s[1], s[3]);
                    break;
                case "iffalse":
                    tac("IFN", s[1], s[3]);
                    break;
                case "goto":
                    tac("GO2", s[1]);
                    break;
                case "Print":
                    tac("PRI", s[1]);
                    break;
                default:
                    if (s[1] == "=")
                    {
                        if (s.Length == 3)
                        {
                            tac("ASN", s[0], s[2]);
                        }
                        else
                        {
                            tac(s[3], s[0], s[2], s[4]);
                        }

                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("三地址异常,匹配字符为 " + s[0]);
                        Console.ForegroundColor = ConsoleColor.White;
                    }

                    break;
            }
        }

        public TAC(string op, string a1, string a2 = "", string a3 = "")
        {
            Op = op;
            A1 = a1;
            A2 = a2;
            A3 = a3;
        }

        public static void Generate(string path)
        {
            string txt="";
            foreach (var t in runtime.tacs)
            {
                txt += t.Op + " " + t.A1 + " " + t.A2 + " " + t.A3+"\n";
            }
            if(File.Exists(path))File.Delete(path);

            FileStream fst = new FileStream(path, FileMode.Append);
            //写数据到txt
            StreamWriter swt = new StreamWriter(fst, System.Text.Encoding.GetEncoding("utf-8"));
            //写入
            swt.WriteLine(txt);
            swt.Close();
            fst.Close();

        }
    }
}
