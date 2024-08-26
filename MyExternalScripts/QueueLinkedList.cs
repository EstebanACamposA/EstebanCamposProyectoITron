using DataStructures.Nodes;

namespace DataStructures
{
    namespace Lists
    {
        public class QueueLinkedList<T>
        {
            private NodeLinkedList<T> first;
            public NodeLinkedList<T> First
            {
                get {return first;}
                set {first = value;}
            }

            private int len = 0;

            public QueueLinkedList(T data)
            {
                first = new();
                First.Data = data;
                len = 1;
            }

            public QueueLinkedList(){}

            public void Display(string msg)
            {
                System.Console.WriteLine(msg);
                System.Console.Write('[');
                DisplayAux(first);
                System.Console.WriteLine(']');
            }
            public void Display()
            {
                System.Console.Write('[');
                DisplayAux(first);
                System.Console.WriteLine(']');
            }
            private void DisplayAux(NodeLinkedList<T> first)
            {
                if (first != null)
                {
                    System.Console.Write($"({first.Data})->");
                    DisplayAux(first.Next);
                }
            }
            private NodeLinkedList<T> FindLastNode(NodeLinkedList<T> first)
            {
                if (first.Next == null)
                {
                    return first;
                } 
                else
                {
                    return FindLastNode(first.Next);
                }
            }

            private NodeLinkedList<T>[] FindNodeAt(NodeLinkedList<T> first, int pos, int i, NodeLinkedList<T> previousNode)
            {
                if (i < pos)
                {
                    if (i == pos - 1)
                
                    {
                        previousNode = first;
                        return FindNodeAt(first.Next, pos, i+1, previousNode);
                    }
                    if (first.Next != null)
                    {
                        return FindNodeAt(first.Next, pos, i+1, previousNode);
                    }
                    System.Console.WriteLine("Index out of range in AddNodeAt/FindNodeAt.");

                    return null;
                } 
                else
                {
                    NodeLinkedList<T>[] res = new Nodes.NodeLinkedList<T>[2];
                    res[0] = previousNode;
                    res[1] = first;

                    return res; //Returns [node at pos-1, node at pos]
                }
            }
            
            private void AddNode(NodeLinkedList<T> node)
            {
                if (first == null)
                {
                    first = node;
                    len ++;
                    return;
                }
                NodeLinkedList<T> lastNode = FindLastNode(first);
                lastNode.Next = node;
                len ++;
            }
            public void Enqueue(T data)
            {
                AddNode(new NodeLinkedList<T>(data));
            }

            // private void AddNodeAt(NodeLinkedList<T> node, int pos)
            // {
            //     NodeLinkedList<T>[] nodeAtAndPrevious = FindNodeAt(first, pos, 0, null);
            //     node.Next = nodeAtAndPrevious[1];
            //     len ++;
            //     if (nodeAtAndPrevious[0] != null) //i.e. pos > 0.
            //     {
            //         nodeAtAndPrevious[0].Next = node;
            //         return;
            //     }
            //     // System.Console.WriteLine("ENTRO AQUI POS == 0 EN AddNodeAt.");
            //     first = node;
            // }

            // public void AddAt(T data, int pos)
            // {
            //     AddNodeAt(new NodeLinkedList<T>(data), pos);
            // }


            public T Dequeue()
            {
                if (len >= 1)
                {
                    T data = first.Data;
                    first = first.Next;
                    len --;
                    return data;
                }
                return default(T);
            }

            public T Peek()
            {
                if (len >= 0)
                {
                    return first.Data;
                }
                System.Console.WriteLine("Cannot peek empty queue on QueueLinkedList.Peek()."); 
                return default(T);
            }

            // public NodeLinkedList<T> FindAt(int pos)
            // {
            //     NodeLinkedList<T>[] nodeAtAndPrevious = FindNodeAt(first, pos, 0, null);
            //     return nodeAtAndPrevious[1];
            // }

            // public void SetAt(T data, int pos)
            // {
            //     FindAt(pos).Data = data;
            // }

            public bool IsEmpty()
            {
                if (len == 0)
                {
                    return true;
                }
                return false;
            }

            public int Size()
            {
                return len;
            }
        }
    }
}