using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// namespace myUnityScripts
// {    
public class Point2D
{
    public int x;
    public int y;
    public Point2D(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

            /// <summary>
            /// Creates new Point2D as adition of this Point2D and given x y values.
            /// Does not modify this Point2D.
            /// </summary>
    public Point2D Add(int x, int y)
    {
        Point2D add_result = new(this.x, this.y);
        add_result.x += x;
        add_result.y += y;
        return add_result;
    }
}
// }