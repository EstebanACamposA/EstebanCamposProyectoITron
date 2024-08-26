using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoToPlayer : MonoBehaviour
{
    public int player_ID = 0;
    public int y = 1;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // for (int i = 0; i < GameManagement.Instance.Rows(); i++)
        // {
        //     for (int j = 0; j < GameManagement.Instance.Columns; j++)
        //     {
        //         int players_in_map_cell = GameManagement.Instance.map.FindAt(i,j).player_IDs.Size();
        //         for (int k = 0; k < players_in_map_cell; k++)
        //         {
        //             if (GameManagement.Instance.map.FindAt(i,j).player_IDs.FindAt(k) = player_ID)
        //             {
        //                 Point2D player_coords = GameManagement.Instance.map.FindAt(i,j);     
        //             }
                    
        //         }
                
        //     }
        // }
        Point2D player_coords_xy = GameManagement.Instance.GetPlayerCoords(player_ID);
        Vector3 move = new(player_coords_xy.x, y, player_coords_xy.y);
        move.x *= GameManagement.Instance.map_scale;
        move.z *= GameManagement.Instance.map_scale;
        transform.position = move;
        
    }
}
