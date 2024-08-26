using DataStructures.Nodes;

namespace DataStructures
{
    namespace Lists
    {
        public class MatrixLinkedList<T>
        {
            private int rows;
            private int columns;
            public LinkedList<LinkedList<T>> first_row;    //THIS SHOULD BE PRIVATE.
            public MatrixLinkedList(int rows, int columns)
            {
                T default_data = default(T);
                first_row = new();
                for (int i = 0; i < columns; i++)
                {
                    LinkedList<T> column = new();
                    for (int j = 0; j < rows; j++)
                    {
                        column.Add(default_data);
                    }
                first_row.Add(column);
                }
                this.rows = rows;
                this.columns = columns;
            }

            public MatrixLinkedList(int rows, int columns, T data)
            {
                first_row = new();
                for (int i = 0; i < columns; i++)
                {
                    LinkedList<T> column = new(data);
                    for (int j = 0; j < rows; j++)
                    {
                        column.Add(data);
                    }
                first_row.Add(column);
                }
                this.rows = rows;
                this.columns = columns;
            }

            public void Display()
            {
                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < columns; j++)
                    {
                        System.Console.Write("(");
                        System.Console.Write(this.FindAt(i,j));
                        System.Console.Write(") ");
                    }
                    System.Console.WriteLine();
                }
            }
            public void Display(string text)
            {
                System.Console.WriteLine(text);
                Display();
            }

            public T FindAt(int row, int column)
            {
                return first_row.FindAt(column).FindAt(row);
            }
            public void SetAt(T data, int row, int column)
            {
                first_row.FindAt(column).SetAt(data, row);
            }
            /// <summary>
            /// Incomplete. Bahviour should be:
            /// Deletes the data of an element at this position. Is different from LinkedList delete because this deletes the node's value and not the node.
            /// </summary>
            public void DeleteAt(int row, int column)
            {
                first_row.FindAt(column).DeleteAt(row);
            }
            public int Rows()
            {
                return rows;
            }
            public int Columns()
            {
                return columns;
            }
        }
    }
}