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
        public void Write()
        {
            var binary_writer=new BinaryWriter(new FileStream("mydata.ore",FileMode.Create));
            binary_writer.Write(666);
            binary_writer.Write(2.2333f);
            binary_writer.Write(true);
            binary_writer.Write(false);
            binary_writer.Write(true);
            binary_writer.Write("HelloWorld");
            binary_writer.Close();
        }

        public void Read()
        {
           var binary_reader = new BinaryReader(new FileStream("mydata.ore",FileMode.Open));
            Console.WriteLine(binary_reader.ReadInt32());
            Console.WriteLine(binary_reader.ReadSingle());
            Console.WriteLine(binary_reader.ReadBoolean());
            Console.WriteLine(binary_reader.ReadBoolean());
            Console.WriteLine(binary_reader.ReadBoolean());
            Console.WriteLine(binary_reader.ReadString());
            binary_reader.Close();
        }
    }
}
