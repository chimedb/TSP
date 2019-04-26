
using System;
using ASD.Graphs;

class Test
{

    public static void Main()
    {
        Graph g;
        double m, w;
        Edge[] c;
        int N = 8;
        DateTime t1, t2;

        // "Odkomentuj" wybrany przykład i "zakomentuj" pozostałe
        // Możesz też podać własne inne przykłady
        // Obliczenia przykładów dla grafów o 200 wierzchołkach mogą chwilę potrwać

        RandomGraphGenerator gen = new RandomGraphGenerator(123);
        g = gen.UndirectedGraph(typeof(AdjacencyMatrixGraph), 20, 1.0, 1, 99);
        //g = gen.UndirectedGraph(typeof(AdjacencyMatrixGraph), 80, 1.0, 1, 99);
        //g = gen.UndirectedGraph(typeof(AdjacencyMatrixGraph), 200, 1.0, 1, 99);
        //g = gen.UndirectedEuclidGraph(typeof(AdjacencyMatrixGraph), 30, 1.0, 0.0, 100.0, 0.0, 100.0);
        //g = gen.UndirectedEuclidGraph(typeof(AdjacencyMatrixGraph), 200, 1.0, 0.0, 100.0, 0.0, 100.0);
        gen.SetSeed(3333);
        //g = gen.UndirectedGraph(typeof(AdjacencyMatrixGraph),8,1.0,10,99);

        Console.WriteLine();
        if (g.VerticesCount <= N)
        {
            for (int i = 0; i < g.VerticesCount; ++i)
            {
                for (int j = 0; j < g.VerticesCount; ++j)
                {
                    w = g.GetEdgeWeight(i, j);
                    Console.Write(" {0}", w.IsNaN() ? "**" : w.ToString());
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        if (g.VerticesCount <= 30)
        {
            t1 = DateTime.Now;
            (m, c) = g.BranchAndBoundTSP();
            t2 = DateTime.Now;
            Console.WriteLine("BranchAndBound: {0}  {1}  {2}", m, m == HamiltonTest(g, c), t2 - t1);
            if (g.VerticesCount <= N)
                WriteCycle(c);
            Console.WriteLine();
        }
        else
            Console.WriteLine("BranchAndBound: nie liczymy - zbyt dlugo by trwalo\n");

        if (g.VerticesCount <= 10)
        {
            t1 = DateTime.Now;
            (m, c) = g.BacktrackingTSP();
            t2 = DateTime.Now;
            Console.WriteLine("Backtracking: {0}  {1}  {2}", m, m == HamiltonTest(g, c), t2 - t1);
            if (g.VerticesCount <= N)
                WriteCycle(c);
            Console.WriteLine();
        }
        else
            Console.WriteLine("Backtracking: nie liczymy - zbyt dlugo by trwalo\n");

        t1 = DateTime.Now;
        (m, c) = g.SimpleGreedyTSP();
        t2 = DateTime.Now;
        Console.WriteLine("SimpleGreedy: {0}  {1}  {2}", m, m == HamiltonTest(g, c), t2 - t1);
        if (g.VerticesCount <= N)
            WriteCycle(c);
        Console.WriteLine();

        t1 = DateTime.Now;
        (m, c) = g.KruskalTSP();
        t2 = DateTime.Now;
        Console.WriteLine("Kruskal: {0}  {1}  {2}", m, m == HamiltonTest(g, c), t2 - t1);
        if (g.VerticesCount <= N)
            WriteCycle(c);
        Console.WriteLine();

        if (!g.Directed)
        {
            t1 = DateTime.Now;
            (m, c) = g.TreeBasedTSP();
            t2 = DateTime.Now;
            Console.WriteLine("TreeBased: {0}  {1}  {2}", m, m == HamiltonTest(g, c), t2 - t1);
            if (g.VerticesCount <= N)
                WriteCycle(c);
            Console.WriteLine();

            t1 = DateTime.Now;
            (m, c) = g.TreeBasedTSP(TSPTreeBasedVersion.Simple);
            t2 = DateTime.Now;
            Console.WriteLine("TreeBased 0: {0}  {1}  {2}", m, m == HamiltonTest(g, c), t2 - t1);
            if (g.VerticesCount <= N)
                WriteCycle(c);
            Console.WriteLine();

            t1 = DateTime.Now;
            (m, c) = g.TreeBasedTSP(TSPTreeBasedVersion.Christofides);
            t2 = DateTime.Now;
            Console.WriteLine("TreeBased 1: {0}  {1}  {2}", m, m == HamiltonTest(g, c), t2 - t1);
            if (g.VerticesCount <= N)
                WriteCycle(c);
            Console.WriteLine();

            t1 = DateTime.Now;
            (m, c) = g.TreeBasedTSP(TSPTreeBasedVersion.ModifiedChristofides);
            t2 = DateTime.Now;
            Console.WriteLine("TreeBased 2: {0}  {1}  {2}", m, m == HamiltonTest(g, c), t2 - t1);
            if (g.VerticesCount <= N)
                WriteCycle(c);
            Console.WriteLine();
        }

        t1 = DateTime.Now;
        (m, c) = g.IncludeTSP();
        t2 = DateTime.Now;
        Console.WriteLine("Include: {0}  {1}  {2}", m, m == HamiltonTest(g, c), t2 - t1);
        if (g.VerticesCount <= N)
            WriteCycle(c);
        Console.WriteLine();

        t1 = DateTime.Now;
        (m, c) = g.IncludeTSP(TSPIncludeVertexSelectionMethod.Sequential);
        t2 = DateTime.Now;
        Console.WriteLine("Include 0: {0}  {1}  {2}", m, m == HamiltonTest(g, c), t2 - t1);
        if (g.VerticesCount <= N)
            WriteCycle(c);
        Console.WriteLine();

        if (!g.Directed)
        {
            t1 = DateTime.Now;
            (m, c) = g.IncludeTSP(TSPIncludeVertexSelectionMethod.Nearest);
            t2 = DateTime.Now;
            Console.WriteLine("Include 1: {0}  {1}  {2}", m, m == HamiltonTest(g, c), t2 - t1);
            if (g.VerticesCount <= N)
                WriteCycle(c);
            Console.WriteLine();

            t1 = DateTime.Now;
            (m, c) = g.IncludeTSP(TSPIncludeVertexSelectionMethod.Furthest);
            t2 = DateTime.Now;
            Console.WriteLine("Include 2: {0}  {1}  {2}", m, m == HamiltonTest(g, c), t2 - t1);
            if (g.VerticesCount <= N)
                WriteCycle(c);
            Console.WriteLine();
        }

        t1 = DateTime.Now;
        (m, c) = g.IncludeTSP(TSPIncludeVertexSelectionMethod.MinimalCost);
        t2 = DateTime.Now;
        Console.WriteLine("Include 3: {0}  {1}  {2}", m, m == HamiltonTest(g, c), t2 - t1);
        if (g.VerticesCount <= N)
            WriteCycle(c);
        Console.WriteLine();

        t1 = DateTime.Now;
        (m, c) = g.ThreeOptTSP();
        t2 = DateTime.Now;
        Console.WriteLine("ThreeOpt: {0}  {1}  {2}", m, m == HamiltonTest(g, c), t2 - t1);
        if (g.VerticesCount <= N)
            WriteCycle(c);
        Console.WriteLine();

        t1 = DateTime.Now;
        (m, c) = g.ThreeOptTSPParallel();
        t2 = DateTime.Now;
        Console.WriteLine("ThreeOptParallel: {0}  {1}  {2}", m, m == HamiltonTest(g, c), t2 - t1);
        if (g.VerticesCount <= N)
            WriteCycle(c);
        Console.WriteLine();
    }

    public static double HamiltonTest(Graph g, Edge[] cycle)
    {
        if (cycle == null) return -1;
        if (g.VerticesCount != cycle.Length) return -2;
        double d;
        bool[] v = new bool[g.VerticesCount];
        d = cycle[0].Weight;
        v[cycle[0].From] = true;
        for (int i = 1; i < g.VerticesCount; ++i)
        {
            if (cycle[i].From != cycle[i - 1].To) return -3;
            if (v[cycle[i].From]) return -4;
            v[cycle[i].From] = true;
            d += cycle[i].Weight;
        }
        if (cycle[0].From != cycle[g.VerticesCount - 1].To) return -5;
        return d;
    }

    public static void WriteCycle(Edge[] cycle)
    {
        for (int i = 0; i < cycle.Length; ++i)
            Console.Write(" {0}", cycle[i]);
        Console.WriteLine();
    }

}