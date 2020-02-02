using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MusicItem
{
    MainMenu,
    Game_Waiting,
    Game_Playing,
    Game_Result,
}

public class MusicPlayer : MonoBehaviour
{
    public AudioClip mainMenuMusic;
    public AudioClip GameWaitingMusic;
    public AudioClip GamePlayingMusic;
    public AudioClip GameResultMusic;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayMusic(MusicItem musicItem)
    {
        audioSource.Stop();

        AudioClip audioClip = this.getAudioClip(musicItem);

        if (!audioClip)
            return;

        audioSource.clip = audioClip;
        audioSource.Play();
    }

    private AudioClip getAudioClip(MusicItem musicItem)
    {
        switch (musicItem)
        {
            case MusicItem.MainMenu:
                return this.mainMenuMusic;
            case MusicItem.Game_Waiting:
                return this.GameWaitingMusic;
            case MusicItem.Game_Playing:
                return this.GamePlayingMusic;
            case MusicItem.Game_Result:
                return this.GameResultMusic;
            default:
                return null;
        }
    }
}
