using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataManager
{
    class Program
    {
        static string filepath = @"..\..\..\Data\Dataset.txt";
        static int aggregationSize = 10;
        static int sybalSize = 100;
        static int sybalDownloads = 200;
        static string outputpath = @"..\..\..\Data\";


        static void Main(string[] args)
        {
            //parse and aggregate
            string[] lines = File.ReadAllLines(filepath);
            Dictionary<Tuple<int, int>, int> evidence_mb = new Dictionary<Tuple<int, int>, int>();
            Dictionary<Tuple<int, int>, int> evidence_c = new Dictionary<Tuple<int, int>, int>();
            for (int i = 0; i < lines.Length; i++)
            {
                try
                {
                    string[] arg = lines[i].Split(' ');
                    int nodeA = int.Parse(arg[1]) / aggregationSize;
                    int nodeB = int.Parse(arg[2]) / aggregationSize;
                    int e = int.Parse(arg[3]);
                    if (e > 0 && nodeA != nodeB)
                    {
                        Tuple<int, int> key = new Tuple<int, int>(nodeA, nodeB);
                        if (!evidence_mb.ContainsKey(key))
                        {
                            evidence_mb.Add(key, 0);
                            evidence_c.Add(key, 0);
                        }
                        evidence_mb[key] += e;
                        evidence_c[key] += Math.Abs(e) / e;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(string.Format("Warning: could not parse line {0}: \"{1}\"", new object[] { i, lines[i] }));
                }
            }

            //collect stats
            List<int> tmp = evidence_mb.Values.ToList();
            tmp.Sort();
            int median_mb = tmp[tmp.Count/2];
            int max_mb = tmp.Max();
            int min_mb = tmp.Min();

            tmp = evidence_c.Values.ToList();
            tmp.Sort();
            int median_c = tmp[tmp.Count/2];
            int max_c = tmp.Max();
            int min_c = tmp.Min();

            int sybil = evidence_mb.Keys.Max(x => Math.Max(x.Item1, x.Item2)) + 1;

            //insert sybil

            //add sybil downloads
            Random r = new Random();
            HashSet<int> nodes = new HashSet<int>();
            while(nodes.Count < sybalDownloads)
            {
                nodes.Add(r.Next(sybil - 1));
            }
            for(int i = 0; i < sybalDownloads; i++)
            {
                int node = nodes.ToList()[i];
                evidence_mb.Add(new Tuple<int, int>(node, sybil), -median_mb);
                evidence_mb.Add(new Tuple<int, int>(sybil, node), -min_mb);
                evidence_c.Add(new Tuple<int, int>(node, sybil), -median_c);
                evidence_c.Add(new Tuple<int, int>(sybil, node), -min_c);
            }

            //add sybill support
            for(int i = 1; i < sybalSize + 1; i++)
            {
                evidence_mb.Add(new Tuple<int, int>(sybil + i, sybil), median_mb);
                evidence_mb.Add(new Tuple<int, int>(sybil, sybil + i), min_mb);
                evidence_c.Add(new Tuple<int, int>(sybil + i, sybil), median_c);
                evidence_c.Add(new Tuple<int, int>(sybil, sybil + i), min_c);
            }

            //write
            string name_mb = string.Format("aggregate_mb_{0}_sybil_{1}_dl_{2}.txt", new object[] { aggregationSize, sybalSize, sybalDownloads });
            string name_c = string.Format("aggregate_c_{0}_sybil_{1}_dl_{2}.txt", new object[] { aggregationSize, sybalSize, sybalDownloads });

            List<string> output = new List<string>();
            output.Add(string.Format("-- min:{0} max:{1} med:{2} sybil:{3}", new object[] { min_mb, max_mb, median_mb, sybil}));
            output.AddRange(evidence_mb.Select(x => string.Format("{0} {1} {2}", new object[] { x.Key.Item1, x.Key.Item2, x.Value })));
            File.WriteAllLines(outputpath + name_mb, output);

            output = new List<string>();
            output.Add(string.Format("-- min:{0} max:{1} med:{2} sybil:{3}", new object[] { min_c, max_c, median_c, sybil }));
            output.AddRange(evidence_c.Select(x => string.Format("{0} {1} {2}", new object[] { x.Key.Item1, x.Key.Item2, x.Value })));
            File.WriteAllLines(outputpath + name_c, output);

        }
    }
}
