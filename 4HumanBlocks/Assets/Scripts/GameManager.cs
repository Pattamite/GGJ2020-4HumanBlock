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
    public BlockDetector blockDetector;
    public MusicPlayer musicPlayer;
    public SfxPlayer sfxPlayer;

    public GameObject[] zone0BlockArray;
    public GameObject[] zone1BlockArray;
    public GameObject[] zone2BlockArray;
    public GameObject[] zone3BlockArray;

    public Transform[] blockSpawnPointArray;

    public Transform[] blockSubmitPointArray;

    public float blockSpawnRadius;

    public int playerCount;

    public GameUiManager uiManager;

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
        BlockDetector.OnRepairsuccess += this.BlockSetComplete;
        this.blockDictionary = new Dictionary<string, GameObject>[4];
        this.blockDictionary[0] = new Dictionary<string, GameObject>();
        this.blockDictionary[1] = new Dictionary<string, GameObject>();
        this.blockDictionary[2] = new Dictionary<string, GameObject>();
        this.blockDictionary[3] = new Dictionary<string, GameObject>();

        this.blockSetNameArray = new string[zone0BlockArray.Length];
        this.remainingBlockSetNameArray = new string[zone0BlockArray.Length];

        this.PopolateBlockSetNameArray(this.blockSetNameArray, this.zone0BlockArray);
        this.PopolateBlockSetNameArray(this.remainingBlockSetNameArray, this.zone0BlockArray);
        print(remainingBlockSetNameArray[0]);

        this.currentGameState = GameState.WaitingForPlayer;
        this.uiManager.UpdateGameStatusBoard(this.currentGameState);
        this.musicPlayer.PlayMusic(MusicItem.Game_Waiting);

        this.SpawnBlockAtSpawnPoint(this.zone0BlockArray, this.blockSpawnPointArray[0], this.blockDictionary[0]);
        this.SpawnBlockAtSpawnPoint(this.zone1BlockArray, this.blockSpawnPointArray[1], this.blockDictionary[1]);
        this.SpawnBlockAtSpawnPoint(this.zone2BlockArray, this.blockSpawnPointArray[2], this.blockDictionary[2]);
        this.SpawnBlockAtSpawnPoint(this.zone3BlockArray, this.blockSpawnPointArray[3], this.blockDictionary[3]);

        this.allBlockSetCount = this.blockDictionary[0].Count;
        this.currentCompleteSet = 0;
    }

    // Update is called once per frame
    void Update()
    {
        this.uiManager.UpdateTimterText( this.GetGameTime() );
    }

    private void OnDestroy()
    {
        BlockDetector.OnRepairsuccess -= this.BlockSetComplete;
    }

    private void PopolateBlockSetNameArray(string[] blockSetNameArray, GameObject[] blockArray)
    {
        for (int i = 0; i < blockArray.Length; i++)
        {
            Block newBlock = blockArray[i].GetComponent<Block>();

            blockSetNameArray[i] = newBlock.getBlockName();
        }
    }

    public void StartGame()
    {
        this.GenerateNewBlockSetCommand();
        this.currentGameState = GameState.GameStart;
        this.uiManager.UpdateGameStatusBoard(this.currentGameState);
        this.musicPlayer.PlayMusic(MusicItem.Game_Playing);
        this.sfxPlayer.PlaySfxClip(SfxItem.Game_Start);
        this.startTime = Time.time;
        Debug.Log("Game Start!");
    }

    public void StopGame()
    {
        this.currentGameState = GameState.GameFinish;
        this.uiManager.UpdateGameStatusBoard(this.currentGameState);
        this.musicPlayer.PlayMusic(MusicItem.Game_Result);
        this.sfxPlayer.PlaySfxClip(SfxItem.Game_TimeOut);
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

        this.blockDetector.SetExpectedBlockName(this.currentBlockSetName);

        //this.uiManager.UpdateBlockCommandImage(sprite);
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
            if (!block)
                continue;

            float randomSpawnPointRadius = UnityEngine.Random.Range(0, this.blockSpawnRadius);
            float randomSpawnAngle = UnityEngine.Random.Range(0, 2 * Mathf.PI);
            Vector3 spawnPointOffset = new Vector3(Mathf.Sin(randomSpawnAngle), 0, Mathf.Cos(randomSpawnAngle)) *
                                            randomSpawnPointRadius;
            Vector3 spawnPoint = spawnPointTransform.position + spawnPointOffset;

            GameObject newBlockObject = Instantiate(block, spawnPoint, Quaternion.identity) as GameObject;
            Block newBlock = newBlockObject.GetComponent<Block>();

            blockDictionary.Add(newBlock.getBlockName(), newBlockObject);
        }
    }
    private void SetBlockToSubmitPosition( string blockName, int playerIndex )
    {
        GameObject block = this.blockDictionary[playerIndex][blockName];

        block.transform.position = blockSubmitPointArray[playerIndex].transform.position;
        block.transform.eulerAngles = new Vector3(-89.95f, block.GetComponent<Block>().correctPosition, 0);
        // block.GetComponent<Rigidbody>().isKinematic = true;
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
