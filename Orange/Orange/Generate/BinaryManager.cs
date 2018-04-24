using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orange.Generate
{
    class BinaryManager
    {
        public static void Write()
        {
            var binary_writer=new BinaryWriter(new FileStream("mydata.ore",FileMode.Create));
            binary_writer.Write((int)Generator.ElementAtttibute.STATIC);
            binary_writer.Close();
        }

        public static void Read()
        {
           var binary_reader = new BinaryReader(new FileStream("mydata.ore",FileMode.Open));
            Console.WriteLine((Generator.ElementAtttibute)binary_reader.ReadInt32());
 
            binary_reader.Close();
        }
    }
}
