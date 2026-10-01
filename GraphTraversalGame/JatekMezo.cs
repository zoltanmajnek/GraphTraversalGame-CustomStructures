using System;
using System.IO;

namespace CABTEX_félévesSztf2
{
    enum Fnev { Választás = 0, Rnd = 1, Moho = 2, BackTrack = 3 }
    internal class JatekMezo
    {
        Graph<ND> JatekGraf { get; set; }
        LinkedList<ND> LepesLista;
        public delegate void LepesTortent(ND lepes);
        public event LepesTortent lepesTortent;
        public string[] adat;
        char[,] terkep;
        XY[] PlayerNodes; int sorok;
        int PN, fokozat; ND nd1;
        public static Random rnd = new Random();
        public bool Ready { get; private set; }

        public JatekMezo() { nd1 = new ND('1'); }

        public void Betolt(string fNev)
        {
            PlayerNodes = new XY[20]; PN = 0; //kezdőhelyek
            JatekGraf = new Graph<ND>(); LepesLista = new LinkedList<ND>(); Ready = false;

            void Csucskereso(int x, int y)
            {
                int ix = 0, iy = 0;
                if (terkep[x, y] == '|') ix = -1;
                else if (terkep[x, y] == '-') iy = -1;
                else if (terkep[x, y] == '/') { ix = 1; iy = -1; }
                else if (terkep[x, y] == '\\') { ix = 1; iy = 1; }

                ND honnan;
                do { x += ix; y += iy; } while (terkep[x, y] == terkep[x - ix, y - iy]);
                honnan = new ND(terkep[x, y], x, y); ix = -ix; iy = -iy; x += ix; y += iy;
                do { x += ix; y += iy; } while (terkep[x, y] == terkep[x - ix, y - iy]);

                JatekGraf.AddEdge(honnan, new ND(terkep[x, y], x, y));
            }
            if (fNev != "gen") adat = File.ReadAllLines(fNev); sorok = adat.Length;
            terkep = new char[sorok, 200];
            char n = 'A';
            string jelek = "O1J";
            for (int i = 0; i < sorok; ++i)
            {
                if (!adat[i].Contains("*"))
                {
                    for (int j = 0; j < adat[i].Length; j++)
                    {
                        char c = adat[i][j];
                        if (jelek.Contains(c.ToString()))
                        {
                            if (c != '1') { if (c == 'J') PlayerNodes[PN++] = new XY(i, j); ++n; if (n > 'Z') n = '2'; c = n; }
                            JatekGraf.AddNode(new ND(c, i, j));
                        }
                        terkep[i, j] = c;
                    }
                }
            }
            jelek = "\\/|-";
            for (int i = 0; i < sorok; ++i)
                if (!adat[i].Contains("*"))
                    for (int j = 0; j < adat[i].Length; j++)
                        if (jelek.Contains(terkep[i, j].ToString())) Csucskereso(i, j);

            if (fNev == "gen")
            {
                bool gen = false;
                if (PN == 0) { gen = true; }
                for (int i = 0; i < PN; i++)
                {
                    int tav = JatekGraf.HasEdge(nd1, new ND(terkep[PlayerNodes[i].x, PlayerNodes[i].y]));
                    if (tav < 3 || tav == 0) { gen = true; }
                }
                if (gen)
                { General(); return; }
            }

            if (PN == 0) throw new Exception("Nincs vírus belépési pont!");
            if (!JatekGraf.OsszeFuggo(nd1)) throw new Exception("Nem összefüggő a gráf (vagy nincs kezdőpont)");
            Ready = true;
        }

        public void General()
        {
            int kp = 4; int np = 30; int ferdeel = 0;
            void bejar(int x, int y)
            {
                --np; char c = 'O'; if (rnd.Next(100) < 10) if (--kp > 0) c = 'J';
                terkep[x, y] = c;
                if (x < 3 || y < 3 || x > 12 || y > 32) return;
                int ix, iy; int safety = 1000;
                int elszam = rnd.Next(0, 5);
                if (np == 29) elszam = 1; else if (np == 28) elszam = rnd.Next(3, 5); else if (elszam < 2) elszam = rnd.Next(0, 5);
                while (elszam > 0 && np > 0 && safety > 0)
                {
                    ix = rnd.Next(3) - 1; if (ix != 0) iy = 0; else do { iy = rnd.Next(3) - 1; } while (iy == 0);
                    if (terkep[x + ix, y + iy] == ' ' && terkep[x + 2 * ix, y + 2 * iy] == ' ')
                    { terkep[x + ix, y + iy] = ix == 0 ? '-' : '|'; bejar(x + 2 * ix, y + 2 * iy); --elszam; }
                    --safety;
                    if (rnd.Next(100) < 8 && ferdeel < 10)
                    {
                        string elek = "\\\\//"; int r = rnd.Next(0, 5);
                        if (r == 0) { ix = -2; iy = -2; }
                        if (r == 1) { ix = 2; iy = 2; }
                        if (r == 2) { ix = -2; iy = 2; }
                        if (r == 3) { ix = 2; iy = -2; }
                        if (terkep[x + ix, y + iy] == 'O' || terkep[x + ix, y + iy] == 'J')
                        { terkep[x + ix / 2, y + iy / 2] = elek[r]; ++ferdeel; }
                    }
                }
            }

            terkep = new char[18, 40];
            for (int i = 0; i < 18; i++) for (int j = 0; j < 40; j++) terkep[i, j] = ' ';
            bejar(6, 12); terkep[6, 12] = '1'; int pici = 1000;
            while ((np > 0 || kp > 1) && pici > 0)
            {
                int tx = rnd.Next(13); int ty = rnd.Next(33);
                if (terkep[tx, ty] == 'O') { if (kp > 1) terkep[tx, ty] = 'J'; bejar(tx, ty); --pici; }
            }
            adat = new string[16];
            for (int i = 0; i < 16; ++i)
            {
                char[] sor = new char[40]; for (int j = 0; j < 40; ++j) sor[j] = terkep[i, j];
                adat[i] = new string(sor);
            }
            Betolt("gen");
        }

        public void Kirajzol()
        {
            bool Hasonló(XY[] k, XY k2)
            {
                for (int i = 0; i < PN; i++) if (k[i].x == k2.x && k[i].y == k2.y) return true;
                return false;
            }

            Console.Clear(); Console.SetCursorPosition(50, 0);
            Console.Write("Fokozat: {0}.  Visszalépés: -", (Fnev)fokozat);
            int ii = 0; Console.SetCursorPosition(50, 5); Console.Write("Lépések:");
            foreach (ND lepes in LepesLista)
                if (ii < 12) { Console.SetCursorPosition(50, 5 + ++ii); Console.Write(lepes + "-" + lepes.Szomszed); }

            for (int i = 0; i < sorok; i++)
                for (int j = 0; j < 45; j++)

                    if (terkep[i, j] > 0)
                    {
                        var cs = JatekGraf.Csucs(new ND(terkep[i, j])); if (cs != null && cs.Infected) Console.BackgroundColor = ConsoleColor.DarkRed;
                        else if (Hasonló(PlayerNodes, new XY(i, j))) Console.BackgroundColor = ConsoleColor.DarkBlue;
                        else Console.BackgroundColor = ConsoleColor.Black;
                        if (terkep[i, j] == '1') Console.BackgroundColor = ConsoleColor.DarkGray;

                        Console.SetCursorPosition(j, i); Console.Write(terkep[i, j]);
                    }
            Console.BackgroundColor = ConsoleColor.Black;
        }

        public string Input(string uzi, string valaszthato)
        {
            string v;
            do
            {
                Console.SetCursorPosition(50, 2); for (int i = 0; i < 55; ++i) Console.Write(" "); Console.SetCursorPosition(50, 2);
                Console.Write(uzi + " (" + valaszthato + "): "); ConsoleKeyInfo ki = Console.ReadKey();
                v = ki.KeyChar.ToString().ToUpper();
            } while (!valaszthato.Contains(v));
            return v;
        }

        public int FokozatValaszt()
        {
            string v = Input("Ellenfél: 1:Random 2:Moho 3:BackTrack", "123-"); if (v == "-") return 3;
            fokozat = int.Parse(v); return 0;
        }
        public bool UjraKezdes()
        { if (Input("Újrakezdés?", "IN") == "I") return true; else return false; }

        public int JatekosLep()
        {
            int vanMegLepes = 0;
            LinkedList<ND> valaszthato = new LinkedList<ND>();
            LinkedList<ND> allNodes = JatekGraf.AllNodes();
            foreach (ND node in allNodes)
                if (node.Infected)
                {
                    LinkedList<ND> allSzomszed = JatekGraf.Neighbors(node);
                    foreach (ND szomszCsucs in allSzomszed)
                        if (!szomszCsucs.Infected) valaszthato.RendezveBeszúr(szomszCsucs);
                }
            string mit; string valaszt = ""; foreach (ND node in valaszthato) valaszt += node;
            if (valaszt.Length == 0)
            {
                mit = "kezdőpontot";
                foreach (var xy in PlayerNodes)
                    if (xy.x > 0 && xy.y > 0 && !JatekGraf.Csucs(new ND(terkep[xy.x, xy.y])).Infected) valaszt += terkep[xy.x, xy.y];
            }
            else mit = "csúcsot";

            if (valaszt.Length > 0)
            {
                ND csucs; var v = Input("Válasszon " + mit, valaszt + "-")[0];
                if (v == '-')
                {
                    ND back = LepesLista.Pop(); if (back == null) vanMegLepes = 3;
                    else
                    {
                        JatekGraf.AddEdge(back, back.Szomszed); TerkepRajz(back, back.Szomszed, false);
                        back = LepesLista.Pop(); csucs = JatekGraf.Csucs(back); csucs.Reverse();
                        vanMegLepes = 4;
                    }
                }
                else
                {
                    csucs = new ND(v);
                    JatekGraf.Csucs(csucs).GotInf(); LepesLista.Push(csucs); lepesTortent?.Invoke(csucs);
                    if (csucs.Name == '1') vanMegLepes = 2;
                }
            }
            else vanMegLepes = 1;

            return vanMegLepes;
        }

        public void TerkepRajz(ND from, ND to, bool torles)
        {
            int x1 = from.X, x2 = to.X, y1 = from.Y, y2 = to.Y;
            int ix, iy;
            if (x2 > x1) ix = 1; else if (x2 < x1) ix = -1; else ix = 0;
            if (y2 > y1) iy = 1; else if (y2 < y1) iy = -1; else iy = 0;
            do
            {
                x1 += ix; y1 += iy; terkep[x1, y1] = torles ? ' ' : adat[x1][y1];
            } while (x1 + ix != x2 || y1 + iy != y2);
        }

        public void GepLep()
        {
            int probakSzama = 0; bool Eltavolitva = false;
            LinkedList<ND> allNodes = JatekGraf.AllNodes();

            bool Torles(ND from, ND to)
            {
                if (from.Name != '1' && to.Name != '1')
                {
                    JatekGraf.RemoveEdge(from, to); ND lepes = new ND(from.Name, from.X, from.Y, to);
                    LepesLista.Push(lepes); lepesTortent?.Invoke(lepes);
                    TerkepRajz(from, to, true);
                    return true;
                }
                return false;
            }

            void GepLepRandom()
            {
                do
                {
                    foreach (ND node in allNodes)
                        if (!node.Infected)
                        {
                            LinkedList<ND> allSzomszed = JatekGraf.Neighbors(node);
                            foreach (ND szomszCsucs in allSzomszed)
                                if (!szomszCsucs.Infected) // nem fertőzött csúcsból mindenképp eltávolítható?! 
                                {
                                    if (!Eltavolitva && rnd.Next(1000) < 30) { Eltavolitva = Torles(node, szomszCsucs); }
                                }
                        }
                } while (!Eltavolitva && ++probakSzama < 10000);
            }

            bool GepLepMoho(bool check)
            {
                LinkedList<ND> mohoLista = new LinkedList<ND>();
                bool InfectedSearch(ND x, ND k)
                {
                    x.AddSzomszed(k); mohoLista.Push(x);
                    if (x.Infected) return true;
                    return false;
                }

                JatekGraf.dataProcess = InfectedSearch;
                JatekGraf.FirstInfected(nd1);

                ND virusosCsucs = mohoLista.Pop(); ND kereses;
                if (!virusosCsucs.Infected && check) return false; //itt kezd majd a backtrack

                do
                { kereses = mohoLista.Pop(); }
                while (!(kereses == null || kereses.Name == virusosCsucs.Szomszed.Name));

                if (kereses != null) Eltavolitva = Torles(kereses.Szomszed, kereses);
                if (!Eltavolitva) { if (!check) GepLepRandom(); return false; }

                return true;
            }

            void GepLepBackTrack()
            {
                bool NotInfectedKeres(ND k) { return !k.Infected; }

                ND max = null; int maxDFS = 0;
                JatekGraf.process = NotInfectedKeres;
                if (!GepLepMoho(true))
                {
                    foreach (ND node in allNodes)
                    {
                        if (node.Infected)
                            foreach (ND szomszed in JatekGraf.Neighbors(node))
                                if (!szomszed.Infected)
                                {
                                    int t = JatekGraf.DFS(szomszed);
                                    if (t > maxDFS) { maxDFS = t; max = szomszed; }
                                }
                    }
                    if (max != null)
                    {
                        foreach (ND node in JatekGraf.Neighbors(max))
                            if (!node.Infected && !Eltavolitva) Eltavolitva = Torles(node, max);
                    }
                    if (!Eltavolitva) GepLepRandom();
                }
            }

            if (fokozat == 1) GepLepRandom(); else if (fokozat == 2) GepLepMoho(false); else GepLepBackTrack();
        }
    }
}
