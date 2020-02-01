using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUiManager : MonoBehaviour
{
    public Text timerText;
    public Image blockCommandImage;

    public GameObject WaitingPlayerBoard;
    public GameObject GameStartBoard;
    public GameObject GameEndBoard;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateTimterText(float time)
    {
        timerText.text = time.ToString("F2");
    }

    public void UpdateBlockCommandImage(Sprite sprite)
    {
        blockCommandImage.sprite = sprite;
    }

    public void UpdateGameStatusBoard(GameState gameState)
    {
        switch(gameState)
        {
            case GameState.WaitingForPlayer:
                WaitingPlayerBoard.SetActive(true);
                GameStartBoard.SetActive(false);
                GameEndBoard.SetActive(false);
                break;
            case GameState.GameStart:
                WaitingPlayerBoard.SetActive(false);
                GameStartBoard.SetActive(true);
                GameEndBoard.SetActive(false);
                break;
            case GameState.GameFinish:
                WaitingPlayerBoard.SetActive(false);
                GameStartBoard.SetActive(false);
                GameEndBoard.SetActive(true);
                break;
            default:
                WaitingPlayerBoard.SetActive(false);
                GameStartBoard.SetActive(false);
                GameEndBoard.SetActive(false);
                break;
        }
    }
}
