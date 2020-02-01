using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConeView : MonoBehaviour {
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