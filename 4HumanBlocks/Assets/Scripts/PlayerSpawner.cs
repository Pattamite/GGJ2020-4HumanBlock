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

    private List<PlayerController> playerControllerList;

    // Start is called before the first frame update
    void Start()
    {
        PlayerSpawner.instance = this;
        playerCount = 0;
        playerControllerList = new List<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void ReportPlayerSpawn(GameObject player, PlayerController playerController)
    {
        if(instance)
        {
            instance.ReportPlayerSpawn_internal(player, playerController);
        }
    }

    public static void StartGame()
    {
        if(instance)
        {
            instance.StartGame_internal();
        }
    }

    private void ReportPlayerSpawn_internal(GameObject player, PlayerController playerController)
    {
        if(this.gameManager.currentGameState == GameState.WaitingForPlayer)
        {
            playerController.SetPosition( spawnPoint[playerCount].transform.position );
            
            playerController.id = playerCount;
            playerControllerList.Add(playerController);
            playerCount++;
        }
        else
        {
            player.transform.position = notActiveSpawnPoint.position;
        }   
    }

    private void StartGame_internal()
    {
        if (this.gameManager.currentGameState != GameState.WaitingForPlayer)
            return;

        foreach (PlayerController playerController in playerControllerList)
        {
            print(playerController);
            playerController.isAllowAction = true;
        }
        this.gameManager.playerCount = this.playerCount;
        this.gameManager.StartGame();
    }
}
