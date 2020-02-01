using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    public static PlayerSpawner instance;

    public GameManager gameManager;
    public int playerCount { get; private set; }
    public Transform[] spawnPoint;

    public Transform notActiveSpawnPoint;

    // WIP: Add player ref array here

    // Start is called before the first frame update
    void Start()
    {
        PlayerSpawner.instance = this;
        playerCount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void ReportPlayerSpawn(GameObject player)
    {
        if(instance)
        {
            instance.ReportPlayerSpawn_internal(player);
        }
    }

    public static void StartGame()
    {
        if(instance)
        {
            instance.StartGame_internal();
        }
    }

    private void ReportPlayerSpawn_internal(GameObject player)
    {
        if(this.gameManager.currentGameState == GameState.WaitingForPlayer)
        {
            player.transform.position = spawnPoint[playerCount].transform.position;
            playerCount++;

            //  WIP: add player ref to array
        }
        else
        {
            player.transform.position = notActiveSpawnPoint.position;
        }   
    }

    private void StartGame_internal()
    {
        //  WIP: Set Player to allow action
        this.gameManager.playerCount = this.playerCount;
        this.gameManager.StartGame();
    }
}
