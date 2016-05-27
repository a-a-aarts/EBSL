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

        public Ebsl(Dictionary<Tuple<int, int>, Tuple<int, int>> evidence)
        {
            matrix = new OpinionMatrix(evidence.Max(x => Math.Max(x.Key.Item1, x.Key.Item2)) + 1);
            foreach (KeyValuePair<Tuple<int, int>, Tuple<int, int>> kv in evidence)
            {
                matrix[kv.Key.Item1, kv.Key.Item2] = new Opinion(kv.Value.Item1, kv.Value.Item2, 2);
            }
        }

        public void run()
        {
            int count = 0;
            OpinionMatrix A = matrix.Clone() as OpinionMatrix;
            do
            {
                Console.WriteLine("iteration " + count);
                /*matrix =*/ matrix.Iterate(A);
                count++;
            } while (count < 100 && !matrix.converged);
            //matrix.ToFile("testsave2");
        }



    }
}
