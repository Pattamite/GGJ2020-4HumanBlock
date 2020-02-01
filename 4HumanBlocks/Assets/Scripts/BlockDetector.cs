using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockDetector : MonoBehaviour
{

    [ SerializeField ] float errorThreshold = 10;

    private Collider detectorCollider;

    private List< Block > blockInDetector;

    // Start is called before the first frame update
    void Start()
    {

        blockInDetector = new List< Block>();
        
        detectorCollider = GetComponent< Collider >();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter( Collider other )
    {
        Block otherBlock = other.gameObject.GetComponent< Block >();

        if (otherBlock != null )
            blockInDetector.Add( otherBlock );

        Debug.Log( "blockInDector size " + blockInDetector.Count );
        Debug.Log( otherBlock );

        foreach ( Block block in blockInDetector )
        {

            Debug.Log( "    Block " + block + " is " + block.isCorrectOrientation( errorThreshold ) );

        }

        if ( blockInDetector.Count >= 4 )

            if ( checkCombineBlock( "B01" ))
                Debug.Log( "SUCCESS!!!");
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
            if ( block.getCredential() != requiredBlockId )
                return false;
        }
        
        return true;

    }
}
