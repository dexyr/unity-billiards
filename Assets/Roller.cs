using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Roller : MonoBehaviour {
    [SerializeField] float speed;
    Rigidbody rigidbody;

    void Awake() {
        rigidbody = GetComponent<Rigidbody>();

        if (rigidbody == null)
            enabled = false;
    }
    void Update() {
        float x = 0, z = 0;

        if (Input.GetKey(KeyCode.W))
            z++;
        if (Input.GetKey(KeyCode.S))
            z--;
        if (Input.GetKey(KeyCode.D))
            x++;
        if (Input.GetKey(KeyCode.A))
            x--;

        rigidbody.velocity = new Vector3(x * speed, 0, z * speed);
    }
}
