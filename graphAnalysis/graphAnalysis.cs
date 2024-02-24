using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuikGraph;
using QuikGraph.Algorithms.ShortestPath;
using QuikGraph.Graphviz;
using static System.Net.Mime.MediaTypeNames;
namespace graphAnalysis
{
    internal class graphAnalysis
    {
        

        public static int[,] parseString2AdjMat(string str)
        {
            str = str.Trim();
            var lines = str.Split('\n');
            int n = int.Parse(lines[0]);
            int[,] adjMat = new int[n, n];

            if (lines.Length != n + 1) { throw new Exception("The input file is having unexpected number of lines."); };
            for (int i = 0; i < n; i++)
            {
                string[] tokens = lines[i + 1].Trim().Split(" ");
                if (tokens.Length != n) { throw new Exception("The input file is having unexpected number of columns."); }
                for (int j = 0; j < n; j++)
                {
                    string token = tokens[j];
                    int connectivity = 0;
                    if (token == "-") connectivity = 0;
                    if (token == "1") connectivity = 1;
                    if (token == "0") connectivity = 0;
                    adjMat[i, j] = connectivity;
                }
            }
            return adjMat;
        }

        public static UndirectedGraph<Node, Edge<Node>> adjMat2Graph(int[,] adjMat)
        {

            // Now that we have a parsed adjacency matrix we can try creating a graph 
            int n = adjMat.GetLength(0);
            // creating n nodes
            Node[] nodes = new Node[n];
            for (int x = 0; x < n; x++)
                nodes[x] = new Node(x);

            // creating edges
            Edge<Node>[] edges = new Edge<Node>[n * (n - 1) / 2];
            int edgeCounter = 0;
            for (int i = 0; i < n; i++)
            {
                for (int j = i + 1; j < n; j++)
                {
                    if (adjMat[i, j] == 1)
                    {
                        edges[edgeCounter] = new Edge<Node>(nodes[i], nodes[j]);
                        edgeCounter++;
                    }
                }
            }
            Array.Resize<Edge<Node>>(ref edges, edgeCounter);
            // creating graph out of edges
            var y = new UndirectedGraph<Node, Edge<Node>>();
            y.AddVertexRange(nodes);
            y.AddEdgeRange(edges);
            return y;
        }

        public static string graph2GraphViz(UndirectedGraph<Node, Edge<Node>> graph)
        {
            var graphviz = new GraphvizAlgorithm<Node, Edge<Node>>(graph);
            return graphviz.Generate();
        }

        public static string readFileToString(string filePath) { return File.ReadAllText(filePath); }
        public static void writeStringToFileI(string filePath, string str) { File.WriteAllText(filePath, str); }

        public static int[,] getRandomAdjMat(double p, int n)
        {
            Random rnd = new Random();
            int[,] adjMat = new int[n, n];
            for (int i = 0; i < n; i++)
            {
                for (int j = i; j < n; j++)
                {
                    if (i == j)
                    {
                        adjMat[i, j] = 0;
                    }
                    else
                    {
                        var x = rnd.NextDouble();
                        adjMat[i, j] = adjMat[j, i] = (x < p) ? 1 : 0;
                    }
                }
            }
            return adjMat;
        }

        
        public static List<int> cardinalityOfComponents(UndirectedGraph<Node, Edge<Node>> graph)
        {
            List<int> cardinality = new List<int>();
            foreach (Node vertex in graph.Vertices)
            {
                vertex.visited = false;
            }

            foreach (Node vertex in graph.Vertices)
            {
                if (vertex.visited == false)
                {
                    //Console.WriteLine("\n\n__________________\nStarting new Component exploration with node "+ vertex.index);
                    int cardinalityCount = 0;
                    Queue<Node> nodesVisited = new Queue<Node>();
                    nodesVisited.Enqueue(vertex);
                    vertex.visited = true;
                    cardinalityCount++;

                    while (nodesVisited.Count > 0)
                    {
                        var nodesVisitedIter = nodesVisited.Dequeue();
                        //Console.WriteLine("\texploring around node" + nodesVisitedIter.index.ToString());
                        foreach (var adjVertex in graph.AdjacentVertices(nodesVisitedIter))
                        {
                            
                            if (adjVertex.visited == false)
                            {
                                //Console.WriteLine("\t\t visited node" + adjVertex.index.ToString()+ " as it is adjacent to node" + nodesVisitedIter.index.ToString());
                                adjVertex.visited = true;
                                cardinalityCount++;
                                nodesVisited.Enqueue(adjVertex);
                            }
                            
                        }

                    }
                    cardinality.Add(cardinalityCount);
                }
            }
            return cardinality;
        }

       
        public static int diameter(UndirectedGraph<Node, Edge<Node>> graph)
        {
            int n = graph.VertexCount;
            int[,] dist = new int[n, n];
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (i == j) { dist[i, j] = 0; }
                    else
                    {
                        dist[i, j] = int.MaxValue/4;
                    }
                }
            }
            foreach(var e in graph.Edges)
            {
                dist[e.Source.index, e.Target.index] = 1;
                dist[e.Target.index, e.Source.index] = 1;
            }

            for(int p = 0; p < n; p++)
            {
                for(int q =0; q<n; q++)
                {
                    for (int r = 0; r < n; r++)
                    {
                        if (dist[q,r]> dist[q, p] + dist[p, r])
                        {
                            dist[ r, q] = dist[q, r] = dist[q, p] + dist[p, r];
                        }
                    }
                }
            }

            int dia = 0;
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (dist[i, j]>dia && dist[i, j] < int.MaxValue/4) { dia= dist[i, j]; }
                }
            }
            return dia;
        }

        public static bool bipartite(UndirectedGraph<Node, Edge<Node>> graph)
        {
            
            
            foreach (Node vertex in graph.Vertices)
            {
                vertex.visited = false;
                vertex.color = string.Empty;
            }
            foreach (Node vertex in graph.Vertices)
            {
                if (vertex.visited == false)
                {
                    //Console.WriteLine("\n\n__________________\nStarting new Component exploration with node "+ vertex.index);
                    
                    Queue<Node> nodesVisited = new Queue<Node>();
                    nodesVisited.Enqueue(vertex);
                    vertex.visited = true;
                    vertex.color = "RED";

                    while (nodesVisited.Count > 0)
                    {
                        var nodesVisitedIter = nodesVisited.Dequeue();
                        //Console.WriteLine("\texploring around node" + nodesVisitedIter.index.ToString());
                        foreach (var adjVertex in graph.AdjacentVertices(nodesVisitedIter))
                        {

                            if (adjVertex.visited == false)
                            {
                                //Console.WriteLine("\t\t visited node" + adjVertex.index.ToString()+ " as it is adjacent to node" + nodesVisitedIter.index.ToString());
                                adjVertex.visited = true;
                                adjVertex.color = (nodesVisitedIter.color == "RED") ? "BLUE" : "RED";
                                nodesVisited.Enqueue(adjVertex);
                            }else if(adjVertex.color == nodesVisitedIter.color)
                            {
                                return false;
                            }

                        }

                    }
                    
                }
            }
            return true;
        }
    }
}

