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
        float threshhold { get; set; } = 0.1f;

        public Ebsl(Tuple<Dictionary<Tuple<int, int>, Evidence>, int>[] evidence)
        {
            matrix = new OpinionMatrix(evidence.Max(x => x.Item1.Max(y => Math.Max(y.Key.Item1, y.Key.Item2))) + 1);
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
            matrix.ToFile("save" + DateTime.UtcNow);
        }



    }
}
