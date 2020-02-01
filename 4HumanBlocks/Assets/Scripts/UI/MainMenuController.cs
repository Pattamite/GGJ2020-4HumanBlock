using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    public MusicPlayer musicPlayer;

    // Start is called before the first frame update
    void Start()
    {
        musicPlayer.PlayMusic(MusicItem.MainMenu);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
