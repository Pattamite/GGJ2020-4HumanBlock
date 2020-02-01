using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
    public float movementSpeed = 5.0f;
    public float rotationSpeed = 200.0f;
    public GameObject mainPlayer;
    public Vector3 conePositionOffset;

    private ConeView coneTrigger;
    private GameObject selectedItem = null;
    private HashSet<GameObject> collidedItems = new HashSet<GameObject> ();
    private bool isSelecting = false;

    void Start () {
        coneTrigger = gameObject.GetComponentInChildren (typeof (ConeView)) as ConeView;
        coneTrigger.triggerEnter += onCollisionEnter;
        coneTrigger.triggerExit += onCollisionExit;
    }

    void OnDestroy () {
        coneTrigger.triggerEnter -= onCollisionEnter;
        coneTrigger.triggerExit -= onCollisionExit;
    }

    void Update () {
        transform.Rotate (0, Input.GetAxis ("Horizontal") * Time.deltaTime * rotationSpeed, 0);
        transform.Translate (0, 0, Input.GetAxis ("Vertical") * Time.deltaTime * movementSpeed);

        if (Input.GetKey ("x")) {
            isSelecting = true;
            updateSelectedItem ();
            OnPickUpItem ();
        }

        if (isSelecting && Input.GetKeyUp ("x")) {
            isSelecting = false;
            OnDropItem ();
        }
    }

    void OnPickUpItem () {
        if (selectedItem != null) {
            selectedItem.GetComponent<Rigidbody> ().useGravity = false;
            selectedItem.GetComponent<Collider> ().enabled = false;
            selectedItem.transform.position = mainPlayer.transform.position + 2 * mainPlayer.transform.forward;
        }
    }

    void OnDropItem () {
        if (selectedItem != null) {
            selectedItem.GetComponent<Rigidbody> ().useGravity = true;
            selectedItem.GetComponent<Collider> ().enabled = true;
            selectedItem.transform.position = mainPlayer.transform.position + 2 * mainPlayer.transform.forward;
            selectedItem = null;
        }
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
        Ray ray = new Ray (mainPlayer.transform.position + conePositionOffset, -1 * directCast.normalized);
        // Debug.DrawLine (ray.origin, ray.origin + ray.direction * directCast.magnitude, Color.green);
        RaycastHit hitInfo;
        if (Physics.Raycast (ray, out hitInfo, directCast.magnitude, 9)) {
            return (hitInfo.collider.gameObject.GetComponent<StaticItem> () != null);
        }
        return false;
    }

    void updateSelectedItem () {
        GameObject nearest = null;
        float minDist = 0;
        foreach (GameObject g in collidedItems) {
            Vector3 directCast = (mainPlayer.transform.position + conePositionOffset - g.transform.position);
            float dist = directCast.magnitude;
            if (nearest == null || minDist < dist) {
                if (!isOccluded (g, directCast)) {
                    nearest = g;
                    if (nearest == null) minDist = dist;
                }
            }
        }
        if (nearest != selectedItem && selectedItem != null) {
            selectedItem.GetComponent<Rigidbody> ().useGravity = true;
            selectedItem.GetComponent<Collider> ().enabled = true;
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