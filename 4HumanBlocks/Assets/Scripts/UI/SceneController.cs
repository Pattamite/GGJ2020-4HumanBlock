using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public static int mainMenuSceneNumber = 0;
    public static int gameSceneNumber = 1;
    public static int highScoreSceneNumber = 2;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadMenuScene()
    {
        SceneManager.LoadScene(mainMenuSceneNumber);
    }

    public void LoadGameScene()
    {
        SceneManager.LoadScene(gameSceneNumber);
    }

    public void LoadHighScoreScene()
    {
        SceneManager.LoadScene(highScoreSceneNumber);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
