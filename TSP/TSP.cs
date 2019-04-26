
using System;
using System.Collections.Generic;

namespace ASD.Graphs
{

    public class TSPHelper : System.MarshalByRefObject
    {
        double best = double.PositiveInfinity;
        List<Edge> solution;
    /// <summary>
    /// Znajduje rozwiązanie dokładne problemu komiwojażera metodą podziału i ograniczeń
    /// </summary>
    /// <param name="g">Badany graf</param>
    /// <param name="cycle">Znaleziony cykl (parametr wyjściowy)</param>
    /// <returns>Długość znalezionego cyklu (suma wag krawędzi)</returns>
    /// <remarks>
    /// Metoda przeznaczona jest dla grafów z nieujemnymi wagami krawędzi.<br/>
    /// Uruchomiona dla grafu zawierającego krawędź o wadze ujemnej zgłasza wyjątek <see cref="System.ArgumentException"/>.<br/>
    /// <br/>
    /// Elementy (krawędzie) umieszczone są w tablicy <i>cycle</i> w kolejności swojego następstwa w znalezionym cyklu Hamiltona.<br/>
    /// <br/>
    /// Jeśli w badanym grafie nie istnieje cykl Hamiltona metoda zwraca wartość <b>NaN</b>,
    /// parametr wyjściowy <i>cycle</i> ma wówczas wartość <b>null</b>.<br/>
    /// <br/>
    /// Metodę można stosować dla grafów skierowanych i nieskierowanych.
    /// </remarks>
        public double TSP(Graph g, out Edge[] cycle)
        {
            int n = g.VerticesCount;
            double[,] CostMatrix = new double[n, n];
            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                    CostMatrix[i, j] = -1;

            for (int i = 0; i < n; i++)
            {
                foreach (Edge e in g.OutEdges(i))
                {
                    CostMatrix[i, e.To] = e.Weight;
                }
            }

            BNB(CostMatrix, 0, new List<Edge>(), n);
            cycle = solution.ToArray();  
            return best;     
        }

        public void BNB(double[,] T, double estimation, List<Edge> edges, int dimension)
        {
            estimation += RowReduction(T) + ColumnReduction(T);
            if (estimation >= best) return;
            int n = T.GetLength(0);
            if (dimension == 2)
            {
                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        if (T[i, j] != -1)
                        {
                            edges.Add(new Edge(i, j));
                        }
                    }
                    solution = OrderEdge(edges);
                }
                best = Math.Min(estimation, best);
                return;
            }
            
            //Preparing L
            int from = -1, to = -1;
            FindBestEdge(T, out from, out to);
            if (from == -1 || to == -1) return;

            //Copy T to L
            double[,] L = new double[n, n];
            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                    L[i, j] = T[i, j];

            //Removing row and column from L
            for (int i = 0; i < n; i++)
            {
                L[from, i] = -1;
                L[i, to] = -1;
            }

            //Adding new edge
            edges.Add(new Edge(from, to));

            //Blocking 
            L[to, from] = -1;
            List<Edge> nedges = OrderEdge(edges);
            if (nedges.Count == edges.Count) L[edges[edges.Count - 1].To, edges[0].From] = -1;
            
            BNB(L, estimation, edges, dimension - 1);

            //Preparing P
            T[from, to] = -1;
            estimation += RowReduction(T) + ColumnReduction(T);
            if (estimation >= best) return;
            BNB(T, estimation, edges, dimension);
        }

        public List<Edge> OrderEdge(List<Edge> edges)
        {
            List<Edge> nedges = new List<Edge>();
            int from = edges[0].From, to = edges[0].To;
            nedges.Add(new Edge(from, to));
            while (nedges.Count < edges.Count)
            {
                from = nedges[nedges.Count - 1].To;
                to = -1;
                for (int i = 0; i < edges.Count; i++)
                {
                    if (edges[i].From == from)
                    {
                        to = edges[i].To;
                        break;
                    }
                }
                if (to == -1) break;
                nedges.Add(new Edge(from, to));
            }
            return nedges;
        }

        public void FindBestEdge(double[,] cost, out int from, out int to)
        {
            from = to = -1;
            int n = cost.GetLength(0);
            double[] min = new double[n];
            int[] index = new int[n]; 
            for (int i = 0; i < n; i++)
            {
                min[i] = double.MaxValue;
                index[i] = -1;
                for (int j = 0; j < n; j++)
                {
                    if (cost[i, j] != -1 && min[i] > cost[i, j])
                    {
                        min[i] = cost[i, j];
                        index[i] = j;
                    }
                }
            }
            double value = double.MaxValue;
            for (int i = 0; i < n; i++)
            {
                if (index[i] != -1 && value > min[i])
                {
                    from = i;
                    to = index[i];
                }
            }
        }

        public double RowMin(double[,] cost, int i)
        {
            int n = cost.GetLength(0);
            double min = cost[i, 0];
            for (int j = 0; j < n; j++)
            {
                if (cost[i, j] < min)
                {
                    min = cost[i, j];
                }
            }
            return min;
        }

        public double ColumnMin(double[,] cost, int i)
        {
            int n = cost.GetLength(1);
            double min = cost[0, i];
            for (int j = 0; j < n; j++)
            {
                if (cost[j, i] < min)
                {
                    min = cost[j, i];
                }
            }
            return min;
        }
        
        public double RowReduction(double[,] cost)
        {
            double row = 0;
            int n = cost.GetLength(0);
            for (int i = 0; i < n; i++)
            {
                double rmin = RowMin(cost, i);
                if (rmin != -1)
                {
                    row += rmin;
                }
                for (int j = 0; j < n; j++)
                {
                    if (cost[i, j] != -1)
                    {
                        cost[i, j] -= rmin;
                    }
                }
            }
            return row;
        }

        public double ColumnReduction(double[,] cost)
        {
            double column = 0;
            int n = cost.GetLength(1);
            for (int i = 0; i < n; i++)
            {
                double cmin = ColumnMin(cost, i);
                if (cmin != -1)
                {
                    column += cmin;
                }
                for (int j = 0; j < n; j++)
                {
                    if (cost[j, i] != -1)
                    {
                        cost[j, i] -= cmin;
                    }
                }
            }
            return column;
        }
    }

}  // namespace ASD.Graphs
