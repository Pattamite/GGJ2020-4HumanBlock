using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockDetector : MonoBehaviour
{

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

        if (otherBlock != null )
            blockInDetector.Add( otherBlock );

        if ( blockInDetector.Count >= 4 )
        {
            if ( checkCombineBlock( expectedBlockName ) )
            {
                if(OnRepairsuccess != null)
                {
                    OnRepairsuccess();
                }
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
