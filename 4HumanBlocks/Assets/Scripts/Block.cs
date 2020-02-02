using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : InteractableItem
{
    //  Mock
    [ SerializeField ] float rotateSpeed = 2;
    [SerializeField] public float defaultXRotation = -89.98f;
    [ SerializeField ] public float correctPosition = -89.98f;

    float rotateInput;

    Vector3 blockRotation;

    public string blockName;

    public bool isPickable = true;

    // Start is called before the first frame update
    void Start()
    {
        
        blockRotation = transform.rotation.eulerAngles;

    }

    // Update is called once per frame
    void Update()
    {

    }

    public bool isCorrectOrientation( float threshold = 10 )
    {
        float currentTransformY =  transform.rotation.eulerAngles.y;

        float deltaAngle = Mathf.Abs(correctPosition - currentTransformY) % 360;

        if (deltaAngle > 180)
            deltaAngle = 360 - deltaAngle;

        Debug.Log(gameObject + " transform y - " + transform.rotation.eulerAngles.y + " delta - " + deltaAngle);

        bool isOriented = deltaAngle < threshold;

        return isOriented;
    }

    public string getBlockName()
    {

        //mock
        return blockName;
    }

}
