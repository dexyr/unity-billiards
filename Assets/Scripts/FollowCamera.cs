using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour {
    [SerializeField] GameObject target;
    Camera camera;

    void Awake() {
        camera = gameObject.GetComponent<Camera>();

        if (!camera || !target)
            enabled = false;
    }

    void Update() {
        transform.LookAt(target.transform);
    }
}
