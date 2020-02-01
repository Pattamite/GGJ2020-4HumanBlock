using System;
using UnityEngine;

public class PlayerInteractableRegion : MonoBehaviour {
    public event Action<Collider> triggerEnter;
    public event Action<Collider> triggerExit;

    private void OnTriggerEnter (Collider other) {
        if (triggerEnter != null) {
            triggerEnter (other);
        }
    }

    private void OnTriggerExit (Collider other) {
        if (triggerExit != null) {
            triggerExit (other);
        }
    }
}