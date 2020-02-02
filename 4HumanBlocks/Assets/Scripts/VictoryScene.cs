using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictoryScene : MonoBehaviour {
    public Transform finishBanner;
    public Transform trumpets;
    public Transform blurBackdrop;

    public float speedRotate = 20f;
    public float trumpetSpeed = 1.3f;
    public Vector3 trumpetStartAngle = new Vector3 (0, 0, 10f);
    public Vector3 trumpetEndAngle = new Vector3 (0, 0, -10f);

    // Update is called once per frame
    void Update () {
        blurBackdrop.Rotate (Vector3.forward * speedRotate * Time.deltaTime);
        float t = Mathf.PingPong (Time.time * trumpetSpeed * 2.0f, 1.0f);
        trumpets.eulerAngles = Vector3.Lerp (trumpetStartAngle, trumpetEndAngle, t);
    }
}