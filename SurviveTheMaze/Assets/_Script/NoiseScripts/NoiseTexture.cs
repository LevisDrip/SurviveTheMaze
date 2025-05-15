using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer.Internal;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class NoiseTexture : MonoBehaviour
{
    // Width and height of the texture in pixels.
    public Vector2 Scale;
    private float[] NoisePositions;

    // The number of cycles of the basic noise pattern that are repeated
    // over the width and height of the texture.
    public float NoiseScale = 1.0F;

    public float TileSize;

    #region MapObjects

        [Header("MapObjects")]
        public GameObject Wall;
        public GameObject Floor;

        public GameObject SpawnPoint;
        public GameObject ExitPoint;

    #endregion

    #region MapRadius & SpawnLimits
        
        [Header("MapRadius & SpawnLimits")]        
        public float SpawnSafeRadius;
        [Range(0, 100)]
        public int BoxLimit;
        [Range(0, 100)]
        public int EnemyLimit;
        [Range(0.0f, 1.0f)]
        public float OptionLimit;
        [Range(0.0f, 1.0f)]
        public float WallChance;

    #endregion

    Vector3 StartPos;
    private NavMeshPath path;

    #region Player

        [Header("Player")]
        public GameObject Player;
        public PlayerController CurrentPlayer;

    #endregion

    public GameObject Box;
    public GameObject MandM;

    public Transform MapContainer;

    private List<Vector3> Options = new List<Vector3>();

    private void Awake()
    {
        CurrentPlayer = FindFirstObjectByType<PlayerController>();

        path = new NavMeshPath();

        NoisePositions = new float[(int)(Scale.x * Scale.y)];
        Options.Clear();

        GenerateMap();
        if(Options.Count < 10)
        {
            print("Map failed");
            RegenerateMap();
        }
        PlaceSpawnPoint();
        PlaceExitPoint();
        EntitySpawning();
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.L))
        {
            SceneManager.LoadScene(0);
        }

        if (Input.GetKey(KeyCode.R))
        {
            RegenerateMap();
        }

        
    }


    void GenerateMap()
    {
        float Seed = Random.Range(0, 9999);

        int currentPosition = 0;

        // For each pixel in the texture...
        for (float y = 0.0F; y < Scale.y; y++)
        {
            for (float x = 0.0F; x < Scale.x; x++)
            {
                float xCoord = Seed + x / Scale.x * NoiseScale;
                float yCoord = Seed + y / Scale.y * NoiseScale;
                float Point = Mathf.PerlinNoise(xCoord, yCoord);

                NoisePositions[currentPosition] = Point;
                currentPosition++;

                Vector3 position = new Vector3(x * TileSize, 0, y * TileSize);

                if (Point > WallChance)
                {
                    Instantiate(Floor, position, transform.rotation, MapContainer.transform);
                    
                    if (Point > OptionLimit && position.x > 1 && position.z > 1 && position.x < 88 && position.z < 88)
                    {
                        Options.Add(position);
                    }
                }
                else if(Point <= WallChance)
                {
                    Instantiate(Wall, position, transform.rotation, MapContainer);
                }

                if(x == 0 || x == Scale.y - 1 || y == 0 || y == Scale.x - 1)
                {
                    Instantiate(Wall, position, transform.rotation, MapContainer);
                }

                
            }
        }
    }


    #region MapPlacingLogic

        void PlaceSpawnPoint()
        {
            Vector3 spawnLocation = Options[Random.Range(0, Options.Count - 1)];
            GameObject SpawnInstance = Instantiate(SpawnPoint, spawnLocation + new Vector3(0, 0.5f, 0), transform.rotation, MapContainer);
            StartPos = SpawnInstance.transform.position;

            if (!CurrentPlayer)
            {
                CurrentPlayer = Instantiate(Player, SpawnInstance.transform.position + new Vector3(0, 3, 0), SpawnInstance.transform.rotation).GetComponent<PlayerController>();
                CurrentPlayer.DisableLoadScreen();
            }
            else
            {
                CurrentPlayer.gameObject.SetActive(false);
                CurrentPlayer.transform.position = SpawnInstance.transform.position + new Vector3(0, 3, 0);
                CurrentPlayer.gameObject.SetActive(true);
                CurrentPlayer.DisableLoadScreen();
                CurrentPlayer.ReloadItem();
            }

            // Store the spawn location for exit placement logic
            selectedSpawnLocation = spawnLocation;
            Options.Remove(spawnLocation);
        }

        // Store the selected spawn position globally
        private Vector3 selectedSpawnLocation = Vector3.zero;


        void PlaceExitPoint()
        {
            bool ExitFound = false;
            int FailCounter = 0;

            while(!ExitFound)
            {
                //bool FoundExitSpot = false;
                Vector3 ExitSpot = Vector3.zero;
                float LastDis = 0;

                foreach (Vector3 Point in Options)
                {
                    float Dis = Vector3.Distance(Point, StartPos);
                    if (Dis > LastDis)
                    {
                        ExitSpot = Point;
                        LastDis = Dis;
                    }
                }

                if (ExitSpot != Vector3.zero)
                {
                    path = new NavMeshPath();
                    NavMesh.CalculatePath(StartPos, ExitSpot, NavMesh.AllAreas, path);

                    Vector3 One = ExitSpot; One.y = 0;
                    Vector3 Two = path.corners[path.corners.Length - 1]; Two.y = 0;

                    if (One == Two)
                    {
                        Instantiate(ExitPoint, ExitSpot, transform.rotation, MapContainer);
                        Options.Remove(ExitSpot);
                        ExitFound = true;
                    }
                }

                FailCounter++;

                if(FailCounter > 100)
                {
                    print("Not Good");
                    RegenerateMap();
                }
            }
        }

        void EntitySpawning()
        {
            foreach(Vector3 Point in Options)
            {
                if(Vector3.Distance(Point, StartPos) > SpawnSafeRadius)
                {
                    int R = Random.Range(0, 200);

                    if(R > 100 && R < EnemyLimit + 100)
                    {
                        Instantiate(MandM, Point + new Vector3(0, 1, 0), transform.rotation, MapContainer);
                    }

                    if(R < BoxLimit)
                    {
                        Instantiate(Box, Point + new Vector3(0, 1, 0), transform.rotation, MapContainer);
                    }
                }
            }
        }

    #endregion


    public void RegenerateMap()
    {
        SceneManager.LoadScene(0);
    } 
}
