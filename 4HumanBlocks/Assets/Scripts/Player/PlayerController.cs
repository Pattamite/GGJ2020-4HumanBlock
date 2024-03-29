﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour {
    // Start is called before the first frame update
    public float movementSpeed = 5.0f;
    public float rotationSpeed = 10.0f;
    public float itemRotationSpeed = 90.0f;
    public Vector3 eyePositionOffset;
    public Vector3 handPositionOffset;
    public GameObject[] bodyList;
    private GameObject body;
    public int id = -1;
    public bool isAllowAction = false;
    public bool isAllowStart = false;
    public SfxPlayer sfxPlayer;

    private PlayerInteractableRegion roi;
    private GameObject selectedItem = null;
    private Transform previousParent = null;
    private HashSet<GameObject> collidedItems = new HashSet<GameObject> ();
    private bool isActive = false;

    private Animator animator;
    CharacterController characterController;

    private Vector2 movementInput;
    private float rotateObjectInput;

    void Start () {
        // Set Region References and Callbacks
        roi = gameObject.GetComponentInChildren<PlayerInteractableRegion> ();
        roi.triggerEnter += onCollisionEnter;
        roi.triggerExit += onCollisionExit;

        characterController = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
        PlayerSpawner.ReportPlayerSpawn(gameObject, this);
        print(transform.position);
    }

    void OnDisable () {
        // Unsubscribe Callbacks, avoid MemLeak
        roi.triggerEnter -= onCollisionEnter;
        roi.triggerExit -= onCollisionExit;
    }

    void Update () {
        // Absolute Position Update
        Vector3 velocity = new Vector3 (movementInput.x, 0, movementInput.y);

        velocity.Normalize();

        // Angle Faced Update
        Vector3 targetDirection = velocity.normalized;
        Vector3 newDirection = Vector3.RotateTowards (transform.forward, targetDirection, rotationSpeed * Time.deltaTime, 0.0f);
        transform.rotation = Quaternion.LookRotation (newDirection);

        //  set animation
        if ( velocity !=  Vector3.zero )
            animator.SetBool("isWalk", true );
        else
            animator.SetBool("isWalk", false);

        //  Add gravity and move character
        velocity.y -= 20.0f;
        velocity *= Time.deltaTime * movementSpeed;
        characterController.Move( velocity );
       
        if ( selectedItem != null && isActive )
        {
            selectedItem.transform.Rotate(0 , rotateObjectInput * itemRotationSpeed * Time.deltaTime, 0, Space.World);
        }
        else if(selectedItem == null && isActive)
        {
            OnDropItem();
        }

    }

    void OnPickUpItem () {
        // Setup selectedItem before pickup
        if (selectedItem != null) {
            if (!selectedItem.GetComponent<Block>().isPickable)
                return;

            OnSetPickUpItemPropertyEnter ();
            selectedItem.transform.position = body.transform.position + 2 * body.transform.forward + handPositionOffset;
            // Aligned selectedItem face
            // (-89.98, <getY>, 0)
            Vector3 targetDirection = new Vector3 (selectedItem.GetComponent<Block>().defaultXRotation, 
                                                        selectedItem.transform.eulerAngles.y, 0);
            Vector3 newDirection = Vector3.RotateTowards (selectedItem.transform.forward, targetDirection, rotationSpeed, 0.0f);

            selectedItem.transform.eulerAngles = targetDirection;
            selectedItem.transform.SetParent (transform);

            animator.SetTrigger( "carryTrigger" );
            this.sfxPlayer.PlaySfxClip(SfxItem.Player_PickUp);

            isActive = true;
        }
    }

    void OnDropItem () {
        // Restore selectedItem after pickup
        if (selectedItem != null) {
            OnSetPickUpItemPropertyExit ();
            this.sfxPlayer.PlaySfxClip(SfxItem.Player_Drop);
        }
        animator.SetTrigger("dropItemTrigger");
        isActive = false;
    }

    void OnSetPickUpItemPropertyEnter () {
        // Misc, avoid code duplication
        selectedItem.GetComponent<Rigidbody> ().useGravity = false;
        // selectedItem.GetComponent<Collider> ().enabled = false;
        selectedItem.GetComponent<Rigidbody> ().isKinematic = true;

    }

    void OnSetPickUpItemPropertyExit () {
        // Misc, avoid code duplication
        selectedItem.GetComponent<Rigidbody> ().useGravity = true;
        // selectedItem.GetComponent<Collider> ().enabled = true;
        selectedItem.GetComponent<Rigidbody> ().isKinematic = false;
        selectedItem.transform.SetParent( transform.parent );
        // selectedItem.transform.SetParent (previousParent);
        // previousParent = null;
    }

    bool isOccluded (GameObject g, Vector3 directCast) {
        // Check if gameObject is occluded by somthing or not

        // Debug.DrawLine (transform.position, transform.position + transform.forward * 100f, Color.green);
        // Ray ray = new Ray (transform.position, transform.forward);
        // RaycastHit hitInfo;
        // if (Physics.Raycast (ray, out hitInfo, 100f)) {
        //     Debug.DrawLine (ray.origin, hitInfo.point, Color.red);
        //     Debug.Log ("Hit");
        // } else {
        //     Debug.DrawLine (ray.origin, ray.origin + ray.direction * 100f, Color.green);
        //     Debug.Log ("Missed");
        // }

        // Raycast from cone tip to gameObject
        Ray ray = new Ray (transform.position + eyePositionOffset, -1 * directCast.normalized);
        // Debug.DrawLine (ray.origin, ray.origin + ray.direction * directCast.magnitude, Color.green);
        RaycastHit hitInfo;
        if (Physics.Raycast (ray, out hitInfo, directCast.magnitude )) {
            Debug.Log ( "" + hitInfo.collider.gameObject + " --- " + g);
            // return (hitInfo.collider.gameObject.GetComponent<StaticItem> () != null);
            return (hitInfo.collider.gameObject != g);
        }
        return false;
    }

    void updateSelectedItem () {
        // Set nearest non-occluded game object within roi region
        GameObject nearest = null;
        float minDist = 0;
        List<GameObject> objList = new List<GameObject>();
        foreach (GameObject g in collidedItems) {
            if (!g)
            {
                objList.Add(g);
                continue;
            }

            Vector3 directCast = (transform.position + eyePositionOffset - g.transform.position);
            float dist = directCast.magnitude;
            if (nearest == null || minDist < dist) {
                if (!isOccluded (g, directCast)) {
                    nearest = g;
                    if (nearest == null) minDist = dist;
                }
            }
        }

        foreach (GameObject g in objList)
        {
            collidedItems.Remove(g);
        }
        selectedItem = nearest;
    }

    void onCollisionEnter (Collider other) {
        if (other.gameObject.GetComponent<InteractableItem> ()) {
            collidedItems.Add (other.gameObject);
        }
    }

    void onCollisionExit (Collider other) {
        collidedItems.Remove (other.gameObject);
        updateSelectedItem ();
    }

    void OnDrawGizmosSelected()
    {

        //Gizmos.DrawSphere( body.transform.position + 2 * body.transform.forward + handPositionOffset, 0.1f);
    }

    void OnMovement(InputValue inputValue)
    {
        if (!isAllowAction)
            return;

        this.movementInput = inputValue.Get<Vector2>();
    }

    void OnPickupDropItem()
    {
        if (!isAllowAction)
            return;

        Debug.Log(this.id + " pickup / drop item." + !isActive);

        if (!isActive)
        {
            // Pickup Item
            updateSelectedItem();
            OnPickUpItem();
        }
        else
        {
            OnDropItem();
        }
    }

    void OnRotateItem(InputValue inputValue)
    {
        if (!isAllowAction)
            return;

        this.rotateObjectInput = inputValue.Get<float>();

        if ( this.rotateObjectInput != 0 )
            animator.SetBool( "isRotatingItem", true );
        else   
            animator.SetBool( "isRotatingItem", false );
    }

    void OnStartGame()
    {
        if (!isAllowStart)
            return;

        Debug.Log(this.id + " start game.");
        PlayerSpawner.StartGame();
    }

    public void SetPosition(Vector3 position)
    {
        this.characterController.enabled = false;
        transform.position = position;
        this.characterController.enabled = true;
    }

    public void setId(int id)
    {
        body = bodyList[id];
        for(int i = 0; i < bodyList.Length; i++)
        {
            if(i != id)
            {
                Destroy(bodyList[i]);
            }
        }

        animator = body.GetComponent<Animator>();
        this.id = id;
        this.isAllowStart = true;
    }
}