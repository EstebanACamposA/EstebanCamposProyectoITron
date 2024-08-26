
using DataStructures.Nodes;

namespace DataStructures
{
    namespace Lists
    {
        public class LinkedList<T>
        {
            private NodeLinkedList<T> first;
            public NodeLinkedList<T> First
            {
                get {return first;}
                set {first = value;}
            }

            private int len = 0;




            public LinkedList(T data)
            {
                first = new();
                First.Data = data;
                len = 1;
            }

            public LinkedList(){}

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
            private int FindNodeAtValue(NodeLinkedList<T> first, T data, int i, NodeLinkedList<T> previousNode)
            {
                if (first == null)
                {
                    System.Console.WriteLine("Data not found at LinkedList.FindNodeAtValue. Data: " + data);
                    return -1;
                }
                if (first.Data.Equals(data))
                {
                    return i; //Returns index of node with data.
                }
                else
                {
                    previousNode = first;
                    return FindNodeAtValue(first.Next, data, i+1, previousNode);

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
            public void Add(T data)
            {
                AddNode(new NodeLinkedList<T>(data));
            }

            private void AddNodeAt(NodeLinkedList<T> node, int pos)
            {
                NodeLinkedList<T>[] nodeAtAndPrevious = FindNodeAt(first, pos, 0, null);
                node.Next = nodeAtAndPrevious[1];
                len ++;
                if (nodeAtAndPrevious[0] != null) //i.e. pos > 0.
                {
                    nodeAtAndPrevious[0].Next = node;
                    return;
                }
                // System.Console.WriteLine("ENTRO AQUI POS == 0 EN AddNodeAt.");
                first = node;
            }

            public void AddAt(T data, int pos)
            {
                AddNodeAt(new NodeLinkedList<T>(data), pos);
            }
            
            /// <summary>
            /// Deletes last element.
            /// </summary>
            public void Delete()
            {
                DeleteAt(len - 1);
            }
            public void DeleteAt(int pos)
            {
                if (len == 0)
                {
                    System.Console.WriteLine("LIST IS ALREADY EMPTY at LinkedList.DeleteAt.");
                    return;
                }
                if (pos >= len )
                {
                    System.Console.WriteLine("Index out of range in DeleteAt.");
                }
                if (pos == 0)
                {
                    if (len >= 1)
                    {
                        first = first.Next;
                        len --;
                    }
                    return;
                }
                NodeLinkedList<T>[] nodeAtAndPrevious = FindNodeAt(first, pos, 0, null);
                nodeAtAndPrevious[0].Next = nodeAtAndPrevious[1].Next;
                len --;
            }
            // public void DeleteValue(T data)  //Old function. Delete if new DeleteValue(T data) is trusted.
            // {
            //     if (len == 0)
            //     {
            //         System.Console.WriteLine("LIST IS ALREADY EMPTY at LinkedList.DeleteValue.");
            //         return;
            //     }
            //     NodeLinkedList<T>[] nodeAtAndPrevious = FindNodeAtValue(first, data, 0, null);
            //     if (nodeAtAndPrevious == null)
            //     {
            //         return;
            //     } 
            //     nodeAtAndPrevious[0].Next = nodeAtAndPrevious[1].Next;
            //     len --;
            // }
            public void DeleteValue(T data)
            {
                int foundAt = FindNodeAtValue(first, data, 0, null);
                if (foundAt == -1)
                {
                    return;
                }
                DeleteAt(foundAt); 
            }

            public T FindAt(int pos)
            {
                NodeLinkedList<T>[] nodeAtAndPrevious = FindNodeAt(first, pos, 0, null);
                return nodeAtAndPrevious[1].Data;
            }
            public T FindLast()
            {
                return FindAt(len-1);
            }

            public void SetAt(T data, int pos)
            {
                FindNodeAt(first, pos, 0, null)[1].Data = data;
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