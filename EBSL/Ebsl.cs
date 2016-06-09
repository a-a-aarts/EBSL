using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EBSL
{
    class Ebsl
    {
        OpinionMatrix matrix;
        public float Threshhold { get; set; } = 0.01f;
        public string name { get; set; } = DateTime.Now.Day + "-" + DateTime.Now.Month + "_" + DateTime.Now.Hour + "-" + DateTime.Now.Minute;

        public Ebsl(Tuple<Dictionary<Tuple<int, int>, Evidence>, int>[] evidence)
        {
            matrix = new OpinionMatrix(evidence.Max(x => x.Item1.Max(y => Math.Max(y.Key.Item1, y.Key.Item2))) + 1);
            matrix.Threshhold = Threshhold;
            foreach(var d in evidence)
            {
                foreach(var kv in d.Item1)
                {
                    int x = kv.Key.Item1;
                    int y = kv.Key.Item2;
                    matrix[x, y] = Opinion.Consensus(matrix[x, y], kv.Value.ToOpinion(d.Item2));
                }
            }
        }

        public void run(float restart)
        {
            int count = 0;
            OpinionMatrix A = matrix.Clone() as OpinionMatrix;
            A.Scale(restart);

            do
            {
                Console.WriteLine("iteration " + count);
                matrix = matrix.Iterate(A);
                count++;
            } while (count < 200 && !matrix.converged);
            matrix.ToFile(name + "_r_" + restart + ".ebsl");
        }



    }
}
