using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;

namespace EBSL
{
    class OpinionMatrix : ICloneable
    {
        public int eq = 0;

        float Threshhold { get; set; } = 0.1F;
        int size = 0;
        public bool converged = false;

        protected Dictionary<Tuple<int, int>, Opinion> matrix;

        protected HashSet<int>[] xs;
        protected HashSet<int>[] ys;

        private object l = new object();
        private ReaderWriterLockSlim rwl = new ReaderWriterLockSlim();

        public OpinionMatrix(int size)
        {
            this.size = size;
            matrix = new Dictionary<Tuple<int, int>, Opinion>();
            xs = new HashSet<int>[size];
            ys = new HashSet<int>[size];
            for (int i = 0; i < size; i++)
            {
                this.xs[i] = new HashSet<int>();
                this.ys[i] = new HashSet<int>();
            }
        }

        private OpinionMatrix(int size, Dictionary<Tuple<int, int>, Opinion> matrix, HashSet<int>[] xs, HashSet<int>[] ys)
        {
            this.size = size;
            this.matrix = new Dictionary<Tuple<int, int>, Opinion>(matrix);
            this.xs = new HashSet<int>[size];
            this.ys = new HashSet<int>[size];
            for (int i = 0; i < size; i++)
            {
                this.xs[i] = new HashSet<int>(xs[i]);
                this.ys[i] = new HashSet<int>(ys[i]);
            }

        }

        public Opinion this[int x, int y]
        {
            get
            {
                if (matrix.ContainsKey(new Tuple<int, int>(x, y)))
                {
                    return matrix[new Tuple<int, int>(x, y)];
                }
                else if (x > size || y > size)
                {
                    throw new IndexOutOfRangeException();
                }
                return Opinion.U;
            }
            set
            {
                if (value != Opinion.U && x != y)
                {
                    lock(l)
                    {
                        matrix[new Tuple<int, int>(x, y)] = value;
                        xs[x].Add(y);
                        ys[y].Add(x);
                    }
                }
                else
                {
                    matrix.Remove(new Tuple<int, int>(x, y));

                }
            }
        }

        public void Scale(float f)
        {
            foreach(Tuple<int,int> t in matrix.Keys.ToList())
            {
                int x = t.Item1;
                int y = t.Item2;
                this[x, y] = Opinion.Scalar(f, this[x, y]);
            }
        }

        public OpinionMatrix Iterate(OpinionMatrix A)
        {
            OpinionMatrix R = this;
            OpinionMatrix R_new = this.Clone() as OpinionMatrix;
            for (int i = 0; i < size; i++)
            {
                Parallel.For(0, size, ((j) =>
                {
                    List<Opinion> sum = new List<Opinion>();
                    foreach (int k in A.ys[j])
                    {
                        if (R.xs[i].Contains(k) && k != i)
                        {
                            sum.Add(Opinion.Discount(R[i, k], A[k, j]));
                        }
                    }
                    if (sum.Count > 0)
                    {
                        R_new[i, j] = Opinion.Consensus(A[i, j], sum.Aggregate((a, b) => Opinion.Consensus(a, b)));
                    }
                }));
            }
            R_new.converged = Compare(R_new) < Threshhold;
            Console.WriteLine("nodes: " + R_new.matrix.Count);
            return R_new;
        }

        public float Compare(OpinionMatrix o)
        {
            HashSet<Tuple<int, int>> cors = new HashSet<Tuple<int, int>>();
            foreach(Tuple<int,int> t in this.matrix.Keys)
            {
                cors.Add(t);
            }
            foreach(Tuple<int,int> t in o.matrix.Keys)
            {
                cors.Add(t);
            }
            float compare = 0f;
            
            Parallel.ForEach(cors, t =>
            {
                int x = t.Item1;
                int y = t.Item2;
                float tmp = Math.Abs(this[x, y].b - o[x, y].b) + Math.Abs(this[x, y].d - o[x, y].d) + Math.Abs(this[x, y].u - o[x, y].u);
                while (true)
                {
                    float tmpcompare = compare;
                    if(compare < tmp)
                    {
                        Interlocked.CompareExchange(ref compare, tmp, tmpcompare);
                    }
                    else
                    {
                        break;
                    }
                }

            });
            Console.WriteLine("compare: " + compare);
            return compare;
        }

        public object Clone()
        {
            return new OpinionMatrix(this.size, this.matrix, this.xs, this.ys);
        }


        #region IO
        public static OpinionMatrix FromFile(string path)
        {
            byte[] file = File.ReadAllBytes(path);
            int length = BitConverter.ToInt32(file, 0);
            int size = BitConverter.ToInt32(file, 4);
            OpinionMatrix om = new OpinionMatrix(size);

            for (int i = 8; i< file.Length; i += 5 * 4)
            {
                om[BitConverter.ToInt32(file, i), BitConverter.ToInt32(file, i + 4)] = Opinion.from_bdu(BitConverter.ToSingle(file, i + 8), BitConverter.ToSingle(file, i + 12), BitConverter.ToSingle(file, i + 16));
            }

            return om;
        }


        /*
        format:
        total opinions,
        matrix size,
        n x opinions: xb, xd, xu
        */
        public void ToFile(string path)
        {
            using ( var file = File.Open(path, FileMode.CreateNew))
            {
                byte[] length = BitConverter.GetBytes(matrix.Count);
                byte[] size = BitConverter.GetBytes(this.size);
                byte[] opinions = matrix.SelectMany(x =>
                {
                    List<byte> tmp = new List<byte>();
                    tmp.AddRange(BitConverter.GetBytes(x.Key.Item1));
                    tmp.AddRange(BitConverter.GetBytes(x.Key.Item2));
                    tmp.AddRange(BitConverter.GetBytes(x.Value.b));
                    tmp.AddRange(BitConverter.GetBytes(x.Value.d));
                    tmp.AddRange(BitConverter.GetBytes(x.Value.u));
                    return tmp;
                }).ToArray();
                file.Write(length, 0, length.Length);
                file.Write(size, 0, size.Length);
                file.Write(opinions, 0, opinions.Length);
            }
        }
        #endregion
    }
}
