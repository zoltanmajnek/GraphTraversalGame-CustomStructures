using System;
using System.Collections;

namespace CABTEX_félévesSztf2
{
    class M<T>
    {
        public T adat;
        public M<T> next;
        public M(T adat, M<T> next)
        {
            this.adat = adat;
            this.next = next;
        }
    }

    /// <summary>
    /// Generic linked list implementation supporting multiple operations:
    /// - Sorted insertion and removal
    /// - Queue operations (Enqueue/Dequeue)
    /// - Stack operations (Push/Pop)
    /// - Set operations (Union, Intersection, Difference)
    /// - Filtering and traversal
    /// </summary>
    public class LinkedList<T> : IEnumerable, IEnumerator
        where T : IComparable
    {
        M<T> fej;
        M<T> forEach;
        M<T> EnuMutato;

        public LinkedList() { }

        public delegate void BejáróFunkció(T t);
        public delegate bool SzűrőFunckció(T t);
        public BejáróFunkció bejáróFunkció;

        public object Current => EnuMutato.adat;

        public bool RendezveBeszúr(T adat)
        {
            M<T> mutato = fej; M<T> előző = null;
            while (mutato != null && adat.CompareTo(mutato.adat) > 0) { előző = mutato; mutato = mutato.next; }
            if (mutato != null && adat.CompareTo(mutato.adat) == 0) return false;
            if (előző == null) fej = new M<T>(adat, mutato); else előző.next = new M<T>(adat, mutato);
            return true;
        }
        public void Sorbaszúr(T adat)
        {
            M<T> mutato = fej; M<T> előző = null;
            while (mutato != null) { előző = mutato; mutato = mutato.next; }
            if (előző == null) fej = new M<T>(adat, mutato); else előző.next = new M<T>(adat, mutato);
        }
        public T SorbólKiszed()
        {
            if (fej != null) { T ki = fej.adat; fej = fej.next; return ki; }
            return default(T);
        }
        public bool Ures => fej == null;

        public void Push(T adat) { var uj = new M<T>(adat, fej); fej = uj; }
        public T Pop() => SorbólKiszed();

        public void BejarInit() => forEach = fej;
        public T KovAdat()
        {
            if (forEach == null) return default(T);
            T ki = forEach.adat; forEach = forEach.next; return ki;
        }

        public void Bejáró()
        {
            M<T> mutato = fej;
            while (mutato != null)
            {
                bejáróFunkció?.Invoke(mutato.adat);
                mutato = mutato.next;
            }
        }

        public bool Keresés(T mit)
        {
            M<T> mutato = fej;
            while (mutato != null && !mutato.adat.Equals(mit)) mutato = mutato.next;
            if (mutato == null) return false;
            return true;
        }

        public void Törlés(string név)
        {
            M<T> mutato = fej; M<T> előző = null;
            while (mutato != null && !mutato.adat.Equals(név)) { előző = mutato; mutato = mutato.next; }
            if (mutato != null)
            { if (előző == null) fej = mutato.next; else előző.next = mutato.next; }
        }

        public void Törlés(T mit)
        {
            M<T> mutato = fej; M<T> előző = null;
            while (mutato != null && mit.CompareTo(mutato.adat) != 0) { előző = mutato; mutato = mutato.next; }
            if (mutato != null)
            { if (előző == null) fej = mutato.next; else előző.next = mutato.next; }
        }

        public LinkedList<T> Szűrés(SzűrőFunckció szűrő)
        {
            M<T> mutato = fej;
            LinkedList<T> újlista = new LinkedList<T>();
            while (mutato != null)
            {
                if (szűrő(mutato.adat)) újlista.RendezveBeszúr(mutato.adat);
                mutato = mutato.next;
            }
            return újlista;
        }

        public LinkedList<T> Metszet(LinkedList<T> lista2)
        {
            LinkedList<T> újlista = new LinkedList<T>();
            M<T> m1 = fej; M<T> m2 = lista2.fej;
            while (m1 != null && m2 != null)
            {
                int comp = m1.adat.CompareTo(m2.adat);
                if (comp < 0) m1 = m1.next;
                else if (comp > 0) m2 = m2.next;
                else { újlista.RendezveBeszúr(m1.adat); m1 = m1.next; m2 = m2.next; }
            }
            return újlista;
        }

        public LinkedList<T> Unió(LinkedList<T> lista2)
        {
            LinkedList<T> újlista = new LinkedList<T>();
            M<T> m1 = fej; M<T> m2 = lista2.fej;
            while (m1 != null && m2 != null)
            {
                int comp = m1.adat.CompareTo(m2.adat);
                if (comp == -1) { újlista.RendezveBeszúr(m1.adat); m1 = m1.next; }
                else if (comp == 1) { újlista.RendezveBeszúr(m2.adat); m2 = m2.next; }
                else { újlista.RendezveBeszúr(m1.adat); m1 = m1.next; m2 = m2.next; }
            }
            while (m1 != null) { újlista.RendezveBeszúr(m1.adat); m1 = m1.next; }
            while (m2 != null) { újlista.RendezveBeszúr(m2.adat); m2 = m2.next; }

            return újlista;
        }

        public LinkedList<T> Különbség(LinkedList<T> lista2)
        {
            LinkedList<T> újlista = this.Szűrés((hős) => true);
            LinkedList<T> metszet = this.Metszet(lista2);

            M<T> m1 = metszet.fej;
            while (m1 != null) { újlista.Törlés(m1.adat); m1 = m1.next; }

            return újlista;
        }

        public IEnumerator GetEnumerator()
        {
            this.EnuMutato = new M<T>(default(T), fej);
            return (IEnumerator)this;
        }

        public bool MoveNext()
        {
            if (EnuMutato != null) EnuMutato = EnuMutato.next;
            return EnuMutato != null;
        }

        public void Reset()
        {
            EnuMutato = new M<T>(default(T), fej);
        }
    }
}
