using Antlr.Runtime;
using Graphviz4Net.Dot;
using Graphviz4Net.Dot.AntlrParser;
using Graphviz4Net.Graphs;
//using QuickGraph;
using DotFileParser;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Zad2Graph
{
    class Program
    {
        static void Main(string[] args)
        {
            Stopwatch sw = new Stopwatch();
            if (args.Length == 1)
            {

                //args[0] = "GraphToTest.dt";
                using var sr = new StreamReader(args[0]);
                //AdjacencyGraph<string, Edge<string>> graph = new AdjacencyGraph<string, Edge<string>>(true);
                ParserClass pc = new ParserClass();
                Tuple<List<int>, int[][], Dictionary<int, List<int>>, Dictionary<int, int>, Dictionary<int, int>> tuple = pc.ParseFile(args[0]);
                int counter = 0;
                sw.Start();

                //Console.WriteLine(sr.ReadToEnd());
                //var graph = Parse(sr.ReadToEnd());
                //Console.WriteLine(graph+"\n\n");
                List<int> vertices = tuple.Item1;
                int[][] edges = tuple.Item2;
                //Dictionary<List<int>,List<int>> edgesConnection= new Dictionary<List<int>, List<int>>();
                Dictionary<int, int> edgesSource =tuple.Item4;
                Dictionary<int, int> edgesDestination = tuple.Item5;
                Dictionary<int, List<int>> edgesConnection = tuple.Item3;           
                Dictionary<int, bool> potentialCapital = new Dictionary<int, bool>();

                //GetInfoAboutGraph(graph, edgesConnection, edgesSource, edgesDestination, vertices);
                sw.Start();
                BFSAlgorithm(vertices, edgesSource, edgesDestination, edgesConnection, edges, potentialCapital);
                //CheckAllVerticiesIsAbleToReach( vertices, edgesSource, edgesConnection, edgesDestination,potentialCapital);
                sw.Stop();
                Console.WriteLine("Time speneded on duty: {0} seconds",sw.Elapsed.TotalSeconds);
                Console.WriteLine("Looked vericies:");
                foreach (var item in potentialCapital)
                {
                    if(item.Value==true)
                        Console.Write(item.Key+" ");
                }
                if (!potentialCapital.Values.Contains(true))
                {
                    Console.WriteLine("Nie znaleziono wierzchołka");
                }
            }
        }

        private async static void BFSAlgorithm(List<int> vertices, Dictionary<int, int> edgesSource, Dictionary<int, int> edgesDestination, Dictionary<int, List<int>> edgesConnection, int[][] edges, Dictionary<int, bool> potentialCapital)
        {
            Queue<int> verteciesToCheck = new Queue<int>();
            Dictionary<int, bool> verteciesWhichImAbletAchive = new Dictionary<int, bool>();
            //uzupełnieni słowników
            foreach (var item in vertices)
            {
                verteciesWhichImAbletAchive.Add(item, false);
            }
            List<Task> tasks = new List<Task>();

            foreach (var item in vertices)
            {
                tasks.Add(checkVertex(item,vertices, edgesSource, edgesDestination, edgesConnection, edges, potentialCapital));
            }
            await Task.WhenAll(tasks);
            //return potentialCapital;
        }

        private async static Task checkVertex(int vertex, List<int> vertices, Dictionary<int, int> edgesSource, Dictionary<int, int> edgesDestination, Dictionary<int, List<int>> edgesConnection, int[][] edges, Dictionary<int, bool> potentialCapital)
        {
            int counter;
            Queue<int> verteciesToCheck = new Queue<int>();
            Dictionary<int, bool> verteciesWhichImAbletAchive = new Dictionary<int, bool>();
            int vertexWchichICheck;
            counter = 0;
            verteciesToCheck.Clear();
            foreach (var itemToClear in vertices)
            {
                verteciesWhichImAbletAchive[itemToClear] = false;
            }
            verteciesWhichImAbletAchive[vertex] = true;
            verteciesToCheck.Enqueue(vertex);
            while (verteciesToCheck.Count > 0)
            {
                vertexWchichICheck = verteciesToCheck.Dequeue();
                for (int i = 0; i < vertices.Count; i++)
                {
                    if (edgesConnection[i].Contains(vertexWchichICheck) && !verteciesWhichImAbletAchive[i])
                    {
                        verteciesWhichImAbletAchive[i] = true;
                        verteciesToCheck.Enqueue(i);
                    }
                }
            }
            for (int i = 0; i < vertices.Count; i++)
            {
                if (verteciesWhichImAbletAchive[i] == true)
                    counter++;
            }
            if (counter == vertices.Count)
            {
                potentialCapital.Add(vertex, true);
            }
            else
            {
                potentialCapital.Add(vertex, false);
            }
        }

        private static void CheckAllVerticiesIsAbleToReach(List<int> vertices, Dictionary<int, int> edgesSource, Dictionary<int, List<int>> edgesConnection, Dictionary<int, int> edgesDestination, Dictionary<int, bool> potentialCapital)
        {
            Dictionary<int, bool> vertexToCheck = new Dictionary<int, bool>();
            List<int> isReached= new List<int>();
            List<int> isNotReached = new List<int>();


            //bede sprawdzał wszystkie wierzchołki po kolei w tej petli czy sa osiagalne z kazdego miejsca grafu v1
            foreach (var item in vertices)
            {
                SetVertexToCheckAndUnreached(vertexToCheck, isNotReached, vertices);

                isReached.Add(item);
                isNotReached.Remove(item);
                if (checkNeighbours(item, edgesConnection, isNotReached, isReached, vertexToCheck))
                    potentialCapital.Add(item, true);
                else
                    potentialCapital.Add(item, false);
                vertexToCheck.Clear();
                isNotReached.Clear();
                isReached.Clear();
            }


        }

        private static void SetVertexToCheckAndUnreached(Dictionary<int, bool> vertexToCheck, List<int> isNotReached, List<int> vertices)
        {
            foreach (var vertex in vertices)
            {
                vertexToCheck.Add(vertex, true);
                isNotReached.Add(vertex);
            }
        }

        private static bool checkNeighbours(int vertex, Dictionary<int, List<int>> edgesConnection, List<int> isNotReached, List<int> isReached, Dictionary<int, bool> vertexToCheck)
        {
            if (isNotReached.Count== 0)
            {
                return true;
            }
            vertexToCheck.Remove(vertex);
            foreach (var item in edgesConnection)
            {
                if (item.Value.Contains(vertex)&&vertexToCheck.ContainsKey(item.Key)&&isNotReached.Contains(item.Key))
                {
                    isReached.Add(item.Key);
                    isNotReached.Remove(item.Key);
                }
            }
            for (int i = 0; i < isReached.Count(); i++)
            {
                if (vertexToCheck.ContainsKey(isReached[i]))
                {
                    checkNeighbours(isReached[i], edgesConnection, isNotReached, isReached, vertexToCheck);
                    if (isNotReached.Count == 0)
                    {
                        return true;
                    }

                }
            }
            //foreach (var item in isReached)
            //{
            //    if (vertexToCheck.ContainsKey(item))
            //    {
            //        checkNeighbours(item, edgesConnection, isNotReached, isReached, vertexToCheck);
            //        if (isNotReached.Count == 0)
            //        {
            //            return true;
            //        }

            //    }
            //}
            return false;

        }

        private static void GetInfoAboutGraph(DotGraph<int> graph, Dictionary<int, List<int>> edgesConnection, Dictionary<int, int> edgesSource, Dictionary<int, int> edgesDestination, List<int> vertices)
        {
            Tuple<int, int> transformed;

            ///System zbierania danych o grafie
            foreach (var item in graph.AllVertices.ToList())
            {
                vertices.Add(item.Id);
                edgesDestination.Add(item.Id, 0);
                edgesSource.Add(item.Id, 0);
                edgesConnection.Add(item.Id, new List<int>());
            }

            foreach (var item in graph.Edges.ToList())
            {
                transformed = transformEdgeIdToInt(item, "Destination");
                edgesSource[transformed.Item1]++;
                edgesDestination[transformed.Item2]++;
                edgesConnection[transformed.Item1].Add(transformed.Item2);
            }
        }
        private static Tuple<int, int> transformEdgeIdToInt(IEdge edge, string whichEndOfEdge)
        {
            int charLoc;
            int idS, idD;

            charLoc = edge.ToString().IndexOf('-');
            Int32.TryParse(edge.Source.ToString().Trim(new char[] { '{', '}' }), out idS);
            Int32.TryParse(edge.Destination.ToString().Trim(new char[] { '{', '}' }), out idD);
            return new Tuple<int, int>(idS, idD);
        }

        private static DotGraph<int> Parse(string content)
        {
            var antlrStream = new ANTLRStringStream(content);
            var lexer = new DotGrammarLexer(antlrStream);
            var tokenStream = new CommonTokenStream(lexer);
            var parser = new DotGrammarParser(tokenStream);
            var builder = new IntDotGraphBuilder();
            parser.Builder = builder;
            // parser.dot();
            try
            {
                parser.dot();
            }
            catch(StackOverflowException e)
            {
                Console.WriteLine(e.Message+"\n"+e.StackTrace+"\n"+e.Source);
            }
            return builder.DotGraph;
        }
    }
}
