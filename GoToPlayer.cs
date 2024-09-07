using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoToPlayer : MonoBehaviour
{
    public int player_ID = 0;
    public int y = 1;
    private Color starting_color;
    private Renderer found_renderer;
    // Start is called before the first frame update
    void Start()
    {
        found_renderer = GetComponent<Renderer>();
        starting_color = Color.HSVToRGB(0.2f*player_ID%1, 0.5f, 0.5f);
        found_renderer.material.color = starting_color; 
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

        ChangeColorPU();
    }

    private void ChangeColorPU()
    {
        Point2D PU_status = GameManagement.Instance.GetPlayerShieldAndHyperspeed(player_ID);
        float shield_color_factor = PU_status.x / (float)GameManagement.Instance.max_shield;
        float hyperspeed_color_factor = PU_status.y / (float)GameManagement.Instance.max_shield;
        
        if (PU_status.x > 0)
        {
            if (PU_status.y > 0)
            {
                float average_factor = (shield_color_factor + hyperspeed_color_factor)/2;
                found_renderer.material.color = Color.Lerp(starting_color, new Color(0, 1, 5), average_factor * 0.8f + 0.2f);
                return;
            }
            found_renderer.material.color = Color.Lerp(starting_color, new Color(0, 1, 0), shield_color_factor * 0.8f + 0.2f);
        }
        if (PU_status.y > 0)
        {
            found_renderer.material.color = Color.Lerp(starting_color, new Color(0, 1, 1), hyperspeed_color_factor * 0.8f + 0.2f);
            return;
        }

        if (PU_status.x == 0 | PU_status.y == 0)
        {
            found_renderer.material.color = starting_color;
        }
    }
}
