using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : InteractableItem
{
    //  Mock
    [ SerializeField ] float rotateSpeed = 2;

    [ SerializeField ] float correctPosition = 0;

    float rotateInput;

    Vector3 blockRotation;

    private string blockName;

    // Start is called before the first frame update
    void Start()
    {
        
        blockRotation = transform.rotation.eulerAngles;

    }

    // Update is called once per frame
    void Update()
    {

    }

    public bool isCorrectOrientation( float threshold = 5 )
    {

        // Debug.Log( "transform y - " + transform.rotation.eulerAngles.y );

        float currentTransformY =  transform.rotation.eulerAngles.y;

        bool isOriented = correctPosition-threshold < currentTransformY &&
                            currentTransformY < correctPosition+threshold;

        return isOriented;
    }

    public string getBlockName()
    {

        //mock
        return blockName;
    }

}
