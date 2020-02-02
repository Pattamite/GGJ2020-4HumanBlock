using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockDetector : MonoBehaviour
{
    public SfxPlayer sfxPlayer;
    public delegate void RepairSuccessAction();
    public static event RepairSuccessAction OnRepairsuccess;

    [ SerializeField ] float errorThreshold = 10;

    private Collider        detectorCollider;
    private string          expectedBlockName;
    private List< Block >   blockInDetector;

    // Start is called before the first frame update
    void Start()
    {

        blockInDetector = new List< Block >();
        
        detectorCollider = GetComponent< Collider >();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetExpectedBlockName( string blockName )
    {
        expectedBlockName = blockName;
    }

    void OnTriggerEnter( Collider other )
    {
        Block otherBlock = other.gameObject.GetComponent< Block >();

        Debug.Log( "Count - " + blockInDetector.Count );

        if (otherBlock != null )
            blockInDetector.Add( otherBlock );

        if( blockInDetector.Count == 1)
        {
            sfxPlayer.PlaySfxClip(SfxItem.Block_Enter1);
        }
        else if (blockInDetector.Count == 2)
        {
            sfxPlayer.PlaySfxClip(SfxItem.Block_Enter2);
        }
        else if (blockInDetector.Count == 3)
        {
            sfxPlayer.PlaySfxClip(SfxItem.Block_Enter3);
        }
        else if ( blockInDetector.Count >= 4 )
        {
            if ( checkCombineBlock( expectedBlockName ) )
            {
                sfxPlayer.PlaySfxClip(SfxItem.Block_EnterCorrect);
                if (OnRepairsuccess != null)
                {
                    OnRepairsuccess();
                }
            }
            else
            {
                sfxPlayer.PlaySfxClip(SfxItem.Block_EnterIncorrect);
            }
        }
    }

    void OnTriggerExit( Collider other )
    {

        Block otherBlock = other.gameObject.GetComponent< Block >();

        blockInDetector.Remove( otherBlock );

    }

    bool checkCombineBlock( string requiredBlockId )
    {

        foreach( Block block in blockInDetector )
        {
            if ( block.getBlockName() != requiredBlockId )
                return false;
        }
        
        return true;

    }

}
