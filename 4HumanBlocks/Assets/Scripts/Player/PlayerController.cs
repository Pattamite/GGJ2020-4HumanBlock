using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    // Start is called before the first frame update
    public float movementSpeed = 5.0f;
    public float rotationSpeed = 10.0f;
    public Vector3 eyePositionOffset;
    public Vector3 handPositionOffset;
    public GameObject body;

    private PlayerInteractableRegion roi;
    private GameObject selectedItem = null;
    private Transform previousParent = null;
    private HashSet<GameObject> collidedItems = new HashSet<GameObject> ();
    private bool isActive = false;

    public int id = -1;
    public bool isAllowAction = false;

    void Start () {
        roi = gameObject.GetComponentInChildren<PlayerInteractableRegion> ();
        roi.triggerEnter += onCollisionEnter;
        roi.triggerExit += onCollisionExit;
    }

    void OnDisable () {
        roi.triggerEnter -= onCollisionEnter;
        roi.triggerExit -= onCollisionExit;
    }

    void Update () {
        // Position
        Vector3 velocity = new Vector3 (Input.GetAxis ("Horizontal") * Time.deltaTime * movementSpeed, 0, Input.GetAxis ("Vertical") * Time.deltaTime * movementSpeed);
        transform.Translate (velocity, Space.World);
        // Angle
        Vector3 targetDirection = velocity.normalized;
        Vector3 newDirection = Vector3.RotateTowards (transform.forward, targetDirection, rotationSpeed * Time.deltaTime, 0.0f);
        transform.rotation = Quaternion.LookRotation (newDirection);

        if (Input.GetKeyDown ("x")) {
            isActive = !isActive;
        }

        if (isActive) {
            updateSelectedItem ();
            OnPickUpItem ();
        } else {
            OnDropItem ();
        }

        // if (Input.GetKeyUp ("x")) {
        //     OnDropItem ();
        // }
    }

    void OnPickUpItem () {
        if (selectedItem != null) {
            OnSetPickUpItemPropertyEnter ();
            selectedItem.transform.position = body.transform.position + 2 * body.transform.forward + handPositionOffset;
            selectedItem.transform.SetParent (transform);
        }
    }

    void OnDropItem () {
        if (selectedItem != null) {
            OnSetPickUpItemPropertyExit ();
        }
    }

    void OnSetPickUpItemPropertyEnter () {
        selectedItem.GetComponent<Rigidbody> ().useGravity = false;
        selectedItem.GetComponent<Collider> ().enabled = false;
        try {

            previousParent = selectedItem.transform.parent.transform;
        } catch {
            previousParent = null;
        }
    }

    void OnSetPickUpItemPropertyExit () {
        selectedItem.GetComponent<Rigidbody> ().useGravity = true;
        selectedItem.GetComponent<Collider> ().enabled = true;
        selectedItem.transform.SetParent (previousParent);
        previousParent = null;
    }

    bool isOccluded (GameObject g, Vector3 directCast) {
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
            // Debug.Log (hitInfo.collider.gameObject);
            return (hitInfo.collider.gameObject.GetComponent<StaticItem> () != null);
        }
        return false;
    }

    void updateSelectedItem () {
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
            OnSetPickUpItemPropertyExit ();
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
}