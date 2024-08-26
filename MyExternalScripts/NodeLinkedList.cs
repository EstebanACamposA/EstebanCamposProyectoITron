using System;
namespace DataStructures
{
    namespace Nodes
    {
        public class NodeLinkedList<T> : Node<T>
        {
            private NodeLinkedList<T> next;
            public NodeLinkedList<T> Next
            {
                get {return next;}
                set {next = value;}
            }
            public NodeLinkedList(){}
            public NodeLinkedList(T data)
            {
                Data = data;
            }
            public NodeLinkedList(T data, NodeLinkedList<T> next)
            {
                Data = data;
                this.next = next;
            }
            public new NodeLinkedList<T> DeepCopy()
            {
                NodeLinkedList<T> new_node = new(Data);

                if(next != null)
                {
                    new_node.next = this.next.DeepCopy();
                }
                return new_node;
            }


            public override void Display()
            {
                string _next = next != null ? $"{next.Data}" : "null";
                Console.WriteLine($"({Data})->({_next})\n");
            }
            
        }
    }
}