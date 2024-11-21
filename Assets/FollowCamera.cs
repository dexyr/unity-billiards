using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour {
    [SerializeField] GameObject target;
    Camera camera;
    void Awake() {
        camera = gameObject.GetComponent<Camera>();

        if (camera == null)
            enabled = false;
    }
    // Start is called before the first frame update
    void Start() {
    }

    // Update is called once per frame
    void Update() {
    }
}
