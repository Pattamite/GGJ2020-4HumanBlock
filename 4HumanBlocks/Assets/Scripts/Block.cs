using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    //  Mock
    [ SerializeField ] float rotateSpeed = 2;

    [ SerializeField ] float correctPosition = 0;

    float rotateInput;


    Vector3 blockRotation;

    // Start is called before the first frame update
    void Start()
    {
        
        blockRotation = transform.rotation.eulerAngles;

    }

    // Update is called once per frame
    void Update()
    {
        rotateInput = Input.GetAxis( "Horizontal" );
        
        transform.Rotate( 0.0f, rotateInput * rotateSpeed, 0.0f, Space.World );
    }

    public bool isCorrectOrientation( float threshold = 5 )
    {



        Debug.Log( "transform y - " + transform.rotation.eulerAngles.y );

        float currentTransformY =  transform.rotation.eulerAngles.y;

        bool isOriented = correctPosition-threshold < currentTransformY &&
                            currentTransformY < correctPosition+threshold;

        return isOriented;
    }

    public string getCredential()
    {

        //mock
        return "B01";
    }

}
