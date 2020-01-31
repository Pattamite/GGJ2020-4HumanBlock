using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestInput : MonoBehaviour
{
    static int maxId = 0;

    int id;

    Vector2 movement;
    float rotate;
    Vector3 baseRotateSpeed;

    // Start is called before the first frame update
    void Start()
    {
        this.id = maxId++;
        this.baseRotateSpeed = new Vector3(0, 90, 0);
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.Translate(movement * Time.deltaTime, Space.World);
        this.transform.Rotate(rotate * Time.deltaTime * this.baseRotateSpeed);
    }

    void OnMovement(InputValue inputValue)
    {
        this.movement = inputValue.Get<Vector2>();
    }

    void OnPickupDropItem()
    {
        Debug.Log(this.id + " pickup / drop item.");
    }

    void OnRotateItem(InputValue inputValue)
    {
        this.rotate = inputValue.Get<float>();
    }
}
