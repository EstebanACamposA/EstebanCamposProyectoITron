using DataStructures.Nodes;

namespace DataStructures
{
    namespace Lists
    {
        public class StackLinkedList<T>
        {
            private NodeLinkedList<T> first;
            public NodeLinkedList<T> First
            {
                get {return first;}
                set {first = value;}
            }

            private int len = 0;




            public StackLinkedList(T data)
            {
                first = new();
                First.Data = data;
                len = 1;
            }

            public StackLinkedList(){}

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
            public void Push(T data)
            {
                AddNode(new NodeLinkedList<T>(data));
            }

            public T Pop()
            {
                if (len == 0)
                {
                    System.Console.WriteLine("STACK IS ALREADY EMPTY at LinkedList.DeleteAt.");
                    return default(T);
                }

                NodeLinkedList<T>[] nodeAtAndPrevious = FindNodeAt(first, len - 1, 0, null);
                nodeAtAndPrevious[0].Next = nodeAtAndPrevious[1].Next;
                len --;
                return nodeAtAndPrevious[1].Data;
            }

            public T Peek()
            {
                if (len == 0)
                {
                    System.Console.WriteLine("STACK IS EMPTY at LinkedList.DeleteAt.");
                    return default(T);
                }

                return FindLastNode(first).Data;
            }

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