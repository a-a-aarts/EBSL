using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EBSL
{
    class Program
    {
        static void Main(string[] args)
        {
            Opinion.g = (x => (float)Math.Sqrt(x.b));
            Ebsl e = new Ebsl(new Tuple<Dictionary<Tuple<int, int>, Evidence>, int>[] { new Tuple<Dictionary<Tuple<int, int>, Evidence>, int>( Dataparser.parseMBToEvidence(@"..\..\..\Data\Dataset.txt", true), 2) });
            e.run(1f);
            /*
            OpinionMatrix a = OpinionMatrix.FromFile("testsave");
            OpinionMatrix b = OpinionMatrix.FromFile("testsave2");
            Console.WriteLine(a.Compare(b));*/
            Console.ReadLine();
        }
    }
}
