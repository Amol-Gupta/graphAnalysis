
using System.Runtime.InteropServices;

namespace graphAnalysis
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
            string filePath = """C:\Users\HP\Downloads\example_graph.txt""";
            var adjMat= graphAnalysis.parseString2AdjMat(File.ReadAllText(filePath));
            var graph = graphAnalysis.adjMat2Graph(adjMat);
            var dotGraph = graphAnalysis.graph2GraphViz(graph);
            filePath = """C:\Users\HP\Downloads\example_graph.dot""";
            File.WriteAllText(filePath, dotGraph);
            Console.WriteLine("Cardinality of different components in the example graph");
            var cardinality = graphAnalysis.cardinalityOfComponents(graph);
            foreach ( var card in cardinality ) { Console.WriteLine("\t"+card); }
            var bpt = graphAnalysis.bipartite(graph);
            Console.WriteLine("The sample graph is " + (bpt?"":"not") + " bipartite");
            Console.WriteLine("The dia of the sample graph is " + graphAnalysis.diameter(graph));
            analyseRandomGraph();
        }

        public static void analyseRandomGraph()
        {

            List<double> prob = new List<double>();
            List<double> AvgCardinality = new List<double>();
            List<double> AvgDia = new List<double>();
            List<decimal> bipartitePercent = new List<decimal>();
            for(double _p = 0.01; _p <0.2; _p = _p + 0.01) // varying probability
            {
                List<int> cardinalities = new List<int>();
                List<int> dia = new List<int>();
                int bipartiteCount = 0;
                for (int i = 0; i < 1000; i++) { // for 1000 iterations
                    var adjMat = graphAnalysis.getRandomAdjMat(_p, 100);
                    var graph = graphAnalysis.adjMat2Graph(adjMat);
                    var bipart = graphAnalysis.bipartite(graph);
                    var diameter = graphAnalysis.diameter(graph);
                    bipartiteCount=bipartiteCount + (bipart?1:0);
                    //Console.WriteLine("\n____________________________________________________________\n New graph \n\n");
                    cardinalities.AddRange(graphAnalysis.cardinalityOfComponents(graph));
                    dia.Add(diameter);

                }
                prob.Add(_p);
                bipartitePercent.Add(bipartiteCount / 1000.0m);
                AvgCardinality.Add(cardinalities.Average());
                AvgDia.Add(dia.Average());
            }
            for ( int i = 0;i < prob.Count;i++) {
                Console.WriteLine("\t" + prob[i] +"\t" + AvgCardinality[i] + "\t " + bipartitePercent[i] +"\t"+ AvgDia[i]);
            }
        }
    }
}
