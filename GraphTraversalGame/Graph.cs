using System;

namespace GraphTraversalGame
{
    public struct XY
    {
        public int x, y;
        public XY(int x1, int y1)
        {
            x = x1; y = y1;
        }
    }

    public delegate bool DataProcess<T>(T x, T k);
    public delegate bool Process<T>(T k);

    public class Graph<T> where T : IComparable
    {
        public DataProcess<T> dataProcess;
        public Process<T> process;

        class Node : IComparable
        {
            public T data { get; }
            public int Tav { get; set; }
            public LinkedList<Node> Edges { get; }
            public Node(T data)
            {
                this.data = data;
                Edges = new LinkedList<Node>();
            }

            public int CompareTo(object obj)
            {
                return data.CompareTo((obj as Node).data);
            }
        }

        LinkedList<Node> Nodes;

        int nodesNum;
        int connectedNodes;

        Node GetNodeFromData(T data)
        {
            Nodes.IteratorInit(); Node keres;
            do { keres = Nodes.NextData(); } while (keres != null && data.CompareTo(keres.data) > 0);
            if (keres != null && keres.data.CompareTo(data) == 0) return keres;
            return null;
        }

        public LinkedList<T> Neighbors(T node)
        {
            LinkedList<T> nb = new LinkedList<T>();
            var RealNode = GetNodeFromData(node);
            if (RealNode != null)
            {
                foreach (Node edge in RealNode.Edges) nb.SortedInsert(edge.data);
            }
            return nb;
        }

        public T Csucs(T node)
        {
            var RNode = GetNodeFromData(node); if (RNode != null) return RNode.data;
            return default(T);
        }
        public int Ertek(T node)
        {
            var RNode = GetNodeFromData(node); if (RNode != null) return RNode.Tav;
            return -1;
        }


        public Graph() { Nodes = new LinkedList<Node>(); }

        public bool AddNode(T node)
        {
            if (Nodes.SortedInsert(new Node(node))) { nodesNum++; return true; }
            return false;
        }

        public void AddEdge(T from, T to)
        {
            var nodeFrom = GetNodeFromData(from);
            var nodeTo = GetNodeFromData(to);
            if (nodeFrom != null && nodeTo != null && from.CompareTo(to) != 0)
            {
                nodeFrom.Edges.SortedInsert(nodeTo);
                nodeTo.Edges.SortedInsert(nodeFrom);
            }
        }

        public void RemoveEdge(T from, T to)
        {
            var nodeFrom = GetNodeFromData(from);
            var nodeTo = GetNodeFromData(to);
            if (nodeFrom != null && nodeTo != null && from.CompareTo(to) != 0)
            {
                nodeFrom.Edges.Delete(nodeTo);
                nodeTo.Edges.Delete(nodeFrom);
            }
        }

        public int HasEdge(T from, T to) //ha from=to, bejárás (hurokél nincs).
        {
            connectedNodes = 1;
            Node nFrom = GetNodeFromData(from); if (nFrom == null) return 0;
            LinkedList<Node> S = new LinkedList<Node>(); S.RowInsert(nFrom);
            LinkedList<Node> F = new LinkedList<Node>(); nFrom.Tav = 0; F.RowInsert(nFrom);

            while (!S.Empty)
            {
                Node k = S.RowOut();
                if (from.CompareTo(to) == 0) process?.Invoke(k.data);
                else if (k.data.CompareTo(to) == 0) return k.Tav;
                foreach (Node node in k.Edges)
                    if (!F.Search(node))
                    { S.RowInsert(node); node.Tav = k.Tav + 1; F.RowInsert(node); connectedNodes++; }
            }
            return 0;
        }

        public bool OsszeFuggo(T start)
        {
            if (GetNodeFromData(start) == null) return false;
            HasEdge(start, start);
            return nodesNum == connectedNodes;
        }

        public LinkedList<T> AllNodes()
        {
            LinkedList<T> lista = new LinkedList<T>();
            foreach (Node node in Nodes) lista.SortedInsert(node.data);
            return lista;
        }

        public void TavRezet() { foreach (Node node in Nodes) node.Tav = 1000; }

        public void FirstInfected(T from)
        {
            Node nFrom = GetNodeFromData(from);
            LinkedList<Node> S = new LinkedList<Node>(); S.RowInsert(nFrom);
            LinkedList<Node> F = new LinkedList<Node>(); nFrom.Tav = 0; F.RowInsert(nFrom);
            bool found = false;

            while (!S.Empty && !found)
            {
                Node k = S.RowOut();
                foreach (Node x in k.Edges)
                    if (!F.Search(x) && !found)
                    {
                        S.RowInsert(x); F.RowInsert(x);
                        if (dataProcess != null) found = dataProcess(x.data, k.data);
                    }
            }
        }

        public int DFS(T start)
        {
            int t = 0;
            LinkedList<Node> F = new LinkedList<Node>();
            var firstNode = GetNodeFromData(start);
            if (firstNode != null) DFSRek(firstNode, ref F, ref t);
            return t;
        }
        void DFSRek(Node k, ref LinkedList<Node> F, ref int t)
        {
            bool tovabb = true;
            F.SortedInsert(k); t++; if (process != null) tovabb = process(k.data);
            if (tovabb) foreach (Node node in k.Edges)
                    if (!F.Search(node))
                    { DFSRek(node, ref F, ref t); }
            k.Tav = t++;
        }
    }
}
