using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SfxItem
{
    Player_PickUp,
    Player_Drop,
    Item_Enter1,
    Item_Enter2,
    Item_Enter3,
    Item_EnterIncorrect,
    Item_EnterCorrect,
    Game_Start,
    Game_TimeOut
}

public class SfxPlayer : MonoBehaviour
{
    public AudioClip playerPickupClip;
    public AudioClip playerDropClip;
    public AudioClip itemEnter1Clip;
    public AudioClip itemEnter2Clip;
    public AudioClip itemEnter3Clip;
    public AudioClip itemEnterIncorrectClip;
    public AudioClip itemEnterCorrectClip;
    public AudioClip gameStartClip;
    public AudioClip gameTimeOutClip;

    private AudioSource audioSource;


    // Start is called before the first frame update
    void Start()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlaySfxClip(SfxItem sfxItem)
    {
        AudioClip audioClip = this.getAudioClip(sfxItem);

        if (!audioClip)
            return;

        audioSource.Stop();
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
            case SfxItem.Item_Enter1:
                return this.itemEnter1Clip;
            case SfxItem.Item_Enter2:
                return this.itemEnter2Clip;
            case SfxItem.Item_Enter3:
                return this.itemEnter3Clip;
            case SfxItem.Item_EnterIncorrect:
                return this.itemEnterIncorrectClip;
            case SfxItem.Item_EnterCorrect:
                return this.itemEnterCorrectClip;
            case SfxItem.Game_Start:
                return this.gameStartClip;
            case SfxItem.Game_TimeOut:
                return this.gameTimeOutClip;
            default:
                return null;
        }
    }
}
