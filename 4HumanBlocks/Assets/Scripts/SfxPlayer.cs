using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SfxItem
{
    Player_PickUp,
    Player_Drop,
    Block_Enter1,
    Block_Enter2,
    Block_Enter3,
    Block_EnterIncorrect,
    Block_EnterCorrect,
    Game_Start,
    Game_TimeOut
}

public class SfxPlayer : MonoBehaviour
{
    public AudioClip playerPickupClip;
    public AudioClip playerDropClip;
    public AudioClip blockEnter1Clip;
    public AudioClip blockEnter2Clip;
    public AudioClip blockEnter3Clip;
    public AudioClip blockEnterIncorrectClip;
    public AudioClip blockEnterCorrectClip;
    public AudioClip gameStartClip;
    public AudioClip gameTimeOutClip;

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

    public void PlaySfxClip(SfxItem sfxItem)
    {
        audioSource.Stop();

        AudioClip audioClip = this.getAudioClip(sfxItem);

        if (!audioClip)
            return;
    
        audioSource.clip = audioClip;
        audioSource.Play();
    }

    private AudioClip getAudioClip(SfxItem sfxItem)
    {
        switch(sfxItem)
        {
            case SfxItem.Player_PickUp:
                return this.playerPickupClip;
            case SfxItem.Player_Drop:
                return this.playerDropClip;
            case SfxItem.Block_Enter1:
                return this.blockEnter1Clip;
            case SfxItem.Block_Enter2:
                return this.blockEnter2Clip;
            case SfxItem.Block_Enter3:
                return this.blockEnter3Clip;
            case SfxItem.Block_EnterIncorrect:
                return this.blockEnterIncorrectClip;
            case SfxItem.Block_EnterCorrect:
                return this.blockEnterCorrectClip;
            case SfxItem.Game_Start:
                return this.gameStartClip;
            case SfxItem.Game_TimeOut:
                return this.gameTimeOutClip;
            default:
                return null;
        }
    }
}
