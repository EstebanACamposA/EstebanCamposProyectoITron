using UnityEngine;
using DataStructures;
// using System.Diagnostics;

public class Player
{
    public int player_ID; //Given by Game Manager.
    public int speed; //Given by Game Manager.  //Must be [1,10]
    public DataStructures.Lists.LinkedList<Point2D> trail = new();  //Built in constructor according to direction and LP_size
    public int direction = 1;  //Starts at 1 (up). //Must be {0,1,2,3}
    public int LP_size = 4; //Starts at 4. i. e. one for the player and a light path of 3.
    public float fuel = 100; //Starts at full.
    public DataStructures.Lists.LinkedList<int> items_queue = new();     //Starts empty.
    public DataStructures.Lists.LinkedList<int> power_ups_stack = new(); //Starts empty.
    public int shield_remaining = 0;      //Starts at 0.  //Set by power ups.   //Timer set on this.usePowerUp; decreased by UpdatePowerUps.
    public int hiperspeed_remaining = 0;  //Starts at 0.  //Set by power ups.   //Timer set on this.usePowerUp; decreased by UpdatePowerUps.
    public DataStructures.Lists.MatrixLinkedList<MapCell> map;  //Game Manager gives access to the map to the Player objects.
    private int item_timer = 90;
    public bool character_destroyed = false;

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

    public Player(int player_ID, Point2D startingCoords, int speed, DataStructures.Lists.MatrixLinkedList<MapCell> map)
    {
        this.player_ID = player_ID;
        this.speed = speed;
        this.map = map;

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
    public void UseItem()
    {
        if (items_queue.Size() >= 1)
        {
            int item_used = items_queue.FindAt(0);
            items_queue.DeleteAt(0);    //Dequeues.
            switch (item_used)
            {
                case 1: //Fuel
                    fuel += Random.Range(1, 21);
                    break;
                case 2: //LP size increase
                    LP_size += Random.Range(1,4);
                    break;
                case 3: //Bomb
                    Debug.Log("FUNCTION TO DESTROY THE PLAYER (BOMB USED) PENDING.");
                    break;
                default:
                    break;
            }
        }
    }
    public void UsePowerUp()
    {
        int power_up_used = power_ups_stack.FindAt(0);
        power_ups_stack.DeleteAt(0);    //Pops.
        switch (power_up_used)
        {
            case 4: //Shield
                shield_remaining = Random.Range(60,300);    //Time in frames
                break;
            case 5: //Hiperspeed
                hiperspeed_remaining = Random.Range(60,180);    //Time in frames
                break;
            default:
                break;
        }
    }
    public void GiveItemOrPU(int item_PU_ID)
    {
        switch (item_PU_ID)
        {
            case 1: //Fuel
                items_queue.AddAt(item_PU_ID, 0);
                break;
            case 2: //LP size increase
                items_queue.Add(item_PU_ID);
                break;
            case 3: //Bomb
                items_queue.Add(item_PU_ID);
                break;
            case 4: //Shield
                power_ups_stack.AddAt(item_PU_ID, 0);
                break;
            case 5: //Hiperspeed
                power_ups_stack.AddAt(item_PU_ID, 0);
                break;
            default:
                break;
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
    public void Update()
    {   
        fuel -= 0.2f;   //CHECK IF FUEL IS LESS THAN 0.2 TO DESTROY PLAYER. THIS CODE IS NOT COMPLETE.
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
        }
        // Deletes oldest LP if LP_size has been reached.
        if (trail.Size() > LP_size)
        {
            if (player_ID == 0)
            {
                Debug.Log("LP_size = " + LP_size);    
                Debug.Log("trail.Size() = " + trail.Size());
            }
        
            if (trail.Size() > 1)
            {
                // Only deletes LP if trail > 1. If it's 1 that means the player is only the head (this happens when is being deleted).
                Point2D LP_to_delete_from_matrix = trail.FindLast();
                map.FindAt(LP_to_delete_from_matrix.x, LP_to_delete_from_matrix.y).LP -= 1;    
            }
            trail.Delete();
        }
    }

    public void UpdatePowerUps()
    {
        shield_remaining -= 1;
        hiperspeed_remaining -= 1;
        if (item_timer <= 0)
        {
            UseItem();
        }
        item_timer --;
    }




    public void Delete()
    {
        Debug.Log("DELETE PLAYER WITH ID: " + player_ID);   //CODE FOR DELETING PLAYERS PENDING HERE!!!
        LP_size = 0;
        character_destroyed = true;
    }
}




