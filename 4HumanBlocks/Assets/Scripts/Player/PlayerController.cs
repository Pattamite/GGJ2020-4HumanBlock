using System.Collections;
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
    public GameObject body;
    public int id = -1;
    public bool isAllowAction = false;
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
        print("Update Before:" + transform.position);
        // Absolute Position Update
        Vector3 velocity = new Vector3 (movementInput.x, 0, movementInput.y);

        velocity.Normalize();

        velocity.y -= 20.0f;

        velocity *= Time.deltaTime * movementSpeed;

        // transform.Translate (velocity, Space.World);
        characterController.Move( velocity );
        print("Update After:" + transform.position);
        velocity.y = 0.0f;
        // Angle Faced Update
        Vector3 targetDirection = velocity.normalized;
        Vector3 newDirection = Vector3.RotateTowards (transform.forward, targetDirection, rotationSpeed * Time.deltaTime, 0.0f);
        transform.rotation = Quaternion.LookRotation (newDirection);

        if ( velocity !=  Vector3.zero )
            animator.SetBool("isWalk", true );
        else
            animator.SetBool("isWalk", false);

        if ( selectedItem != null && isActive )
        {
            selectedItem.transform.Rotate(0, rotateObjectInput * itemRotationSpeed * Time.deltaTime, 0);
        }

    }

    void OnPickUpItem () {
        // Setup selectedItem before pickup
        if (selectedItem != null) {
            OnSetPickUpItemPropertyEnter ();
            selectedItem.transform.position = body.transform.position + 2 * body.transform.forward + handPositionOffset;
            // Aligned selectedItem face
            // (-89.98, <getY>, 0)
            Vector3 targetDirection = new Vector3 (-89.98f, selectedItem.transform.rotation.y, 0);
            Vector3 newDirection = Vector3.RotateTowards (selectedItem.transform.forward, targetDirection, rotationSpeed, 0.0f);

            selectedItem.transform.rotation = Quaternion.LookRotation (newDirection.normalized);
            selectedItem.transform.SetParent (transform);
        }
    }

    void OnDropItem () {
        // Restore selectedItem after pickup
        if (selectedItem != null) {
            OnSetPickUpItemPropertyExit ();
        }
    }

    void OnSetPickUpItemPropertyEnter () {
        // Misc, avoid code duplication
        selectedItem.GetComponent<Rigidbody> ().useGravity = false;
        selectedItem.GetComponent<Collider> ().enabled = false;
        selectedItem.GetComponent<Rigidbody> ().isKinematic = true;
        try {
            previousParent = selectedItem.transform.parent.transform;
        } catch {
            previousParent = null;
        }
    }

    void OnSetPickUpItemPropertyExit () {
        // Misc, avoid code duplication
        selectedItem.GetComponent<Rigidbody> ().useGravity = true;
        selectedItem.GetComponent<Collider> ().enabled = true;
        selectedItem.GetComponent<Rigidbody> ().isKinematic = false;
        selectedItem.transform.SetParent (previousParent);
        previousParent = null;
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
        if (Physics.Raycast (ray, out hitInfo, directCast.magnitude, 9)) {
            Debug.Log ( "" + hitInfo.collider.gameObject + " : " + g);
            // return (hitInfo.collider.gameObject.GetComponent<StaticItem> () != null);
            Debug.Log( hitInfo.collider.gameObject != g );
            return (hitInfo.collider.gameObject != g);
        }
        return false;
    }

    void updateSelectedItem () {
        // Set nearest non-occluded game object within roi region
        GameObject nearest = null;
        float minDist = 0;
        foreach (GameObject g in collidedItems) {
            Vector3 directCast = (transform.position + eyePositionOffset - g.transform.position);
            float dist = directCast.magnitude;
            if (nearest == null || minDist < dist) {
                if (!isOccluded (g, directCast)) {
                    nearest = g;
                    if (nearest == null) minDist = dist;
                }
            }
        }
        if (nearest != selectedItem && selectedItem != null) {
            // OnSetPickUpItemPropertyExit ();
            Debug.Log( "OOps");
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

        Gizmos.DrawSphere( body.transform.position + 2 * body.transform.forward + handPositionOffset, 0.1f);
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

        Debug.Log(this.id + " pickup / drop item.");

        isActive = !isActive;

        if (isActive)
        {
            // Pickup Item
            updateSelectedItem();
            OnPickUpItem();
            this.sfxPlayer.PlaySfxClip(SfxItem.Player_PickUp);
        }
        else
        {
            OnDropItem();
            this.sfxPlayer.PlaySfxClip(SfxItem.Player_Drop);
        }
    }

    void OnRotateItem(InputValue inputValue)
    {
        if (!isAllowAction)
            return;

        this.rotateObjectInput = inputValue.Get<float>();
    }

    void OnStartGame()
    {
        Debug.Log(this.id + " start game.");
        PlayerSpawner.StartGame();
    }

    public void SetPosition(Vector3 position)
    {
        this.characterController.enabled = false;
        transform.position = position;
        this.characterController.enabled = true;
    }
}