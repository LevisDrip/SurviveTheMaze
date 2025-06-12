using System.Collections;
using System.Collections.Generic;
using NUnit;
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

    bool CompleteFinish;
    bool MapFinish;
    bool EntityFinish;

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
    public float EnemyLimit;
    [Range(0.0f, 1.0f)]
    public float OptionLimit;
    [Range(0.0f, 1.0f)]
    public float WallChance;


    public int EnemyCap;

    #endregion

    Vector3 StartPos;
    Vector3 EndPos;
    int FailCounter;
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
        StartPos = Vector3.zero;
        EndPos = Vector3.zero;

        CurrentPlayer = FindFirstObjectByType<PlayerController>();

        path = new NavMeshPath();

        NoisePositions = new float[(int)(Scale.x * Scale.y)];
        Options.Clear();

        //EntitySpawning();
    }

    private void Update()
    {
        if(!MapFinish)
        {
            GenerateMap();

            if(Options.Count <= 10)
            {
                print("Map failed");
                RegenerateMap();
            }
            else
            {
                MapFinish = true;
            }
        }
        else if(!CompleteFinish)
        {
            FindSpawnPoint();

            if (EndPos != Vector3.zero)
            {
                CompleteFinish = true;
            }
        }
        else if(!EntityFinish)
        {
            EntitySpawning();
            
            if (!CurrentPlayer)
            {
                CurrentPlayer = Instantiate(Player, StartPos + new Vector3(0, 1, 0), transform.rotation).GetComponent<PlayerController>();
            }
            else
            {
                CurrentPlayer.transform.position = StartPos + new Vector3(0, 1, 0);
                CurrentPlayer.DisableLoadScreen();
            }

            EntityFinish = true;
        }

        if (Input.GetKey(KeyCode.L))
        {
            SceneManager.LoadScene(0);
        }

        if (Input.GetKey(KeyCode.R))
        {
            RegenerateMap();
        }

        if (path.corners.Length <= 0)
        {
            return;
        }
        for (int i = 0; i < path.corners.Length - 1; i++)
        {
            Debug.DrawLine(path.corners[i] + new Vector3(0, 10, 0), path.corners[i + 1] + new Vector3(0, 10, 0), Color.red);
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
                else if (Point <= WallChance)
                {
                    Instantiate(Wall, position, transform.rotation, MapContainer);
                }

                if (x == 0 || x == Scale.y - 1 || y == 0 || y == Scale.x - 1)
                {
                    Instantiate(Wall, position, transform.rotation, MapContainer);
                }


            }
        }
    }


    #region MapPlacingLogic

    void FindSpawnPoint()
    {
        if(FailCounter > 100)
        {
            RegenerateMap();
            return;
        }

        if (StartPos == Vector3.zero)
        {
            StartPos = Instantiate(SpawnPoint, Options[Random.Range(0, Options.Count)] + new Vector3(0, 0.5f, 0), transform.rotation).transform.position + new Vector3(0, 1, 0);
        }
        else if (EndPos == Vector3.zero)
        {
            Vector3 FarOption = GetFarOption();

            path = new NavMeshPath();
            NavMesh.CalculatePath(StartPos, FarOption, NavMesh.AllAreas, path);

            Vector3 TestPoint = path.corners[path.corners.Length - 1];
            TestPoint.y = 0;
            Vector3 TestPoint2 = FarOption; TestPoint2.y = 0;
            if (TestPoint == TestPoint2)
            {
                EndPos = Instantiate(ExitPoint, FarOption, transform.rotation).transform.position + new Vector3(0, 1, 0);
            }
            else
            {
                Options.Remove(FarOption);
                print("DidStep");

                FailCounter++;
            }
        }
        else
        {
            Options.Clear();
        }
    }

    void EntitySpawning()
    {
        int EnemyCount = 0;

        foreach (Vector3 Point in Options)
        {
            if (Vector3.Distance(Point, StartPos) > SpawnSafeRadius)
            {
                int R = Random.Range(0, 200);

                if (R > 100 && R < EnemyLimit + 100 && EnemyCount < EnemyCap)
                {
                    Instantiate(MandM, Point + new Vector3(0, 1, 0), transform.rotation, MapContainer);
                    EnemyCount++;
                }

                if (R < BoxLimit)
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

    Vector3 GetFarOption()
    {
        Vector3 ReturnValue = StartPos;
        float LastDis = 0;
        foreach (Vector3 Option in Options)
        {
            float Dis = Vector3.Distance(StartPos, Option);
            if (Dis > LastDis)
            {
                ReturnValue = Option;
                LastDis = Dis;
            }
        }

        return ReturnValue;
    }
}
