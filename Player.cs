using UnityEngine;
using DataStructures;
using Unity.VisualScripting;
using System.Data.Common;
using JetBrains.Annotations;
// using System.Diagnostics;

public class Player
{
    public int player_ID; //Given by Game Manager.
    private int speed; //Given by Game Manager.  //Must be [1,10]
    public int current_speed; //Starts the same as speed. It changes depending on whether hyperspeed is active (> 0).
    public DataStructures.Lists.LinkedList<Point2D> trail = new();  //Built in constructor according to direction and LP_size
    public int direction = 1;  //Starts at 1 (up). //Must be {0,1,2,3}
    public int LP_size = 4; //Starts at 4. i. e. one for the player and a light path of 3.
    public float fuel = 100; //Starts at full. THIS SHOULD BE 100 CHANGE FOR DEBUGGING!!!
    public DataStructures.Lists.LinkedList<int> items_queue = new();     //Starts empty.
    public DataStructures.Lists.LinkedList<int> power_ups_stack = new(); //Starts empty.
    public int shield_remaining = 0;      //Starts at 0.  //Set by power ups.   //Timer set on this.usePowerUp; decreased by UpdatePowerUps.
    public int hyperspeed_remaining = 0;  //Starts at 0.  //Set by power ups.   //Timer set on this.usePowerUp; decreased by UpdatePowerUps.
    public DataStructures.Lists.MatrixLinkedList<MapCell> map;  //Game Manager gives access to the map to the Player objects.
    private int item_timer = 90;
    public bool character_destroyed = false;
    private int max_items = 6;
    private int max_PU = 6;
    public bool UI_is_updated = false;
    public bool has_moved = true;   //Accesed by GameManagement.Control(). Set false when a key is down. Set true on Update. 
    public int last_fuel_increase = 0;  //Accesed By UpdateUIFuel. Decreased by main Update function each frame.

    //Movement information. Used to decide direction when multiple wasd keys are pressed. true for vertical; false for horizontal.
    private bool previous_direction = true;
    // private void DirectionToXYConversion(int x, int y)
    // {
    //     x = 0;
    //     y = 0;
    //     switch (direction)
    //         {
    //             case 0:
    //                 x = 1;
    //                 break;
    //             case 1:
    //                 y = 1;
    //                 break;
    //             case 2:
    //                 x = -1;
    //                 break;
    //             case 3:
    //                 y = -1;
    //                 break;
    //             default:
    //                 break;
    //         }
    // }

    public Player(int player_ID, Point2D startingCoords, int speed, DataStructures.Lists.MatrixLinkedList<MapCell> map, int max_items, int max_PU)
    {
        this.player_ID = player_ID;
        this.speed = speed;
        current_speed = speed;
        this.map = map;
        this.max_items = max_items;
        this.max_PU = max_PU;

        int x_to_add = 0;
        int y_to_add = 0;
        switch (direction)
            {
                case 0:
                    x_to_add = 1;
                    break;
                case 1:
                    y_to_add = 1;
                    break;
                case 2:
                    x_to_add = -1;
                    break;
                case 3:
                    y_to_add = -1;
                    break;
                default:
                    break;
            }
        
        trail.Add(startingCoords);
        map.FindAt(startingCoords.x, startingCoords.y).player_IDs.Add(player_ID);
        Debug.Log("Passed map.FindAt(startingCoords.x, startingCoords.y).player_IDs.Add(player_ID);");

        for (int i = 1; i < LP_size; i++)
        {
            // Fills trail with Point2Ds in a straight line starting at startingCoords towards opposite of direction.
            Point2D trail_node = startingCoords.Add(x_to_add*i*-1, y_to_add*i*-1);
            Debug.Log("At Player constructor Added LP at (" + trail_node.x + ',' + trail_node.y + ')');
            trail.Add(trail_node);
            MapCell current_mapCell_for_LP = map.FindAt(trail_node.x, trail_node.y);
            current_mapCell_for_LP.LP += 1;
            current_mapCell_for_LP.LP_direction = direction;

        }
    }
            /// <summary>
            /// This function must be periodically called by Game Manager so that items are spent at a set interval.
            /// </summary>
    private void UseItem()
    {
        if (items_queue.Size() >= 1)
        {
            int item_used = items_queue.FindAt(0);
            items_queue.DeleteAt(0);    //Dequeues.
            UI_is_updated = false;
            switch (item_used)
            {
                case 1: //Fuel
                    last_fuel_increase = 10 + Random.Range(0, 21);
                    fuel += last_fuel_increase;
                    GameManagement.Instance.fuel_textbox_add_fuel_timer = 30;
                    break;
                case 2: //LP size increase
                    LP_size += Random.Range(1,4);
                    break;
                case 3: //Bomb
                    Debug.Log("Bomb has exploded on player of ID: " + player_ID + ". shield_remaining: " + shield_remaining);
                    Delete();
                    break;
                default:
                    break;
            }
        }
    }
    public void UsePowerUp()
    {
        if (power_ups_stack.Size() >= 1)
        {
            int power_up_used = power_ups_stack.FindLast();
            power_ups_stack.Delete();    //Pops.
            UI_is_updated = false;
            switch (power_up_used)
            {
                case 4: //Shield
                    shield_remaining = GameManagement.Instance.min_shield + Random.Range(0,GameManagement.Instance.max_shield - GameManagement.Instance.min_shield);    //Time in frames
                    break;
                case 5: //Hyperspeed
                    GameManagement.Instance.turns[player_ID] = Time.frameCount - 1; //Makes the player act instantly once when using hiperspeed.
                    hyperspeed_remaining = GameManagement.Instance.min_hyperspeed + Random.Range(0,GameManagement.Instance.max_hyperspeed - GameManagement.Instance.min_hyperspeed);    //Time in frames
                    break;
                default:
                    break;
            }
        }

    }
    public void GiveItemOrPU(int item_PU_ID)
    {
        switch (item_PU_ID)
        {
            case 1: //Fuel
                if (items_queue.Size() < max_items)
                {
                    items_queue.AddAt(item_PU_ID, 0);
                    UI_is_updated = false;
                }
                break;
            case 2: //LP size increase
                if (items_queue.Size() < max_items)
                {
                    items_queue.Add(item_PU_ID);
                    UI_is_updated = false;
                }
                break;
            case 3: //Bomb
                if (items_queue.Size() < max_items)
                {
                    items_queue.Add(item_PU_ID);
                    UI_is_updated = false;
                }
                break;
            case 4: //Shield
                if (power_ups_stack.Size() < max_PU)
                {
                    power_ups_stack.Add(item_PU_ID);    
                    UI_is_updated = false;
                }
                break;
            case 5: //Hyperspeed
                if (power_ups_stack.Size() < max_PU)
                {
                    power_ups_stack.Add(item_PU_ID);    
                    UI_is_updated = false;
                }
                break;
            default:
                break;
        }
    }
    public void RotatePULeft()
    {
        if (power_ups_stack.Size() > 0)
        {
            int current_top = power_ups_stack.FindLast();
            power_ups_stack.Delete();
            power_ups_stack.AddAt(current_top, 0);
            UI_is_updated = false;
        }
    }
    public void RotatePURight()
    {
        if (power_ups_stack.Size() > 0)
        {
            int current_first = power_ups_stack.FindAt(0);
            power_ups_stack.DeleteAt(0);
            power_ups_stack.Add(current_first);
            UI_is_updated = false;
        }
    }


    public void ChangeDirection(int x, int y)   //direction: {0,1,2,3} -> {right, up, left, down}
    {
        if ((x != 0) && (y != 0))
        {
            if (previous_direction)
            {
                direction = 1-x;
            }
            else
            {
                direction = 2-y;
            }
            return;
        }
        if (x != 0)
        {
            direction = 1-x;
            return;
        }
        if (y != 0)
        {
            direction = 2-y;
            return;
        }
        //Do nothing if both x and y are 0.
    }
    public void ChangeDirection(Point2D xy)
    {
        ChangeDirection(xy.x, xy.y);
    }

            /// <summary>
            /// Moves player one cell. The rate at which this function is called must depend on speed.
            /// </summary>
    public void Update()    //This is not Unity's Update()
    {   
        fuel -= 0.2f;   //Consume fuel.
        if (fuel < 0.2f & !character_destroyed) //Destroy the player when out of fuel.
        {
            Debug.Log("Player " + player_ID + " has run out of fuel!");
            Delete();
        }
        if (!character_destroyed)
        {
            
            
            //Use current direction to get y and x to add a new node. The new node will be the new position of the player;
            //Sets previous_direction. false, horizontal; true, vertical
            int x_to_add = 0;
            int y_to_add = 0;
            switch (direction)
                {
                    case 0:
                        x_to_add = 1;
                        previous_direction = false;
                        break;
                    case 1:
                        y_to_add = 1;
                        previous_direction = true;
                        break;
                    case 2:
                        x_to_add = -1;
                        previous_direction = false;
                        break;
                    case 3:
                        y_to_add = -1;
                        previous_direction = true;
                        break;
                    default:
                        break;
                }
            Point2D current_head = trail.First.Data;
            Point2D new_head = new(current_head.x + x_to_add, current_head.y + y_to_add);

            // Use modulus of current coords by the dimensions of the map matrix to make the player teleport to the opposite side once they've reached a limit.
            new_head.x = (new_head.x + GameManagement.Instance.Rows())%GameManagement.Instance.Rows();       //rows columns may be REVERSED!!!!!
            new_head.y = (new_head.y + GameManagement.Instance.Columns())%GameManagement.Instance.Columns();

            // Puts the player at the start of the trail list.
            trail.AddAt(new_head, 0);
            // Updates de player's position on the map.
            map.FindAt(new_head.x, new_head.y).player_IDs.Add(player_ID);
            map.FindAt(current_head.x, current_head.y).player_IDs.DeleteValue(player_ID);
            // Creates LP on the map.
            map.FindAt(current_head.x, current_head.y).LP += 1;
            map.FindAt(current_head.x, current_head.y).LP_direction = direction;
            map.FindAt(current_head.x, current_head.y).LP_particle_is_instantiated = false;

            has_moved = true;   // Related to WASD controls.
        }
        // Deletes oldest LP if LP_size has been reached.
        if (trail.Size() > LP_size)
        {
            // Prints player 0's trail information. Debugging
            // if (player_ID == 0)
            // {
            //     Debug.Log("LP_size = " + LP_size);    
            //     Debug.Log("trail.Size() = " + trail.Size());
            // }
        
            if (trail.Size() >= 1)
            {
                // Only deletes LP if trail > 1. If it's 1 that means the player is only the head (this happens when is being deleted).
                Point2D LP_to_delete_from_matrix = trail.FindLast();
                map.FindAt(LP_to_delete_from_matrix.x, LP_to_delete_from_matrix.y).LP -= 1;    
                if (character_destroyed & player_ID == 0)
                {
                    Debug.Log("Deleted an LP");
                }
            }
            trail.Delete();
        }

        if (hyperspeed_remaining > 0)
        {
            int flat_speed_increase = 8;
            current_speed = (speed + flat_speed_increase)*2 - (speed + flat_speed_increase)/hyperspeed_remaining;    //Change speed multiplier for something reasonable.
        }
        else
        {
            current_speed = speed;
        }
    }

    public void UpdatePowerUps()
    {
        shield_remaining -= 1;
        hyperspeed_remaining -= 1;
        if (item_timer <= 0)
        {
            UseItem();
            item_timer += 90;
        }
        if (items_queue.Size() >= 1)
        {
            item_timer --;    
        }
    }




    public void Delete()
    {
        if (!character_destroyed)   //THIS IS REDUNDANTLY VALIDATED IN OTHER PLACES!!!!!!
        {
            if (shield_remaining > 0)
            {
                if (fuel > 0.2)
                {
                    Debug.Log("Player " + player_ID + " has been saved by a shield! ♥");
                    return;    
                }
                Debug.Log("A shield won't protect Player " + player_ID + " from low fuel :(");
            }
            Debug.Log("Delete player of ID: " + player_ID);
            LP_size = 0;
            character_destroyed = true;
            // Erases head.
            Point2D player_ID_to_delete_from_matrix = trail.FindAt(0);
            map.FindAt(player_ID_to_delete_from_matrix.x, player_ID_to_delete_from_matrix.y).player_IDs.DeleteValue(player_ID);
            trail.DeleteAt(0);    
            // Spawns its objects in the stage
            GameManagement.Instance.SpawnDroppedItemsAndPowerUps(items_queue);
            GameManagement.Instance.SpawnDroppedItemsAndPowerUps(power_ups_stack);
            UI_is_updated = false;
        }
        

    }
}




