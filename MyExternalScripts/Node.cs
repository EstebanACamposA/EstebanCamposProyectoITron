using System;
namespace DataStructures
{
    namespace Nodes
    {
        public class Node<T>
        {
            private T data;
            
            public T Data
            {
                get {return data;}
                set {data = value;}
            }

            public virtual void Display()
            {
                Console.WriteLine("Display method from Node<T> class (not overriden).");
                System.Console.WriteLine("kzzkt.");
            }

            public Node<T> ShallowCopy()
            {
                return (Node<T>) this.MemberwiseClone();
            }

            public virtual Node<T> DeepCopy()
            {
                System.Console.WriteLine("DeepCopy method from Node<T> class (not overriden).");
                return null;
            }
        }
    }
}
