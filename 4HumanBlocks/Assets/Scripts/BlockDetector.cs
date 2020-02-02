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
        if (blockInDetector.Count >= 4)
        {
            if (checkCombineBlock(expectedBlockName))
            {
                sfxPlayer.PlaySfxClip(SfxItem.Block_EnterCorrect);
                if (OnRepairsuccess != null)
                {
                    OnRepairsuccess();
                    this.RepairSuccess();
                }
            }
        }
    }

    public void SetExpectedBlockName( string blockName )
    {
        expectedBlockName = blockName;
    }

    void OnTriggerEnter( Collider other )
    {
        Block otherBlock = other.gameObject.GetComponent< Block >();

        Debug.Log( "Count - " + blockInDetector.Count );

        if (otherBlock == null)
            return;
        
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
                    this.RepairSuccess();
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
            print(block.getBlockName());
            if ( block.getBlockName() != requiredBlockId || (!block.isCorrectOrientation()) )
                return false;
        }
        
        return true;

    }

    void RepairSuccess()
    {
        foreach (Block block in blockInDetector)
        {
            Destroy(block.gameObject);
        }

        blockInDetector.Clear();
    }

}
