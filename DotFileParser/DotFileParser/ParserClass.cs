using System;
using System.Collections.Generic;
using System.IO;

namespace DotFileParser
{
    public class ParserClass
    {

        public Tuple<List<int>, int[][], Dictionary<int, List<int>>, Dictionary<int, int>, Dictionary<int, int>> ParseFile(string fileName)
        {
            using StreamReader sr = new StreamReader(fileName);
            // string date = sr.ReadToEnd();
            List<string> lines = new List<string>();
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                if (!line.Contains("{") && !line.Contains("}"))
                {
                    lines.Add(line.Remove(line.Length - 1));
                }
            }
            List<int> vertecies = CountVertecies(lines);
            ClearData(lines, vertecies.Count);
            int[][] edges = SaveEdges(lines);
            Dictionary<int, List<int>> vertexAndItsDest = CreateListOdEdges(edges, vertecies);
            Dictionary<int, int> amountOfOutVertex = CountOutputOfVertex(vertexAndItsDest);
            Dictionary<int, int> amountOfInVertex = CountInputOfVertex(vertexAndItsDest);
            Tuple<List<int>, int[][], Dictionary<int, List<int>>, Dictionary<int, int>, Dictionary<int, int>> tuple = new Tuple<List<int>, int[][], Dictionary<int, List<int>>, Dictionary<int, int>, Dictionary<int, int>>(vertecies, edges, vertexAndItsDest, amountOfOutVertex, amountOfInVertex);
            return tuple;
            
        }

        private Dictionary<int, int> CountInputOfVertex(Dictionary<int, List<int>> vertexAndItsDest)
        {
            Dictionary<int, int> toReturn = new Dictionary<int, int>();
            int[][] counter = new int[vertexAndItsDest.Count][];

            for (int i = 0; i < counter.Length; i++)
            {
                counter[i] = new int[2];
                counter[i][0] = i;
                counter[i][1] = 0;
            }

            foreach (var vertesies in vertexAndItsDest.Values)
            {
                foreach (var vertex in vertesies)
                {
                    for (int i=0; i< counter.Length;i++)
                    {
                        if (counter[i][0]==vertex)
                        {
                            counter[i][1]++;
                        }
                    }
                }
            }
            for(int i=0;i<vertexAndItsDest.Count;i++)
            {
                toReturn.Add(counter[i][0], counter[i][1]);
            }
            return toReturn;
        }

        private Dictionary<int, int> CountOutputOfVertex(Dictionary<int, List<int>> vertexAndItsDest)
        {
            Dictionary<int, int> toReturn = new Dictionary<int, int>();
            foreach (var item in vertexAndItsDest)
            {
                toReturn.Add(item.Key, item.Value.Count);
            }
            return toReturn;
        }

        private Dictionary<int, List<int>> CreateListOdEdges(int[][] edges, List<int> vertecies)
        {
            Dictionary<int, List<int>> toReturn = new Dictionary<int, List<int>>();
            foreach (var vertex in vertecies)
            {
                toReturn.Add(vertex, new List<int>());
            }
            foreach (var item in edges)
            {
                foreach (var source in toReturn)
                {
                    if (source.Key == item[0])
                    {
                        source.Value.Add(item[1]);
                    }
                }
            }
            return toReturn;
        }

        private void ClearData(List<string> lines, int amountOfVertecies)
        {
            for (int i = 0; i < amountOfVertecies; amountOfVertecies--)
            {
                lines.RemoveAt(i);
            }
        }

        private int[][] SaveEdges(List<string> lines)
        {
            int[][] edges = new int[lines.Count][];
            for (int i = 0; i < edges.Length; i++)
            {
                edges[i] = new int[2];
            }
            int number1;
            int number2;
            for (int i = 0; i < lines.Count; i++)
            {
                lines[i]=lines[i].Trim(' ');
                Int32.TryParse(lines[i].Substring(0, lines[i].IndexOf("-")), out number1);
                Int32.TryParse(lines[i].Substring(lines[i].IndexOf("-") + 2, (lines[i].Length - (lines[i].IndexOf("-") + 2))), out number2);

                edges[i][0] = number1;
                edges[i][1] = number2;
            }

            return edges;
        }

        private List<int> CountVertecies(List<string> lines)
        {
            List<int> toReturn = new List<int>();
            int num;
            foreach (var item in lines)
            {
                if (item.Contains("-"))
                    break;

                Int32.TryParse(item, out num);
                toReturn.Add(num);
            }

            return toReturn;
        }
    }
}