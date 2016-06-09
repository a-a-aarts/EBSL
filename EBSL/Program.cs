using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EBSL
{
    class Program
    {
        static string path = @"..\..\..\Data\";
        static string name = "aggregate_mb_5_sybil_20_dl_1";
        static int c = 489;
        static float stepsize = 0.1f;

        static void Main(string[] args)
        {
            for (float i = 1f; i >= stepsize; i -= stepsize)
            {
                Opinion.g = (x => (float)Math.Sqrt(x.b));
                Ebsl e = new Ebsl(new Tuple<Dictionary<Tuple<int, int>, Evidence>, int>[] 
                {
                    new Tuple<Dictionary<Tuple<int, int>, Evidence>, int>(Dataparser.parseToEvidence(path + name + ".txt", true), c),
                });
                e.name = path + name;
                e.run(i);
            }
            Console.WriteLine("done");
            Console.ReadLine();
        }
    }
}
