using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallCollision : MonoBehaviour {
    SphereCollider sphereCollider;
    void Awake() {
        sphereCollider = gameObject.GetComponent<SphereCollider>();

        if (!sphereCollider)
            enabled = false;
    }

    void OnCollisionEnter(Collision collision) {
        Debug.Log(collision.ToString());
    }
}
