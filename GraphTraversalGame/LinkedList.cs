using System;
using System.Collections;

namespace GraphTraversalGame
{
    class Pointer<T>
    {
        public T data;
        public Pointer<T> next;
        public Pointer(T data, Pointer<T> next)
        {
            this.data = data;
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
        Pointer<T> head;
        Pointer<T> forEach;
        Pointer<T> EnumeratorPointer;

        public LinkedList() { }

        public delegate void IteratorFunction(T t);
        public delegate bool FilterFunction(T t);
        public IteratorFunction iteratorFunction;

        public object Current => EnumeratorPointer.data;

        public bool SortedInsert(T data)
        {
            Pointer<T> pointer = head; Pointer<T> previous = null;
            while (pointer != null && data.CompareTo(pointer.data) > 0) { previous = pointer; pointer = pointer.next; }
            if (pointer != null && data.CompareTo(pointer.data) == 0) return false;
            if (previous == null) head = new Pointer<T>(data, pointer); else previous.next = new Pointer<T>(data, pointer);
            return true;
        }
        public void RowInsert(T data)
        {
            Pointer<T> pointer = head; Pointer<T> previous = null;
            while (pointer != null) { previous = pointer; pointer = pointer.next; }
            if (previous == null) head = new Pointer<T>(data, pointer); else previous.next = new Pointer<T>(data, pointer);
        }
        public T RowOut()
        {
            if (head != null) { T ki = head.data; head = head.next; return ki; }
            return default(T);
        }
        public bool Empty => head == null;

        public void Push(T data) { var newPointer = new Pointer<T>(data, head); head = newPointer; }
        public T Pop() => RowOut();

        public void IteratorInit() => forEach = head;
        public T NextData()
        {
            if (forEach == null) return default(T);
            T data = forEach.data; forEach = forEach.next; return data;
        }

        public void Iterator()
        {
            Pointer<T> pointer = head;
            while (pointer != null)
            {
                iteratorFunction?.Invoke(pointer.data);
                pointer = pointer.next;
            }
        }

        public bool Search(T item)
        {
            Pointer<T> pointer = head;
            while (pointer != null && !pointer.data.Equals(item)) pointer = pointer.next;
            if (pointer == null) return false;
            return true;
        }

        public void Delete(string name)
        {
            Pointer<T> pointer = head; Pointer<T> previous = null;
            while (pointer != null && !pointer.data.Equals(name)) { previous = pointer; pointer = pointer.next; }
            if (pointer != null)
            { if (previous == null) head = pointer.next; else previous.next = pointer.next; }
        }

        public void Delete(T item)
        {
            Pointer<T> pointer = head; Pointer<T> previous = null;
            while (pointer != null && item.CompareTo(pointer.data) != 0) { previous = pointer; pointer = pointer.next; }
            if (pointer != null)
            { if (previous == null) head = pointer.next; else previous.next = pointer.next; }
        }

        public LinkedList<T> Filter(FilterFunction szűrő)
        {
            Pointer<T> pointer = head;
            LinkedList<T> newList = new LinkedList<T>();
            while (pointer != null)
            {
                if (szűrő(pointer.data)) newList.SortedInsert(pointer.data);
                pointer = pointer.next;
            }
            return newList;
        }

        public LinkedList<T> Intersection(LinkedList<T> list2)
        {
            LinkedList<T> newList = new LinkedList<T>();
            Pointer<T> m1 = head; Pointer<T> m2 = list2.head;
            while (m1 != null && m2 != null)
            {
                int comp = m1.data.CompareTo(m2.data);
                if (comp < 0) m1 = m1.next;
                else if (comp > 0) m2 = m2.next;
                else { newList.SortedInsert(m1.data); m1 = m1.next; m2 = m2.next; }
            }
            return newList;
        }

        public LinkedList<T> Union(LinkedList<T> list2)
        {
            LinkedList<T> újlista = new LinkedList<T>();
            Pointer<T> m1 = head; Pointer<T> m2 = list2.head;
            while (m1 != null && m2 != null)
            {
                int comp = m1.data.CompareTo(m2.data);
                if (comp == -1) { újlista.SortedInsert(m1.data); m1 = m1.next; }
                else if (comp == 1) { újlista.SortedInsert(m2.data); m2 = m2.next; }
                else { újlista.SortedInsert(m1.data); m1 = m1.next; m2 = m2.next; }
            }
            while (m1 != null) { újlista.SortedInsert(m1.data); m1 = m1.next; }
            while (m2 != null) { újlista.SortedInsert(m2.data); m2 = m2.next; }

            return újlista;
        }

        public LinkedList<T> Difference(LinkedList<T> lista2)
        {
            LinkedList<T> újlista = this.Filter((hős) => true);
            LinkedList<T> metszet = this.Intersection(lista2);

            Pointer<T> m1 = metszet.head;
            while (m1 != null) { újlista.Delete(m1.data); m1 = m1.next; }

            return újlista;
        }

        public IEnumerator GetEnumerator()
        {
            this.EnumeratorPointer = new Pointer<T>(default(T), head);
            return (IEnumerator)this;
        }

        public bool MoveNext()
        {
            if (EnumeratorPointer != null) EnumeratorPointer = EnumeratorPointer.next;
            return EnumeratorPointer != null;
        }

        public void Reset()
        {
            EnumeratorPointer = new Pointer<T>(default(T), head);
        }
    }
}
