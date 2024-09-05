using UnityEngine;
using DataStructures;
using System.Drawing;
using DataStructures.Lists;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using System;
using TMPro;
// using myUnityScripts;

public class GameManagement : MonoBehaviour
{
    public static GameManagement Instance { get; private set; }

    public int playerCount = 6;
    public int[] turns;
    public Player[] players;
    public int m = 24;
    public int n = 30;
    public int user_player = 0;
    public int base_spawn_time = 21600;   //Time between spawns per cell.    //Underscore is ignored (e.g 500_000).
    private int spawn_timer = 240;  //First item spawn in frames.
    public int max_items = 6;
    public int max_PU = 6;
    public int max_shield = 360; // Maximum duration of shield PU in frames.
    public int min_shield = 60; // Min duration of shield PU in frames.
    public int max_hyperspeed = 360; // Maximum duration of hyperspeed PU in frames.
    public int min_hyperspeed = 360; // Min duration of hyperspeed PU in frames.
    // Controller related variables.
    private int current_horizontal_key = 0;
    private int current_vertical_key = 0;
    public DataStructures.Lists.MatrixLinkedList<MapCell> map;
    // Graphics related variables.
    public GameObject light_path_particle;

    public GameObject item_fuel_cell;
    public GameObject item_LP_size_increase;
    public GameObject item_bomb;
    public GameObject PU_shield;
    public GameObject PU_hyperspeed;

    public GameObject tile;
    public GameObject Bike;
    
    public int map_scale = 2;
    public int base_y = 0;
    public GameObject floor;
    public GameObject main_camera;
    private DataStructures.Lists.MatrixLinkedList<GameObject> LP_matrix;
    private DataStructures.Lists.MatrixLinkedList<LinkedList<GameObject>> item_PU_matrix;
    // UI Related variables.
    public Canvas canvas;
    public GameObject fuel_textbox;
    public int fuel_textbox_add_fuel_timer = -1;
    public GameObject fuel_cell_image;
    public GameObject LP_size_increase_image;
    public GameObject bomb_image;
    public GameObject shield_image;
    public GameObject hspeed_image;
    private DataStructures.Lists.LinkedList<LinkedList<GameObject>> UI_item_list;
    private DataStructures.Lists.LinkedList<LinkedList<GameObject>> UI_PU_list;
    private void Awake()
    {
        Debug.Log("HAS ENTERED AWAKE FUNCTION IN THE GAME MANAGER.");
        // Implement the singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keeps this object when loading new scenes
        }
        else
        {
            Destroy(gameObject); // Ensures there's only one instance
        }

        // Main program starts here.
        // Creates map with empty MapCells
        map = new(m, n);
        for (int i = 0; i < m; i++)
        {
            for (int j = 0; j < n; j++)
            {
                DataStructures.Lists.LinkedList<int> instantiation_map_cell_player_IDs = new();
                DataStructures.Lists.LinkedList<int> instantiation_map_cell_item_PU_IDs = new();
                MapCell instantiation_map_cell = new(instantiation_map_cell_player_IDs, instantiation_map_cell_item_PU_IDs, 0);
                map.SetAt(instantiation_map_cell, i, j);
            }
        }
        turns = new int[playerCount];
        players = new Player[playerCount];

        // Creates players in the program and instantiates the visible players. 
        for (int i = 0; i < playerCount; i++)
        {
            //Player object creation.
            turns[i] = i*60;    // Delay at the start of the game for each player to start based on their ID. Player 0 is the user and has no delay.
            int x_of_new_player = m/playerCount*i;
            int y_of_new_player = n/2 + 3;
            Point2D coords_of_new_player = new(x_of_new_player, y_of_new_player);
            int SPEEDdELETETHIS = 9;   //others DEBUG
            if (i == 0)
            {
                SPEEDdELETETHIS = 9;   //player 0 DEBUG
            }
            players[i] = new Player(i, coords_of_new_player, SPEEDdELETETHIS, map, max_items, max_PU);   //  ----!!!!!!!!!!!!speed SHOULD BE Random.Range(1,11), NOT 3!!!!!!!!!!!!----
        
            //Visible player instantiation.
            GameObject new_bike = Instantiate(Bike);    // Visible player control their own position and rotation.
            GoToPlayer new_bike_script = new_bike.GetComponent<GoToPlayer>();
            new_bike_script.player_ID = i;
            new_bike_script.y = base_y;
        }


        // Creates graphic maps.
        LP_matrix = new(m,n);
        item_PU_matrix = new(m,n);
        for (int i = 0; i < m; i++)
        {
            for (int j = 0; j < n; j++)
            {
                LinkedList<GameObject> item_PU_list = new();
                item_PU_matrix.SetAt(item_PU_list, i, j);
            }
        }
        // Creates UI lists.
        GameObject[] items_images_array = new GameObject[3];
        items_images_array[0] = fuel_cell_image;
        items_images_array[1] = LP_size_increase_image;
        items_images_array[2] = bomb_image;

        GameObject[] PU_images_array = new GameObject[2];
        PU_images_array[0] = shield_image;
        PU_images_array[1] = hspeed_image;

        UnityEngine.UI.Image sprite;
        UI_item_list = new();
        for (int i = 0; i < max_items; i++)
        {
            
            DataStructures.Lists.LinkedList<GameObject> items_image_list = new();
            for (int j = 0; j < items_images_array.Length; j++)
            {
                GameObject instantiated_image = Instantiate(items_images_array[j], canvas.transform);
                sprite = instantiated_image.GetComponent<UnityEngine.UI.Image>();
                sprite.enabled = false;
                RectTransform rectTransform = instantiated_image.GetComponent<RectTransform>();
                rectTransform.anchoredPosition = new Vector2(0, -100*i-75);
                // rectTransform.sizeDelta = new Vector2(100, 100);
                items_image_list.Add(instantiated_image);
            }   
            UI_item_list.Add(items_image_list);
        }

        UI_PU_list = new();
        for (int i = 0; i < max_PU; i++)
        {
            
            DataStructures.Lists.LinkedList<GameObject> PU_image_list = new();
            for (int j = 0; j < PU_images_array.Length; j++)
            {
                GameObject instantiated_image = Instantiate(PU_images_array[j], canvas.transform);
                sprite = instantiated_image.GetComponent<UnityEngine.UI.Image>();
                sprite.enabled = false;
                RectTransform rectTransform = instantiated_image.GetComponent<RectTransform>();
                rectTransform.anchoredPosition = new Vector2(-100*i-75, -525);
                // rectTransform.sizeDelta = new Vector2(100, 100);
                PU_image_list.Add(instantiated_image);
            }   
            UI_PU_list.Add(PU_image_list);
        }


        //Sets max fps to 60.
        Application.targetFrameRate = 60;
        Debug.Log("Exits Awake function.");
    }

    public int Rows()
    {
        return m;
    }
    public int Columns()
    {
        return n;
    }


    private void UpdatePlayers()
    {
        for (int i = 0; i < playerCount; i++)
        {
            if (turns[i] < Time.frameCount)
            {
                // Updates player and then map only when is their turn.

                // Debug prints
                // if (i == 0)
                // {
                //     Debug.Log("Player " + i + "s turn" + "\n" + "players[i].LP_size = " + players[i].LP_size + "\n" + "players[i].trail.Size() = " + players[i].trail.Size());
                //     PrintLPMatrixOnce();
                // }
                
                if (i != user_player)
                {
                    if (UnityEngine.Random.Range(0,6-players[i].power_ups_stack.Size()) == 0) // Bots are more likely to use PU the more they have.
                    {
                        players[i].UsePowerUp();
                    }
                }
                BotBehavior(i, false);
                players[i].Update();
                UpdateUIFuel(); // Move this after UpdateMap ?????????
                turns[i] += 120/players[i].current_speed;   // frecuency = 1/2 * speed in steps/second.
                UpdateMap();
            }
            players[i].UpdatePowerUps();
        }
    }

    private void UpdateMap()
    {
        // int IPROBE = 0;  //For debugging.
        for (int i = 0; i < m; i++)
        {
            // IPROBE = i;  //For debugging.
            for (int j = 0; j < n; j++)
            {
                MapCell current_MC = map.FindAt(i,j);

                //Checks for collision between players.
                int player_count = current_MC.player_IDs.Size();
                if (player_count > 1)
                {
                    for (int k = 0; k < player_count; k++)
                    {
                        players[current_MC.player_IDs.FindAt(0)].Delete();
                    }
                }
                //Checks for a single player collision with LP or items.
                if (player_count == 1)
                {
                    Player current_player = players[current_MC.player_IDs.FindAt(0)];
                    //Checks for a single player collision with LP.
                    if (current_MC.LP >= 1)
                    {
                        Debug.Log("Deleting player of ID: " + current_MC.player_IDs.FindAt(0));
                        current_player.Delete();
                    }
                    //Checks for player collision with items.
                    // int item_PU_IDs_size = current_MC.item_PU_IDs.Size();
                    // if (item_PU_IDs_size >= 1)
                    // {
                    //     for (int k = 0; k < item_PU_IDs_size; k++)
                    //     {
                    //         //Finds a player in current MapCell and gives all its items to it.
                    //         players[current_MC.player_IDs.FindAt(0)].GiveItemOrPU(current_MC.item_PU_IDs.FindAt(k));    //DOES NOT EMPTY current_MC.item_PU_IDs. PROBABLY SHOULD.!!!!!!!!!!!!! 
                    //     }
                    // }
                    while (current_MC.item_PU_IDs.Size() != 0)
                    {
                        current_player.GiveItemOrPU(current_MC.item_PU_IDs.FindAt(0));
                        current_MC.item_PU_IDs.DeleteAt(0);
                    }
                }

                //Particle instantiation and removal (graphics).

                //Checks for new LP in current_MC to instantiate or delete its particle effect GameObject.
                //Checks if a new LP particle needs to be instantiated.
                if (current_MC.LP >= 1 && !current_MC.LP_particle_is_instantiated)
                {
                    Point2D new_LP_coords = new(i,j);
                    //InstantiateLPParticle(int amount, Point2D coordinates, int direction, MapCell current_MC) currently does nothing with 'int amount'. May not need it.
                    InstantiateLPParticle(current_MC.LP, new_LP_coords, current_MC.LP_direction, current_MC);
                }
                //If an old LP particle needs to be deleted.
                if (current_MC.LP == 0 && current_MC.LP_particle_is_instantiated)
                {
                    Point2D LP_to_delete_coords = new(i,j);
                    DeleteLPParticle(LP_to_delete_coords, current_MC);

                }

                //Checks if new item/PU particles need to be instantiated.
                if (current_MC.item_PU_particles_instantiated < current_MC.item_PU_IDs.Size())
                {
                    Point2D new_item_PU_coords = new(i,j);
                    for (int k = current_MC.item_PU_particles_instantiated; k < current_MC.item_PU_IDs.Size(); k++)
                    {
                        int item_PU_type = current_MC.item_PU_IDs.FindAt(k);
                        InstantiateItemPUParticle(new_item_PU_coords, item_PU_type, current_MC);
                    }
                    // int item_PU_type = current_MC.item_PU_IDs.FindLast();
                    // InstantiateItemPUParticle(new_item_PU_coords, current_MC);
                }
                //If used item/PU particles need to be deleted.
                if (current_MC.item_PU_IDs.Size() == 0 && current_MC.item_PU_particles_instantiated > 0)
                {
                    Point2D item_PU_to_delete_coords = new(i,j);
                    DeleteItemPUParticle(item_PU_to_delete_coords, current_MC);

                }

            }

        }
        // Debug.Log("Exits UpdateMap with IPROBE = " + IPROBE);
    } 

    private void InstantiateLPParticle(int amount, Point2D coordinates, int direction, MapCell current_MC)
    {
        //Creates particle GameObject;
        Vector3 scaled_coordinates = new(coordinates.x, base_y, coordinates.y);
        scaled_coordinates *= map_scale;
        scaled_coordinates.y = base_y;
        GameObject instantiated = GameObject.Instantiate(light_path_particle, scaled_coordinates, Quaternion.identity);

        //Tells map that particle has been instantiated.
        map.FindAt(coordinates.x, coordinates.y).LP_particle_is_instantiated = true;
        //Rotates particle.
        instantiated.GetComponent<ParticleSystem>().transform.Rotate(0, -90*direction, 0);

        //If there were already an LP particle it must be overwritten.
        GameObject particle_at_postition = LP_matrix.FindAt(coordinates.x, coordinates.y);
        if (!particle_at_postition.IsDestroyed())
        {
            // particle_at_postition.Destroy(); //Is this the same as using Destroy method?
            Destroy(particle_at_postition);
        }

        //Stores particle GameObject in LP_matrix for future removal.
        // Debug.Log("attempting to instantiate LP at coords (" + coordinates.x + ',' + coordinates.y + ')');
        LP_matrix.SetAt(instantiated, coordinates.x, coordinates.y);
    }
    private void DeleteLPParticle(Point2D coordinates, MapCell current_MC)
    {
        // Debug.Log("attempting to delete LP at coords (" + coordinates.x + ',' + coordinates.y + ')');
        //Tells current_MC that the graphics are updated.
        current_MC.LP_particle_is_instantiated = false;
        //Removes the graphic LP.
        GameObject LPParticle = LP_matrix.FindAt(coordinates.x, coordinates.y);
        Destroy(LPParticle);
        // LP_matrix.DeleteAt(coordinates.x, coordinates.y);   //Unnecesary to delete element of the matrix since it is not accesed.
    }

    private void InstantiateItemPUParticle(Point2D coordinates, int item_PU_type, MapCell current_MC)
    {
        //Creates particle GameObject;
        Vector3 scaled_coordinates = new(coordinates.x, base_y, coordinates.y);
        scaled_coordinates *= map_scale;
        scaled_coordinates.y = base_y;

        //Stores particle GameObject in item_PU_matrix for future removal.
        // Debug.Log("attempting to instantiate ITEM/PU particle at coords (" + coordinates.x + ',' + coordinates.y + ')');
        if (item_PU_matrix.FindAt(coordinates.x, coordinates.y) == null)
        {
            Debug.Log("item_PU_matrix.FindAt(coordinates.x, coordinates.y) == null!!!!!!!!!!!!!");
        }
        switch (item_PU_type)
        {
            case 1:
                GameObject instantiated_fuel_cell = GameObject.Instantiate(item_fuel_cell, scaled_coordinates, Quaternion.identity);
                item_PU_matrix.FindAt(coordinates.x, coordinates.y).Add(instantiated_fuel_cell);
                break;
            case 2:
                GameObject instantiated_LP_size_increase = GameObject.Instantiate(item_LP_size_increase, scaled_coordinates, Quaternion.identity);
                item_PU_matrix.FindAt(coordinates.x, coordinates.y).Add(instantiated_LP_size_increase);
                break;
            case 3:
                GameObject instantiated_bomb = GameObject.Instantiate(item_bomb, scaled_coordinates, Quaternion.identity);
                item_PU_matrix.FindAt(coordinates.x, coordinates.y).Add(instantiated_bomb);
                break;
            case 4:
                GameObject instantiated_shield = GameObject.Instantiate(PU_shield, scaled_coordinates, Quaternion.identity);
                item_PU_matrix.FindAt(coordinates.x, coordinates.y).Add(instantiated_shield);
                break;
            case 5:
                GameObject instantiated_hyperspeed = GameObject.Instantiate(PU_hyperspeed, scaled_coordinates, Quaternion.identity);
                item_PU_matrix.FindAt(coordinates.x, coordinates.y).Add(instantiated_hyperspeed);
                break;
            default:
                Debug.Log("AT InstantiateItemPUParticle SHOULDN'T REACH THIS LINE IN SWITCH CASE!!!");
                break;
        }
        
        //Tells map that a particle has been instantiated.
        map.FindAt(coordinates.x, coordinates.y).item_PU_particles_instantiated ++;


        
    }
    private void DeleteItemPUParticle(Point2D coordinates, MapCell current_MC)
    {
        // Debug.Log("attempting to delete item/pu at coords (" + coordinates.x + ',' + coordinates.y + ')');
        //Tells current_MC that the graphics are updated.
        current_MC.item_PU_particles_instantiated = 0;
        //Removes all the graphic Items.
        while (item_PU_matrix.FindAt(coordinates.x, coordinates.y).Size() != 0)
        {
            GameObject item_PU_particle = item_PU_matrix.FindAt(coordinates.x, coordinates.y).FindAt(0);
            Destroy(item_PU_particle);
            item_PU_matrix.FindAt(coordinates.x, coordinates.y).DeleteAt(0);
        }
        
        // LP_matrix.DeleteAt(coordinates.x, coordinates.y);   //Unnecesary to delete element of the matrix since it is not accesed.


    }

    public Point2D GetPlayerCoords(int player_ID)
    {
        for (int i = 0; i < m; i++)
        {
            for (int j = 0; j < n; j++)
            {
                int players_on_map_cell = map.FindAt(i,j).player_IDs.Size();
                for (int k = 0; k < players_on_map_cell; k++)
                {
                    if (map.FindAt(i,j).player_IDs.FindAt(k) == player_ID)
                    {
                        Point2D player_coords = new(i,j);
                        return player_coords;
                    } 
                }
            }
        }
        // Debug.Log("Couldn't find player of ID" + player_ID + "on GetPlayerCoords().");
        return new Point2D(-1,-1);
    }
    /// <summary>
    /// x = shield_remaining; y = hyperspeed_remaining.
    /// </summary>
    public Point2D GetPlayerShieldAndHyperspeed(int player_ID)
    {
        // Debug.Log("At GetPlayerShieldAndHyperspeed player_ID = " + player_ID);
        int x = players[player_ID].shield_remaining;
        int y = players[player_ID].hyperspeed_remaining;
        return new Point2D(x,y);
    }
    public Player GetPlayer(int player_ID)
    {
        return players[player_ID];
    }
    private void BotBehavior(int current_player_ID, bool affect_player_0)
    {
        if (players[user_player].character_destroyed)
        {
            BotBehaviorRandom(current_player_ID);
            return;
        }
        if (current_player_ID == user_player & !affect_player_0)
        {
            return;
        }
        if ((!players[current_player_ID].character_destroyed & !affect_player_0) | (!players[0].character_destroyed&affect_player_0))
        {
            if (!affect_player_0)
            {
                switch (current_player_ID)
                {
                    case 0:
                    BotBehavior(user_player, true);
                    return;
                    case 1:
                    BotBehaviorFollow(1, 1, user_player);
                    return;
                    case 2:
                    if (user_player == 1)
                    {
                        BotBehaviorFollow(0, 2, user_player);    
                    }
                    else
                    {
                        if (players[1].character_destroyed)
                        {
                            BotBehaviorRandom(2);
                            return;
                        }
                        BotBehaviorFollow(1, 2, user_player);
                    }
                    return;
                    case 3:
                    BotBehaviorItems(3);
                    return;
                    case 4:
                    BotBehaviorRandom(4);
                    return;
                }    
            }
            else
            {
                switch (current_player_ID)
                {
                    case 0:
                    // BotBehavior(user_player, true);
                    return;
                    case 1:
                    BotBehaviorFollow(0, 0, user_player);
                    return;
                    case 2:
                    BotBehaviorFollow(0, 0, user_player);
                    return;
                    case 3:
                    BotBehaviorItems(0);
                    return;
                    case 4:
                    BotBehaviorRandom(0);
                    return;
                }
            }
                
        }
        
    }
    private void BotBehaviorFollow(int from_player_of_ID, int apply_on, int to_player_of_ID)
    {
        //Gets displacement necesary to reach the user from the player's (bot's) position.
        Point2D player_position = players[from_player_of_ID].trail.FindAt(0);   //This crashes if player is destroyed and thus trail has no elements.
        Point2D user_position = players[to_player_of_ID].trail.FindAt(0);   //This crashes if the user's players is destroyed (loses). SHOULD BOTBEHAVIORS BE DISABLED WHEN PLAYER LOSES?
        int x_displacement = user_position.x - player_position.x;
        int y_displacement = user_position.y - player_position.y;
        //Accounts for map cyclical movement.
        if (x_displacement > m/2)
        {
            x_displacement -= m;
        }
        if (x_displacement < -1*m/2)
        {
            x_displacement += m;
        }

        if (y_displacement > n/2)
        {
            y_displacement -= n;
        }
        if (y_displacement < -1*n/2)
        {
            y_displacement += n;
        }
        //Moves in a different direction when too close to player.
        int square_distance = (int)Math.Pow(x_displacement,2) + (int)Math.Pow(y_displacement,2);
        int square_LP_lenght_plus_one = (int)Math.Pow(players[to_player_of_ID].LP_size + 1,2);

        //Takes normalized components so that they can be used in Player.ChangeDirection().
        if (x_displacement < 0)
        {
            x_displacement = -1;
        }
        if (x_displacement > 0)
        {
            x_displacement = 1;
        }
        if (y_displacement < 0)
        {
            y_displacement = -1;
        }
        if (y_displacement > 0)
        {
            y_displacement = 1;
        }

        if (square_distance <= square_LP_lenght_plus_one)
        {
            //Perpendicular movement if close.
            players[apply_on].ChangeDirection(-1*y_displacement, x_displacement);
            return;
        }
        // Helps to avoid LP.
        if (map.FindAt((player_position.x + x_displacement + m)%m, player_position.y).LP >= 1)
        {
            x_displacement = 0;
        }
        if (map.FindAt(player_position.x, (player_position.y + y_displacement + n)%n).LP >= 1)
        {
            y_displacement = 0;
        }
        players[apply_on].ChangeDirection(x_displacement, y_displacement);
    }
    private void BotBehaviorItems(int from_player_of_ID)
    {
        //Finds nearest item.
        Point2D nearest = new(-1,-1);
        int min_square_distance = (int)Math.Pow(m, 2) + (int)Math.Pow(n, 2);

        Point2D player_position = players[from_player_of_ID].trail.FindAt(0);   //This crashes if player is destroyed and thus trail has no elements.

        for (int i = 0; i < m; i++)
        {
            for (int j = 0; j < n; j++)
            {
                if (map.FindAt(i,j).item_PU_IDs.Size() >= 1 && map.FindAt(i,j).item_PU_IDs.FindAt(0) != 3)
                //Second condition is validated only if first condition is true. Otherwise it crashes when finding an element in an empty list.
                //Second condition prevents bot from picking bombs when the bomb is in the first place of the list (most cases).
                {
                    //Gets displacement necesary to reach the nearest item/PU from the player's (bot's) position.
                    Point2D item_position = new(i,j);
                    int x_displacement = item_position.x - player_position.x;
                    int y_displacement = item_position.y - player_position.y;
                    //Accounts for map cyclical movement.
                    if (x_displacement > m/2)
                    {
                        x_displacement -= m;
                    }
                    if (x_displacement < -1*m/2)
                    {
                        x_displacement += m;
                    }

                    if (y_displacement > n/2)
                    {
                        y_displacement -= n;
                    }
                    if (y_displacement < -1*n/2)
                    {
                        y_displacement += n;
                    }
                    //Gets square distance to object. Keeps it if is the new minimum distance.
                    int square_distance = (int)Math.Pow(x_displacement,2) + (int)Math.Pow(y_displacement,2);
                    if (square_distance < min_square_distance)
                    {
                        min_square_distance = square_distance;
                        nearest = item_position;
                    }
                }
            }
        }
        if (nearest.x != -1)    // If an item was found.
        {
            int x_displacement = nearest.x - player_position.x;
            int y_displacement = nearest.y - player_position.y;
            //Accounts for map cyclical movement.
            if (x_displacement > m/2)
            {
                x_displacement -= m;
            }
            if (x_displacement < -1*m/2)
            {
                x_displacement += m;
            }

            if (y_displacement > n/2)
            {
                y_displacement -= n;
            }
            if (y_displacement < -1*n/2)
            {
                y_displacement += n;
            }

            //Takes normalized components so that they can be used in Player.ChangeDirection().
            if (x_displacement < 0)
            {
                x_displacement = -1;
            }
            if (x_displacement > 0)
            {
                x_displacement = 1;
            }
            if (y_displacement < 0)
            {
                y_displacement = -1;
            }
            if (y_displacement > 0)
            {
                y_displacement = 1;
            }

            // Helps to avoid LP.
            if (map.FindAt((player_position.x + x_displacement + m)%m, player_position.y).LP >= 1)
            {
                x_displacement = 0;
            }
            if (map.FindAt(player_position.x, (player_position.y + y_displacement + n)%n).LP >= 1)
            {
                y_displacement = 0;
            }

            players[from_player_of_ID].ChangeDirection(x_displacement, y_displacement);
        }
    }
    private void BotBehaviorRandom(int from_player_of_ID)
    {
        if (UnityEngine.Random.Range(1,6) == 1)
        {
            players[from_player_of_ID].direction += 1 + UnityEngine.Random.Range(0,2)*2;
            players[from_player_of_ID].direction %= 4;
        }
    }

    private void SpawnItemsAndPowerUps()
    {
        int random_x = UnityEngine.Random.Range(0,m);
        int random_y = UnityEngine.Random.Range(0,n);
        int random_item_PU = UnityEngine.Random.Range(1,6);
        map.FindAt(random_x, random_y).item_PU_IDs.Add(random_item_PU);
        // Debug.Log("SPAWNED AN ITEM/PU OF INDEX: " + random_item_PU + " AT (" + random_x + ',' + random_y + ')');
    }
    public void SpawnDroppedItemsAndPowerUps(LinkedList<int> items_or_PU_list)
    {
        int initial_size = items_or_PU_list.Size();
        for (int i = 0; i < initial_size; i++)
        {
            int random_x = UnityEngine.Random.Range(0,m);
            int random_y = UnityEngine.Random.Range(0,n);
            int current_item_PU = items_or_PU_list.FindAt(0);
            items_or_PU_list.DeleteAt(0);
            map.FindAt(random_x, random_y).item_PU_IDs.Add(current_item_PU); 
        }
        // Debug.Log("SPAWNED AN ITEM/PU OF INDEX: " + current_item_PU = items_or_PU_list.FindAt(0); + " AT (" + random_x + ',' + random_y + ')');
    }


    private void UpdateUI(int target_player)
    {
        Player player = players[target_player];
        LinkedList<int> items_queue = player.items_queue;
        LinkedList<int> PU_stack = player.power_ups_stack;
        if (!player.UI_is_updated)
        {
            //Updates Item Queue UI.
            //Turns on respective images.
            for (int i = 0; i < items_queue.Size(); i++)
            {
                //Turns on respective image.
                int item_ID = items_queue.FindAt(i);
                for (int j = 0; j < 3; j++)
                {
                    // Debug.Log("Enters 'Turns on respective image' with i,j = " + i + "," + j);

                    if (item_ID == j+1) //Ads 1 because items ID go from 1 to 5, not 0 to 4.
                    {
                        UI_item_list.FindAt(i).FindAt(j).GetComponent<UnityEngine.UI.Image>().enabled = true;
                    }
                    else
                    {
                        UI_item_list.FindAt(i).FindAt(j).GetComponent<UnityEngine.UI.Image>().enabled = false;
                    }
                }
            }
            //Turns off images of empty slots.
            for (int i = items_queue.Size(); i < max_items; i++)
            {
                for (int j = 0; j < 3; j++)
                {   
                    if (UI_item_list.FindAt(i) == null)
                    {
                        Debug.Log("UI_item_list.FindAt(i) == null. i = " + i);
                    }
                    if (UI_item_list.FindAt(i).FindAt(j) == null)
                    {
                        Debug.Log("UI_item_list.FindAt(i).FindAt(j) == null. i = " + i + ". j = " + j);
                    }
                    UI_item_list.FindAt(i).FindAt(j).GetComponent<UnityEngine.UI.Image>().enabled = false;
                }
            }


            //Updates PU Stack UI.
            //Turns on respective images.
            for (int i = 0; i < PU_stack.Size(); i++)
            {
                //Turns on respective image.
                int PU_ID = PU_stack.FindAt(i);
                for (int j = 0; j < 2; j++)     //  SUBTRACT NUMBER OF ITEMS???????
                {
                    // Debug.Log("Enters 'Turns on respective image' with i,j = " + i + "," + j);

                    if (PU_ID == j + 3 + 1) //Ads 1 because items ID go from 1 to 5, not 0 to 4.
                    {
                        UI_PU_list.FindAt(i).FindAt(j).GetComponent<UnityEngine.UI.Image>().enabled = true;
                    }
                    else
                    {
                        UI_PU_list.FindAt(i).FindAt(j).GetComponent<UnityEngine.UI.Image>().enabled = false;
                    }
                }
            }
            //Turns off images of empty slots.
            for (int i = PU_stack.Size(); i < max_PU; i++)
            {
                for (int j = 0; j < 2; j++)
                {   
                    if (UI_PU_list.FindAt(i) == null)
                    {
                        Debug.Log("UI_PU_list.FindAt(i) == null. i = " + i);
                    }
                    if (UI_PU_list.FindAt(i).FindAt(j) == null)
                    {
                        Debug.Log("UI_PU_list.FindAt(i).FindAt(j) == null. i = " + i + ". j = " + j);
                    }
                    UI_PU_list.FindAt(i).FindAt(j).GetComponent<UnityEngine.UI.Image>().enabled = false;
                }
            }


            player.UI_is_updated = true;
        }
        fuel_textbox_add_fuel_timer --; // Any better place to put this? Must run once per frame.

    }
    private void UpdateUIFuel()
    {
        //Updates fuel_textbox.
        // if (fuel_textbox.GetComponent<TextMeshProUGUI>() == null)
        // {
        //     Debug.Log("fuel_textbox.GetComponent<TextMeshProUGUI>() == null!!!!!!!!!");
        // }
        Player current_player = players[user_player];
        if (current_player.character_destroyed)
        {
            fuel_textbox.GetComponent<TextMeshProUGUI>().text = "";
            return;    
        }
        string extra_fuel_msg = "";
        if (fuel_textbox_add_fuel_timer > 0)
        {
            extra_fuel_msg = " + " + current_player.last_fuel_increase.ToString();
        }
        fuel_textbox.GetComponent<TextMeshProUGUI>().text = ((int)current_player.fuel).ToString() + extra_fuel_msg;
    }

    private void PrintLPMatrix()
    {
        // Print LP_matrix
        if (Time.frameCount % 180 == 0)
        {
            string msg = "";
            for (int i = 0; i < m; i++)
            {
                msg = string.Concat(msg, "[");
                for (int j = 0; j < n; j++)
                {
                    msg = string.Concat(msg, " ");
                    msg = string.Concat(msg, map.FindAt(i,j).LP);
                    // Debug.Log(map.FindAt(i,j).LP);
                }
                msg = string.Concat(msg, "]\n");
            }
            Debug.Log(msg);
        }
    }
    private void PrintLPMatrixOnce()
    {
        // Print LP_matrix
            string msg = "";
            for (int i = 0; i < m; i++)
            {
                msg = string.Concat(msg, "[");
                for (int j = 0; j < n; j++)
                {
                    msg = string.Concat(msg, " ");
                    msg = string.Concat(msg, map.FindAt(i,j).LP);
                    // Debug.Log(map.FindAt(i,j).LP);
                }
                msg = string.Concat(msg, "]\n");
            }
            Debug.Log(msg);
    }
    private void PrintItemPUMatrix(bool no_timer)
    {
        // Print PrintItemPUMatrix
        if (Time.frameCount % 240 == 0 | no_timer)
        {
            string msg = "";
            for (int i = 0; i < m; i++)
            {
                msg = string.Concat(msg, "[");
                for (int j = 0; j < n; j++)
                {
                    msg = string.Concat(msg, " ");
                    msg = string.Concat(msg, map.FindAt(i,j).item_PU_IDs.Size());
                }
                msg = string.Concat(msg, "]\n");
            }
            Debug.Log(msg);
        }
    }


    void Start()
    {
        // Draws tiles;
        for (int i = 0; i < m; i++)
        {
            for (int j = 0; j < n; j++)
            {
                Vector3 tile_coords = new(i, base_y, j);
                tile_coords *= map_scale;
                GameObject current_tile = Instantiate(tile, tile_coords, Quaternion.identity);
                current_tile.transform.Rotate(90,0,0);
            }
        }
        // Instantiates the BackGround.
        Vector3 scene_center = new(m/2*map_scale, base_y - 5, n/2*map_scale);
        GameObject bg = Instantiate(floor, scene_center, Quaternion.identity);
        Vector3 resize = new(m*map_scale*3/2, 1, n*map_scale*3/2);
        bg.transform.localScale = resize;
        // Repositions the main camera.
        scene_center.y = n*10;
        main_camera.transform.position = scene_center;

        //Manually destroy players. Debugging.
        // players[0].Delete();
        // players[1].Delete();
        // players[2].Delete();
        // players[3].Delete();
        // players[4].Delete();
    }    

    void Update()
    {
        Controls();
        UpdatePlayers();
        UpdateUI(user_player);
        // PrintLPMatrix();

        // Spawn item/PU.
        if (spawn_timer < Time.frameCount) // & UnityEngine.Random.Range(0,60) == 1 //Code to add random extra time between spawns.
        {
            SpawnItemsAndPowerUps();
            spawn_timer += base_spawn_time / m / n;
            // PrintItemPUMatrix(true);
        }        

        //Time. Debug.
        // Debug.Log($"Total frames: {Time.frameCount}" + $"spawn_timer: {spawn_timer}" + $"base_spawn_time / m / n: {base_spawn_time / m / n}");
        // if (Time.frameCount % 60 == 0)
        // {
        //     Debug.Log("60f.");
        // }
    }

    public void Controls()
    {
        //Grants user control of player 0.
        //Controls direction.
        
        //Makes quick presses linger until player moves. (1)
        if (players[user_player].has_moved)
        {
            current_horizontal_key = 0;
            current_vertical_key = 0;
        }

        int verticalKey = current_vertical_key;
        if (Input.GetKey(KeyCode.W) | Input.GetKey(KeyCode.UpArrow))
        {
            verticalKey = 1;
        }
        if (Input.GetKey(KeyCode.S) | Input.GetKey(KeyCode.DownArrow))
        {
            verticalKey = -1;
        }
        int horizontalKey = current_horizontal_key;
        if (Input.GetKey(KeyCode.D) | Input.GetKey(KeyCode.RightArrow))
        {
            horizontalKey = 1;
        }
        if (Input.GetKey(KeyCode.A) | Input.GetKey(KeyCode.LeftArrow))
        {
            horizontalKey = -1;
        }

        //Makes quick presses linger until player moves. (2)
        if (verticalKey != 0)
        {
            current_vertical_key = verticalKey;
        }
        if (horizontalKey != 0)
        {
            current_horizontal_key = horizontalKey;
        }
        
        if (verticalKey != 0 | horizontalKey != 0)
        {
            // Debug.Log("Enters ChangeDirection with (x,y) = " + horizontalKey + ',' + verticalKey);
            players[user_player].ChangeDirection(horizontalKey, verticalKey);
            players[user_player].has_moved = false;
        }

        //Uses PU when user presses space key.
        if (Input.GetKeyDown(KeyCode.Space))
        {
            players[user_player].UsePowerUp();
            Debug.Log("User used PU. Space key pressed.");
        }

        //Rotates PU with Q and E keys.
        if (Input.GetKeyDown(KeyCode.Q) | Input.GetKeyDown(KeyCode.Alpha1))
        {
            players[user_player].RotatePULeft();
            Debug.Log("RotatePowerPULeft()");
        }
        if (Input.GetKeyDown(KeyCode.E) | Input.GetKeyDown(KeyCode.Alpha2))
        {
            players[user_player].RotatePURight();
            Debug.Log("RotatePowerPURight()");
        }

    }

}

