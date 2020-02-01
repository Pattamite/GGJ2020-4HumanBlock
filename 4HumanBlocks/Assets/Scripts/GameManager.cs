using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    WaitingForPlayer,
    GameStart,
    GameFinish
}
public class GameManager : MonoBehaviour
{

    public GameObject[] zone0BlockArray;
    public GameObject[] zone1BlockArray;
    public GameObject[] zone2BlockArray;
    public GameObject[] zone3BlockArray;

    public Transform[] blockSpawnPointArray;

    public Transform[] blockSubmitPointArray;

    public float blockSpawnRadius;

    public int playerCount;

    public string currentBlockSetName { get; private set; }
    public GameState currentGameState { get; private set; }

    private Dictionary<string, GameObject>[] blockDictionary;
    private string[] blockSetNameArray;
    private string[] remainingBlockSetNameArray;

    private int allBlockSetCount;
    private int currentCompleteSet;

    private float startTime;
    private float endTime;


    // Start is called before the first frame update
    void Start()
    {
        this.blockDictionary = new Dictionary<string, GameObject>[4];

        this.PopolateBlockSetNameArray(this.blockSetNameArray, this.zone0BlockArray);
        this.PopolateBlockSetNameArray(this.remainingBlockSetNameArray, this.zone0BlockArray);

        this.SpawnBlockAtSpawnPoint(this.zone0BlockArray, blockSpawnPointArray[0], this.blockDictionary[0]);
        this.SpawnBlockAtSpawnPoint(this.zone0BlockArray, blockSpawnPointArray[1], this.blockDictionary[1]);
        this.SpawnBlockAtSpawnPoint(this.zone0BlockArray, blockSpawnPointArray[2], this.blockDictionary[2]);
        this.SpawnBlockAtSpawnPoint(this.zone0BlockArray, blockSpawnPointArray[3], this.blockDictionary[3]);

        this.allBlockSetCount = this.blockDictionary[0].Count;
        this.currentCompleteSet = 0;

        this.currentGameState = GameState.WaitingForPlayer;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void PopolateBlockSetNameArray(string[] blockSetNameArray, GameObject[] blockArray)
    {
        blockSetNameArray = new string[blockArray.Length];

        for (int i = 0; i < blockArray.Length; i++)
        {
            blockSetNameArray[i] = "test";
        }
    }

    public void StartGame()
    {
        this.GenerateNewBlockSetCommand();
        this.currentGameState = GameState.GameStart;
        this.startTime = Time.time;
        Debug.Log("Game Start!");
    }

    public void StopGame()
    {
        this.currentGameState = GameState.GameFinish;
        this.endTime = Time.time;
        Debug.Log("Game Finish! Total Time: " + (this.startTime - this.endTime));
    }

    public void BlockSetComplete()
    {
        
        int originalIndex = -1;

        for(int i = 0; i < this.remainingBlockSetNameArray.Length; i++)
        {
            if(this.currentBlockSetName == this.remainingBlockSetNameArray[i])
            {
                originalIndex = i;
                break;
            }
        }

        if(originalIndex != -1)
        {
            var foos = new List<string>(this.remainingBlockSetNameArray);
            foos.RemoveAt(originalIndex);
            this.remainingBlockSetNameArray = foos.ToArray();
        }
        else
        {
            Debug.LogError("GameManager::BlockSetComplete " +
                               this.currentBlockSetName + 
                               " not found in remainingBlockSetNameArray");
            return;
        }

        this.currentCompleteSet++;
        if (this.currentCompleteSet == this.allBlockSetCount)
        {
            this.StopGame();
        }
        else
        {
            this.GenerateNewBlockSetCommand();
        }
    }

    private void GenerateNewBlockSetCommand()
    {
        this.currentBlockSetName = this.RandomCurrentBlockSetName();

        for(int i = 3; i >= this.playerCount; i--)
        {
            this.SetBlockToSubmitPosition(this.currentBlockSetName, i);
        }

        //  WIP: notify block detector
    }

    private string RandomCurrentBlockSetName()
    {
        int randomIndex = UnityEngine.Random.Range(0, this.remainingBlockSetNameArray.Length);

        return this.remainingBlockSetNameArray[randomIndex];
    }

    private void SpawnBlockAtSpawnPoint( GameObject[] blockArray, Transform spawnPointTransform, Dictionary<string, GameObject> blockDictionary)
    {
        foreach(GameObject block in blockArray)
        {
            float randomSpawnPointRadius = UnityEngine.Random.Range(0, this.blockSpawnRadius);
            float randomSpawnAngle = UnityEngine.Random.Range(0, 2 * Mathf.PI);
            Vector3 spawnPointOffset = new Vector3(Mathf.Sin(randomSpawnAngle), 0, Mathf.Cos(randomSpawnAngle)) *
                                            randomSpawnPointRadius;
            Vector3 spawnPoint = spawnPointTransform.position + spawnPointOffset;

            GameObject newBlock = Instantiate(block, spawnPoint, Quaternion.identity) as GameObject;

            blockDictionary.Add("test", newBlock);
        }
    }
    private void SetBlockToSubmitPosition( string blockName, int playerIndex )
    {
        GameObject block = this.blockDictionary[playerIndex][blockName];

        block.transform.position = blockSubmitPointArray[playerIndex].transform.position;
        block.transform.eulerAngles = Vector3.zero;
    }

    public float GetGameTime()
    {
        if (this.currentGameState == GameState.WaitingForPlayer)
            return 0;
        else if (this.currentGameState == GameState.GameStart)
            return Time.time - this.startTime;
        else if (this.currentGameState == GameState.GameFinish)
            return this.endTime - this.startTime;

        return 0;
    }
}
