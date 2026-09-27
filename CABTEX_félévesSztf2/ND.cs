using System;

namespace CABTEX_félévesSztf2
{
    public class ND : IComparable
    {
        public char Name { get; }
        public bool Infected { get; private set; }
        public int X, Y;
        public ND Szomszed { get; private set;}

        public override string ToString() => Name.ToString();

        public ND(char name, int x, int y, ND szomszed) { Name = name; Infected = false; X = x; Y = y; Szomszed = szomszed; }
        public ND(char name, int x, int y) : this(name, x, y, null) { }
        public ND(char name) : this(name, 0, 0, null) { }

        public int CompareTo(object obj)
        {
            return Name.CompareTo((obj as ND).Name);
        }

        public void GotInf() { Infected = true; }
        public void Reverse() { Infected = false; }
        public void AddSzomszed(ND who) { Szomszed = who; }
    }
}
