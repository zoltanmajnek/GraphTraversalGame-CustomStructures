using System;

namespace CABTEX_félévesSztf2
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
            public LáncoltLista<Node> Edges { get; }
            public Node(T data)
            {
                this.data = data;
                Edges = new LáncoltLista<Node>();
            }

            public int CompareTo(object obj)
            {
                return data.CompareTo((obj as Node).data);
            }
        }

        LáncoltLista<Node> Nodes;

        int nodesNum;
        int connectedNodes;

        Node GetNodeFromData(T data)
        {
            Nodes.BejarInit(); Node keres;
            do { keres = Nodes.KovAdat(); } while (keres != null && data.CompareTo(keres.data) > 0);
            if (keres != null && keres.data.CompareTo(data) == 0) return keres;
            return null;
        }

        public LáncoltLista<T> Neighbors(T node)
        {
            LáncoltLista<T> nb = new LáncoltLista<T>();
            var RealNode = GetNodeFromData(node);
            if (RealNode != null)
            {
                foreach (Node edge in RealNode.Edges) nb.RendezveBeszúr(edge.data);
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


        public Graph() { Nodes = new LáncoltLista<Node>(); }

        public bool AddNode(T node)
        {
            if (Nodes.RendezveBeszúr(new Node(node))) { nodesNum++; return true; }
            return false;
        }

        public void AddEdge(T from, T to)
        {
            var nodeFrom = GetNodeFromData(from);
            var nodeTo = GetNodeFromData(to);
            if (nodeFrom != null && nodeTo != null && from.CompareTo(to) != 0)
            {
                nodeFrom.Edges.RendezveBeszúr(nodeTo);
                nodeTo.Edges.RendezveBeszúr(nodeFrom);
            }
        }

        public void RemoveEdge(T from, T to)
        {
            var nodeFrom = GetNodeFromData(from);
            var nodeTo = GetNodeFromData(to);
            if (nodeFrom != null && nodeTo != null && from.CompareTo(to) != 0)
            {
                nodeFrom.Edges.Törlés(nodeTo);
                nodeTo.Edges.Törlés(nodeFrom);
            }
        }

        public int HasEdge(T from, T to) //ha from=to, bejárás (hurokél nincs).
        {
            connectedNodes = 1;
            Node nFrom = GetNodeFromData(from); if (nFrom == null) return 0;
            LáncoltLista<Node> S = new LáncoltLista<Node>(); S.Sorbaszúr(nFrom);
            LáncoltLista<Node> F = new LáncoltLista<Node>(); nFrom.Tav = 0; F.Sorbaszúr(nFrom);

            while (!S.Ures)
            {
                Node k = S.SorbólKiszed();
                if (from.CompareTo(to) == 0) process?.Invoke(k.data);
                else if (k.data.CompareTo(to) == 0) return k.Tav;
                foreach (Node node in k.Edges)
                    if (!F.Keresés(node))
                    { S.Sorbaszúr(node); node.Tav = k.Tav + 1; F.Sorbaszúr(node); connectedNodes++; }
            }
            return 0;
        }

        public bool OsszeFuggo(T start)
        {
            if (GetNodeFromData(start) == null) return false;
            HasEdge(start, start);
            return nodesNum == connectedNodes;
        }

        public LáncoltLista<T> AllNodes()
        {
            LáncoltLista<T> lista = new LáncoltLista<T>();
            foreach (Node node in Nodes) lista.RendezveBeszúr(node.data);
            return lista;
        }

        public void TavRezet() { foreach (Node node in Nodes) node.Tav = 1000; }

        public void FirstInfected(T from)
        {
            Node nFrom = GetNodeFromData(from);
            LáncoltLista<Node> S = new LáncoltLista<Node>(); S.Sorbaszúr(nFrom);
            LáncoltLista<Node> F = new LáncoltLista<Node>(); nFrom.Tav = 0; F.Sorbaszúr(nFrom);
            bool found = false;

            while (!S.Ures && !found)
            {
                Node k = S.SorbólKiszed();
                foreach (Node x in k.Edges)
                    if (!F.Keresés(x) && !found)
                    {
                        S.Sorbaszúr(x); F.Sorbaszúr(x);
                        if (dataProcess != null) found = dataProcess(x.data, k.data);
                    }
            }
        }

        public int DFS(T start)
        {
            int t = 0;
            LáncoltLista<Node> F = new LáncoltLista<Node>();
            var firstNode = GetNodeFromData(start);
            if (firstNode != null) DFSRek(firstNode, ref F, ref t);
            return t;
        }
        void DFSRek(Node k, ref LáncoltLista<Node> F, ref int t)
        {
            bool tovabb = true;
            F.RendezveBeszúr(k); t++; if (process != null) tovabb = process(k.data);
            if (tovabb) foreach (Node node in k.Edges)
                    if (!F.Keresés(node))
                    { DFSRek(node, ref F, ref t); }
            k.Tav = t++;
        }
    }
}
