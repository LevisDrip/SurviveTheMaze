using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int EnemiesKilled = 0;
    public int Objective;

    public Transform CamHolder;

    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Update()
    {
        if (!CamHolder && FindFirstObjectByType<PlayerController>())
        {
            CamHolder = FindFirstObjectByType<PlayerController>().CamHolder;
        }

        
    }

    public void ObjectiveReached()
    {
        if(EnemiesKilled > Objective)
        {

        }
    }
}
