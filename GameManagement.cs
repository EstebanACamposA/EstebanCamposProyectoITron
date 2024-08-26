using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DataStructures;
using System.Drawing;
using DataStructures.Lists;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;

// using myUnityScripts;

public class GameManagement : MonoBehaviour
{
    public static GameManagement Instance { get; private set; }

    public int playerCount = 6;
    public int[] turns;
    public Player[] players;
    public static int m = 12;
    public static int n = 15;
    public DataStructures.Lists.MatrixLinkedList<MapCell> map;
    // Graphics related variables.
    public GameObject light_path_particle;
    // public GameObject item_fuel_cell;
    // public GameObject item_LP_size_increase;
    // public GameObject item_bomb;
    // public GameObject PU_bomb;
    // public GameObject PU_hiperspeed;

    public GameObject tile;
    
    public int map_scale = 2;
    public int base_y = 0;
    private DataStructures.Lists.MatrixLinkedList<GameObject> LP_matrix;

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

        // Creates players.
        for (int i = 0; i < playerCount; i++)
        {
            turns[i] = i*60;   //  ----!!!!!!!!!!!!THIS NUMBER SHOULD BE 15, NOT 1500000. MEANT TO MAKE ALL OTHER PLAYERS WAIT FOR TESTING!!!!!!!!!!!!----
            int x_of_new_player = n/playerCount*i;
            int y_of_new_player = m/2 + 3;
            Point2D coords_of_new_player = new(x_of_new_player, y_of_new_player);
            players[i] = new Player(i, coords_of_new_player, 1, map);   //  ----!!!!!!!!!!!!speed SHOULD BE Random.Range(1,11), NOT 3!!!!!!!!!!!!----
        }


        // Creates graphic maps
        LP_matrix = new(m,n);


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
                // Debug.Log("At UpdatePlayers i = " + i + "and turns[i] = " + turns[i]);
                // Debug.Log("At UpdatePlayers Time.frameCount = " + Time.frameCount);
                players[i].Update();
                turns[i] += 1200/players[i].speed;
                UpdateMap();
            }
            players[i].UpdatePowerUps();
        }
    }

    private void UpdateMap()
    {
        int IPROBE = 0;
        for (int i = 0; i < m; i++)
        {
            IPROBE = i;
            for (int j = 0; j < n; j++)
            {
                MapCell current_MC = map.FindAt(i,j);
                //Checks for players collision.



                // Debug.Log("current_MC (i,j) = ("+i+','+j+')');
                // if (current_MC == null)
                // {
                //     Debug.Log("current_MC == null!!!");
                // }
                // if (current_MC.player_IDs == null)
                // {
                //     Debug.Log("current_MC.player_IDs == null!!!");
                // }
                // Debug.Log("current_MC.player_IDs.Size() = " + current_MC.player_IDs.Size());
                // if (current_MC.player_IDs.Size() > 1)
                // {
                //     Debug.Log("PLAYERS COLLISION DETECTED. NEED CODE TO DESTROY PLAYERS. NEED TO REMOVE THE PLAYERS FROM player_IDs.");
                // }
                //Checks for player collision with LP or items.




                if (current_MC.player_IDs.Size() == 1)
                {
                    //Checks for player collision with LP.
                    if (current_MC.LP >= 1)
                    {
                        Debug.Log("PLAYER COLLISION WITH LP DETECTED. NEED CODE TO DESTROY THE PLAYER. NEED TO REMOVE THE PLAYER FROM player_IDs.");
                    }
                    //Checks for player collision with items.
                    int item_PU_IDs_size = current_MC.item_PU_IDs.Size();
                    if (item_PU_IDs_size >= 1)
                    {
                        for (int k = 0; k < item_PU_IDs_size; k++)
                        {
                            //Finds a player in current MapCell and gives all its items to it.
                            players[current_MC.player_IDs.FindAt(0)].GiveItemOrPU(current_MC.item_PU_IDs.FindAt(k));
                        }
                    }
                }
                //Check for new LP in current_MC to instantiate its particle effect GameObject.
                //If a new LP particle needs to be instantiated.
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
            }

        }
        Debug.Log("Exits UpdateMap with IPROBE = " + IPROBE);
    } 

    private void InstantiateLPParticle(int amount, Point2D coordinates, int direction, MapCell current_MC)
    {
        //Creates particle GameObject;
        Vector3 scaled_coordinates = new(coordinates.x, base_y, coordinates.y);
        scaled_coordinates *= map_scale;
        GameObject instantiated = GameObject.Instantiate(light_path_particle, scaled_coordinates, Quaternion.identity);

        //Tells map that particle has been instantiated.
        map.FindAt(coordinates.x, coordinates.y).LP_particle_is_instantiated = true;
        //Rotates particle.
        instantiated.GetComponent<ParticleSystem>().transform.Rotate(0, 90*direction, 0);
        //Stores particle GameObject in LP_matrix for future removal.
        Debug.Log("attempting to instantiate LP at coords (" + coordinates.x + ',' + coordinates.y + ')');
        LP_matrix.SetAt(instantiated, coordinates.x, coordinates.y);
    }
    private void DeleteLPParticle(Point2D coordinates, MapCell current_MC)
    {
        Debug.Log("attempting to delete LP at coords (" + coordinates.x + ',' + coordinates.y + ')');
        //Tells current_MC that the graphics are updated.
        current_MC.LP_particle_is_instantiated = false;
        //Removes the graphic LP.
        Debug.Log("LP_matrix.first_row.Size() = " + LP_matrix.first_row.Size());
        Debug.Log("LP_matrix.first_row.FindAt(6).Size() = " + LP_matrix.first_row.FindAt(6).Size());
        GameObject LPParticle = LP_matrix.FindAt(coordinates.x, coordinates.y);
        Destroy(LPParticle);
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
        Debug.Log("Couldn't find player of ID" + player_ID + "on GetPlayerCoords().");
        return new Point2D(-1,-1);
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
    }    

    void Update()
    {
        // Debug.Log("HAS ENTERED AWAKE FUNCTION IN THE GAME MANAGER.");
        // Debug.Log(Time.frameCount);
        UpdatePlayers();

    }

}

