using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Roller : MonoBehaviour {
    [SerializeField] GameObject target;
    [SerializeField] float speed;
    Rigidbody rigidbody;

    void Awake() {
        rigidbody = target.GetComponent<Rigidbody>();

        if (!target || !rigidbody)
            enabled = false;
    }

    void Update() {
        float x = 0, z = 0;

        if (Input.GetKeyDown(KeyCode.W))
            z++;
        if (Input.GetKeyDown(KeyCode.S))
            z--;
        if (Input.GetKeyDown(KeyCode.D))
            x++;
        if (Input.GetKeyDown(KeyCode.A))
            x--;

        rigidbody.AddForce(new Vector3(x * speed, 0, z * speed));
    }
}
