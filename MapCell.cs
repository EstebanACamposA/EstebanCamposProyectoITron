using System.Collections.Generic;
using UnityEngine;
using DataStructures;

public class MapCell
{
    public DataStructures.Lists.LinkedList<int> player_IDs = new();
    public DataStructures.Lists.LinkedList<int> item_PU_IDs = new();
    // item IDs:
    // 1 Fuel Cell
    // 2 LP size increase
    // 3 Bomb
    // 4 Shield
    // 5 Hiperspeed

    // Amount of Light Paths. Invincibility can result in multiple LP in the same MapCell.
    public int LP = 0;
    public int LP_direction = 0;
    public bool LP_particle_is_instantiated = false;
        
    public MapCell(DataStructures.Lists.LinkedList<int> player_IDs, DataStructures.Lists.LinkedList<int> item_PU_IDs, int LP)
    {
        this.player_IDs = player_IDs;
        this.item_PU_IDs = item_PU_IDs;
        this.LP = LP;
    }
    public MapCell(){}
}