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
        public static Dictionary<Tuple<int, int>, Evidence> parseMBToEvidence(string filepath, bool IsBidirectional)
        {
            string[] lines = File.ReadAllLines(filepath);
            Dictionary<Tuple<int, int>, Evidence> evidence = new Dictionary<Tuple<int, int>, Evidence>();

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
                        evidence.Add(key, new Evidence(0,0));
                    }
                        evidence[key] += e;
                    if (IsBidirectional)
                    {
                        key = new Tuple<int, int>(nodeB, nodeA);
                        if (!evidence.ContainsKey(key))
                        {
                            evidence.Add(key, new Evidence(0, 0));
                        }
                        evidence[key] += -e;
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


        public static Dictionary<Tuple<int, int>, Evidence> parseCountToEvidence(string filepath, bool IsBidirectional)
        {
            string[] lines = File.ReadAllLines(filepath);
            Dictionary<Tuple<int, int>, Evidence> evidence = new Dictionary<Tuple<int, int>, Evidence>();

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
                        evidence.Add(key, new Evidence(0, 0));
                    }
                    evidence[key] += e/e;
                    if (IsBidirectional)
                    {
                        key = new Tuple<int, int>(nodeB, nodeA);
                        if (!evidence.ContainsKey(key))
                        {
                            evidence.Add(key, new Evidence(0, 0));
                        }
                        evidence[key] += -(e/e);
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
