using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EBSLAnalyzer
{
    class Program
    {
        static void Main(string[] args)
        {
            var test = FromFile(@"..\..\..\Data\mb\aggregate_mb_10_sybil_20_dl_1_r_1.ebsl");
            var test2 = FromFile(@"..\..\..\Data\mb\aggregate_mb_10_sybil_20_dl_1_r_0,1.ebsl");

            Console.WriteLine(test.Find(x => x.Item1 == 41 && x.Item2 == 1064).Item3.Item1);
            Console.WriteLine(test2.Find(x => x.Item1 == 41 && x.Item2 == 1064).Item3.Item1);
            

            Console.ReadLine();
        }

        public static List<Tuple<int,int,Tuple<float,float,float>>> FromFile(string path)
        {
            byte[] file = File.ReadAllBytes(path);
            int length = BitConverter.ToInt32(file, 0);
            int size = BitConverter.ToInt32(file, 4);
            List<Tuple<int, int, Tuple<float, float, float>>> matrix = new List<Tuple<int, int, Tuple<float, float, float>>>();

            for (int i = 8; i < file.Length; i += 5 * 4)
            {
                matrix.Add(
                    new Tuple<int, int, Tuple<float, float, float>>
                        ( BitConverter.ToInt32(file, i)
                        , BitConverter.ToInt32(file, i + 4)
                        , new Tuple<float, float, float>
                            ( BitConverter.ToSingle(file, i + 8)
                            , BitConverter.ToSingle(file, i + 12)
                            , BitConverter.ToSingle(file, i + 16))));
            }

            return matrix;
        }
    }
}
