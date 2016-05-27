using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace EBSL
{
    static class Dataparser
    {
        public static Dictionary<Tuple<int, int>, Tuple<int, int>> parseToEvidence(string filepath)
        {
            string[] lines = File.ReadAllLines(filepath);
            Dictionary<Tuple<int, int>, Tuple<int, int>> evidence = new Dictionary<Tuple<int, int>, Tuple<int, int>>();

            for (int i = 0; i < lines.Length; i++)
            {
                try
                {
                    string[] args = lines[i].Split(' ');
                    int nodeA = int.Parse(args[1]);
                    int nodeB = int.Parse(args[2]);
                    int e = int.Parse(args[3]);

                    Tuple<int, int> key = new Tuple<int, int>(nodeA, nodeB);
                    if (!evidence.ContainsKey(key))
                    {
                        evidence.Add(key, new Tuple<int, int>(0, 0));
                    }
                    if (e > 0)
                    {
                        Tuple<int, int> tmp = evidence[key];
                        evidence[key] = new Tuple<int, int>(tmp.Item1 + e, tmp.Item2);
                    }
                    else if (e < 0)
                    {
                        Tuple<int, int> tmp = evidence[key];
                        evidence[key] = new Tuple<int, int>(tmp.Item1, tmp.Item2 - e);
                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine(string.Format("Warning: could not parse line {0}: \"{1}\"", new object[] { i, lines[i] }));
                }
            }
            Console.WriteLine("done parsing");
            return evidence;
        }
    }
}
